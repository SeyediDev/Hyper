using Microsoft.EntityFrameworkCore;

using Hyper.Infrastructure.Data.Repository.Hyper;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed class AdminMerchantSimulationService(IIntegrationPlatformShopPort platformShops, HyperIntegrationContext integrations)
    : IAdminMerchantSimulationService
{
    public async Task<IReadOnlyList<SimulationShop>> SearchShopsAsync(string? search, CancellationToken ct)
    {
        var shops = await platformShops.SearchAsync(search, ct);
        return shops.OrderBy(x => x.ShopName).ThenBy(x => x.ShopId).Take(100)
            .Select(x => new SimulationShop(x.ShopId, x.ShopName, x.MerchantIdentifier)).ToList();
    }

    public async Task<IntegrationAdminSimulation?> SelectAsync(string adminId, int shopId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(adminId) || adminId.Length > 128) return null;
        var shop = await platformShops.GetAsync(shopId, ct);
        if (shop is null || string.IsNullOrWhiteSpace(shop.MerchantIdentifier)) return null;
        var selection = new IntegrationAdminSimulation
        {
            AdminUserId = adminId, ShopId = shop.ShopId, ShopName = shop.ShopName,
            MerchantIdentifier = shop.MerchantIdentifier, TenantId = IntegrationConnectionScope.CanonicalTenant(shop.ShopId, shop.TenantId),
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(30)
        };
        integrations.IntegrationAdminSimulations.Add(selection);
        await integrations.SaveChangesAsync(ct);
        return selection;
    }

    public async Task<IntegrationAdminSimulation?> GetAsync(string adminId, Guid simulationId, CancellationToken ct)
    {
        var selection = await integrations.IntegrationAdminSimulations.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == simulationId, ct);
        if (selection is null || !AdminSimulationBoundary.Allows(selection, adminId, DateTime.UtcNow)) return null;
        // Ownerid is a displayed business identifier, never the administrator's authentication identity.
        var shop = await platformShops.GetAsync(selection.ShopId, ct);
        return shop is not null && shop.MerchantIdentifier == selection.MerchantIdentifier
            && IntegrationConnectionScope.CanonicalTenant(selection.ShopId, shop.TenantId) == selection.TenantId
            ? selection : null;
    }

    public async Task EndAsync(string adminId, Guid simulationId, CancellationToken ct)
    {
        var selection = await integrations.IntegrationAdminSimulations.SingleOrDefaultAsync(x => x.Id == simulationId, ct);
        if (selection is null || !string.Equals(selection.AdminUserId, adminId, StringComparison.Ordinal)) return;
        selection.EndedAtUtc ??= DateTime.UtcNow;
        await integrations.SaveChangesAsync(ct);
    }

    public async Task<IntegrationTokenRequest?> RequestTokenAsync(string adminId, Guid simulationId, int displayedShopId,
        IntegrationProvider provider, IntegrationCredentialType credentialType, CancellationToken ct)
    {
        if (!Enum.IsDefined(provider) || !Enum.IsDefined(credentialType)) return null;
        var selection = await GetAsync(adminId, simulationId, ct);
        if (selection is null || !AdminSimulationBoundary.Allows(selection, adminId, DateTime.UtcNow, displayedShopId)) return null;
        var request = new IntegrationTokenRequest { SimulationId = selection.Id, Provider = provider, CredentialType = credentialType };
        // This records an on-behalf-of simulation only. No fabricated token or provider success is returned.
        var now = DateTime.UtcNow;
        var inserted = await integrations.Database.ExecuteSqlInterpolatedAsync($@"
            INSERT INTO dbo.IntegrationTokenRequests (Id, SimulationId, Provider, CredentialType, Status, RequestedAtUtc)
            SELECT {request.Id}, s.Id, {(byte)provider}, {(byte)credentialType}, {request.Status}, {request.RequestedAtUtc}
            FROM dbo.IntegrationAdminSimulations AS s
            WHERE s.Id = {simulationId} AND s.AdminUserId = {adminId} AND s.ShopId = {displayedShopId}
              AND s.EndedAtUtc IS NULL AND s.ExpiresAtUtc > {now}", ct);
        return inserted == 1 ? request : null;
    }
}
