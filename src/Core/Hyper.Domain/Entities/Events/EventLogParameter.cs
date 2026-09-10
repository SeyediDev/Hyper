namespace Hyper.Domain.Entities.Events;


/// <summary>
/// می خواهیم مقادیر پارامتر را به ازای هر رویداد ذخیره کنیم 
/// </summary>
[DisplayName("مقدار پارامتر لاگ رویداد")]
public class EventLogParameter : HyperBaseCoreLogAuditableEntity<long>
{
    public long EventLogId { get; set; }
    [DisplayName("لاگ رویداد")]
    public EventLog EventLog { get; set; } = null!;
    
    public int ParameterId { get; set; }
    [DisplayName("پارامتر")]
    public EventTypeParameter Parameter { get; set; } = null!;

    [DisplayName("مقدار")]
    [MaxLength(512)]
    public string Value { get; set; } = null!;
}
