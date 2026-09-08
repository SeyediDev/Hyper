using PlanDiscountType = Hyper.Domain.Entities.Plans.Enums.PlanDiscountType;

namespace Hyper.Domain.Entities.Plans;

/// <summary>
/// طرح اشتراک - طرح‌های اشتراکی که مشتریان می‌توانند خریداری کنند
/// با خرید طرح، مشتریان می‌توانند از تخفیف در خرید ریوارد‌ها بهره‌مند شوند
/// </summary>
[DisplayName("طرح اشتراک")]
[SBVR(SBVRModality.Obligatory, "مدیریت طرح‌های اشتراک", "هر طرح باید برای فروش، محاسبه تخفیف و تحلیل سودآوری قابل شناسایی باشد")]
[SBVR(SBVRModality.Recommended, "مدیریت طرح‌های اشتراک", "طرح‌ها باید برای تحلیل اثربخشی، افزایش وفاداری مشتری و بهینه‌سازی درآمد سازماندهی شوند")]
public class Plan : HyperBaseCoreAuditableEntity<int>
{
    /// <summary>
    /// شناسه اکوسیستم
    /// </summary>
    [DisplayName("شناسه اکوسیستم")]
    [SBVR(SBVRModality.Obligatory, "چندین اکوسیستم", "هر طرح باید به یک اکوسیستم مشخص تعلق داشته باشد تا از تداخل داده‌ها جلوگیری شود")]
    public int TenantId { get; set; }

    /// <summary>
    /// اکوسیستم
    /// </summary>
    [DisplayName("اکوسیستم")]
    [SBVR(SBVRModality.Obligatory, "چندین اکوسیستم", "هر طرح باید به یک اکوسیستم مشخص تعلق داشته باشد تا از تداخل داده‌ها جلوگیری شود")]
    public Tenant Tenant { get; set; } = null!;

    /// <summary>
    /// عنوان طرح
    /// </summary>
    [DisplayName("عنوان طرح")]
    [InDisplayString]
    [MaxLength(100)]
    [SBVR(SBVRModality.Obligatory, "شناسایی طرح", "عنوان طرح باید برای نمایش در پرتال مشتریان واضح و قابل فهم باشد")]
    public string Title { get; set; } = null!;

    /// <summary>
    /// توضیحات طرح
    /// </summary>
    [DisplayName("توضیحات")]
    [MaxLength(1000)]
    [SBVR(SBVRModality.Recommended, "اطلاع‌رسانی", "توضیحات برای توضیح مزایای طرح به مشتریان ضروری است")]
    public string? Description { get; set; }

    /// <summary>
    /// قیمت طرح به امتیاز
    /// </summary>
    [DisplayName("قیمت به امتیاز")]
    [SBVR(SBVRModality.Obligatory, "محاسبه قیمت", "قیمت طرح به امتیاز برای خرید طرح توسط مشتری ضروری است")]
    public long PriceInPoints { get; set; }

    /// <summary>
    /// شناسه نوع امتیاز
    /// </summary>
    [DisplayName("شناسه نوع امتیاز")]
    [SBVR(SBVRModality.Obligatory, "نوع امتیاز", "هر طرح باید برای یک نوع امتیاز مشخص تعریف شود")]
    public int PointId { get; set; }

    /// <summary>
    /// نوع امتیاز
    /// </summary>
    [DisplayName("نوع امتیاز")]
    [SBVR(SBVRModality.Obligatory, "نوع امتیاز", "هر طرح باید برای یک نوع امتیاز مشخص تعریف شود")]
    public Point Point { get; set; } = null!;

    /// <summary>
    /// مدت اعتبار طرح به روز
    /// </summary>
    [DisplayName("مدت اعتبار (روز)")]
    [SBVR(SBVRModality.Obligatory, "مدیریت اعتبار", "مدت اعتبار برای تعیین تاریخ انقضای طرح ضروری است")]
    public int ValidityDays { get; set; }

    /// <summary>
    /// نوع تخفیف - مقداری یا درصدی
    /// </summary>
    [DisplayName("نوع تخفیف")]
    [SBVR(SBVRModality.Obligatory, "محاسبه تخفیف", "نوع تخفیف برای محاسبه صحیح قیمت ریوارد‌ها ضروری است")]
    public PlanDiscountType DiscountType { get; set; }

    /// <summary>
    /// مقدار تخفیف
    /// درصدی: عدد بین 0 تا 100
    /// مقداری: مقدار تخفیف به امتیاز
    /// </summary>
    [DisplayName("مقدار تخفیف")]
    [SBVR(SBVRModality.Obligatory, "محاسبه تخفیف", "مقدار تخفیف برای محاسبه قیمت نهایی ریوارد‌ها ضروری است")]
    public decimal DiscountValue { get; set; }

    /// <summary>
    /// شناسه جامعه مشتریان - برای محدود کردن دسترسی طرح به گروه خاص
    /// </summary>
    [DisplayName("شناسه جامعه مشتریان")]
    [SBVR(SBVRModality.Permitted, "دسترسی محدود", "جامعه مشتریان برای محدود کردن دسترسی طرح به گروه‌های خاص مشتریان استفاده می‌شود")]
    public int? CustomerSegmentId { get; set; }

    /// <summary>
    /// جامعه مشتریان
    /// </summary>
    [DisplayName("جامعه مشتریان")]
    [SBVR(SBVRModality.Permitted, "دسترسی محدود", "جامعه مشتریان برای محدود کردن دسترسی طرح به گروه‌های خاص مشتریان استفاده می‌شود")]
    public CustomerSegment? CustomerSegment { get; set; }

    /// <summary>
    /// فعال/غیرفعال
    /// </summary>
    [DisplayName("فعال")]
    [SBVR(SBVRModality.Recommended, "مدیریت طرح‌ها", "وضعیت فعال بودن برای کنترل نمایش و فروش طرح استفاده می‌شود")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// آیا این طرح به تمام ریوارد‌ها تخفیف می‌دهد؟
    /// اگر true باشد به همه ریوارد‌ها تخفیف اعمال می‌شود
    /// اگر false باشد فقط به ریوارد‌هایی که در RewardCost این طرح را دارند تخفیف اعمال می‌شود
    /// </summary>
    [DisplayName("تخفیف عمومی")]
    [SBVR(SBVRModality.Recommended, "محاسبه تخفیف", "تخفیف عمومی تعیین می‌کند که آیا تخفیف به تمام ریوارد‌ها یا فقط ریوارد‌های خاص اعمال شود")]
    public bool IsGlobalDiscount { get; set; } = false;

    /// <summary>
    /// شناسه سفارش - برای مرتب‌سازی طرح‌ها
    /// </summary>
    [DisplayName("ترتیب نمایش طرح")]
    [SBVR(SBVRModality.Permitted, "مرتب‌سازی", "شناسه سفارش برای مرتب‌سازی و اولویت‌بندی نمایش طرح‌ها استفاده می‌شود")]
    public int? OrderId { get; set; }

    public int? PictureId { get; set; }

    /// <summary>
    /// تصویر طرح
    /// </summary>
    [DisplayName("تصویر طرح")]
    [SBVR(SBVRModality.Permitted, "نمایش بصری", "تصویر طرح برای بهبود تجربه کاربری و افزایش نرخ تبدیل استفاده می‌شود")]
    public Document? Picture { get; set; }

    // Navigation Properties
    public ICollection<CustomerPlan> CustomerPlans { get; set; } = [];
    public ICollection<RewardCost> RewardCosts { get; set; } = [];
}
