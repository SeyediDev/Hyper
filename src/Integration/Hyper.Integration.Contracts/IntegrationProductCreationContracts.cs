namespace Hyper.Integration.Contracts;

// Deliberately no caller-supplied title, price, stock or vendor: these come from
// the scoped source and connection. This operation only creates an unpublished draft.
public sealed record IntegrationProductDraftRequest(int ShopId, string TenantId, long ConnectionId,
    Guid RequestId, int HyperProductId, int CategoryId, int? PreparationDays, int PackageWeight,
    string? Description = null, long? PhotoId = null);
public sealed record IntegrationProductDraftStatus(Guid RequestId, long? JobId, string Status,
    string? ErrorCode, string? ExternalProductId, long? MappingId);
public interface IIntegrationProductDraftApi
{
    Task<IntegrationProductDraftStatus?> StartAsync(IntegrationProductDraftRequest request, CancellationToken ct);
    Task<IntegrationProductDraftStatus?> ReadAsync(IntegrationConnectionCommandRequest scope, Guid requestId, CancellationToken ct);
}
