using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed class AdminOverviewQuery(HyperSqlServerContext accounting, HyperIntegrationContext integrations) : IAdminOverviewQuery
{
    public async Task<AdminOverviewSnapshot> GetAsync(int days, int? shopId, string? tenantId, CancellationToken ct)
    {
        if (days is not (7 or 30)) throw new ArgumentOutOfRangeException(nameof(days));
        if (shopId.HasValue && (shopId <= 0 || string.IsNullOrWhiteSpace(tenantId))) throw new ArgumentException("Shop requires tenant scope.");
        var now = DateTime.UtcNow;
        var periodStart = now.Date.AddDays(1 - days);
        var until = now.Date.AddDays(1);
        var shops = accounting.TblShops.AsNoTracking();
        var products = accounting.TblProducts.AsNoTracking();
        var people = accounting.TblPersons.AsNoTracking();
        var invoices = accounting.TblSaleorders.AsNoTracking();
        var connections = integrations.ExternalIntegrationConnections.AsNoTracking();
        if (shopId.HasValue)
        {
            var id = shopId.Value;
            var tenantless = tenantId == IntegrationConnectionScope.CanonicalTenant(id, null);
            shops = shops.Where(x => x.Shopid == id && (x.TenantId == tenantId || tenantless && (x.TenantId == null || x.TenantId == "")));
            products = products.Where(x => x.Shopid == id && (x.TenantId == tenantId || tenantless && (x.TenantId == null || x.TenantId == "")));
            people = people.Where(x => x.Shopid == id && (x.TenantId == tenantId || tenantless && (x.TenantId == null || x.TenantId == "")));
            invoices = invoices.Where(x => x.Shopid == id && (x.TenantId == tenantId || tenantless && (x.TenantId == null || x.TenantId == "")));
            connections = connections.Where(x => x.ShopId == id && x.TenantId == tenantId);
        }
        var periodInvoices = invoices.Where(x => x.Issuedatetime >= periodStart && x.Issuedatetime < until);
        var runs = from run in integrations.IntegrationSyncRuns.AsNoTracking()
                   join connection in connections on run.ConnectionId equals connection.Id
                   where run.StartedAtUtc >= periodStart && run.StartedAtUtc < until
                   select new { Run = run, connection.DisplayName, connection.Provider };
        var queue = from message in integrations.IntegrationOutbox.AsNoTracking()
                    join connection in connections on message.ConnectionId equals connection.Id select message;
        var inbox = from message in integrations.IntegrationWebhookInbox.AsNoTracking()
                    join connection in connections on message.ConnectionId equals connection.Id select message;
        // Aggregate independently; joins between multiple child collections would inflate counts.
        var shopCount = await shops.CountAsync(ct);
        var productCount = await products.CountAsync(ct);
        var activeProducts = await products.CountAsync(x => x.Isenabled, ct);
        var peopleCount = await people.CountAsync(ct);
        var invoiceCount = await periodInvoices.CountAsync(ct);
        var lowStock = await products.CountAsync(x => x.Isenabled && x.Isstockable && x.Minimumstock != null && x.Accountingstock < x.Minimumstock, ct);
        var connectionCount = await connections.CountAsync(ct);
        var enabled = await connections.CountAsync(x => x.IsEnabled, ct);
        var expired = await connections.CountAsync(x => x.IsEnabled && x.ExpiresAtUtc <= now, ct);
        var runCounts = await runs.GroupBy(x => x.Run.Status).Select(g => new { Status = g.Key, Count = g.Count() }).ToListAsync(ct);
        var queueCounts = await queue.GroupBy(x => x.Status).Select(g => new { Status = g.Key, Count = g.Count() }).ToListAsync(ct);
        var inboxCounts = await inbox.GroupBy(x => x.Status).Select(g => new { Status = g.Key, Count = g.Count() }).ToListAsync(ct);
        var invoiceTrend = await periodInvoices.GroupBy(x => x.Issuedatetime.Date).Select(g => new { Date = g.Key, Count = g.Count() }).ToListAsync(ct);
        var runTrend = await runs.GroupBy(x => x.Run.StartedAtUtc.Date).Select(g => new { Date = g.Key, Success = g.Count(x => x.Run.Status == 1), Failed = g.Count(x => x.Run.Status == 2) }).ToListAsync(ct);
        var providers = await connections.GroupBy(x => x.Provider).Select(g => new AdminProviderOverview(g.Key, g.Count(), g.Count(x => x.IsEnabled), g.Max(x => x.LastSyncAtUtc))).ToListAsync(ct);
        var recent = await runs.OrderByDescending(x => x.Run.StartedAtUtc).ThenByDescending(x => x.Run.Id).Take(6)
            .Select(x => new IntegrationRecentRun(x.Run.Id, x.DisplayName, x.Provider, x.Run.Status, x.Run.StartedAtUtc,
                x.Run.FinishedAtUtc, x.Run.ItemsRead, x.Run.ItemsWritten, x.Run.ItemsFailed, x.Run.Error)).ToListAsync(ct);
        var trend = Enumerable.Range(0, days).Select(offset =>
        {
            var date = periodStart.AddDays(offset);
            var run = runTrend.SingleOrDefault(x => x.Date == date);
            return new AdminOverviewDay(date, invoiceTrend.SingleOrDefault(x => x.Date == date)?.Count ?? 0, run?.Success ?? 0, run?.Failed ?? 0);
        }).ToArray();
        return new(now, days, shopCount, productCount, activeProducts, peopleCount, invoiceCount, lowStock,
            connectionCount, enabled, expired, runCounts.Where(x => x.Status == 1).Sum(x => x.Count), runCounts.Where(x => x.Status == 2).Sum(x => x.Count),
            queueCounts.Where(x => x.Status == 0).Sum(x => x.Count), queueCounts.Where(x => x.Status == 1).Sum(x => x.Count),
            queueCounts.Where(x => x.Status == 2).Sum(x => x.Count), queueCounts.Where(x => x.Status == 3).Sum(x => x.Count),
            inboxCounts.Where(x => x.Status == 0).Sum(x => x.Count), inboxCounts.Where(x => x.Status == 2).Sum(x => x.Count), trend, providers, recent);
    }
}
