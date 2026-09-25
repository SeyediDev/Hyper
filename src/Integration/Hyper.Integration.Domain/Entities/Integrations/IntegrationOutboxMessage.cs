namespace Hyper.Integration.Domain.Entities.Integrations;

public sealed class IntegrationOutboxMessage
{
    public long Id { get; set; }
    public long ConnectionId { get; set; }
    public long MappingId { get; set; }
    public long SourceVersion { get; set; }
    public string Operation { get; set; } = null!;
    public string PayloadJson { get; set; } = null!;
    // 0 pending/retry; 1 leased; 2 delivered; 3 dead letter.
    public byte Status { get; set; }
    public int Attempts { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime NextAttemptAtUtc { get; set; }
    public Guid? LeaseId { get; set; }
    public DateTime? LeaseExpiresAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public string? LastError { get; set; }
}

