using System.Net;
using Learning.NotificationWorker.Models;
using Learning.NotificationWorker.Services;

namespace Learning.NotificationWorker.Channels;

public sealed class EmailNotificationChannel(
    BrevoEmailSender emailSender,
    ILogger<EmailNotificationChannel> logger)
    : INotificationChannel
{
    public bool CanHandle(ProductNotificationEvent notification)
    {
        return string.Equals(
            notification.EventType,
            "ProductCreated",
            StringComparison.OrdinalIgnoreCase);
    }

    public async Task SendAsync(
        ProductNotificationEvent notification,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(notification.RecipientEmail))
        {
            throw new InvalidOperationException(
                "Recipient email is missing.");
        }

        var subject = $"Item {notification.ItemId} {notification.EventType} successfully";

        var safeItemId = WebUtility.HtmlEncode(notification.ItemId);

        var htmlBody = $"""
            <h2>Item {notification.EventType} Successfully</h2>
            <p>Your item has been {notification.EventType.ToLower()} successfully.</p>
            <p>Item ID: {safeItemId}</p>
            """;

        var textBody = $"""
            Item {notification.EventType} Successfully

            Your item has been {notification.EventType.ToLower()} successfully.
            Item ID: {notification.ItemId}
            """;

        logger.LogInformation(
            "Sending product-created email. EventId: {EventId}, ItemId: {ItemId}",
            notification.EventId,
            notification.ItemId);

        await emailSender.SendAsync(
            notification.RecipientEmail,
            subject,
            htmlBody,
            textBody,
            cancellationToken);
    }
}