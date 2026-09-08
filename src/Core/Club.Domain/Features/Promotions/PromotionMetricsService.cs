namespace Hyper.Domain.Features.Promotions;

/// <summary>
/// سرویس محاسبه و به‌روزرسانی معیارهای عملکرد پویش
/// </summary>
public interface IPromotionMetricsService
{
    /// <summary>
    /// محاسبه CAC با استفاده از تسهیم هزینه
    /// </summary>
    /// <param name="promotionId">شناسه پویش</param>
    /// <param name="customerAcquisitionDate">تاریخ جذب مشتری</param>
    /// <returns>هزینه جذب مشتری</returns>
    Task<decimal?> CalculateCACWithAllocationAsync(int promotionId, DateTime customerAcquisitionDate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// به‌روزرسانی معیارهای عملکرد پویش
    /// </summary>
    Task UpdatePromotionMetricsAsync(int promotionId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// محاسبه و به‌روزرسانی AllocatedCost برای همه بازه‌های تسهیم هزینه
    /// </summary>
    Task UpdateAllocatedCostsAsync(int promotionId, decimal totalCost, CancellationToken cancellationToken = default);
}

// Stub implementation until repository methods are updated
internal class PromotionMetricsService : IPromotionMetricsService
{
    public Task<decimal?> CalculateCACWithAllocationAsync(int promotionId, DateTime customerAcquisitionDate, CancellationToken cancellationToken = default)
        => Task.FromResult<decimal?>(null);
    
    public Task UpdatePromotionMetricsAsync(int promotionId, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
    
    public Task UpdateAllocatedCostsAsync(int promotionId, decimal totalCost, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}

#if false
// Original implementation - needs repository API fixes

internal class PromotionMetricsService_Original(
    ILogger<PromotionMetricsService> logger,
    IQueryRepository<Promotion, int> promotionRepo,
    IQueryRepository<PromotionCostAllocation, int> costAllocationRepo,
    IQueryRepository<PromotionMetrics, int> metricsQueryRepo,
    ICommandRepository<PromotionMetrics, int> metricsCommandRepo,
    ICommandRepository<PromotionCostAllocation, int> costAllocationCommandRepo,
    IQueryRepository<PromotionParticipation, int> participationRepo
) : IPromotionMetricsService
{
    /// <summary>
    /// محاسبه CAC با استفاده از تسهیم هزینه
    /// اگر مشتری در بازه‌ای خاص جذب شده، فقط درصد هزینه آن بازه برای او محاسبه می‌شود
    /// </summary>
    public async Task<decimal?> CalculateCACWithAllocationAsync(
        int promotionId, 
        DateTime customerAcquisitionDate,
        CancellationToken cancellationToken = default)
    {
        // دریافت بازه‌های تسهیم هزینه
        var allocations = await costAllocationRepo.Query()
            .Where(a => a.PromotionId == promotionId)
            .OrderBy(a => a.Order)
            .ToListAsync(cancellationToken);
        
        if (allocations.Count == 0)
        {
            logger.LogDebug("پویش {PromotionId} بازه تسهیم هزینه ندارد - از هزینه کل استفاده می‌شود", promotionId);
            return await CalculateSimpleCACAsync(promotionId, cancellationToken);
        }
        
        // پیدا کردن بازه مناسب برای تاریخ جذب مشتری
        var matchingAllocation = allocations.FirstOrDefault(a => 
            customerAcquisitionDate >= a.FromDate && 
            customerAcquisitionDate <= a.ToDate);
        
        if (matchingAllocation == null)
        {
            logger.LogWarning("تاریخ جذب مشتری {Date} در هیچ بازه تسهیم هزینه‌ای قرار ندارد برای پویش {PromotionId}",
                customerAcquisitionDate, promotionId);
            return await CalculateSimpleCACAsync(promotionId, cancellationToken);
        }
        
        // محاسبه CAC بر اساس هزینه تخصیص یافته به بازه
        var allocatedCost = matchingAllocation.AllocatedCost;
        if (!allocatedCost.HasValue || allocatedCost.Value == 0)
        {
            logger.LogWarning("هزینه تخصیص یافته برای بازه {AllocationTitle} صفر است", matchingAllocation.Title);
            return null;
        }
        
        // تعداد مشتریان جذب شده در این بازه
        var customersInPeriod = await participationRepo.Query()
            .Where(p => p.PromotionId == promotionId && 
                        p.CompletedDate.HasValue &&
                        p.CompletedDate.Value >= matchingAllocation.FromDate &&
                        p.CompletedDate.Value <= matchingAllocation.ToDate)
            .CountAsync(cancellationToken);
        
        if (customersInPeriod == 0)
        {
            logger.LogWarning("هیچ مشتری‌ای در بازه {AllocationTitle} جذب نشده", matchingAllocation.Title);
            return null;
        }
        
        var cac = allocatedCost.Value / customersInPeriod;
        
        logger.LogInformation(
            "CAC محاسبه شد برای پویش {PromotionId} در بازه {AllocationTitle}: {CAC} (هزینه: {Cost}, مشتریان: {Customers})",
            promotionId, matchingAllocation.Title, cac, allocatedCost.Value, customersInPeriod);
        
        return cac;
    }
    
    /// <summary>
    /// محاسبه ساده CAC (بدون تسهیم هزینه)
    /// </summary>
    private async Task<decimal?> CalculateSimpleCACAsync(int promotionId, CancellationToken cancellationToken)
    {
        var metrics = await metricsQueryRepo.Query()
            .Where(m => m.PromotionId == promotionId)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (metrics?.CampaignCost == null || metrics.CampaignCost == 0)
            return null;
        
        if (metrics.NewCustomersAcquired == null || metrics.NewCustomersAcquired == 0)
            return null;
        
        return metrics.CampaignCost.Value / metrics.NewCustomersAcquired.Value;
    }
    
    /// <summary>
    /// به‌روزرسانی معیارهای عملکرد پویش
    /// </summary>
    public async Task UpdatePromotionMetricsAsync(int promotionId, CancellationToken cancellationToken = default)
    {
        var promotion = await promotionRepo.Query()
            .Include(p => p.Metrics)
            .Include(p => p.CostAllocations)
            .FirstOrDefaultAsync(p => p.Id == promotionId, cancellationToken);
        
        if (promotion == null)
        {
            logger.LogWarning("پویش {PromotionId} یافت نشد", promotionId);
            return;
        }
        
        // ایجاد یا به‌روزرسانی Metrics
        var metrics = promotion.Metrics;
        var isNew = false;
        if (metrics == null)
        {
            isNew = true;
            metrics = new PromotionMetrics
            {
                PromotionId = promotionId
            };
        }
        
        // محاسبه تعداد مشتریان جدید
        var newCustomers = await participationRepo.Query()
            .Where(p => p.PromotionId == promotionId && p.CompletedDate.HasValue)
            .CountAsync(cancellationToken);
        
        metrics.NewCustomersAcquired = newCustomers;
        
        // محاسبه CAC
        if (metrics.CampaignCost.HasValue && metrics.CampaignCost > 0 && newCustomers > 0)
        {
            // اگر تسهیم هزینه دارد، CAC میانگین‌وزنی محاسبه می‌شود
            if (promotion.CostAllocations?.Count > 0)
            {
                var totalWeightedCAC = 0m;
                var totalCustomers = 0;
                
                foreach (var allocation in promotion.CostAllocations)
                {
                    var customersInPeriod = await participationRepo.Query()
                        .Where(p => p.PromotionId == promotionId &&
                                    p.CompletedDate.HasValue &&
                                    p.CompletedDate.Value >= allocation.FromDate &&
                                    p.CompletedDate.Value <= allocation.ToDate)
                        .CountAsync(cancellationToken);
                    
                    if (customersInPeriod > 0 && allocation.AllocatedCost.HasValue)
                    {
                        totalWeightedCAC += allocation.AllocatedCost.Value;
                        totalCustomers += customersInPeriod;
                    }
                }
                
                metrics.CustomerAcquisitionCost = totalCustomers > 0 ? totalWeightedCAC / totalCustomers : null;
            }
            else
            {
                // محاسبه ساده
                metrics.CustomerAcquisitionCost = metrics.CampaignCost.Value / newCustomers;
            }
        }
        
        // محاسبه ROI
        if (metrics.CampaignCost.HasValue && metrics.CampaignCost > 0 && metrics.CampaignRevenue.HasValue)
        {
            metrics.ReturnOnInvestment = ((metrics.CampaignRevenue.Value - metrics.CampaignCost.Value) / metrics.CampaignCost.Value) * 100;
        }
        
        // محاسبه نرخ تبدیل
        if (metrics.TargetAudienceCount.HasValue && metrics.TargetAudienceCount > 0)
        {
            metrics.ConversionCount = newCustomers;
            metrics.ConversionRate = ((decimal)newCustomers / metrics.TargetAudienceCount.Value) * 100;
        }
        
        // محاسبه Cost Per Conversion
        if (metrics.CampaignCost.HasValue && newCustomers > 0)
        {
            metrics.CostPerConversion = metrics.CampaignCost.Value / newCustomers;
        }
        
        metrics.LastMetricsCalculationDate = DateTime.UtcNow;
        
        if (isNew)
        {
            await metricsCommandRepo.AddAsync(metrics, cancellationToken);
        }
        else
        {
            metricsCommandRepo.Update(metrics);
        }
        
        await metricsCommandRepo.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("معیارهای پویش {PromotionId} به‌روزرسانی شد - CAC: {CAC}, مشتریان: {Customers}",
            promotionId, metrics.CustomerAcquisitionCost, newCustomers);
    }
    
    /// <summary>
    /// محاسبه و به‌روزرسانی AllocatedCost برای همه بازه‌های تسهیم هزینه
    /// </summary>
    public async Task UpdateAllocatedCostsAsync(int promotionId, decimal totalCost, CancellationToken cancellationToken = default)
    {
        var allocations = await costAllocationRepo.Query()
            .Where(a => a.PromotionId == promotionId)
            .ToListAsync(cancellationToken);
        
        foreach (var allocation in allocations)
        {
            allocation.AllocatedCost = totalCost * (allocation.CostPercentage / 100);
            costAllocationCommandRepo.Update(allocation);
        }
        
        await costAllocationCommandRepo.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("هزینه‌های تخصیص یافته برای پویش {PromotionId} به‌روزرسانی شد - هزینه کل: {TotalCost}",
            promotionId, totalCost);
    }
}
#endif
