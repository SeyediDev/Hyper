using Hyper.Domain.Features.Channels;

namespace Hyper.Domain.Features.Promotions;

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
        PromotionProcessingRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// ارزیابی رویداد بدون اجرای اقدامات (برای استعلام)
    /// این متد فقط بررسی می‌کند که چه کمپین‌هایی فعال می‌شوند و چه اقداماتی انجام می‌شوند
    /// اما هیچ action را execute نمی‌کند
    /// </summary>
    /// <param name="request">درخواست ارزیابی رویداد</param>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>نتیجه ارزیابی (شامل اطلاعات کمپین‌های فعال و اقدامات احتمالی)</returns>
    [Telemetry]
    Task<PromotionEvaluationResponse?> EvaluateEventAsync(
        PromotionProcessingRequest request, CancellationToken cancellationToken = default);

}

/// <summary>
/// درخواست پردازش رویداد برای پویش
/// </summary>
public record PromotionProcessingRequest(EventRequest EventRequest, EventResponse EventResponse)
{
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
    /// تعداد پویش‌هایی که محرک‌های آن‌ها فعال شد
    /// </summary>
    public int TriggersActivatedCount { get; set; }

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

/// <summary>
/// پاسخ ارزیابی پویش (برای استعلام - بدون اجرای actions)
/// </summary>
public record PromotionEvaluationResponse
{
    /// <summary>
    /// تعداد پویش‌های فعال که این رویداد برای آن‌ها معتبر است
    /// </summary>
    public int ActivePromotionsCount { get; set; }

    /// <summary>
    /// تعداد پویش‌هایی که محرک‌های آن‌ها فعال می‌شوند
    /// </summary>
    public int TriggersActivatedCount { get; set; }

    /// <summary>
    /// تعداد پویش‌هایی که تکمیل می‌شوند
    /// </summary>
    public int CompletedPromotionsCount { get; set; }

    /// <summary>
    /// تعداد اقداماتی که انجام می‌شوند
    /// </summary>
    public int ActionsToExecuteCount { get; set; }

    /// <summary>
    /// لیست شناسه پویش‌های فعال
    /// </summary>
    public List<int> ActivePromotionIds { get; set; } = [];

    /// <summary>
    /// لیست شناسه پویش‌های تکمیل شده
    /// </summary>
    public List<int> CompletedPromotionIds { get; set; } = [];

    /// <summary>
    /// جزئیات پویش‌های فعال
    /// </summary>
    public List<PromotionEvaluationDetail> PromotionDetails { get; set; } = [];
}

/// <summary>
/// جزئیات ارزیابی یک پویش
/// </summary>
public record PromotionEvaluationDetail
{
    /// <summary>
    /// شناسه پویش
    /// </summary>
    public int PromotionId { get; set; }

    /// <summary>
    /// عنوان پویش
    /// </summary>
    public string PromotionTitle { get; set; } = string.Empty;

    /// <summary>
    /// آیا مشتری در Segment های پویش است؟
    /// </summary>
    public bool IsInSegment { get; set; }

    /// <summary>
    /// آیا محرک‌ها فعال می‌شوند؟
    /// </summary>
    public bool TriggersActivated { get; set; }

    /// <summary>
    /// آیا پویش تکمیل می‌شود؟
    /// </summary>
    public bool WillComplete { get; set; }

    /// <summary>
    /// تعداد اقداماتی که انجام می‌شوند
    /// </summary>
    public int ActionsToExecuteCount { get; set; }

    /// <summary>
    /// لیست اقدامات احتمالی
    /// </summary>
    public List<string> ActionDescriptions { get; set; } = [];
}

internal class PromotionService(
    ILogger<PromotionService> logger,
    IPromotionActionService promotionActionService,
    IEvaluateFormulaService evaluateFormulaService,
    IQueryRepository<PromotionCustomerSegment, int> promotionCustomerSegmentRepo,
    IQueryRepository<Promotion, int> promotionRepo,
    IQueryRepository<PromotionEventReceived, int> eventReceivedQueryRepo,
    IQueryRepository<CustomerSegmentMembership, int> segmentMembershipRepo,
    IQueryRepository<PromotionTrigger, int> promotionTriggerRepo,
    IQueryRepository<PromotionAction, int> promotionActionRepo,
    IQueryRepository<PromotionParticipation, int> participationRepo,
    ICommandRepository<PromotionParticipation, int> participationCmdRepo,
    ICommandRepository<PromotionEventReceived, int> eventReceivedCmdRepo
    ) : IPromotionService
{
    public async Task ProcessScheduledPromotions(CancellationToken cancellationToken)
    {
#if ScheduledPromotion
        DateTime now = DateTime.UtcNow;

        // Get active promotions with scheduled conditions
        var allPromotions = await promotionRepo.Query()
            .Where(p => (p.FromDate == null || p.FromDate <= now)
                    && (p.ToDate == null || p.ToDate >= now)
                    && (p.Status == null || p.Status == PromotionStatus.Active))
            .ToListAsync(cancellationToken);

        // Find scheduled triggers for these promotions
        var scheduledTriggers = await promotionTriggerRepo.Query()
            .Where(pt => allPromotions.Select(p => p.Id).Contains(pt.PromotionId)
                    && pt.IsScheduled
                    && pt.SchedulingKind.HasValue)
            .Include(pt => pt.Promotion)
            .ToListAsync(cancellationToken);

        logger.LogInformation("Found {Count} scheduled triggers to evaluate", scheduledTriggers.Count);

        foreach (var trigger in scheduledTriggers)
        {
            try
            {
                // Check if it's time to run this trigger
                if (ShouldRunTrigger(trigger, now))
                {
                    logger.LogInformation("Executing scheduled trigger: {TriggerId} - {TriggerTitle} for promotion {PromotionId}", 
                        trigger.Id, trigger.Title, trigger.PromotionId);
                    await ExecuteScheduledPromotion(trigger.Promotion, now, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing scheduled trigger {TriggerId}", trigger.Id);
            }
        }
#else
        await Task.CompletedTask;
#endif
    }

#if ScheduledPromotion
    private static bool ShouldRunTrigger(PromotionTrigger trigger, DateTime now)
    {
        if (!trigger.IsScheduled || !trigger.SchedulingKind.HasValue)
            return false;

        // Check hour and minute if specified
        if (trigger.ScheduledHour.HasValue && now.Hour != trigger.ScheduledHour.Value)
            return false;

        if (trigger.ScheduledMinute.HasValue && now.Minute != trigger.ScheduledMinute.Value)
            return false;

        return trigger.SchedulingKind switch
        {
            SchedulingKind.Daily => true, // Run every day at specified time
            SchedulingKind.Weekly => trigger.ScheduledWeekDay.HasValue && now.DayOfWeek == trigger.ScheduledWeekDay.Value,
            SchedulingKind.Monthly => trigger.ScheduledMonthDay.HasValue && now.Day == trigger.ScheduledMonthDay.Value,
            SchedulingKind.Yearly => trigger.ScheduledMonthDay.HasValue 
                                    && trigger.ScheduledMonth.HasValue 
                                    && now.Day == trigger.ScheduledMonthDay.Value
                                    && now.Month == trigger.ScheduledMonth.Value,
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
            segmentMemberships = [.. allMemberships
                .GroupBy(m => m.CustomerTenantId)
                .Select(g => g.First())];
        }
        else
        {
            // اگر جامعه‌هایی تعریف شده بود، فقط مشتریانی که در حداقل یک جامعه عضو هستند
            var allMemberships = await segmentMembershipRepo.GetAllAsync(cancellationToken);
            segmentMemberships = [.. allMemberships
                .Where(m => promotionSegments.Contains(m.SegmentId))
                .GroupBy(m => m.CustomerTenantId)
                .Select(g => g.First())];
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
#endif

    public async Task<PromotionProcessingResponse?> ProcessEventAsync(
        PromotionProcessingRequest request, CancellationToken cancellationToken = default)
    {
        var response = new PromotionProcessingResponse();
        var now = DateTime.UtcNow;
        var tenantId = request.EventRequest.TenantId;
        var customerTenantId = request.EventResponse.CustomerTenantId;

        // 1. پیدا کردن همه پویش‌های فعال 
        var activePromotions = await promotionRepo.Query()
            .Where(p => p.TenantId == tenantId
                && (p.Status == null || p.Status == PromotionStatus.Active)
                && (p.FromDate == null || p.FromDate <= now)
                && (p.ToDate == null || p.ToDate >= now))
            .ToListAsync(cancellationToken);

        if (activePromotions.Count == 0)
        {
            logger.LogDebug("No active promotions found for tenant {TenantId}", tenantId);
            return response;
        }

        // 4. بررسی هر پویش
        foreach (var promotion in activePromotions)
        {
            try
            {
                // بررسی Segment membership
                if (!await IsCustomerInPromotionSegments(promotion, customerTenantId, cancellationToken))
                {
                    continue;
                }

                // پیدا کردن محرک‌های مربوط به این رویداد/Trigger
                var matchingTriggers = await FindMatchingTriggers(promotion.Id, request, cancellationToken);
                if (matchingTriggers.Count == 0)
                {
                    // اگر شرطی نداشت، فقط برای Immediate Actions بررسی می‌کنیم
                    var immediateActionsWithoutCondition = await promotionActionRepo.Query()
                        .Where(pa => pa.PromotionId == promotion.Id
                            && pa.RunTimeType == PromotionActionRunTimeType.OnOneEventCompletion)
                        .ToListAsync(cancellationToken);

                    if (immediateActionsWithoutCondition.Count != 0)
                    {
                        // اجرای Immediate Actions بدون نیاز به شرط
                        await ExecuteImmediateActions(promotion, request, response, cancellationToken);
                    }
                    continue;
                }

                response.ActivePromotionsCount++;

                // پیدا کردن Actions بر اساس RunTimeType
                var immediateActions = await promotionActionRepo.Query()
                    .Where(pa => pa.PromotionId == promotion.Id
                        && pa.RunTimeType == PromotionActionRunTimeType.OnOneEventCompletion)
                    .ToListAsync(cancellationToken);

                // اگر Immediate Actions داشت، فوری اجرا می‌کنیم (بدون نیاز به Participation)
                if (immediateActions.Count != 0)
                {
                    // بررسی Triggers برای Immediate Actions
                    bool triggerPassed = await EvaluateTriggersForImmediate(
                        matchingTriggers, request, cancellationToken);
                    if (triggerPassed)
                    {
                        await ExecuteImmediateActions(promotion, request, response, cancellationToken);
                    }
                }

                // برای Campaign ها (پویش‌هایی که OnTrigger/OnCompletion Actions دارند) با OnTrigger/OnCompletion
                // تشخیص Campaign: وجود Actions با RunTimeType = OnTrigger یا OnCompletion
                var campaignActions = await promotionActionRepo.Query()
                    .Where(pa => pa.PromotionId == promotion.Id
                        && (pa.RunTimeType == PromotionActionRunTimeType.OnOneEventCompletion
                            || pa.RunTimeType == PromotionActionRunTimeType.OnCompletion))
                    .AnyAsync(cancellationToken);

                if (campaignActions)
                {
                    // پیدا کردن یا ایجاد Participation
                    var participation = await participationRepo.FirstOrDefaultAsync(
                        p => p.PromotionId == promotion.Id && p.CustomerTenantId == customerTenantId,
                        cancellationToken);

                    if (participation == null)
                    {
                        // TODO: اگر نیاز به محدودیت تعداد شرکت در سطح پویش هست، باید فیلد MaxParticipationCount به Promotion اضافه شود
                        // فیلد Threshold به PromotionTrigger منتقل شده و برای شمارش رویدادها در سطح محرک استفاده می‌شود

                        // ایجاد Participation جدید
                        participation = new PromotionParticipation
                        {
                            PromotionId = promotion.Id,
                            CustomerTenantId = customerTenantId,
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

                    // پردازش هر محرک رویداد (فقط برای Event Type)
                    var eventTriggers = matchingTriggers
                        .Where(t => t.ReceiveEventType == ReceiveEventType.DynamicEvent)
                        .ToList();

                    foreach (var trigger in eventTriggers)
                    {
                        await ProcessEventTrigger(
                            promotion, trigger, participation, request, now, response, cancellationToken);
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
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing event for promotion {PromotionId}", promotion.Id);
            }
        }

        return response;
    }

    private async Task<bool> IsCustomerInPromotionSegments(
        Promotion promotion, int customerTenantId, CancellationToken cancellationToken)
    {
        // اگر لیست Segment خالی باشد، برای همه مشتریان
        var promotionSegments = await promotionCustomerSegmentRepo.Query()
            .Where(pcs => pcs.PromotionId == promotion.Id)
            .Select(pcs => pcs.CustomerSegmentId)
            .ToListAsync(cancellationToken);

        if (promotionSegments.Count == 0)
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

    private async Task ProcessEventTrigger(
        Promotion promotion, PromotionTrigger trigger, PromotionParticipation participation,
        PromotionProcessingRequest request, DateTime now,
        PromotionProcessingResponse response, CancellationToken cancellationToken)
    {
        // بررسی توالی و وابستگی
        if (trigger.DependencyTriggerId.HasValue)
        {
            var dependencyMet = await CheckDependencyTrigger(
                participation,
                trigger.DependencyTriggerId.Value,
                cancellationToken);

            if (!dependencyMet)
            {
                logger.LogDebug("Dependency trigger {DependencyId} not met for trigger {TriggerId}",
                    trigger.DependencyTriggerId.Value, trigger.Id);
                return;
            }
        }

        // بررسی حداقل تعداد
        var eventCount = await eventReceivedQueryRepo.Query()
            .Where(er => er.PromotionParticipationId == participation.Id
                && er.PromotionTriggerId == trigger.Id)
            .CountAsync(cancellationToken);

        if (trigger.Threshold.HasValue && eventCount >= trigger.Threshold.Value)
        {
            // حداقل تعداد برقرار شده - نیازی به ثبت دوباره نیست
            return;
        }

        // ثبت رویداد دریافتی
        var eventReceived = new PromotionEventReceived
        {
            PromotionParticipationId = participation.Id,
            PromotionTriggerId = trigger.Id,
            EventLogId = request.EventResponse.EventLogId,
            ReceivedDate = now,
            SequenceNumber = trigger.SequenceOrder,
            FlowType = trigger.FlowType
        };
        eventReceivedCmdRepo.Add(eventReceived);
        await eventReceivedCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);

        response.TriggersActivatedCount++;

        // اجرای اقدامات OnTrigger
        await ExecuteTriggerActions(promotion, trigger, request, response, cancellationToken);
    }

    private async Task<bool> CheckDependencyTrigger(
        PromotionParticipation participation, int dependencyTriggerId, CancellationToken cancellationToken)
    {
        var dependencyTrigger = await promotionTriggerRepo.FirstOrDefaultAsync(
            pt => pt.Id == dependencyTriggerId,
            cancellationToken);

        if (dependencyTrigger == null)
            return false;

        var dependencyEventCount = await eventReceivedQueryRepo.Query()
            .Where(er => er.PromotionParticipationId == participation.Id
                && er.PromotionTriggerId == dependencyTriggerId)
            .CountAsync(cancellationToken);

        return dependencyTrigger.Threshold.HasValue
            ? dependencyEventCount >= dependencyTrigger.Threshold.Value
            : dependencyEventCount > 0;
    }

    private async Task<bool> CheckPromotionCompletion(
        Promotion promotion, PromotionParticipation participation, CancellationToken cancellationToken)
    {
        var allTriggers = await promotionTriggerRepo.Query()
            .Where(pt => pt.PromotionId == promotion.Id
                && pt.ReceiveEventType == ReceiveEventType.DynamicEvent)
            .ToListAsync(cancellationToken);

        if (allTriggers.Count == 0)
            return false;

        // بررسی همه محرک‌ها
        foreach (var trigger in allTriggers)
        {
            var eventCount = await eventReceivedQueryRepo.Query()
                .Where(er => er.PromotionParticipationId == participation.Id
                    && er.PromotionTriggerId == trigger.Id)
                .CountAsync(cancellationToken);

            var requiredCount = trigger.Threshold ?? 1;
            if (eventCount < requiredCount)
            {
                return false; // این محرک هنوز برقرار نشده
            }
        }

        return true; // همه محرک‌ها برقرار شده‌اند
    }

    private async Task ExecuteTriggerActions(
        Promotion promotion, PromotionTrigger trigger, PromotionProcessingRequest request,
        PromotionProcessingResponse response, CancellationToken cancellationToken)
    {
        var actions = await promotionActionRepo.Query()
            .Where(pa => pa.PromotionId == promotion.Id
                && pa.RunTimeType == PromotionActionRunTimeType.OnOneEventCompletion
                && pa.PromotionTriggerId == trigger.Id)
            .ToListAsync(cancellationToken);

        // اجرای اقدامات OnTrigger
        foreach (var action in actions)
        {
            try
            {
                await promotionActionService.DoActionAsync(request, action, cancellationToken);
                response.ActionsExecutedCount++;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error executing OnTrigger action {ActionId} for promotion {PromotionId}, trigger {TriggerId}",
                    action.Id, promotion.Id, trigger.Id);
            }
        }
        logger.LogDebug("Executed {Count} OnTrigger actions for promotion {PromotionId}, trigger {TriggerId}",
            actions.Count, promotion.Id, trigger.Id);
    }

    private async Task ExecuteCompletionActions(
        Promotion promotion,
        PromotionProcessingRequest request,
        PromotionProcessingResponse response,
        CancellationToken cancellationToken)
    {
        var actions = await promotionActionRepo.Query()
            .Where(pa => pa.PromotionId == promotion.Id
                && pa.RunTimeType == PromotionActionRunTimeType.OnCompletion
                && pa.PromotionTriggerId == null)
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
    /// پیدا کردن Triggers مطابق با TriggerType و Request
    /// </summary>
    private async Task<List<PromotionTrigger>> FindMatchingTriggers(
        int promotionId, PromotionProcessingRequest request, CancellationToken cancellationToken)
    {
        var query = promotionTriggerRepo.Query()
            .Where(pt => pt.PromotionId == promotionId);
        query = request.EventRequest.ReceiveEventType switch
        {
            ReceiveEventType.DynamicEvent => query.Where(pt => pt.ReceiveEventType == ReceiveEventType.DynamicEvent
                && pt.EventTypeId == request.EventRequest.EventTypeId
                && (pt.ChannelId == null || pt.ChannelId == request.EventRequest.ChannelId)
                && (pt.ProductId == null || pt.ProductId == request.EventRequest.ProductId)
                && (pt.ProductCategoryId == null || pt.ProductCategoryId == request.EventRequest.ProductCategoryId)
                ),
            ReceiveEventType.UpgradePointLevel => query.Where(pt => pt.ReceiveEventType == ReceiveEventType.UpgradePointLevel
                && pt.PointLevelId == request.EventRequest.PointLevelId),
            ReceiveEventType.PurchaseReward or ReceiveEventType.ConsumeReward
                => query.Where(pt => pt.ReceiveEventType == ReceiveEventType.PurchaseReward
                && pt.RewardId == request.EventRequest.RewardId),
            _ => query
        };
        {
            // اگر TriggerType مشخص نشده، فقط Event Triggers را بررسی می‌کنیم
            query = query.Where(pt => pt.ReceiveEventType == ReceiveEventType.DynamicEvent
                && pt.EventTypeId == request.EventRequest.EventTypeId
                && (pt.ChannelId == null || pt.ChannelId == request.EventRequest.ChannelId));
        }

        return await query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// بررسی Triggers برای Immediate Actions
    /// </summary>
    private Task<bool> EvaluateTriggersForImmediate(
        List<PromotionTrigger> triggers, PromotionProcessingRequest request, CancellationToken cancellationToken)
    {
        if (triggers.Count == 0)
            return Task.FromResult(true); // اگر محرکی نداشت، پاس می‌شود

        // بررسی Triggers به صورت گروهی (TriggerGroup)
        foreach (var group in triggers.GroupBy(t => t.TriggerGroup))
        {
            bool passGroup = true;
            foreach (var trigger in group)
            {
                // ارزیابی شرط محرک (اگر شرطی وجود داشته باشد)
                bool triggerResult = evaluateFormulaService.Check(trigger.Condition, request.EventResponse.AttributeValues);

                if (!triggerResult)
                {
                    passGroup = false;
                    break;
                }
            }

            if (passGroup)
            {
                return Task.FromResult(true); // حداقل یک گروه پاس شد
            }
        }

        return Task.FromResult(false);
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
            [pa => pa.Reward, pa => pa.Point, pa => pa.ExternalApi], cancellationToken,
            pa => pa.PromotionId == promotion.Id
                && pa.RunTimeType == PromotionActionRunTimeType.OnOneEventCompletion);

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

    /// <summary>
    /// ارزیابی رویداد بدون اجرای اقدامات (برای استعلام)
    /// </summary>
    public async Task<PromotionEvaluationResponse?> EvaluateEventAsync(
        PromotionProcessingRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = new PromotionEvaluationResponse();
        var now = DateTime.UtcNow;
        var tenantId = request.EventRequest.TenantId;
        var customerTenantId = request.EventResponse.CustomerTenantId;

        // 1. پیدا کردن همه پویش‌های فعال
        var activePromotions = await promotionRepo.Query()
            .Where(p => p.TenantId == tenantId
                && (p.Status == null || p.Status == PromotionStatus.Active)
                && (p.FromDate == null || p.FromDate <= now)
                && (p.ToDate == null || p.ToDate >= now))
            .ToListAsync(cancellationToken);

        if (activePromotions.Count == 0)
        {
            logger.LogDebug("No active promotions found for tenant {TenantId} (evaluation)", tenantId);
            return response;
        }

        // 4. بررسی هر پویش (بدون اجرای actions)
        foreach (var promotion in activePromotions)
        {
            try
            {
                var detail = new PromotionEvaluationDetail
                {
                    PromotionId = promotion.Id,
                    PromotionTitle = promotion.Title ?? string.Empty
                };

                // بررسی Segment membership
                bool isInSegment = await IsCustomerInPromotionSegments(promotion, customerTenantId, cancellationToken);
                detail.IsInSegment = isInSegment;

                if (!isInSegment)
                {
                    continue; // مشتری در Segment نیست
                }

                response.ActivePromotionsCount++;
                response.ActivePromotionIds.Add(promotion.Id);

                // پیدا کردن محرک‌های مربوط به این رویداد
                var matchingTriggers = await FindMatchingTriggers(
                    promotion.Id,
                    request,
                    cancellationToken);

                if (matchingTriggers.Count == 0)
                {
                    // بررسی Immediate Actions بدون شرط
                    var immediateActionsWithoutCondition = await promotionActionRepo.Query()
                        .Where(pa => pa.PromotionId == promotion.Id
                            && pa.RunTimeType == PromotionActionRunTimeType.OnOneEventCompletion)
                        .ToListAsync(cancellationToken);

                    if (immediateActionsWithoutCondition.Count != 0)
                    {
                        detail.TriggersActivated = true;
                        detail.ActionsToExecuteCount = immediateActionsWithoutCondition.Count;
                        detail.ActionDescriptions = immediateActionsWithoutCondition
                            .Select(a => $"{a.ActionKind}")
                            .ToList();
                        response.TriggersActivatedCount++;
                        response.ActionsToExecuteCount += immediateActionsWithoutCondition.Count;
                    }
                    response.PromotionDetails.Add(detail);
                    continue;
                }

                detail.TriggersActivated = true;
                response.TriggersActivatedCount++;

                // بررسی Immediate Actions
                var immediateActions = await promotionActionRepo.Query()
                    .Where(pa => pa.PromotionId == promotion.Id
                        && pa.RunTimeType == PromotionActionRunTimeType.OnOneEventCompletion)
                    .ToListAsync(cancellationToken);

                if (immediateActions.Count != 0)
                {
                    // بررسی اینکه آیا Triggers فعال می‌شوند (بدون اجرای واقعی)
                    bool triggerPassed = await EvaluateTriggersForImmediate(
                        matchingTriggers,
                        request,
                        cancellationToken);

                    if (triggerPassed)
                    {
                        detail.ActionsToExecuteCount += immediateActions.Count;
                        detail.ActionDescriptions.AddRange(immediateActions
                            .Select(a => $"Immediate: {a.ActionKind}"));
                        response.ActionsToExecuteCount += immediateActions.Count;
                    }
                }

                // بررسی Campaign Actions (OnTrigger/OnCompletion)
                var campaignActions = await promotionActionRepo.Query()
                    .Where(pa => pa.PromotionId == promotion.Id
                        && (pa.RunTimeType == PromotionActionRunTimeType.OnOneEventCompletion
                            || pa.RunTimeType == PromotionActionRunTimeType.OnCompletion))
                    .ToListAsync(cancellationToken);

                if (campaignActions.Count != 0 )
                {
                    // بررسی اینکه آیا پویش تکمیل می‌شود (بدون ایجاد Participation واقعی)
                    var eventTriggers = matchingTriggers
                        .Where(t => t.ReceiveEventType == ReceiveEventType.DynamicEvent)
                        .ToList();

                    if (eventTriggers.Count != 0)
                    {
                        // شبیه‌سازی پردازش محرک (بدون ذخیره در دیتابیس)
                        bool willComplete = false;
                        foreach (var trigger in eventTriggers)
                        {
                            // بررسی Threshold (بدون شمارش واقعی)
                            if (trigger.Threshold.HasValue && trigger.Threshold.Value > 1)
                            {
                                // در حالت استعلام، فرض می‌کنیم که Threshold برآورده می‌شود
                                // (در واقعیت باید تعداد رویدادهای قبلی را بررسی کنیم)
                                willComplete = true;
                            }
                            else
                            {
                                willComplete = true;
                            }
                        }

                        if (willComplete)
                        {
                            var onCompletionActions = campaignActions
                                .Where(a => a.RunTimeType == PromotionActionRunTimeType.OnCompletion)
                                .ToList();

                            if (onCompletionActions.Count != 0)
                            {
                                detail.WillComplete = true;
                                detail.ActionsToExecuteCount += onCompletionActions.Count;
                                detail.ActionDescriptions.AddRange(onCompletionActions
                                    .Select(a => $"OnCompletion: {a.ActionKind}"));
                                response.CompletedPromotionsCount++;
                                response.CompletedPromotionIds.Add(promotion.Id);
                                response.ActionsToExecuteCount += onCompletionActions.Count;
                            }
                            else
                            {
                                var onTriggerActions = campaignActions
                                    .Where(a => a.RunTimeType == PromotionActionRunTimeType.OnOneEventCompletion)
                                    .ToList();

                                if (onTriggerActions.Count != 0)
                                {
                                    detail.ActionsToExecuteCount += onTriggerActions.Count;
                                    detail.ActionDescriptions.AddRange(onTriggerActions
                                        .Select(a => $"OnTrigger: {a.ActionKind}"));
                                    response.ActionsToExecuteCount += onTriggerActions.Count;
                                }
                            }
                        }
                    }
                }

                response.PromotionDetails.Add(detail);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error evaluating event for promotion {PromotionId} (evaluation)", promotion.Id);
            }
        }

        return response;
    }
}