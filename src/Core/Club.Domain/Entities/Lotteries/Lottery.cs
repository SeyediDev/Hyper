using Hyper.Domain.Entities.Lotteries.Enums;

namespace Hyper.Domain.Entities.Lotteries;

/// <summary>
/// قرعه‌کشی - سیستم قرعه‌کشی برای اهدای پاداش‌ها به مشتریان
/// قرعه‌کشی می‌تواند به صورت چرخونه (کاربر درخواست می‌دهد) یا زمان‌بندی شده (خودکار در زمان مشخص) باشد
/// </summary>
[DisplayName("قرعه‌کشی/چرخونه")]
[SBVR(SBVRModality.Obligatory, "مدیریت قرعه‌کشی", "هر قرعه‌کشی باید برای تعیین پاداش‌ها، نرخ برنده شدن و مدیریت شرکت‌کنندگان قابل شناسایی باشد")]
[SBVR(SBVRModality.Recommended, "مدیریت قرعه‌کشی", "قرعه‌کشی‌ها باید برای افزایش تعامل مشتریان، تشویق خرید و رقابت مثبت استفاده شوند")]
[SBVR(SBVRModality.Permitted, "انواع قرعه‌کشی", "قرعه‌کشی‌ها می‌توانند چرخونه (فوری) یا زمان‌بندی شده (خودکار) باشند")]
public class Lottery : HyperBaseCoreConfigAuditableEntity<int>, ISubOfPromotion
{
    public int PromotionId { get; set; }
    [DisplayName("پویش")]
    public Promotion Promotion { get; set; } = null!;

    /// <summary>
    /// عنوان قرعه‌کشی
    /// </summary>
    [DisplayName("عنوان")]
    [InDisplayString]
    [MaxLength(41)]
    [SBVR(SBVRModality.Obligatory, "شناسایی قرعه‌کشی", "عنوان برای نمایش به مشتریان و مدیریت قرعه‌کشی‌ها ضروری است")]
    [SBVR(SBVRModality.Recommended, "شناسایی قرعه‌کشی", "عنوان باید واضح و جذاب باشد تا مشتریان را به شرکت ترغیب کند")]
    public string Title { get; set; } = null!;

    /// <summary>
    /// وضعیت فعال/غیرفعال بودن قرعه‌کشی
    /// </summary>
    [DisplayName("فعال")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// نوع قرعه‌کشی: چرخونه یا زمان‌بندی شده
    /// </summary>
    [DisplayName("نوع قرعه‌کشی")]
    [SBVR(SBVRModality.Obligatory, "نوع قرعه‌کشی", "نوع قرعه‌کشی تعیین می‌کند که آیا چرخونه انجام می‌شود یا زمان‌بندی شده")]
    public LotteryType LotteryType { get; set; } = LotteryType.Wheel;

    /// <summary>
    /// توضیحات کامل قرعه‌کشی برای نمایش در UI
    /// </summary>
    [DisplayName("توضیحات")]
    [MaxLength(512)]
    public string? Description { get; set; }

    /// <summary>
    /// تم ظاهری چرخونه
    /// </summary>
    [DisplayName("تم چرخونه")]
    [MaxLength(64)]
    public string WheelTheme { get; set; } = "sunset";

    /// <summary>
    /// زیرعنوان یا پیام احساسی زیر عنوان اصلی
    /// </summary>
    [DisplayName("زیرعنوان")]
    [MaxLength(256)]
    public string? WheelSubtitle { get; set; }

    /// <summary>
    /// متن دکمه اقدام برای شروع چرخش
    /// </summary>
    [DisplayName("متن دکمه چرخش")]
    [MaxLength(64)]
    public string WheelButtonLabel { get; set; } = "شروع چرخش";

    /// <summary>
    /// پیام تشویقی برای نمایش هنگام انتظار چرخش
    /// </summary>
    [DisplayName("پیام تشویقی")]
    [MaxLength(256)]
    public string? WheelCallToAction { get; set; }

    /// <summary>
    /// پیام جشن هنگام برنده شدن
    /// </summary>
    [DisplayName("پیام جشن برنده شدن")]
    [MaxLength(512)]
    public string? WheelCelebrationMessage { get; set; }

    /// <summary>
    /// رنگ پس‌زمینه چرخونه در UI
    /// </summary>
    [DisplayName("رنگ پس‌زمینه")]
    [MaxLength(16)]
    public string WheelBackgroundColor { get; set; } = "#FDF2F8";

    /// <summary>
    /// آیکون مرکزی نمایش داده شده در چرخونه
    /// </summary>
    [DisplayName("نماد مرکزی")]
    [MaxLength(16)]
    public string? WheelCenterIcon { get; set; } = "🎉";

    /// <summary>
    /// مدت زمان چرخش در ثانیه برای ایجاد حس واقعی
    /// </summary>
    [DisplayName("مدت چرخش (ثانیه)")]
    [Range(2, 20)]
    public int SpinDurationSeconds { get; set; } = 6;

    /// <summary>
    /// حداکثر تعداد چرخش مجاز در هر روز برای یک مشتری
    /// </summary>
    [DisplayName("حداکثر چرخش روزانه هر مشتری")]
    [Range(1, 50)]
    public int MaxDailySpins { get; set; } = 3;

    /// <summary>
    /// حداکثر تعداد کل چرخش‌های مجاز برای هر مشتری
    /// </summary>
    [DisplayName("حداکثر چرخش کل هر مشتری")]
    public int? MaxTotalSpins { get; set; }

    /// <summary>
    /// آیا شرکت در قرعه‌کشی نیاز به پرداخت دارد
    /// </summary>
    [DisplayName("نیاز به پرداخت")]
    [SBVR(SBVRModality.Permitted, "پرداخت برای شرکت", "قرعه‌کشی می‌تواند رایگان باشد یا نیاز به پرداخت امتیاز داشته باشد")]
    public bool RequiresPayment { get; set; } = false;

    /// <summary>
    /// شناسه نوع امتیاز برای پرداخت (اگر RequiresPayment = true)
    /// </summary>
    [DisplayName("شناسه نوع امتیاز")]
    [SBVR(SBVRModality.Necessary, "نوع امتیاز", "باید تعیین شود", "وقتی نیاز به پرداخت فعال باشد")]
    public int? PaymentPointId { get; set; }

    /// <summary>
    /// مبلغ پرداخت (امتیاز) برای شرکت در قرعه‌کشی
    /// </summary>
    [DisplayName("مبلغ پرداخت")]
    [SBVR(SBVRModality.Necessary, "مبلغ پرداخت", "باید تعیین شود", "وقتی نیاز به پرداخت فعال باشد")]
    [Range(1, long.MaxValue, ErrorMessage = "مبلغ پرداخت باید بیشتر از صفر باشد")]
    public long? PaymentAmount { get; set; }

    /// <summary>
    /// تعداد برندگان - برای قرعه‌کشی‌های زمان‌بندی شده (LotteryType=Scheduled)
    /// تعیین می‌کند که چند نفر می‌توانند برنده شوند
    /// </summary>
    [DisplayName("تعداد برندگان")]
    [SBVR(SBVRModality.Permitted, "تعداد برندگان", "تعداد برندگان برای قرعه‌کشی‌های زمان‌بندی شده - تعیین می‌کند که چند نفر می‌توانند برنده شوند")]
    [SBVR(SBVRModality.Necessary, "تعداد برندگان", "باید تعیین شود", "وقتی نوع قرعه‌کشی 'زمان‌بندی شده' باشد")]
    [Range(1, int.MaxValue, ErrorMessage = "تعداد برندگان باید بیشتر از صفر باشد")]
    public int? WinnerCount { get; set; }

    [DisplayName("از تاریخ")]
    [SBVR(SBVRModality.Recommended, "تاریخ شروع", "تاریخ شروع برای قرعه‌کشی‌های زمان‌بندی شده استفاده می‌شود")]
    public DateTime? FromDate { get; set; }

    [DisplayName("تا تاریخ")]
    [SBVR(SBVRModality.Recommended, "تاریخ پایان", "تاریخ پایان برای قرعه‌کشی‌های زمان‌بندی شده استفاده می‌شود")]
    public DateTime? ToDate { get; set; }

    // ===== SCHEDULING FIELDS =====
    
    /// <summary>
    /// آیا برنامه‌ریزی فعال است
    /// </summary>
    [DisplayName("فعال‌سازی برنامه‌ریزی")]
    [SBVR(SBVRModality.Permitted, "برنامه‌ریزی زمان‌بندی", "برای قرعه‌کشی‌های زمان‌بندی شده استفاده می‌شود")]
    public bool IsScheduled { get; set; }

    /// <summary>
    /// نوع پنجره زمانی برنامه‌ریزی
    /// </summary>
    [DisplayName("نوع برنامه‌ریزی")]
    [SBVR(SBVRModality.Necessary, "نوع برنامه‌ریزی", "باید تعیین شود", "وقتی برنامه‌ریزی فعال باشد")]
    public SchedulingKind? SchedulingKind { get; set; }

    /// <summary>
    /// روز هفته برای برنامه‌ریزی (1=شنبه تا 7=جمعه)
    /// </summary>
    [DisplayName("روز هفته")]
    [SBVR(SBVRModality.Necessary, "روز هفته", "باید تعیین شود", "وقتی نوع برنامه‌ریزی 'روز هفته' باشد")]
    public int? DayOfWeek { get; set; }

    /// <summary>
    /// روز ماه برای برنامه‌ریزی (1-31)
    /// </summary>
    [DisplayName("روز ماه")]
    [SBVR(SBVRModality.Necessary, "روز ماه", "باید تعیین شود", "وقتی نوع برنامه‌ریزی 'روز ماه' باشد")]
    public int? DayOfMonth { get; set; }

    /// <summary>
    /// ماه برای برنامه‌ریزی (1-12)
    /// </summary>
    [DisplayName("ماه")]
    [SBVR(SBVRModality.Necessary, "ماه", "باید تعیین شود", "وقتی نوع برنامه‌ریزی 'ماهانه' باشد")]
    public int? Month { get; set; }

    /// <summary>
    /// روز سال برای برنامه‌ریزی (1-365)
    /// </summary>
    [DisplayName("روز سال")]
    [SBVR(SBVRModality.Necessary, "روز سال", "باید تعیین شود", "وقتی نوع برنامه‌ریزی 'سالیانه' باشد")]
    public int? DayOfYear { get; set; }

    /// <summary>
    /// ساعت انجام قرعه‌کشی (0-23)
    /// </summary>
    [DisplayName("ساعت")]
    [SBVR(SBVRModality.Recommended, "ساعت", "ساعت انجام قرعه‌کشی برای زمان‌بندی استفاده می‌شود")]
    public int? Hour { get; set; }

    /// <summary>
    /// دقیقه انجام قرعه‌کشی (0-59)
    /// </summary>
    [DisplayName("دقیقه")]
    [SBVR(SBVRModality.Recommended, "دقیقه", "دقیقه انجام قرعه‌کشی برای زمان‌بندی استفاده می‌شود")]
    public int? Minute { get; set; }

    /// <summary>
    /// شناسه Job زمان‌بندی شده برای اجرای قرعه‌کشی
    /// این فیلد برای ردیابی Job schedule شده در سیستم Job queue استفاده می‌شود
    /// </summary>
    [DisplayName("شناسه Job زمان‌بندی شده")]
    [MaxLength(128)]
    [SBVR(SBVRModality.Permitted, "ردیابی Job", "شناسه Job برای ردیابی و مدیریت Job زمان‌بندی شده")]
    public string? ScheduledJobId { get; set; }

    /// <summary>
    /// رابط‌های Navigation
    /// </summary>
    [DisplayName("پاداش‌های قرعه‌کشی")]
    public ICollection<LotteryReward> LotteryRewards { get; set; } = [];

    [DisplayName("شرکت‌کنندگان")]
    public ICollection<LotteryParticipant> Participants { get; set; } = [];
}
