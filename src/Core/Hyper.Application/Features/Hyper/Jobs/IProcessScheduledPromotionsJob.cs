using Neo.Application.Features.Queue;

namespace Hyper.Application.Features.Hyper.Jobs;

/// <summary>
/// Job برای اجرای پویش‌های زمان‌بندی شده
/// هر 30 دقیقه یک‌بار اجرا می‌شود
/// </summary>
[RecurringJob(
    cronJob: "0 */30 * * * *",  // هر 30 دقیقه یک‌بار
    queue: "promotion"
)]
public interface IProcessScheduledPromotionsJob : IRecurringJob
{
}

