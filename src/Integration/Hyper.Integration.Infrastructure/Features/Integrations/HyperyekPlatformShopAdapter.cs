using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;

namespace Hyper.Infrastructure.Features.Integrations;

/// <summary>Anti-corruption adapter for read-only shop metadata owned by Hyperyek.</summary>
public sealed class HyperyekPlatformShopAdapter(HyperSqlServerContext accounting) : IIntegrationPlatformShopPort
{
    public async Task<IReadOnlyList<IntegrationPlatformShop>> SearchAsync(string? search, CancellationToken ct)
    {
        var shops = accounting.TblShops.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();
            shops = shops.Where(x => x.Shopid.ToString().Contains(search)
                || x.Name.Contains(search) || x.Ownerid.Contains(search) || x.Mobile!.Contains(search));
        }
        return await shops.OrderBy(x => x.Name).ThenBy(x => x.Shopid).Take(100)
            .Select(x => new IntegrationPlatformShop(x.Shopid, x.Name, x.Ownerid,
                IntegrationConnectionScope.CanonicalTenant(x.Shopid, x.TenantId))).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<IntegrationPlatformShop>> GetByIdsAsync(IReadOnlyCollection<int> shopIds,
        CancellationToken ct)
    {
        if (shopIds.Count == 0) return [];
        return await accounting.TblShops.AsNoTracking().Where(x => shopIds.Contains(x.Shopid))
            .Select(x => new IntegrationPlatformShop(x.Shopid, x.Name, x.Ownerid,
                IntegrationConnectionScope.CanonicalTenant(x.Shopid, x.TenantId))).ToListAsync(ct);
    }

    public async Task<IntegrationPlatformShop?> GetAsync(int shopId, CancellationToken ct) =>
        await accounting.TblShops.AsNoTracking().Where(x => x.Shopid == shopId)
            .Select(x => new IntegrationPlatformShop(x.Shopid, x.Name, x.Ownerid,
                IntegrationConnectionScope.CanonicalTenant(x.Shopid, x.TenantId)))
            .SingleOrDefaultAsync(ct);
}
