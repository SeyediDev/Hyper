using Hyper.Integration.Domain.Features.Integrations;
namespace Hyper.Integration.Domain.Entities.Integrations;

/// <summary>A provider-neutral durable request. Event payloads and credentials are deliberately not stored here.</summary>
public sealed class IntegrationScenarioJob
{
    public long Id { get; set; }
    public long ConnectionId { get; set; }
    public int ShopId { get; set; }
    public string TenantId { get; set; } = null!;
    public string EventId { get; set; } = null!;
    public IntegrationSyncItem Item { get; set; }
    public IntegrationSyncTrigger Trigger { get; set; }
    public IntegrationScenarioStatus Status { get; set; }
    public int Attempts { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime NextAttemptAtUtc { get; set; }
    public Guid? LeaseId { get; set; }
    public DateTime? LeaseExpiresAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public string? ErrorCode { get; set; }
    public string? ResultJson { get; set; }
}

