using Hyper.Integration.Domain.Features.Integrations;
using Microsoft.Extensions.Logging;

namespace Hyper.Infrastructure.Features.Integrations;

internal static class IntegrationInventoryCaptureAttempt
{
    internal static async Task<bool> RunAsync(long connectionId, long mappingId,
        Func<CancellationToken, Task<bool>> capture, ILogger? logger, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        try { return await capture(ct); }
        catch (InvalidOperationException ex) when (ex.Message is "VersionSourceConflict" or "VersionSourceUnassigned")
        {
            // Ownership can change after candidate selection; the next scan excludes it.
            return false;
        }
        catch (Exception ex) when (!ct.IsCancellationRequested
            && ex is IntegrationProviderException or HttpRequestException or OperationCanceledException)
        {
            // CaptureOneAsync has unwound its transaction before reaching this catch.
            // No same-pass retry or replacement quantity: the next scan re-reads accounting.
            // Do not catch SQL/unknown errors: reusing their DbContext may be unsafe.
            logger?.LogWarning("Inventory capture deferred for connection {ConnectionId}, mapping {MappingId} ({ErrorType}); continuing scan.",
                connectionId, mappingId, ex.GetType().Name);
            return false;
        }
    }
}
