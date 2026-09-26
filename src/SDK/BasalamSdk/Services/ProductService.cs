namespace Basalam.SDK.Services;

public interface IProductService
{
    Task<Product?> GetProductAsync(int productId, CancellationToken ct = default);
    Task<Product?> CreateProductAsync(ProductWriteRequest product, CancellationToken ct = default);
    Task<Product?> UpdateProductAsync(int productId, ProductWriteRequest product, CancellationToken ct = default);
    Task PatchStockAsync(int productId, int stock, CancellationToken ct = default);
    Task PatchDetailsAsync(int productId, string? name, long? primaryPrice, CancellationToken ct = default);
}

public sealed class ProductService(IBasalamHttpClient client, ILogger<ProductService>? logger = null) : IProductService
{
    public async Task<Product?> GetProductAsync(int productId, CancellationToken ct = default)
    {
        ValidateId(productId);
        logger?.LogInformation("Getting product {ProductId}", productId);
        return await client.GetAsync<Product>($"/v1/products/{productId}", ct);
    }

    public Task<Product?> CreateProductAsync(ProductWriteRequest product, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(product); ValidateProduct(product);
        return client.PostAsync<Product>("/v1/products", product, ct);
    }

    public Task<Product?> UpdateProductAsync(int productId, ProductWriteRequest product, CancellationToken ct = default)
    {
        ValidateId(productId); ArgumentNullException.ThrowIfNull(product); ValidateProduct(product);
        return client.PatchAsync<Product>($"/v1/products/{productId}", product, ct);
    }

    public async Task PatchStockAsync(int productId, int stock, CancellationToken ct = default)
    {
        ValidateId(productId);
        logger?.LogInformation("Patching stock for product {ProductId} to {Stock}", productId, stock);
        if (stock < 0)
            throw new BasalamValidationError(new Dictionary<string, IReadOnlyList<string>>
            {
                ["stock"] = ["Stock must be non-negative"]
            });
        await client.PatchAsync<object>($"/v1/products/{productId}", new { stock }, ct);
    }

    public async Task PatchDetailsAsync(int productId, string? name, long? primaryPrice, CancellationToken ct = default)
    {
        ValidateId(productId);
        if (name is null && primaryPrice is null || primaryPrice < 0
            || name is not null && (string.IsNullOrWhiteSpace(name) || name.Length > 500))
            throw new BasalamValidationError(new Dictionary<string, IReadOnlyList<string>>
            { ["product"] = ["A valid name or nonnegative primary price is required"] });
        // Official PATCH schema uses name/primary_price, not title/price.
        // Omit absent fields entirely; never clear stock/category or a missing name.
        var patch = new Dictionary<string, object>();
        if (name is not null) patch["name"] = name;
        if (primaryPrice is not null) patch["primary_price"] = primaryPrice.Value;
        await client.PatchAsync<object>($"/v1/products/{productId}", patch, ct);
    }

    private static void ValidateProduct(ProductWriteRequest product)
    {
        if (string.IsNullOrWhiteSpace(product.Name) || product.Name.Length > 500 || product.VendorId <= 0)
            throw new BasalamValidationError(new Dictionary<string, IReadOnlyList<string>>
            { ["product"] = ["Name and a positive VendorId are required"] });
        if (product.Stock < 0) throw new BasalamValidationError(new Dictionary<string, IReadOnlyList<string>>
            { ["stock"] = ["Stock must be non-negative"] });
    }
    private static void ValidateId(int id)
    {
        if (id <= 0) throw new BasalamValidationError(new Dictionary<string, IReadOnlyList<string>>
            { ["id"] = ["Identifier must be positive"] });
    }
}
