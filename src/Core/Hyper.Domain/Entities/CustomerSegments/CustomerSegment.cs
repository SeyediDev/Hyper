namespace Hyper.Domain.Entities.CustomerSegments;

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

    [DisplayName("عنوان")]
    [InDisplayString]
    [MaxLength(41)]
    [SBVR(SBVRModality.Obligatory, "شناسایی جامعه/بازار", "عنوان جامعه/بازار باید برای مدیریت کمپین‌ها و گزارش‌گیری واضح و قابل فهم باشد")]
    public string Title { get; set; } = null!;

    [DisplayName("کلید")]
    [MaxLength(100)]
    [SBVR(SBVRModality.Obligatory, "فرمول نویسی", "کلید برای خواندن مقادیر جامعه در فرمول نویسی استفاده می شود")]
    public string? Key { get; set; }

    [DisplayName("نوع جامعه/بازار")]
	[SBVR(SBVRModality.Obligatory, "نوع جامعه/بازار", "نوع جامعه/بازار تعیین می‌کند که جامعه/بازار چگونه تعریف شده است")]
	[SBVR(SBVRModality.Recommended, "نوع جامعه/بازار", "انتخاب نوع صحیح باعث مدیریت بهتر جامعه/بازار می‌شود")]
	public CustomerSegmentKind Kind { get; set; }
	
	[DisplayName("توضیحات")]
    [MaxLength(512)]
    [SBVR(SBVRModality.Recommended, "مستندسازی جامعه/بازار", "توضیحات جامعه/بازار باید شامل معیارهای تشکیل و هدف از ایجاد جامعه/بازار باشد")]
    public string? Description { get; set; } = null!;

    /// <summary>
    /// نحوه عضویت در جامعه/بازار
    /// </summary>
    [DisplayName("نحوه عضویت در جامعه/بازار")]
    [SBVR(SBVRModality.Obligatory, "مدیریت عضویت", "نحوه عضویت تعیین می‌کند که مشتری چگونه می‌تواند عضو جامعه/بازار شود")]
    [SBVR(SBVRModality.Recommended, "کنترل دسترسی", "انتخاب صحیح نحوه عضویت برای جلوگیری از عضویت نامناسب ضروری است")]
    public CustomerSegmentJoinMode JoinMode { get; set; } = CustomerSegmentJoinMode.SystemOnly;

    [DisplayName("فرمول شرط")]
    [MaxLength(512)]
    [SBVR(SBVRModality.Permitted, "فرمول شرط", "فرمول شرط برای شرط‌های پیچیده استفاده می‌شود")]
    [SBVR(SBVRModality.Permitted, "فرمول شرط", "در شرط می توانید از Tenant.* استفاده کنید و مقادیر تنظیم شده در اکوسیستم را استفاده کنید. بجای * کلید مقدار را استفاده کنید.")]
    [SBVR(SBVRModality.Permitted, "فرمول شرط", "در شرط می توانید از Customer.* استفاده کنید و مقادیر ویژگی‌ها مشتری را استفاده کنید. بجای * کلید مقدار را استفاده کنید.")]
    [SBVR(SBVRModality.Permitted, "فرمول شرط", "در شرط می توانید از Customer.Point.*.Level استفاده کنید و سطح یک امتیاز مشتری را استفاده کنید. بجای * کلید امتیاز را استفاده کنید.")]
    [SBVR(SBVRModality.Permitted, "فرمول شرط", "در شرط می توانید از Customer.Point.* استفاده کنید و موجودی یک امتیاز مشتری را استفاده کنید. بجای * کلید امتیاز را استفاده کنید.")]
    public string? Constraint { get; set; }

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
    public int? CountryId { get; set; }
    [DisplayName("کشور")]
    public Country? Country { get; set; }

    public int? ProvinceId { get; set; }
    [DisplayName("استان")]
    public Province? Province { get; set; }

    public int? CityId { get; set; }
    [DisplayName("شهر")]
    public City? City { get; set; }

    [DisplayName("وضعیت فعال بودن")]
    [SBVR(SBVRModality.Recommended, "مدیریت چرخه حیات", "وضعیت فعال تعیین می‌کند که آیا جامعه/بازار در کمپین‌ها و تحلیل‌ها استفاده شود")]
    public bool IsActive { get; set; } = true;

    [DisplayName("اندازه تخمینی")]
    [SBVR(SBVRModality.Calculated, "برنامه‌ریزی کمپین", "اندازه تخمینی برای برنامه‌ریزی بودجه و منابع کمپین‌ها استفاده می‌شود")]
    public int EstimatedSize { get; set; }

    [DisplayName("اندازه واقعی")]
    [SBVR(SBVRModality.Calculated, "تحلیل اثربخشی", "اندازه واقعی برای تحلیل اثربخشی کمپین‌ها و محاسبه نرخ تبدیل استفاده می‌شود")]
    public int ActualSize { get; set; }

    [DisplayName("تاریخ آخرین محاسبه")]
    [SBVR(SBVRModality.Calculated, "مدیریت به‌روزرسانی", "تاریخ آخرین محاسبه برای اطمینان از به‌روز بودن داده‌های جامعه/بازار استفاده می‌شود")]
    public DateTime? LastCalculationDate { get; set; }

    [DisplayName("فاصله زمانی محاسبه مجدد")]
    [SBVR(SBVRModality.Recommended, "اتوماسیون محاسبات", "فاصله زمانی تعیین می‌کند که جامعه/بازار هر چند روز یکبار محاسبه مجدد شود")]
    public int CalculationIntervalDays { get; set; } = 1;

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

    public ICollection<CustomerSegmentMembership> Memberships { get; set; } = [];
}
