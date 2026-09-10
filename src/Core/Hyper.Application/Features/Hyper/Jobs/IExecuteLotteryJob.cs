using Neo.Application.Features.Queue;

namespace Hyper.Application.Features.Hyper.Jobs;

/// <summary>
/// Job برای اجرای یک قرعه‌کشی خاص در زمان مشخص شده
/// این Job به صورت one-time در زمان مشخص شده اجرا می‌شود
/// </summary>
public interface IExecuteLotteryJob : IJob
{
    /// <summary>
    /// اجرای قرعه‌کشی
    /// </summary>
    /// <param name="lotteryId">شناسه قرعه‌کشی</param>
    Task ExecuteLottery(int lotteryId, CancellationToken cancellationToken = default);
}


