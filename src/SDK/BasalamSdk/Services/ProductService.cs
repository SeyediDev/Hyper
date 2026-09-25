namespace Basalam.SDK.Services;

public interface IProductService
{
    Task<Product?> GetProductAsync(int productId, CancellationToken ct = default);
    Task PatchStockAsync(int productId, int stock, CancellationToken ct = default);
}

public sealed class ProductService(IBasalamHttpClient client, ILogger<ProductService>? logger = null) : IProductService
{
    public async Task<Product?> GetProductAsync(int productId, CancellationToken ct = default)
    {
        logger?.LogInformation("Getting product {ProductId}", productId);
        return await client.GetAsync<Product>($"/v1/products/{productId}", ct);
    }

    public async Task PatchStockAsync(int productId, int stock, CancellationToken ct = default)
    {
        logger?.LogInformation("Patching stock for product {ProductId} to {Stock}", productId, stock);
        if (stock < 0)
            throw new BasalamValidationError(new Dictionary<string, IReadOnlyList<string>>
            {
                ["stock"] = ["Stock must be non-negative"]
            });
        await client.PatchAsync<object>($"/v1/products/{productId}", new { stock }, ct);
    }
}
