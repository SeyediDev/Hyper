namespace Hyper.Domain.Entities.Promotions.Surveys.Data;

/// <summary>
/// شرکت در نظرسنجی - ثبت پاسخ‌های مشتریان به نظرسنجی‌ها
/// </summary>
[DisplayName("شرکت در نظرسنجی")]
[SBVR(SBVRModality.Obligatory, "مدیریت نظرسنجی", "هر شرکت در نظرسنجی باید برای تحلیل پاسخ‌ها و اعطای امتیاز قابل شناسایی باشد")]
public class SurveyParticipation : HyperBaseCoreAuditableEntity<int>
{
    /// <summary>
    /// شناسه نظرسنجی
    /// </summary>
    [DisplayName("شناسه نظرسنجی")]
    public int SurveyId { get; set; }

    /// <summary>
    /// نظرسنجی
    /// </summary>
    [DisplayName("نظرسنجی")]
    public Survey Survey { get; set; } = null!;

    [DisplayName("شناسه مشتری")]
    public int CustomerTenantId { get; set; }

    /// <summary>
    /// مشتری
    /// </summary>
    [DisplayName("مشتری")]
    public CustomerTenant CustomerTenant { get; set; } = null!;

    /// <summary>
    /// شناسه گزینه انتخاب شده
    /// </summary>
    [DisplayName("شناسه گزینه")]
    [SBVR(SBVRModality.Obligatory, "انتخاب گزینه", "مشتری باید حداقل یک گزینه را انتخاب کند")]
    public int SelectedItemId { get; set; }

    /// <summary>
    /// گزینه انتخاب شده
    /// </summary>
    [DisplayName("گزینه انتخابی")]
    public SurveyItem SelectedItem { get; set; } = null!;

    /// <summary>
    /// تاریخ شرکت
    /// </summary>
    [DisplayName("تاریخ شرکت")]
    [SBVR(SBVRModality.Calculated, "زمان‌بندی", "تاریخ شرکت برای تحلیل زمانی مشارکت مشتریان استفاده می‌شود")]
    public DateTime ParticipationDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// آیا پاسخ صحیح بود (فقط برای مسابقه)
    /// </summary>
    [DisplayName("پاسخ صحیح")]
    [SBVR(SBVRModality.Calculated, "ارزیابی مسابقه", "در مسابقات، صحت پاسخ برای اعطای امتیاز محاسبه می‌شود")]
    public bool? IsCorrect { get; set; }

    /// <summary>
    /// امتیاز کسب شده
    /// </summary>
    [DisplayName("امتیاز کسب شده")]
    [SBVR(SBVRModality.Calculated, "پاداش", "امتیاز کسب شده برای افزودن به موجودی امتیاز مشتری استفاده می‌شود")]
    public long PointsEarned { get; set; } = 0;

    /// <summary>
    /// نظر یا توضیحات مشتری (اختیاری)
    /// </summary>
    [DisplayName("نظر")]
    [MaxLength(1000)]
    [SBVR(SBVRModality.Permitted, "بازخورد کیفی", "نظر مشتری برای دریافت بازخورد کیفی استفاده می‌شود")]
    public string? Comment { get; set; }
}

