using Hyper.Domain.Entities.Lotteries;
using Neo.Domain.Entities.Integrations;

namespace Hyper.Domain.Entities.Promotions;
/// <summary>
/// هدف : کنترل حداکثر دریافت امتیاز، تعداد پاداش، عضویت در جامعه، 
/// شرکت در قرعه‌کشی، فراخوانی API و اطلاع‌رسانی در یک بازه زمانی
/// </summary>
[DisplayName("بودجه پویش")]
public class PromotionBudget : HyperBaseCoreConfigAuditableEntity<int>, ISubOfPromotion
{
    public int PromotionId { get; set; }
    [DisplayName("پویش")]
    public Promotion Promotion { get; set; } = null!;

    [DisplayName("نوع بودجه")]
    [SBVR(SBVRModality.Obligatory, "نوع بودجه", "تعیین می‌کند که بودجه‌بندی بر اساس چه معیاری انجام شود")]
    [InDisplayString]
    public PromotionBudgetKind Kind { get; set; }

    public int? PointId { get; set; }
    [DisplayName("امتیاز")]
    [SBVR(SBVRModality.Permitted, "امتیاز", "تعیین می‌کند که بودجه بندی برای چه امتیازی تعیین شود - برای بودجه امتیاز")]
    public Point? Point { get; set; }

    public int? RewardId { get; set; }
    [DisplayName("پاداش")]
    [SBVR(SBVRModality.Permitted, "پاداش", "پاداش برای بودجه‌بندی تعداد اعطای پاداش - برای بودجه اعطای پاداش")]
    public Reward? Reward { get; set; }

    public int? LotteryId { get; set; }
    [DisplayName("قرعه‌کشی")]
    [SBVR(SBVRModality.Permitted, "قرعه‌کشی", "قرعه‌کشی برای بودجه‌بندی تعداد شرکت‌کنندگان - برای بودجه قرعه‌کشی")]
    public Lottery? Lottery { get; set; }

    public int? CustomerSegmentId { get; set; }
    [DisplayName("جامعه/بازار هدف")]
    [SBVR(SBVRModality.Permitted, "جامعه مشتریان", "جامعه مشتریان برای بودجه‌بندی تعداد عضویت - برای بودجه عضویت در جامعه")]
    public CustomerSegment? CustomerSegment { get; set; }

    public int? ExternalApiId { get; set; }
    [DisplayName("API بیرونی")]
    [SBVR(SBVRModality.Permitted, "API بیرونی", "API بیرونی برای بودجه‌بندی تعداد فراخوانی - برای بودجه فراخوانی API")]
    public ExternalApi? ExternalApi { get; set; }

    [DisplayName("زیرنوع اطلاع‌رسانی")]
    [SBVR(SBVRModality.Permitted, "روش اطلاع‌رسانی", "روش اطلاع‌رسانی برای بودجه‌بندی تفکیکی - برای بودجه اطلاع‌رسانی")]
    public NotificationBudgetSubType? NotificationSubType { get; set; }

    [DisplayName("بودجه")]
    [SBVR(SBVRModality.Obligatory, "حداکثر مقدار بودجه", "تعیین حداکثر مقدار یا تعداد بسته به نوع بودجه")]
    public long Amount { get; set; }

    // ===== SCHEDULING FIELDS FOR AUTOMATIC BUDGET RESET =====

    [DisplayName("نوع بازنشانی بودجه")]
    public BudgetResetType? ResetType { get; set; }

    /// <summary>
    /// روز هفته برای بازنشانی (1=شنبه تا 7=جمعه)
    /// </summary>
    [DisplayName("روز هفته بازنشانی")]
    [SBVR(SBVRModality.Necessary, "روز هفته", "باید تعیین شود", "وقتی نوع برنامه‌ریزی 'روز هفته' باشد")]
    public int? ResetDayOfWeek { get; set; }

    /// <summary>
    /// روز ماه برای بازنشانی (1-31)
    /// </summary>
    [DisplayName("روز ماه بازنشانی")]
    [SBVR(SBVRModality.Necessary, "روز ماه", "باید تعیین شود", "وقتی نوع برنامه‌ریزی 'روز ماه' باشد")]
    public int? ResetDayOfMonth { get; set; }

    /// <summary>
    /// ماه برای بازنشانی (1-12)
    /// </summary>
    [DisplayName("ماه بازنشانی")]
    [SBVR(SBVRModality.Necessary, "ماه", "باید تعیین شود", "وقتی نوع برنامه‌ریزی 'ماهانه' باشد")]
    public int? ResetMonth { get; set; }

    /// <summary>
    /// روز سال برای بازنشانی (1-365)
    /// </summary>
    [DisplayName("روز سال بازنشانی")]
    [SBVR(SBVRModality.Necessary, "روز سال", "باید تعیین شود", "وقتی نوع برنامه‌ریزی 'سالیانه' باشد")]
    public int? ResetDayOfYear { get; set; }

    /// <summary>
    /// ساعت بازنشانی (0-23)
    /// </summary>
    [DisplayName("ساعت بازنشانی")]
    public int? ResetHour { get; set; }

    /// <summary>
    /// دقیقه بازنشانی (0-59)
    /// </summary>
    [DisplayName("دقیقه بازنشانی")]
    public int? ResetMinute { get; set; }
}