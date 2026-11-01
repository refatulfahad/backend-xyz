namespace Learning.NotificationWorker.Configuration;

public sealed class BrevoOptions
{
    public string KeyApi { get; set; } = string.Empty;

    public string SenderEmail { get; set; } = string.Empty;

    public string SenderName { get; set; } = string.Empty;
}