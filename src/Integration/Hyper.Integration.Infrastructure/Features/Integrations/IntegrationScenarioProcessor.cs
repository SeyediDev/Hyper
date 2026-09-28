using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed class IntegrationScenarioProcessor(HyperIntegrationContext db, IIntegrationStrategyResolver strategies,
    IIntegrationInventoryCapture inventory, IOptions<IntegrationInventoryCaptureOptions> options,
    IOptions<BasalamOAuthSettings> basalamOptions, BasalamDemoProvisioner demoProvisioner,
    IntegrationBusinessEventDispatcher businessEvents, IIntegrationPlatformCatalogPort catalog)
{
    public async Task<IntegrationScenarioResult> ProcessAsync(IntegrationScenarioJob job, CancellationToken ct)
    {
        var connection = await db.ExternalIntegrationConnections.AsNoTracking().SingleOrDefaultAsync(x =>
            x.Id == job.ConnectionId && x.ShopId == job.ShopId && x.TenantId == job.TenantId, ct)
            ?? throw new IntegrationProviderException("ConnectionScopeChanged", false);
        IntegrationConnectionReadiness.Validate(connection, DateTime.UtcNow);
        if (job.Item == IntegrationSyncItem.Product)
        {
            // Only real notifications need an inbox. Initial/manual/store-side
            // reconciliation reads accounting and must not be routed inbound.
            if (await db.IntegrationWebhookInbox.AnyAsync(x => x.ConnectionId == job.ConnectionId
                && x.ExternalEventId == job.EventId, ct))
                return await businessEvents.DispatchAsync(job, connection, ct);
            if (job.Trigger == IntegrationSyncTrigger.BoothChanged)
                throw new IntegrationProviderException("WebhookInboxNotFound", false);
        }
        else if (job.Item is IntegrationSyncItem.Counterparty
            or IntegrationSyncItem.Sale or IntegrationSyncItem.Purchase
            or IntegrationSyncItem.Subscription or IntegrationSyncItem.Review or IntegrationSyncItem.Chat)
            return await businessEvents.DispatchAsync(job, connection, ct);
        var demoBasalam = connection.Provider == IntegrationProvider.Basalam
            && string.Equals(basalamOptions.Value.Mode, "Demo", StringComparison.OrdinalIgnoreCase);
        if (job.Item == IntegrationSyncItem.Inventory && !options.Value.AccountingStockSourceVerified && !demoBasalam)
            throw new IntegrationProviderException("AccountingStockSourceUnverified", false);
        var checkpoint = job.ResultJson is null ? null : JsonSerializer.Deserialize<IntegrationScenarioResult>(job.ResultJson);
        var delivered = checkpoint?.OutboxMessageIds is { Count: > 0 };
        var deliveryErrors = new List<IntegrationDifference>();
        if (delivered)
        {
            var ids = checkpoint!.OutboxMessageIds!.Distinct().ToArray();
            var operation = job.Item == IntegrationSyncItem.Product ? IntegrationOutbox.ProductOperation : IntegrationOutbox.InventoryOperation;
            var messages = await db.IntegrationOutbox.AsNoTracking().Where(x => ids.Contains(x.Id)
                && x.ConnectionId == connection.Id && x.Operation == operation).ToListAsync(ct);
            if (messages.Count != ids.Length)
                return checkpoint with { AwaitingDelivery = false, Differences = [new(null, null, null, "ReconciliationDeliveryMissing")] };
            if (messages.Any(x => x.Status is 0 or 1)) return checkpoint with { AwaitingDelivery = true };
            if (messages.Any(x => x.Status != 2))
                deliveryErrors.Add(new(null, null, null, "ReconciliationDeliveryFailed"));
        }
        if (demoBasalam)
            await demoProvisioner.EnsureProductMappingsAsync(connection.Id, connection.ShopId, connection.TenantId, ct);
        // All trigger types re-read both authoritative sources. Simulator/webhook payloads cannot overwrite accounting.
        var adapter = strategies.Resolve(connection.Provider, connection.CredentialType);
        var remote = await adapter.ReadCatalogAsync(connection, ct);
        var mappings = await db.ExternalProductMappings.AsNoTracking().Where(x => x.ConnectionId == connection.Id
            && x.ShopId == connection.ShopId && x.IsActive).ToListAsync(ct);
        IntegrationLocalProduct[] local;
        await using (var snapshot = await db.Database.BeginTransactionAsync(ct))
        {
            await IntegrationInventoryReadLock.AcquireAsync(db, job.ShopId, ct);
            var rows = await catalog.GetProductsAsync(job.ShopId, job.TenantId, ct);
            var reserved = await db.InventoryReservationLogs.AsNoTracking()
                .Where(x => x.ShopId == job.ShopId && x.Status == 0 && x.ReleasedAtUtc == null)
                .GroupBy(x => x.HyperProductId).Select(x => new { ProductId = x.Key, Quantity = x.Sum(r => r.Quantity) })
                .ToDictionaryAsync(x => x.ProductId, x => x.Quantity, ct);
            local = rows.Select(x => new IntegrationLocalProduct(x.ProductId, x.Name,
                job.Item == IntegrationSyncItem.Inventory
                    ? IntegrationAvailableInventory.Calculate(x.Stock, reserved.GetValueOrDefault(x.ProductId), x.CanSell) : 0,
                x.Sku, x.Price)).ToArray();
            await snapshot.CommitAsync(ct);
        }
        var result = IntegrationCatalogComparison.Compare(local, remote, mappings, job.Item);
        // Read back after ACK, without automatically resending failed messages or
        // overwriting intervening changes. A new explicit job may reconcile again.
        if (delivered)
            return result with { Enqueued = checkpoint!.Enqueued, OutboxMessageIds = checkpoint.OutboxMessageIds,
                Differences = result.Differences.Concat(deliveryErrors).ToArray() };
        var messageIds = new List<long>();
        var errors = new List<IntegrationDifference>();
        var actionable = result.Differences.Where(x => job.Item == IntegrationSyncItem.Inventory
            ? x.Code is "InventoryMismatch" or "ExternalInventoryUnknown"
            : x.Code is "ProductTitleMismatch" or "ProductPriceMismatch" or "ExternalPriceUnknown")
            .GroupBy(x => (x.HyperProductId, x.ExternalProductId, x.VariantId));
        foreach (var group in actionable)
        {
            var diff = group.First();
            var candidates = mappings.Where(x => x.HyperProductId == diff.HyperProductId
                && x.ExternalProductId == diff.ExternalProductId && x.ExternalVariantId == diff.VariantId).ToArray();
            if (candidates.Length != 1) continue;
            try
            {
                if (inventory is not IIntegrationCatalogReconciliation reconcile)
                    throw new IntegrationProviderException("CatalogReconciliationUnavailable", false);
                var id = job.Item == IntegrationSyncItem.Inventory
                    ? await reconcile.ReconcileInventoryAsync(job.ConnectionId, candidates[0].Id, ct)
                    : await reconcile.ReconcileProductAsync(job.ConnectionId, candidates[0].Id, ct);
                if (id.HasValue) messageIds.Add(id.Value);
                else errors.Add(diff with { Code = "ReconciliationSourceChanged" });
            }
            catch (InvalidOperationException ex) when (ex.Message is "VersionSourceConflict" or "VersionSourceUnassigned")
            { errors.Add(diff with { Code = ex.Message }); }
            catch (IntegrationProviderException ex) when (!ex.Retryable)
            { errors.Add(diff with { Code = ex.Code }); }
        }
        return result with { Enqueued = messageIds.Count, OutboxMessageIds = messageIds,
            AwaitingDelivery = messageIds.Count > 0, Differences = result.Differences.Concat(errors).ToArray() };
    }

}
