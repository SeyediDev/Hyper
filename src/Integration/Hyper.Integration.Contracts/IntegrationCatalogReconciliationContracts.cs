namespace Hyper.Integration.Contracts;

public sealed record IntegrationCatalogReconciliationRequest(int ShopId, string TenantId, long ConnectionId, Guid RequestId);
public sealed record IntegrationCatalogJobStatus(long JobId, string Status, string? ErrorCode, DateTime? CompletedAtUtc);
public sealed record IntegrationCatalogReconciliationResponse(Guid RequestId,
    IntegrationCatalogJobStatus Product, IntegrationCatalogJobStatus Inventory);
