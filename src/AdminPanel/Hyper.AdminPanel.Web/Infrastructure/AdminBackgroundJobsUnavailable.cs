using System.Linq.Expressions;

namespace Hyper.AdminPanel.Web.Infrastructure;

// Legacy framework services require these contracts during DI validation. The admin host
// does not own job execution; integration work must use the independent outbox worker.
// Fail explicitly if a retired workflow tries to enqueue work, rather than losing it silently.
internal sealed class AdminBackgroundJobsUnavailable : IJobExecuter, IRecurringJobsManager
{
    private static NotSupportedException Disabled() => new("Background jobs are not hosted by Hyper admin. Use the integration outbox worker.");
    public Task<string?> EnqueueAsync<T>(Expression<Func<T, CancellationToken, Task>> call, string queue, CancellationToken ct) where T : IJob => throw Disabled();
    public string Enqueue<T>(Expression<Func<T, Task>> call, string queue) where T : IJob => throw Disabled();
    public string Enqueue(Expression<Action> call, string queue) => throw Disabled();
    public string Schedule<T>(Expression<Func<T, Task>> call, TimeSpan delay, string queue) where T : IJob => throw Disabled();
    public string Schedule(Expression<Action> call, TimeSpan delay, string queue) => throw Disabled();
    public string Schedule<T>(Expression<Func<T, Task>> call, DateTimeOffset at, string queue) where T : IJob => throw Disabled();
    public string Schedule(Expression<Action> call, DateTimeOffset at, string queue) => throw Disabled();
    public bool Requeue(string id) => throw Disabled();
    public bool Reschedule(string id, TimeSpan delay) => throw Disabled();
    public bool Reschedule(string id, DateTimeOffset at) => throw Disabled();
    public bool Delete(string id) => throw Disabled();
    public string ContinueJobWith<T>(string parent, Expression<Action> call, string queue) where T : IJob => throw Disabled();
    public string ContinueJobWith(string parent, Expression<Action> call, string queue) => throw Disabled();
    public string ContinueJobWith<T>(string parent, Expression<Func<T, Task>> call, string queue) where T : IJob => throw Disabled();
    public string ContinueJobWith(string parent, Expression<Func<Task>> call, string queue) => throw Disabled();
    public string ContinueWith<T>(string parent, Expression<Action> call, string queue) where T : IJob => throw Disabled();
    public string ContinueWith(string parent, Expression<Action> call, string queue) => throw Disabled();
    public string ContinueWith<T>(string parent, Expression<Func<T, Task>> call, string queue) where T : IJob => throw Disabled();
    public string ContinueWith(string parent, Expression<Func<Task>> call, string queue) => throw Disabled();
    public void RemoveIfExists(string id) => throw Disabled();
    public void AddOrUpdate<T>(string id, Expression<Func<T, Task>> call, string cron, string queue = "default", string timeZoneName = null!) where T : IRecurringJob => throw Disabled();
    public void Set(string name, DateTime date) => throw Disabled();
    public DateTime? Get(string name) => throw Disabled();
}
