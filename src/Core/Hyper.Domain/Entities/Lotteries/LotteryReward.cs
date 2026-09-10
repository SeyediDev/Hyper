namespace Hyper.Domain.Entities.Lotteries;

/// <summary>
/// پاداش‌های قرعه‌کشی - فهرست پاداش‌های یک قرعه‌کشی با نرخ و درصد برنده شدن
/// برای هر پاداش در قرعه‌کشی می‌توان نرخ برنده شدن، درصد، سقف تعداد ارائه و تعداد ارائه شده را تعیین کرد
/// </summary>
[DisplayName("پاداش قرعه‌کشی")]
[SBVR(SBVRModality.Obligatory, "مدیریت پاداش‌های قرعه‌کشی", "هر قرعه‌کشی باید فهرستی از پاداش‌ها داشته باشد تا تعیین کند چه پاداش‌هایی می‌توانند به برندگان اهدا شوند")]
[SBVR(SBVRModality.Recommended, "مدیریت پاداش‌های قرعه‌کشی", "پاداش‌های قرعه‌کشی باید برای مدیریت نرخ برنده شدن، کنترل تعداد پاداش‌ها و تحلیل اثربخشی قرعه‌کشی تنظیم شوند")]
public class LotteryReward : HyperBaseCoreConfigAuditableEntity<int>
{
    public int LotteryId { get; set; }
    [DisplayName("قرعه‌کشی")]
    [SBVR(SBVRModality.Obligatory, "رابطه پاداش-قرعه‌کشی", "هر پاداش باید به یک قرعه‌کشی مشخص تعلق داشته باشد")]
    public Lottery Lottery { get; set; } = null!;

    public int RewardId { get; set; }
    [DisplayName("پاداش")]
    [SBVR(SBVRModality.Obligatory, "رابطه پاداش-قرعه‌کشی", "هر پاداش باید برای تعیین نوع پاداشی که به برنده اهدا می‌شود تعیین شود")]
    [SBVR(SBVRModality.Recommended, "رابطه پاداش-قرعه‌کشی", "رابطه پاداش-قرعه‌کشی برای مدیریت موجودی، تحلیل ترجیحات مشتریان و بهینه‌سازی ارزش پاداش‌ها استفاده می‌شود")]
    public Reward Reward { get; set; } = null!;

    /// <summary>
    /// عنوان بخش چرخونه که در UI نمایش داده می‌شود
    /// </summary>
    [DisplayName("عنوان بخش چرخونه")]
    [MaxLength(64)]
    public string SegmentLabel { get; set; } = null!;

    /// <summary>
    /// توضیح یا پیام این بخش برای نمایش بعد از توقف چرخش
    /// </summary>
    [DisplayName("پیام بخش")]
    [MaxLength(256)]
    public string? SegmentMessage { get; set; }

    /// <summary>
    /// رنگ پس‌زمینه بخش چرخونه به صورت HEX
    /// </summary>
    [DisplayName("رنگ بخش")]
    [MaxLength(16)]
    public string SegmentColor { get; set; } = "#f97316";

    /// <summary>
    /// رنگ متن داخل بخش
    /// </summary>
    [DisplayName("رنگ متن بخش")]
    [MaxLength(16)]
    public string SegmentTextColor { get; set; } = "#ffffff";

    /// <summary>
    /// آیکون نمایشی بخش
    /// </summary>
    [DisplayName("نماد بخش")]
    [MaxLength(16)]
    public string? SegmentIcon { get; set; }

    /// <summary>
    /// آیا این بخش به عنوان جایزه ویژه نمایش داده شود
    /// </summary>
    [DisplayName("جایزه ویژه")]
    public bool IsJackpot { get; set; }

    /// <summary>
    /// میزان پاداش - تعداد پاداش‌هایی که به هر برنده اهدا می‌شود
    /// </summary>
    [DisplayName("میزان پاداش")]
    [SBVR(SBVRModality.Obligatory, "مقدار پاداش", "مقدار پاداش برای تعیین تعداد پاداش‌هایی که به هر برنده اهدا می‌شود ضروری است")]
    [SBVR(SBVRModality.Calculated, "مقدار پاداش", "مقدار پاداش بر اساس ارزش قرعه‌کشی، بودجه و استراتژی بازاریابی تنظیم می‌شود")]
    public int Amount { get; set; } = 1;

    /// <summary>
    /// نرخ برنده شدن - نرخ برنده شدن این پاداش در قرعه‌کشی (به ازای 10000 یا 1.0000)
    /// مثال: 1000 یعنی 10% شانس برنده شدن
    /// </summary>
    [DisplayName("نرخ برنده شدن (در 10000)")]
    [SBVR(SBVRModality.Obligatory, "نرخ برنده شدن", "نرخ برنده شدن برای تعیین احتمال اهدای این پاداش در قرعه‌کشی ضروری است")]
    [SBVR(SBVRModality.Recommended, "نرخ برنده شدن", "نرخ برنده شدن برای تعادل بین ارزش پاداش و هزینه قرعه‌کشی استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نرخ برنده شدن", "نرخ برنده شدن = (تعداد کل پاداش این نوع / تعداد کل قرعه‌کشی‌ها × 10000)")]
    public int WinRate { get; set; }

    /// <summary>
    /// درصد برنده شدن - درصد شانس برنده شدن این پاداش (0-100)
    /// </summary>
    [DisplayName("درصد برنده شدن (%)")]
    [SBVR(SBVRModality.Calculated, "درصد برنده شدن", "درصد برنده شدن = نرخ برنده شدن / 100")]
    [SBVR(SBVRModality.Recommended, "درصد برنده شدن", "درصد برنده شدن برای نمایش شفاف به مشتریان و مدیریت انتظارات استفاده می‌شود")]
    public decimal WinPercentage => (decimal)WinRate / 100;

    /// <summary>
    /// سقف تعداد ارائه - حداکثر تعداد دفعاتی که این پاداش می‌تواند در این قرعه‌کشی اهدا شود
    /// </summary>
    [DisplayName("سقف تعداد ارائه")]
    [SBVR(SBVRModality.Obligatory, "محدودیت تعداد پاداش", "سقف تعداد ارائه برای کنترل بودجه قرعه‌کشی و جلوگیری از اهدای بیش از حد یک پاداش ضروری است")]
    [SBVR(SBVRModality.Recommended, "محدودیت تعداد پاداش", "سقف تعداد ارائه برای مدیریت موجودی، کنترل هزینه‌ها و تضمین عدالت در توزیع پاداش‌ها استفاده می‌شود")]
    public int? MaxDistributionCount { get; set; }

    /// <summary>
    /// تعداد ارائه شده - تعداد دفعاتی که این پاداش تا کنون اهدا شده است
    /// </summary>
    [DisplayName("تعداد ارائه شده")]
    [SBVR(SBVRModality.Calculated, "ردیابی توزیع پاداش", "تعداد ارائه شده برای ردیابی استفاده از پاداش‌ها و اطمینان از عدم تجاوز از سقف استفاده می‌شود")]
    [SBVR(SBVRModality.Recommended, "ردیابی توزیع پاداش", "تعداد ارائه شده برای تحلیل اثربخشی قرعه‌کشی و بهینه‌سازی ترکیب پاداش‌ها استفاده می‌شود")]
    public int DistributedCount { get; set; }

    /// <summary>
    /// ترتیب نمایش - ترتیب نمایش این پاداش در فهرست (برای UI)
    /// </summary>
    [DisplayName("ترتیب نمایش")]
    [SBVR(SBVRModality.Recommended, "مدیریت UI", "ترتیب نمایش برای سازماندهی بصری پاداش‌ها در رابط کاربری استفاده می‌شود")]
    public int DisplayOrder { get; set; }

    /// <summary>
    /// وضعیت فعال - آیا این پاداش در قرعه‌کشی فعال است
    /// </summary>
    [DisplayName("فعال")]
    [SBVR(SBVRModality.Permitted, "مدیریت پاداش", "وضعیت فعال برای کنترل اینکه آیا پاداش در قرعه‌کشی در نظر گرفته می‌شود استفاده می‌شود")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// آیا هنوز می‌توان از این پاداش توزیع کرد
    /// </summary>
    [DisplayName("قابل توزیع")]
    [SBVR(SBVRModality.Calculated, "مدیریت موجودی پاداش", "قابل توزیع برای بررسی اینکه آیا هنوز می‌توان این پاداش را اهدا کرد استفاده می‌شود")]
    public bool CanDistribute => IsActive && (!MaxDistributionCount.HasValue || DistributedCount < MaxDistributionCount.Value);
}