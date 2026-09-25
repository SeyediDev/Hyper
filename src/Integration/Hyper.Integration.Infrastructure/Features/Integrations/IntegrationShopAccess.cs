using Microsoft.EntityFrameworkCore;

using Hyper.Infrastructure.Data.Repository.Hyper;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed class IntegrationShopAccess(IIntegrationPlatformShopPort platformShops, HyperIntegrationContext integrations) : IIntegrationShopAccess
{
    public async Task<IReadOnlyList<OwnedIntegrationShop>> GetOwnedShopsAsync(IntegrationMerchantIdentity identity, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var shopIds = await integrations.IntegrationMerchantAccess.AsNoTracking()
            .Where(x => x.Issuer == identity.Issuer && x.SubjectId == identity.SubjectId
                && x.IsEnabled && x.RevokedAtUtc == null && (x.ExpiresAtUtc == null || x.ExpiresAtUtc > now))
            .Select(x => x.ShopId).ToListAsync(cancellationToken);
        if (shopIds.Count == 0) return [];
        var shops = await platformShops.GetByIdsAsync(shopIds, cancellationToken);
        return shops.Select(s => new OwnedIntegrationShop(s.ShopId,
            IntegrationConnectionScope.CanonicalTenant(s.ShopId, s.TenantId)))
            .ToArray();
    }
}
