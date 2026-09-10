namespace Hyper.Application.Features.Products.Services;

/// <summary>
/// سرویس دریافت قیمت و CLV از کانال
/// </summary>
public interface IChannelPriceService
{
    /// <summary>
    /// دریافت قیمت محصول از کانال
    /// </summary>
    Task<decimal?> GetProductPriceFromChannelAsync(
        string channelKey, 
        string productKey, 
        string? customerId = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// دریافت CLV محصول از کانال
    /// </summary>
    Task<decimal?> GetProductClvFromChannelAsync(
        string channelKey,
        string productKey,
        string? customerId = null,
        CancellationToken cancellationToken = default);
}

