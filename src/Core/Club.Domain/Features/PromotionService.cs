using Club.Domain.Features.Promotions;
using Club.Domain.Enums;

namespace Club.Domain.Features;

public interface IPromotionService
{
    /// <summary>
    /// اجرای پویش‌های زمان‌بندی شده
    /// </summary>
    [Telemetry]
    Task ProcessScheduledPromotions(CancellationToken cancellationToken);
    
    /// <summary>
    /// پردازش رویداد دریافتی و بررسی پویش‌های فعال
    /// </summary>
    /// <param name="request">درخواست پردازش رویداد</param>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>نتیجه پردازش</returns>
    [Telemetry]
    Task<PromotionProcessingResponse?> ProcessEventAsync(
        PromotionProcessingRequest request, 
        CancellationToken cancellationToken = default);
    
}

/// <summary>
/// درخواست پردازش رویداد برای پویش
/// </summary>
public record PromotionProcessingRequest(
    int TenantId,
    int EventLogId,
    int EventChannelId,
    int EventTypeId,
    Customer Customer,
    Dictionary<string, string>? Parameters = null,
    List<PointLevel>? CustomerPointLevels = null)
{
    /// <summary>
    /// نوع Trigger (Event, UpgradePointLevel, PurchaseAward, ConsumeAward, PurchaseProductOrService)
    /// </summary>
    public TriggerType? TriggerType { get; init; }
    
    /// <summary>
    /// شناسه PointLevel (برای TriggerType.UpgradePointLevel)
    /// </summary>
    public int? PointLevelId { get; init; }
    
    /// <summary>
    /// شناسه Award (برای TriggerType.PurchaseAward یا ConsumeAward)
    /// </summary>
    public int? AwardId { get; init; }
    
    /// <summary>
    /// شناسه Product (برای TriggerType.PurchaseProductOrService)
    /// </summary>
    public int? ProductId { get; init; }
}

/// <summary>
/// پاسخ پردازش پویش
/// </summary>
public record PromotionProcessingResponse
{
    /// <summary>
    /// تعداد پویش‌های فعال که این رویداد برای آن‌ها معتبر است
    /// </summary>
    public int ActivePromotionsCount { get; set; }
    
    /// <summary>
    /// تعداد پویش‌هایی که شرط‌های آن‌ها برقرار شد
    /// </summary>
    public int ConditionsMetCount { get; set; }
    
    /// <summary>
    /// تعداد پویش‌هایی که تکمیل شدند
    /// </summary>
    public int CompletedPromotionsCount { get; set; }
    
    /// <summary>
    /// تعداد اقدامات انجام شده
    /// </summary>
    public int ActionsExecutedCount { get; set; }
    
    /// <summary>
    /// لیست شناسه پویش‌های تکمیل شده
    /// </summary>
    public List<int> CompletedPromotionIds { get; set; } = [];
}

internal class PromotionService(
    ILogger<PromotionService> logger,
    IQueryRepository<Promotion, int> promotionRepo,
    ICommandRepository<PromotionMessage, int> messageRepo,
    ICommandRepository<PromotionRecipient, int> recipientRepo,
    IQueryRepository<CustomerSegmentMembership, int> segmentMembershipRepo,
    IQueryRepository<PromotionCondition, int> promotionConditionRepo,
    IQueryRepository<PromotionAction, int> promotionActionRepo,
    IQueryRepository<PromotionParticipation, int> participationRepo,
    ICommandRepository<PromotionParticipation, int> participationCmdRepo,
    ICommandRepository<PromotionEventReceived, int> eventReceivedCmdRepo,
    IQueryRepository<PromotionEventReceived, int> eventReceivedQueryRepo,
    IQueryRepository<CustomerTenant, int> customerTenantRepo,
    IQueryRepository<EventLog, long> eventLogRepo,
    IQueryRepository<PromotionCustomerSegment, int> promotionCustomerSegmentRepo,
    IPromotionActionService promotionActionService,
    IEvaluateFormulaService evaluateFormulaService,
    ICustomerService customerService
    ) : IPromotionService
{
    public async Task ProcessScheduledPromotions(CancellationToken cancellationToken)
    {
        DateTime now = DateTime.Now;

        // Get active scheduled promotions
        var allPromotions = await promotionRepo.GetAllAsync(cancellationToken);
        var scheduledPromotions = allPromotions
            .Where(p => p.IsScheduled
                    && p.PromotionSchedulingKind.HasValue
                    && (p.FromDate == null || p.FromDate <= now)
                    && (p.ToDate == null || p.ToDate >= now))
            .ToList();

        logger.LogInformation("Found {Count} scheduled promotions to evaluate", scheduledPromotions.Count);

        foreach (var promotion in scheduledPromotions)
        {
            try
            {
                // Check if it's time to run this promotion
                if (ShouldRunPromotion(promotion, now))
                {
                    logger.LogInformation("Executing scheduled promotion: {PromotionId} - {PromotionTitle}", 
                        promotion.Id, promotion.Title);
                    await ExecuteScheduledPromotion(promotion, now, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing scheduled promotion {PromotionId}", promotion.Id);
            }
        }
    }

    private bool ShouldRunPromotion(Promotion promotion, DateTime now)
    {
        if (!promotion.IsScheduled || !promotion.PromotionSchedulingKind.HasValue)
            return false;

        // Check hour and minute if specified
        if (promotion.ScheduledHour.HasValue && now.Hour != promotion.ScheduledHour.Value)
            return false;

        if (promotion.ScheduledMinute.HasValue && now.Minute != promotion.ScheduledMinute.Value)
            return false;

        return promotion.PromotionSchedulingKind switch
        {
            SchedulingKind.Daily => true, // Run every day at specified time
            SchedulingKind.Weekly => promotion.WeekDay.HasValue && now.DayOfWeek == promotion.WeekDay.Value,
            SchedulingKind.Monthly => promotion.MonthDay.HasValue && now.Day == promotion.MonthDay.Value,
            SchedulingKind.Yearly => promotion.MonthDay.HasValue 
                                    && promotion.ScheduledMonth.HasValue 
                                    && now.Day == promotion.MonthDay.Value
                                    && now.Month == promotion.ScheduledMonth.Value,
            _ => false
        };
    }

    private async Task ExecuteScheduledPromotion(Promotion promotion, DateTime now, CancellationToken cancellationToken)
    {
        logger.LogInformation("Executing promotion {PromotionId}: {Title}", promotion.Id, promotion.Title);

        // Get promotion segments
        var promotionSegments = await promotionCustomerSegmentRepo.Query()
            .Where(pcs => pcs.PromotionId == promotion.Id)
            .Select(pcs => pcs.CustomerSegmentId)
            .ToListAsync(cancellationToken);

        List<CustomerSegmentMembership> segmentMemberships;

        if (!promotionSegments.Any())
        {
            // اگر هیچ جامعه‌ای تعریف نشده بود، برای همه مشتریان
            logger.LogInformation("No segments defined for promotion {PromotionId}, targeting all customers", promotion.Id);
            var allMemberships = await segmentMembershipRepo.GetAllAsync(cancellationToken);
            segmentMemberships = allMemberships
                .GroupBy(m => m.CustomerTenantId)
                .Select(g => g.First()) // هر مشتری را یک بار بگیر
                .ToList();
        }
        else
        {
            // اگر جامعه‌هایی تعریف شده بود، فقط مشتریانی که در حداقل یک جامعه عضو هستند
            var allMemberships = await segmentMembershipRepo.GetAllAsync(cancellationToken);
            segmentMemberships = allMemberships
                .Where(m => promotionSegments.Contains(m.SegmentId))
                .GroupBy(m => m.CustomerTenantId)
                .Select(g => g.First()) // هر مشتری را یک بار بگیر
                .ToList();
        }

        if (!segmentMemberships.Any())
        {
            logger.LogWarning("No customers found for promotion {PromotionId}", promotion.Id);
            return;
        }

        logger.LogInformation("Found {CustomerCount} customers for promotion {PromotionId}",
            segmentMemberships.Count, promotion.Id);

        // Create promotion message
        PromotionMessage message = new()
        {
            PromotionId = promotion.Id,
            Subject = $"پیام پویش: {promotion.Title}",
            Content = $"شما مشمول پویش {promotion.Title} هستید",
            Type = PromotionMessageType.Informational,
            Status = PromotionMessageStatus.Sent,
            Priority = 1,
            ScheduledTime = now,
            SentDate = now
        };
        messageRepo.Add(message);
        await messageRepo.UnitOfWork.SaveChangesAsync(cancellationToken);

        // Create recipients for all customers in segment
        int recipientCount = 0;
        foreach (var membership in segmentMemberships)
        {
            try
            {
                PromotionRecipient recipient = new()
                {
                    PromotionId = promotion.Id,
                    CustomerTenantId = membership.CustomerTenantId,
                    Status = PromotionRecipientStatus.Sent,
                    SentDate = now
                };
                recipientRepo.Add(recipient);
                recipientCount++;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating recipient for customer tenant {CustomerTenantId} in promotion {PromotionId}",
                    membership.CustomerTenantId, promotion.Id);
            }
        }

        await recipientRepo.UnitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Created {RecipientCount} recipients for promotion {PromotionId}",
            recipientCount, promotion.Id);

        // Update message counts
        message.SentCount = recipientCount;
        message.DeliveredCount = recipientCount;
        messageRepo.Update(message);
        await messageRepo.UnitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Successfully executed scheduled promotion {PromotionId}", promotion.Id);
    }

    public async Task<PromotionProcessingResponse?> ProcessEventAsync(
        PromotionProcessingRequest request, 
        CancellationToken cancellationToken = default)
    {
        var response = new PromotionProcessingResponse();
        var now = DateTime.UtcNow;

        // 1. پیدا کردن همه پویش‌های فعال (هم Campaign و هم Scoring)
        var activePromotions = await promotionRepo.Query()
            .Where(p => p.TenantId == request.TenantId
                && (p.Status == null || p.Status == PromotionStatus.Active)
                && (p.FromDate == null || p.FromDate <= now)
                && (p.ToDate == null || p.ToDate >= now))
            .ToListAsync(cancellationToken);

        if (!activePromotions.Any())
        {
            logger.LogDebug("No active promotions found for tenant {TenantId}", request.TenantId);
            return response;
        }

        // 2. پیدا کردن CustomerTenant
        var customerTenant = await customerTenantRepo.FirstOrDefaultAsync(
            ct => ct.CustomerId == request.Customer.Id && ct.TenantId == request.TenantId,
            cancellationToken);

        if (customerTenant == null)
        {
            logger.LogWarning("CustomerTenant not found for customer {CustomerId} and tenant {TenantId}",
                request.Customer.Id, request.TenantId);
            return response;
        }

        // 3. پیدا کردن EventLog (اگر EventLogId موجود باشد)
        EventLog? eventLog = null;
        if (request.EventLogId > 0)
        {
            eventLog = await eventLogRepo.FirstOrDefaultAsync(
                e => e.Id == request.EventLogId,
                cancellationToken);

            if (eventLog == null)
            {
                logger.LogWarning("EventLog not found for EventLogId {EventLogId}", request.EventLogId);
            }
        }

        // 4. بررسی هر پویش
        foreach (var promotion in activePromotions)
        {
            try
            {
                // بررسی Segment membership
                if (!await IsCustomerInPromotionSegments(promotion, customerTenant.Id, cancellationToken))
                {
                    continue;
                }

                // پیدا کردن شرط‌های مربوط به این رویداد/Trigger
                var matchingConditions = await FindMatchingConditions(
                    promotion.Id, 
                    request, 
                    cancellationToken);

                if (!matchingConditions.Any())
                {
                    // اگر شرطی نداشت، فقط برای Immediate Actions بررسی می‌کنیم
                    var immediateActionsWithoutCondition = await promotionActionRepo.Query()
                        .Where(pa => pa.PromotionId == promotion.Id
                            && pa.ActionType == PromotionActionType.Immediate)
                        .ToListAsync(cancellationToken);

                    if (immediateActionsWithoutCondition.Any())
                    {
                        // اجرای Immediate Actions بدون نیاز به شرط
                        await ExecuteImmediateActions(promotion, request, response, cancellationToken);
                    }
                    continue;
                }

                response.ActivePromotionsCount++;

                // پیدا کردن Actions بر اساس ActionType
                var immediateActions = await promotionActionRepo.Query()
                    .Where(pa => pa.PromotionId == promotion.Id
                        && pa.ActionType == PromotionActionType.Immediate)
                    .ToListAsync(cancellationToken);

                // اگر Immediate Actions داشت، فوری اجرا می‌کنیم (بدون نیاز به Participation)
                if (immediateActions.Any())
                {
                    // بررسی Conditions برای Immediate Actions
                    bool conditionPassed = await EvaluateConditionsForImmediate(
                        matchingConditions, 
                        request, 
                        cancellationToken);

                    if (conditionPassed)
                    {
                        await ExecuteImmediateActions(promotion, request, response, cancellationToken);
                    }
                }

                // برای Campaign ها (Category != RewardsAndPointsPrograms) با OnCondition/OnCompletion
                if (promotion.Category != PromotionCategory.RewardsAndPointsPrograms)
                {
                    // پیدا کردن یا ایجاد Participation
                    var participation = await participationRepo.FirstOrDefaultAsync(
                        p => p.PromotionId == promotion.Id && p.CustomerTenantId == customerTenant.Id,
                        cancellationToken);

                    if (participation == null)
                    {
                        // بررسی محدودیت تعداد شرکت
                        if (promotion.Threshold.HasValue)
                        {
                            var existingParticipations = await participationRepo.Query()
                                .Where(p => p.PromotionId == promotion.Id 
                                    && p.CustomerTenantId == customerTenant.Id
                                    && p.Status == PromotionParticipationStatus.Completed)
                                .CountAsync(cancellationToken);

                            if (existingParticipations >= promotion.Threshold.Value)
                            {
                                logger.LogDebug("Customer {CustomerId} has reached participation threshold for promotion {PromotionId}",
                                    request.Customer.Id, promotion.Id);
                                continue;
                            }
                        }

                        // ایجاد Participation جدید
                        participation = new PromotionParticipation
                        {
                            PromotionId = promotion.Id,
                            CustomerTenantId = customerTenant.Id,
                            StartDate = now,
                            Status = PromotionParticipationStatus.InProgress,
                            ParticipationCount = 1,
                            IsCompleted = false
                        };
                        participationCmdRepo.Add(participation);
                        await participationCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);
                    }
                    else if (participation.Status != PromotionParticipationStatus.InProgress)
                    {
                        continue; // پویش قبلاً تکمیل یا لغو شده
                    }

                    // پردازش هر شرط رویداد (فقط برای Event Type)
                    if (eventLog != null)
                    {
                        var eventConditions = matchingConditions
                            .Where(c => c.Type == PromotionConditionType.Event)
                            .ToList();

                        foreach (var condition in eventConditions)
                        {
                            await ProcessEventCondition(
                                promotion, 
                                condition, 
                                participation, 
                                eventLog, 
                                request, 
                                now, 
                                response,
                                cancellationToken);
                        }

                        // بررسی تکمیل پویش
                        if (await CheckPromotionCompletion(promotion, participation, cancellationToken))
                        {
                            participation.IsCompleted = true;
                            participation.CompletedDate = now;
                            participation.Status = PromotionParticipationStatus.Completed;
                            participationCmdRepo.Update(participation);
                            await participationCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);

                            response.CompletedPromotionsCount++;
                            response.CompletedPromotionIds.Add(promotion.Id);

                            // اجرای اقدامات OnCompletion
                            await ExecuteCompletionActions(promotion, request, response, cancellationToken);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing event for promotion {PromotionId}", promotion.Id);
            }
        }

        return response;
    }

    private async Task<bool> IsCustomerInPromotionSegments(
        Promotion promotion, 
        int customerTenantId, 
        CancellationToken cancellationToken)
    {
        // اگر لیست Segment خالی باشد، برای همه مشتریان
        var promotionSegments = await promotionCustomerSegmentRepo.Query()
            .Where(pcs => pcs.PromotionId == promotion.Id)
            .Select(pcs => pcs.CustomerSegmentId)
            .ToListAsync(cancellationToken);

        if (!promotionSegments.Any())
        {
            return true; // برای همه مشتریان
        }

        // بررسی عضویت مشتری در یکی از Segment ها
        var isMember = await segmentMembershipRepo.AnyAsync(
            m => m.CustomerTenantId == customerTenantId 
                && promotionSegments.Contains(m.SegmentId),
            cancellationToken);

        return isMember;
    }

    private async Task ProcessEventCondition(
        Promotion promotion,
        PromotionCondition condition,
        PromotionParticipation participation,
        EventLog eventLog,
        PromotionProcessingRequest request,
        DateTime now,
        PromotionProcessingResponse response,
        CancellationToken cancellationToken)
    {
        // بررسی توالی و وابستگی
        if (condition.DependencyConditionId.HasValue)
        {
            var dependencyMet = await CheckDependencyCondition(
                participation, 
                condition.DependencyConditionId.Value, 
                cancellationToken);
            
            if (!dependencyMet)
            {
                logger.LogDebug("Dependency condition {DependencyId} not met for condition {ConditionId}",
                    condition.DependencyConditionId.Value, condition.Id);
                return;
            }
        }

        // بررسی حداقل تعداد
        var eventCount = await eventReceivedQueryRepo.Query()
            .Where(er => er.PromotionParticipationId == participation.Id
                && er.PromotionConditionId == condition.Id)
            .CountAsync(cancellationToken);

        if (condition.MinimumCount.HasValue && eventCount >= condition.MinimumCount.Value)
        {
            // حداقل تعداد برقرار شده - نیازی به ثبت دوباره نیست
            return;
        }

        // ثبت رویداد دریافتی
        var eventReceived = new PromotionEventReceived
        {
            PromotionParticipationId = participation.Id,
            PromotionConditionId = condition.Id,
            EventLogId = eventLog.Id,
            ReceivedDate = now,
            SequenceNumber = condition.SequenceOrder,
            IsParallel = condition.IsParallel
        };
        eventReceivedCmdRepo.Add(eventReceived);
        await eventReceivedCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);

        response.ConditionsMetCount++;

        // اجرای اقدامات OnCondition
        await ExecuteConditionActions(promotion, condition, request, response, cancellationToken);
    }

    private async Task<bool> CheckDependencyCondition(
        PromotionParticipation participation,
        int dependencyConditionId,
        CancellationToken cancellationToken)
    {
        var dependencyCondition = await promotionConditionRepo.FirstOrDefaultAsync(
            pc => pc.Id == dependencyConditionId,
            cancellationToken);

        if (dependencyCondition == null)
            return false;

        var dependencyEventCount = await eventReceivedQueryRepo.Query()
            .Where(er => er.PromotionParticipationId == participation.Id
                && er.PromotionConditionId == dependencyConditionId)
            .CountAsync(cancellationToken);

        return dependencyCondition.MinimumCount.HasValue
            ? dependencyEventCount >= dependencyCondition.MinimumCount.Value
            : dependencyEventCount > 0;
    }

    private async Task<bool> CheckPromotionCompletion(
        Promotion promotion,
        PromotionParticipation participation,
        CancellationToken cancellationToken)
    {
        var allConditions = await promotionConditionRepo.Query()
            .Where(pc => pc.PromotionId == promotion.Id
                && pc.Type == PromotionConditionType.Event)
            .ToListAsync(cancellationToken);

        if (!allConditions.Any())
            return false;

        // بررسی همه شرط‌ها
        foreach (var condition in allConditions)
        {
            var eventCount = await eventReceivedQueryRepo.Query()
                .Where(er => er.PromotionParticipationId == participation.Id
                    && er.PromotionConditionId == condition.Id)
                .CountAsync(cancellationToken);

            var requiredCount = condition.MinimumCount ?? 1;
            if (eventCount < requiredCount)
            {
                return false; // این شرط هنوز برقرار نشده
            }
        }

        return true; // همه شرط‌ها برقرار شده‌اند
    }

    private async Task ExecuteConditionActions(
        Promotion promotion,
        PromotionCondition condition,
        PromotionProcessingRequest request,
        PromotionProcessingResponse response,
        CancellationToken cancellationToken)
    {
        var actions = await promotionActionRepo.Query()
            .Where(pa => pa.PromotionId == promotion.Id
                && pa.ActionType == PromotionActionType.OnCondition
                && pa.PromotionConditionId == condition.Id)
            .ToListAsync(cancellationToken);

        // اجرای اقدامات OnCondition
        foreach (var action in actions)
        {
            try
            {
                await promotionActionService.DoActionAsync(request, action, cancellationToken);
                response.ActionsExecutedCount++;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error executing OnCondition action {ActionId} for promotion {PromotionId}, condition {ConditionId}",
                    action.Id, promotion.Id, condition.Id);
            }
        }
        logger.LogDebug("Executed {Count} OnCondition actions for promotion {PromotionId}, condition {ConditionId}",
            actions.Count, promotion.Id, condition.Id);
    }

    private async Task ExecuteCompletionActions(
        Promotion promotion,
        PromotionProcessingRequest request,
        PromotionProcessingResponse response,
        CancellationToken cancellationToken)
    {
        var actions = await promotionActionRepo.Query()
            .Where(pa => pa.PromotionId == promotion.Id
                && pa.ActionType == PromotionActionType.OnCompletion
                && pa.PromotionConditionId == null)
            .ToListAsync(cancellationToken);

        // اجرای اقدامات OnCompletion
        foreach (var action in actions)
        {
            try
            {
                await promotionActionService.DoActionAsync(request, action, cancellationToken);
                response.ActionsExecutedCount++;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error executing OnCompletion action {ActionId} for promotion {PromotionId}",
                    action.Id, promotion.Id);
            }
        }
        logger.LogDebug("Executed {Count} OnCompletion actions for promotion {PromotionId}",
            actions.Count, promotion.Id);
    }


    /// <summary>
    /// پیدا کردن Conditions مطابق با TriggerType و Request
    /// </summary>
    private async Task<List<PromotionCondition>> FindMatchingConditions(
        int promotionId,
        PromotionProcessingRequest request,
        CancellationToken cancellationToken)
    {
        var query = promotionConditionRepo.Query()
            .Where(pc => pc.PromotionId == promotionId);

        // اگر TriggerType مشخص شده باشد، بر اساس آن فیلتر می‌کنیم
        if (request.TriggerType.HasValue)
        {
            query = request.TriggerType.Value switch
            {
                TriggerType.Event => query.Where(pc => pc.Type == PromotionConditionType.Event
                    && pc.EventTypeId == request.EventTypeId
                    && (pc.EventChannelId == null || pc.EventChannelId == request.EventChannelId)),
                TriggerType.UpgradePointLevel => query.Where(pc => pc.Type == PromotionConditionType.UpgradePointLevel
                    && pc.PointLevelId == request.PointLevelId),
                TriggerType.PurchaseAward => query.Where(pc => pc.Type == PromotionConditionType.PurchaseAward
                    && pc.AwardId == request.AwardId),
                TriggerType.ConsumeAward => query.Where(pc => pc.Type == PromotionConditionType.ConsumeAward
                    && pc.AwardId == request.AwardId),
                TriggerType.PurchaseProductOrService => query.Where(pc => pc.Type == PromotionConditionType.PurchaseProduct
                    && pc.ProductId == request.ProductId),
                _ => query
            };
        }
        else
        {
            // اگر TriggerType مشخص نشده، فقط Event Conditions را بررسی می‌کنیم
            query = query.Where(pc => pc.Type == PromotionConditionType.Event
                && pc.EventTypeId == request.EventTypeId
                && (pc.EventChannelId == null || pc.EventChannelId == request.EventChannelId));
        }

        return await query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// بررسی Conditions برای Immediate Actions
    /// </summary>
    private async Task<bool> EvaluateConditionsForImmediate(
        List<PromotionCondition> conditions,
        PromotionProcessingRequest request,
        CancellationToken cancellationToken)
    {
        if (!conditions.Any())
            return true; // اگر شرطی نداشت، پاس می‌شود

        // بررسی Conditions به صورت گروهی (ConditionGroup)
        foreach (var group in conditions.GroupBy(c => c.ConditionGroup))
        {
            bool passGroup = true;
            foreach (var condition in group)
            {
                bool conditionResult = condition.Kind == PromotionConditionKind.Formula
                    ? await EvaluateFormulaCondition(request, condition, cancellationToken)
                    : await EvaluateComparisonCondition(request, condition, cancellationToken);

                if (!conditionResult)
                {
                    passGroup = false;
                    break;
                }
            }

            if (passGroup)
            {
                return true; // حداقل یک گروه پاس شد
            }
        }

        return false;
    }

    /// <summary>
    /// اجرای Immediate Actions
    /// </summary>
    private async Task ExecuteImmediateActions(
        Promotion promotion,
        PromotionProcessingRequest request,
        PromotionProcessingResponse response,
        CancellationToken cancellationToken)
    {
        var actions = await promotionActionRepo.GetAllWithIncludeAsync(
            [pa => pa.Reward, pa => pa.AmountParameter, pa => pa.Point, pa => pa.ExternalApi], cancellationToken,
            pa => pa.PromotionId == promotion.Id
                && pa.ActionType == PromotionActionType.Immediate);

        if (!actions.Any())
            return;

        // اجرای Actions
        var actionTasks = actions.Select(action =>
            promotionActionService.DoActionAsync(request, action, cancellationToken)
                .ContinueWith(t =>
                {
                    if (t.Exception != null)
                    {
                        logger.LogError(t.Exception, "DoPromotionActionAsync {message}", t.Exception.Message);
                    }
                }, TaskContinuationOptions.OnlyOnFaulted)
        );

        response.ActionsExecutedCount += actionTasks.Count();
        await Task.WhenAll(actionTasks);
    }

    private async Task<bool> EvaluateFormulaCondition(
        PromotionProcessingRequest request,
        PromotionCondition condition,
        CancellationToken cancellationToken)
    {
        if (condition.Constraint == null)
            throw new ArgumentException(nameof(condition.Constraint));

        object eval = request.Parameters != null
            ? await evaluateFormulaService.Evaluate(condition.EventTypeId, condition.Constraint, request.Parameters, cancellationToken)
            : evaluateFormulaService.Evaluate(condition.Constraint);

        return Convert.ToBoolean(eval);
    }

    private async Task<bool> EvaluateComparisonCondition(
        PromotionProcessingRequest request,
        PromotionCondition condition,
        CancellationToken cancellationToken)
    {
        object? compareValue = condition.CompareWith switch
        {
            PromotionConditionCompareWith.Point => condition.PointId.HasValue
                ? await customerService.GetPointBalanceAsync(request.TenantId, condition.PointId.Value, request.Customer.Id, cancellationToken)
                : throw new ArgumentNullException(nameof(condition.PointId)),
            PromotionConditionCompareWith.Parameter => condition.EventTypeParameter != null
                ? request.Parameters?.TryGetValue(condition.EventTypeParameter.Key, out string? paramValue) == true && paramValue != null
                    ? (object)paramValue
                    : throw new ArgumentNullException(nameof(condition.EventTypeParameter.Key))
                : throw new ArgumentNullException(nameof(condition.EventTypeParameter)),
            _ => null
        };

        Dictionary<string, string> parameters = new()
        {
            { "_x", compareValue?.ToString() ?? "" },
            { "_y", condition.Value?.ToString() ?? "" }
        };

        string formula = condition.Kind switch
        {
            PromotionConditionKind.EqualTo => "_x == _y",
            PromotionConditionKind.NotEqualTo => "_x != _y",
            PromotionConditionKind.GreaterThan => "_x > _y",
            PromotionConditionKind.LessThan => "_x < _y",
            PromotionConditionKind.GreaterThanOrEqualTo => "_x >= _y",
            PromotionConditionKind.LessThanOrEqualTo => "_x <= _y",
            _ => throw new ArgumentException($"Invalid condition kind: {condition.Kind}")
        };

        return Convert.ToBoolean(await evaluateFormulaService.Evaluate(condition.EventTypeId, formula, parameters, cancellationToken));
    }
}

