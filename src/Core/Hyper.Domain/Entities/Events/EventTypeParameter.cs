namespace Hyper.Domain.Entities.Events;

/// <summary>
/// می خواهیم یه سری پارامتر را به ازای هر رویداد تعریف کنیم که هنگام دریافت رویداد دریافت کنیم 
/// </summary>
[DisplayName("پارامتر دریافتی حین رویداد")]
public class EventTypeParameter : HyperBaseCoreConfigAuditableEntity<int>
{
    public int EventTypeId { get; set; }
    [DisplayName("رویداد")]
    public EventType EventType { get; set; } = null!;

    [DisplayName("کلید")]
    [InDisplayString]
    [MaxLength(40)]
    public string Key { get; set; } = null!;

    [DisplayName("عنوان")]
    [InDisplayString]
    [MaxLength(41)]
    public string Title { get; set; } = null!;

    [DisplayName("نوع پارامتر")]
    public ParameterType ParameterType { get; set; }

    [DisplayName("اختیاری است ؟")]
    public bool? IsOptional { get; set; }

    [DisplayName("ایجاد شده توسط سیستم")]
    public bool? CreatedBySystem { get; set; }

    //TODO این مپینگ می تونه many to many بشه فعلا برای سهولت یدونه تعریف می کنیم
    public int? CustomerParameterId { get; set; }
    [DisplayName("معادل پارامتر مشتری")]
    public CustomerParameter? CustomerParameter { get; set; }
}
