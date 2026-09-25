using System.Text.Json;

namespace Basalam.SDK.Services;

public interface IWebhookService
{
    Task CreateOfficialWebhookAsync(string url, IReadOnlyCollection<int> eventIds,
        string? authorizationHeader = null, CancellationToken ct = default);
    Task RegisterWebhookAsync(string url, string[] events, CancellationToken ct = default);
    Task UnregisterWebhookAsync(int webhookId, CancellationToken ct = default);
    Task<List<WebhookResource>> GetWebhooksAsync(CancellationToken ct = default);
}

public record WebhookResource
{
    public required int Id { get; init; }
    public required string Url { get; init; }
    public required string[] Events { get; init; }
    public string? Status { get; init; }
    public DateTime? CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public sealed class WebhookService(IBasalamHttpClient client, ILogger<WebhookService>? logger = null) : IWebhookService
{
    public async Task CreateOfficialWebhookAsync(string url, IReadOnlyCollection<int> eventIds,
        string? authorizationHeader = null, CancellationToken ct = default)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var callback) || callback.Scheme != Uri.UriSchemeHttps)
            throw new ArgumentException("Webhook callback must be HTTPS.", nameof(url));
        if (eventIds.Count == 0 || eventIds.Any(x => x <= 0))
            throw new ArgumentException("At least one valid Basalam event id is required.", nameof(eventIds));
        var headers = string.IsNullOrWhiteSpace(authorizationHeader) ? null : $"Authorization: {authorizationHeader}";
        await client.PostAsync<JsonElement>("https://webhook.basalam.com/v1/webhooks", new
        {
            event_ids = eventIds,
            request_method = "POST",
            request_headers = headers,
            url,
            is_active = true,
            register_me = true
        }, ct);
    }

    public async Task RegisterWebhookAsync(string url, string[] events, CancellationToken ct = default)
    {
        logger?.LogInformation("Registering webhook at {Url}", url);
        await client.PostAsync<object>("/v1/webhooks", new { url, events }, ct);
    }

    public async Task UnregisterWebhookAsync(int webhookId, CancellationToken ct = default)
    {
        logger?.LogInformation("Unregistering webhook {WebhookId}", webhookId);
        await client.DeleteAsync<object>($"/v1/webhooks/{webhookId}", ct);
    }

    public async Task<List<WebhookResource>> GetWebhooksAsync(CancellationToken ct = default)
    {
        logger?.LogInformation("Getting webhooks");
        var result = await client.GetAsync<PageResult<WebhookResource>>("/v1/webhooks", ct);
        return result?.Data ?? new List<WebhookResource>();
    }
}
