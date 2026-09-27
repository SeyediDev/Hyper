using System.Text.Json;
using Basalam.SDK.Clients;
using Basalam.SDK.Errors;

namespace Basalam.SDK.Services;

public interface IOrderProcessingService
{
    Task<JsonElement> GetCustomerOrdersAsync(string? query = null, CancellationToken ct = default);
    Task<JsonElement> GetCustomerOrderAsync(int orderId, CancellationToken ct = default);
    Task<JsonElement> GetCustomerOrderParcelHintsAsync(int orderId, CancellationToken ct = default);
    Task<JsonElement> GetCustomerOrderItemsAsync(string? query = null, CancellationToken ct = default);
    Task<JsonElement> GetCustomerOrderItemAsync(int itemId, CancellationToken ct = default);
}

public sealed class OrderProcessingService(IBasalamHttpClient client) : IOrderProcessingService
{
    public Task<JsonElement> GetCustomerOrdersAsync(string? query = null, CancellationToken ct = default) => Get("/v1/customer-orders" + Suffix(query), ct);
    public Task<JsonElement> GetCustomerOrderAsync(int orderId, CancellationToken ct = default) => Get($"/v1/customer-orders/{Id(orderId, "orderId")}", ct);
    public Task<JsonElement> GetCustomerOrderParcelHintsAsync(int orderId, CancellationToken ct = default) => Get($"/v1/customer-orders/{Id(orderId, "orderId")}/parcel-hints", ct);
    public Task<JsonElement> GetCustomerOrderItemsAsync(string? query = null, CancellationToken ct = default) => Get("/v1/customer-orders/items" + Suffix(query), ct);
    public Task<JsonElement> GetCustomerOrderItemAsync(int itemId, CancellationToken ct = default) => Get($"/v1/customer-orders/items/{Id(itemId, "itemId")}", ct);

    private Task<JsonElement> Get(string path, CancellationToken ct) => client.GetAsync<JsonElement>(path, ct);
    private static int Id(int value, string field) { if (value <= 0) throw new BasalamValidationError(new Dictionary<string, IReadOnlyList<string>> { [field] = ["Identifier must be positive"] }); return value; }
    private static string Suffix(string? query) => string.IsNullOrWhiteSpace(query) ? string.Empty : query.StartsWith('?') ? query : "?" + query;
}
