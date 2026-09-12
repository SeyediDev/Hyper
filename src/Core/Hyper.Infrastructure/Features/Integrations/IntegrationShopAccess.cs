using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed class IntegrationShopAccess(HyperSqlServerContext accounting, HyperIntegrationContext integrations) : IIntegrationShopAccess
{
    public async Task<IReadOnlyList<OwnedIntegrationShop>> GetOwnedShopsAsync(IntegrationMerchantIdentity identity, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var shopIds = await integrations.IntegrationMerchantAccess.AsNoTracking()
            .Where(x => x.Issuer == identity.Issuer && x.SubjectId == identity.SubjectId
                && x.IsEnabled && x.RevokedAtUtc == null && (x.ExpiresAtUtc == null || x.ExpiresAtUtc > now))
            .Select(x => x.ShopId).ToListAsync(cancellationToken);
        if (shopIds.Count == 0) return [];
        // Only business shop metadata is read; neither administrator User nor Neo identity tables are queried.
        var shops = await accounting.TblShops.AsNoTracking().Where(x => shopIds.Contains(x.Shopid))
            .Select(x => new { x.Shopid, x.TenantId }).ToListAsync(cancellationToken);
        return shops.Select(s => new OwnedIntegrationShop(s.Shopid,
            IntegrationConnectionScope.CanonicalTenant(s.Shopid, s.TenantId))).ToArray();
    }
}
