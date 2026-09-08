using Hyper.Domain.Entities.Customers.Enums;

namespace Hyper.Application.Features.Points.Jobs;

/// <summary>
/// Job برای منقضی کردن خودکار امتیازهای منقضی شده
/// </summary>
public class ExpirePointsJob(
    IQueryRepository<CustomerTransaction, long> customerTransactionQueryRepo,
    ICommandRepository<CustomerTransaction, long> customerTransactionCmdRepo,
    ILogger<ExpirePointsJob> logger
) : IExpirePointsJob
{
    public async Task Run()
    {
        logger.LogInformation("Starting expired points processing at {Time}", DateTime.UtcNow);

        try
        {
            var now = DateTime.UtcNow;
            
            // Find all credit transactions that:
            // 1. Have an expiration date
            // 2. Are not already expired
            // 3. Are not spent
            // 4. Have expiration date <= now
            var expiredTransactions = await customerTransactionQueryRepo.GetAllAsync(
                CancellationToken.None,
                predicate: x => x.ExpirationDate.HasValue &&
                     x.ExpirationDate.Value <= now &&
                     !x.IsExpired &&
                     !x.IsSpent &&
                     x.TransactionType == CustomerTransactionType.Credit &&
                     x.Credit.HasValue &&
                     x.Credit.Value > 0);

            var expiredCount = 0;
            foreach (var transaction in expiredTransactions)
            {
                transaction.IsExpired = true;
                transaction.ExpiredDate = now;
                customerTransactionCmdRepo.Update(transaction);
                expiredCount++;
            }

            if (expiredCount > 0)
            {
                await customerTransactionCmdRepo.UnitOfWork.SaveChangesAsync(CancellationToken.None);
                logger.LogInformation("Expired {Count} point transactions at {Time}", expiredCount, DateTime.UtcNow);
            }
            else
            {
                logger.LogInformation("No expired points found at {Time}", DateTime.UtcNow);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error expiring points at {Time}", DateTime.UtcNow);
            throw;
        }
    }
}

