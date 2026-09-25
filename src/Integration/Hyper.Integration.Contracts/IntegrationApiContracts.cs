using System.Security.Claims;

namespace Hyper.Integration.Contracts;

public enum IntegrationProvider : byte
{
    Basalam = 1,
    Hyperyek = 2,
    Digikala = 3,
    Torob = 4,
    Custom = 255
}

public sealed record WebhookIngressRequest(
    IntegrationProvider Provider,
    string ConnectionKey,
    string EventId,
    string EventType,
    string? Timestamp,
    string? Signature,
    byte[] Body,
    string? CorrelationId = null,
    string? Authorization = null);

public enum WebhookIngressStatus : byte { Accepted = 1, Duplicate = 2, Invalid = 3, Unsupported = 4 }

public sealed record WebhookIngressResult(WebhookIngressStatus Status, long? InboxId = null,
    long? ScenarioJobId = null, string? ErrorCode = null);

public sealed record AccountingInventoryChangedRequest(int ShopId, string TenantId, long ConnectionId,
    string ExternalProductId, string? ExternalVariantId, long SourceVersion, decimal AvailableQuantity);
public sealed record AccountingInventoryChangedResponse(long OutboxMessageId, string Status);
public interface IIntegrationAccountingEventIngress
{
    Task<AccountingInventoryChangedResponse?> ReceiveInventoryChangedAsync(
        AccountingInventoryChangedRequest request, CancellationToken cancellationToken = default);
}

public interface IIntegrationWebhookIngress
{
    Task<WebhookIngressResult> ReceiveAsync(WebhookIngressRequest request, CancellationToken cancellationToken = default);
}

public sealed record IntegrationConnectionSummary(long Id, int ShopId, string TenantId,
    IntegrationProvider Provider, string DisplayName, bool IsEnabled, DateTime? ExpiresAtUtc, DateTime? LastSyncAtUtc);

public sealed record IntegrationReplayRequest(long InboxId);

public sealed record IntegrationSimulationRequest(int ShopId, string TenantId,
    IntegrationProvider Provider, string EventType, string PayloadJson, bool Execute = false);

public sealed record IntegrationConnectionCreateRequest(
    int ShopId,
    string TenantId,
    IntegrationProvider Provider,
    string DisplayName,
    string AccountIdentifier,
    byte CredentialType);

public sealed record IntegrationConnectionListRequest(int ShopId, string TenantId);
public sealed record IntegrationConnectionCommandRequest(int ShopId, string TenantId, long ConnectionId);
public sealed record IntegrationConnectionResponse(IntegrationConnectionSummary Connection);
public sealed record IntegrationReplayResponse(long InboxId, string Status);

public interface IIntegrationManagementApi
{
    Task<IReadOnlyList<IntegrationConnectionSummary>> ListConnectionsAsync(
        IntegrationConnectionListRequest request, CancellationToken cancellationToken = default);
    Task<IntegrationConnectionResponse?> CreateConnectionAsync(
        IntegrationConnectionCreateRequest request, CancellationToken cancellationToken = default);
    Task<bool> SetConnectionEnabledAsync(IntegrationConnectionCommandRequest request, bool enabled,
        CancellationToken cancellationToken = default);
    Task<IntegrationReplayResponse?> ReplayWebhookAsync(IntegrationConnectionCommandRequest request,
        long inboxId, CancellationToken cancellationToken = default);
}

public sealed record IntegrationProductMappingSummary(long Id, long ConnectionId, int ShopId,
    int HyperProductId, string ExternalProductId, string? ExternalSku, string? ExternalVariantId,
    decimal? LastExternalPrice, decimal? LastExternalInventory, DateTime? LastSyncAtUtc, bool IsActive);
public sealed record IntegrationProductMappingRequest(int ShopId, string TenantId, long ConnectionId,
    int HyperProductId, string ExternalProductId, string? ExternalSku = null, string? ExternalVariantId = null);
public interface IIntegrationMappingApi
{
    Task<IReadOnlyList<IntegrationProductMappingSummary>> ListProductMappingsAsync(
        IntegrationConnectionCommandRequest request, CancellationToken cancellationToken = default);
    Task<IntegrationProductMappingSummary?> CreateProductMappingAsync(
        IntegrationProductMappingRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeactivateProductMappingAsync(IntegrationConnectionCommandRequest request, long mappingId,
        CancellationToken cancellationToken = default);
}

public sealed record IntegrationSyncTriggerRequest(int ShopId, string TenantId, long ConnectionId);
public sealed record IntegrationSyncTriggerResponse(long RunId, string Status);
public interface IIntegrationSyncApi
{
    Task<IntegrationSyncTriggerResponse?> TriggerAsync(IntegrationSyncTriggerRequest request,
        CancellationToken cancellationToken = default);
}

public sealed record IntegrationDashboardStatusCount(byte Status, int Count);
public sealed record IntegrationDashboardRun(long Id, string ConnectionName, IntegrationProvider Provider,
    byte Status, DateTime StartedAtUtc, DateTime? FinishedAtUtc, int ItemsRead, int ItemsWritten,
    int ItemsFailed, string? Error);
public sealed record IntegrationDashboardResponse(int Connections, int EnabledConnections, int MappedProducts,
    IReadOnlyList<IntegrationDashboardStatusCount> Runs,
    IReadOnlyList<IntegrationDashboardStatusCount> Webhooks,
    IReadOnlyList<IntegrationDashboardRun> RecentRuns,
    IReadOnlyList<IntegrationDashboardStatusCount> Outbox)
{
    public IReadOnlyList<IntegrationDashboardConnectionHealth> ConnectionsHealth { get; init; } = [];
    public IReadOnlyList<IntegrationDashboardOutbox> RecentOutbox { get; init; } = [];
}
public sealed record IntegrationDashboardConnectionHealth(long ConnectionId, string ConnectionName,
    IntegrationProvider Provider, bool Enabled, bool TokenExpired, bool HasError,
    DateTime? LastSyncAtUtc, string Status);
public sealed record IntegrationDashboardOutbox(long Id, string ConnectionName, byte Status, int Attempts,
    DateTime CreatedAtUtc, DateTime NextAttemptAtUtc, string? LastError);
public interface IIntegrationDashboardApi
{
    Task<IntegrationDashboardResponse> GetDashboardAsync(int shopId, string tenantId,
        CancellationToken cancellationToken = default);
}

/// <summary>Authorizes the requested shop/tenant scope from trusted identity claims.</summary>
public interface IIntegrationScopeAuthorization
{
    Task<bool> CanAccessAsync(ClaimsPrincipal principal, int shopId, string tenantId,
        CancellationToken cancellationToken = default);
}

public sealed record IntegrationTokenRequestCommand(Guid SimulationId, int ShopId, string TenantId,
    IntegrationProvider Provider, byte CredentialType);
public sealed record IntegrationTokenRequestStatus(Guid RequestId, byte Status, DateTime RequestedAtUtc);
public sealed record IntegrationTokenStatus(long ConnectionId, IntegrationProvider Provider,
    bool IsActive, bool ConnectionEnabled, DateTime? ExpiresAtUtc);
public interface IIntegrationTokenApi
{
    Task<IntegrationTokenRequestStatus?> RequestAsync(string adminId, IntegrationTokenRequestCommand command,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<IntegrationTokenStatus>> ListAsync(int shopId, string tenantId,
        CancellationToken cancellationToken = default);
    Task<bool> RevokeAsync(int shopId, string tenantId, long connectionId,
        CancellationToken cancellationToken = default);
}
