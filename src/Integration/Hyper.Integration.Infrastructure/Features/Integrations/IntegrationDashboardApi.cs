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
            snapshot.Outbox.Select(x => new IntegrationDashboardStatusCount(x.Status, x.Count)).ToArray());
    }

    private static Hyper.Integration.Contracts.IntegrationProvider ToContractProvider(
        Hyper.Integration.Domain.Entities.Integrations.IntegrationProvider provider) => provider switch
    {
        Hyper.Integration.Domain.Entities.Integrations.IntegrationProvider.Basalam => Hyper.Integration.Contracts.IntegrationProvider.Basalam,
        Hyper.Integration.Domain.Entities.Integrations.IntegrationProvider.Digikala => Hyper.Integration.Contracts.IntegrationProvider.Digikala,
        Hyper.Integration.Domain.Entities.Integrations.IntegrationProvider.Torob => Hyper.Integration.Contracts.IntegrationProvider.Torob,
        _ => Hyper.Integration.Contracts.IntegrationProvider.Custom
    };
}
