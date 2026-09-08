using System.Linq.Expressions;
using Neo.Application.Features.Queue;

namespace Hyper.CustomerPortal.Api.Services;

internal sealed class InMemoryJobExecuter : IJobExecuter
{
    private static string NewId() => Guid.NewGuid().ToString("N");

    public Task<string?> EnqueueAsync<TJob>(Expression<Func<TJob, CancellationToken, Task>> methodCall, string queue, CancellationToken ct) where TJob : IJob
        => Task.FromResult<string?>(NewId());

    public string Enqueue<TJob>(Expression<Func<TJob, Task>> methodCall, string queue) where TJob : IJob => NewId();

    public string Enqueue(Expression<Action> methodCall, string queue) => NewId();

    public string Schedule<TJob>(Expression<Func<TJob, Task>> methodCall, TimeSpan delay, string queue) where TJob : IJob => NewId();

    public string Schedule(Expression<Action> methodCall, TimeSpan delay, string queue) => NewId();

    public string Schedule<TJob>(Expression<Func<TJob, Task>> methodCall, DateTimeOffset enqueueAt, string queue) where TJob : IJob => NewId();

    public string Schedule(Expression<Action> methodCall, DateTimeOffset enqueueAt, string queue) => NewId();

    public bool Requeue(string jobId) => true;

    public bool Reschedule(string jobId, TimeSpan delay) => true;

    public bool Reschedule(string jobId, DateTimeOffset enqueueAt) => true;

    public bool Delete(string jobId) => true;

    public string ContinueJobWith<TJob>(string parentId, Expression<Action> methodCall, string queue) where TJob : IJob => NewId();

    public string ContinueJobWith(string parentId, Expression<Action> methodCall, string queue) => NewId();

    public string ContinueJobWith<TJob>(string parentId, Expression<Func<TJob, Task>> methodCall, string queue) where TJob : IJob => NewId();

    public string ContinueJobWith(string parentId, Expression<Func<Task>> methodCall, string queue) => NewId();

    public string ContinueWith<TJob>(string parentId, Expression<Action> methodCall, string queue) where TJob : IJob => NewId();

    public string ContinueWith(string parentId, Expression<Action> methodCall, string queue) => NewId();

    public string ContinueWith<TJob>(string parentId, Expression<Func<TJob, Task>> methodCall, string queue) where TJob : IJob => NewId();

    public string ContinueWith(string parentId, Expression<Func<Task>> methodCall, string queue) => NewId();
}

