namespace Hyper.Domain.Entities.Points;

/// <summary>
/// هدف : کنترل حداکثر دریافت امتیاز در یک بازه زمانی
/// </summary>
[DisplayName("بودجه امتیازدهی")]
public class PointBudget : HyperBaseCoreConfigAuditableEntity<int>
{
    public int PointId { get; set; }
    [DisplayName("امتیاز")]
    [SBVR(SBVRModality.Obligatory, "امتیاز", "تعیین می کنیم که بودجه بندی را داریم برای چه امتیازی تعیین می کنیم")]
    public Point Point { get; set; } = null!;

    [DisplayName("حوزه")]
    [SBVR(SBVRModality.Obligatory, "حوزه", 
        " حوزه تعیین می کند که در چه حوزه ای بودجه گذاری می کنیم."+
        " با این فیلد تعیین می شود که بودجه برای کلیه مشتریان است،" +
        " یا کلیه مشتریان یک جامعه خاص،" +
        " یا فقط بودجه برای یک مشتری خاص است")]
    public PointBudgetScope Scope { get; set; }

    public int? CustomerSegmentId { get; set; }
    [DisplayName("جامعه مشتریان")]
    [SBVR(SBVRModality.Necessary, "جامعه مشتریان", "باید تعیین شود", "وقتی حوزه 'مشتریان یک جامعه' باشد")]
    public CustomerSegment? CustomerSegment { get; set; }

    [DisplayName("حداکثر امتیاز قابل دریافت")]
    public long Amount { get; set; }

    [DisplayName("از تاریخ")]
    [SBVR(SBVRModality.Permitted, "از تاریخ", "تاریخ ازل در نظرگرفته می شود", "وقتی مقدار نگرفته باشد")]
    [SBVR(SBVRModality.Necessary, "بازه تاریخی", "بازه تاریخی اعتبار این بودجه بندی تعیین می شود")]
    public DateTime? FromDate { get; set; }

    [DisplayName("تا تاریخ")]
    [SBVR(SBVRModality.Permitted, "تا تاریخ", "تاریخ ابد در نظرگرفته می شود", "وقتی مقدار نگرفته باشد")]
    [SBVR(SBVRModality.Necessary, "بازه تاریخی", "بازه تاریخی اعتبار این بودجه بندی تعیین می شود")]
    public DateTime? ToDate { get; set; }

    [DisplayName("نوع پنجره زمانی کنترل بودجه")]
    [SBVR(SBVRModality.Obligatory, "نوع بودجه بندی", "تعیین می شود که در تاریخ اعتبار این بودجه بندی بودجه در چه پنجره زمانی ای کنترل شود")]
    public PointBudgetKind Kind { get; set; }

    /// <summary>
    /// در صورت نال بودن برای همه صادرکننده‌های رویداد این محدودیت اعمال می شود
    /// </summary>
    public int? EventChannelId { get; set; }
    [DisplayName("فیلتر صادرکننده رویداد")]
    [SBVR(SBVRModality.Permitted, "فیلتر صادرکننده رویداد", "اگر بخواهیم امتیازات دریافتی از یک صادرکننده‌رویداد خاص را محدود و بودجه بندی کنیم آن صادرکننده‌رویداد را اینجا فیلتر می کنیم")]
    public EventChannel? EventChannel { get; set; } = null!;

    /// <summary>
    /// در صورت نال بودن برای همه رویداد این محدودیت اعمال می شود
    /// </summary>
    public int? EventTypeId { get; set; }
    [DisplayName("فیلتر رویداد")]
    [SBVR(SBVRModality.Permitted, "فیلتر رویداد", "اگر بخواهیم امتیازات دریافتی از یک رویداد خاص را محدود و بودجه بندی کنیم آن رویداد را اینجا فیلتر می کنیم")]
    public EventType? EventType { get; set; } = null!;

    /// <summary>
    /// در صورت نال بودن برای همه سطوح امتیاز محدودیت اعمال می شود
    /// در غیر اینصورت اگر مشتری سطوح امتیاز را دارا باشد این محدودیت برایش اعمال نمی شود
    /// </summary>
    public int? PointLevelId { get; set; }

    [DisplayName("سطح‌امتیاز رفع کننده")]
    [SBVR(SBVRModality.Permitted, "سطح‌امتیاز رفع کننده", "در صورتیکه مشتری دارای این سطح‌امتیاز و یا بیشتر باشد شامل این بودجه بندی نمی شود")]
    public PointLevel? PointLevel { get; set; } = null!;

    // ===== SCHEDULING FIELDS FOR AUTOMATIC BUDGET RESET =====
    
    /// <summary>
    /// آیا بودجه به صورت خودکار بازنشانی می‌شود
    /// </summary>
    [DisplayName("بازنشانی خودکار")]
    [SBVR(SBVRModality.Permitted, "بازنشانی خودکار", "برای بودجه‌هایی که می‌خواهند به صورت دوره‌ای بازنشانی شوند")]
    public bool AutoReset { get; set; }

    /// <summary>
    /// نوع برنامه‌ریزی بازنشانی
    /// </summary>
    [DisplayName("نوع برنامه‌ریزی")]
    [SBVR(SBVRModality.Necessary, "نوع برنامه‌ریزی", "باید تعیین شود", "وقتی بازنشانی خودکار فعال باشد")]
    public SchedulingKind? ResetSchedulingKind { get; set; }

    /// <summary>
    /// روز هفته برای بازنشانی (1=شنبه تا 7=جمعه)
    /// </summary>
    [DisplayName("روز هفته")]
    [SBVR(SBVRModality.Necessary, "روز هفته", "باید تعیین شود", "وقتی نوع برنامه‌ریزی 'روز هفته' باشد")]
    public int? ResetDayOfWeek { get; set; }

    /// <summary>
    /// روز ماه برای بازنشانی (1-31)
    /// </summary>
    [DisplayName("روز ماه")]
    [SBVR(SBVRModality.Necessary, "روز ماه", "باید تعیین شود", "وقتی نوع برنامه‌ریزی 'روز ماه' باشد")]
    public int? ResetDayOfMonth { get; set; }

    /// <summary>
    /// ماه برای بازنشانی (1-12)
    /// </summary>
    [DisplayName("ماه")]
    [SBVR(SBVRModality.Necessary, "ماه", "باید تعیین شود", "وقتی نوع برنامه‌ریزی 'ماهانه' باشد")]
    public int? ResetMonth { get; set; }

    /// <summary>
    /// روز سال برای بازنشانی (1-365)
    /// </summary>
    [DisplayName("روز سال")]
    [SBVR(SBVRModality.Necessary, "روز سال", "باید تعیین شود", "وقتی نوع برنامه‌ریزی 'سالیانه' باشد")]
    public int? ResetDayOfYear { get; set; }

    /// <summary>
    /// ساعت بازنشانی (0-23)
    /// </summary>
    [DisplayName("ساعت")]
    public int? ResetHour { get; set; }

    /// <summary>
    /// دقیقه بازنشانی (0-59)
    /// </summary>
    [DisplayName("دقیقه")]
    public int? ResetMinute { get; set; }
}