namespace Hyper.Integration.Domain.Features.Integrations;

public sealed record IntegrationStatusCount(byte Status, int Count);
public sealed record IntegrationRecentRun(long Id, string ConnectionName, IntegrationProvider Provider,
    byte Status, DateTime StartedAtUtc, DateTime? FinishedAtUtc, int ItemsRead, int ItemsWritten, int ItemsFailed, string? Error);
public sealed record IntegrationDashboardSnapshot(int Connections, int EnabledConnections, int MappedProducts,
    IReadOnlyList<IntegrationStatusCount> Runs, IReadOnlyList<IntegrationStatusCount> Webhooks,
    IReadOnlyList<IntegrationRecentRun> RecentRuns)
{
    public IReadOnlyList<IntegrationStatusCount> Outbox { get; init; } = [];
    public IReadOnlyList<IntegrationRecentOutbox> RecentOutbox { get; init; } = [];
    public IReadOnlyList<IntegrationConnectionHealth> ConnectionsHealth { get; init; } = [];
    public int OutboxCount(byte status) => Outbox.Where(x => x.Status == status).Sum(x => x.Count);
    public int RunCount(byte status) => Runs.Where(x => x.Status == status).Sum(x => x.Count);
    public int WebhookCount(byte status) => Webhooks.Where(x => x.Status == status).Sum(x => x.Count);
    public int PriceDifferenceCount { get; init; }
    public int InventoryDifferenceCount { get; init; }
}

public sealed record IntegrationRecentOutbox(long Id, string ConnectionName, byte Status, int Attempts,
    DateTime CreatedAtUtc, DateTime NextAttemptAtUtc, string? LastError);
public sealed record IntegrationConnectionHealth(long ConnectionId, string ConnectionName,
    IntegrationProvider Provider, bool Enabled, bool TokenExpired, bool HasError,
    DateTime? LastSyncAtUtc, string Status);

public interface IIntegrationDashboardQuery
{
    Task<IntegrationDashboardSnapshot> GetAsync(int shopId, string tenantId, CancellationToken ct);
}

