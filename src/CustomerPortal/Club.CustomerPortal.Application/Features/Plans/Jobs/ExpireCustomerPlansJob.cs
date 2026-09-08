using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Plans.Jobs;

/// <summary>
/// Job برای منقضی کردن خودکار طرح‌های اشتراک تمام‌شده
/// </summary>
public class ExpireCustomerPlansJob(
    IPlanService planService,
    ILogger<ExpireCustomerPlansJob> logger
) : IExpireCustomerPlansJob
{
    public async Task Run()
    {
        logger.LogInformation("Starting expired customer plans processing at {Time}", DateTime.UtcNow);

        try
        {
            var affectedCount = await planService.ExpireExpiredPlansAsync(CancellationToken.None);
            logger.LogInformation("Expired {Count} customer plans at {Time}", affectedCount, DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error expiring customer plans at {Time}", DateTime.UtcNow);
            throw;
        }
    }
}

