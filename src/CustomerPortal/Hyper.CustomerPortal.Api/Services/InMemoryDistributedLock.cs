using System.Collections.Concurrent;
using Neo.Application.Features.Outbox;

namespace Hyper.CustomerPortal.Api.Services;

internal sealed class InMemoryDistributedLock : IDistributedLock
{
    private readonly ConcurrentDictionary<string, object> _locks = new();

    public Task<bool> TryAcquireAsync(string key, TimeSpan timeout, CancellationToken ct = default)
    {
        bool acquired = _locks.TryAdd(key, new object());
        return Task.FromResult(acquired);
    }

    public Task ReleaseAsync(string key, CancellationToken ct = default)
    {
        _locks.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    public async Task<bool> ExecuteWithLockAsync(string key, TimeSpan timeout, Func<Task> action, CancellationToken ct = default)
    {
        if (await TryAcquireAsync(key, timeout, ct))
        {
            try
            {
                await action();
                return true;
            }
            finally
            {
                await ReleaseAsync(key, ct);
            }
        }
        return false;
    }
}

