using Basalam.SDK.Auth;
using Basalam.SDK.Clients;
using Basalam.SDK.Config;
using Basalam.SDK.Errors;
using Basalam.SDK.Models;
using Microsoft.Extensions.Logging;

namespace Basalam.SDK.Services;

public interface ICatalogService
{
    Task<PageResult<CatalogProduct>> GetProductsAsync(int vendorId, int page = 1, int perPage = 100, CancellationToken ct = default);
    Task<CatalogProduct?> GetProductAsync(int productId, CancellationToken ct = default);
}

public sealed class CatalogService(IBasalamHttpClient client, ILogger<CatalogService>? logger = null) : ICatalogService
{
    public async Task<PageResult<CatalogProduct>> GetProductsAsync(int vendorId, int page = 1, int perPage = 100, CancellationToken ct = default)
    {
        logger?.LogInformation("Getting catalog products for vendor {VendorId}", vendorId);
        var url = $"/v1/vendors/{vendorId}/products?page={page}&per_page={perPage}&variants_flatting=false&sort=id:asc";
        return await client.GetAsync<PageResult<CatalogProduct>>(url, ct)
            ?? new PageResult<CatalogProduct> { Data = new(), Total = 0, Page = page, PerPage = perPage, TotalPages = 0, HasMore = false };
    }

    public async Task<CatalogProduct?> GetProductAsync(int productId, CancellationToken ct = default)
    {
        logger?.LogInformation("Getting catalog product {ProductId}", productId);
        return await client.GetAsync<CatalogProduct>($"/v1/products/{productId}", ct);
    }
}
