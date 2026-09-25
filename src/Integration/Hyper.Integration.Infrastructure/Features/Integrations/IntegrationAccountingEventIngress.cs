using Hyper.Integration.Contracts;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;

namespace Hyper.Infrastructure.Features.Integrations;

/// <summary>Provider-neutral ingress for Hyperyek changes. It only creates an Integration outbox message.</summary>
public sealed class IntegrationAccountingEventIngress(HyperIntegrationContext db, IIntegrationOutbox outbox)
    : IIntegrationAccountingEventIngress
{
    public async Task<AccountingInventoryChangedResponse?> ReceiveInventoryChangedAsync(
        AccountingInventoryChangedRequest request, CancellationToken ct = default)
    {
        if (request.ShopId <= 0 || string.IsNullOrWhiteSpace(request.TenantId) || request.ConnectionId <= 0
            || string.IsNullOrWhiteSpace(request.ExternalProductId) || request.SourceVersion <= 0
            || request.AvailableQuantity < 0) return null;
        var mapping = await (from m in db.ExternalProductMappings.AsNoTracking()
                             join c in db.ExternalIntegrationConnections.AsNoTracking() on m.ConnectionId equals c.Id
                             where m.ConnectionId == request.ConnectionId && m.ShopId == request.ShopId
                                && m.ExternalProductId == request.ExternalProductId
                                && m.ExternalVariantId == request.ExternalVariantId
                                && m.IsActive && c.TenantId == request.TenantId && c.IsEnabled
                             select m).SingleOrDefaultAsync(ct);
        if (mapping is null) return null;
        var id = await outbox.EnqueueInventoryAsync(mapping.ConnectionId, mapping.Id,
            request.SourceVersion, request.AvailableQuantity, ct);
        return new(id, "Queued");
    }
}
