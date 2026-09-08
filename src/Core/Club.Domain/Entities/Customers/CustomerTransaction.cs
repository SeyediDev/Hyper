namespace Hyper.Domain.Entities.Customers;

/// <summary>
/// می خواهیم تراکنش امتیازات های هر مشتری را ذخیره کنیم
/// به ازای هر تغییر یک رکورد جدید اضافه می شود
/// به ازای بازدید کاربر از امتیازاتش فیلد VisitedAt پر میشود
/// </summary>
[EntityIndex($"{nameof(CustomerTenantId)},{nameof(VisitedAt)},{nameof(IsDeleted)}")]
[DisplayName("تراکنش مشتری")]
public class CustomerTransaction : HyperBaseCoreAuditableEntity<long>
{
    public int CustomerTenantId { get; set; }
    [DisplayName("مشتری اکوسیستم")]
    [InDisplayString]
    public CustomerTenant CustomerTenant { get; set; } = null!;

    public int PointId { get; set; }
    [DisplayName("امتیاز")]
    public Point Point { get; set; } = null!;

    [DisplayName("بدهکار")]
    [SBVR(SBVRModality.Obligatory, "ثبت کاهش امتیاز", "بدهکار برای ثبت تمام کاهش‌های امتیاز مشتری ضروری است")]
    [SBVR(SBVRModality.Recommended, "ثبت کاهش امتیاز", "بدهکار برای ردیابی استفاده از امتیازات و تحلیل رفتار مشتری استفاده می‌شود")]
    public long? Debit { get; set; }

    [DisplayName("بستانکار")]
    [SBVR(SBVRModality.Obligatory, "ثبت افزایش امتیاز", "بستانکار برای ثبت تمام افزایش‌های امتیاز مشتری ضروری است")]
    [SBVR(SBVRModality.Recommended, "ثبت افزایش امتیاز", "بستانکار برای ردیابی کسب امتیازات و محاسبه Loyalty Score استفاده می‌شود")]
    public long? Credit { get; set; }

    [DisplayName("مانده")]
    [SBVR(SBVRModality.Obligatory, "مانده حساب", "مانده برای نمایش امتیازات قابل استفاده مشتری ضروری است")]
    [SBVR(SBVRModality.Calculated, "مانده حساب", "مانده = مانده قبلی + بستانکار - بدهکار")]
    public long Balance { get; set; }

    [DisplayName("نوع عملیات")]
    [SBVR(SBVRModality.Obligatory, "دسته‌بندی تراکنش", "نوع عملیات برای تفکیک منابع تراکنش و تحلیل رفتار ضروری است")]
    [SBVR(SBVRModality.Recommended, "دسته‌بندی تراکنش", "نوع عملیات برای گروه‌بندی تراکنش‌ها و تحلیل الگوهای استفاده از امتیاز استفاده می‌شود")]
    public CustomerTransactionType TransactionType { get; set; }

    [DisplayName("تاریخ بازدید")]
    [SBVR(SBVRModality.Calculated, "ردیابی تعامل", "تاریخ بازدید پس از مشاهده امتیاز توسط مشتری ثبت می‌شود")]
    public DateTime? VisitedAt { get; set; }

    public long EventLogId { get; set; }
    [DisplayName("لاگ رویداد")]
    public EventLog EventLog { get; set; } = null!;

    public int? EventChannelId { get; set; }
    [DisplayName("کانال تراکنش")]
    public EventChannel? EventChannel { get; set; }

    public int? ActivePlanId { get; set; }
    [DisplayName("طرح فعال در لحظه تراکنش")]
    public Plan? ActivePlan { get; set; }

    public int? RewardId { get; set; }
    [DisplayName("ریوارد")]
    public Reward? Reward { get; set; }

    public int? PromotionId { get; set; }
    [DisplayName("پویش")]
    public Promotion? Promotion { get; set; }

    public int? PromotionActionId { get; set; }
    [DisplayName("عملیات پویش")]
    public PromotionAction? PromotionAction { get; set; }

    /// <summary>
    /// تاریخ انقضای این تراکنش امتیازی (فقط برای تراکنش‌های Credit)
    /// </summary>
    [DisplayName("تاریخ انقضا")]
    [SBVR(SBVRModality.Permitted, "مدیریت اعتبار امتیاز", "تاریخ انقضای این تراکنش امتیازی. این فیلد فقط برای تراکنش‌های Credit (افزایش امتیاز) معنی دارد و در زمان دریافت امتیاز محاسبه می‌شود.")]
    public DateTime? ExpirationDate { get; set; }

    /// <summary>
    /// آیا این تراکنش منقضی شده است؟
    /// </summary>
    [DisplayName("منقضی شده")]
    [SBVR(SBVRModality.Calculated, "مدیریت اعتبار امتیاز", "این فیلد نشان می‌دهد که آیا این تراکنش امتیازی منقضی شده است یا خیر. یک تراکنش منقضی می‌شود اگر ExpirationDate <= Today و IsSpent = false باشد.")]
    public bool IsExpired { get; set; } = false;

    /// <summary>
    /// تاریخ منقضی شدن (اگر منقضی شده باشد)
    /// </summary>
    [DisplayName("تاریخ منقضی شدن")]
    [SBVR(SBVRModality.Permitted, "مدیریت اعتبار امتیاز", "تاریخ منقضی شدن این تراکنش امتیازی. این فیلد زمانی پر می‌شود که تراکنش منقضی می‌شود.")]
    public DateTime? ExpiredDate { get; set; }

    /// <summary>
    /// آیا این امتیاز خرج شده است؟
    /// </summary>
    [DisplayName("خرج شده")]
    [SBVR(SBVRModality.Calculated, "مدیریت اعتبار امتیاز", "این فیلد نشان می‌دهد که آیا این امتیاز خرج شده است یا خیر. یک امتیاز خرج شده نمی‌تواند منقضی شود.")]
    public bool IsSpent { get; set; } = false;
}
