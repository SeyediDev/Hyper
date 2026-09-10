using Hyper.Domain.Entities.Products;

namespace Hyper.Application.Features.Products.Services;

/// <summary>
/// پیاده‌سازی سرویس محاسبه داینامیک CLV
/// </summary>
public class ClvCalculationService : IClvCalculationService
{
    private readonly IQueryRepository<CustomerProductMetrics, int> _customerProductMetricsRepo;
    private readonly IQueryRepository<Product, int> _productRepo;
    private readonly ILogger<ClvCalculationService> _logger;

    public ClvCalculationService(
        IQueryRepository<CustomerProductMetrics, int> customerProductMetricsRepo,
        IQueryRepository<Product, int> productRepo,
        ILogger<ClvCalculationService> logger)
    {
        _customerProductMetricsRepo = customerProductMetricsRepo;
        _productRepo = productRepo;
        _logger = logger;
    }

    public async Task<decimal?> CalculateProductClvAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        var product = await _productRepo.FirstOrDefaultAsync(
            x => x.Id == productId, cancellationToken);
        
        if (product == null)
        {
            _logger.LogWarning("Product {ProductId} not found for CLV calculation", productId);
            return null;
        }

        // محاسبه CLV بر اساس میانگین CLV مشتریان
        var metrics = await _customerProductMetricsRepo.GetAllAsync(
            cancellationToken,
            x => x.ProductId == productId && x.CustomerLifetimeValue.HasValue);

        if (!metrics.Any())
        {
            _logger.LogInformation("No customer metrics found for product {ProductId} to calculate CLV", productId);
            return null;
        }

        var averageClv = metrics.Average(x => x.CustomerLifetimeValue!.Value);
        _logger.LogInformation("Calculated average CLV {Clv} for product {ProductId}", averageClv, productId);
        return averageClv;
    }

    public async Task<decimal?> CalculateCustomerProductClvAsync(
        int customerTenantId,
        int productId,
        CancellationToken cancellationToken = default)
    {
        // فرمول CLV: (Average Order Value × Purchase Frequency × Customer Lifespan) - CAC
        var metrics = await _customerProductMetricsRepo.FirstOrDefaultAsync(
            x => x.CustomerTenantId == customerTenantId && x.ProductId == productId,
            cancellationToken);

        if (metrics == null)
        {
            _logger.LogInformation(
                "No metrics found for customer {CustomerTenantId} and product {ProductId}",
                customerTenantId, productId);
            return null;
        }

        // اگر قبلاً محاسبه شده، از آن استفاده کن
        if (metrics.CustomerLifetimeValue.HasValue)
        {
            return metrics.CustomerLifetimeValue;
        }

        // محاسبه بر اساس داده‌های موجود
        decimal? clv = null;
        
        if (metrics.AverageOrderValue.HasValue && 
            metrics.PurchaseFrequency.HasValue && 
            metrics.AverageTimeBetweenPurchasesDays.HasValue)
        {
            // تخمین طول عمر مشتری بر اساس داده‌های تاریخی
            var customerLifespan = metrics.AverageTimeBetweenPurchasesDays.Value > 0
                ? 365.0m / metrics.AverageTimeBetweenPurchasesDays.Value
                : 1.0m;

            clv = metrics.AverageOrderValue.Value * 
                  metrics.PurchaseFrequency.Value * 
                  customerLifespan;

            // کسر هزینه جذب (در صورت وجود)
            if (metrics.CustomerAcquisitionCost.HasValue)
            {
                clv -= metrics.CustomerAcquisitionCost.Value;
            }

            _logger.LogInformation(
                "Calculated CLV {Clv} for customer {CustomerTenantId} and product {ProductId}",
                clv, customerTenantId, productId);
        }
        else
        {
            _logger.LogWarning(
                "Insufficient data to calculate CLV for customer {CustomerTenantId} and product {ProductId}",
                customerTenantId, productId);
        }

        return clv;
    }
}

