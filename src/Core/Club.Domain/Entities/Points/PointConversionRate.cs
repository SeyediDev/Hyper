namespace Hyper.Domain.Entities.Points;

/// <summary>
/// نرخ تبدیل امتیاز - مانند نرخ تبدیل ارزها
/// </summary>
[DisplayName("نرخ تبدیل امتیاز")]
[SBVR(SBVRModality.Obligatory, "سیستم تبدیل امتیاز", "نرخ تبدیل برای تعیین چگونگی تبدیل یک نوع امتیاز به نوع دیگر ضروری است")]
[SBVR(SBVRModality.Recommended, "سیستم تبدیل امتیاز", "نرخ تبدیل برای ایجاد انعطاف در استفاده از امتیازات و افزایش رضایت مشتری استفاده می‌شود")]
public class PointConversionRate : HyperBaseCoreAuditableEntity<int>
{
    /// <summary>
    /// امتیاز مبدا (از)
    /// </summary>
    [DisplayName("امتیاز مبدا")]
    [SBVR(SBVRModality.Obligatory, "تبدیل امتیاز", "امتیاز مبدا تعیین می‌کند که چه نوع امتیازی قابل تبدیل است")]
    public int FromPointId { get; set; }
    
    [DisplayName("امتیاز مبدا")]
    public Point FromPoint { get; set; } = null!;

    /// <summary>
    /// امتیاز مقصد (به)
    /// </summary>
    [DisplayName("امتیاز مقصد")]
    [SBVR(SBVRModality.Obligatory, "تبدیل امتیاز", "امتیاز مقصد تعیین می‌کند که به چه نوع امتیازی تبدیل می‌شود")]
    public int ToPointId { get; set; }
    
    [DisplayName("امتیاز مقصد")]
    public Point ToPoint { get; set; } = null!;

    /// <summary>
    /// نرخ تبدیل (هر واحد امتیاز مبدا = چند واحد امتیاز مقصد)
    /// </summary>
    [DisplayName("نرخ تبدیل")]
    [SBVR(SBVRModality.Obligatory, "محاسبه تبدیل", "نرخ تبدیل برای محاسبه مقدار امتیاز دریافتی در مقصد ضروری است")]
    [SBVR(SBVRModality.Calculated, "محاسبه تبدیل", "مقدار مقصد = مقدار مبدا × نرخ تبدیل")]
    public decimal ConversionRate { get; set; }

    /// <summary>
    /// امتیاز کارمزد (اختیاری)
    /// </summary>
    [DisplayName("امتیاز کارمزد")]
    [SBVR(SBVRModality.Permitted, "کارمزد تبدیل", "امتیاز کارمزد برای دریافت هزینه تبدیل به صورت امتیاز استفاده می‌شود")]
    public int? CommissionPointId { get; set; }
    
    [DisplayName("امتیاز کارمزد")]
    public Point? CommissionPoint { get; set; }

    /// <summary>
    /// مقدار کارمزد (اختیاری)
    /// </summary>
    [DisplayName("مقدار کارمزد")]
    [SBVR(SBVRModality.Permitted, "کارمزد تبدیل", "مقدار کارمزد برای تعیین هزینه تبدیل امتیاز استفاده می‌شود")]
    [SBVR(SBVRModality.Recommended, "کارمزد تبدیل", "کارمزد برای مدیریت هزینه‌های عملیاتی و جلوگیری از سوءاستفاده استفاده می‌شود")]
    public long? CommissionAmount { get; set; }

    /// <summary>
    /// فعال/غیرفعال
    /// </summary>
    [DisplayName("فعال")]
    [SBVR(SBVRModality.Obligatory, "مدیریت نرخ", "وضعیت فعال بودن برای کنترل دسترسی به نرخ تبدیل ضروری است")]
    public bool IsActive { get; set; } = true;
}
