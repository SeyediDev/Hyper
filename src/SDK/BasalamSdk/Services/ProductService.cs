using System.Text.Json;

namespace Basalam.SDK.Services;

public interface IProductService
{
    Task<Product?> GetProductAsync(int productId, CancellationToken ct = default);
    Task<Product?> CreateProductAsync(ProductWriteRequest product, CancellationToken ct = default);
    Task<Product?> UpdateProductAsync(int productId, ProductWriteRequest product, CancellationToken ct = default);
    Task PatchStockAsync(int productId, int stock, CancellationToken ct = default);
    Task PatchDetailsAsync(int productId, string? name, long? primaryPrice, CancellationToken ct = default);
    Task<JsonElement> UpdateBulkProductsAsync(int vendorId, ProductBatchUpdateRequest request,
        bool? continueOnError = null, CancellationToken ct = default);
    Task<JsonElement> CreateStockReminderAsync(int productId, CancellationToken ct = default);
    Task<JsonElement> DeleteStockReminderAsync(int productId, CancellationToken ct = default);
    Task<JsonElement> GetPriceHistoryAsync(int productId, string? startTime = null,
        string? endTime = null, CancellationToken ct = default);
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
        return client.PostAsync<Product>($"/v1/vendors/{product.VendorId}/products", ToWire(product), ct);
    }

    public Task<Product?> UpdateProductAsync(int productId, ProductWriteRequest product, CancellationToken ct = default)
    {
        ValidateId(productId); ArgumentNullException.ThrowIfNull(product); ValidateProduct(product);
        return client.PatchAsync<Product>($"/v1/products/{productId}", ToWire(product), ct);
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

    public Task<JsonElement> UpdateBulkProductsAsync(int vendorId, ProductBatchUpdateRequest request,
        bool? continueOnError = null, CancellationToken ct = default)
    {
        ValidateId(vendorId);
        ArgumentNullException.ThrowIfNull(request);
        if (request.Data.Count == 0 || request.Data.Any(item => item.Id <= 0
            || item.Stock is < 0 || item.PrimaryPrice is < 0
            || item.Name is { Length: > 500 }))
            throw new BasalamValidationError(new Dictionary<string, IReadOnlyList<string>>
            { ["data"] = ["At least one valid product update is required"] });

        var query = continueOnError.HasValue
            ? $"?continue_on_error={continueOnError.Value.ToString().ToLowerInvariant()}"
            : string.Empty;
        return client.PatchAsync<JsonElement>($"/v1/vendors/{vendorId}/products/batch-updates" + query,
            new { data = request.Data.Select(item => new
            {
                id = item.Id,
                name = item.Name,
                stock = item.Stock,
                primary_price = item.PrimaryPrice,
                status = item.Status
            }) }, ct);
    }

    public Task<JsonElement> CreateStockReminderAsync(int productId, CancellationToken ct = default)
    {
        ValidateId(productId);
        return client.PostAsync<JsonElement>($"/v1/products/{productId}/reminders", null, ct);
    }

    public Task<JsonElement> DeleteStockReminderAsync(int productId, CancellationToken ct = default)
    {
        ValidateId(productId);
        return client.DeleteAsync<JsonElement>($"/v1/products/{productId}/reminders", ct);
    }

    public Task<JsonElement> GetPriceHistoryAsync(int productId, string? startTime = null,
        string? endTime = null, CancellationToken ct = default)
    {
        ValidateId(productId);
        var query = Query(("start_time", startTime), ("end_time", endTime));
        return client.GetAsync<JsonElement>($"/v1/products/{productId}/price-history{query}", ct);
    }

    private static void ValidateProduct(ProductWriteRequest product)
    {
        if (string.IsNullOrWhiteSpace(product.Name) || product.Name.Length > 500 || product.VendorId <= 0)
            throw new BasalamValidationError(new Dictionary<string, IReadOnlyList<string>>
            { ["product"] = ["Name and a positive VendorId are required"] });
        if (product.Stock < 0) throw new BasalamValidationError(new Dictionary<string, IReadOnlyList<string>>
            { ["stock"] = ["Stock must be non-negative"] });
    }

    private static object ToWire(ProductWriteRequest product) => new
    {
        name = product.Name,
        vendor_id = product.VendorId,
        primary_price = product.Price,
        sku = product.Sku,
        barcode = product.Barcode,
        description = product.Description,
        stock = product.Stock,
        category_id = product.CategoryId
    };
    private static void ValidateId(int id)
    {
        if (id <= 0) throw new BasalamValidationError(new Dictionary<string, IReadOnlyList<string>>
            { ["id"] = ["Identifier must be positive"] });
    }

    private static string Query(params (string Name, string? Value)[] values)
    {
        var parts = values.Where(x => !string.IsNullOrWhiteSpace(x.Value))
            .Select(x => Uri.EscapeDataString(x.Name) + "=" + Uri.EscapeDataString(x.Value!));
        var query = string.Join('&', parts);
        return query.Length == 0 ? string.Empty : "?" + query;
    }
}
