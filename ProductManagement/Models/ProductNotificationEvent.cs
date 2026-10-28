namespace ProductManagement.Models;

public sealed record ProductNotificationEvent(
    string EventId,
    string EventType,
    string ItemId,
    string RecipientEmail);