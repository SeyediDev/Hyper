namespace Hyper.Domain.Entities.Customers;

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
    [DisplayName("کلید API اکوسیستم")]
    [MaxLength(41)]
    [SBVR(SBVRModality.Obligatory, "امنیت API", "کلید API باید برای احراز هویت و دسترسی امن به خدمات سیستم منحصر به فرد باشد")]
    public string ApiKey { get; set; } = null!;

    /// <summary>
    /// آیا امکان ثبت خرید دستی با سریال در کلاب فعال است؟
    /// برای محصولاتی که در کانال‌ها ارائه نمی‌شوند
    /// </summary>
    [DisplayName("فعال‌سازی ثبت خرید دستی")]
    [SBVR(SBVRModality.Permitted, "مدیریت قابلیت‌ها", "برای فعال/غیرفعال کردن امکان ثبت خرید دستی با سریال در کلاب")]
    public bool AllowSerialEntry { get; set; } = false;
}