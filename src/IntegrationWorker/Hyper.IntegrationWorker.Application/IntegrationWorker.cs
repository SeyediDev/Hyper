using Hyper.Integration.Domain.Features.Integrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hyper.IntegrationWorker.Application;

public sealed class IntegrationWorker(IServiceScopeFactory scopes, ILogger<IntegrationWorker> logger,
    IOptions<IntegrationWorkerOptions>? options = null) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var schedule = options?.Value ?? new IntegrationWorkerOptions();
        if (!schedule.IsValid()) throw new InvalidOperationException("Invalid integration worker intervals.");
        logger.LogInformation("Hyper integration worker started with independent capture, outbox and scenario lanes.");
        // Never share a scoped DbContext across these concurrent lanes. Slow
        // accounting reads or an outbound failure must not stop inbound work.
        return Task.WhenAll(
            RunLaneAsync<IIntegrationInventoryCapture>("Capture", async (capture, ct) =>
            {
                await capture.CaptureAsync(ct);
                return false; // Capture always waits its interval, even after changes.
            }, schedule.CaptureInterval, stoppingToken),
            RunLaneAsync<IIntegrationOutbox>("Outbox", (outbox, ct) => outbox.ProcessNextAsync(ct),
                schedule.IdleDelay, stoppingToken),
            RunLaneAsync<IIntegrationScenarioQueue>("Scenario", (queue, ct) => queue.ProcessNextAsync(ct),
                schedule.IdleDelay, stoppingToken));
    }

    private async Task RunLaneAsync<T>(string lane, Func<T, CancellationToken, Task<bool>> process,
        TimeSpan idleDelay, CancellationToken stoppingToken) where T : notnull
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var processed = false;
            try
            {
                await using var scope = scopes.CreateAsyncScope();
                processed = await process(scope.ServiceProvider.GetRequiredService<T>(), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            catch (Exception error)
            {
                // Exception bodies can contain SQL, credentials or customer payloads.
                processed = false; // Includes scope resolution/disposal failures.
                logger.LogError("Integration worker {Lane} failed ({ErrorType}); durable lease will recover.", lane, error.GetType().Name);
            }
            if (!processed)
            {
                try { await Task.Delay(idleDelay, stoppingToken); }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            }
            else await Task.Yield(); // A synchronously busy lane cannot starve startup/shutdown or its peers.
        }
    }
}

