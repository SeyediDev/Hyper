namespace Club.Domain.Entities.Events;

[DisplayName("رویداد کانال دریافت رویداد")]
[OldDbMap("EventChannelValidEventTypes")]
public class EventChannelValidEvent : ClubBaseCoreConfigAuditableEntity<int>
{
    public int EventChannelId { get; set; }
    [DisplayName("کانال دریافت رویداد")]
    public EventChannel EventChannel { get; set; } = null!;

    public int EventTypeId { get; set; }
    [DisplayName("رویداد")]
    public EventType EventType { get; set; } = null!;
}
