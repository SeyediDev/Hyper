using Club.Domain.Entities.Lotteries;
using Club.Domain.Entities.Lotteries.Enums;
using Lottery = Club.Domain.Entities.Lotteries.Lottery;
using LotteryParticipant = Club.Domain.Entities.Lotteries.LotteryParticipant;
using LotteryReward = Club.Domain.Entities.Lotteries.LotteryReward;

namespace Club.Domain.Features;

/// <summary>
/// سرویس قرعه‌کشی - مدیریت قرعه‌کشی‌های چرخونه و زمان‌بندی شده
/// </summary>
public interface ILotteryService
{
    [Telemetry]
    Task<SpinLotteryResponse> SpinLottery(SpinLotteryRequest request, CancellationToken cancellationToken);

    [Telemetry]
    Task ProcessScheduledLotteries(CancellationToken cancellationToken);

    /// <summary>
    /// دریافت قرعه‌کشی‌های زمان‌بندی شده فعال برای نمایش به مشتری
    /// </summary>
    [Telemetry]
    Task<List<ScheduledLotteryDto>> GetScheduledLotteriesForCustomer(
        GetScheduledLotteriesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت قرعه‌کشی بر اساس شناسه
    /// </summary>
    [Telemetry]
    Task<Lottery?> GetLotteryByIdAsync(int lotteryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// اجرای یک قرعه‌کشی زمان‌بندی شده خاص
    /// </summary>
    [Telemetry]
    Task ExecuteScheduledLotteryAsync(int lotteryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت نتایج قرعه‌کشی (برندگان)
    /// </summary>
    [Telemetry]
    Task<List<LotteryWinnerDto>> GetLotteryResultsAsync(int lotteryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت تاریخچه قرعه‌کشی‌های مشتری
    /// </summary>
    [Telemetry]
    Task<List<CustomerLotteryHistoryDto>> GetCustomerLotteryHistoryAsync(
        int customerTenantId,
        int? lotteryId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت جایگاه مشتری در قرعه‌کشی
    /// </summary>
    [Telemetry]
    Task<CustomerLotteryPositionDto?> GetCustomerPositionInLotteryAsync(
        int lotteryId,
        int customerTenantId,
        CancellationToken cancellationToken = default);
}

public record SpinLotteryRequest
{
    public int LotteryId { get; set; }
    public string CustomerId { get; set; } = null!;
}

public record SpinLotteryResponse
{
    public int ParticipantId { get; set; }
    public bool IsWinner { get; set; }
    public int? AwardId { get; set; }
    public int? AwardAmount { get; set; }
    public string? Message { get; set; }
}

/// <summary>
/// درخواست دریافت قرعه‌کشی‌های زمان‌بندی شده
/// </summary>
public record GetScheduledLotteriesRequest
{
    public int TenantId { get; set; }
    public int CustomerTenantId { get; set; }
}

/// <summary>
/// DTO برای نمایش قرعه‌کشی زمان‌بندی شده به مشتری
/// </summary>
public record ScheduledLotteryDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public int? Hour { get; set; }
    public int? Minute { get; set; }
    public SchedulingKind? SchedulingKind { get; set; }
    public int? WinnerCount { get; set; }
    public List<LotteryRewardDto> Rewards { get; set; } = [];
    public bool CanParticipate { get; set; }
    public int? ParticipantChance { get; set; }
}

/// <summary>
/// DTO برای نمایش پاداش‌های قرعه‌کشی
/// </summary>
public record LotteryRewardDto
{
    public int Id { get; set; }
    public string SegmentLabel { get; set; } = null!;
    public string? SegmentMessage { get; set; }
    public string SegmentColor { get; set; } = null!;
    public string? SegmentIcon { get; set; }
    public bool IsJackpot { get; set; }
    public int Amount { get; set; }
    public decimal WinPercentage { get; set; }
    public string? AwardTitle { get; set; }
}

/// <summary>
/// DTO برای نمایش برنده قرعه‌کشی
/// </summary>
public record LotteryWinnerDto
{
    public int ParticipantId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;
    public int? AwardId { get; set; }
    public string? AwardTitle { get; set; }
    public int AwardAmount { get; set; }
    public DateTime AnnouncedAt { get; set; }
    public bool IsAwardDistributed { get; set; }
}

/// <summary>
/// DTO برای نمایش تاریخچه قرعه‌کشی‌های مشتری
/// </summary>
public record CustomerLotteryHistoryDto
{
    public int ParticipationId { get; set; }
    public int LotteryId { get; set; }
    public string LotteryTitle { get; set; } = null!;
    public LotteryType LotteryType { get; set; }
    public DateTime ParticipatedAt { get; set; }
    public bool IsWinner { get; set; }
    public int? AwardId { get; set; }
    public string? AwardTitle { get; set; }
    public int AwardAmount { get; set; }
    public DateTime? AnnouncedAt { get; set; }
    public bool IsAwardDistributed { get; set; }
    public int Chance { get; set; }
}

/// <summary>
/// DTO برای نمایش جایگاه مشتری در قرعه‌کشی
/// </summary>
public record CustomerLotteryPositionDto
{
    public int ParticipationId { get; set; }
    public int LotteryId { get; set; }
    public DateTime ParticipatedAt { get; set; }
    public bool IsWinner { get; set; }
    public int Rank { get; set; }
    public int TotalParticipants { get; set; }
    public int? WinnerRank { get; set; }
    public int TotalWinners { get; set; }
    public int Chance { get; set; }
    public int? AwardId { get; set; }
    public string? AwardTitle { get; set; }
    public int AwardAmount { get; set; }
    public DateTime? AnnouncedAt { get; set; }
    public bool IsAwardDistributed { get; set; }
}

internal class LotteryService(
    ILogger<LotteryService> logger,
    ICustomerService customerService,
    IAwardAssetInternalService awardAssetInternalService,
    IQueryRepository<Lottery, int> lotteryRepo,
    IQueryRepository<LotteryReward, int> lotteryRewardRepo,
    IQueryRepository<LotteryParticipant, int> participantQueryRepo,
    ICommandRepository<LotteryParticipant, int> participantRepo,
    ICommandRepository<CustomerTenant, int> customerTenantRepo,
    IQueryRepository<CustomerSegmentMembership, int> segmentMembershipRepo,
    IQueryRepository<CustomerTransaction, long> customerTransactionQueryRepo,
    ICommandRepository<CustomerTransaction, long> customerTransactionCmdRepo,
    ICommandRepository<EventLog, long> eventLogCmdRepo
    ) : ILotteryService
{
    /// <summary>
    /// اجرای روتین قرعه‌کشی چرخونه - کاربر درخواست می‌دهد و فوراً قرعه‌کشی می‌شود
    /// </summary>
    public async Task<SpinLotteryResponse> SpinLottery(SpinLotteryRequest request, CancellationToken cancellationToken)
    {
        // Get lottery
        Lottery? lottery = await lotteryRepo.FirstOrDefaultAsync(
            x => x.Id == request.LotteryId, cancellationToken)
            ?? throw new NullReferenceException("قرعه‌کشی یافت نشد");

        // Validate lottery is active and within date range
        if (lottery.FromDate.HasValue && lottery.FromDate > DateTime.Now)
            throw new InvalidOperationException("قرعه‌کشی هنوز آغاز نشده است");
        
        if (lottery.ToDate.HasValue && lottery.ToDate < DateTime.Now)
            throw new InvalidOperationException("قرعه‌کشی به پایان رسیده است");

        // Check if lottery is wheel type
        if (lottery.LotteryType != LotteryType.Wheel)
            throw new InvalidOperationException("این قرعه‌کشی از نوع چرخونه نیست");

        // Get customer
        Customer? customer = await customerService.GetCustomer(request.CustomerId, false, null, cancellationToken)
            ?? throw new NullReferenceException("مشتری یافت نشد");

        // Check if customer is in segment (if specified)
        // TODO: Implement segment validation if needed
        // For now, we skip this check as it requires additional repository setup
        
        // Ensure customer-tenant relation
        CustomerTenant? customerTenant = await customerTenantRepo.FirstOrDefaultAsync(
            ct => ct.CustomerId == customer.Id && ct.TenantId == lottery.TenantId, cancellationToken);

        if (customerTenant == null)
        {
            customerTenant = new CustomerTenant
            {
                CustomerId = customer.Id,
                TenantId = lottery.TenantId,
                JoinDate = DateTime.UtcNow,
                IsActive = true
            };
            customerTenantRepo.Add(customerTenant);
            await customerTenantRepo.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Check payment requirement and deduct points if needed
        if (lottery.RequiresPayment && lottery.PaymentPointId.HasValue && lottery.PaymentAmount.HasValue)
        {
            // Get customer's current balance for the payment point type
            var customerBalances = await customerService.GetCustomerPointBalances(customer.Id, lottery.TenantId, cancellationToken);
            var paymentPointBalance = customerBalances.FirstOrDefault(b => b.PointId == lottery.PaymentPointId.Value);

            if (paymentPointBalance == null || paymentPointBalance.Balance < lottery.PaymentAmount.Value)
            {
                throw new InvalidOperationException($"موجودی امتیاز کافی نیست. موجودی فعلی: {paymentPointBalance?.Balance ?? 0}, مبلغ مورد نیاز: {lottery.PaymentAmount.Value}");
            }

            // Deduct points
            var lastTransaction = await customerTransactionQueryRepo.FirstOrDefaultAsync(
                t => t.CustomerTenantId == customerTenant.Id && t.PointId == lottery.PaymentPointId.Value,
                cancellationToken);

            long newBalance = (lastTransaction?.Balance ?? 0) - lottery.PaymentAmount.Value;

            // Create EventLog for lottery payment
            var eventLog = new EventLog
            {
                TenantId = lottery.TenantId,
                CustomerTenantId = customerTenant.Id,
                CustomerTenant = customerTenant,
                TriggerType = TriggerType.PointTransfer
            };

            eventLogCmdRepo.Add(eventLog);
            await eventLogCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);

            var paymentTransaction = new CustomerTransaction
            {
                TenantId = lottery.TenantId,
                CustomerTenantId = customerTenant.Id,
                CustomerTenant = customerTenant,
                PointId = lottery.PaymentPointId.Value,
                TransactionType = CustomerTransactionType.Debit,
                Debit = lottery.PaymentAmount.Value,
                Balance = newBalance,
                EventLogId = eventLog.Id,
                PromotionId = null, // Not related to a promotion
                PromotionActionId = null
            };

            customerTransactionCmdRepo.Add(paymentTransaction);

            // Update last transaction expire date if exists
            if (lastTransaction != null)
            {
                lastTransaction.ExpireDate = DateTime.UtcNow;
                customerTransactionCmdRepo.Update(lastTransaction);
            }

            await customerTransactionCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Deducted {Amount} points (PointId: {PointId}) from customer {CustomerId} for lottery {LotteryId}",
                lottery.PaymentAmount.Value, lottery.PaymentPointId.Value, customer.Id, lottery.Id);
        }

        // Check if customer has already participated (optional - depends on business logic)
        var existingParticipant = await participantQueryRepo.FirstOrDefaultAsync(
            x => x.LotteryId == lottery.Id && x.CustomerTenantId == customerTenant.Id, cancellationToken);
        
        // For now, we allow multiple participations - adjust based on requirements
        
        // Get active rewards for this lottery
        var allRewards = await lotteryRewardRepo.GetAllAsync(cancellationToken);
        var rewards = allRewards
            .Where(x => x.LotteryId == lottery.Id && x.IsActive && x.CanDistribute)
            .ToList();
        
        if (!rewards.Any())
            throw new InvalidOperationException("هیچ پاداش فعالی در این قرعه‌کشی وجود ندارد");
        
        // Select winning reward based on probability
        LotteryReward? winningReward = SelectWinningReward(rewards);

        // Create participant record
        LotteryParticipant participant = new()
        {
            LotteryId = lottery.Id,
            CustomerTenantId = customerTenant.Id,
            CustomerTenant = customerTenant,
            IsWinner = winningReward != null,
            AwardId = winningReward?.AwardId,
            AwardAmount = winningReward?.Amount,
            ParticipatedAt = DateTime.Now
        };

        participantRepo.Add(participant);
        await participantRepo.UnitOfWork.SaveChangesAsync(cancellationToken);

        // If winner, distribute award
        if (winningReward != null && winningReward.CanDistribute)
        {
            // Update distributed count
            winningReward.DistributedCount++;
            
            // Create AwardAsset for winner if applicable
            if (winningReward.Award != null)
            {
                try
                {
                    var awardAssets = await awardAssetInternalService.Create(
                        winningReward.Award,
                        customer,
                        null, // eventLogId - can be linked later
                        null, // ruleId
                        null, // actionId
                        winningReward.Amount,
                        cancellationToken);
                    
                    if (awardAssets.Any())
                    {
                        participant.AwardAssetId = awardAssets.First().Id;
                        participant.IsAwardDistributed = true;
                        participant.AwardDistributedAt = DateTime.Now;
                    }
                    
                    logger.LogInformation("Award distributed to lottery winner: {CustomerTenantId}, Award: {AwardId}, Quantity: {Amount}",
                        customerTenant.Id, winningReward.AwardId, winningReward.Amount);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to distribute award to lottery winner");
                    // Don't fail the entire process
                }
            }

            await participantRepo.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        return new SpinLotteryResponse
        {
            ParticipantId = participant.Id,
            IsWinner = winningReward != null,
            AwardId = winningReward?.AwardId,
            AwardAmount = winningReward?.Amount,
            Message = winningReward != null && winningReward.Award != null ? $"تبریک! شما برنده شدید: {winningReward.Award.Title}" : "متأسفانه این بار برنده نشدید"
        };
    }

    /// <summary>
    /// انتخاب پاداش برنده بر اساس احتمال (WinRate)
    /// </summary>
    private LotteryReward? SelectWinningReward(List<LotteryReward> rewards)
    {
        // Calculate total probability space
        int totalProbability = rewards.Sum(r => r.WinRate);
        
        // Generate random number
        Random random = new();
        int randomValue = random.Next(1, totalProbability + 1);

        // Find winning reward
        int cumulativeProbability = 0;
        foreach (var reward in rewards)
        {
            cumulativeProbability += reward.WinRate;
            if (randomValue <= cumulativeProbability)
            {
                return reward;
            }
        }

        return null; // No winner (based on probabilities)
    }

    /// <summary>
    /// پردازش قرعه‌کشی‌های زمان‌بندی شده
    /// این متد باید به صورت scheduled job اجرا شود
    /// </summary>
    public async Task ProcessScheduledLotteries(CancellationToken cancellationToken)
    {
        DateTime now = DateTime.Now;
        
        // Get active scheduled lotteries
        var allLotteries = await lotteryRepo.GetAllAsync(cancellationToken);
        var scheduledLotteries = allLotteries
            .Where(l => l.LotteryType == LotteryType.Scheduled 
                    && l.IsScheduled 
                    && (l.FromDate == null || l.FromDate <= now)
                    && (l.ToDate == null || l.ToDate >= now))
            .ToList();

        foreach (var lottery in scheduledLotteries)
        {
            // Check if it's time to run this lottery
            if (ShouldRunLottery(lottery, now))
            {
                try
                {
                    await ExecuteScheduledLottery(lottery, now, cancellationToken);
                    logger.LogInformation("Executed scheduled lottery: {LotteryId}", lottery.Id);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to execute scheduled lottery: {LotteryId}", lottery.Id);
                }
            }
        }
    }

    /// <summary>
    /// دریافت قرعه‌کشی بر اساس شناسه
    /// </summary>
    public async Task<Lottery?> GetLotteryByIdAsync(int lotteryId, CancellationToken cancellationToken = default)
    {
        return await lotteryRepo.FirstOrDefaultAsync(
            l => l.Id == lotteryId,
            cancellationToken);
    }

    /// <summary>
    /// اجرای یک قرعه‌کشی زمان‌بندی شده خاص
    /// </summary>
    public async Task ExecuteScheduledLotteryAsync(int lotteryId, CancellationToken cancellationToken = default)
    {
        var lottery = await GetLotteryByIdAsync(lotteryId, cancellationToken);
        if (lottery == null)
        {
            logger.LogWarning("Lottery {LotteryId} not found", lotteryId);
            return;
        }

        DateTime now = DateTime.UtcNow;
        await ExecuteScheduledLottery(lottery, now, cancellationToken);
    }

    /// <summary>
    /// بررسی اینکه آیا قرعه‌کشی باید اجرا شود
    /// </summary>
    private bool ShouldRunLottery(Lottery lottery, DateTime now)
    {
        if (!lottery.IsScheduled || !lottery.SchedulingKind.HasValue)
            return false;

        return lottery.SchedulingKind switch
        {
            SchedulingKind.Daily => true, // Run every day
            SchedulingKind.Weekly => lottery.DayOfWeek.HasValue && (int)now.DayOfWeek == lottery.DayOfWeek,
            SchedulingKind.Monthly => lottery.DayOfMonth.HasValue && now.Day == lottery.DayOfMonth,
            SchedulingKind.Yearly => lottery.DayOfYear.HasValue && now.DayOfYear == lottery.DayOfYear,
            _ => false
        };
    }

    /// <summary>
    /// اجرای قرعه‌کشی زمان‌بندی شده
    /// </summary>
    private async Task ExecuteScheduledLottery(Lottery lottery, DateTime now, CancellationToken cancellationToken)
    {
        // 1. Add pre-lottery participants (from segments and pre-added individuals)
        await AddPreLotteryParticipants(lottery, now, cancellationToken);
        
        // 2. Execute lottery and select winners
        await ExecuteLottery(lottery, now, cancellationToken);
        
        logger.LogInformation("Scheduled lottery {LotteryId} executed successfully", lottery.Id);
    }

    /// <summary>
    /// اجرای قرعه‌کشی و انتخاب برندگان
    /// </summary>
    private async Task ExecuteLottery(Lottery lottery, DateTime now, CancellationToken cancellationToken)
    {
        // Load lottery with rewards and participants
        var lotteryWithData = await lotteryRepo.FirstOrDefaultWithIncludeAsync(
            l => l.LotteryRewards,
            l => l.Id == lottery.Id,
            cancellationToken);

        if (lotteryWithData == null)
        {
            logger.LogWarning("Lottery {LotteryId} not found", lottery.Id);
            return;
        }

        // Get active rewards
        var activeRewards = lotteryWithData.LotteryRewards
            .Where(r => r.IsActive && r.CanDistribute)
            .ToList();

        if (!activeRewards.Any())
        {
            logger.LogWarning("No active rewards found for lottery {LotteryId}", lottery.Id);
            return;
        }

        // Get all participants for this lottery
        var allParticipants = await participantQueryRepo.Query()
            .Where(p => p.LotteryId == lottery.Id && !p.IsWinner)
            .Include(p => p.CustomerTenant)
            .ThenInclude(ct => ct.Customer)
            .ToListAsync(cancellationToken);

        if (!allParticipants.Any())
        {
            logger.LogWarning("No participants found for lottery {LotteryId}", lottery.Id);
            return;
        }

        // Determine number of winners
        int winnerCount = lottery.LotteryType == LotteryType.Scheduled && lottery.WinnerCount.HasValue
            ? lottery.WinnerCount.Value
            : 1; // Default to 1 winner for Wheel type

        // Select winners based on their chances
        var winners = SelectWinners(allParticipants, winnerCount, activeRewards);

        // Distribute awards to winners
        int distributedCount = 0;
        foreach (var winner in winners)
        {
            try
            {
                // Select a reward for this winner
                var winningReward = SelectWinningReward(activeRewards);
                
                if (winningReward == null)
                {
                    logger.LogWarning("No reward selected for winner {ParticipantId}", winner.Id);
                    continue;
                }

                // Update participant as winner
                winner.IsWinner = true;
                winner.LotteryRewardId = winningReward.Id;
                winner.AwardId = winningReward.AwardId;
                winner.AwardAmount = winningReward.Amount;
                winner.AnnouncedAt = now;

                // Distribute award if applicable
                if (winningReward.CanDistribute && winningReward.Award != null)
                {
                    try
                    {
                        var awardAssets = await awardAssetInternalService.Create(
                            winningReward.Award,
                            winner.CustomerTenant.Customer,
                            null, // eventLogId
                            null, // ruleId
                            null, // actionId
                            winningReward.Amount,
                            cancellationToken);

                        if (awardAssets.Any())
                        {
                            winner.AwardAssetId = awardAssets.First().Id;
                            winner.IsAwardDistributed = true;
                            winner.AwardDistributedAt = now;
                        }

                        // Update reward distribution count
                        winningReward.DistributedCount++;
                        distributedCount++;
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Failed to distribute award to winner {ParticipantId}", winner.Id);
                    }
                }

                participantRepo.Update(winner);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing winner {ParticipantId}", winner.Id);
            }
        }

        await participantRepo.UnitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Lottery {LotteryId} executed: {WinnerCount} winners selected, {DistributedCount} awards distributed",
            lottery.Id, winners.Count, distributedCount);
    }

    /// <summary>
    /// انتخاب برندگان بر اساس شانس آن‌ها
    /// </summary>
    private List<LotteryParticipant> SelectWinners(
        List<LotteryParticipant> participants,
        int winnerCount,
        List<LotteryReward> rewards)
    {
        if (participants.Count == 0 || winnerCount <= 0)
            return [];

        // Calculate total chance
        int totalChance = participants.Sum(p => p.Chance);

        if (totalChance <= 0)
        {
            logger.LogWarning("Total chance is zero, cannot select winners");
            return [];
        }

        var winners = new List<LotteryParticipant>();
        var availableParticipants = participants.ToList();
        Random random = new();

        for (int i = 0; i < winnerCount && availableParticipants.Any(); i++)
        {
            // Recalculate total chance for remaining participants
            int remainingTotalChance = availableParticipants.Sum(p => p.Chance);

            if (remainingTotalChance <= 0)
                break;

            // Generate random number
            int randomValue = random.Next(1, remainingTotalChance + 1);

            // Find winner
            int cumulativeChance = 0;
            LotteryParticipant? selectedWinner = null;

            foreach (var participant in availableParticipants)
            {
                cumulativeChance += participant.Chance;
                if (randomValue <= cumulativeChance)
                {
                    selectedWinner = participant;
                    break;
                }
            }

            if (selectedWinner != null)
            {
                winners.Add(selectedWinner);
                availableParticipants.Remove(selectedWinner);
            }
        }

        return winners;
    }

    /// <summary>
    /// اضافه کردن شرکت‌کنندگان قبل از قرعه‌کشی
    /// شامل: افراد از جوامع مختلف و افراد از قبل اضافه شده
    /// برای افراد تکراری، شانس‌ها تجمیع می‌شود
    /// </summary>
    private async Task AddPreLotteryParticipants(Lottery lottery, DateTime now, CancellationToken cancellationToken)
    {
        // Load lottery with customer segments
        var lotteryWithSegments = await lotteryRepo.FirstOrDefaultWithIncludeAsync(
            l => l.CustomerSegments,
            l => l.Id == lottery.Id,
            cancellationToken);

        if (lotteryWithSegments == null)
        {
            logger.LogWarning("Lottery {LotteryId} not found", lottery.Id);
            return;
        }

        // If no segments defined, skip (all customers can participate manually)
        if (!lotteryWithSegments.CustomerSegments.Any())
        {
            logger.LogInformation("Lottery {LotteryId} has no customer segments defined, skipping pre-lottery participant addition", lottery.Id);
            return;
        }

        // Dictionary to aggregate chances for duplicate customers
        var participantChances = new Dictionary<int, int>(); // CustomerTenantId -> Total Chance

        // Process each customer segment
        foreach (var lotterySegment in lotteryWithSegments.CustomerSegments)
        {
            // Get all members of this segment
            var segmentMemberships = await segmentMembershipRepo.Query()
                .Where(sm => sm.SegmentId == lotterySegment.CustomerSegmentId
                    && sm.CustomerTenant.TenantId == lottery.TenantId
                    && sm.CustomerTenant.IsActive)
                .Include(sm => sm.CustomerTenant)
                .ToListAsync(cancellationToken);

            logger.LogInformation("Found {Count} members in segment {SegmentId} for lottery {LotteryId}",
                segmentMemberships.Count, lotterySegment.CustomerSegmentId, lottery.Id);

            // Add each member with their chance
            foreach (var membership in segmentMemberships)
            {
                int customerTenantId = membership.CustomerTenantId;
                int segmentChance = lotterySegment.Chance;

                if (participantChances.ContainsKey(customerTenantId))
                {
                    // Aggregate chance for duplicate customer
                    participantChances[customerTenantId] += segmentChance;
                    logger.LogDebug("Aggregated chance for customer {CustomerTenantId}: +{Chance} (total: {Total})",
                        customerTenantId, segmentChance, participantChances[customerTenantId]);
                }
                else
                {
                    // New participant
                    participantChances[customerTenantId] = segmentChance;
                }
            }
        }

        // Create or update participants
        int addedCount = 0;
        int updatedCount = 0;

        foreach (var (customerTenantId, totalChance) in participantChances)
        {
            // Check if participant already exists
            var existingParticipant = await participantQueryRepo.FirstOrDefaultAsync(
                p => p.LotteryId == lottery.Id && p.CustomerTenantId == customerTenantId,
                cancellationToken);

            if (existingParticipant == null)
            {
                // Create new participant
                var customerTenant = await customerTenantRepo.FirstOrDefaultAsync(
                    ct => ct.Id == customerTenantId,
                    cancellationToken);

                if (customerTenant == null)
                {
                    logger.LogWarning("CustomerTenant {CustomerTenantId} not found, skipping", customerTenantId);
                    continue;
                }

                var participant = new LotteryParticipant
                {
                    LotteryId = lottery.Id,
                    Lottery = lottery,
                    CustomerTenantId = customerTenantId,
                    CustomerTenant = customerTenant,
                    Chance = totalChance,
                    IsWinner = false,
                    ParticipatedAt = now
                };

                participantRepo.Add(participant);
                addedCount++;
            }
            else
            {
                // Update existing participant - aggregate chance
                existingParticipant.Chance += totalChance;
                participantRepo.Update(existingParticipant);
                updatedCount++;
            }
        }

        await participantRepo.UnitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Pre-lottery participants added for lottery {LotteryId}: {Added} new, {Updated} updated (total chances aggregated)",
            lottery.Id, addedCount, updatedCount);
    }

    /// <summary>
    /// دریافت قرعه‌کشی‌های زمان‌بندی شده فعال برای نمایش به مشتری
    /// </summary>
    public async Task<List<ScheduledLotteryDto>> GetScheduledLotteriesForCustomer(
        GetScheduledLotteriesRequest request,
        CancellationToken cancellationToken = default)
    {
        DateTime now = DateTime.UtcNow;

        // Get all scheduled lotteries that are active and within date range
        var lotteries = await lotteryRepo.Query()
            .Where(l => l.TenantId == request.TenantId
                && l.LotteryType == LotteryType.Scheduled
                && l.IsActive
                && (l.FromDate == null || l.FromDate <= now)
                && (l.ToDate == null || l.ToDate >= now))
            .Include(l => l.CustomerSegments)
            .Include(l => l.LotteryRewards)
                .ThenInclude(lr => lr.Award)
            .ToListAsync(cancellationToken);

        if (!lotteries.Any())
        {
            return [];
        }

        // Get customer's segment memberships
        var customerSegments = await segmentMembershipRepo.Query()
            .Where(sm => sm.CustomerTenantId == request.CustomerTenantId
                && sm.CustomerTenant.IsActive)
            .Select(sm => sm.SegmentId)
            .ToListAsync(cancellationToken);

        var result = new List<ScheduledLotteryDto>();

        foreach (var lottery in lotteries)
        {
            // Check if customer is eligible (if segments are defined)
            bool canParticipate = true;
            if (lottery.CustomerSegments.Any())
            {
                // Customer must be in at least one of the lottery's segments
                canParticipate = lottery.CustomerSegments
                    .Any(ls => customerSegments.Contains(ls.CustomerSegmentId));
            }

            if (!canParticipate)
            {
                continue; // Skip this lottery
            }

            // Get participant's chance if already participated
            int? participantChance = null;
            var existingParticipant = await participantQueryRepo.FirstOrDefaultAsync(
                p => p.LotteryId == lottery.Id && p.CustomerTenantId == request.CustomerTenantId,
                cancellationToken);
            if (existingParticipant != null)
            {
                participantChance = existingParticipant.Chance;
            }

            // Calculate next scheduled date
            DateTime? scheduledDate = CalculateNextScheduledDate(lottery, now);

            // Map rewards
            var rewards = lottery.LotteryRewards
                .Where(r => r.IsActive)
                .Select(r => new LotteryRewardDto
                {
                    Id = r.Id,
                    SegmentLabel = r.SegmentLabel,
                    SegmentMessage = r.SegmentMessage,
                    SegmentColor = r.SegmentColor,
                    SegmentIcon = r.SegmentIcon,
                    IsJackpot = r.IsJackpot,
                    Amount = r.Amount,
                    WinPercentage = r.WinPercentage,
                    AwardTitle = r.Award?.Title
                })
                .ToList();

            result.Add(new ScheduledLotteryDto
            {
                Id = lottery.Id,
                Title = lottery.Title,
                Description = lottery.Description,
                FromDate = lottery.FromDate,
                ToDate = lottery.ToDate,
                ScheduledDate = scheduledDate,
                Hour = lottery.Hour,
                Minute = lottery.Minute,
                SchedulingKind = lottery.SchedulingKind,
                WinnerCount = lottery.WinnerCount,
                Rewards = rewards,
                CanParticipate = canParticipate,
                ParticipantChance = participantChance
            });
        }

        return result;
    }

    /// <summary>
    /// محاسبه تاریخ بعدی اجرای قرعه‌کشی زمان‌بندی شده
    /// </summary>
    private DateTime? CalculateNextScheduledDate(Lottery lottery, DateTime now)
    {
        if (!lottery.IsScheduled || !lottery.SchedulingKind.HasValue)
            return null;

        DateTime nextDate = now;

        switch (lottery.SchedulingKind.Value)
        {
            case SchedulingKind.Daily:
                // Next day at specified hour/minute
                nextDate = now.Date.AddDays(1);
                if (lottery.Hour.HasValue)
                    nextDate = nextDate.AddHours(lottery.Hour.Value);
                if (lottery.Minute.HasValue)
                    nextDate = nextDate.AddMinutes(lottery.Minute.Value);
                break;

            case SchedulingKind.Weekly:
                if (lottery.DayOfWeek.HasValue)
                {
                    int daysUntilNext = ((int)lottery.DayOfWeek.Value - (int)now.DayOfWeek + 7) % 7;
                    if (daysUntilNext == 0) daysUntilNext = 7; // Next week
                    nextDate = now.Date.AddDays(daysUntilNext);
                    if (lottery.Hour.HasValue)
                        nextDate = nextDate.AddHours(lottery.Hour.Value);
                    if (lottery.Minute.HasValue)
                        nextDate = nextDate.AddMinutes(lottery.Minute.Value);
                }
                break;

            case SchedulingKind.Monthly:
                if (lottery.DayOfMonth.HasValue)
                {
                    nextDate = new DateTime(now.Year, now.Month, lottery.DayOfMonth.Value);
                    if (nextDate <= now)
                        nextDate = nextDate.AddMonths(1);
                    if (lottery.Hour.HasValue)
                        nextDate = nextDate.AddHours(lottery.Hour.Value);
                    if (lottery.Minute.HasValue)
                        nextDate = nextDate.AddMinutes(lottery.Minute.Value);
                }
                break;

            case SchedulingKind.Yearly:
                if (lottery.DayOfYear.HasValue)
                {
                    nextDate = new DateTime(now.Year, 1, 1).AddDays(lottery.DayOfYear.Value - 1);
                    if (nextDate <= now)
                        nextDate = nextDate.AddYears(1);
                    if (lottery.Hour.HasValue)
                        nextDate = nextDate.AddHours(lottery.Hour.Value);
                    if (lottery.Minute.HasValue)
                        nextDate = nextDate.AddMinutes(lottery.Minute.Value);
                }
                break;
        }

        return nextDate;
    }

    /// <summary>
    /// دریافت نتایج قرعه‌کشی (برندگان)
    /// </summary>
    public async Task<List<LotteryWinnerDto>> GetLotteryResultsAsync(int lotteryId, CancellationToken cancellationToken = default)
    {
        var winners = await participantQueryRepo.Query()
            .Where(p => p.LotteryId == lotteryId && p.IsWinner)
            .Include(p => p.CustomerTenant)
                .ThenInclude(ct => ct.Customer)
            .Include(p => p.Award)
            .OrderByDescending(p => p.AnnouncedAt ?? p.ParticipatedAt)
            .ToListAsync(cancellationToken);

        return winners.Select(w => 
        {
            var customer = w.CustomerTenant.Customer;
            var customerName = !string.IsNullOrWhiteSpace(customer.FirstName) || !string.IsNullOrWhiteSpace(customer.LastName)
                ? $"{customer.FirstName} {customer.LastName}".Trim()
                : customer.MobileNo ?? $"مشتری {w.CustomerTenant.CustomerId}";

            return new LotteryWinnerDto
            {
                ParticipantId = w.Id,
                CustomerId = w.CustomerTenant.CustomerId,
                CustomerName = customerName,
                AwardId = w.AwardId,
                AwardTitle = w.Award?.Title,
                AwardAmount = w.AwardAmount ?? 0,
                AnnouncedAt = w.AnnouncedAt ?? w.ParticipatedAt,
                IsAwardDistributed = w.IsAwardDistributed
            };
        }).ToList();
    }

    /// <summary>
    /// دریافت تاریخچه قرعه‌کشی‌های مشتری
    /// </summary>
    public async Task<List<CustomerLotteryHistoryDto>> GetCustomerLotteryHistoryAsync(
        int customerTenantId,
        int? lotteryId = null,
        CancellationToken cancellationToken = default)
    {
        var query = participantQueryRepo.Query()
            .Where(p => p.CustomerTenantId == customerTenantId);

        if (lotteryId.HasValue)
        {
            query = query.Where(p => p.LotteryId == lotteryId.Value);
        }

        var participations = await query
            .Include(p => p.Lottery)
            .Include(p => p.Award)
            .OrderByDescending(p => p.ParticipatedAt)
            .ToListAsync(cancellationToken);

        return participations.Select(p => new CustomerLotteryHistoryDto
        {
            ParticipationId = p.Id,
            LotteryId = p.LotteryId,
            LotteryTitle = p.Lottery.Title,
            LotteryType = p.Lottery.LotteryType,
            ParticipatedAt = p.ParticipatedAt,
            IsWinner = p.IsWinner,
            AwardId = p.AwardId,
            AwardTitle = p.Award?.Title,
            AwardAmount = p.AwardAmount ?? 0,
            AnnouncedAt = p.AnnouncedAt,
            IsAwardDistributed = p.IsAwardDistributed,
            Chance = p.Chance
        }).ToList();
    }

    /// <summary>
    /// دریافت جایگاه مشتری در قرعه‌کشی
    /// </summary>
    public async Task<CustomerLotteryPositionDto?> GetCustomerPositionInLotteryAsync(
        int lotteryId,
        int customerTenantId,
        CancellationToken cancellationToken = default)
    {
        // Get customer's participation
        var customerParticipation = await participantQueryRepo.FirstOrDefaultAsync(
            p => p.LotteryId == lotteryId && p.CustomerTenantId == customerTenantId,
            cancellationToken);

        if (customerParticipation == null)
        {
            return null; // Customer hasn't participated
        }

        // Get total participants count
        var totalParticipants = await participantQueryRepo.Query()
            .CountAsync(p => p.LotteryId == lotteryId, cancellationToken);

        // Get winners count
        var winnersCount = await participantQueryRepo.Query()
            .CountAsync(p => p.LotteryId == lotteryId && p.IsWinner, cancellationToken);

        // Get customer's rank (position among all participants by participation date)
        var participantsBeforeCustomer = await participantQueryRepo.Query()
            .CountAsync(p => p.LotteryId == lotteryId && p.ParticipatedAt < customerParticipation.ParticipatedAt, cancellationToken);

        int rank = participantsBeforeCustomer + 1;

        // Get customer's rank among winners (if winner)
        int? winnerRank = null;
        if (customerParticipation.IsWinner)
        {
            var winnersBeforeCustomer = await participantQueryRepo.Query()
                .CountAsync(p => p.LotteryId == lotteryId && p.IsWinner && 
                     (p.AnnouncedAt ?? p.ParticipatedAt) < (customerParticipation.AnnouncedAt ?? customerParticipation.ParticipatedAt),
                     cancellationToken);
            winnerRank = winnersBeforeCustomer + 1;
        }

        return new CustomerLotteryPositionDto
        {
            ParticipationId = customerParticipation.Id,
            LotteryId = lotteryId,
            ParticipatedAt = customerParticipation.ParticipatedAt,
            IsWinner = customerParticipation.IsWinner,
            Rank = rank,
            TotalParticipants = totalParticipants,
            WinnerRank = winnerRank,
            TotalWinners = winnersCount,
            Chance = customerParticipation.Chance,
            AwardId = customerParticipation.AwardId,
            AwardTitle = customerParticipation.Award?.Title,
            AwardAmount = customerParticipation.AwardAmount ?? 0,
            AnnouncedAt = customerParticipation.AnnouncedAt,
            IsAwardDistributed = customerParticipation.IsAwardDistributed
        };
    }
}
