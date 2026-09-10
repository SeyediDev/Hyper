using Hyper.Domain.Entities.Metrics.Enums;

namespace Hyper.Domain.Entities.Metrics;

/// <summary>
/// تعریف شاخص - تعریف پویای شاخص‌های وفاداری و اعتباری
/// این موجودیت امکان تعریف شاخص جدید بدون تغییر کد را فراهم می‌کند
/// </summary>
[DisplayName("تعریف شاخص")]
[SBVR(SBVRModality.Obligatory, "تعریف شاخص", "هر شاخص باید برای محاسبه، ذخیره و نمایش قابل شناسایی باشد")]
[SBVR(SBVRModality.Recommended, "تعریف شاخص", "تعریف شاخص باید شامل فرمول محاسبه، وابستگی‌ها و تنظیمات نمایش باشد")]
public class MetricDefinition : HyperBaseCoreAuditableEntity<int>, ISubOfTenant
{
    public int TenantId { get; set; }
    [DisplayName("اکوسیستم")]
    public Tenant Tenant { get; set; } = null!;

    /// <summary>
    /// کد یکتا شاخص - شناسه یکتا برای ارجاع
    /// </summary>
    [MaxLength(100)]
    [DisplayName("کد شاخص")]
    [SBVR(SBVRModality.Obligatory, "شناسه یکتا", "کد شاخص باید یکتا باشد تا از تداخل جلوگیری شود")]
    public string Code { get; set; } = null!;

    /// <summary>
    /// نام شاخص
    /// </summary>
    [MaxLength(200)]
    [DisplayName("نام شاخص")]
    [InDisplayString]
    [SBVR(SBVRModality.Obligatory, "نام شاخص", "نام شاخص برای نمایش در UI و گزارش‌ها ضروری است")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// توضیحات شاخص
    /// </summary>
    [MaxLength(1000)]
    [DisplayName("توضیحات")]
    [SBVR(SBVRModality.Recommended, "توضیحات شاخص", "توضیحات برای درک بهتر شاخص و نحوه استفاده از آن")]
    public string? Description { get; set; }

    /// <summary>
    /// دسته‌بندی شاخص
    /// </summary>
    [DisplayName("دسته‌بندی")]
    [SBVR(SBVRModality.Recommended, "دسته‌بندی شاخص", "دسته‌بندی برای گروه‌بندی و فیلتر کردن شاخص‌ها استفاده می‌شود")]
    [OldDbMap("Category")]
    public MetricCategory MetricCategory { get; set; }

    /// <summary>
    /// دسته‌بندی فرعی (اختیاری)
    /// </summary>
    [MaxLength(100)]
    [DisplayName("دسته‌بندی فرعی")]
    public string? SubCategory { get; set; }

    /// <summary>
    /// سطح محاسبه شاخص
    /// </summary>
    [DisplayName("سطح محاسبه")]
    [SBVR(SBVRModality.Obligatory, "سطح محاسبه", "سطح محاسبه تعیین می‌کند شاخص برای چه موجودیتی محاسبه می‌شود")]
    public MetricLevel Level { get; set; }

    /// <summary>
    /// نوع داده شاخص
    /// </summary>
    [DisplayName("نوع داده")]
    [SBVR(SBVRModality.Obligatory, "نوع داده", "نوع داده برای ذخیره و نمایش صحیح مقدار شاخص ضروری است")]
    public MetricDataType DataType { get; set; }

    /// <summary>
    /// واحد اندازه‌گیری
    /// </summary>
    [MaxLength(50)]
    [DisplayName("واحد")]
    [SBVR(SBVRModality.Recommended, "واحد اندازه‌گیری", "واحد برای نمایش صحیح مقدار شاخص استفاده می‌شود")]
    public string? Unit { get; set; }

    /// <summary>
    /// حداقل مقدار مجاز
    /// </summary>
    [DisplayName("حداقل مقدار")]
    public decimal? MinValue { get; set; }

    /// <summary>
    /// حداکثر مقدار مجاز
    /// </summary>
    [DisplayName("حداکثر مقدار")]
    public decimal? MaxValue { get; set; }

    /// <summary>
    /// فرمول محاسبه شاخص (SQL, C#, Python)
    /// </summary>
    [MaxLength(2000)]
    [DisplayName("فرمول محاسبه")]
    [SBVR(SBVRModality.Obligatory, "فرمول محاسبه", "فرمول برای محاسبه مقدار شاخص ضروری است")]
    public string Formula { get; set; } = null!;

    /// <summary>
    /// نوع فرمول (SQL, CSharp, Python)
    /// </summary>
    [DisplayName("نوع فرمول")]
    [SBVR(SBVRModality.Obligatory, "نوع فرمول", "نوع فرمول تعیین می‌کند چگونه فرمول اجرا شود")]
    public MetricFormulaType FormulaType { get; set; }

    /// <summary>
    /// فاصله زمانی محاسبه (ساعت)
    /// </summary>
    [DisplayName("فاصله محاسبه (ساعت)")]
    [SBVR(SBVRModality.Recommended, "فاصله محاسبه", "فاصله محاسبه تعیین می‌کند هر چند ساعت یکبار شاخص محاسبه شود")]
    public int? CalculationIntervalHours { get; set; }

    /// <summary>
    /// آیا محاسبه لحظه‌ای است؟
    /// </summary>
    [DisplayName("محاسبه لحظه‌ای")]
    [SBVR(SBVRModality.Recommended, "محاسبه لحظه‌ای", "محاسبه لحظه‌ای برای شاخص‌های مهم که نیاز به به‌روزرسانی فوری دارند")]
    public bool IsRealTime { get; set; }

    /// <summary>
    /// آیا شاخص فعال است؟
    /// </summary>
    [DisplayName("فعال")]
    [SBVR(SBVRModality.Recommended, "وضعیت فعال", "شاخص‌های غیرفعال محاسبه نمی‌شوند")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Metadata اضافی (JSON)
    /// </summary>
    [MaxLength(2000)]
    [DisplayName("اطلاعات اضافی")]
    [SBVR(SBVRModality.Permitted, "اطلاعات اضافی", "Metadata برای ذخیره تنظیمات اضافی و پارامترهای فرمول")]
    public string? Metadata { get; set; }

    // =====================================================
    // Navigation Properties
    // =====================================================

    /// <summary>
    /// وابستگی‌های شاخص (شاخص‌های مورد نیاز)
    /// </summary>
    [DisplayName("وابستگی‌ها")]
    public ICollection<MetricDependency> Dependencies { get; set; } = [];

    /// <summary>
    /// مقادیر محاسبه شده این شاخص
    /// </summary>
    [DisplayName("مقادیر")]
    public ICollection<MetricValue> Values { get; set; } = [];
}

