using Hyper.Integration.Contracts;
using Hyper.Integration.Domain.Features.Integrations;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed class IntegrationDashboardApi(IIntegrationDashboardQuery query) : IIntegrationDashboardApi
{
    public async Task<IntegrationDashboardResponse> GetDashboardAsync(int shopId, string tenantId,
        CancellationToken cancellationToken = default)
    {
        var snapshot = await query.GetAsync(shopId, tenantId, cancellationToken);
        return new(snapshot.Connections, snapshot.EnabledConnections, snapshot.MappedProducts,
            snapshot.Runs.Select(x => new IntegrationDashboardStatusCount(x.Status, x.Count)).ToArray(),
            snapshot.Webhooks.Select(x => new IntegrationDashboardStatusCount(x.Status, x.Count)).ToArray(),
            snapshot.RecentRuns.Select(x => new IntegrationDashboardRun(x.Id, x.ConnectionName,
                ToContractProvider(x.Provider), x.Status, x.StartedAtUtc, x.FinishedAtUtc, x.ItemsRead,
                x.ItemsWritten, x.ItemsFailed, x.Error)).ToArray(),
            snapshot.Outbox.Select(x => new IntegrationDashboardStatusCount(x.Status, x.Count)).ToArray())
        {
            ConnectionsHealth = snapshot.ConnectionsHealth.Select(x => new IntegrationDashboardConnectionHealth(
                x.ConnectionId, x.ConnectionName, ToContractProvider(x.Provider), x.Enabled, x.TokenExpired,
                x.HasError, x.LastSyncAtUtc, x.Status)).ToArray(),
            RecentOutbox = snapshot.RecentOutbox.Select(x => new IntegrationDashboardOutbox(x.Id, x.ConnectionName,
                x.Status, x.Attempts, x.CreatedAtUtc, x.NextAttemptAtUtc, x.LastError)).ToArray(),
            RecentInbox = snapshot.RecentInbox.Select(ToWebhook).ToArray(),
            FailedInbox = snapshot.FailedInbox.Select(ToWebhook).ToArray(),
            DeadLetterOutbox = snapshot.DeadLetterOutbox.Select(x => new IntegrationDashboardOutbox(x.Id,
                x.ConnectionName, x.Status, x.Attempts, x.CreatedAtUtc, x.NextAttemptAtUtc, x.LastError)).ToArray()
        };
    }

    private static IntegrationDashboardWebhook ToWebhook(IntegrationRecentWebhook x) =>
        new(x.Id, x.ConnectionName, x.ExternalEventId, x.EventType, x.Status,
            x.ReceivedAtUtc, x.ProcessedAtUtc, x.Error);

    private static Hyper.Integration.Contracts.IntegrationProvider ToContractProvider(
        Hyper.Integration.Domain.Entities.Integrations.IntegrationProvider provider) => provider switch
    {
        Hyper.Integration.Domain.Entities.Integrations.IntegrationProvider.Basalam => Hyper.Integration.Contracts.IntegrationProvider.Basalam,
        Hyper.Integration.Domain.Entities.Integrations.IntegrationProvider.Digikala => Hyper.Integration.Contracts.IntegrationProvider.Digikala,
        Hyper.Integration.Domain.Entities.Integrations.IntegrationProvider.Torob => Hyper.Integration.Contracts.IntegrationProvider.Torob,
        _ => Hyper.Integration.Contracts.IntegrationProvider.Custom
    };
}
