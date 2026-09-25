using Basalam.SDK.Auth;

namespace Basalam.SDK.Models;

public record Vendor
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Address { get; init; }
    public string? City { get; init; }
    public string? Province { get; init; }
    public string? PostalCode { get; init; }
    public string? NationalId { get; init; }
    public string? RegistrationNumber { get; init; }
    public string? EconomicCode { get; init; }
    public string? FiscalNumber { get; init; }
    public string? LogoUrl { get; init; }
    public string? CoverUrl { get; init; }
    public string? Description { get; init; }
    public string? Lat { get; init; }
    public string? Lng { get; init; }
    public string? ShopId { get; init; }
    public string? Status { get; init; }
    public DateTime? CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record Product
{
    public required int Id { get; init; }
    public required int VendorId { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public string? CategoryId { get; init; }
    public string? Brand { get; init; }
    public string? Model { get; init; }
    public string? Sku { get; init; }
    public string? Barcode { get; init; }
    public decimal? Price { get; init; }
    public decimal? SpecialPrice { get; init; }
    public int? Stock { get; init; }
    public int? MinStock { get; init; }
    public string? Unit { get; init; }
    public string? Weight { get; init; }
    public string? Dimensions { get; init; }
    public string? ImageUrl { get; init; }
    public string? GalleryJson { get; init; }
    public string? SpecsJson { get; init; }
    public string? Status { get; init; }
    public DateTime? CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record Variation
{
    public required int Id { get; init; }
    public required int ProductId { get; init; }
    public required int VendorId { get; init; }
    public string? Title { get; init; }
    public string? ImageUrl { get; init; }
    public decimal? Price { get; init; }
    public decimal? SpecialPrice { get; init; }
    public int? Stock { get; init; }
    public string? Sku { get; init; }
    public string? Barcode { get; init; }
    public string? Status { get; init; }
    public DateTime? CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record CatalogProduct
{
    public required int Id { get; init; }
    public required int VendorId { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public string? CategoryId { get; init; }
    public string? Brand { get; init; }
    public string? Model { get; init; }
    public string? Sku { get; init; }
    public string? Barcode { get; init; }
    public decimal? Price { get; init; }
    public decimal? SpecialPrice { get; init; }
    public int? Stock { get; init; }
    public string? Unit { get; init; }
    public string? Weight { get; init; }
    public string? Dimensions { get; init; }
    public string? ImageUrl { get; init; }
    public string? GalleryJson { get; init; }
    public string? SpecsJson { get; init; }
    public string? Status { get; init; }
    public DateTime? CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public List<Variation> Variants { get; init; } = new();
}

public record PageResult<T>
{
    public required List<T> Data { get; init; }
    public int Total { get; init; }
    public int Page { get; init; }
    public int PerPage { get; init; }
    public int TotalPages { get; init; }
    public bool HasMore { get; init; }
}
