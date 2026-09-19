using Hyper.SDK.Auth;
using Hyper.SDK.Errors;
using Hyper.SDK.Clients;
using Hyper.SDK.Models;
using Microsoft.Extensions.Logging;

namespace Hyper.SDK.Services;

public interface IVendorService
{
    Task<Vendor?> GetVendorAsync(int vendorId, CancellationToken ct = default);
    Task<PageResult<Vendor>> GetVendorsAsync(int page = 1, int perPage = 100, CancellationToken ct = default);
}

public sealed class VendorService : IVendorService
{
    private readonly IBasalamHttpClient _client;
    private readonly ILogger<VendorService>? _logger;

    public VendorService(IBasalamHttpClient client, ILogger<VendorService>? logger = null)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<Vendor?> GetVendorAsync(int vendorId, CancellationToken ct = default)
    {
        _logger?.LogInformation("Getting vendor {VendorId}", vendorId);
        return await _client.GetAsync<Vendor>($"/v1/vendors/{vendorId}", ct);
    }

    public async Task<PageResult<Vendor>> GetVendorsAsync(int page = 1, int perPage = 100, CancellationToken ct = default)
    {
        _logger?.LogInformation("Getting vendors page {Page}", page);
        var url = $"/v1/vendors?page={page}&per_page={perPage}&sort=id:asc";
        return await _client.GetAsync<PageResult<Vendor>>(url, ct) ?? new PageResult<Vendor> { Data = new(), Total = 0, Page = page, PerPage = perPage, TotalPages = 0, HasMore = false };
    }
}

public interface IProductService
{
    Task<CatalogProduct?> GetProductAsync(int productId, CancellationToken ct = default);
    Task<CatalogProduct?> GetProductBySkuAsync(string sku, CancellationToken ct = default);
    Task PatchStockAsync(int productId, int stock, CancellationToken ct = default);
    Task<PageResult<CatalogProduct>> GetProductsAsync(int vendorId, int page = 1, int perPage = 100, CancellationToken ct = default);
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

    public async Task<CatalogProduct?> GetProductAsync(int productId, CancellationToken ct = default)
    {
        _logger?.LogInformation("Getting product {ProductId}", productId);
        return await _client.GetAsync<CatalogProduct>($"/v1/products/{productId}", ct);
    }

    public async Task<CatalogProduct?> GetProductBySkuAsync(string sku, CancellationToken ct = default)
    {
        _logger?.LogInformation("Getting product by SKU {Sku}", sku);
        return await _client.GetAsync<CatalogProduct>($"/v1/products?sku={System.Uri.EscapeDataString(sku)}", ct);
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

    public async Task<PageResult<CatalogProduct>> GetProductsAsync(int vendorId, int page = 1, int perPage = 100, CancellationToken ct = default)
    {
        _logger?.LogInformation("Getting products for vendor {VendorId}", vendorId);
        var url = $"/v1/vendors/{vendorId}/products?page={page}&per_page={perPage}&variants_flatting=false&sort=id:asc";
        return await _client.GetAsync<PageResult<CatalogProduct>>(url, ct) ?? new PageResult<CatalogProduct> { Data = new(), Total = 0, Page = page, PerPage = perPage, TotalPages = 0, HasMore = false };
    }
}

public interface IVariationService
{
    Task<Variation?> GetVariationAsync(int variationId, CancellationToken ct = default);
    Task PatchStockAsync(int variationId, int stock, CancellationToken ct = default);
}

public sealed class VariationService : IVariationService
{
    private readonly IBasalamHttpClient _client;
    private readonly ILogger<VariationService>? _logger;

    public VariationService(IBasalamHttpClient client, ILogger<VariationService>? logger = null)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<Variation?> GetVariationAsync(int variationId, CancellationToken ct = default)
    {
        _logger?.LogInformation("Getting variation {VariationId}", variationId);
        return await _client.GetAsync<Variation>($"/v1/variations/{variationId}", ct);
    }

    public async Task PatchStockAsync(int variationId, int stock, CancellationToken ct = default)
    {
        _logger?.LogInformation("Patching stock for variation {VariationId} to {Stock}", variationId, stock);
        if (stock < 0)
            throw new BasalamValidationError(new Dictionary<string, IReadOnlyList<string>>
            {
                ["stock"] = ["Stock must be non-negative"]
            });
        await _client.PatchAsync<object>($"/v1/variations/{variationId}", new { stock }, ct);
    }
}

public interface ICatalogService
{
    Task<PageResult<CatalogProduct>> GetProductsAsync(int vendorId, int page = 1, int perPage = 100, CancellationToken ct = default);
    Task<CatalogProduct?> GetProductAsync(int productId, CancellationToken ct = default);
}

public sealed class CatalogService : ICatalogService
{
    private readonly IBasalamHttpClient _client;
    private readonly ILogger<CatalogService>? _logger;

    public CatalogService(IBasalamHttpClient client, ILogger<CatalogService>? logger = null)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<PageResult<CatalogProduct>> GetProductsAsync(int vendorId, int page = 1, int perPage = 100, CancellationToken ct = default)
    {
        _logger?.LogInformation("Getting catalog products for vendor {VendorId}", vendorId);
        var url = $"/v1/vendors/{vendorId}/products?page={page}&per_page={perPage}&variants_flatting=false&sort=id:asc";
        return await _client.GetAsync<PageResult<CatalogProduct>>(url, ct) ?? new PageResult<CatalogProduct> { Data = new(), Total = 0, Page = page, PerPage = perPage, TotalPages = 0, HasMore = false };
    }

    public async Task<CatalogProduct?> GetProductAsync(int productId, CancellationToken ct = default)
    {
        _logger?.LogInformation("Getting catalog product {ProductId}", productId);
        return await _client.GetAsync<CatalogProduct>($"/v1/products/{productId}", ct);
    }
}
