namespace Hyper.Domain.Entities.Customers;

[DisplayName("عضویت جامعه/بازار مشتریان")]
public class CustomerSegmentMembership : HyperBaseCoreAuditableEntity<int>
{
    public int CustomerTenantId { get; set; }
    [DisplayName("مشتری")]
    public CustomerTenant CustomerTenant { get; set; } = null!;

    public int SegmentId { get; set; }
    [DisplayName("جامعه/بازار")]
    public CustomerSegment Segment { get; set; } = null!;

    public long EventLogId { get; set; }
    [DisplayName("لاگ رویداد")]
    public EventLog EventLog { get; set; } = null!;
}
