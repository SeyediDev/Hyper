using System.Text.Json;
using Basalam.SDK.Auth;
using Basalam.SDK.Errors;
using Basalam.SDK.Clients;
using Basalam.SDK.Models;
using Microsoft.Extensions.Logging;

namespace Basalam.SDK.Services;

public interface IOrderService
{
    Task<object?> CreateOrderAsync(object order, CancellationToken ct = default);
    Task<OrderSnapshot?> CreateOrderSnapshotAsync(object order, CancellationToken ct = default);
    Task<object?> GetOrderAsync(int orderId, CancellationToken ct = default);
    Task<OrderSnapshot?> GetOrderSnapshotAsync(int orderId, CancellationToken ct = default);
    Task<JsonElement> GetOrderStatsAsync(OrderStatsQuery query, CancellationToken ct = default);
}

public sealed record OrderStatsQuery(string ResourceCount, int? VendorId = null, int? ProductId = null,
    int? CustomerId = null, string? CouponCode = null);

public sealed class OrderService(IBasalamHttpClient client, ILogger<OrderService>? logger = null) : IOrderService
{
    public async Task<object?> CreateOrderAsync(object order, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(order);
        logger?.LogInformation("Creating order");
        return await client.PostAsync<object>("/v1/orders", order, ct);
    }

    public Task<OrderSnapshot?> CreateOrderSnapshotAsync(object order, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(order);
        logger?.LogInformation("Creating typed order snapshot");
        return client.PostAsync<OrderSnapshot>("/v1/orders", order, ct);
    }

    public async Task<object?> GetOrderAsync(int orderId, CancellationToken ct = default)
    {
        ValidateId(orderId);
        logger?.LogInformation("Getting order {OrderId}", orderId);
        return await client.GetAsync<object>($"/v1/orders/{orderId}", ct);
    }

    public Task<OrderSnapshot?> GetOrderSnapshotAsync(int orderId, CancellationToken ct = default)
    {
        ValidateId(orderId);
        return client.GetAsync<OrderSnapshot>($"/v1/orders/{orderId}", ct);
    }

    public Task<JsonElement> GetOrderStatsAsync(OrderStatsQuery query, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        if (string.IsNullOrWhiteSpace(query.ResourceCount) || query.ResourceCount.Length > 100)
            throw new BasalamValidationError(new Dictionary<string, IReadOnlyList<string>>
            { ["resourceCount"] = ["ResourceCount is required"] });
        return client.GetAsync<JsonElement>("/v1/orders/stats" + Query(
            ("resource_count", query.ResourceCount), ("vendor_id", query.VendorId?.ToString()),
            ("product_id", query.ProductId?.ToString()), ("customer_id", query.CustomerId?.ToString()),
            ("coupon_code", query.CouponCode)), ct);
    }

    private static string Query(params (string Name, string? Value)[] values)
    {
        var parts = values.Where(x => !string.IsNullOrWhiteSpace(x.Value))
            .Select(x => Uri.EscapeDataString(x.Name) + "=" + Uri.EscapeDataString(x.Value!));
        var query = string.Join('&', parts);
        return query.Length == 0 ? string.Empty : "?" + query;
    }

    private static void ValidateId(int id)
    {
        if (id <= 0) throw new BasalamValidationError(new Dictionary<string, IReadOnlyList<string>>
        { ["id"] = ["Identifier must be positive"] });
    }
}

public interface IParcelService
{
    Task<object?> GetParcelAsync(int parcelId, CancellationToken ct = default);
    Task<object?> UpdateParcelStatusAsync(int parcelId, string status, CancellationToken ct = default);
    Task<ParcelSnapshot?> UpdateParcelStatusSnapshotAsync(int parcelId, string status,
        CancellationToken ct = default);
    Task<ParcelSnapshot?> GetParcelSnapshotAsync(int parcelId, CancellationToken ct = default);
    Task<JsonElement> GetVendorParcelsAsync(VendorParcelsQuery? query = null, CancellationToken ct = default);
    Task<JsonElement> GetVendorParcelAsync(int parcelId, CancellationToken ct = default);
    Task<JsonElement> SetParcelPreparationAsync(int parcelId, CancellationToken ct = default);
    Task<JsonElement> SetParcelPostedAsync(int parcelId, object postedData, CancellationToken ct = default);
}

public sealed record VendorParcelsQuery(string? CreatedAt = null, string? Cursor = null,
    string? EstimateSendAt = null, string? Ids = null, string? CustomerIds = null,
    string? OrderIds = null, string? ProductIds = null, string? VendorIds = null,
    int? PerPage = null, string? Sort = null, IReadOnlyCollection<string>? Statuses = null);

public sealed class ParcelService(IBasalamHttpClient client, ILogger<ParcelService>? logger = null) : IParcelService
{
    public async Task<VendorParcelSnapshot> ReadVendorParcelSnapshotAsync(int parcelId, CancellationToken ct = default)
    {
        ValidateId(parcelId);
        var body = await client.GetAsync<JsonElement>($"/v1/vendor-parcels/{parcelId}", ct);
        return VendorParcelSnapshot.Parse(body, parcelId);
    }

    public async Task<object?> GetParcelAsync(int parcelId, CancellationToken ct = default)
    {
        ValidateId(parcelId);
        logger?.LogInformation("Getting parcel {ParcelId}", parcelId);
        return await client.GetAsync<object>($"/v1/parcels/{parcelId}", ct);
    }

    public async Task<object?> UpdateParcelStatusAsync(int parcelId, string status, CancellationToken ct = default)
    {
        ValidateId(parcelId); ValidateStatus(status);
        logger?.LogInformation("Updating parcel {ParcelId} status to {Status}", parcelId, status);
        return await client.PatchAsync<object>($"/v1/parcels/{parcelId}", new { status }, ct);
    }

    public Task<ParcelSnapshot?> UpdateParcelStatusSnapshotAsync(int parcelId, string status,
        CancellationToken ct = default)
    {
        ValidateId(parcelId); ValidateStatus(status);
        return client.PatchAsync<ParcelSnapshot>($"/v1/parcels/{parcelId}", new { status }, ct);
    }

    public Task<ParcelSnapshot?> GetParcelSnapshotAsync(int parcelId, CancellationToken ct = default)
    {
        ValidateId(parcelId);
        return client.GetAsync<ParcelSnapshot>($"/v1/parcels/{parcelId}", ct);
    }

    public Task<JsonElement> GetVendorParcelsAsync(VendorParcelsQuery? query = null, CancellationToken ct = default)
    {
        var q = query ?? new VendorParcelsQuery();
        if (q.PerPage is < 1 or > 100) throw Invalid("perPage");
        var url = "/v1/vendor-parcels" + Query(
            ("created_at", q.CreatedAt), ("cursor", q.Cursor), ("estimate_send_at", q.EstimateSendAt),
            ("ids", q.Ids), ("items.customer_ids", q.CustomerIds), ("items.order_ids", q.OrderIds),
            ("items.product_ids", q.ProductIds), ("items.vendor_ids", q.VendorIds),
            ("per_page", q.PerPage?.ToString()), ("sort", q.Sort),
            ("statuses", q.Statuses is { Count: > 0 } ? string.Join(',', q.Statuses) : null));
        return client.GetAsync<JsonElement>(url, ct);
    }

    public Task<JsonElement> GetVendorParcelAsync(int parcelId, CancellationToken ct = default)
    {
        ValidateId(parcelId);
        return client.GetAsync<JsonElement>($"/v1/vendor-parcels/{parcelId}", ct);
    }

    public Task<JsonElement> SetParcelPreparationAsync(int parcelId, CancellationToken ct = default)
    {
        ValidateId(parcelId);
        return PostOnce($"/v1/vendor-parcels/{parcelId}/set-preparation", null, ct);
    }

    public Task<JsonElement> SetParcelPostedAsync(int parcelId, object postedData, CancellationToken ct = default)
    {
        ValidateId(parcelId);
        ArgumentNullException.ThrowIfNull(postedData);
        return PostOnce($"/v1/vendor-parcels/{parcelId}/set-posted", postedData, ct);
    }

    private async Task<JsonElement> PostOnce(string path, object? body, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, path);
        if (body is not null) request.Content = System.Net.Http.Json.JsonContent.Create(body);
        request.Options.Set(BasalamHttpClient.DisableRetries, true);
        using var response = await client.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode) throw new BasalamAPIError("Parcel command failed", (int)response.StatusCode, null);
        var content = await response.Content.ReadAsStringAsync(ct);
        return string.IsNullOrWhiteSpace(content) ? default : JsonSerializer.Deserialize<JsonElement>(content);
    }

    private static void ValidateId(int id)
    {
        if (id <= 0) throw new BasalamValidationError(new Dictionary<string, IReadOnlyList<string>>
        { ["id"] = ["Identifier must be positive"] });
    }
    private static void ValidateStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status) || status.Length > 100 || status.Any(char.IsControl))
            throw new BasalamValidationError(new Dictionary<string, IReadOnlyList<string>>
            { ["status"] = ["Status is required and must be a short value"] });
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

public interface ICustomerService
{
    Task<object?> GetCustomerAsync(int customerId, CancellationToken ct = default);
    Task<CustomerSnapshot?> GetCustomerSnapshotAsync(int customerId, CancellationToken ct = default);
}

public sealed class CustomerService(IBasalamHttpClient client, ILogger<CustomerService>? logger = null) : ICustomerService
{
    public async Task<object?> GetCustomerAsync(int customerId, CancellationToken ct = default)
    {
        if (customerId <= 0) throw new BasalamValidationError(new Dictionary<string, IReadOnlyList<string>>
        { ["id"] = ["Identifier must be positive"] });
        logger?.LogInformation("Getting customer {CustomerId}", customerId);
        return await client.GetAsync<object>($"/v1/customers/{customerId}", ct);
    }

    public Task<CustomerSnapshot?> GetCustomerSnapshotAsync(int customerId, CancellationToken ct = default)
    {
        if (customerId <= 0) throw new BasalamValidationError(new Dictionary<string, IReadOnlyList<string>>
        { ["id"] = ["Identifier must be positive"] });
        logger?.LogInformation("Getting typed customer snapshot {CustomerId}", customerId);
        return client.GetAsync<CustomerSnapshot>($"/v1/customers/{customerId}", ct);
    }
}
