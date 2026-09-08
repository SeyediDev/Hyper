using Hyper.CustomerPortal.Application.Interfaces;
using Hyper.Domain.Entities.Promotions;
using Hyper.Domain.Entities.Promotions.Enums;
using Hyper.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Reflection;

namespace Hyper.CustomerPortal.Application.Services;

public class PromotionService(
    IQueryRepository<Promotion, int> promotionRepo,
    IQueryRepository<Hyper.Domain.Entities.Promotions.Data.PromotionParticipation, int> participationRepo,
    IQueryRepository<CustomerTenant, int> customerTenantRepo) : IPromotionService
{
    public async Task<PaginatedList<PromotionDto>> GetActivePromotionsAsync(
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        // Get active campaigns (within date range)
        // Filter by Status=Active (or null) and IsVisibleToCustomer=true
        // Note: Campaigns are identified by having OnCondition or OnCompletion Actions
        var promotions = await promotionRepo.Query()
            .Include(p => p.Actions)
            .Where(p => p.Actions.Any(a => a.RunTimeType == PromotionActionRunTimeType.OnOneEventCompletion
                || a.RunTimeType == PromotionActionRunTimeType.OnCompletion) // Only campaigns
                && (p.FromDate == null || p.FromDate <= now)
                && (p.ToDate == null || p.ToDate >= now)
                && (p.Status == null || p.Status == PromotionStatus.Active)
                && p.IsVisibleToCustomer
                && !p.IsDeleted)
            .OrderByDescending(p => p.FromDate ?? p.CreateDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var totalCount = await promotionRepo.Query()
            .Include(p => p.Actions)
            .CountAsync(p => p.Actions.Any(a => a.RunTimeType == PromotionActionRunTimeType.OnOneEventCompletion
                || a.RunTimeType == PromotionActionRunTimeType.OnCompletion) // Only campaigns
                && (p.FromDate == null || p.FromDate <= now)
                && (p.ToDate == null || p.ToDate >= now)
                && (p.Status == null || p.Status == PromotionStatus.Active)
                && p.IsVisibleToCustomer
                && !p.IsDeleted, cancellationToken);

        var promotionDtos = promotions.Select(p => MapToDto(p, now)).ToList();

        return new PaginatedList<PromotionDto>(promotionDtos, totalCount, pageNumber, pageSize);
    }

    public async Task<PromotionDto?> GetPromotionByIdAsync(int promotionId, CancellationToken cancellationToken = default)
    {
        var promotion = await promotionRepo.Query()
            .Include(p => p.Triggers)
                .ThenInclude(t => t.EventType)
            .Include(p => p.Actions)
                .ThenInclude(a => a.Point)
            .Include(p => p.Actions)
                .ThenInclude(a => a.Reward)
            .Include(p => p.Actions)
                .ThenInclude(a => a.Lottery)
            .FirstOrDefaultAsync(p => p.Id == promotionId && !p.IsDeleted, cancellationToken);

        if (promotion == null)
            return null;

        var now = DateTime.UtcNow;
        var isActive = (promotion.FromDate == null || promotion.FromDate <= now)
            && (promotion.ToDate == null || promotion.ToDate >= now);

        // Build benefits description from actions
        var benefitsDescription = BuildBenefitsDescription(promotion.Actions);

        // Build requirements description from triggers
        var requirementsDescription = BuildRequirementsDescription(promotion.Triggers);

        var fullDescription = promotion.Title;
        if (!string.IsNullOrEmpty(requirementsDescription))
        {
            fullDescription += $"\n\nشرایط شرکت:\n{requirementsDescription}";
        }
        if (!string.IsNullOrEmpty(benefitsDescription))
        {
            fullDescription += $"\n\nمزایا:\n{benefitsDescription}";
        }

        return MapToDto(promotion, now, fullDescription, isActive);
    }

    private string BuildBenefitsDescription(ICollection<PromotionAction> actions)
    {
        if (actions == null || !actions.Any())
            return "";

        var benefits = new List<string>();

        foreach (var action in actions.Where(a => a.RunTimeType == PromotionActionRunTimeType.OnCompletion))
        {
            var benefit = InterpretAction(action);
            if (!string.IsNullOrEmpty(benefit))
                benefits.Add(benefit);
        }

        return string.Join("\n", benefits);
    }

    private string BuildRequirementsDescription(ICollection<PromotionTrigger> triggers)
    {
        if (triggers == null || !triggers.Any())
            return "";

        var requirements = new List<string>();

        foreach (var trigger in triggers.OrderBy(t => t.SequenceOrder))
        {
            var requirement = InterpretTrigger(trigger);
            if (!string.IsNullOrEmpty(requirement))
                requirements.Add(requirement);
        }

        return string.Join("\n", requirements);
    }

    private string InterpretAction(PromotionAction action)
    {
        var actionKindDescription = GetEnumDescription(action.ActionKind);

        switch (action.ActionKind)
        {
            case PromotionActionKind.CreditPoint:
                var pointName = action.Point?.Title ?? "امتیاز";
                var amountText = action.AmountFormula ?? "0";
                return $"• دریافت {amountText} {pointName}";

            case PromotionActionKind.GrantReward:
                var rewardName = action.Reward?.Title ?? "پاداش";
                var rewardAmountStr = action.AmountFormula ?? "1";
                int.TryParse(rewardAmountStr, out var rewardAmount);
                if (rewardAmount <= 0) rewardAmount = 1;
                return $"• دریافت {rewardAmount} عدد {rewardName}";

            case PromotionActionKind.JoinLottery:
                var lotteryName = action.Lottery?.Title ?? "قرعه‌کشی";
                return $"• شرکت در {lotteryName}";

            case PromotionActionKind.JoinInCustomerSegment:
                return $"• عضویت در جامعه مشتریان";

            case PromotionActionKind.None:
                return $"• {actionKindDescription}";

            default:
                return $"• {actionKindDescription}";
        }
    }

    private string InterpretTrigger(PromotionTrigger trigger)
    {
        switch (trigger.ReceiveEventType)
        {
            case ReceiveEventType.DynamicEvent:
                var eventName = trigger.EventType?.Title ?? "رویداد";
                var minCount = trigger.Threshold ?? 1;
                var sequenceText = trigger.SequenceOrder > 0 
                    ? $" (مرحله {trigger.SequenceOrder})" 
                    : "";
                var parallelText = trigger.FlowType==PromotionTriggerFlowType.Parallel 
                    ? " (همزمان)" 
                    : trigger.FlowType.ToString();
                
                if (minCount > 1)
                    return $"• انجام {minCount} بار {eventName}{sequenceText}{parallelText}";
                else
                    return $"• انجام {eventName}{sequenceText}{parallelText}";

            case ReceiveEventType.UpgradePointLevel:
                return $"• ارتقاء سطح امتیاز";

            case ReceiveEventType.PurchaseReward:
                return $"• خرید پاداش";

            case ReceiveEventType.ConsumeReward:
                return $"• مصرف پاداش";

            default:
                return $"• {GetEnumDescription(trigger.ReceiveEventType)}";
        }
    }

    private static Hyper.CustomerPortal.Application.Interfaces.PromotionDto MapToDto(
        Promotion p,
        DateTime now,
        string? description = null,
        bool? isActiveOverride = null)
    {
        var endDate = p.ToDate ?? DateTime.MaxValue;
        var isActive = isActiveOverride ?? ((p.FromDate == null || p.FromDate <= now) && (p.ToDate == null || p.ToDate >= now));
        var daysUntilEnd = (int)Math.Ceiling((endDate - now).TotalDays);

        return new Hyper.CustomerPortal.Application.Interfaces.PromotionDto
        {
            Id = p.Id,
            Title = p.Title,
            Category = p.Category.ToString(),
            Description = description ?? p.Title,
            ShortDescription = p.ShortDescription,
            Benefits = p.Benefits,
            ParticipationGuide = p.ParticipationGuide,
            StartDate = p.FromDate ?? p.CreateDate,
            EndDate = endDate,
            DaysUntilEnd = daysUntilEnd,
            ImageUrl = p.CardImageUrl ?? p.BannerImageUrl,
            CardImageUrl = p.CardImageUrl,
            BannerImageUrl = p.BannerImageUrl,
            IconUrl = p.IconUrl,
            PrimaryColor = p.PrimaryColor,
            RewardType = p.RewardType?.ToString(),
            IsActive = isActive,
            CanParticipate = isActive
        };
    }

    private string GetEnumDescription(Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        if (field == null)
            return value.ToString();

        var attribute = field.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }

    public Task<ParticipationResultDto> ParticipateAsync(
        int customerId,
        int promotionId,
        CancellationToken cancellationToken = default)
    {
        // Participation is handled automatically through event processing
        // This method is kept for API compatibility
        return Task.FromResult(new ParticipationResultDto
        {
            Success = false,
            Message = "شرکت در پویش از طریق انجام رویدادهای مورد نیاز انجام می‌شود"
        });
    }

    public async Task<PaginatedList<MyParticipationDto>> GetMyParticipationsAsync(
        int customerId,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        // Get customer's tenant IDs
        var customerTenants = await customerTenantRepo.Query()
            .Where(ct => ct.CustomerId == customerId)
            .Select(ct => ct.Id)
            .ToListAsync(cancellationToken);

        if (!customerTenants.Any())
            return new PaginatedList<MyParticipationDto>([], 0, pageNumber, pageSize);

        // Get participations
        var participations = await participationRepo.Query()
            .Where(p => customerTenants.Contains(p.CustomerTenantId))
            .Include(p => p.Promotion)
            .OrderByDescending(p => p.StartDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var totalCount = await participationRepo.Query()
            .CountAsync(p => customerTenants.Contains(p.CustomerTenantId), cancellationToken);

        var participationDtos = participations.Select(p => new MyParticipationDto
        {
            PromotionId = p.PromotionId,
            PromotionTitle = p.Promotion.Title,
            PromotionCategorty = p.Promotion.Category,
            ParticipatedAt = p.StartDate,
            Status = GetEnumDescription(p.Status)
        }).ToList();

        return new PaginatedList<MyParticipationDto>(participationDtos, totalCount, pageNumber, pageSize);
    }
}

