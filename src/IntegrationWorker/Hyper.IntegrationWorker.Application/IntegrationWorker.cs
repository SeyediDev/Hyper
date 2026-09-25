using Hyper.Integration.Domain.Features.Integrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hyper.IntegrationWorker.Application;

public sealed class IntegrationWorker(IServiceScopeFactory scopes, ILogger<IntegrationWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Hyper integration outbox worker started.");
        var nextCapture = DateTime.UtcNow;
        while (!stoppingToken.IsCancellationRequested)
        {
            var processed = false;
            try
            {
                await using var scope = scopes.CreateAsyncScope();
                if (DateTime.UtcNow >= nextCapture)
                {
                    nextCapture = DateTime.UtcNow.AddSeconds(30);
                    try { await scope.ServiceProvider.GetRequiredService<IIntegrationInventoryCapture>().CaptureAsync(stoppingToken); }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { throw; }
                    catch (Exception error) { logger.LogError("Inventory capture failed ({ErrorType}).", error.GetType().Name); }
                }
                processed = await scope.ServiceProvider.GetRequiredService<IIntegrationOutbox>().ProcessNextAsync(stoppingToken);
                // A busy inventory outbox must not starve reconciliation requests.
                var scenarioProcessed = await scope.ServiceProvider.GetRequiredService<IIntegrationScenarioQueue>().ProcessNextAsync(stoppingToken);
                processed = processed || scenarioProcessed;
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            catch (Exception error)
            {
                // Exception bodies can contain SQL, credentials or customer payloads.
                logger.LogError("Integration worker iteration failed ({ErrorType}); durable lease will recover.", error.GetType().Name);
            }
            if (!processed)
            {
                try { await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            }
        }
    }
}

