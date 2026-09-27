using System.Text.Json;
using Basalam.SDK.Clients;
using Basalam.SDK.Errors;

namespace Basalam.SDK.Services;

public interface IShippingService
{
    Task<JsonElement> GetProfilesAsync(int? page = null, int? perPage = null, int? vendorId = null, CancellationToken ct = default);
    Task<JsonElement> CreateProfileAsync(object request, CancellationToken ct = default);
    Task<JsonElement> GetProfileAsync(int profileId, CancellationToken ct = default);
    Task<JsonElement> UpdateProfileAsync(int profileId, object request, CancellationToken ct = default);
    Task<JsonElement> DeleteProfileAsync(int profileId, CancellationToken ct = default);
    Task<JsonElement> GetProfileZonesAsync(int profileId, int? page = null, int? perPage = null, CancellationToken ct = default);
    Task<JsonElement> CreateProfileZoneAsync(int profileId, object request, CancellationToken ct = default);
    Task<JsonElement> GetProfileUncoveredLocationsAsync(int profileId, CancellationToken ct = default);
    Task<JsonElement> GetProfileProductsAsync(int profileId, ShippingProfileProductsQuery? query = null, CancellationToken ct = default);
    Task<JsonElement> CreateProfileProductsAsync(int profileId, object request, CancellationToken ct = default);
    Task<JsonElement> GetProfileProductsListAsync(ShippingProfileProductsListQuery? query = null, CancellationToken ct = default);
    Task<JsonElement> DeleteProfileProductAsync(int productId, CancellationToken ct = default);
    Task<JsonElement> GetFreeShippingRulesAsync(int productId, CancellationToken ct = default);
    Task<JsonElement> UpdateFreeShippingRulesAsync(object request, CancellationToken ct = default);
    Task<JsonElement> GetProductShippingInfoAsync(int productId, CancellationToken ct = default);
    Task<JsonElement> GetZoneAsync(int zoneId, CancellationToken ct = default);
    Task<JsonElement> UpdateZoneAsync(int zoneId, object request, CancellationToken ct = default);
    Task<JsonElement> DeleteZoneAsync(int zoneId, CancellationToken ct = default);
    Task<JsonElement> CreateZoneCarrierRateAsync(int zoneId, object request, CancellationToken ct = default);
    Task<JsonElement> SetZoneCarrierRatesAsync(int zoneId, object request, CancellationToken ct = default);
    Task<JsonElement> GetZoneCarrierRatesAsync(int zoneId, int? page = null, int? perPage = null, CancellationToken ct = default);
    Task<JsonElement> CreateZoneOwnRatesAsync(int zoneId, object request, CancellationToken ct = default);
    Task<JsonElement> GetZoneOwnRatesAsync(int zoneId, int? page = null, int? perPage = null, CancellationToken ct = default);
    Task<JsonElement> GetDeliveryEstimatesAsync(int? vendorId = null, CancellationToken ct = default);
    Task<JsonElement> GetOwnRateAsync(int ownRateId, CancellationToken ct = default);
    Task<JsonElement> UpdateOwnRateAsync(int ownRateId, object request, CancellationToken ct = default);
    Task<JsonElement> DeleteOwnRateAsync(int ownRateId, CancellationToken ct = default);
    Task<JsonElement> GetCarriersAsync(CancellationToken ct = default);
    Task<JsonElement> GetVendorCarriersAsync(int? status = null, int? vendorId = null, string? prefer = null, CancellationToken ct = default);
    Task<JsonElement> GetCarrierRateAsync(int carrierRateId, CancellationToken ct = default);
    Task<JsonElement> UpdateCarrierRateAsync(int carrierRateId, object request, CancellationToken ct = default);
    Task<JsonElement> DeleteCarrierRateAsync(int carrierRateId, CancellationToken ct = default);
    Task<JsonElement> GetLocationsAsync(CancellationToken ct = default);
    Task<JsonElement> GetProfileStrategyAsync(int? vendorId = null, CancellationToken ct = default);
    Task<JsonElement> SetProfileStrategyAsync(object request, CancellationToken ct = default);
}

public sealed record ShippingProfileProductsQuery(string? ProductTitleLike = null,
    string? NeverFreeZoneIds = null, string? ConditionalZoneIds = null, string? Sort = null,
    int? PerPage = null, string? Cursor = null);

public sealed record ShippingProfileProductsListQuery(int? ProfileIdEq = null, int? ProfileIdNe = null,
    string? ProductTitleLike = null, string? Sort = null, int? PerPage = null,
    int? VendorId = null, string? Cursor = null);

public sealed class ShippingService(IBasalamHttpClient client, ILogger<ShippingService>? logger = null) : IShippingService
{
    public Task<JsonElement> GetProfilesAsync(int? page = null, int? perPage = null, int? vendorId = null, CancellationToken ct = default) =>
        Get("/v1/shipping/profiles" + Query(("page", page), ("per_page", perPage), ("vendor_id", vendorId)), ct);

    public Task<JsonElement> CreateProfileAsync(object request, CancellationToken ct = default) => Post("/v1/shipping/profiles", request, ct);
    public Task<JsonElement> GetProfileAsync(int profileId, CancellationToken ct = default) => Get($"/v1/shipping/profiles/{Id(profileId, "profileId")}", ct);
    public Task<JsonElement> UpdateProfileAsync(int profileId, object request, CancellationToken ct = default) => Patch($"/v1/shipping/profiles/{Id(profileId, "profileId")}", request, ct);
    public Task<JsonElement> DeleteProfileAsync(int profileId, CancellationToken ct = default) => Delete($"/v1/shipping/profiles/{Id(profileId, "profileId")}", ct);
    public Task<JsonElement> GetProfileZonesAsync(int profileId, int? page = null, int? perPage = null, CancellationToken ct = default) =>
        Get($"/v1/shipping/profiles/{Id(profileId, "profileId")}/zones" + Query(("page", page), ("per_page", perPage)), ct);
    public Task<JsonElement> CreateProfileZoneAsync(int profileId, object request, CancellationToken ct = default) =>
        Post($"/v1/shipping/profiles/{Id(profileId, "profileId")}/zones", request, ct);
    public Task<JsonElement> GetProfileUncoveredLocationsAsync(int profileId, CancellationToken ct = default) =>
        Get($"/v1/shipping/profiles/{Id(profileId, "profileId")}/zones/unrecovered-locations", ct);

    public Task<JsonElement> GetProfileProductsAsync(int profileId, ShippingProfileProductsQuery? query = null, CancellationToken ct = default)
    {
        var q = query ?? new ShippingProfileProductsQuery();
        return Get($"/v1/shipping/profiles/{Id(profileId, "profileId")}/products" + Query(
            ("product.title[like]", q.ProductTitleLike), ("never_free_zone_ids", q.NeverFreeZoneIds),
            ("conditional_zone_ids", q.ConditionalZoneIds), ("sort", q.Sort), ("per_page", q.PerPage), ("cursor", q.Cursor)), ct);
    }

    public Task<JsonElement> CreateProfileProductsAsync(int profileId, object request, CancellationToken ct = default) =>
        Post($"/v1/shipping/profiles/{Id(profileId, "profileId")}/products", request, ct);
    public Task<JsonElement> GetProfileProductsListAsync(ShippingProfileProductsListQuery? query = null, CancellationToken ct = default)
    {
        var q = query ?? new ShippingProfileProductsListQuery();
        return Get("/v1/shipping/profile-products" + Query(
            ("profile_id[eq]", q.ProfileIdEq), ("profile_id[ne]", q.ProfileIdNe),
            ("product.title[like]", q.ProductTitleLike), ("sort", q.Sort), ("per_page", q.PerPage),
            ("vendor_id", q.VendorId), ("cursor", q.Cursor)), ct);
    }

    public Task<JsonElement> DeleteProfileProductAsync(int productId, CancellationToken ct = default) =>
        Delete($"/v1/shipping/profile-products/{Id(productId, "productId")}", ct);
    public Task<JsonElement> GetFreeShippingRulesAsync(int productId, CancellationToken ct = default) =>
        Get($"/v1/shipping/profile-products/{Id(productId, "productId")}/free-shipping-rules", ct);
    public Task<JsonElement> UpdateFreeShippingRulesAsync(object request, CancellationToken ct = default) =>
        Patch("/v1/shipping/profile-products/free-shipping-rules", request, ct);
    public Task<JsonElement> GetProductShippingInfoAsync(int productId, CancellationToken ct = default) =>
        Get($"/v1/shipping/profile-products/{Id(productId, "productId")}/shipping-info", ct);

    public Task<JsonElement> GetZoneAsync(int zoneId, CancellationToken ct = default) => Get($"/v1/shipping/zones/{Id(zoneId, "zoneId")}", ct);
    public Task<JsonElement> UpdateZoneAsync(int zoneId, object request, CancellationToken ct = default) => Patch($"/v1/shipping/zones/{Id(zoneId, "zoneId")}", request, ct);
    public Task<JsonElement> DeleteZoneAsync(int zoneId, CancellationToken ct = default) => Delete($"/v1/shipping/zones/{Id(zoneId, "zoneId")}", ct);
    public Task<JsonElement> CreateZoneCarrierRateAsync(int zoneId, object request, CancellationToken ct = default) => Post($"/v1/shipping/zones/{Id(zoneId, "zoneId")}/carrier-rates", request, ct);
    public Task<JsonElement> SetZoneCarrierRatesAsync(int zoneId, object request, CancellationToken ct = default) => Put($"/v1/shipping/zones/{Id(zoneId, "zoneId")}/carrier-rates", request, ct);
    public Task<JsonElement> GetZoneCarrierRatesAsync(int zoneId, int? page = null, int? perPage = null, CancellationToken ct = default) =>
        Get($"/v1/shipping/zones/{Id(zoneId, "zoneId")}/carrier-rates" + Query(("page", page), ("per_page", perPage)), ct);
    public Task<JsonElement> CreateZoneOwnRatesAsync(int zoneId, object request, CancellationToken ct = default) => Post($"/v1/shipping/zones/{Id(zoneId, "zoneId")}/own-rates", request, ct);
    public Task<JsonElement> GetZoneOwnRatesAsync(int zoneId, int? page = null, int? perPage = null, CancellationToken ct = default) =>
        Get($"/v1/shipping/zones/{Id(zoneId, "zoneId")}/own-rates" + Query(("page", page), ("per_page", perPage)), ct);
    public Task<JsonElement> GetDeliveryEstimatesAsync(int? vendorId = null, CancellationToken ct = default) => Get("/v1/shipping/own-rates/delivery-estimates" + Query(("vendor_id", vendorId)), ct);
    public Task<JsonElement> GetOwnRateAsync(int ownRateId, CancellationToken ct = default) => Get($"/v1/shipping/own-rates/{Id(ownRateId, "ownRateId")}", ct);
    public Task<JsonElement> UpdateOwnRateAsync(int ownRateId, object request, CancellationToken ct = default) => Put($"/v1/shipping/own-rates/{Id(ownRateId, "ownRateId")}", request, ct);
    public Task<JsonElement> DeleteOwnRateAsync(int ownRateId, CancellationToken ct = default) => Delete($"/v1/shipping/own-rates/{Id(ownRateId, "ownRateId")}", ct);
    public Task<JsonElement> GetCarriersAsync(CancellationToken ct = default) => Get("/v1/shipping/carriers", ct);
    public Task<JsonElement> GetVendorCarriersAsync(int? status = null, int? vendorId = null, string? prefer = null, CancellationToken ct = default) =>
        Get("/v1/shipping/vendor-carriers" + Query(("status", status), ("vendor_id", vendorId), ("prefer", prefer)), ct);
    public Task<JsonElement> GetCarrierRateAsync(int carrierRateId, CancellationToken ct = default) => Get($"/v1/shipping/carrier-rates/{Id(carrierRateId, "carrierRateId")}", ct);
    public Task<JsonElement> UpdateCarrierRateAsync(int carrierRateId, object request, CancellationToken ct = default) => Patch($"/v1/shipping/carrier-rates/{Id(carrierRateId, "carrierRateId")}", request, ct);
    public Task<JsonElement> DeleteCarrierRateAsync(int carrierRateId, CancellationToken ct = default) => Delete($"/v1/shipping/carrier-rates/{Id(carrierRateId, "carrierRateId")}", ct);
    public Task<JsonElement> GetLocationsAsync(CancellationToken ct = default) => Get("/v1/shipping/locations", ct);
    public Task<JsonElement> GetProfileStrategyAsync(int? vendorId = null, CancellationToken ct = default) => Get("/v1/shipping/profile-strategy" + Query(("vendor_id", vendorId)), ct);
    public Task<JsonElement> SetProfileStrategyAsync(object request, CancellationToken ct = default) => Put("/v1/shipping/profile-strategy", request, ct);

    private Task<JsonElement> Get(string path, CancellationToken ct) => client.GetAsync<JsonElement>(path, ct);
    private Task<JsonElement> Post(string path, object request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);
        return client.PostAsync<JsonElement>(path, request, ct);
    }
    private Task<JsonElement> Put(string path, object request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);
        return client.PutAsync<JsonElement>(path, request, ct);
    }
    private Task<JsonElement> Patch(string path, object request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);
        return client.PatchAsync<JsonElement>(path, request, ct);
    }
    private Task<JsonElement> Delete(string path, CancellationToken ct) => client.DeleteAsync<JsonElement>(path, ct);
    private static int Id(int value, string field)
    {
        if (value <= 0) throw new BasalamValidationError(new Dictionary<string, IReadOnlyList<string>>
        { [field] = ["Identifier must be positive"] });
        return value;
    }
    private static string Query(params (string Name, object? Value)[] values)
    {
        var parts = values.Where(x => x.Value is not null && (!string.IsNullOrWhiteSpace(x.Value.ToString())))
            .Select(x => Uri.EscapeDataString(x.Name) + "=" + Uri.EscapeDataString(x.Value!.ToString()!));
        var query = string.Join('&', parts);
        return query.Length == 0 ? string.Empty : "?" + query;
    }
}
