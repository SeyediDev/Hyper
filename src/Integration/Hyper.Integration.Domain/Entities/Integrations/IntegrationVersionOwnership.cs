namespace Hyper.Integration.Domain.Entities.Integrations;

// One durable version allocator per mapping, across inventory and product operations.
public sealed class IntegrationVersionOwnership
{
    public long MappingId { get; set; }
    public byte Source { get; set; } // 1 accounting events, 2 inventory capture
    public long VersionFloor { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
