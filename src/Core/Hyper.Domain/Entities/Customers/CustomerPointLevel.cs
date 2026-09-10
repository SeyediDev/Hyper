namespace Hyper.Domain.Entities.Customers;

/// <summary>
/// می خواهیم برای هر مشتری سطح‌امتیازاش را در اکوسیستم ذخیره کنیم
/// </summary>
[DisplayName("سطح‌امتیاز مشتری")]
public class CustomerPointLevel : HyperBaseCoreConfigAuditableEntity<int>
{
    public int CustomerTenantId { get; set; }
    [DisplayName("مشتری")]
    public CustomerTenant CustomerTenant { get; set; } = null!;

    public int PointLevelId { get; set; }
    [DisplayName("سطح‌امتیاز")]
    public PointLevel PointLevel { get; set; } = null!;

    public long EventLogId { get; set; }
    [DisplayName("لاگ رویداد")]
    public EventLog EventLog { get; set; } = null!;
}
