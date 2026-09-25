using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;

namespace Hyper.Infrastructure.Features.Integrations;

/// <summary>Dashboard query; platform accounting data is consumed through a port.</summary>
public sealed class AdminOverviewQuery(IIntegrationPlatformOverviewPort platform, HyperIntegrationContext integrations)
    : IAdminOverviewQuery
{
    public async Task<AdminOverviewSnapshot> GetAsync(int days, int? shopId, string? tenantId, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var start = now.Date.AddDays(1 - days);
        var until = now.Date.AddDays(1);
        var data = await platform.GetAsync(days, shopId, tenantId, ct);
        var connections = integrations.ExternalIntegrationConnections.AsNoTracking();
        if (shopId.HasValue) connections = connections.Where(x => x.ShopId == shopId && x.TenantId == tenantId);
        var runs = from run in integrations.IntegrationSyncRuns.AsNoTracking()
                   join connection in connections on run.ConnectionId equals connection.Id
                   where run.StartedAtUtc >= start && run.StartedAtUtc < until
                   select new { Run = run, connection.DisplayName, connection.Provider };
        var queue = from message in integrations.IntegrationOutbox.AsNoTracking()
                    join connection in connections on message.ConnectionId equals connection.Id select message;
        var inbox = from message in integrations.IntegrationWebhookInbox.AsNoTracking()
                    join connection in connections on message.ConnectionId equals connection.Id select message;
        var connectionCount = await connections.CountAsync(ct);
        var enabled = await connections.CountAsync(x => x.IsEnabled, ct);
        var expired = await connections.CountAsync(x => x.IsEnabled && x.ExpiresAtUtc <= now, ct);
        var runCounts = await runs.GroupBy(x => x.Run.Status).Select(g => new { g.Key, Count = g.Count() }).ToListAsync(ct);
        var queueCounts = await queue.GroupBy(x => x.Status).Select(g => new { g.Key, Count = g.Count() }).ToListAsync(ct);
        var inboxCounts = await inbox.GroupBy(x => x.Status).Select(g => new { g.Key, Count = g.Count() }).ToListAsync(ct);
        var runTrend = await runs.GroupBy(x => x.Run.StartedAtUtc.Date)
            .Select(g => new { Date = g.Key, Success = g.Count(x => x.Run.Status == 1), Failed = g.Count(x => x.Run.Status == 2) }).ToListAsync(ct);
        var providers = await connections.GroupBy(x => x.Provider)
            .Select(g => new AdminProviderOverview(g.Key, g.Count(), g.Count(x => x.IsEnabled), g.Max(x => x.LastSyncAtUtc))).ToListAsync(ct);
        var recent = await runs.OrderByDescending(x => x.Run.StartedAtUtc).ThenByDescending(x => x.Run.Id).Take(6)
            .Select(x => new IntegrationRecentRun(x.Run.Id, x.DisplayName, x.Provider, x.Run.Status, x.Run.StartedAtUtc,
                x.Run.FinishedAtUtc, x.Run.ItemsRead, x.Run.ItemsWritten, x.Run.ItemsFailed, x.Run.Error)).ToListAsync(ct);
        var trend = Enumerable.Range(0, days).Select(offset =>
        {
            var date = start.AddDays(offset);
            var run = runTrend.SingleOrDefault(x => x.Date == date);
            return new AdminOverviewDay(date, data.InvoiceTrend.SingleOrDefault(x => x.Date == date)?.Invoices ?? 0,
                run?.Success ?? 0, run?.Failed ?? 0);
        }).ToArray();
        return new(now, days, data.Shops, data.Products, data.ActiveProducts, data.People, data.Invoices,
            data.LowStockProducts, connectionCount, enabled, expired, runCounts.Where(x => x.Key == 1).Sum(x => x.Count),
            runCounts.Where(x => x.Key == 2).Sum(x => x.Count), queueCounts.Where(x => x.Key == 0).Sum(x => x.Count),
            queueCounts.Where(x => x.Key == 1).Sum(x => x.Count), queueCounts.Where(x => x.Key == 2).Sum(x => x.Count),
            queueCounts.Where(x => x.Key == 3).Sum(x => x.Count), inboxCounts.Where(x => x.Key == 0).Sum(x => x.Count),
            inboxCounts.Where(x => x.Key == 2).Sum(x => x.Count), trend, providers, recent);
    }
}
