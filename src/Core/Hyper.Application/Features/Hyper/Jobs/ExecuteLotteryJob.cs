using Neo.Application.Features.Queue;
using Hyper.Domain.Features.Promotions;
using Hyper.Domain.Entities.Lotteries;

namespace Hyper.Application.Features.Hyper.Jobs;

/// <summary>
/// پیاده‌سازی Job برای اجرای یک قرعه‌کشی خاص
/// بعد از اجرا، Job بعدی را schedule می‌کند (برای recurring lotteries)
/// </summary>
internal class ExecuteLotteryJob(
    ILotteryService lotteryService,
    ICommandRepository<Lottery, int> lotteryCommandRepo,
    IJobExecuter jobExecuter,
    ILogger<ExecuteLotteryJob> logger
) : IExecuteLotteryJob
{
    public async Task ExecuteLottery(int lotteryId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Executing scheduled lottery {LotteryId} at {Time}", lotteryId, DateTime.UtcNow);

        try
        {
            // Get lottery
            var lottery = await lotteryService.GetLotteryByIdAsync(lotteryId, cancellationToken);
            if (lottery == null)
            {
                logger.LogWarning("Lottery {LotteryId} not found", lotteryId);
                return;
            }

            // Clear scheduled job ID before execution
            lottery.ScheduledJobId = null;
            lotteryCommandRepo.Update(lottery);
            await lotteryCommandRepo.UnitOfWork.SaveChangesAsync(cancellationToken);

            // Execute lottery
            await lotteryService.ExecuteScheduledLotteryAsync(lotteryId, cancellationToken);
            
            // Schedule next execution if lottery is still active and recurring
            DateTime now = DateTime.UtcNow;
            if (lottery.IsActive 
                && lottery.IsScheduled 
                && (lottery.ToDate == null || lottery.ToDate >= now))
            {
                DateTime? nextExecutionDate = CalculateNextExecutionDate(lottery, now);
                if (nextExecutionDate.HasValue && nextExecutionDate.Value > now)
                {
                    string nextJobId = jobExecuter.Schedule<IExecuteLotteryJob>(
                        job => job.ExecuteLottery(lotteryId, CancellationToken.None),
                        nextExecutionDate.Value,
                        "lottery-execution"
                    );

                    lottery.ScheduledJobId = nextJobId;
                    lotteryCommandRepo.Update(lottery);
                    await lotteryCommandRepo.UnitOfWork.SaveChangesAsync(cancellationToken);

                    logger.LogInformation(
                        "Scheduled next execution for lottery {LotteryId} at {NextExecutionDate} with job ID {JobId}",
                        lotteryId, nextExecutionDate.Value, nextJobId);
                }
            }
            
            logger.LogInformation("Successfully executed lottery {LotteryId} at {Time}", lotteryId, DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error executing lottery {LotteryId} at {Time}", lotteryId, DateTime.UtcNow);
            throw;
        }
    }

    /// <summary>
    /// محاسبه تاریخ بعدی اجرای قرعه‌کشی
    /// </summary>
    private DateTime? CalculateNextExecutionDate(Lottery lottery, DateTime now)
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
}

