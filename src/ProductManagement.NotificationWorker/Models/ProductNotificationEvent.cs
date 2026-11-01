namespace Learning.NotificationWorker.Models;

public sealed class ProductNotificationEvent
{
    public string EventId { get; init; } = string.Empty;

    public string EventType { get; init; } = string.Empty;

    public string ItemId { get; init; } = string.Empty;

    public string RecipientEmail { get; init; } = string.Empty;
}