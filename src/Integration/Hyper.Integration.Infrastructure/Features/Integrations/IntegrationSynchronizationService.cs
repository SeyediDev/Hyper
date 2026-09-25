using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed class IntegrationSynchronizationService(
    HyperIntegrationContext db, IIntegrationStrategyResolver strategies) : IIntegrationSynchronizationService
{
    public async Task<long> SynchronizeAsync(long connectionId, CancellationToken cancellationToken = default)
    {
        var connection = await db.ExternalIntegrationConnections.SingleAsync(
            x => x.Id == connectionId, cancellationToken);
        IntegrationConnectionReadiness.Validate(connection, DateTime.UtcNow);
        // Resolve before recording a run: unavailable providers are not successful empty syncs.
        var adapter = strategies.Resolve(connection.Provider, connection.CredentialType);
        var run = new IntegrationSyncRun { ConnectionId = connectionId };
        db.IntegrationSyncRuns.Add(run);
        await db.SaveChangesAsync(cancellationToken);

        try
        {
            var items = await adapter.ReadCatalogAsync(connection, cancellationToken);
            run.ItemsRead = items.Count;
            var mappings = await db.ExternalProductMappings.Where(x =>
                x.ConnectionId == connectionId && x.ShopId == connection.ShopId && x.IsActive)
                .ToListAsync(cancellationToken);
            var mapped = mappings.ToDictionary(x => (x.ExternalProductId, x.ExternalVariantId));
            var observed = new HashSet<(string, string?)>();
            var updates = new List<(ExternalProductMapping Mapping, ExternalCatalogItem Item)>();
            foreach (var item in items)
            {
                if (!observed.Add((item.ExternalProductId, item.VariantId)))
                    throw new InvalidOperationException("The provider returned duplicate catalog identifiers.");
                if (!mapped.TryGetValue((item.ExternalProductId, item.VariantId), out var mapping)
                    || mapping.HyperProductId <= 0)
                {
                    // Catalog discovery cannot manufacture a valid Hyper product mapping.
                    run.ItemsFailed++;
                    continue;
                }
                updates.Add((mapping, item));
            }

            // This operation currently reconciles mapped catalog snapshots only.
            // Inventory publishing belongs to the outbox flow using actual Hyper inventory,
            // never LastExternalInventory read back from the provider.
            cancellationToken.ThrowIfCancellationRequested();
            foreach (var (mapping, item) in updates)
            {
                mapping.LastExternalPrice = item.Price;
                mapping.LastExternalInventory = item.Inventory;
                mapping.LastSyncAtUtc = DateTime.UtcNow;
                run.ItemsWritten++;
            }
            run.Status = run.ItemsFailed == 0 ? (byte)1 : (byte)3; // 3 = requires mapping
            run.Error = run.ItemsFailed == 0 ? null : "Some catalog entries require an approved Hyper product mapping.";
            if (run.ItemsFailed == 0)
            {
                connection.LastSyncAtUtc = DateTime.UtcNow;
                connection.LastError = null;
            }
            else connection.LastError = run.Error;
            run.FinishedAtUtc = DateTime.UtcNow;
            await db.SaveChangesAsync(cancellationToken);
            return run.Id;
        }
        catch (Exception error)
        {
            // Discard uncommitted catalog mutations; persist only the failure record.
            db.ChangeTracker.Clear();
            var failedRun = await db.IntegrationSyncRuns.SingleAsync(x => x.Id == run.Id, CancellationToken.None);
            var failedConnection = await db.ExternalIntegrationConnections.SingleAsync(x => x.Id == connectionId, CancellationToken.None);
            failedRun.Status = 2;
            failedRun.FinishedAtUtc = DateTime.UtcNow;
            failedRun.Error = error is OperationCanceledException ? "Synchronization cancelled." : "Synchronization failed.";
            failedConnection.LastError = failedRun.Error;
            await db.SaveChangesAsync(CancellationToken.None);
            throw;
        }
    }
}
