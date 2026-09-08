using Neo.Application.Features.Queue;

namespace Hyper.Application.Features.Hyper.Jobs;

/// <summary>
/// Job برای اجرای قرعه‌کشی‌های زمان‌بندی شده
/// هر ساعت یک‌بار اجرا می‌شود
/// </summary>
[RecurringJob(
    cronJob: "0 */1 * * *",  // هر ساعت یک‌بار
    queue: "lottery"
)]
public interface IProcessScheduledLotteriesJob : IRecurringJob
{
}

