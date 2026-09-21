using Hyper.SDK.Auth;
using Hyper.SDK.Errors;
using Hyper.SDK.Clients;
using Hyper.SDK.Models;
using Microsoft.Extensions.Logging;

namespace Hyper.SDK.Services;

public interface IOrderService
{
    Task<object?> CreateOrderAsync(object order, CancellationToken ct = default);
    Task<object?> GetOrderAsync(int orderId, CancellationToken ct = default);
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
        logger?.LogInformation("Getting order {OrderId}", orderId);
        return await client.GetAsync<object>($"/v1/orders/{orderId}", ct);
    }
}

public interface IParcelService
{
    Task<object?> GetParcelAsync(int parcelId, CancellationToken ct = default);
    Task<object?> UpdateParcelStatusAsync(int parcelId, string status, CancellationToken ct = default);
}

public sealed class ParcelService(IBasalamHttpClient client, ILogger<ParcelService>? logger = null) : IParcelService
{
    public async Task<object?> GetParcelAsync(int parcelId, CancellationToken ct = default)
    {
        logger?.LogInformation("Getting parcel {ParcelId}", parcelId);
        return await client.GetAsync<object>($"/v1/parcels/{parcelId}", ct);
    }

    public async Task<object?> UpdateParcelStatusAsync(int parcelId, string status, CancellationToken ct = default)
    {
        logger?.LogInformation("Updating parcel {ParcelId} status to {Status}", parcelId, status);
        return await client.PatchAsync<object>($"/v1/parcels/{parcelId}", new { status }, ct);
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
        logger?.LogInformation("Getting customer {CustomerId}", customerId);
        return await client.GetAsync<object>($"/v1/customers/{customerId}", ct);
    }
}
