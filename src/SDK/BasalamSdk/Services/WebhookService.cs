using System.Globalization;
using System.Text.Json;
using Basalam.SDK.Errors;

namespace Basalam.SDK.Services;

public interface IWebhookService
{
    Task CreateOfficialWebhookAsync(string url, IReadOnlyCollection<int> eventIds,
        string? authorizationHeader = null, CancellationToken ct = default);
    Task RegisterWebhookAsync(string url, string[] events, CancellationToken ct = default);
    Task UnregisterWebhookAsync(int webhookId, CancellationToken ct = default);
    Task<List<WebhookResource>> GetWebhooksAsync(CancellationToken ct = default);
    Task<WebhookServiceListResource?> GetWebhookServicesAsync(CancellationToken ct = default);
    Task<WebhookServiceResource?> CreateWebhookServiceAsync(CreateWebhookServiceRequest request, CancellationToken ct = default);
    Task<WebhookListResource?> GetWebhooksAsync(WebhookQuery? query, CancellationToken ct = default);
    Task<WebhookResource?> CreateWebhookAsync(CreateWebhookRequest request, CancellationToken ct = default);
    Task<WebhookEventListResource?> GetWebhookEventsAsync(CancellationToken ct = default);
    Task<WebhookCustomerListResource?> GetWebhookCustomersAsync(WebhookCustomersQuery? query = null, CancellationToken ct = default);
    Task<WebhookResource?> UpdateWebhookAsync(int webhookId, UpdateWebhookRequest request, CancellationToken ct = default);
    Task<WebhookDeleteResponse?> DeleteWebhookAsync(int webhookId, CancellationToken ct = default);
    Task<WebhookLogListResource?> GetWebhookLogsAsync(int webhookId, CancellationToken ct = default);
    Task<WebhookClientResource?> RegisterWebhookAsync(RegisterWebhookRequest request, CancellationToken ct = default);
    Task<WebhookUnregisterResponse?> UnregisterWebhookAsync(UnregisterWebhookRequest request, CancellationToken ct = default);
    Task<WebhookRegisteredListResource?> GetRegisteredWebhooksAsync(RegisteredWebhooksQuery? query = null, CancellationToken ct = default);
}

public record WebhookResource
{
    public int Id { get; init; }
    public int? ServiceId { get; init; }
    public string? Url { get; init; }
    // Basalam returns event objects on the lifecycle API; keep the raw provider
    // shape so newer event fields are not discarded during deserialization.
    public JsonElement? Events { get; init; }
    public JsonElement? RequestHeaders { get; init; }
    public string? RequestMethod { get; init; }
    public bool? IsActive { get; init; }
    public string? Status { get; init; }
    public DateTime? CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public sealed record WebhookServiceResource
{
    public int? Id { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public bool? IsVerified { get; init; }
    public bool? IsActive { get; init; }
    public DateTime? CreatedAt { get; init; }
}

public sealed record WebhookServiceListResource
{
    public List<WebhookServiceResource> Data { get; init; } = [];
    public int? ResultCount { get; init; }
    public int? TotalCount { get; init; }
    public int? TotalPage { get; init; }
    public int? Page { get; init; }
    public int? PerPage { get; init; }
}

public sealed record WebhookListResource
{
    public List<WebhookResource> Data { get; init; } = [];
    public int? ResultCount { get; init; }
    public int? TotalCount { get; init; }
    public int? TotalPage { get; init; }
    public int? Page { get; init; }
    public int? PerPage { get; init; }
}

public sealed record WebhookEventResource
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public JsonElement? SampleData { get; init; }
    public string? Scopes { get; init; }
}

public sealed record WebhookEventListResource
{
    public List<WebhookEventResource> Data { get; init; } = [];
    public int? ResultCount { get; init; }
    public int? TotalCount { get; init; }
    public int? TotalPage { get; init; }
    public int? Page { get; init; }
    public int? PerPage { get; init; }
}

public sealed record WebhookCustomerResource
{
    public int Id { get; init; }
    public int CustomerId { get; init; }
    public int WebhookId { get; init; }
    public DateTime? CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public sealed record WebhookClientResource
{
    public int Id { get; init; }
    public int CustomerId { get; init; }
    public int WebhookId { get; init; }
    public DateTime? CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public sealed record WebhookCustomerListResource
{
    public List<WebhookCustomerResource> Data { get; init; } = [];
    public int? ResultCount { get; init; }
    public int? TotalCount { get; init; }
    public int? TotalPage { get; init; }
    public int? Page { get; init; }
    public int? PerPage { get; init; }
}

public sealed record WebhookLogResource
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public int StatusCode { get; init; }
    public JsonElement? Request { get; init; }
    public string? Response { get; init; }
    public DateTime? CreatedAt { get; init; }
}

public sealed record WebhookLogListResource
{
    public List<WebhookLogResource> Data { get; init; } = [];
    public int? ResultCount { get; init; }
    public int? TotalCount { get; init; }
    public int? TotalPage { get; init; }
    public int? Page { get; init; }
    public int? PerPage { get; init; }
}

public sealed record WebhookRegisteredResource
{
    public int Id { get; init; }
    public int ServiceId { get; init; }
    public int CustomerId { get; init; }
    public JsonElement? Events { get; init; }
    public bool? IsActive { get; init; }
    public DateTime? RegisteredAt { get; init; }
}

public sealed record WebhookRegisteredListResource
{
    public List<WebhookRegisteredResource> Data { get; init; } = [];
    public int? ResultCount { get; init; }
    public int? TotalCount { get; init; }
    public int? TotalPage { get; init; }
    public int? Page { get; init; }
    public int? PerPage { get; init; }
}

public sealed record CreateWebhookServiceRequest(string Title, string Description);
public sealed record CreateWebhookRequest(IReadOnlyCollection<int> EventIds, string RequestMethod, string Url,
    int? ServiceId = null, string? RequestHeaders = null, bool? IsActive = null, bool? RegisterMe = null);
public sealed record UpdateWebhookRequest(IReadOnlyCollection<int>? EventIds = null, string? RequestHeaders = null,
    string? RequestMethod = null, string? Url = null, bool? IsActive = null);
public sealed record RegisterWebhookRequest(int WebhookId);
public sealed record UnregisterWebhookRequest(int WebhookId, int? CustomerId = null);
public sealed record WebhookQuery(int? ServiceId = null, string? EventIds = null);
public sealed record WebhookCustomersQuery(int Page = 1, int PerPage = 10, int? WebhookId = null);
public sealed record RegisteredWebhooksQuery(int Page = 1, int PerPage = 10, int? ServiceId = null);
public sealed record WebhookDeleteResponse(int Id, DateTime? DeletedAt = null);
public sealed record WebhookUnregisterResponse(int WebhookId, int CustomerId, DateTime? DeletedAt = null);

public sealed class WebhookService(IBasalamHttpClient client, ILogger<WebhookService>? logger = null) : IWebhookService
{
    public Task<WebhookServiceListResource?> GetWebhookServicesAsync(CancellationToken ct = default) =>
        client.GetAsync<WebhookServiceListResource>("/v1/webhooks/services", ct);

    public Task<WebhookServiceResource?> CreateWebhookServiceAsync(CreateWebhookServiceRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrWhiteSpace(request.Title)) throw Invalid("title");
        if (string.IsNullOrWhiteSpace(request.Description)) throw Invalid("description");
        return client.PostAsync<WebhookServiceResource>("/v1/webhooks/services", request, ct);
    }

    public Task<WebhookListResource?> GetWebhooksAsync(WebhookQuery? query, CancellationToken ct = default) =>
        client.GetAsync<WebhookListResource>("/v1/webhooks" + Query(
            ("service_id", query?.ServiceId?.ToString(CultureInfo.InvariantCulture)), ("event_ids", query?.EventIds)), ct);

    public Task<WebhookResource?> CreateWebhookAsync(CreateWebhookRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateCreate(request);
        return client.PostAsync<WebhookResource>("/v1/webhooks", new
        {
            service_id = request.ServiceId,
            event_ids = request.EventIds,
            request_headers = request.RequestHeaders,
            request_method = request.RequestMethod,
            url = request.Url,
            is_active = request.IsActive,
            register_me = request.RegisterMe
        }, ct);
    }

    public Task<WebhookEventListResource?> GetWebhookEventsAsync(CancellationToken ct = default) =>
        client.GetAsync<WebhookEventListResource>("/v1/webhooks/events", ct);

    public Task<WebhookCustomerListResource?> GetWebhookCustomersAsync(WebhookCustomersQuery? query = null, CancellationToken ct = default)
    {
        var q = query ?? new WebhookCustomersQuery();
        ValidatePage(q.Page, q.PerPage);
        return client.GetAsync<WebhookCustomerListResource>("/v1/webhooks/customers" + Query(
            ("page", q.Page.ToString(CultureInfo.InvariantCulture)), ("per_page", q.PerPage.ToString(CultureInfo.InvariantCulture)),
            ("webhook_id", q.WebhookId?.ToString(CultureInfo.InvariantCulture))), ct);
    }

    public Task<WebhookResource?> UpdateWebhookAsync(int webhookId, UpdateWebhookRequest request, CancellationToken ct = default)
    {
        ValidateId(webhookId, "webhookId");
        ArgumentNullException.ThrowIfNull(request);
        if (request.EventIds is { Count: 0 } || request.EventIds?.Any(x => x <= 0) == true) throw Invalid("eventIds");
        return client.PatchAsync<WebhookResource>($"/v1/webhooks/{webhookId}", new
        {
            event_ids = request.EventIds,
            request_headers = request.RequestHeaders,
            request_method = request.RequestMethod,
            url = request.Url,
            is_active = request.IsActive
        }, ct);
    }

    public Task<WebhookDeleteResponse?> DeleteWebhookAsync(int webhookId, CancellationToken ct = default)
    {
        ValidateId(webhookId, "webhookId");
        return client.DeleteAsync<WebhookDeleteResponse>($"/v1/webhooks/{webhookId}", ct);
    }

    public Task<WebhookLogListResource?> GetWebhookLogsAsync(int webhookId, CancellationToken ct = default)
    {
        ValidateId(webhookId, "webhookId");
        return client.GetAsync<WebhookLogListResource>($"/v1/webhooks/{webhookId}/logs", ct);
    }

    public Task<WebhookClientResource?> RegisterWebhookAsync(RegisterWebhookRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateId(request.WebhookId, "webhookId");
        return client.PostAsync<WebhookClientResource>("/v1/customers/webhooks", new { webhook_id = request.WebhookId }, ct);
    }

    public Task<WebhookUnregisterResponse?> UnregisterWebhookAsync(UnregisterWebhookRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateId(request.WebhookId, "webhookId");
        if (request.CustomerId is <= 0) throw Invalid("customerId");
        return client.DeleteAsync<WebhookUnregisterResponse>("/v1/customers/webhooks", new
        {
            webhook_id = request.WebhookId,
            customer_id = request.CustomerId
        }, ct);
    }

    public Task<WebhookRegisteredListResource?> GetRegisteredWebhooksAsync(RegisteredWebhooksQuery? query = null, CancellationToken ct = default)
    {
        var q = query ?? new RegisteredWebhooksQuery();
        ValidatePage(q.Page, q.PerPage);
        return client.GetAsync<WebhookRegisteredListResource>("/v1/customers/webhooks" + Query(
            ("page", q.Page.ToString(CultureInfo.InvariantCulture)), ("per_page", q.PerPage.ToString(CultureInfo.InvariantCulture)),
            ("service_id", q.ServiceId?.ToString(CultureInfo.InvariantCulture))), ct);
    }
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

    private static void ValidateCreate(CreateWebhookRequest request)
    {
        if (request.EventIds.Count == 0 || request.EventIds.Any(x => x <= 0)) throw Invalid("eventIds");
        ValidateUrl(request.Url);
        if (string.IsNullOrWhiteSpace(request.RequestMethod)) throw Invalid("requestMethod");
    }

    private static void ValidateUrl(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
            throw Invalid("url");
    }

    private static void ValidateId(int value, string field)
    {
        if (value <= 0) throw Invalid(field);
    }

    private static void ValidatePage(int page, int perPage)
    {
        if (page < 1) throw Invalid("page");
        if (perPage is < 1 or > 100) throw Invalid("perPage");
    }

    private static string Query(params (string Name, string? Value)[] values)
    {
        var parts = values.Where(x => !string.IsNullOrWhiteSpace(x.Value))
            .Select(x => Uri.EscapeDataString(x.Name) + "=" + Uri.EscapeDataString(x.Value!));
        var query = string.Join('&', parts);
        return query.Length == 0 ? string.Empty : "?" + query;
    }

    private static BasalamValidationError Invalid(string field) =>
        new(new Dictionary<string, IReadOnlyList<string>> { [field] = ["Value is invalid"] });
}
