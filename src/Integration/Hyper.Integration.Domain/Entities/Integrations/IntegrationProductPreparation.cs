namespace Hyper.Integration.Domain.Entities.Integrations;

public sealed class IntegrationProductPreparation
{
    public long Id { get; set; }
    public long ConnectionId { get; set; }
    public byte Direction { get; set; }
    public string SourceProductId { get; set; } = null!;
    public int Revision { get; set; }
    public string InputJson { get; set; } = null!;
    public string PreparedJson { get; set; } = null!;
    public string IssuesJson { get; set; } = "[]";
    public string ChangesJson { get; set; } = "[]";
    public bool WasAdapted { get; set; }
    public Guid? DraftRequestId { get; set; }
    public long? JobId { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
public sealed class IntegrationProductPreparationHistory
{
    public long Id { get; set; }
    public long PreparationId { get; set; }
    public int Revision { get; set; }
    public string AuditJson { get; set; } = null!;
}
public sealed class IntegrationProductPreparationPolicy
{
    public long Id { get; set; }
    public long ConnectionId { get; set; }
    public byte Direction { get; set; }
    public string PolicyJson { get; set; } = null!;
    public string UpdatedBy { get; set; } = null!;
    public DateTime UpdatedAtUtc { get; set; }
}
