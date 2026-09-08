using Neo.Application.Features.Queue;

namespace Hyper.Application.Features.Points.Jobs;

/// <summary>
/// Job برای منقضی کردن امتیازهای منقضی شده
/// هر روز در ساعت 2 صبح اجرا می‌شود
/// </summary>
[RecurringJob(
    cronJob: "0 2 * * *",
    queue: "points"
)]
public interface IExpirePointsJob : IRecurringJob
{
}

