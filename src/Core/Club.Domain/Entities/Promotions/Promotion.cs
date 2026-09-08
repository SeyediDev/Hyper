using Neo.Bpms.Domain.Models.Attributes.RelationshipAttributes;

namespace Hyper.Domain.Entities.Promotions;

[DisplayName("پویش/کمپین")]
[SBVR(SBVRModality.Obligatory, "پویش‌های بازاریابی", "هر پویش باید برای تشویق رفتارهای مطلوب مشتریان و افزایش فروش قابل اجرا باشد")]
[SBVR(SBVRModality.Recommended, "پویش‌های بازاریابی", "پویش‌ها باید برای تحلیل اثربخشی، محاسبه ROI و بهینه‌سازی استراتژی‌های بازاریابی طراحی شوند")]
public class Promotion : HyperBaseCoreConfigAuditableEntity<int>, ISubOfTenant
{
    public int TenantId { get; set; }
    [DisplayName("اکوسیستم")]
    public Tenant Tenant { get; set; } = null!;

    /// <summary>
    /// عنوان پویش
    /// </summary>
    [DisplayName("عنوان")]
    [InDisplayString]
    [MaxLength(41)]
    public string Title { get; set; } = null!;

    /// <summary>
    /// دسته‌بندی پویش بر اساس محتوا/موضوع
    /// </summary>
    [DisplayName("دسته‌بندی")]
    [SBVR(SBVRModality.Obligatory, "دسته‌بندی پویش", "هر پویش باید برای دسته‌بندی و گزارش‌گیری بر اساس محتوا/موضوع دسته‌بندی شود")]
    [SBVR(SBVRModality.Recommended, "دسته‌بندی پویش", "دسته‌بندی پویش باید برای تحلیل اثربخشی و بهینه‌سازی استراتژی‌های بازاریابی استفاده شود")]
    [SBVR(SBVRModality.Permitted, "دسته‌بندی پویش", "دسته‌بندی پویش می‌تواند برای فیلتر کردن و جستجوی پویش‌ها استفاده شود")]
    [SBVR(SBVRModality.Obligatory, "استفاده از دسته‌بندی", "دسته‌بندی پویش فقط برای دسته‌بندی و گزارش‌گیری استفاده می‌شود و نباید در منطق بیزینس استفاده شود")]
    public PromotionCategory Category { get; set; }

    /// <summary>
    /// تاریخ شروع پویش (بازه زمانی کلی پویش)
    /// این بازه تعریف کننده بازه اصلی پویش است و رویدادهای دریافتی فقط در این بازه بررسی می‌شوند
    /// برای تعریف هزینه‌ها در بازه‌های زمانی مختلف، باید از جدول تسهیم هزینه‌ها (PromotionCostAllocation) استفاده شود
    /// </summary>
    [DisplayName("تاریخ شروع")]
    [SBVR(SBVRModality.Recommended, "مدیریت زمان‌بندی", "تاریخ شروع پویش باید برای برنامه‌ریزی و هماهنگی با سایر پویش‌ها مشخص باشد")]
    [SBVR(SBVRModality.Obligatory, "بازه زمانی پویش", "بازه زمانی پویش تعریف کننده بازه اصلی پویش است")]
    [SBVR(SBVRModality.Obligatory, "بررسی رویدادها", "رویدادهای دریافتی فقط در بازه زمانی پویش بررسی می‌شوند")]
    [SBVR(SBVRModality.Obligatory, "تسهیم هزینه", "برای تعریف هزینه‌ها در بازه‌های زمانی مختلف، باید از جدول تسهیم هزینه‌ها استفاده شود")]
    public DateTime? FromDate { get; set; }

    /// <summary>
    /// تاریخ پایان پویش (بازه زمانی کلی پویش)
    /// این بازه تعریف کننده بازه اصلی پویش است و رویدادهای دریافتی فقط در این بازه بررسی می‌شوند
    /// برای تعریف هزینه‌ها در بازه‌های زمانی مختلف، باید از جدول تسهیم هزینه‌ها (PromotionCostAllocation) استفاده شود
    /// </summary>
    [DisplayName("تاریخ پایان")]
    [SBVR(SBVRModality.Recommended, "مدیریت زمان‌بندی", "تاریخ پایان پویش باید برای تحلیل دوره‌ای و برنامه‌ریزی پویش‌های آینده مشخص باشد")]
    [SBVR(SBVRModality.Obligatory, "بازه زمانی پویش", "بازه زمانی پویش تعریف کننده بازه اصلی پویش است")]
    [SBVR(SBVRModality.Obligatory, "بررسی رویدادها", "رویدادهای دریافتی فقط در بازه زمانی پویش بررسی می‌شوند")]
    [SBVR(SBVRModality.Obligatory, "تسهیم هزینه", "برای تعریف هزینه‌ها در بازه‌های زمانی مختلف، باید از جدول تسهیم هزینه‌ها استفاده شود")]
    public DateTime? ToDate { get; set; }

    [DisplayName("ساعت شروع")]
    public int? FromHour { get; set; }

    [DisplayName("ساعت پایان")]
    public int? ToHour { get; set; }

    /// <summary>
    /// وضعیت پویش - Active, Paused, Completed, Cancelled
    /// </summary>
    [DisplayName("وضعیت پویش")]
    [SBVR(SBVRModality.Recommended, "مدیریت چرخه حیات", "وضعیت پویش برای مدیریت و گزارش‌گیری استفاده می‌شود")]
    public PromotionStatus? Status { get; set; }

    /// <summary>
    /// آیا این کمپین در پرتال مشتریان قابل نمایش است؟
    /// </summary>
    [DisplayName("قابل نمایش برای مشتری")]
    [SBVR(SBVRModality.Permitted, "نمایش کمپین در پرتال", "این فیلد تعیین می‌کند که آیا این کمپین در پرتال باشگاه مشتریان برای مشتریان قابل نمایش است یا خیر. مشتریان می‌توانند از کمپین‌های قابل نمایش مطلع شوند و از نحوه امتیازگیری و اقدامات لازم آگاه شوند.")]
    public bool IsVisibleToCustomer { get; set; } = false;

    /// <summary>
    /// توضیح کوتاه کمپین برای نمایش در پرتال مشتریان
    /// </summary>
    [DisplayName("توضیح کوتاه")]
    [MaxLength(500)]
    [SBVR(SBVRModality.Permitted, "اطلاع‌رسانی به مشتری", "توضیح کوتاه برای آگاهی سریع مشتری از هدف و مزایای کمپین")]
    public string? ShortDescription { get; set; }

    /// <summary>
    /// متنی که مزایای شرکت در کمپین را توضیح می‌دهد
    /// </summary>
    [DisplayName("مزایای کمپین")]
    [MaxLength(1000)]
    [SBVR(SBVRModality.Permitted, "اطلاع‌رسانی به مشتری", "بیان مزایایی که مشتری با شرکت در این کمپین برای خود به دست می‌آورد")]
    public string? Benefits { get; set; }

    /// <summary>
    /// نحوه شرکت و رفتار موردعلاقه برای کسب امتیاز/جوایز
    /// </summary>
    [DisplayName("نحوه شرکت")]
    [MaxLength(1000)]
    [SBVR(SBVRModality.Permitted, "اطلاع‌رسانی به مشتری", "توضیح در خصوص اقداماتی که مشتری باید انجام دهد تا در کمپین شرکت کند و امتیاز/جایزه کسب کند")]
    public string? ParticipationGuide { get; set; }

    /// <summary>
    /// آدرس تصویر از پیش تعریف شده (1:1) برای کارت کمپین
    /// </summary>
    [DisplayName("تصویر کارت کمپین")]
    [MaxLength(500)]
    [SBVR(SBVRModality.Permitted, "نمایش تصویری", "تصویری با نسبت 1:1 برای نمایش در کارت کمپین در پرتال مشتری")]
    public string? CardImageUrl { get; set; }

    /// <summary>
    /// آدرس تصویر بنر کمپین برای نمایش در بخش تفاصیل کمپین (16:9 یا 2:1)
    /// </summary>
    [DisplayName("تصویر بنر کمپین")]
    [MaxLength(500)]
    [SBVR(SBVRModality.Permitted, "نمایش تصویری", "تصویر بنر با نسبت‌های استاندارد (16:9 یا 2:1) برای نمایش در صفحه جزئیات کمپین")]
    public string? BannerImageUrl { get; set; }

    /// <summary>
    /// رنگ اصلی کمپین برای تطابق بصری (Hex Color)
    /// </summary>
    [DisplayName("رنگ اصلی")]
    [MaxLength(9)]
    [SBVR(SBVRModality.Permitted, "طراحی رابط", "کد رنگ HEX برای تطابق طراحی کمپین در رابط کاربری")]
    public string? PrimaryColor { get; set; }

    /// <summary>
    /// نوع مزایا/جوایز این کمپین (مثلا: امتیاز، تخفیف، جایزه فیزیکی)
    /// </summary>
    [DisplayName("نوع جایزه")]
    [SBVR(SBVRModality.Permitted, "توضیح جوایز", "نوع جایزه یا مزایایی که کمپین ارائه می‌دهد")]
    public PromotionRewardType? RewardType { get; set; }

    /// <summary>
    /// آدرس آیکن نمادین برای نمایش در لیست کمپین‌ها
    /// </summary>
    [DisplayName("آیکن کمپین")]
    [MaxLength(500)]
    [SBVR(SBVRModality.Permitted, "نمایش تصویری", "آیکن یا نماد کوچک برای شناخت سریع کمپین در لیست‌ها")]
    public string? IconUrl { get; set; }

    // =====================================================
    // Navigation Properties
    // =====================================================

    /// <summary>
    /// محرک‌های عملیات پویش - لیست محرک‌هایی که باید دریافت شوند
    /// </summary>
    [DisplayName("محرک‌های عملیات پویش")]
    [SBVR(SBVRModality.Permitted, "محرک‌های عملیات پویش", "محرک‌های عملیات پویش برای تعریف رویدادهای فعال‌سازی پویش")]
    [OldDbMap("Conditions")]
    public ICollection<PromotionTrigger> Triggers { get; set; } = [];

    /// <summary>
    /// عملیات پویش - لیست عملیاتی که انجام می‌شوند
    /// </summary>
    [DisplayName("عملیات پویش")]
    [SBVR(SBVRModality.Permitted, "عملیات پویش", "عملیات پویش برای تعریف اقداماتی که در پویش انجام می‌شود")]
    public ICollection<PromotionAction> Actions { get; set; } = [];

    /// <summary>
    /// جوامع/بازارهای مشتریان هدف - لیست جوامعی که پویش برای آن‌ها قابل دسترسی است
    /// اگر لیست خالی باشد، پویش برای همه مشتریان قابل دسترسی است
    /// </summary>
    [DisplayName("جوامع/بازارهای مشتریان هدف")]
    [SBVR(SBVRModality.Permitted, "هدف‌گذاری مشتریان", "لیست جوامع برای تعریف چند جامعه/بازار مشتریان برای یک پویش")]
    [SBVR(SBVRModality.Obligatory, "هدف‌گذاری مشتریان", "اگر هیچ جامعه‌ای تعریف نشده باشد، پویش برای همه مشتریان قابل دسترسی است")]
    [SBVR(SBVRModality.Obligatory, "شرکت در پویش", "مشتری می‌تواند در پویش شرکت کند", "اگر حداقل در یک جامعه از جامعه‌های تعریف شده برای پویش عضو باشد")]
    public ICollection<PromotionCustomerSegment> CustomerSegments { get; set; } = [];

    /// <summary>
    /// تسهیم‌های هزینه پویش - توزیع هزینه در بازه‌های زمانی مختلف
    /// </summary>
    [DisplayName("تسهیم‌های هزینه")]
    [SBVR(SBVRModality.Permitted, "تسهیم هزینه", "تسهیم‌های هزینه برای توزیع هزینه در بازه‌های زمانی")]
    public ICollection<PromotionCostAllocation> CostAllocations { get; set; } = [];

    /// <summary>
    /// عضویت مشتریان در این پویش
    /// </summary>
    [DisplayName("عضویت مشتریان")]
    [SBVR(SBVRModality.Permitted, "شرکت‌های پویش", "عضویت مشتریان برای ردیابی وضعیت شرکت در پویش")]
    public ICollection<PromotionParticipation> Participations { get; set; } = [];

    /// <summary>
    /// معیارهای عملکرد پویش - فیلدهای محاسبه شده (Calculated)
    /// رابطه یک به یک - FK در PromotionMetrics.PromotionId
    /// </summary>
    [DisplayName("معیارهای عملکرد")]
    [SBVR(SBVRModality.Calculated, "معیارهای عملکرد", "معیارهای عملکرد برای تحلیل اثربخشی و بهینه‌سازی")]
    [OAttr_AssociationMap("Id", "PromotionId")]
    public PromotionMetrics? Metrics { get; set; }
}