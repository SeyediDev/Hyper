using Hyper.SDK.Auth;
using Hyper.SDK.Errors;
using Hyper.SDK.Clients;
using Hyper.SDK.Models;
using Microsoft.Extensions.Logging;

namespace Hyper.SDK.Services;

public interface IWebhookService
{
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

public sealed class WebhookService : IWebhookService
{
    private readonly IBasalamHttpClient _client;
    private readonly ILogger<WebhookService>? _logger;

    public WebhookService(IBasalamHttpClient client, ILogger<WebhookService>? logger = null)
    {
        _client = client;
        _logger = logger;
    }

    public async Task RegisterWebhookAsync(string url, string[] events, CancellationToken ct = default)
    {
        _logger?.LogInformation("Registering webhook at {Url}", url);
        await _client.PostAsync<object>("/v1/webhooks", new { url, events }, ct);
    }

    public async Task UnregisterWebhookAsync(int webhookId, CancellationToken ct = default)
    {
        _logger?.LogInformation("Unregistering webhook {WebhookId}", webhookId);
        await _client.DeleteAsync<object>($"/v1/webhooks/{webhookId}", ct);
    }

    public async Task<List<WebhookResource>> GetWebhooksAsync(CancellationToken ct = default)
    {
        _logger?.LogInformation("Getting webhooks");
        var result = await _client.GetAsync<PageResult<WebhookResource>>("/v1/webhooks", ct);
        return result?.Data ?? new List<WebhookResource>();
    }
}
