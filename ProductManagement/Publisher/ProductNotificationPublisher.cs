using System.Text.Json;
using Azure.Messaging.ServiceBus;
using ProductManagement.Models;

namespace ProductManagement.Publisher;

public sealed class ProductNotificationPublisher(
    ServiceBusClient client,
    IConfiguration configuration)
{
    private readonly string _queueName =
        configuration["ServiceBus:QueueName"]
        ?? throw new InvalidOperationException(
            "Queue name is not configured.");

    public async Task PublishProductNotificationAsync(
        string itemId,
        string eventType,
        string recipientEmail,
        CancellationToken cancellationToken)
    {
        var notification = new ProductNotificationEvent(
            EventId: Guid.NewGuid().ToString(),
            EventType: eventType,
            ItemId: itemId,
            RecipientEmail: recipientEmail);

        var body = JsonSerializer.Serialize(notification);

        var message = new ServiceBusMessage(body)
        {
            MessageId = notification.EventId,
            ContentType = "application/json"
        };

        await using var sender = client.CreateSender(_queueName);

        await sender.SendMessageAsync(
            message,
            cancellationToken);
    }
}