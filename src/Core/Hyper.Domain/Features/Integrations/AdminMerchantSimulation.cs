namespace Hyper.Domain.Features.Integrations;

public sealed record SimulationShop(int ShopId, string ShopName, string MerchantIdentifier);

public interface IAdminMerchantSimulationService
{
    Task<IReadOnlyList<SimulationShop>> SearchShopsAsync(string? search, CancellationToken ct);
    Task<IntegrationAdminSimulation?> SelectAsync(string adminId, int shopId, CancellationToken ct);
    Task<IntegrationAdminSimulation?> GetAsync(string adminId, Guid simulationId, CancellationToken ct);
    Task EndAsync(string adminId, Guid simulationId, CancellationToken ct);
    Task<IntegrationTokenRequest?> RequestTokenAsync(string adminId, Guid simulationId, int displayedShopId,
        IntegrationProvider provider, IntegrationCredentialType credentialType, CancellationToken ct);
}

public static class AdminSimulationBoundary
{
    public static bool Allows(IntegrationAdminSimulation simulation, string adminId, DateTime utcNow, int? displayedShopId = null) =>
        !string.IsNullOrWhiteSpace(adminId) && string.Equals(simulation.AdminUserId, adminId, StringComparison.Ordinal)
        && simulation.ShopId > 0 && simulation.EndedAtUtc is null && simulation.ExpiresAtUtc > utcNow
        && (displayedShopId is null || displayedShopId == simulation.ShopId);
}
