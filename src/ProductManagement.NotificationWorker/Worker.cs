using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Learning.NotificationWorker.Channels;
using Learning.NotificationWorker.Models;

namespace Learning.NotificationWorker;

public sealed class Worker(
    ServiceBusClient serviceBusClient,
    IConfiguration configuration,
    IEnumerable<INotificationChannel> channels,
    ILogger<Worker> logger) : BackgroundService
{
    private readonly JsonSerializerOptions _jsonOptions =
        new(JsonSerializerDefaults.Web);

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var queueName = configuration["ServiceBus:QueueName"];

        if (string.IsNullOrWhiteSpace(queueName))
        {
            throw new InvalidOperationException(
                "Service Bus queue name is not configured.");
        }

        await using var processor = serviceBusClient.CreateProcessor(
            queueName,
            new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = 1
            });

        processor.ProcessMessageAsync += ProcessMessageAsync;

        processor.ProcessErrorAsync += args =>
        {
            logger.LogError(
                args.Exception,
                "Service Bus processor error. Entity: {EntityPath}",
                args.EntityPath);

            return Task.CompletedTask;
        };

        logger.LogInformation(
            "Starting Notification Worker for queue {QueueName}",
            queueName);

        await processor.StartProcessingAsync(stoppingToken);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            // Normal application shutdown.
        }
        finally
        {
            await processor.StopProcessingAsync(
                CancellationToken.None);
        }
    }

    private async Task ProcessMessageAsync(
        ProcessMessageEventArgs args)
    {
        var message = args.Message;

        ProductNotificationEvent? notification;

        try
        {
            notification =
                JsonSerializer.Deserialize<ProductNotificationEvent>(
                    message.Body.ToString(),
                    _jsonOptions);

            if (notification is null ||
                string.IsNullOrWhiteSpace(notification.EventId) ||
                string.IsNullOrWhiteSpace(notification.EventType) ||
                string.IsNullOrWhiteSpace(notification.ItemId))
            {
                await args.DeadLetterMessageAsync(
                    message,
                    "InvalidNotificationEvent",
                    "Required event fields are missing.");

                return;
            }
        }
        catch (JsonException ex)
        {
            logger.LogError(
                ex,
                "Invalid JSON in Service Bus message {MessageId}",
                message.MessageId);

            await args.DeadLetterMessageAsync(
                message,
                "InvalidJson",
                "Message body is not valid notification event JSON.");

            return;
        }

        logger.LogInformation(
            "Received notification event. EventId: {EventId}, EventType: {EventType}, ItemId: {ItemId}",
            notification.EventId,
            notification.EventType,
            notification.ItemId);

        try
        {
            var matchingChannels = channels
                .Where(channel => channel.CanHandle(notification))
                .ToArray();

            if (matchingChannels.Length == 0)
            {
                logger.LogWarning(
                    "No notification channel handles event type {EventType}. EventId: {EventId}",
                    notification.EventType,
                    notification.EventId);

                // Avoid repeatedly retrying an event this worker
                // intentionally doesn't support.
                await args.CompleteMessageAsync(message);
                return;
            }

            foreach (var channel in matchingChannels)
            {
                await channel.SendAsync(
                    notification,
                    args.CancellationToken);
            }

            await args.CompleteMessageAsync(message);

            logger.LogInformation(
                "Notification event processed successfully. EventId: {EventId}",
                notification.EventId);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Notification processing failed. EventId: {EventId}, ItemId: {ItemId}. Message will remain uncompleted.",
                notification.EventId,
                notification.ItemId);

            // Do not complete the message. Its lock will eventually
            // expire, allowing Service Bus to redeliver it.
        }
    }
}