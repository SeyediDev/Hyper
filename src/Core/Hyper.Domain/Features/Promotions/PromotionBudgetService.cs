using Hyper.Domain.Entities.Lotteries;

namespace Hyper.Domain.Features.Promotions;

public interface IPromotionBudgetService
{
    /// <summary>
    /// بررسی بودجه امتیاز
    /// </summary>
    Task<bool> HasPointBudgetAsync(
        int pointId, long amount, int customerTenantId, CancellationToken cancellationToken);

    /// <summary>
    /// بررسی بودجه پاداش
    /// </summary>
    Task<bool> HasRewardBudgetAsync(int promotionId, int? rewardId, int count,
        CancellationToken cancellationToken);

    /// <summary>
    /// بررسی بودجه عضویت در جامعه مشتریان
    /// </summary>
    Task<bool> HasCustomerSegmentBudgetAsync(int promotionId, int? customerSegmentId, int count,
        CancellationToken cancellationToken);

    /// <summary>
    /// بررسی بودجه شرکت در قرعه‌کشی
    /// </summary>
    Task<bool> HasLotteryBudgetAsync(int promotionId, int? lotteryId, int count,
        CancellationToken cancellationToken);

    /// <summary>
    /// بررسی بودجه فراخوانی API بیرونی
    /// </summary>
    Task<bool> HasExternalApiBudgetAsync(int promotionId, int? externalApiId, int count,
        CancellationToken cancellationToken);

    /// <summary>
    /// بررسی بودجه اطلاع‌رسانی
    /// </summary>
    Task<bool> HasNotificationBudgetAsync(int promotionId, NotificationBudgetSubType subType, int count,
        CancellationToken cancellationToken);
}

public class PromotionBudgetService(
    IQueryRepository<PromotionBudget, int> budgetRepo,
    IQueryRepository<CustomerTransaction, long> customerTransactionRepo,
    IQueryRepository<CustomerSegmentMembership, int> segmentMembershipRepo,
    IQueryRepository<LotteryParticipant, int> lotteryParticipantRepo,
    IQueryRepository<CustomerPointLevel, int> customerPointLevelQueryRepo,
    ILogger<PromotionBudgetService> logger)
    : IPromotionBudgetService
{
    public async Task<bool> HasPointBudgetAsync(
        int pointId, long amount, int customerTenantId, CancellationToken cancellationToken)
    {
        var customerPointLevels = await customerPointLevelQueryRepo
            .GetAllWithIncludeAsync(
                [cpl => cpl.PointLevel],
                cancellationToken,
                cpl => cpl.CustomerTenantId == customerTenantId && !cpl.ExpireDate.HasValue);

        List<PointLevel> pointLevels = customerPointLevels.Select(cpl => cpl.PointLevel).ToList();
        var budgets = await budgetRepo.GetAllAsync(cancellationToken,
            x => x.Kind == PromotionBudgetKind.Point && x.PointId == pointId);

        foreach (var budget in budgets)
        {
            long used = await CalculatePointUsedBudgetAsync(budget, customerTenantId, cancellationToken);

            if ((used + amount) > budget.Amount)
            {
                logger.LogWarning("Budget exhausted for point {PointId}. Used: {Used}, Requested: {Amount}, Limit: {Limit}",
                    pointId, used, amount, budget.Amount);
                return false;
            }
        }

        return true;
    }

    public async Task<bool> HasRewardBudgetAsync(int promotionId, int? rewardId, int count,
        CancellationToken cancellationToken)
    {
        var budgets = await budgetRepo.GetAllWithIncludeAsync(
            [x => x.Reward, x => x.Promotion],
            cancellationToken,
            x => x.Kind == PromotionBudgetKind.GrantReward 
                && x.PromotionId == promotionId
                && (rewardId == null || x.RewardId == rewardId));

        if (!budgets.Any())
            return true;

        foreach (var budget in budgets)
        {
            // TODO: Implement proper reward budget tracking using RewardAsset or similar entity
            // For now, return true to allow the action
            logger.LogInformation("Reward budget check not fully implemented for budget {BudgetId}", budget.Id);
        }

        return true;
    }

    public async Task<bool> HasCustomerSegmentBudgetAsync(int promotionId, int? customerSegmentId, int count,
        CancellationToken cancellationToken)
    {
        var budgets = await budgetRepo.GetAllWithIncludeAsync(
            [x => x.CustomerSegment, x => x.Promotion],
            cancellationToken,
            x => x.Kind == PromotionBudgetKind.JoinInCustomerSegment 
                && x.PromotionId == promotionId
                && (customerSegmentId == null || x.CustomerSegmentId == customerSegmentId));

        if (!budgets.Any())
            return true;

        foreach (var budget in budgets)
        {
            long used = await CalculateSegmentUsedBudgetAsync(budget, cancellationToken);

            if ((used + count) > budget.Amount)
            {
                logger.LogWarning("Customer segment budget exhausted for promotion {PromotionId}. Used: {Used}, Requested: {Count}, Limit: {Limit}",
                    promotionId, used, count, budget.Amount);
                return false;
            }
        }

        return true;
    }

    public async Task<bool> HasLotteryBudgetAsync(int promotionId, int? lotteryId, int count,
        CancellationToken cancellationToken)
    {
        var budgets = await budgetRepo.GetAllWithIncludeAsync(
            [x => x.Lottery, x => x.Promotion],
            cancellationToken,
            x => x.Kind == PromotionBudgetKind.JoinLottery 
                && x.PromotionId == promotionId
                && (lotteryId == null || x.LotteryId == lotteryId));

        if (!budgets.Any())
            return true;

        foreach (var budget in budgets)
        {
            long used = await CalculateLotteryUsedBudgetAsync(budget, cancellationToken);

            if ((used + count) > budget.Amount)
            {
                logger.LogWarning("Lottery budget exhausted for promotion {PromotionId}. Used: {Used}, Requested: {Count}, Limit: {Limit}",
                    promotionId, used, count, budget.Amount);
                return false;
            }
        }

        return true;
    }

    public async Task<bool> HasExternalApiBudgetAsync(int promotionId, int? externalApiId, int count,
        CancellationToken cancellationToken)
    {
        var budgets = await budgetRepo.GetAllWithIncludeAsync(
            [x => x.ExternalApi, x => x.Promotion],
            cancellationToken,
            x => x.Kind == PromotionBudgetKind.CallExternalApi 
                && x.PromotionId == promotionId
                && (externalApiId == null || x.ExternalApiId == externalApiId));

        if (!budgets.Any())
            return true;

        foreach (var budget in budgets)
        {
            // TODO: Implement proper ExternalApi budget tracking
            logger.LogInformation("External API budget tracking not yet implemented for budget {BudgetId}", budget.Id);
        }

        return true;
    }

    public async Task<bool> HasNotificationBudgetAsync(int promotionId, NotificationBudgetSubType subType, int count,
        CancellationToken cancellationToken)
    {
        var budgets = await budgetRepo.GetAllWithIncludeAsync(
            [x => x.Promotion],
            cancellationToken,
            x => x.Kind == PromotionBudgetKind.Notification 
                && x.PromotionId == promotionId
                && (x.NotificationSubType == subType || x.NotificationSubType == NotificationBudgetSubType.All));

        if (!budgets.Any())
            return true;

        foreach (var budget in budgets)
        {
            // TODO: Implement proper notification budget tracking
            logger.LogInformation("Notification budget tracking not yet implemented for budget {BudgetId}", budget.Id);
        }

        return true;
    }

    #region Calculate Used Budget Methods

    private async Task<long> CalculatePointUsedBudgetAsync(PromotionBudget budget, int customerTenantId, CancellationToken cancellationToken)
    {
        var query = customerTransactionRepo.Query()
            .Where(x => x.PointId == budget.PointId);

        query = query.Where(x => x.CustomerTenantId == customerTenantId);

        return await query.SumAsync(x => (long)((x.Credit ?? 0) - (x.Debit ?? 0)), cancellationToken);
    }

    private async Task<long> CalculateSegmentUsedBudgetAsync(PromotionBudget budget, CancellationToken cancellationToken)
    {
        var query = segmentMembershipRepo.Query()
            .Where(x => x.SegmentId == budget.CustomerSegmentId);

        return await query.CountAsync(x => x.Id > 0, cancellationToken);
    }

    private async Task<long> CalculateLotteryUsedBudgetAsync(PromotionBudget budget, CancellationToken cancellationToken)
    {
        var query = lotteryParticipantRepo.Query()
            .Where(x => x.LotteryId == budget.LotteryId);

        return await query.CountAsync(x => x.Id > 0, cancellationToken);
    }

    #endregion
}
