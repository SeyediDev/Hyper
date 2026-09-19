using Hyper.SDK.Auth;
using Hyper.SDK.Clients;
using Hyper.SDK.Config;
using Hyper.SDK.Errors;
using Hyper.SDK.Models;
using Microsoft.Extensions.Logging;

namespace Hyper.SDK.Services;

public interface IProductService
{
    Task<Product?> GetProductAsync(int productId, CancellationToken ct = default);
    Task PatchStockAsync(int productId, int stock, CancellationToken ct = default);
}

public sealed class ProductService : IProductService
{
    private readonly IBasalamHttpClient _client;
    private readonly ILogger<ProductService>? _logger;

    public ProductService(IBasalamHttpClient client, ILogger<ProductService>? logger = null)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<Product?> GetProductAsync(int productId, CancellationToken ct = default)
    {
        _logger?.LogInformation("Getting product {ProductId}", productId);
        return await _client.GetAsync<Product>($"/v1/products/{productId}", ct);
    }

    public async Task PatchStockAsync(int productId, int stock, CancellationToken ct = default)
    {
        _logger?.LogInformation("Patching stock for product {ProductId} to {Stock}", productId, stock);
        if (stock < 0)
            throw new BasalamValidationError(new Dictionary<string, IReadOnlyList<string>>
            {
                ["stock"] = ["Stock must be non-negative"]
            });
        await _client.PatchAsync<object>($"/v1/products/{productId}", new { stock }, ct);
    }
}
