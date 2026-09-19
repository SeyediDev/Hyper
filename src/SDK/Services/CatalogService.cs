using Hyper.SDK.Auth;
using Hyper.SDK.Clients;
using Hyper.SDK.Config;
using Hyper.SDK.Errors;
using Hyper.SDK.Models;
using Microsoft.Extensions.Logging;

namespace Hyper.SDK.Services;

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
        return await _client.GetAsync<PageResult<CatalogProduct>>(url, ct)
            ?? new PageResult<CatalogProduct> { Data = new(), Total = 0, Page = page, PerPage = perPage, TotalPages = 0, HasMore = false };
    }

    public async Task<CatalogProduct?> GetProductAsync(int productId, CancellationToken ct = default)
    {
        _logger?.LogInformation("Getting catalog product {ProductId}", productId);
        return await _client.GetAsync<CatalogProduct>($"/v1/products/{productId}", ct);
    }
}
