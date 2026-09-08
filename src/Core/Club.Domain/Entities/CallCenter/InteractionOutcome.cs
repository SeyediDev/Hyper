namespace Hyper.Domain.Entities.CallCenter;

/// <summary>
/// نتیجه تعامل - نتایج مختلف تعاملات با مشتری (حل شده، نیاز به پیگیری، و غیره)
/// </summary>
[DisplayName("نتیجه تعامل")]
[SBVR(SBVRModality.Obligatory, "مدیریت تعاملات", "هر نتیجه تعامل باید برای تحلیل اثربخشی و پیگیری استفاده شود")]
public class InteractionOutcome : HyperBaseCoreConfigAuditableEntity<int>
{
    /// <summary>
    /// شناسه اکوسیستم
    /// </summary>
    [DisplayName("اکوسیستم")]
    public int TenantId { get; set; }

    [DisplayName("اکوسیستم")]
    public Tenant Tenant { get; set; } = null!;

    /// <summary>
    /// عنوان نتیجه
    /// </summary>
    [DisplayName("عنوان")]
    [InDisplayString]
    [MaxLength(100)]
    [SBVR(SBVRModality.Obligatory, "شناسایی نتیجه", "عنوان برای نمایش در رابط کاربری و گزارش‌ها ضروری است")]
    public string Title { get; set; } = null!;

    /// <summary>
    /// کلید یکتا برای نتیجه
    /// </summary>
    [DisplayName("کلید")]
    [MaxLength(50)]
    [SBVR(SBVRModality.Obligatory, "شناسایی یکتا", "کلید برای ارجاع برنامه‌نویسی و یکپارچه‌سازی ضروری است")]
    public string Key { get; set; } = null!;

    /// <summary>
    /// توضیحات
    /// </summary>
    [DisplayName("توضیحات")]
    [MaxLength(500)]
    [SBVR(SBVRModality.Recommended, "مستندسازی", "توضیحات برای راهنمایی کاربران استفاده می‌شود")]
    public string? Description { get; set; }

    /// <summary>
    /// رنگ نمایش
    /// </summary>
    [DisplayName("رنگ")]
    [MaxLength(20)]
    [SBVR(SBVRModality.Permitted, "نمایش بصری", "رنگ برای تمایز بصری نتایج استفاده می‌شود")]
    public string? Color { get; set; }

    /// <summary>
    /// آیا نیاز به پیگیری دارد
    /// </summary>
    [DisplayName("نیاز به پیگیری")]
    [SBVR(SBVRModality.Recommended, "مدیریت پیگیری", "پیگیری برای نتایجی که نیاز به اقدام بعدی دارند استفاده می‌شود")]
    public bool RequiresFollowUp { get; set; }

    /// <summary>
    /// آیا فعال است
    /// </summary>
    [DisplayName("فعال")]
    [SBVR(SBVRModality.Permitted, "مدیریت چرخه حیات", "وضعیت فعال برای کنترل استفاده از نتیجه استفاده می‌شود")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// ترتیب نمایش
    /// </summary>
    [DisplayName("ترتیب")]
    [SBVR(SBVRModality.Recommended, "تجربه کاربری", "ترتیب برای نمایش منطقی نتایج در رابط کاربری استفاده می‌شود")]
    public int? OrderId { get; set; }
}