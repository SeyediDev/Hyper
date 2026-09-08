namespace Hyper.Domain.Entities.Channels;

[DisplayName("رویداد کانال دریافت رویداد")]
public class EventChannelValidEvent : HyperBaseCoreConfigAuditableEntity<int>, ISubOfEventType
{
    public int EventChannelId { get; set; }
    [DisplayName("کانال دریافت رویداد")]
    public EventChannel EventChannel { get; set; } = null!;

    public int EventTypeId { get; set; }
    [DisplayName("رویداد")]
    public EventType EventType { get; set; } = null!;
}