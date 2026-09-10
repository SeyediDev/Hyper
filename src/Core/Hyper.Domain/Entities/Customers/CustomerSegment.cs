namespace Hyper.Domain.Entities.Customers;

[DisplayName("جامعه/بازار مشتریان")]
[SBVR(SBVRModality.Obligatory, "بخش‌بندی مشتریان", "هر جامعه/بازار مشتریان باید برای هدف‌گذاری کمپین‌ها، شخصی‌سازی خدمات و تحلیل رفتار قابل شناسایی باشد")]
[SBVR(SBVRModality.Recommended, "بخش‌بندی مشتریان", "جامعه‌ها/بازارها باید برای بهینه‌سازی استراتژی‌های بازاریابی و افزایش نرخ تبدیل طراحی شوند")]
[Microsoft.EntityFrameworkCore.IndexAttribute(nameof(CountryId))]
[Microsoft.EntityFrameworkCore.IndexAttribute(nameof(ProvinceId))]
[Microsoft.EntityFrameworkCore.IndexAttribute(nameof(CityId))]
public partial class CustomerSegment : HyperBaseCoreAuditableEntity<int>
{
    public int TenantId { get; set; }
    [DisplayName("اکوسیستم")]
    public Tenant Tenant { get; set; } = null!;

    /// <summary>
    /// عنوان جامعه/بازار مشتریان
    /// </summary>
    [DisplayName("عنوان جامعه/بازار مشتریان")]
    [InDisplayString]
    [MaxLength(41)]
    [SBVR(SBVRModality.Obligatory, "شناسایی جامعه/بازار", "عنوان جامعه/بازار باید برای مدیریت کمپین‌ها و گزارش‌گیری واضح و قابل فهم باشد")]
    public string Title { get; set; } = null!;

    /// <summary>
    /// توضیحات جامعه/بازار مشتریان
    /// </summary>
    [DisplayName("توضیحات جامعه/بازار مشتریان")]
    [MaxLength(512)]
    [SBVR(SBVRModality.Recommended, "مستندسازی جامعه/بازار", "توضیحات جامعه/بازار باید شامل معیارهای تشکیل و هدف از ایجاد جامعه/بازار باشد")]
    public string? Description { get; set; } = null!;

    /// <summary>
    /// وضعیت فعال جامعه/بازار
    /// </summary>
    [DisplayName("وضعیت فعال بودن جامعه/بازار")]
    [SBVR(SBVRModality.Recommended, "مدیریت چرخه حیات", "وضعیت فعال تعیین می‌کند که آیا جامعه/بازار در کمپین‌ها و تحلیل‌ها استفاده شود")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// تخمین تعداد جامعه/بازار
    /// </summary>
    [DisplayName("اندازه تخمینی جامعه/بازار")]
    [SBVR(SBVRModality.Calculated, "برنامه‌ریزی کمپین", "اندازه تخمینی برای برنامه‌ریزی بودجه و منابع کمپین‌ها استفاده می‌شود")]
    public int EstimatedSize { get; set; }

    /// <summary>
    /// تعداد واقعی جامعه/بازار
    /// </summary>
    [DisplayName("اندازه واقعی جامعه/بازار")]
    [SBVR(SBVRModality.Calculated, "تحلیل اثربخشی", "اندازه واقعی برای تحلیل اثربخشی کمپین‌ها و محاسبه نرخ تبدیل استفاده می‌شود")]
    public int ActualSize { get; set; }

    /// <summary>
    /// آخرین محاسبه جامعه/بازار
    /// </summary>
    [DisplayName("تاریخ آخرین محاسبه جامعه/بازار")]
    [SBVR(SBVRModality.Calculated, "مدیریت به‌روزرسانی", "تاریخ آخرین محاسبه برای اطمینان از به‌روز بودن داده‌های جامعه/بازار استفاده می‌شود")]
    public DateTime? LastCalculationDate { get; set; }

    /// <summary>
    /// فاصله محاسبه جامعه/بازار
    /// </summary>
    [DisplayName("فاصله زمانی محاسبه مجدد جامعه/بازار")]
    [SBVR(SBVRModality.Recommended, "اتوماسیون محاسبات", "فاصله زمانی تعیین می‌کند که جامعه/بازار هر چند روز یکبار محاسبه مجدد شود")]
    public int CalculationIntervalDays { get; set; } = 1;

    /// <summary>
    /// نحوه محاسبه جامعه/بازار
    /// </summary>
    [DisplayName("نحوه محاسبه جامعه/بازار")]
    [SBVR(SBVRModality.Recommended, "روش‌شناسی محاسبه", "نحوه محاسبه تعیین می‌کند که جامعه/بازار بر اساس چه الگوریتمی تشکیل شود")]
    public SegmentCalculationMode CalculationMode { get; set; } = SegmentCalculationMode.Automatic;

    /// <summary>
    /// حداقل امتیاز عضویت
    /// </summary>
    [DisplayName("حداقل نمره عضویت در جامعه/بازار")]
    [SBVR(SBVRModality.Calculated, "تعریف مرزهای جامعه/بازار", "حداقل نمره عضویت برای تعیین مرزهای جامعه/بازار و جلوگیری از تداخل استفاده می‌شود")]
    public decimal? MinMembershipScore { get; set; }

    /// <summary>
    /// حداکثر امتیاز عضویت
    /// </summary>
    [DisplayName("حداکثر نمره عضویت در جامعه/بازار")]
    [SBVR(SBVRModality.Calculated, "تعریف مرزهای جامعه/بازار", "حداکثر نمره عضویت برای تعیین مرزهای جامعه/بازار و جلوگیری از تداخل استفاده می‌شود")]
    public decimal? MaxMembershipScore { get; set; }

    /// <summary>
    /// نرخ رشد جامعه/بازار
    /// </summary>
    [DisplayName("نرخ رشد جامعه/بازار")]
    [SBVR(SBVRModality.Calculated, "تحلیل روندها", "نرخ رشد جامعه/بازار برای تحلیل روندهای بازار و پیش‌بینی نیازهای آینده استفاده می‌شود")]
    public decimal GrowthRate { get; set; }

    /// <summary>
    /// نرخ نگهداری جامعه/بازار
    /// </summary>
    [DisplayName("نرخ حفظ مشتریان جامعه/بازار")]
    [SBVR(SBVRModality.Calculated, "تحلیل وفاداری", "نرخ حفظ برای تحلیل وفاداری مشتریان و بهینه‌سازی استراتژی‌های حفظ مشتری استفاده می‌شود")]
    public decimal RetentionRate { get; set; }

    /// <summary>
    /// نرخ تعامل جامعه/بازار
    /// </summary>
    [DisplayName("نرخ تعامل جامعه/بازار")]
    [SBVR(SBVRModality.Calculated, "تحلیل مشارکت", "نرخ تعامل برای تحلیل میزان مشارکت مشتریان در جامعه/بازار و بهینه‌سازی محتوا استفاده می‌شود")]
    public decimal EngagementRate { get; set; }

    /// <summary>
    /// نحوه عضویت در جامعه/بازار
    /// </summary>
    [DisplayName("نحوه عضویت در جامعه/بازار")]
    [SBVR(SBVRModality.Obligatory, "مدیریت عضویت", "نحوه عضویت تعیین می‌کند که مشتری چگونه می‌تواند عضو جامعه/بازار شود")]
    [SBVR(SBVRModality.Recommended, "کنترل دسترسی", "انتخاب صحیح نحوه عضویت برای جلوگیری از عضویت نامناسب ضروری است")]
    public CustomerSegmentJoinMode JoinMode { get; set; } = CustomerSegmentJoinMode.SystemOnly;

    /// <summary>
    /// قابلیت نمایش در پرتال مشتریان
    /// </summary>
    [DisplayName("نمایش در پرتال مشتریان")]
    [SBVR(SBVRModality.Recommended, "نمایش جامعه/بازار", "تعیین می‌کند که آیا جامعه/بازار در پرتال مشتریان قابل مشاهده است")]
    [SBVR(SBVRModality.Permitted, "محرمانگی", "برخی جامعه‌ها/بازارها ممکن است محرمانه باشند و نباید در پرتال نمایش داده شوند")]
    public bool IsVisibleInPortal { get; set; } = false;

    /// <summary>
    /// آدرس تصویر جامعه/بازار
    /// </summary>
    [DisplayName("آدرس تصویر جامعه/بازار")]
    [MaxLength(512)]
    [SBVR(SBVRModality.Permitted, "تجربه کاربری", "تصویر جامعه/بازار برای بهبود تجربه کاربری در پرتال مشتریان استفاده می‌شود")]
    public string? ImageUrl { get; set; }

    /// <summary>
    /// مزایای عضویت در جامعه/بازار (JSON array)
    /// </summary>
    [DisplayName("مزایای عضویت")]
    [MaxLength(2000)]
    [SBVR(SBVRModality.Recommended, "انگیزش مشتری", "مزایای عضویت برای ترغیب مشتریان به عضویت در جامعه/بازار استفاده می‌شود")]
    public string? Benefits { get; set; }

    // Optional Geography scope
    [DisplayName("کشور")]
    public int? CountryId { get; set; }
    [DisplayName("کشور")]
    public Country? Country { get; set; }

    [DisplayName("استان")]
    public int? ProvinceId { get; set; }
    [DisplayName("استان")]
    public Province? Province { get; set; }

    [DisplayName("شهر")]
    public int? CityId { get; set; }
    [DisplayName("شهر")]
    public City? City { get; set; }

    // Navigation Properties
    public ICollection<CustomerSegmentKindCondition> KindConditions { get; set; } = [];
    public ICollection<CustomerSegmentMembership> Memberships { get; set; } = [];
}
