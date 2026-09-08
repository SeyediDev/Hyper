namespace Hyper.CustomerPortal.Application.Interfaces;

/// <summary>
/// سرویس مدیریت محصولات
/// </summary>
public interface IProductService
{
    /// <summary>
    /// دریافت کاتالوگ محصولات با دسته‌بندی درختی
    /// </summary>
    Task<ProductCatalogDto> GetProductCatalogAsync(
        int? categoryId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت جزئیات محصول
    /// </summary>
    Task<ProductDto?> GetProductByIdAsync(int productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// ثبت خرید محصول
    /// </summary>
    Task<RegisterProductPurchaseResponse> RegisterProductPurchaseAsync(
        RegisterProductPurchaseRequest request,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// کاتالوگ محصولات با ساختار درختی
/// </summary>
public record ProductCatalogDto
{
    public List<ProductCategoryTreeDto> Categories { get; set; } = [];
    public List<ProductDto> Products { get; set; } = [];
}

/// <summary>
/// دسته‌بندی محصولات با ساختار درختی
/// </summary>
public record ProductCategoryTreeDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int? ParentCategoryId { get; set; }
    public int DisplayOrder { get; set; }
    public string? ImageUrl { get; set; }
    public List<ProductCategoryTreeDto> Children { get; set; } = [];
}

/// <summary>
/// اطلاعات محصول
/// </summary>
public record ProductDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryTitle { get; set; }
    public decimal? Price { get; set; }
    public long? PointsEarnable { get; set; }
    public bool RequiresSerialEntry { get; set; }
    public string? ImageUrl { get; set; }
    public ProductTypeDto ProductType { get; set; }
}

/// <summary>
/// نوع محصول
/// </summary>
public enum ProductTypeDto
{
    PhysicalProduct = 1,
    Service = 2,
    DigitalProduct = 3,
    CourseOrEvent = 4,
    ConsultingService = 5
}

/// <summary>
/// درخواست ثبت خرید محصول
/// </summary>
public record RegisterProductPurchaseRequest
{
    public int ProductId { get; set; }
    public string? SerialNumber { get; set; }
    public int Quantity { get; set; } = 1;
}

/// <summary>
/// پاسخ ثبت خرید محصول
/// </summary>
public record RegisterProductPurchaseResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public long? PointsAwarded { get; set; }
}