using Basalam.SDK.Auth;
using Basalam.SDK.Errors;
using Basalam.SDK.Clients;
using Basalam.SDK.Models;
using Microsoft.Extensions.Logging;

namespace Basalam.SDK.Services;

public interface IOrderService
{
    Task<object?> CreateOrderAsync(object order, CancellationToken ct = default);
    Task<object?> GetOrderAsync(int orderId, CancellationToken ct = default);
    Task<OrderSnapshot?> GetOrderSnapshotAsync(int orderId, CancellationToken ct = default);
}

public sealed class OrderService(IBasalamHttpClient client, ILogger<OrderService>? logger = null) : IOrderService
{
    public async Task<object?> CreateOrderAsync(object order, CancellationToken ct = default)
    {
        logger?.LogInformation("Creating order");
        return await client.PostAsync<object>("/v1/orders", order, ct);
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
    Task<ParcelSnapshot?> GetParcelSnapshotAsync(int parcelId, CancellationToken ct = default);
}

public sealed class ParcelService(IBasalamHttpClient client, ILogger<ParcelService>? logger = null) : IParcelService
{
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

    public Task<ParcelSnapshot?> GetParcelSnapshotAsync(int parcelId, CancellationToken ct = default)
    {
        ValidateId(parcelId);
        return client.GetAsync<ParcelSnapshot>($"/v1/parcels/{parcelId}", ct);
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
}

public interface ICustomerService
{
    Task<object?> GetCustomerAsync(int customerId, CancellationToken ct = default);
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
}
