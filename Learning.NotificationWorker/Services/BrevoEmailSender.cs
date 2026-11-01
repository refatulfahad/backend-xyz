using System.Net.Http.Json;
using Learning.NotificationWorker.Configuration;
using Microsoft.Extensions.Options;

namespace Learning.NotificationWorker.Services;

public sealed class BrevoEmailSender(
    HttpClient httpClient,
    IOptions<BrevoOptions> options,
    ILogger<BrevoEmailSender> logger)
{
    private readonly BrevoOptions _options = options.Value;

    public async Task SendAsync(
        string recipientEmail,
        string subject,
        string htmlBody,
        string textBody,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.KeyApi))
        {
            throw new InvalidOperationException(
                "Brevo API key is not configured.");
        }

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://api.brevo.com/v3/smtp/email");

        request.Headers.TryAddWithoutValidation(
            "api-key",
            _options.KeyApi);

        request.Content = JsonContent.Create(new
        {
            sender = new
            {
                name = _options.SenderName,
                email = _options.SenderEmail
            },
            to = new[]
            {
                new { email = recipientEmail }
            },
            subject,
            htmlContent = htmlBody,
            textContent = textBody
        });

        using (request)
        using (var response = await httpClient.SendAsync(
            request,
            cancellationToken))
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(
                    cancellationToken);

                logger.LogError(
                    "Brevo email request failed. Status: {StatusCode}, Error: {Error}",
                    (int)response.StatusCode,
                    errorBody);

                throw new HttpRequestException(
                    $"Brevo returned HTTP {(int)response.StatusCode}.");
            }

            logger.LogInformation(
                "Brevo accepted the email request for recipient {RecipientEmail}",
                recipientEmail);
        }
    }
}