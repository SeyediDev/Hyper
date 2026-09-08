namespace Hyper.Domain.Entities.Promotions.Surveys;

/// <summary>
/// آیتم نظرسنجی - گزینه‌های قابل انتخاب در نظرسنجی
/// </summary>
[DisplayName("آیتم نظرسنجی")]
[SBVR(SBVRModality.Obligatory, "ساختار نظرسنجی", "هر نظرسنجی باید حداقل دو گزینه داشته باشد")]
public class SurveyItem : HyperBaseCoreAuditableEntity<int>
{
    /// <summary>
    /// شناسه نظرسنجی
    /// </summary>
    [DisplayName("شناسه نظرسنجی")]
    [SBVR(SBVRModality.Obligatory, "ارتباط با نظرسنجی", "هر آیتم باید به یک نظرسنجی تعلق داشته باشد")]
    public int SurveyId { get; set; }

    /// <summary>
    /// نظرسنجی
    /// </summary>
    [DisplayName("نظرسنجی")]
    public Survey Survey { get; set; } = null!;

    /// <summary>
    /// متن گزینه
    /// </summary>
    [DisplayName("متن گزینه")]
    [InDisplayString]
    [MaxLength(500)]
    [SBVR(SBVRModality.Obligatory, "محتوای گزینه", "متن گزینه برای نمایش به مشتریان ضروری است")]
    public string OptionText { get; set; } = null!;

    /// <summary>
    /// ترتیب نمایش
    /// </summary>
    [DisplayName("ترتیب")]
    [SBVR(SBVRModality.Recommended, "سازماندهی", "ترتیب برای نمایش منظم گزینه‌ها به مشتریان استفاده می‌شود")]
    public int DisplayOrder { get; set; }

    /// <summary>
    /// آیا این گزینه، پاسخ صحیح است (فقط برای مسابقه)
    /// </summary>
    [DisplayName("پاسخ صحیح")]
    [SBVR(SBVRModality.Obligatory, "مسابقه", "در مسابقات، یک گزینه باید به عنوان پاسخ صحیح مشخص شود")]
    public bool IsCorrectAnswer { get; set; } = false;

    /// <summary>
    /// تعداد رأی‌ها
    /// </summary>
    [DisplayName("تعداد رأی")]
    [SBVR(SBVRModality.Calculated, "تحلیل نتایج", "تعداد رأی‌ها برای تحلیل نتایج نظرسنجی و نمایش آمار استفاده می‌شود")]
    public int VoteCount { get; set; } = 0;

    /// <summary>
    /// شناسه تصویر گزینه (اختیاری)
    /// </summary>
    [DisplayName("شناسه تصویر")]
    public int? PictureId { get; set; }

    /// <summary>
    /// تصویر گزینه
    /// </summary>
    [DisplayName("تصویر")]
    [SBVR(SBVRModality.Permitted, "نمایش بصری", "تصویر برای بهبود تجربه کاربری استفاده می‌شود")]
    public Document? Picture { get; set; }

    /// <summary>
    /// انتخاب‌های مشتریان
    /// </summary>
    [DisplayName("انتخاب‌ها")]
    public ICollection<SurveyParticipation> Participations { get; set; } = [];
}



