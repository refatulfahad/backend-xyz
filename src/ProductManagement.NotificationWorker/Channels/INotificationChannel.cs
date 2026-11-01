using Learning.NotificationWorker.Models;

namespace Learning.NotificationWorker.Channels;

public interface INotificationChannel
{
    bool CanHandle(ProductNotificationEvent notification);

    Task SendAsync(
        ProductNotificationEvent notification,
        CancellationToken cancellationToken);
}