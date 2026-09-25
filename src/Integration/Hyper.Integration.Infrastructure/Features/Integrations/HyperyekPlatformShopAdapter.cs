using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;

namespace Hyper.Infrastructure.Features.Integrations;

/// <summary>Anti-corruption adapter for read-only shop metadata owned by Hyperyek.</summary>
public sealed class HyperyekPlatformShopAdapter(HyperSqlServerContext accounting) : IIntegrationPlatformOverviewPort
{
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
