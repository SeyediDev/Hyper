using Hyper.Integration.Contracts;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed class IntegrationAccountingProductEventIngress(HyperIntegrationContext db,
    IIntegrationProductOutbox outbox) : IIntegrationAccountingProductEventIngress
{
    public async Task<AccountingProductChangedResponse> ReceiveAsync(AccountingProductChangedRequest request, CancellationToken ct = default)
    {
        var update = new ExternalProductUpdate(request.ExternalProductId, request.ExternalVariantId, request.Title, request.PrimaryPrice);
        var error = update.ValidationError();
        if (request.ShopId <= 0 || request.ConnectionId <= 0 || request.SourceVersion <= 0
            || string.IsNullOrWhiteSpace(request.TenantId) || request.TenantId.Length > 30)
            error = "InvalidScopeOrVersion";
        if (error is not null) return new(null, "Rejected", error);
        var mapping = await (from m in db.ExternalProductMappings.AsNoTracking()
                             join c in db.ExternalIntegrationConnections.AsNoTracking() on m.ConnectionId equals c.Id
                             where m.ConnectionId == request.ConnectionId && m.ShopId == request.ShopId
                                && c.ShopId == request.ShopId && c.TenantId == request.TenantId && c.IsEnabled
                                && m.IsActive && m.ExternalProductId == request.ExternalProductId
                                && m.ExternalVariantId == request.ExternalVariantId
                             select m).SingleOrDefaultAsync(ct);
        if (mapping is null) return new(null, "Rejected", "MappingOrScopeNotFound");
        try
        {
            var id = await outbox.EnqueueProductAsync(request.ConnectionId, mapping.Id, request.SourceVersion, update, ct);
            return new(id, "Queued");
        }
        catch (InvalidOperationException ex) when (ex.Message is "SourceVersionConflict" or "StaleSourceVersion" or "VersionSourceConflict" or "VersionSourceUnassigned")
        { return new(null, "Conflict", ex.Message); }
        catch (IntegrationProviderException ex) when (!ex.Retryable)
        { return new(null, "Rejected", ex.Code); }
    }
}
