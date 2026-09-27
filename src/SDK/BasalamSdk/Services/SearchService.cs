using System.Text.Json;
using Basalam.SDK.Clients;
using Basalam.SDK.Errors;

namespace Basalam.SDK.Services;

public interface ISearchService
{
    Task<JsonElement> SearchProductsAsync(ProductSearchRequest request, CancellationToken ct = default);
}

public sealed record ProductSearchFilters(int? FreeShipping = null, string? Slug = null,
    string? VendorIdentifier = null, int? MaxPrice = null, int? MinPrice = null,
    int? SameCity = null, int? MinRating = null, int? VendorScore = null);

public sealed record ProductSearchRequest(ProductSearchFilters? Filters = null, string? Query = null,
    int? Rows = null, int? Start = null);

public sealed class SearchService(IBasalamHttpClient client) : ISearchService
{
    public Task<JsonElement> SearchProductsAsync(ProductSearchRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.Rows is < 1 or > 1000 || request.Start is < 0)
            throw new BasalamValidationError(new Dictionary<string, IReadOnlyList<string>>
            { ["pagination"] = ["Rows must be between 1 and 1000 and start cannot be negative"] });
        return client.PostAsync<JsonElement>("/v1/products/search", new
        {
            filters = request.Filters is null ? null : new
            {
                freeShipping = request.Filters.FreeShipping,
                slug = request.Filters.Slug,
                vendorIdentifier = request.Filters.VendorIdentifier,
                maxPrice = request.Filters.MaxPrice,
                minPrice = request.Filters.MinPrice,
                sameCity = request.Filters.SameCity,
                minRating = request.Filters.MinRating,
                vendorScore = request.Filters.VendorScore
            },
            q = request.Query,
            rows = request.Rows,
            start = request.Start
        }, ct);
    }
}
