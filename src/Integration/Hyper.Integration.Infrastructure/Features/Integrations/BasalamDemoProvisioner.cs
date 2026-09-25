using Hyper.Integration.Domain.Entities.Integrations;
using Hyper.Integration.Domain.Features.Integrations;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;

namespace Hyper.Infrastructure.Features.Integrations;

/// <summary>Creates deterministic product mappings only for enabled demo connections.</summary>
public sealed class BasalamDemoProvisioner(
    HyperIntegrationContext integrations,
    HyperSqlServerContext accounting,
    IIntegrationStrategyResolver strategies)
{
    public async Task<int> EnsureProductMappingsAsync(long connectionId, int shopId, string tenantId,
        CancellationToken ct)
    {
        var connection = await integrations.ExternalIntegrationConnections.SingleAsync(x =>
            x.Id == connectionId && x.ShopId == shopId && x.TenantId == tenantId, ct);
        if (connection.Provider != IntegrationProvider.Basalam || !connection.IsEnabled) return 0;

        var remote = await strategies.Resolve(connection.Provider, connection.CredentialType)
            .ReadCatalogAsync(connection, ct);
        var local = await accounting.TblProducts.AsNoTracking()
            .Where(x => x.Shopid == shopId &&
                (x.TenantId == tenantId || (x.TenantId == null && tenantId == "shop:" + shopId)))
            .Select(x => new LocalProduct(x.Id, x.Name, x.Taxcode)).ToListAsync(ct);
        var existing = await integrations.ExternalProductMappings
            .Where(x => x.ConnectionId == connectionId && x.ShopId == shopId).ToListAsync(ct);
        var used = existing.Select(x => x.HyperProductId).ToHashSet();
        var added = 0;
        foreach (var item in remote)
        {
            if (existing.Any(x => x.ExternalProductId == item.ExternalProductId && x.ExternalVariantId == item.VariantId)) continue;
            var candidate = local.FirstOrDefault(x => !used.Contains(x.Id) && !string.IsNullOrWhiteSpace(item.Sku)
                && string.Equals(x.Taxcode?.Trim(), item.Sku.Trim(), StringComparison.OrdinalIgnoreCase));
            candidate ??= local.FirstOrDefault(x => !used.Contains(x.Id)
                && string.Equals(x.Name.Trim(), item.Title.Trim(), StringComparison.OrdinalIgnoreCase));
            if (candidate is null) continue;
            integrations.ExternalProductMappings.Add(new ExternalProductMapping
            {
                ConnectionId = connectionId, ShopId = shopId, HyperProductId = candidate.Id,
                ExternalProductId = item.ExternalProductId, ExternalSku = item.Sku,
                ExternalVariantId = item.VariantId, LastExternalPrice = item.Price,
                LastExternalInventory = item.Inventory, IsActive = true
            });
            used.Add(candidate.Id); added++;
        }
        if (added != 0) await integrations.SaveChangesAsync(ct);
        return added;
    }

    private sealed record LocalProduct(int Id, string Name, string? Taxcode);
}

