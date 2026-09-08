namespace Hyper.Domain.Entities.Surveys;

/// <summary>
/// نظرسنجی - موجودیت برای مدیریت نظرسنجی‌ها و مسابقات
/// </summary>
[DisplayName("نظرسنجی")]
[SBVR(SBVRModality.Obligatory, "مدیریت نظرسنجی", "هر نظرسنجی باید برای جمع‌آوری بازخورد مشتریان و تحلیل رضایت قابل شناسایی باشد")]
[SBVR(SBVRModality.Recommended, "مدیریت نظرسنجی", "نظرسنجی‌ها باید برای بهبود محصولات، خدمات و افزایش تعامل مشتریان استفاده شوند")]
public class Survey : HyperBaseCoreAuditableEntity<int>
{
    /// <summary>
    /// شناسه اکوسیستم
    /// </summary>
    [DisplayName("شناسه اکوسیستم")]
    [SBVR(SBVRModality.Obligatory, "چندین اکوسیستم", "هر نظرسنجی باید به یک اکوسیستم مشخص تعلق داشته باشد")]
    public int TenantId { get; set; }

    /// <summary>
    /// اکوسیستم
    /// </summary>
    [DisplayName("اکوسیستم")]
    public Tenant Tenant { get; set; } = null!;

    /// <summary>
    /// عنوان نظرسنجی
    /// </summary>
    [DisplayName("عنوان")]
    [InDisplayString]
    [MaxLength(200)]
    [SBVR(SBVRModality.Obligatory, "شناسایی نظرسنجی", "عنوان برای نمایش به مشتریان و مدیریت نظرسنجی‌ها ضروری است")]
    public string Title { get; set; } = null!;

    /// <summary>
    /// توضیحات نظرسنجی
    /// </summary>
    [DisplayName("توضیحات")]
    [MaxLength(1000)]
    [SBVR(SBVRModality.Recommended, "مستندسازی نظرسنجی", "توضیحات برای ارائه اطلاعات کامل به مشتریان استفاده می‌شود")]
    public string? Description { get; set; }

    /// <summary>
    /// نوع نظرسنجی (عادی یا مسابقه)
    /// </summary>
    [DisplayName("نوع نظرسنجی")]
    [SBVR(SBVRModality.Obligatory, "تمایز نظرسنجی", "نوع برای تمایز بین نظرسنجی عادی و مسابقه ضروری است")]
    public SurveyType SurveyType { get; set; } = SurveyType.RegularSurvey;

    /// <summary>
    /// شناسه محصول مرتبط (اختیاری)
    /// </summary>
    [DisplayName("شناسه محصول")]
    [SBVR(SBVRModality.Permitted, "ارتباط با محصول", "نظرسنجی می‌تواند به یک محصول خاص مرتبط باشد")]
    public int? ProductId { get; set; }

    /// <summary>
    /// محصول مرتبط
    /// </summary>
    [DisplayName("محصول")]
    public Product? Product { get; set; }

    /// <summary>
    /// آیا نظرسنجی فعال است
    /// </summary>
    [DisplayName("فعال")]
    [SBVR(SBVRModality.Permitted, "مدیریت چرخه حیات", "وضعیت فعال برای کنترل نمایش نظرسنجی به مشتریان استفاده می‌شود")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// تاریخ شروع نظرسنجی
    /// </summary>
    [DisplayName("تاریخ شروع")]
    [SBVR(SBVRModality.Recommended, "زمان‌بندی نظرسنجی", "تاریخ شروع برای زمان‌بندی انتشار نظرسنجی استفاده می‌شود")]
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// تاریخ پایان نظرسنجی
    /// </summary>
    [DisplayName("تاریخ پایان")]
    [SBVR(SBVRModality.Recommended, "زمان‌بندی نظرسنجی", "تاریخ پایان برای محدود کردن مدت زمان نظرسنجی استفاده می‌شود")]
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// آیا مشتری می‌تواند چند گزینه را انتخاب کند (فقط برای نظرسنجی عادی)
    /// </summary>
    [DisplayName("انتخاب چند گزینه‌ای")]
    [SBVR(SBVRModality.Permitted, "تنظیمات نظرسنجی", "در نظرسنجی‌های عادی می‌توان به مشتری اجازه داد چند گزینه را انتخاب کند")]
    public bool AllowMultipleSelection { get; set; } = false;

    /// <summary>
    /// آیا نتایج نظرسنجی به مشتری نمایش داده شود
    /// </summary>
    [DisplayName("نمایش نتایج")]
    [SBVR(SBVRModality.Recommended, "شفافیت", "نمایش نتایج برای افزایش تعامل و شفافیت با مشتریان استفاده می‌شود")]
    public bool ShowResults { get; set; } = true;

    /// <summary>
    /// تعداد کل شرکت‌کنندگان
    /// </summary>
    [DisplayName("تعداد شرکت‌کنندگان")]
    [SBVR(SBVRModality.Calculated, "تحلیل مشارکت", "تعداد شرکت‌کنندگان برای تحلیل میزان مشارکت مشتریان استفاده می‌شود")]
    public int TotalParticipants { get; set; } = 0;

    /// <summary>
    /// امتیاز قابل کسب از شرکت در نظرسنجی
    /// </summary>
    [DisplayName("امتیاز شرکت")]
    [SBVR(SBVRModality.Recommended, "تشویق مشارکت", "امتیاز برای تشویق مشتریان به شرکت در نظرسنجی استفاده می‌شود")]
    public long? ParticipationPoints { get; set; }

    /// <summary>
    /// امتیاز قابل کسب از پاسخ صحیح (فقط برای مسابقه)
    /// </summary>
    [DisplayName("امتیاز پاسخ صحیح")]
    [SBVR(SBVRModality.Recommended, "پاداش مسابقه", "امتیاز پاسخ صحیح برای پاداش دادن به برندگان مسابقه استفاده می‌شود")]
    public long? CorrectAnswerPoints { get; set; }

    /// <summary>
    /// آیتم‌های نظرسنجی
    /// </summary>
    [DisplayName("آیتم‌ها")]
    public ICollection<SurveyItem> Items { get; set; } = [];

    /// <summary>
    /// شرکت‌کنندگان در نظرسنجی
    /// </summary>
    [DisplayName("شرکت‌کنندگان")]
    public ICollection<SurveyParticipation> Participations { get; set; } = [];
}



