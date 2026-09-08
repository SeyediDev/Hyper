using Neo.Application.Features.Queue;
using Hyper.Domain.Entities.Lotteries.Enums;
using Hyper.Domain.Entities.Lotteries;

namespace Hyper.Application.Features.Hyper.Jobs;

/// <summary>
/// پیاده‌سازی Job برای زمان‌بندی قرعه‌کشی‌های زمان‌بندی شده
/// این Job قرعه‌کشی‌های جدید را پیدا می‌کند و آنها را در زمان مشخص شده schedule می‌کند
/// </summary>
internal class ScheduleLotteriesJob(
    IQueryRepository<Lottery, int> lotteryQueryRepo,
    ICommandRepository<Lottery, int> lotteryCommandRepo,
    IJobExecuter jobExecuter,
    ILogger<ScheduleLotteriesJob> logger
) : IScheduleLotteriesJob
{
    public async Task Run()
    {
        logger.LogInformation("Starting lottery scheduling at {Time}", DateTime.UtcNow);

        try
        {
            DateTime now = DateTime.UtcNow;

            // Get all active scheduled lotteries that haven't been scheduled yet
            var lotteries = await lotteryQueryRepo.Query()
                .Where(l => l.LotteryType == LotteryType.Scheduled
                    && l.IsScheduled
                    && l.IsActive
                    && (l.FromDate == null || l.FromDate <= now)
                    && (l.ToDate == null || l.ToDate >= now)
                    && (string.IsNullOrEmpty(l.ScheduledJobId) || !IsJobValid(l.ScheduledJobId))) // Only schedule if not already scheduled or job is invalid
                .ToListAsync();

            foreach (var lottery in lotteries)
            {
                try
                {
                    DateTime? nextExecutionDate = CalculateNextExecutionDate(lottery, now);
                    if (nextExecutionDate.HasValue && nextExecutionDate.Value > now)
                    {
                        // Schedule the lottery execution job
                        string jobId = jobExecuter.Schedule<IExecuteLotteryJob>(
                            job => job.ExecuteLottery(lottery.Id, CancellationToken.None),
                            nextExecutionDate.Value,
                            "lottery-execution"
                        );

                        // Store job ID in lottery
                        lottery.ScheduledJobId = jobId;
                        lotteryCommandRepo.Update(lottery);
                        await lotteryCommandRepo.UnitOfWork.SaveChangesAsync(CancellationToken.None);
                        
                        logger.LogInformation(
                            "Scheduled lottery {LotteryId} to execute at {ExecutionDate} with job ID {JobId}",
                            lottery.Id, nextExecutionDate.Value, jobId);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to schedule lottery {LotteryId}", lottery.Id);
                }
            }

            logger.LogInformation("Completed lottery scheduling at {Time}", DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in lottery scheduling at {Time}", DateTime.UtcNow);
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

    /// <summary>
    /// بررسی اینکه آیا Job معتبر است یا خیر
    /// در صورت نیاز می‌توان از Hangfire API برای بررسی استفاده کرد
    /// </summary>
    private bool IsJobValid(string? jobId)
    {
        if (string.IsNullOrEmpty(jobId))
            return false;

        // TODO: Implement job validation using Hangfire API if needed
        // For now, we assume job is valid if it exists
        return true;
    }
}

