using Hyper.CustomerPortal.Application.Interfaces;
using Hyper.Domain.Entities.Promotions.Plans;
using Hyper.Domain.Entities.Promotions.Plans.Data;
using Hyper.Domain.Entities.Promotions.Plans.Enums;
using Hyper.Domain.Entities.Rewards;
using Hyper.Domain.Repository;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;

namespace Hyper.CustomerPortal.Application.Services;

/// <summary>
/// سرویس مدیریت طرح‌های اشتراک
/// </summary>
public class PlanService(
    IQueryRepository<Plan, int> planQueryRepository,
    IQueryRepository<CustomerPlan, int> customerPlanQueryRepository,
    ICommandRepository<CustomerPlan, int> customerPlanCommandRepository,
    IQueryRepository<RewardCost, int> rewardCostQueryRepository,
    IHyperUnitOfWorkCommand commandUnitOfWork) : IPlanService
{
    public async Task<PaginatedList<PlanServiceDto>> GetAvailablePlansAsync(
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = planQueryRepository
            .Query()
            .Include(p => p.Point)
            .Where(p => p.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Title.Contains(search) ||
                                     (p.Description != null && p.Description.Contains(search)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var plans = await query
            .OrderBy(p => p.OrderId ?? int.MaxValue)
            .ThenByDescending(p => p.CreateDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PlanServiceDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description ?? string.Empty,
                PriceInPoints = p.PriceInPoints,
                PointTypeName = p.Point.Title,
                PointTypeColor = null,
                PointId = p.PointId,
                ValidityDays = p.ValidityDays,
                DiscountType = p.DiscountType,
                DiscountValue = p.DiscountValue,
                IsGlobalDiscount = p.IsGlobalDiscount,
                PictureId = p.PictureId,
                IsActive = p.IsActive,
                CreatedAt = p.CreateDate
            })
            .ToListAsync(cancellationToken);

        return new PaginatedList<PlanServiceDto>(plans, totalCount, pageNumber, pageSize);
    }

    public async Task<PlanServiceDto?> GetPlanByIdAsync(int planId, CancellationToken cancellationToken = default)
    {
        return await planQueryRepository
            .Query()
            .Include(p => p.Point)
            .Where(p => p.Id == planId && p.IsActive)
            .Select(p => new PlanServiceDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description ?? string.Empty,
                PriceInPoints = p.PriceInPoints,
                PointTypeName = p.Point.Title,
                PointTypeColor = null,
                PointId = p.PointId,
                ValidityDays = p.ValidityDays,
                DiscountType = p.DiscountType,
                DiscountValue = p.DiscountValue,
                IsGlobalDiscount = p.IsGlobalDiscount,
                PictureId = p.PictureId,
                IsActive = p.IsActive,
                CreatedAt = p.CreateDate
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CustomerPlanServiceDto?> GetActiveCustomerPlanAsync(
        int customerId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var activePlan = await customerPlanQueryRepository
            .Query()
            .Include(cp => cp.Plan)
            .ThenInclude(p => p.Point)
            .Include(cp => cp.CustomerTenant)
            .Where(cp => cp.CustomerTenant.CustomerId == customerId &&
                         cp.IsActive &&
                         cp.Status == CustomerPlanStatus.Active &&
                         cp.StartDate <= now &&
                         cp.ExpiryDate >= now)
            .OrderByDescending(cp => cp.CreateDate)
            .FirstOrDefaultAsync(cancellationToken);

        if (activePlan is null)
        {
            return null;
        }

        return new CustomerPlanServiceDto
        {
            Id = activePlan.Id,
            CustomerTenantId = activePlan.CustomerTenantId,
            PlanId = activePlan.PlanId,
            PointId = activePlan.Plan.PointId,
            PointTypeName = activePlan.Plan.Point.Title,
            PointTypeColor = null,
            PlanTitle = activePlan.Plan.Title,
            PlanDescription = activePlan.Plan.Description ?? string.Empty,
            PlanPictureId = activePlan.Plan.PictureId,
            PurchaseDate = activePlan.PurchaseDate,
            StartDate = activePlan.StartDate,
            ExpiryDate = activePlan.ExpiryDate,
            Status = activePlan.Status,
            PaidAmount = activePlan.PaidAmount,
            UsageCount = activePlan.UsageCount,
            TotalDiscountReceived = activePlan.TotalDiscountReceived,
            DiscountType = activePlan.Plan.DiscountType,
            DiscountValue = activePlan.Plan.DiscountValue,
            IsGlobalDiscount = activePlan.Plan.IsGlobalDiscount,
            IsValid = true
        };
    }

    public async Task<PurchasePlanResult> PurchasePlanAsync(
        int customerId,
        int planId,
        CancellationToken cancellationToken = default)
    {
        var plan = await planQueryRepository
            .Query()
            .Include(p => p.Point)
            .Include(p => p.Promotion)
            .FirstOrDefaultAsync(p => p.Id == planId && p.IsActive, cancellationToken);

        if (plan == null)
        {
            throw new InvalidOperationException("Plan not found");
        }

        var hasActivePlan = await customerPlanQueryRepository
            .Query()
            .AnyAsync(cp =>
                cp.CustomerTenant.CustomerId == customerId &&
                cp.IsActive &&
                cp.Status == CustomerPlanStatus.Active &&
                cp.ExpiryDate >= DateTime.UtcNow, cancellationToken);

        if (hasActivePlan)
        {
            throw new InvalidOperationException("شما در حال حاضر یک طرح فعال دارید");
        }

        var startDate = DateTime.UtcNow;
        var expiryDate = startDate.AddDays(plan.ValidityDays);

        var customerTenantRepo = commandUnitOfWork.Repository<CustomerTenant, int>();
        var customerTenant = await customerTenantRepo.FirstOrDefaultAsync(
            ct => ct.CustomerId == customerId && ct.TenantId == plan.Promotion.TenantId,
            cancellationToken);

        if (customerTenant == null)
        {
            customerTenant = new CustomerTenant
            {
                CustomerId = customerId,
                TenantId = plan.Promotion.TenantId,
                JoinDate = DateTime.UtcNow,
                IsActive = true
            };
            customerTenantRepo.Add(customerTenant);
        }

        var customerPlan = new CustomerPlan
        {
            CustomerTenantId = customerTenant.Id,
            CustomerTenant = customerTenant,
            PlanId = planId,
            PurchaseDate = DateTime.UtcNow,
            StartDate = startDate,
            ExpiryDate = expiryDate,
            Status = CustomerPlanStatus.Active,
            PaidAmount = plan.PriceInPoints,
            IsActive = true
        };

        await customerPlanCommandRepository.AddAsync(customerPlan);
        await commandUnitOfWork.SaveChangesAsync(cancellationToken);

        return new PurchasePlanResult
        {
            CustomerPlanId = customerPlan.Id,
            ExpiryDate = expiryDate
        };
    }

    public async Task<int> ExpireExpiredPlansAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return await customerPlanCommandRepository.Query()
            .Where(cp => cp.IsActive &&
                  cp.Status == CustomerPlanStatus.Active &&
                  cp.ExpiryDate < now)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(cp => cp.Status, _ => CustomerPlanStatus.Expired)
                .SetProperty(cp => cp.IsActive, _ => false),
            cancellationToken);
    }

    public async Task<long> CalculateRewardPriceWithPlanDiscountAsync(
        int customerId,
        int rewardId,
        int? pointId,
        long originalPrice,
        CustomerPlanServiceDto? activePlan,
        CancellationToken cancellationToken = default)
    {
        var plan = activePlan ?? await GetActiveCustomerPlanAsync(customerId, cancellationToken);

        if (plan == null || !plan.IsValid)
        {
            return originalPrice;
        }

        var effectivePointId = pointId ?? plan.PointId;

        if (!plan.IsGlobalDiscount)
        {
            var specificPrice = await rewardCostQueryRepository
                .Query()
                .Where(rc => rc.RewardId == rewardId &&
                             rc.PointId == effectivePointId &&
                             rc.PlanId == plan.PlanId)
                .Select(rc => rc.Amount)
                .FirstOrDefaultAsync(cancellationToken);

            if (specificPrice > 0)
            {
                return specificPrice;
            }

            return originalPrice;
        }

        long discountedPrice = originalPrice;

        if (plan.DiscountType == PlanDiscountType.Percentage)
        {
            var discountAmount = (long)Math.Round(originalPrice * (plan.DiscountValue / 100m));
            discountedPrice = originalPrice - discountAmount;
        }
        else if (plan.DiscountType == PlanDiscountType.FixedAmount)
        {
            discountedPrice = originalPrice - (long)plan.DiscountValue;
        }

        return Math.Max(0, discountedPrice);
    }
}

