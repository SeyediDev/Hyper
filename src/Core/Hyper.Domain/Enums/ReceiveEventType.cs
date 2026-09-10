namespace Hyper.Domain.Enums;

/// <summary>
/// نوع رویداد شرط پویش - تعیین می‌کند که به ازای دریافت چه رویدادی شرط حاصل می‌شود
/// </summary>
public enum ReceiveEventType
{
    /// <summary>
    /// دریافت رویداد پویا - رویدادهای تعریف شده در EventType
    /// فیلد تکمیلی: EventType, EventChannel
    /// </summary>
    [Description("دریافت رویداد پویا")]
    DynamicEvent = 1,
    
    /// <summary>
    /// معرفی مشتری جدید (دعوت دوست)
    /// فیلد تکمیلی: ندارد
    /// </summary>
    [Description("معرفی مشتری")]
    ReferCustomer = 2,
    
    /// <summary>
    /// ارتقاء سطح امتیاز مشتری
    /// فیلد تکمیلی: PointLevel
    /// </summary>
    [Description("ارتقاء سطح امتیاز")]
    UpgradePointLevel = 3,
    
    /// <summary>
    /// خرید پاداش توسط مشتری
    /// فیلد تکمیلی: Reward
    /// </summary>
    [Description("خرید پاداش")]
    PurchaseReward = 4,
    
    /// <summary>
    /// مصرف پاداش توسط مشتری
    /// فیلد تکمیلی: Reward
    /// </summary>
    [Description("مصرف پاداش")]
    ConsumeReward = 5,
    
    /// <summary>
    /// شرکت در نظرسنجی یا مسابقه
    /// فیلد تکمیلی: Survey (نال = همه نظرسنجی‌ها)
    /// </summary>
    [Description("شرکت در نظرسنجی/مسابقه")]
    ParticipateInSurvey = 7,
    
    /// <summary>
    /// شرکت در قرعه‌کشی یا چرخونه
    /// فیلد تکمیلی: Lottery (نال = همه قرعه‌کشی‌ها)
    /// </summary>
    [Description("شرکت در قرعه‌کشی/چرخونه")]
    ParticipateInLottery = 8,
    
    /// <summary>
    /// تعامل در بازخورد (ثبت نظر، لایک، پاسخ)
    /// فیلد تکمیلی: FeedbackType (نال = همه انواع)
    /// </summary>
    [Description("تعامل در بازخورد")]
    InteractInFeedback = 9,
    
    /// <summary>
    /// تعامل در انجمن (ایجاد موضوع، پاسخ، لایک)
    /// فیلد تکمیلی: ForumTopic (نال = همه موضوعات)
    /// </summary>
    [Description("تعامل در انجمن")]
    InteractInForum = 10,
    
    /// <summary>
    /// خرید طرح اشتراک
    /// فیلد تکمیلی: Plan (نال = همه طرح‌ها)
    /// </summary>
    [Description("خرید طرح اشتراک")]
    PurchasePlan = 11,
    
    /// <summary>
    /// تعامل با مرکز تماس
    /// فیلد تکمیلی: InteractionType (نال = همه انواع)
    /// </summary>
    [Description("تعامل با مرکز تماس")]
    CustomerCallCenterInteraction = 12,

    /// <summary>
    /// انتقال امتیاز
    /// فیلد تکمیلی: Point (نال = همه انواع)
    /// </summary>
    [Description("انتقال امتیاز")]
    TransferPoint = 13,

    /// <summary>
    /// انتقال امتیاز
    /// فیلد تکمیلی: Point (نال = همه انواع)
    /// </summary>
    [Description("تبدیل امتیاز")]
    ChangePoint = 14,
}