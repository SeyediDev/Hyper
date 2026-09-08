using Neo.Application.Features.Queue;

namespace Hyper.CustomerPortal.Application.Features.Plans.Jobs;

/// <summary>
/// Job برای منقضی کردن طرح‌های اشتراک که زمان آن‌ها به پایان رسیده است
/// هر نیم ساعت اجرا می‌شود
/// </summary>
[RecurringJob(
    cronJob: "*/30 * * * *",
    queue: "plan"
)]
public interface IExpireCustomerPlansJob : IRecurringJob
{
}