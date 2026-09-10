namespace Hyper.Application.Features.Products.Services;

/// <summary>
/// سرویس محاسبه داینامیک CLV
/// </summary>
public interface IClvCalculationService
{
    /// <summary>
    /// محاسبه CLV محصول بر اساس داده‌های واقعی
    /// </summary>
    Task<decimal?> CalculateProductClvAsync(
        int productId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// محاسبه CLV مشتری-محصول بر اساس داده‌های واقعی
    /// </summary>
    Task<decimal?> CalculateCustomerProductClvAsync(
        int customerTenantId,
        int productId,
        CancellationToken cancellationToken = default);
}

