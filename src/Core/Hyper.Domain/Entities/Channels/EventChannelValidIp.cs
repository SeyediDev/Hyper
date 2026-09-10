namespace Hyper.Domain.Entities.Channels;

[DisplayName("آدرس معتبر کانال دریافت رویداد")]
public class EventChannelValidIp : HyperBaseCoreConfigAuditableEntity<int>
{
    public int EventChannelId { get; set; }
    [DisplayName("کانال رویداد")]
    public EventChannel EventChannel { get; set; } = null!;

    [DisplayName("آدرس Ip معتبر")]
    [MaxLength(61)]
    public string? ValidIp { get; set; }
}
