using Hyper.Domain.Entities.Lotteries;
using Neo.Bpms.Domain.Models.Attributes.RelationshipAttributes;
using Neo.Domain.Entities.Integrations;

namespace Hyper.Domain.Entities.Promotions;

/// <summary>
/// عملیات پویش - عملیاتی که به ازای یک پویش انجام می‌شود
/// می‌تواند فوری (Immediate)، روی شرط خاص (OnCondition) یا روی تکمیل (OnCompletion) باشد
/// </summary>
[DisplayName("عملیات پویش")]
[SBVR(SBVRModality.Obligatory, "عملیات پویش", "هر عملیات پویش باید برای تعیین اقداماتی که در پویش انجام می‌شود قابل شناسایی باشد")]
public class PromotionAction : HyperBaseCoreConfigAuditableEntity<int>, ISubOfPromotion
{
    public int PromotionId { get; set; }
    
    [DisplayName("پویش")]
    [SBVR(SBVRModality.Obligatory, "رابطه عملیات-پویش", "هر عملیات باید به یک پویش مشخص تعلق داشته باشد")]
    public Promotion Promotion { get; set; } = null!;

    [DisplayName("چه کسی")]
    [SBVR(SBVRModality.Obligatory, "هدف عملیات", "چه کسی تعیین می‌کند که عملیات بر روی چه کسی اعمال می‌شود")]
    public PromotionActionOnWho ActionOnWho { get; set; }
    
    [DisplayName("نوع اقدام")]
    [SBVR(SBVRModality.Obligatory, "نوع اقدام", "نوع اقدام تعیین می‌کند که چه عملیاتی انجام می‌شود")]
    public PromotionActionKind ActionKind { get; set; }

    [DisplayName("زمان اجرا")]
    [SBVR(SBVRModality.Obligatory, "نوع زمان اجرا", "نوع زمان اجرا تعیین می‌کند که عملیات چه زمانی اجرا می‌شود")]
    public PromotionActionRunTimeType RunTimeType { get; set; }

    public int? PromotionTriggerId { get; set; }
    [DisplayName("محرک عملیات پویش")]
    public PromotionTrigger? PromotionTrigger { get; set; }

    [DisplayName("روش اطلاع رسانی")]
    [SBVR(SBVRModality.Permitted, "اطلاع رسانی", "روش اطلاع رسانی تعیین می‌کند که چگونه به مشتری اطلاع داده می‌شود")]
    public PromotionNotificationSendMethod NotificationSendMethod { get; set; }

    [DisplayName("تمپلیت پیام")]
    [MaxLength(512)]
    [SBVR(SBVRModality.Permitted, "تمپلیت پیام", "تمپلیت پیام برای اطلاع رسانی")]
    public string? MessageTemplate { get; set; } = null!;

    public int? PointId { get; set; }
    
    [DisplayName("امتیاز")]
    [SBVR(SBVRModality.Permitted, "امتیاز عملیات", "کدام امتیاز تغییر کند؟")]
    public Point? Point { get; set; }

    public int? PointLevelId { get; set; }
    
    [DisplayName("سطح امتیاز")]
    [SBVR(SBVRModality.Permitted, "سطح امتیاز عملیات", "سطح امتیاز برای عملیات تنظیم سطح امتیاز")]
    public PointLevel? PointLevel { get; set; }

    public int? RewardId { get; set; }
    
    [DisplayName("پاداش")]
    [SBVR(SBVRModality.Permitted, "اعطای پاداش", "پاداش برای تعریف پاداشی که به مشتری اعطا می‌شود")]
    public Reward? Reward { get; set; }

    /// <summary>
    /// For PromotionActionKind.JoinLottery
    /// </summary>
    public int? LotteryId { get; set; }
    
    [DisplayName("قرعه‌کشی")]
    [SBVR(SBVRModality.Permitted, "قرعه‌کشی", "قرعه‌کشی برای عملیات شرکت در قرعه‌کشی")]
    public Lottery? Lottery { get; set; }

    public int? CustomerSegmentId { get; set; }
    
    [DisplayName("جامعه/بازار هدف")]
    [SBVR(SBVRModality.Permitted, "جامعه مشتریان", "جامعه مشتریان برای عملیات عضویت در جامعه")]
    public CustomerSegment? CustomerSegment { get; set; }

    [DisplayName("ترتیب")]
    [SBVR(SBVRModality.Permitted, "ترتیب عملیات", "ترتیب برای تعیین اولویت اجرای عملیات")]
    public int? Order { get; set; }

    [OldDbMap("CustomerParameterId")]
    public int? CustomerAttributeId { get; set; }
    [DisplayName("ویژگی مشتری")]
    [RelationshipConstraint($"{nameof(TenantAttribute.CustomerUsage)}>0")]
    [SBVR(SBVRModality.Permitted, "ویژگی", "ویژگی برای عملیات مقداردهی")]
    public TenantAttribute? CustomerAttribute { get; set; }

    /// <summary>
    /// فرمول مقدار - فرمول به صورت یک رشته قابل خواندن نوشته می‌شود
    ///
    /// فرمت فرمول:
    /// - می‌تواند از Customer.*, Tenant.*, Event.* استفاده کند
    /// - می‌تواند از توابع ریاضی و منطقی استفاده کند
    /// - می‌تواند مقدار ثابت باشد (مثال: "100")
    /// - می‌تواند از ویژگی رویداد استفاده کند (مثال: "Event.Cost")
    /// - می‌تواند از ویژگی مشتری استفاده کند (مثال: "Customer.Age")
    /// - می‌تواند از ویژگی اکوسیستم استفاده کند (مثال: "Tenant.MaxRegularAge")
    ///
    /// مثال‌ها:
    /// - "100" (مقدار ثابت)
    /// - "Event.Cost" (از ویژگی رویداد)
    /// - "Event.Cost * 0.1" (10 درصد از هزینه رویداد)
    /// - "Math.Max(Event.Cost, Tenant.MinPurchaseAmount)" (حداکثر بین هزینه و حداقل خرید)
    /// - "Customer.Age > 18 ? 100 : 50" (شرطی)
    /// </summary>
    [DisplayName("فرمول مقدار")]
    [MaxLength(2000)]
    [SBVR(SBVRModality.Obligatory, "فرمول مقدار",
        "فرمول مقدار به صورت یک رشته قابل خواندن نوشته می‌شود که مقدار عملیات را تعریف می‌کند. " +
        "این فرمول می‌تواند یک مقدار عددی، متنی یا بولی را برمی‌گرداند. " +
        "فرمول می‌تواند از موارد زیر استفاده کند: " +
        "1. ویژگی‌های رویداد با پیشوند Event.* (مثال: Event.Cost, Event.Amount, Event.Force) " +
        "2. نوع رفتار مشتری با Event.BehaviorTypeValue (عدد) یا Event.BehaviorType (string) " +
        "   - Event.BehaviorTypeValue == 1 (خرید), 2 (ابطال خرید), 3 (مشاهده محصول), 4 (افزودن به سبد), 5 (حذف از سبد), " +
        "     6 (شروع تسویه), 7 (تکمیل تسویه), 8 (نصب برنامه), 9 (حذف برنامه), 10 (ورود), 11 (خروج), 12 (ثبت‌نام), " +
        "     13 (جستجو), 14 (اشتراک‌گذاری), 15 (نظر دادن), 16 (دانلود), 17 (اشتراک), 18 (لغو اشتراک), " +
        "     19 (مشاهده صفحه), 20 (پخش ویدیو), 21 (اتمام ویدیو), 22 (کلیک), 23 (اسکرول), 24 (زمان سپری شده), " +
        "     25 (افزودن به علاقه‌مندی‌ها), 26 (حذف از علاقه‌مندی‌ها), 27 (بازگشت), 28 (خرید مجدد), " +
        "     29 (ارجاع), 30 (تعامل با محتوا), 0 (سفارشی) " +
        "   - Event.BehaviorType == \"Purchase\" یا Event.BehaviorType == \"Login\" (نام enum به صورت string) " +
        "3. فیلدهای مشتری با پیشوند Customer.* (مثال: Customer.Age, Customer.FirstName, Customer.BirthDate) " +
        "4. مقادیر تنظیمات سازمان با پیشوند Tenant.* (مثال: Tenant.MaxRegularAge, Tenant.AverageCost) " +
        "5. توابع ریاضی: Math.Max(), Math.Min(), Math.Abs(), Math.Round(), Math.Floor(), Math.Ceiling() " +
        "6. توابع رشته: String.Contains(), String.StartsWith(), String.EndsWith(), String.ToLower(), String.ToUpper() " +
        "7. توابع تاریخ: DateDiff(), Year(), Month(), Day(), Now(), Today() " +
        "8. عملگرهای ریاضی: +, -, *, /, % " +
        "9. عملگرهای مقایسه: ==, !=, >, <, >=, <= " +
        "10. عملگرهای منطقی: && (AND), || (OR), ! (NOT) " +
        "11. عملگر شرطی: ? : (ternary operator) " +
        "12. پرانتز برای اولویت عملیات: () " +
        "مثال مقدار ثابت: '100' " +
        "مثال از ویژگی: 'Event.Cost' " +
        "مثال محاسباتی: 'Event.Cost * 0.1' " +
        "مثال شرطی بر اساس رفتار: 'Event.BehaviorTypeValue == 1 ? Event.Cost * 0.1 : 0' (10% هزینه برای خرید) " +
        "مثال شرطی بر اساس رفتار: 'Event.BehaviorType == \"Registration\" ? 1000 : 0' (1000 امتیاز برای ثبت‌نام) " +
        "مثال شرطی پیچیده: '(Event.BehaviorTypeValue == 1) && (Event.Cost > 1000000) ? Event.Cost * 0.15 : Event.Cost * 0.1' " +
        "مثال شرطی ترکیبی: 'Customer.Age > 18 ? (Event.BehaviorTypeValue == 1 ? 100 : 50) : 25' " +
        "نکته: برای مقادیر متنی از کوتیشن استفاده کنید: '\"متن\"' یا 'Event.BehaviorType == \"Purchase\" ? \"خرید\" : \"سایر\"' " +
        "نکته: برای مقادیر بولی از true یا false استفاده کنید: 'true' " +
        "نکته: استفاده از BehaviorTypeValue (عدد) برای مقایسه سریع‌تر است. " +
        "نکته: استفاده از BehaviorType (string) برای خوانایی بهتر است.")]
    [SBVR(SBVRModality.Permitted, "توابع ریاضی", "راهنمای استفاده در فرمول نویسی ها" +
@"
### Math Patterns:

| Pattern               | Meaning                                | Example                      | Output                |
|-----------------------|----------------------------------------|------------------------------|-----------------------|
| Math.Max()            |                                        | Max(2,3)                     | 3                     |
| Math.Min()            |                                        | Min(2,3)                     | 2                     |
| Math.Abs()            |                                        | Abs(-2)                      | 2                     |
| Math.Round()          |                                        | Round(2.2)                   | 2                     |
| Math.Floor()          |                                        | Floor(2.3000)                | 2.3                   |
| Math.Ceiling()        |                                        | Ceiling(2.1)                 | 2                     |
")]
    [SBVR(SBVRModality.Permitted, "کلید خواندن ویژگی‌ها", "راهنمای استفاده در فرمول نویسی ها" +
@"
### Patterns:

| Pattern                     | Meaning                                | Example                      | Output                |
|-----------------------------|----------------------------------------|------------------------------|-----------------------|
| CustomerPoint.{Key}         | موجودی مشتری در امتیاز با این کلید    | CustomerPoint.Momtaz         | 125                   |
| CustomerPoint.{Key}.Level   | کلید سطح مشتری در امتیاز با این کلید  | CustomerPoint.Momtaz.Level   | Platinium             |
| CustomerPoint.{Key}.LevelId | سطح، سطح مشتری در امتیاز با این کلید  | CustomerPoint.Momtaz.LevelId | 3                     |")]
    public string? AmountFormula { get; set; }

    #region ExternalApi
    public int? ExternalApiId { get; set; }
    
    [DisplayName("API بیرونی")]
    [SBVR(SBVRModality.Permitted, "API بیرونی", "API بیرونی برای فراخوانی API خارجی")]
    public ExternalApi? ExternalApi { get; set; }

    [DisplayName("نگاشت پارامتر مسیر")]
    [Column(TypeName = "nvarchar(max)")]
    [SBVR(SBVRModality.Permitted, "نگاشت مسیر", "نگاشت پارامتر مسیر برای API بیرونی")]
    public string? PathParameterMappingsJson { get; set; }

    [DisplayName("نگاشت پارامتر Query")]
    [Column(TypeName = "nvarchar(max)")]
    [SBVR(SBVRModality.Permitted, "نگاشت Query", "نگاشت پارامتر Query برای API بیرونی")]
    public string? QueryParameterMappingsJson { get; set; }

    [DisplayName("نگاشت هدرها")]
    [Column(TypeName = "nvarchar(max)")]
    [SBVR(SBVRModality.Permitted, "نگاشت هدر", "نگاشت هدرها برای API بیرونی")]
    public string? HeaderMappingsJson { get; set; }

    [DisplayName("الگوی بدنه")]
    [Column(TypeName = "nvarchar(max)")]
    [SBVR(SBVRModality.Permitted, "الگوی بدنه", "الگوی بدنه برای API بیرونی")]
    public string? BodyTemplate { get; set; }

    [DisplayName("نوع محتوای بدنه")]
    [MaxLength(128)]
    [SBVR(SBVRModality.Permitted, "نوع محتوا", "نوع محتوای بدنه برای API بیرونی")]
    public string? BodyContentType { get; set; }

    [DisplayName("نگاشت پاسخ")]
    [Column(TypeName = "nvarchar(max)")]
    [SBVR(SBVRModality.Permitted, "نگاشت پاسخ", "نگاشت پاسخ برای API بیرونی")]
    public string? ResponseMappingsJson { get; set; }
    #endregion ExternalApi
}

