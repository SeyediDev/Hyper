using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using Basalam.SDK.Clients;
using Basalam.SDK.Errors;

namespace Basalam.SDK.Services;

public sealed record AppstoreTransactionsQuery(int? Page = null, int? PerPage = null,
    string? Status = null, string? FromDate = null, string? ToDate = null);
public sealed record AppstoreSubscriptionsQuery(int? PlanId = null, string? Status = null,
    int? CustomerId = null, int? Page = null, int? PerPage = null);

public interface IAppstoreService
{
    Task<JsonElement> GetPaymentMethodsAsync(bool? includeDisabled = null, string? gatewaySecret = null, CancellationToken ct = default);
    Task<JsonElement> ListTransactionsAsync(AppstoreTransactionsQuery? query = null, string? gatewaySecret = null, CancellationToken ct = default);
    Task<JsonElement> ListUnverifiedTransactionsAsync(int? page = null, int? perPage = null, string? gatewaySecret = null, CancellationToken ct = default);
    Task<JsonElement> InquiryTransactionAsync(string hashId, string? gatewaySecret = null, CancellationToken ct = default);
    Task<JsonElement> VerifyTransactionAsync(string hashId, string? gatewaySecret = null, CancellationToken ct = default);
    Task<JsonElement> CreatePreTransactionAsync(object request, string? gatewaySecret = null, CancellationToken ct = default);
    Task<JsonElement> ListPlansAsync(CancellationToken ct = default);
    Task<JsonElement> ListPlanSubscriptionsAsync(AppstoreSubscriptionsQuery? query = null, CancellationToken ct = default);
    Task<JsonElement> GetPlanSubscriptionAsync(int subscriptionId, CancellationToken ct = default);
}

public sealed class AppstoreService(IBasalamHttpClient client, ILogger<AppstoreService>? logger = null) : IAppstoreService
{
    public Task<JsonElement> GetPaymentMethodsAsync(bool? includeDisabled = null, string? gatewaySecret = null, CancellationToken ct = default) =>
        GetAsync("/v1/pay/methods" + Query(("include_disabled", includeDisabled?.ToString().ToLowerInvariant())), gatewaySecret, ct);

    public Task<JsonElement> ListTransactionsAsync(AppstoreTransactionsQuery? query = null, string? gatewaySecret = null, CancellationToken ct = default) =>
        GetAsync("/v1/pay/transactions" + Query(
            ("page", query?.Page?.ToString(CultureInfo.InvariantCulture)),
            ("per_page", query?.PerPage?.ToString(CultureInfo.InvariantCulture)),
            ("status", query?.Status), ("from_date", query?.FromDate), ("to_date", query?.ToDate)), gatewaySecret, ct);

    public Task<JsonElement> ListUnverifiedTransactionsAsync(int? page = null, int? perPage = null, string? gatewaySecret = null, CancellationToken ct = default) =>
        GetAsync("/v1/pay/transactions/unverified" + Query(("page", page?.ToString(CultureInfo.InvariantCulture)),
            ("per_page", perPage?.ToString(CultureInfo.InvariantCulture))), gatewaySecret, ct);

    public Task<JsonElement> InquiryTransactionAsync(string hashId, string? gatewaySecret = null, CancellationToken ct = default) =>
        GetAsync($"/v1/pay/transactions/{SafeText(hashId, "hashId")}/inquiry", gatewaySecret, ct);

    public Task<JsonElement> VerifyTransactionAsync(string hashId, string? gatewaySecret = null, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Post, $"/v1/pay/transactions/{SafeText(hashId, "hashId")}/verify", null, gatewaySecret, ct);

    public Task<JsonElement> CreatePreTransactionAsync(object request, string? gatewaySecret = null, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return SendAsync(HttpMethod.Post, "/v1/pay/pre-transactions", request, gatewaySecret, ct);
    }

    public Task<JsonElement> ListPlansAsync(CancellationToken ct = default) => GetAsync("/v1/plans", null, ct);

    public Task<JsonElement> ListPlanSubscriptionsAsync(AppstoreSubscriptionsQuery? query = null, CancellationToken ct = default) =>
        GetAsync("/v1/plans/subscriptions" + Query(("plan_id", query?.PlanId?.ToString(CultureInfo.InvariantCulture)),
            ("status", query?.Status), ("customer_id", query?.CustomerId?.ToString(CultureInfo.InvariantCulture)),
            ("page", query?.Page?.ToString(CultureInfo.InvariantCulture)),
            ("per_page", query?.PerPage?.ToString(CultureInfo.InvariantCulture))), null, ct);

    public Task<JsonElement> GetPlanSubscriptionAsync(int subscriptionId, CancellationToken ct = default)
    {
        if (subscriptionId <= 0) throw Invalid("subscriptionId");
        return GetAsync($"/v1/plans/subscriptions/{subscriptionId}", null, ct);
    }

    private Task<JsonElement> GetAsync(string url, string? secret, CancellationToken ct) =>
        SendAsync(HttpMethod.Get, url, null, secret, ct);

    private async Task<JsonElement> SendAsync(HttpMethod method, string url, object? body, string? secret, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(method, url);
        if (!string.IsNullOrWhiteSpace(secret)) request.Headers.TryAddWithoutValidation("X-Gateway-Secret", secret);
        if (body is not null) request.Content = JsonContent.Create(body);
        logger?.LogInformation("Calling Basalam Appstore {Method} {Path}", method, url.Split('?')[0]);
        using var response = await client.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode)
            throw new BasalamAPIError($"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}", (int)response.StatusCode);
        var json = await response.Content.ReadAsStringAsync(ct);
        return string.IsNullOrWhiteSpace(json) ? default : JsonDocument.Parse(json).RootElement.Clone();
    }

    private static string SafeText(string value, string field)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 200 || value.Any(char.IsControl)
            || value.Contains('/') || value.Contains('?')) throw Invalid(field);
        return Uri.EscapeDataString(value);
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
