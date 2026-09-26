using System.Globalization;
using Hyper.Integration.Domain.Entities.Integrations;
using Hyper.Integration.Domain.Features.Integrations;

namespace Hyper.Infrastructure.Features.Integrations;

internal static class IntegrationScenarioScheduling
{
    // Deployment: drain old scenario processors before starting this version.
    // Connection-scoped locks from older builds do not coordinate with this key.
    internal static string LockResource(int shopId) =>
        "Hyper.Integration.Scenarios:Shop:" + shopId.ToString(CultureInfo.InvariantCulture);

    internal static IQueryable<IntegrationScenarioJob> Active(IQueryable<IntegrationScenarioJob> jobs) =>
        jobs.Where(x => x.Status == IntegrationScenarioStatus.Pending || x.Status == IntegrationScenarioStatus.Running);

    internal static IQueryable<IntegrationScenarioJob> ReadyHeads(
        IQueryable<IntegrationScenarioJob> jobs, DateTime now, long? onlyConnection = null)
    {
        var active = Active(jobs);
        // Select one head per shop BEFORE applying readiness/connection/page filters.
        // A retry delay or live lease blocks later events even on another connection;
        // a large tail must not consume the candidate slots of unrelated shops.
        return active.Where(x => !active.Any(earlier => earlier.ShopId == x.ShopId && earlier.Id < x.Id))
            .Where(x => !onlyConnection.HasValue || x.ConnectionId == onlyConnection.Value)
            .Where(x => x.Status == IntegrationScenarioStatus.Pending && x.NextAttemptAtUtc <= now
                || x.Status == IntegrationScenarioStatus.Running && x.LeaseExpiresAtUtc <= now)
            .OrderBy(x => x.Id).Take(50);
    }
}
