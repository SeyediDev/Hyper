using Neo.Application.Features.Queue;

namespace Hyper.Application.Features.Hyper.Jobs;

/// <summary>
/// Job برای زمان‌بندی قرعه‌کشی‌های زمان‌بندی شده
/// این Job به صورت recurring اجرا می‌شود و قرعه‌کشی‌های جدید را schedule می‌کند
/// </summary>
[RecurringJob(
    cronJob: "0 */1 * * *",  // هر ساعت یک‌بار
    queue: "lottery-scheduler"
)]
public interface IScheduleLotteriesJob : IRecurringJob
{
}


