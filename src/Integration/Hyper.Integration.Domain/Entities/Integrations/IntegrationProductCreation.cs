namespace Hyper.Integration.Domain.Entities.Integrations;

public sealed class IntegrationProductCreation
{
    public long Id { get; set; }
    public long ConnectionId { get; set; }
    public Guid RequestId { get; set; }
    public int HyperProductId { get; set; }
    public string AccountIdentifier { get; set; } = null!;
    public long? JobId { get; set; }
    public string RequestJson { get; set; } = null!;
    public string PayloadJson { get; set; } = null!;
    // 0 pending; 1 send started (ambiguous after crash); 2 identity received;
    // 3 verified and mapped; 4 needs attention. No automatic second POST.
    public byte State { get; set; }
    public string? ExternalProductId { get; set; }
    public long? MappingId { get; set; }
    public string? ErrorCode { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
