using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PublicQ.Application.Interfaces;
using PublicQ.Application.Models;
using PublicQ.Domain.Enums;
using PublicQ.Infrastructure.Options;
using PublicQ.Shared;

namespace PublicQ.Infrastructure.Handlers;

/// <summary>
/// Resend message handler using direct REST API.
/// </summary>
public class ResendMessageHandler(
    IHttpClientFactory httpClientFactory,
    IOptionsMonitor<ResendOptions> options,
    ILogger<ResendMessageHandler> logger)
    : IMessageHandler
{
    private const string ResendApiUrl = "https://api.resend.com/emails";

    public MessageProvider Provider => MessageProvider.Resend;

    public async Task<Response<GenericOperationStatuses>> SendAsync(
        Message message,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Message received by '{Provider}'.", Provider);

        Guard.AgainstNull(message, nameof(message));
        Guard.AgainstNullOrWhiteSpace(message.Sender, nameof(message.Sender));
        Guard.AgainstNullOrWhiteSpace(message.Recipients, nameof(message.Recipients));
        Guard.AgainstNullOrWhiteSpace(message.Body, nameof(message.Body));

        var recipientsString = string.Join(", ", message.Recipients);

        logger.LogInformation("Sending message from '{Sender}' to '{Recipients}' using '{Provider}'.",
            message.Sender,
            recipientsString,
            Provider);

        var apiKey = options.CurrentValue.ApiKey;
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            logger.LogError("Resend API Key is not configured.");
            return Response<GenericOperationStatuses>.Failure(
                GenericOperationStatuses.Failed,
                "Resend API Key is not configured.");
        }

        using var client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var requestBody = new
        {
            from = message.Sender,
            to = message.Recipients,
            subject = message.Subject ?? "No Subject",
            html = message.Body
        };

        try
        {
            var response = await client.PostAsJsonAsync(ResendApiUrl, requestBody, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Message sent successfully to '{Recipients}' using Resend.", recipientsString);
                return Response<GenericOperationStatuses>.Success(GenericOperationStatuses.Completed,
                    $"Message sent to '{recipientsString}' via Resend.");
            }

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("Resend API returned error: {StatusCode} - {Error}", response.StatusCode, errorContent);
            
            return Response<GenericOperationStatuses>.Failure(
                GenericOperationStatuses.Failed,
                $"Resend API error: {response.StatusCode}. Details: {errorContent}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while sending email via Resend: {Message}", ex.Message);
            return Response<GenericOperationStatuses>.Failure(
                GenericOperationStatuses.Failed,
                $"An error occurred while sending email via Resend: {ex.Message}");
        }
    }
}
