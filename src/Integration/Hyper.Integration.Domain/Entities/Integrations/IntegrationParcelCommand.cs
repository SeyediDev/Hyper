namespace Hyper.Integration.Domain.Entities.Integrations;

public sealed class IntegrationParcelCommand
{
    public long Id { get; set; }
    public long ConnectionId { get; set; }
    public Guid RequestId { get; set; }
    public string ParcelId { get; set; } = null!;
    public byte Target { get; set; }
    public string AccountIdentifier { get; set; } = null!;
    public string RequestJson { get; set; } = null!;
    public string Actor { get; set; } = null!;
    public long? JobId { get; set; }
    // 0 pending, 1 send marker (GET-only recovery), 2 verified, 3 attention.
    public byte State { get; set; }
    public string? ErrorCode { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
