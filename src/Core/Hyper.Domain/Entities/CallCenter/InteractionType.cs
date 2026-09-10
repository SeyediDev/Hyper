namespace Hyper.Domain.Entities.CallCenter;

/// <summary>
/// نوع تعامل - انواع مختلف تعاملات با مشتری (تماس تلفنی، چت، ایمیل، و غیره)
/// </summary>
[DisplayName("نوع تعامل")]
[SBVR(SBVRModality.Obligatory, "مدیریت تعاملات", "هر نوع تعامل باید برای دسته‌بندی و تحلیل تعاملات قابل شناسایی باشد")]
public class InteractionType : HyperBaseCoreConfigAuditableEntity<int>
{
    /// <summary>
    /// شناسه اکوسیستم
    /// </summary>
    [DisplayName("اکوسیستم")]
    public int TenantId { get; set; }

    [DisplayName("اکوسیستم")]
    public Tenant Tenant { get; set; } = null!;

    /// <summary>
    /// عنوان نوع تعامل
    /// </summary>
    [DisplayName("عنوان")]
    [InDisplayString]
    [MaxLength(100)]
    [SBVR(SBVRModality.Obligatory, "شناسایی نوع تعامل", "عنوان برای نمایش در رابط کاربری و گزارش‌ها ضروری است")]
    public string Title { get; set; } = null!;

    /// <summary>
    /// کلید یکتا برای نوع تعامل
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
    [SBVR(SBVRModality.Recommended, "مستندسازی", "توضیحات برای راهنمایی کاربران و مستندسازی استفاده می‌شود")]
    public string? Description { get; set; }

    /// <summary>
    /// آیکون یا نماد
    /// </summary>
    [DisplayName("آیکون")]
    [MaxLength(100)]
    [SBVR(SBVRModality.Permitted, "نمایش بصری", "آیکون برای بهبود تجربه کاربری استفاده می‌شود")]
    public string? Icon { get; set; }

    /// <summary>
    /// رنگ نمایش
    /// </summary>
    [DisplayName("رنگ")]
    [MaxLength(20)]
    [SBVR(SBVRModality.Permitted, "نمایش بصری", "رنگ برای تمایز بصری انواع تعاملات استفاده می‌شود")]
    public string? Color { get; set; }

    /// <summary>
    /// آیا نیاز به زمان‌بندی دارد
    /// </summary>
    [DisplayName("نیاز به زمان‌بندی")]
    [SBVR(SBVRModality.Recommended, "مدیریت زمان", "زمان‌بندی برای تعاملات پیچیده‌تر که نیاز به برنامه‌ریزی دارند استفاده می‌شود")]
    public bool RequiresScheduling { get; set; }

    /// <summary>
    /// آیا نیاز به نتیجه دارد
    /// </summary>
    [DisplayName("نیاز به نتیجه")]
    [SBVR(SBVRModality.Recommended, "پیگیری", "نتیجه برای تعاملات مهم که نیاز به پیگیری دارند ضروری است")]
    public bool RequiresOutcome { get; set; }

    /// <summary>
    /// آیا فعال است
    /// </summary>
    [DisplayName("فعال")]
    [SBVR(SBVRModality.Permitted, "مدیریت چرخه حیات", "وضعیت فعال برای کنترل استفاده از نوع تعامل استفاده می‌شود")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// ترتیب نمایش
    /// </summary>
    [DisplayName("ترتیب")]
    [SBVR(SBVRModality.Recommended, "تجربه کاربری", "ترتیب برای نمایش منطقی انواع تعاملات در رابط کاربری استفاده می‌شود")]
    public int? OrderId { get; set; }
}

