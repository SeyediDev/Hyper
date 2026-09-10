namespace Hyper.Domain.Entities.Tenants;

[DisplayName("اکوسیستم")]
[SBVR(SBVRModality.Obligatory, "چندین اکوسیستم", "هر اکوسیستم باید برای جداسازی داده‌ها، مدیریت دسترسی و تحلیل عملکرد مستقل قابل شناسایی باشد")]
[SBVR(SBVRModality.Recommended, "چندین اکوسیستم", "اکوسیستم‌ها باید برای مدیریت چندین مشتری، محصول و کمپین‌های بازاریابی طراحی شوند")]
public class Tenant : HyperBaseCoreConfigAuditableEntity<int>
{
    /// <summary>
    /// عنوان اکوسیستم
    /// </summary>
    [DisplayName("عنوان اکوسیستم")]
    [InDisplayString]
    [MaxLength(41)]
    [SBVR(SBVRModality.Obligatory, "شناسایی اکوسیستم", "عنوان اکوسیستم باید برای نمایش در رابط کاربری و گزارش‌ها واضح و قابل فهم باشد")]
    public string Title { get; set; } = null!;

    /// <summary>
    /// کلید API اکوسیستم
    /// </summary>
    [DisplayName("کلید")]
    [MaxLength(41)]
    public string Key { get; set; } = null!;

    public ICollection<TenantAttribute> TenantAttributes { get; set; } = [];
}
