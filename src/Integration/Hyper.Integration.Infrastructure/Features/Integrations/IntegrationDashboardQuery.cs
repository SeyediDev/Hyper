using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed class IntegrationDashboardQuery(HyperIntegrationContext db, IIntegrationPlatformCatalogPort catalog) : IIntegrationDashboardQuery
{
    public async Task<IntegrationDashboardSnapshot> GetAsync(int shopId, string tenantId, CancellationToken ct)
    {
        var connections = db.ExternalIntegrationConnections.AsNoTracking()
            .Where(x => x.ShopId == shopId && x.TenantId == tenantId);
        var runs = from run in db.IntegrationSyncRuns.AsNoTracking()
                   join connection in connections on run.ConnectionId equals connection.Id
                   select new { Run = run, connection.DisplayName, connection.Provider };
        var inbox = from message in db.IntegrationWebhookInbox.AsNoTracking()
                    join connection in connections on message.ConnectionId equals connection.Id
                    select message;
        // Aggregate each one-to-many relationship independently: no mapping/run/webhook fan-out.
        var connectionCount = await connections.CountAsync(ct);
        var enabledCount = await connections.CountAsync(x => x.IsEnabled, ct);
        var mappingCount = await (from mapping in db.ExternalProductMappings.AsNoTracking()
                                  join connection in connections on mapping.ConnectionId equals connection.Id
                                  where mapping.ShopId == shopId && mapping.IsActive && mapping.HyperProductId > 0
                                  select mapping.Id).CountAsync(ct);
        var mappings = await (from mapping in db.ExternalProductMappings.AsNoTracking()
                              join connection in connections on mapping.ConnectionId equals connection.Id
                              where mapping.ShopId == shopId && mapping.IsActive && mapping.HyperProductId > 0
                              select mapping).ToListAsync(ct);
        var products = await catalog.GetProductsAsync(shopId, tenantId, ct);
        var productById = products.ToDictionary(x => x.ProductId);
        var priceDifferences = mappings.Count(x => x.LastExternalPrice is not null
            && productById.TryGetValue(x.HyperProductId, out var product)
            && x.LastExternalPrice.Value != product.Price);
        var inventoryDifferences = mappings.Count(x => x.LastExternalInventory is not null
            && productById.TryGetValue(x.HyperProductId, out var product)
            && x.LastExternalInventory.Value != product.Stock);
        var runCounts = await runs.GroupBy(x => x.Run.Status)
            .Select(x => new IntegrationStatusCount(x.Key, x.Count())).ToListAsync(ct);
        var webhookCounts = await inbox.GroupBy(x => x.Status)
            .Select(x => new IntegrationStatusCount(x.Key, x.Count())).ToListAsync(ct);
        var recent = await runs.OrderByDescending(x => x.Run.StartedAtUtc).ThenByDescending(x => x.Run.Id).Take(30)
            .Select(x => new IntegrationRecentRun(x.Run.Id, x.DisplayName, x.Provider, x.Run.Status,
                x.Run.StartedAtUtc, x.Run.FinishedAtUtc, x.Run.ItemsRead, x.Run.ItemsWritten, x.Run.ItemsFailed, x.Run.Error))
            .ToListAsync(ct);
        var outbox = from message in db.IntegrationOutbox.AsNoTracking()
                     join connection in connections on message.ConnectionId equals connection.Id
                     select new { Message = message, connection.DisplayName };
        var outboxCounts = await outbox.GroupBy(x => x.Message.Status)
            .Select(x => new IntegrationStatusCount(x.Key, x.Count())).ToListAsync(ct);
        var recentOutbox = await outbox.OrderByDescending(x => x.Message.Id).Take(30)
            .Select(x => new IntegrationRecentOutbox(x.Message.Id, x.DisplayName, x.Message.Status, x.Message.Attempts,
                x.Message.CreatedAtUtc, x.Message.NextAttemptAtUtc, x.Message.LastError)).ToListAsync(ct);
        var now = DateTime.UtcNow;
        var health = await connections.OrderBy(x => x.Id)
            .Select(x => new IntegrationConnectionHealth(x.Id, x.DisplayName, x.Provider, x.IsEnabled,
                x.ExpiresAtUtc != null && x.ExpiresAtUtc <= now, x.LastError != null,
                x.LastSyncAtUtc, !x.IsEnabled ? "Disabled" : x.LastError != null ? "Error" :
                    x.ExpiresAtUtc != null && x.ExpiresAtUtc <= now ? "TokenExpired" : "Healthy"))
            .ToListAsync(ct);
        return new(connectionCount, enabledCount, mappingCount, runCounts, webhookCounts, recent)
            { Outbox = outboxCounts, RecentOutbox = recentOutbox, ConnectionsHealth = health,
              PriceDifferenceCount = priceDifferences, InventoryDifferenceCount = inventoryDifferences };
    }
}
