namespace Hyper.Domain.Features;

/// <summary>
/// ارائه‌دهنده قیمت محصول - برای دریافت قیمت از منابع مختلف (کانال، دیتابیس، و غیره)
/// </summary>
public interface IProductPriceProvider
{
    /// <summary>
    /// دریافت قیمت محصول
    /// </summary>
    Task<decimal?> GetProductPriceAsync(
        int productId,
        string? customerId = null,
        CancellationToken cancellationToken = default);
}

