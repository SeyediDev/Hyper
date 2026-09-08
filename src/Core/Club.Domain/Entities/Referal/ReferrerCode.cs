namespace Hyper.Domain.Entities.Referal;

/// <summary>
/// کد معرف منحصر به فرد برای هر مشتری
/// این موجودیت برای تولید و مدیریت کدهای معرف استفاده می‌شود
/// </summary>
[DisplayName("کد معرف")]
public class ReferrerCode : HyperBaseCoreAuditableEntity<int>
{
    [DisplayName("شناسه مشتری")]
    public int CustomerTenantId { get; set; }

    public CustomerTenant CustomerTenant { get; set; } = null!;


    [InDisplayString]
    [MaxLength(20)]
    [Required]
    public string Code { get; set; } = null!;

    // روابط
    public ICollection<CustomerReferrer> CustomerReferrers { get; set; } = [];
}
