using Hyper.Domain.Entities.Products;

namespace Hyper.Application.Features.Products.Services;

/// <summary>
/// پیاده‌سازی ارائه‌دهنده قیمت محصول
/// </summary>
public class ProductPriceProvider : IProductPriceProvider
{
    private readonly IQueryRepository<Product, int> _productRepo;
    private readonly IChannelPriceService? _channelPriceService;
    private readonly ILogger<ProductPriceProvider> _logger;

    public ProductPriceProvider(
        IQueryRepository<Product, int> productRepo,
        IChannelPriceService? channelPriceService,
        ILogger<ProductPriceProvider> logger)
    {
        _productRepo = productRepo;
        _channelPriceService = channelPriceService;
        _logger = logger;
    }

    public async Task<decimal?> GetProductPriceAsync(
        int productId,
        string? customerId = null,
        CancellationToken cancellationToken = default)
    {
        var product = await _productRepo.FirstOrDefaultAsync(
            x => x.Id == productId, cancellationToken);

        if (product == null)
        {
            _logger.LogWarning("Product {ProductId} not found", productId);
            return null;
        }
        /*
        // اگر محصول تنظیم شده که قیمت را از کانال دریافت کند
        if (product.FetchPriceFromChannel && 
            !string.IsNullOrEmpty(product.PriceChannelKey) &&
            !string.IsNullOrEmpty(product.ChannelProductKey) &&
            _channelPriceService != null)
        {
            try
            {
                var price = await _channelPriceService.GetProductPriceFromChannelAsync(
                    product.PriceChannelKey,
                    product.ChannelProductKey,
                    customerId,
                    cancellationToken);

                if (price.HasValue)
                {
                    _logger.LogInformation(
                        "Fetched price {Price} from channel for product {ProductId}",
                        price.Value, productId);
                    return price;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error fetching price from channel for product {ProductId}",
                    productId);
            }
        }
        */
        // در غیر این صورت از قیمت ثابت استفاده کن
        return 0;// product.Price;
    }
}

