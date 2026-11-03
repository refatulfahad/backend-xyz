using Azure.Messaging.ServiceBus;
using Learning.NotificationWorker;
using Learning.NotificationWorker.Channels;
using Learning.NotificationWorker.Configuration;
using Learning.NotificationWorker.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddOptions<BrevoOptions>()
    .Bind(builder.Configuration.GetSection("Brevo"))
    .Validate(
        options =>
            !string.IsNullOrWhiteSpace(options.KeyApi) &&
            !string.IsNullOrWhiteSpace(options.SenderEmail) &&
            !string.IsNullOrWhiteSpace(options.SenderName),
        "Brevo configuration is incomplete.")
    .ValidateOnStart();

    if (builder.Environment.IsProduction())
    {
        builder.Configuration.AddAzureKeyVault(
        new Uri("https://project-xyz-key-vault.vault.azure.net/"),
        new DefaultAzureCredential());
    }
    
builder.Services.AddSingleton(serviceProvider =>
{
    var configuration = serviceProvider
        .GetRequiredService<IConfiguration>();

    var connectionString =
        configuration["ServiceBus:ConnectionString"];

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException(
            "Service Bus connection string is not configured.");
    }

    return new ServiceBusClient(connectionString);
});

builder.Services.AddHttpClient<BrevoEmailSender>();

builder.Services.AddTransient<
    INotificationChannel,
    EmailNotificationChannel>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

await host.RunAsync();