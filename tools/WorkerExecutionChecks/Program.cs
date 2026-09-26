using System.Collections.Concurrent;
using Hyper.Integration.Domain.Entities.Integrations;
using Hyper.Integration.Domain.Features.Integrations;
using Hyper.IntegrationWorker.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

var checks = 0;
void Check(bool value, string description)
{
    if (!value) throw new InvalidOperationException(description);
    Console.WriteLine($"PASS {++checks}: {description}");
}

try
{
    foreach (var blocked in new[] { "Capture", "Outbox", "Scenario" })
    {
        var state = new State();
        state.Work = async (lane, _, ct) =>
        {
            if (lane == blocked) await Task.Delay(Timeout.InfiniteTimeSpan, ct);
            return false;
        };
        await using var fixture = new Fixture(state);
        await fixture.Worker.StartAsync(default);
        await Task.WhenAll(state.Entered.Values.Select(x => x.Task)).WaitAsync(TimeSpan.FromSeconds(10));
        Check(state.Active[blocked] == 1, $"{blocked} can stay blocked while both peer lanes make progress");
        await fixture.Stop();
        Check(state.Active.Values.All(x => x == 0), $"shutdown cancels and joins in-flight {blocked} operation");
        Check(state.Probes.All(x => x.Disposed) && state.Probes.Select(x => x.Id).Distinct().Count() == state.Probes.Count,
            $"{blocked}: independent asynchronous scopes are all disposed");
        Check(state.Peak.Values.All(x => x == 1), $"{blocked}: each lane has at most one in-flight operation");
        Check(state.Errors.IsEmpty, $"host cancellation of {blocked} is not logged as a failure");
    }

    {
        var state = new State();
        state.Work = (lane, attempt, _) =>
        {
            if (attempt == 1)
            {
                if (lane == "Capture") throw new OperationCanceledException("fixture-secret-body");
                throw new InvalidOperationException("fixture-secret-body");
            }
            return Task.FromResult(false);
        };
        await using var fixture = new Fixture(state, fast: true);
        await fixture.Worker.StartAsync(default);
        await Task.WhenAll(state.Retried.Values.Select(x => x.Task)).WaitAsync(TimeSpan.FromSeconds(10));
        await fixture.Stop();
        Check(state.Counts.Values.All(x => x >= 2), "all lanes retry independently after transient exceptions");
        Check(state.Errors.Count == 3 && state.Errors.Any(x => x.Contains("OperationCanceledException")),
            "provider cancellation without host shutdown is a retryable lane failure");
        Check(state.Errors.All(x => !x.Contains("fixture-secret-body")) && !state.ExceptionWasLogged,
            "logs contain only lane and exception type, never payload or exception object");
        Check(state.Probes.All(x => x.Disposed) && !state.ReusedScope,
            "retry creates fresh scopes after disposing failed contexts");
    }

    {
        var state = new State { FailFirstOutboxResolution = true, FailFirstOutboxDisposal = true };
        state.Work = (lane, attempt, _) => Task.FromResult(lane == "Outbox" && attempt == 1);
        await using var fixture = new Fixture(state, fast: true);
        await fixture.Worker.StartAsync(default);
        await state.Retried["Outbox"].Task.WaitAsync(TimeSpan.FromSeconds(10));
        await fixture.Stop();
        Check(state.OutboxResolutions >= 3, "DI resolution and scope disposal failures do not permanently stop the outbox lane");
        Check(state.Errors.Count == 2 && state.Errors.All(x => x.Contains("Outbox") && !x.Contains("fixture-secret-body")),
            "scope failures are sanitized and attributed to the correct lane");
        Check(state.Counts["Scenario"] > 0 && state.Counts["Capture"] > 0 && state.Probes.All(x => x.Disposed),
            "peer lanes progress and owned scopes are disposed after resolution/disposal failures");
    }

    {
        var state = new State { Work = (lane, _, _) => Task.FromResult(lane != "Capture") };
        await using var fixture = new Fixture(state);
        await fixture.Worker.StartAsync(default);
        await Task.WhenAll(state.Retried["Outbox"].Task, state.Retried["Scenario"].Task,
            state.Entered["Capture"].Task).WaitAsync(TimeSpan.FromSeconds(10));
        await fixture.Stop();
        Check(state.Counts["Outbox"] >= 2 && state.Counts["Scenario"] >= 2,
            "synchronously busy queues yield so neither queue starves its peer");
        Check(state.Counts["Capture"] == 1, "capture waits its own interval even when both queues remain busy");
        Check(state.Probes.All(x => x.Disposed), "busy worker shutdown disposes every per-attempt scope");
    }

    {
        var state = new State();
        await using var fixture = new Fixture(state);
        await fixture.Worker.StartAsync(default);
        await Task.WhenAll(state.Entered.Values.Select(x => x.Task)).WaitAsync(TimeSpan.FromSeconds(10));
        await fixture.Stop();
        Check(state.Counts.Values.All(x => x == 1), "idle queues wait instead of spinning; shutdown interrupts long scheduled delays");
    }
    Check(new IntegrationWorkerOptions() is { CaptureInterval.TotalSeconds: 30, IdleDelay.TotalSeconds: 5 },
        "default capture and idle intervals remain 30 and 5 seconds");
    foreach (var interval in new[] { TimeSpan.Zero, TimeSpan.FromSeconds(-1), TimeSpan.FromHours(2) })
        Check(!new IntegrationWorkerOptions { IdleDelay = interval }.IsValid()
            && !new IntegrationWorkerOptions { CaptureInterval = interval }.IsValid(),
            "invalid schedule rejected: " + interval);
    {
        var state = new State();
        await using var fixture = new Fixture(state, invalid: true);
        await fixture.Worker.StartAsync(default);
        try
        {
            await fixture.Worker.ExecuteTask!.WaitAsync(TimeSpan.FromSeconds(10));
            throw new Exception("Invalid options were accepted");
        }
        catch (InvalidOperationException ex) when (ex.Message == "Invalid integration worker intervals.")
        {
            Check(state.Probes.IsEmpty, "invalid configuration fails before resolving any work service");
        }
    }
    Console.WriteLine($"{checks} worker execution checks passed. No SQL, provider HTTP or production worker host started.");
    return 0;
}
catch (Exception error)
{
    Console.Error.WriteLine(error);
    return 1;
}

sealed class Fixture : IAsyncDisposable
{
    private readonly ServiceProvider services;
    public IntegrationWorker Worker { get; }
    public Fixture(State state, bool fast = false, bool invalid = false)
    {
        var registrations = new ServiceCollection();
        registrations.AddSingleton(state);
        registrations.AddScoped<Probe>();
        registrations.AddScoped<IIntegrationInventoryCapture, Capture>();
        registrations.AddScoped<IIntegrationScenarioQueue, Scenario>();
        registrations.AddScoped<IIntegrationOutbox>(sp =>
        {
            var probe = sp.GetRequiredService<Probe>();
            if (Interlocked.Increment(ref state.OutboxResolutions) == 1 && state.FailFirstOutboxResolution)
                throw new InvalidOperationException("fixture-secret-body");
            return new Outbox(probe);
        });
        services = registrations.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
        Worker = new(services.GetRequiredService<IServiceScopeFactory>(), new WorkerLog(state), Options.Create(new IntegrationWorkerOptions
        {
            CaptureInterval = fast ? TimeSpan.FromMilliseconds(10) : TimeSpan.FromHours(1),
            IdleDelay = invalid ? TimeSpan.Zero : fast ? TimeSpan.FromMilliseconds(10) : TimeSpan.FromHours(1)
        }));
    }
    public async Task Stop()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        await Worker.StopAsync(timeout.Token);
        if (Worker.ExecuteTask is { } task) await task.WaitAsync(timeout.Token);
    }
    public async ValueTask DisposeAsync()
    {
        // Cancel even when an assertion/timeout fails, without abandoning a loop.
        try { await Stop(); } catch (InvalidOperationException) { }
        Worker.Dispose();
        await services.DisposeAsync();
    }
}

sealed class State
{
    public static readonly string[] Lanes = ["Capture", "Outbox", "Scenario"];
    public readonly ConcurrentDictionary<string, int> Counts = new(Lanes.Select(x => new KeyValuePair<string, int>(x, 0)));
    public readonly ConcurrentDictionary<string, int> Active = new(Lanes.Select(x => new KeyValuePair<string, int>(x, 0)));
    public readonly ConcurrentDictionary<string, int> Peak = new(Lanes.Select(x => new KeyValuePair<string, int>(x, 0)));
    public readonly Dictionary<string, TaskCompletionSource> Entered = Lanes.ToDictionary(x => x, _ => new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously));
    public readonly Dictionary<string, TaskCompletionSource> Retried = Lanes.ToDictionary(x => x, _ => new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously));
    public readonly ConcurrentBag<Probe> Probes = [];
    public readonly ConcurrentQueue<string> Errors = [];
    public Func<string, int, CancellationToken, Task<bool>> Work = (_, _, _) => Task.FromResult(false);
    public bool ExceptionWasLogged, ReusedScope, FailFirstOutboxResolution, FailFirstOutboxDisposal;
    public int OutboxResolutions, DisposalFailures;
}

sealed class Probe : IAsyncDisposable
{
    private readonly State state;
    public Probe(State state)
    {
        this.state = state;
        state.Probes.Add(this);
    }
    public Guid Id { get; } = Guid.NewGuid();
    public bool Disposed;
    private string? lane;
    public async Task<bool> Process(string name, CancellationToken ct)
    {
        if (lane is not null || Disposed) state.ReusedScope = true;
        lane = name;
        var attempt = state.Counts.AddOrUpdate(name, 1, (_, count) => count + 1);
        var active = state.Active.AddOrUpdate(name, 1, (_, count) => count + 1);
        state.Peak.AddOrUpdate(name, active, (_, peak) => Math.Max(peak, active));
        state.Entered[name].TrySetResult();
        if (attempt >= 2) state.Retried[name].TrySetResult();
        try { return await state.Work(name, attempt, ct); }
        finally { state.Active.AddOrUpdate(name, 0, (_, count) => count - 1); }
    }
    public ValueTask DisposeAsync()
    {
        Disposed = true;
        if (lane == "Outbox" && state.FailFirstOutboxDisposal && Interlocked.Increment(ref state.DisposalFailures) == 1)
            throw new InvalidOperationException("fixture-secret-body");
        return ValueTask.CompletedTask;
    }
}

sealed class Capture(Probe probe) : IIntegrationInventoryCapture
{
    public async Task<int> CaptureAsync(CancellationToken ct) => await probe.Process("Capture", ct) ? 1 : 0;
    public Task<bool> ReconcileOneAsync(long connectionId, long mappingId, CancellationToken ct) => throw new NotSupportedException();
}
sealed class Outbox(Probe probe) : IIntegrationOutbox
{
    public Task<bool> ProcessNextAsync(CancellationToken ct) => probe.Process("Outbox", ct);
    public Task<long> EnqueueInventoryAsync(long connectionId, long mappingId, long version, decimal quantity, CancellationToken ct) => throw new NotSupportedException();
    public Task<bool> RetryAsync(long connectionId, long messageId, CancellationToken ct) => throw new NotSupportedException();
}
sealed class Scenario(Probe probe) : IIntegrationScenarioQueue
{
    public Task<bool> ProcessNextAsync(CancellationToken ct) => probe.Process("Scenario", ct);
    public Task<long> EnqueueAsync(OwnedIntegrationShop shop, long connectionId, IntegrationScenarioRequest request, CancellationToken ct) => throw new NotSupportedException();
    public Task<IReadOnlyList<IntegrationScenarioConnection>> ConnectionsAsync(OwnedIntegrationShop shop, CancellationToken ct) => throw new NotSupportedException();
    public Task<IReadOnlyList<IntegrationScenarioJob>> RecentAsync(OwnedIntegrationShop shop, CancellationToken ct) => throw new NotSupportedException();
}
sealed class WorkerLog(State state) : ILogger<IntegrationWorker>
{
    public IDisposable? BeginScope<TState>(TState value) where TState : notnull => null;
    public bool IsEnabled(LogLevel level) => true;
    public void Log<TState>(LogLevel level, EventId eventId, TState value, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (level < LogLevel.Error) return;
        if (exception is not null) state.ExceptionWasLogged = true;
        state.Errors.Enqueue(formatter(value, exception));
    }
}
