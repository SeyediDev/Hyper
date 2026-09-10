namespace Hyper.Domain.Entities.Plans;

/// <summary>
/// طرح فعال مشتری - رابطه بین مشتری و طرح‌های خریداری شده
/// </summary>
[DisplayName("طرح فعال مشتری")]
[SBVR(SBVRModality.Obligatory, "مدیریت طرح‌های مشتری", "هر خرید طرح باید برای محاسبه تخفیف، مدیریت اعتبار و تحلیل فروش قابل شناسایی باشد")]
[SBVR(SBVRModality.Recommended, "مدیریت طرح‌های مشتری", "طرح‌های فعال مشتری باید برای ارائه خدمات و محاسبه تخفیف‌ها قابل دسترسی سریع باشند")]
public class CustomerPlan : HyperBaseCoreAuditableEntity<int>
{
    /// <summary>
    /// شناسه مشتری
    /// </summary>
    [DisplayName("شناسه مشتری")]
    [SBVR(SBVRModality.Obligatory, "رابطه مشتری-طرح", "هر طرح فعال باید به یک مشتری مشخص تعلق داشته باشد")]
    public int CustomerTenantId { get; set; }

    /// <summary>
    /// مشتری
    /// </summary>
    [DisplayName("مشتری")]
    [SBVR(SBVRModality.Obligatory, "رابطه مشتری-طرح", "هر طرح فعال باید به یک مشتری مشخص تعلق داشته باشد")]
    public CustomerTenant CustomerTenant { get; set; } = null!;

    /// <summary>
    /// شناسه طرح
    /// </summary>
    [DisplayName("شناسه طرح")]
    [SBVR(SBVRModality.Obligatory, "رابطه طرح", "هر طرح فعال باید به یک طرح اشتراک مشخص اشاره کند")]
    public int PlanId { get; set; }

    /// <summary>
    /// طرح
    /// </summary>
    [DisplayName("طرح")]
    [SBVR(SBVRModality.Obligatory, "رابطه طرح", "هر طرح فعال باید به یک طرح اشتراک مشخص اشاره کند")]
    public Plan Plan { get; set; } = null!;

    /// <summary>
    /// تاریخ خرید
    /// </summary>
    [DisplayName("تاریخ خرید")]
    [SBVR(SBVRModality.Calculated, "تاریخ خرید", "تاریخ خرید به صورت خودکار هنگام ایجاد رکورد ثبت می‌شود")]
    public DateTime PurchaseDate { get; set; }

    /// <summary>
    /// تاریخ شروع اعتبار
    /// </summary>
    [DisplayName("تاریخ شروع اعتبار")]
    [SBVR(SBVRModality.Recommended, "مدیریت اعتبار", "تاریخ شروع برای محاسبه دوره اعتبار طرح استفاده می‌شود")]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// تاریخ پایان اعتبار
    /// </summary>
    [DisplayName("تاریخ پایان اعتبار")]
    [SBVR(SBVRModality.Calculated, "مدیریت اعتبار", "تاریخ پایان = تاریخ شروع + مدت اعتبار طرح")]
    public DateTime ExpiryDate { get; set; }

    /// <summary>
    /// وضعیت طرح
    /// </summary>
    [DisplayName("وضعیت")]
    [SBVR(SBVRModality.Calculated, "وضعیت طرح", "وضعیت بر اساس تاریخ اعتبار و فعال بودن محاسبه می‌شود")]
    public CustomerPlanStatus Status { get; set; }

    /// <summary>
    /// قیمت پرداختی (امتیاز)
    /// </summary>
    [DisplayName("قیمت پرداختی")]
    [SBVR(SBVRModality.Obligatory, "ثبت تراکنش", "قیمت پرداختی برای ثبت تراکنش و تحلیل فروش ضروری است")]
    public long PaidAmount { get; set; }

    /// <summary>
    /// شناسه تراکنش مشتری
    /// </summary>
    [DisplayName("شناسه تراکنش")]
    [SBVR(SBVRModality.Recommended, "رهگیری تراکنش", "شناسه تراکنش برای پیگیری پرداخت و حسابرسی مالی استفاده می‌شود")]
    public long? CustomerTransactionId { get; set; }

    /// <summary>
    /// تراکنش مشتری
    /// </summary>
    [DisplayName("تراکنش")]
    [SBVR(SBVRModality.Recommended, "رهگیری تراکنش", "تراکنش برای پیگیری پرداخت و حسابرسی مالی استفاده می‌شود")]
    public CustomerTransaction? CustomerTransaction { get; set; }

    /// <summary>
    /// تعداد استفاده از تخفیف
    /// </summary>
    [DisplayName("تعداد استفاده")]
    [SBVR(SBVRModality.Calculated, "تحلیل استفاده", "تعداد استفاده برای تحلیل میزان بهره‌برداری از طرح استفاده می‌شود")]
    public int UsageCount { get; set; } = 0;

    /// <summary>
    /// مجموع تخفیف دریافتی (امتیاز)
    /// </summary>
    [DisplayName("مجموع تخفیف")]
    [SBVR(SBVRModality.Calculated, "تحلیل سودآوری", "مجموع تخفیف برای تحلیل بازگشت سرمایه مشتری و ارزش طرح استفاده می‌شود")]
    public long TotalDiscountReceived { get; set; } = 0;

    /// <summary>
    /// فعال/غیرفعال دستی
    /// </summary>
    [DisplayName("فعال")]
    [SBVR(SBVRModality.Recommended, "مدیریت دستی", "امکان غیرفعال کردن دستی طرح توسط مدیر یا پشتیبانی")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// یادداشت
    /// </summary>
    [DisplayName("یادداشت")]
    [MaxLength(500)]
    [SBVR(SBVRModality.Permitted, "مستندسازی", "یادداشت برای ثبت توضیحات اضافی درباره طرح استفاده می‌شود")]
    public string? Notes { get; set; }

    /// <summary>
    /// آیا این طرح معتبر و قابل استفاده است؟
    /// </summary>
    public bool IsValid()
    {
        return IsActive && 
               Status == CustomerPlanStatus.Active && 
               DateTime.UtcNow >= StartDate && 
               DateTime.UtcNow <= ExpiryDate;
    }
}

/// <summary>
/// وضعیت طرح مشتری
/// </summary>
public enum CustomerPlanStatus
{
    /// <summary>
    /// فعال - طرح در حال استفاده است
    /// </summary>
    [Display(Name = "فعال")]
    Active = 1,

    /// <summary>
    /// منقضی شده - اعتبار طرح تمام شده است
    /// </summary>
    [Display(Name = "منقضی شده")]
    Expired = 2,

    /// <summary>
    /// لغو شده - طرح توسط مدیر یا مشتری لغو شده است
    /// </summary>
    [Display(Name = "لغو شده")]
    Cancelled = 3,

    /// <summary>
    /// معلق - طرح به صورت موقت غیرفعال شده است
    /// </summary>
    [Display(Name = "معلق")]
    Suspended = 4
}

