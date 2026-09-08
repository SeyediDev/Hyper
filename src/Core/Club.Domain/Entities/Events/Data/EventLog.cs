namespace Hyper.Domain.Entities.Events.Data;

/// <summary>
/// لاگ رویداد - ثبت تمام رویدادهای سیستم
/// </summary>
[DisplayName("لاگ رویداد")]
[SBVR(SBVRModality.Obligatory, "ردیابی رفتار مشتریان", "هر رویداد باید برای تحلیل رفتار مشتریان، محاسبه امتیازات و اجرای قوانین امتیازدهی ثبت شود")]
[SBVR(SBVRModality.Recommended, "ردیابی رفتار مشتریان", "لاگ‌ها باید برای تحلیل RFM، پیش‌بینی رفتار و بهینه‌سازی کمپین‌های بازاریابی استفاده شوند")]
public class EventLog : HyperBaseCoreLogAuditableEntity<long>
{
    public int TenantId { get; set; }
    [DisplayName("اکوسیستم")]
    [SBVR(SBVRModality.Obligatory, "چندین اکوسیستم", "اکوسیستم مشخص می‌کند رویداد در کدام اکوسیستم ثبت شده است تا کنترل دسترسی و تحلیل‌های اکوسیستمی فراهم شود")]
    public Tenant Tenant { get; set; } = null!;

    public int CustomerTenantId { get; set; }
    [DisplayName("مشتری")]
    [SBVR(SBVRModality.Obligatory, "چندین اکوسیستم", "مشتری برای تحلیل دقیق فعالیت‌های مشتری در محدوده هر اکوسیستم استفاده می‌شود")]
    public CustomerTenant CustomerTenant { get; set; } = null!;

    [DisplayName("نوع فعال‌سازی رویداد")]
    [SBVR(SBVRModality.Obligatory, "دسته‌بندی رویدادها", "نوع فعال‌سازی تعیین می‌کند که رویداد چگونه ایجاد شده است تا تحلیل الگوهای رفتاری امکان‌پذیر باشد")]
    [OldDbMap("TriggerType")]
    public ReceiveEventType ReceiveEventType { get; set; }

    public int? EventChannelId { get; set; }
    [DisplayName("کانال رویداد")]
    [SBVR(SBVRModality.Recommended, "ردیابی کانال رویداد", "کانال رویداد تعیین می‌کند که رویداد از کجا آمده است تا تحلیل کانال‌های ورودی امکان‌پذیر باشد")]
    public EventChannel? EventChannel { get; set; } = null!;

    public int? EventTypeId { get; set; }
    [DisplayName("نوع رویداد")]
    [SBVR(SBVRModality.Recommended, "دسته‌بندی فعالیت‌ها", "نوع رویداد تعیین می‌کند که چه نوع فعالیتی انجام شده است تا تحلیل رفتار مشتریان امکان‌پذیر باشد")]
    public EventType? EventType { get; set; } = null!;

    public int? PromotionId { get; set; }
    [DisplayName("پویش/کمپین")]
    [SBVR(SBVRModality.Permitted, "تحلیل اثربخشی کمپین", "پویش رویداد برای ردیابی اثربخشی کمپین‌های بازاریابی و محاسبه ROI استفاده می‌شود")]
    public Promotion? Promotion { get; set; }

    public int? PointLevelId { get; set; }
    [DisplayName("سطح امتیاز رویداد")]
    [SBVR(SBVRModality.Permitted, "تحلیل سطح مشتریان", "سطح امتیاز برای تحلیل رفتار مشتریان در سطوح مختلف و بهینه‌سازی استراتژی‌ها استفاده می‌شود")]
    public PointLevel? PointLevel { get; set; }

    public int? RewardId { get; set; }

    [DisplayName("پاداش رویداد")]
    [SBVR(SBVRModality.Permitted, "تحلیل علایق مشتریان", "پاداش رویداد برای تحلیل علایق مشتریان در دریافت پاداش‌ها و بهینه‌سازی کاتالوگ استفاده می‌شود")]
    public Reward? Reward { get; set; }

    public int? ProductCategoryId { get; set; }

    [DisplayName("طبقه‌بندی محصول")]
    [SBVR(SBVRModality.Permitted, "تحلیل محصولات", "طبقه‌بندی محصول برای ثبت رویدادهای خرید یا استفاده از محصولات و کسب امتیاز استفاده می‌شود")]
    public ProductCategory? ProductCategory { get; set; }

    public int? ProductId { get; set; }

    [DisplayName("محصول")]
    [SBVR(SBVRModality.Permitted, "تحلیل محصولات", "محصول برای ثبت رویدادهای خرید یا استفاده از محصولات و کسب امتیاز استفاده می‌شود")]
    public Product? Product { get; set; }

    public int? AssetId { get; set; }

    [DisplayName("دارایی پاداش")]
    [SBVR(SBVRModality.Permitted, "ردیابی استفاده از دارایی‌ها", "دارایی رویداد برای ردیابی استفاده از دارایی‌ها و تحلیل اثربخشی محصولات استفاده می‌شود")]
    public RewardAsset? Asset { get; set; }

    public int? ReferrerCodeId { get; set; }

    [DisplayName("کد معرف")]
    [SBVR(SBVRModality.Permitted, "ردیابی معرف", "کد معرف برای ردیابی معرفی‌های موفق و مدیریت پاداش‌های معرف استفاده می‌شود")]
    public ReferrerCode? ReferrerCode { get; set; }

    [DisplayName("ماه شمسی")]
    public ShamsiMonth? ShamsiMonth { get; set; }
}
