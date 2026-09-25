using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;

namespace Hyper.Infrastructure.Features.Integrations;

/// <summary>Anti-corruption adapter for read-only shop metadata owned by Hyperyek.</summary>
public sealed class HyperyekPlatformShopAdapter(HyperSqlServerContext accounting) : IIntegrationPlatformShopPort,
    IIntegrationPlatformCatalogPort, IIntegrationPlatformOverviewPort
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

    public async Task<IReadOnlyList<IntegrationPlatformProduct>> GetProductsAsync(int shopId, string tenantId,
        CancellationToken ct)
    {
        var tenantless = tenantId == IntegrationConnectionScope.CanonicalTenant(shopId, null);
        return await accounting.TblProducts.AsNoTracking()
            .Where(x => x.Shopid == shopId && (x.TenantId == tenantId ||
                tenantless && (x.TenantId == null || x.TenantId == "")))
            .OrderBy(x => x.Id)
            .Select(x => new IntegrationPlatformProduct(x.Id, x.Name, x.Taxcode, x.Saleprice,
                x.Accountingstock, x.Isenabled, x.Isstockable, x.Minimumstock))
            .ToListAsync(ct);
    }

    public async Task<PlatformOverviewData> GetAsync(int days, int? shopId, string? tenantId,
        CancellationToken ct)
    {
        if (days is not (7 or 30)) throw new ArgumentOutOfRangeException(nameof(days));
        if (shopId.HasValue && (shopId <= 0 || string.IsNullOrWhiteSpace(tenantId)))
            throw new ArgumentException("Shop requires tenant scope.");
        var now = DateTime.UtcNow;
        var periodStart = now.Date.AddDays(1 - days);
        var until = now.Date.AddDays(1);
        var shops = accounting.TblShops.AsNoTracking();
        var products = accounting.TblProducts.AsNoTracking();
        var people = accounting.TblPersons.AsNoTracking();
        var invoices = accounting.TblSaleorders.AsNoTracking();
        if (shopId.HasValue)
        {
            var id = shopId.Value;
            var tenantless = tenantId == IntegrationConnectionScope.CanonicalTenant(id, null);
            shops = shops.Where(x => x.Shopid == id && (x.TenantId == tenantId || tenantless && (x.TenantId == null || x.TenantId == "")));
            products = products.Where(x => x.Shopid == id && (x.TenantId == tenantId || tenantless && (x.TenantId == null || x.TenantId == "")));
            people = people.Where(x => x.Shopid == id && (x.TenantId == tenantId || tenantless && (x.TenantId == null || x.TenantId == "")));
            invoices = invoices.Where(x => x.Shopid == id && (x.TenantId == tenantId || tenantless && (x.TenantId == null || x.TenantId == "")));
        }
        var periodInvoices = invoices.Where(x => x.Issuedatetime >= periodStart && x.Issuedatetime < until);
        var trend = await periodInvoices.GroupBy(x => x.Issuedatetime.Date)
            .Select(g => new PlatformOverviewDay(g.Key, g.Count())).ToListAsync(ct);
        return new(await shops.CountAsync(ct), await products.CountAsync(ct),
            await products.CountAsync(x => x.Isenabled, ct), await people.CountAsync(ct),
            await periodInvoices.CountAsync(ct),
            await products.CountAsync(x => x.Isenabled && x.Isstockable && x.Minimumstock != null
                && x.Accountingstock < x.Minimumstock, ct), trend);
    }
}
