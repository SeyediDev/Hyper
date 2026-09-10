namespace Hyper.Domain.Entities.Events.Data;


/// <summary>
/// می خواهیم مقادیر ویژگی های دریافتی را به ازای هر رویداد ذخیره کنیم 
/// </summary>
[DisplayName("مقدار ویژگی های لاگ رویداد")]
public class EventLogAttribute : HyperBaseCoreLogAuditableEntity<long>
{
    public long EventLogId { get; set; }
    [DisplayName("لاگ رویداد")]
    public EventLog EventLog { get; set; } = null!;
    
    public int AttributeId { get; set; }
    [DisplayName("ویژگی")]
    public TenantAttribute Attribute { get; set; } = null!;

    [DisplayName("مقدار")]
    [MaxLength(512)]
    public string Value { get; set; } = null!;
}