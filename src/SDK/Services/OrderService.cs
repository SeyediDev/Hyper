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

public sealed class OrderService : IOrderService
{
    private readonly IBasalamHttpClient _client;
    private readonly ILogger<OrderService>? _logger;

    public OrderService(IBasalamHttpClient client, ILogger<OrderService>? logger = null)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<object?> CreateOrderAsync(object order, CancellationToken ct = default)
    {
        _logger?.LogInformation("Creating order");
        return await _client.PostAsync<object>("/v1/orders", order, ct);
    }

    public async Task<object?> GetOrderAsync(int orderId, CancellationToken ct = default)
    {
        _logger?.LogInformation("Getting order {OrderId}", orderId);
        return await _client.GetAsync<object>($"/v1/orders/{orderId}", ct);
    }
}

public interface IParcelService
{
    Task<object?> GetParcelAsync(int parcelId, CancellationToken ct = default);
    Task<object?> UpdateParcelStatusAsync(int parcelId, string status, CancellationToken ct = default);
}

public sealed class ParcelService : IParcelService
{
    private readonly IBasalamHttpClient _client;
    private readonly ILogger<ParcelService>? _logger;

    public ParcelService(IBasalamHttpClient client, ILogger<ParcelService>? logger = null)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<object?> GetParcelAsync(int parcelId, CancellationToken ct = default)
    {
        _logger?.LogInformation("Getting parcel {ParcelId}", parcelId);
        return await _client.GetAsync<object>($"/v1/parcels/{parcelId}", ct);
    }

    public async Task<object?> UpdateParcelStatusAsync(int parcelId, string status, CancellationToken ct = default)
    {
        _logger?.LogInformation("Updating parcel {ParcelId} status to {Status}", parcelId, status);
        return await _client.PatchAsync<object>($"/v1/parcels/{parcelId}", new { status }, ct);
    }
}

public interface ICustomerService
{
    Task<object?> GetCustomerAsync(int customerId, CancellationToken ct = default);
}

public sealed class CustomerService : ICustomerService
{
    private readonly IBasalamHttpClient _client;
    private readonly ILogger<CustomerService>? _logger;

    public CustomerService(IBasalamHttpClient client, ILogger<CustomerService>? logger = null)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<object?> GetCustomerAsync(int customerId, CancellationToken ct = default)
    {
        _logger?.LogInformation("Getting customer {CustomerId}", customerId);
        return await _client.GetAsync<object>($"/v1/customers/{customerId}", ct);
    }
}
