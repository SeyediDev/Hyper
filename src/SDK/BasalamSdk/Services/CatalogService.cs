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
        var root = await client.GetAsync<JsonElement>(url, ct);
        if (root.ValueKind != JsonValueKind.Object || !root.TryGetProperty("data", out var data)
            || data.ValueKind != JsonValueKind.Array) throw new JsonException("Invalid catalog page");
        var products = data.EnumerateArray().Select(item => ReadProduct(item, vendorId)).ToList();
        if (products.Any(product => product.VendorId != vendorId)) throw new JsonException("Catalog vendor mismatch");
        var currentPage = Integer(root, "page") ?? page;
        var totalPages = Integer(root, "total_page", "totalPages");
        var hasMore = totalPages.HasValue ? currentPage < totalPages.Value
            : root.TryGetProperty("hasMore", out var more) && more.ValueKind is JsonValueKind.True or JsonValueKind.False
                ? more.GetBoolean() : products.Count != 0;
        return new PageResult<CatalogProduct>
        {
            Data = products, Total = Integer(root, "total_count", "total") ?? products.Count,
            Page = currentPage, PerPage = Integer(root, "per_page", "perPage") ?? perPage,
            TotalPages = totalPages ?? 0, HasMore = products.Count != 0 && hasMore
        };
    }

    public async Task<CatalogProduct?> GetProductAsync(int productId, CancellationToken ct = default)
    {
        logger?.LogInformation("Getting catalog product {ProductId}", productId);
        var root = await client.GetAsync<JsonElement>($"/v1/products/{productId}", ct);
        var product = ReadProduct(root);
        if (product.Id != productId) throw new JsonException("Product identity mismatch");
        return product;
    }

    // Adapt the official wire shape without changing the SDK's public DTOs.
    // In particular: title, inventory, vendor.id and snake_case pagination.
    private static CatalogProduct ReadProduct(JsonElement root, int? requestedVendor = null)
    {
        if (root.ValueKind != JsonValueKind.Object) throw new JsonException("Invalid product");
        var id = Integer(root, "id") ?? throw new JsonException("Missing product identity");
        var vendor = root.TryGetProperty("vendor", out var nested) && nested.ValueKind == JsonValueKind.Object
            ? Integer(nested, "id") : Integer(root, "vendor_id", "vendorId") ?? requestedVendor;
        if (id <= 0 || vendor is null or <= 0) throw new JsonException("Invalid product ownership");
        var title = Text(root, "title", "name");
        if (string.IsNullOrWhiteSpace(title)) throw new JsonException("Missing product title");
        var variations = new List<Variation>();
        if (root.TryGetProperty("variants", out var variants) && variants.ValueKind != JsonValueKind.Null)
        {
            if (variants.ValueKind != JsonValueKind.Array) throw new JsonException("Invalid variants");
            foreach (var variant in variants.EnumerateArray())
            {
                var variantId = Integer(variant, "id");
                if (variantId is null or <= 0) throw new JsonException("Invalid variant identity");
                variations.Add(new Variation { Id = variantId.Value, ProductId = id, VendorId = vendor.Value,
                    Title = Text(variant, "title"), Sku = Text(variant, "sku"),
                    Price = Decimal(variant, "price"), Stock = Integer(variant, "stock", "inventory") });
            }
        }
        return new CatalogProduct { Id = id, VendorId = vendor.Value, Name = title,
            Sku = Text(root, "sku"), Price = Decimal(root, "price"), Stock = Integer(root, "inventory", "stock"),
            Description = Text(root, "description"), Variants = variations };
    }

    private static JsonElement? Value(JsonElement root, params string[] names)
    {
        if (root.ValueKind != JsonValueKind.Object) throw new JsonException("Expected object");
        foreach (var name in names)
            if (root.TryGetProperty(name, out var value) && value.ValueKind != JsonValueKind.Null) return value;
        return null;
    }
    private static int? Integer(JsonElement root, params string[] names) => Value(root, names) is { } value
        ? value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var number) ? number : throw new JsonException("Invalid integer field") : null;
    private static decimal? Decimal(JsonElement root, params string[] names) => Value(root, names) is { } value
        ? value.ValueKind == JsonValueKind.Number && value.TryGetDecimal(out var number) ? number : throw new JsonException("Invalid price field") : null;
    private static string? Text(JsonElement root, params string[] names) => Value(root, names) is { } value
        ? value.ValueKind == JsonValueKind.String ? value.GetString() : throw new JsonException("Invalid text field") : null;
}
