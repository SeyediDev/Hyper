using Hyper.Infrastructure.Features.Integrations;
using Hyper.Integration.Domain.Features.Integrations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

var checks = 0;
void Check(bool value, string name)
{
    if (!value) throw new InvalidOperationException(name);
    Console.WriteLine($"PASS {++checks}: {name}");
}

foreach (var failure in new Exception[]
{
    new IntegrationProviderException("AccountingApi_503", true),
    new IntegrationProviderException("AccountingApi_429", true, TimeSpan.FromMinutes(2)),
    new IntegrationProviderException("AccountingApi_403", false),
    new HttpRequestException("fixture-secret-url"),
    new TaskCanceledException("fixture-secret-timeout")
})
{
    var visited = new List<int>();
    var log = new CaptureLog();
    var captured = 0;
    for (var mapping = 1; mapping <= 3; mapping++)
    {
        var current = mapping;
        if (await IntegrationInventoryCaptureAttempt.RunAsync(100 + current, current, _ =>
        {
            visited.Add(current);
            return current == 1 ? Task.FromException<bool>(failure) : Task.FromResult(current == 2);
        }, log, default)) captured++;
    }
    Check(visited.SequenceEqual([1, 2, 3]) && captured == 1,
        $"{failure.GetType().Name}: failed, changed and unchanged mappings each attempted once; healthy mapping proceeds");
    Check(log.Messages.Count == 1 && log.Messages[0].Contains("101") && log.Messages[0].Contains("mapping 1")
        && !log.Messages[0].Contains(failure.Message) && log.Exceptions.All(x => x is null),
        "diagnostic retains mapping identity without exception body or object");
}

foreach (var conflict in new[] { "VersionSourceConflict", "VersionSourceUnassigned" })
{
    var log = new CaptureLog();
    Check(!await IntegrationInventoryCaptureAttempt.RunAsync(1, 2,
        _ => Task.FromException<bool>(new InvalidOperationException(conflict)), log, default)
        && log.Messages.Count == 0, "existing source-ownership race remains a quiet skip: " + conflict);
}

var unexpected = new InvalidOperationException("unexpected persistence failure");
try
{
    await IntegrationInventoryCaptureAttempt.RunAsync(1, 2, _ => Task.FromException<bool>(unexpected), new CaptureLog(), default);
    throw new Exception("Unexpected failure was swallowed");
}
catch (InvalidOperationException ex) when (ReferenceEquals(ex, unexpected))
{ Check(true, "unexpected/persistence failure escapes so worker discards its scope"); }

using (var cancelled = new CancellationTokenSource())
{
    cancelled.Cancel();
    var called = false;
    try
    {
        await IntegrationInventoryCaptureAttempt.RunAsync(1, 2, _ => { called = true; return Task.FromResult(true); }, null, cancelled.Token);
        throw new Exception("Cancelled scan continued");
    }
    catch (OperationCanceledException)
    { Check(!called, "already cancelled scan does not start another mapping"); }
}
using (var stopping = new CancellationTokenSource())
{
    var log = new CaptureLog();
    try
    {
        await IntegrationInventoryCaptureAttempt.RunAsync(1, 2, token =>
        {
            stopping.Cancel();
            return Task.FromCanceled<bool>(token);
        }, log, stopping.Token);
        throw new Exception("Host shutdown swallowed");
    }
    catch (OperationCanceledException)
    { Check(log.Messages.Count == 0, "host cancellation escapes without a provider-failure warning"); }
}

var attempts = 0;
Task<bool> Recover(CancellationToken _) => ++attempts == 1
    ? Task.FromException<bool>(new HttpRequestException()) : Task.FromResult(true);
Check(!await IntegrationInventoryCaptureAttempt.RunAsync(1, 2, Recover, null, default) && attempts == 1,
    "failed capture is not retried in a tight loop");
Check(await IntegrationInventoryCaptureAttempt.RunAsync(1, 2, Recover, null, default) && attempts == 2,
    "next scan can recover the previously failed mapping");

var disposed = false;
async Task<bool> FailAfterCleanup(CancellationToken _)
{
    try { await Task.Yield(); throw new HttpRequestException(); }
    finally { disposed = true; }
}
Check(!await IntegrationInventoryCaptureAttempt.RunAsync(1, 2, FailAfterCleanup, null, default) && disposed,
    "failed attempt unwinds asynchronous cleanup before scan proceeds");
var disabled = new IntegrationInventoryCapture(null!, null!, null!,
    Options.Create(new IntegrationInventoryCaptureOptions()), null!);
Check(await disabled.CaptureAsync(default) == 0
    && !await disabled.ReconcileOneAsync(1, 2, default),
    "unverified accounting source remains disabled before touching any dependency");
Console.WriteLine($"{checks} capture checks passed. Production attempt policy with controlled delegates and disabled-source gate; no SQL, HTTP, real transaction or financial posting executed.");

sealed class CaptureLog : ILogger
{
    public List<string> Messages { get; } = [];
    public List<Exception?> Exceptions { get; } = [];
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
    public bool IsEnabled(LogLevel level) => true;
    public void Log<TState>(LogLevel level, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        Messages.Add(formatter(state, exception));
        Exceptions.Add(exception);
    }
}
