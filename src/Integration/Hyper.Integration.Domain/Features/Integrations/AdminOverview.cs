namespace Hyper.Integration.Domain.Features.Integrations;

public sealed record AdminOverviewDay(DateTime Date, int Invoices, int SuccessfulRuns, int FailedRuns);
public sealed record AdminProviderOverview(IntegrationProvider Provider, int Connections, int Enabled, DateTime? LastSyncAtUtc);
public sealed record AdminOverviewSnapshot(DateTime GeneratedAtUtc, int Days, int Shops, int Products, int ActiveProducts,
    int People, int Invoices, int LowStockProducts, int Connections, int EnabledConnections, int ExpiredConnections,
    int SuccessfulRuns, int FailedRuns, int PendingOutbox, int SendingOutbox, int DeliveredOutbox, int FailedOutbox,
    int PendingWebhooks, int FailedWebhooks, IReadOnlyList<AdminOverviewDay> Trend,
    IReadOnlyList<AdminProviderOverview> Providers, IReadOnlyList<IntegrationRecentRun> RecentRuns);

public interface IAdminOverviewQuery
{
    Task<AdminOverviewSnapshot> GetAsync(int days, int? shopId, string? tenantId, CancellationToken ct);
}

