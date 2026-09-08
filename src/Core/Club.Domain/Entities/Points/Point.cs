namespace Hyper.Domain.Entities.Points;

[DisplayName("امتیاز")]
[SBVR(SBVRModality.Obligatory, "سیستم امتیازدهی", "هر امتیاز باید برای تشویق رفتارهای مطلوب مشتریان و محاسبه تراکنش‌ها قابل شناسایی باشد")]
[SBVR(SBVRModality.Recommended, "سیستم امتیازدهی", "امتیازات باید برای افزایش وفاداری مشتریان و تحلیل اثربخشی کمپین‌ها طراحی شوند")]
public class Point : HyperBaseCoreConfigAuditableEntity<int>, ISubOfTenant
{
    public int TenantId { get; set; }
    [DisplayName("اکوسیستم")]
    [SBVR(SBVRModality.Obligatory, "چندین اکوسیستم", "هر امتیاز باید به یک اکوسیستم مشخص تعلق داشته باشد تا از تداخل قوانین امتیازدهی جلوگیری شود")]
    public Tenant Tenant { get; set; } = null!;

    /// <summary>
    /// عنوان امتیاز
    /// </summary>
    [DisplayName("عنوان امتیاز")]
    [InDisplayString]
    [MaxLength(41)]
    [SBVR(SBVRModality.Obligatory, "شناسایی امتیاز", "عنوان امتیاز باید برای نمایش در تراکنش‌ها و گزارش‌ها واضح و قابل فهم باشد")]
    public string Title { get; set; } = null!;

    [DisplayName("کلید")]
    [MaxLength(41)]
    [SBVR(SBVRModality.Permitted, "شناسایی امتیاز", "قابل استفاده در فرمول نویسی ها" +
@"
| Pattern                     | Meaning                                | Example                      | Output                |
|-----------------------------|----------------------------------------|------------------------------|-----------------------|
| CustomerPoint.{Key}         | موجودی مشتری در امتیاز با این کلید    | CustomerPoint.Momtaz         | 125                   |
| CustomerPoint.{Key}.Level   | کلید سطح مشتری در امتیاز با این کلید  | CustomerPoint.Momtaz.Level   | Platinium             |
| CustomerPoint.{Key}.LevelId | سطح، سطح مشتری در امتیاز با این کلید  | CustomerPoint.Momtaz.LevelId | 3                     |")]
    public string Key { get; set; } = null!;


    /// <summary>
    /// نوع امتیاز
    /// </summary>
    [DisplayName("نوع امتیاز")]
    [SBVR(SBVRModality.Obligatory, "دسته‌بندی امتیازات", "نوع امتیاز تعیین می‌کند که امتیاز برای چه نوع فعالیتی اعطا می‌شود")]
    public PointType PointType { get; set; }

    /// <summary>
    /// بازدید خودکار
    /// </summary>
    [DisplayName("بازدید خودکار")]
    [SBVR(SBVRModality.Permitted, "افزایش سر زدن به صفحه باشگاه", "بعضی از امتیازها لازم است کاربر حتما به صفحه باشگاه مشتریان سر بزند و بگوید که این امتیاز را دیده تا به صورت واقعی آن را کسب کند.")]
    public bool? AutoVisit { get; set; }

    /// <summary>
    /// قابل مشاهده
    /// </summary>
    [DisplayName("قابل مشاهده")]
    [SBVR(SBVRModality.Recommended, "شفافیت امتیازدهی", "قابل مشاهده تعیین می‌کند که آیا امتیاز برای مشتریان قابل مشاهده باشد تا اعتماد افزایش یابد")]
    public bool? Visible { get; set; }

    /// <summary>
    /// نمایش در لیدربورد
    /// </summary>
    [DisplayName("نمایش در لیدربورد")]
    [SBVR(SBVRModality.Recommended, "مدیریت لیدربورد", "این فیلد مشخص می‌کند که آیا امتیاز در لیدربورد نمایش داده شود تا از نمایش امتیازات نامرتبط جلوگیری شود")]
    public bool? ShowInLeaderboard { get; set; }

    /// <summary>
    /// قابل انتقال
    /// </summary>
    [DisplayName("قابل انتقال")]
    [SBVR(SBVRModality.Recommended, "امکانات امتیازدهی", "قابل انتقال تعیین می‌کند که آیا امتیاز مشتریان قابل انتقال به دیگر مشتریان هست ؟")]
    public bool? Transferable { get; set; }

    /// <summary>
    /// آیا این امتیاز تاریخ اعتبار دارد؟
    /// </summary>
    [DisplayName("دارای تاریخ اعتبار")]
    [SBVR(SBVRModality.Permitted, "مدیریت اعتبار امتیاز", "این فیلد تعیین می‌کند که آیا امتیازات دریافت شده از این نوع امتیاز تاریخ اعتبار دارند یا نه. اگر true باشد، باید مدت روز اعتبار (ExpirationDays) مشخص شود.")]
    public bool HasExpiration { get; set; } = false;

    /// <summary>
    /// مدت روز اعتبار (فقط اگر HasExpiration = true)
    /// </summary>
    [DisplayName("مدت روز اعتبار")]
    [SBVR(SBVRModality.Permitted, "مدیریت اعتبار امتیاز", "مدت روز اعتبار تعیین می‌کند که امتیازات دریافت شده از این نوع امتیاز چند روز پس از دریافت منقضی می‌شوند. این فیلد فقط زمانی معنی دارد که HasExpiration = true باشد.")]
    public int? ExpirationDays { get; set; }

    [DisplayName("اجازه منفی شدن موجودی")]
    public bool AllowNegativeBalance { get; set; } = false;
}
