using Hyper.Domain.Features.Promotions;

namespace Hyper.Application.Features.Hyper.Jobs;

/// <summary>
/// پیاده‌سازی Job برای اجرای قرعه‌کشی‌های زمان‌بندی شده
/// </summary>
public class ProcessScheduledLotteriesJob(
    ILotteryService lotteryService,
    ILogger<ProcessScheduledLotteriesJob> logger
    ) : IProcessScheduledLotteriesJob
{
    public async Task Run()
    {
        logger.LogInformation("Starting scheduled lotteries processing at {Time}", DateTime.UtcNow);

        try
        {
            await lotteryService.ProcessScheduledLotteries(CancellationToken.None);
            logger.LogInformation("Completed scheduled lotteries processing at {Time}", DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing scheduled lotteries at {Time}", DateTime.UtcNow);
            throw;
        }
    }
}

