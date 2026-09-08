namespace Hyper.Domain.Entities.Promotions;

/// <summary>
/// پیام پویش - مدیریت پیام‌های ارسالی در کمپین‌های بازاریابی
/// این موجودیت شامل اطلاعات کامل پیام‌های ارسالی، آمار ارسال و نرخ‌های تعامل می‌باشد
/// </summary>
[DisplayName("پیام پویش")]
public class PromotionMessage : HyperBaseCoreAuditableEntity<int>, ISubOfPromotion
{
    public int PromotionId { get; set; }
    [DisplayName("پویش")]
    public Promotion Promotion { get; set; } = null!;

    [DisplayName("روش اطلاع رسانی")]
    [SBVR(SBVRModality.Permitted, "اطلاع رسانی", "روش اطلاع رسانی تعیین می‌کند که چگونه به مشتری اطلاع داده می‌شود")]
    public PromotionMessageSendMethod SendMethod { get; set; }

    /// <summary>
    /// موضوع پیام - عنوان کوتاه و جذاب برای پیام (اختیاری)
    /// حداکثر 100 کاراکتر
    /// </summary>
    [DisplayName("موضوع پیام")]
    [MaxLength(100)]
    [SBVR(SBVRModality.Recommended, "موضوع پیام", "موضوع پیام باید جذاب و مختصر باشد تا توجه گیرنده را جلب کند")]
    public string? Subject { get; set; }

    /// <summary>
    /// متن اصلی پیام - محتوای پیامک که به مشتریان ارسال می‌شود
    /// حداکثر 1000 کاراکتر - اجباری
    /// </summary>
    [DisplayName("متن پیام")]
    [MaxLength(1000)]
    [SBVR(SBVRModality.Obligatory, "متن پیام", "متن پیام باید واضح، مختصر و جذاب باشد")]
    [SBVR(SBVRModality.Recommended, "متن پیام", "متن پیام باید شامل دعوت به عمل (Call to Action) باشد")]
    public string Content { get; set; } = null!;

    /// <summary>
    /// نوع پیام - تعیین نوع پیام (تبلیغاتی، اطلاع‌رسانی، تعاملی و...)
    /// </summary>
    [DisplayName("نوع پیام")]
    [SBVR(SBVRModality.Obligatory, "نوع پیام", "نوع پیام باید بر اساس هدف کمپین انتخاب شود")]
    public PromotionMessageType Type { get; set; }

    /// <summary>
    /// اولویت پیام - تعیین اولویت ارسال پیام (1=بالا، 5=پایین)
    /// پیش‌فرض: 1
    /// </summary>
    [DisplayName("اولویت پیام")]
    [SBVR(SBVRModality.Recommended, "اولویت پیام", "پیام‌های با اولویت بالاتر زودتر ارسال می‌شوند")]
    [SBVR(SBVRModality.Permitted, "اولویت پیام", "اولویت 1 برای پیام‌های فوری، اولویت 5 برای پیام‌های معمولی")]
    public int Priority { get; set; } = 1;

    /// <summary>
    /// زمان برنامه‌ریزی شده - زمان مشخص شده برای ارسال پیام (اختیاری)
    /// </summary>
    [DisplayName("زمان برنامه‌ریزی شده")]
    [SBVR(SBVRModality.Permitted, "زمان برنامه‌ریزی شده", "زمان برنامه‌ریزی شده برای ارسال خودکار پیام استفاده می‌شود")]
    public DateTime? ScheduledTime { get; set; }

    /// <summary>
    /// تعداد تکرار - تعداد دفعات تلاش برای ارسال پیام
    /// </summary>
    [DisplayName("تعداد تکرار")]
    [SBVR(SBVRModality.Calculated, "تعداد تکرار", "تعداد تکرار بر اساس تلاش‌های ناموفق محاسبه می‌شود")]
    public int RetryCount { get; set; }

    /// <summary>
    /// حداکثر تکرار - حداکثر تعداد تلاش برای ارسال پیام
    /// پیش‌فرض: 3
    /// </summary>
    [DisplayName("حداکثر تکرار")]
    [SBVR(SBVRModality.Recommended, "حداکثر تکرار", "حداکثر تکرار باید بین 1 تا 5 باشد تا از اسپم جلوگیری شود")]
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// وضعیت پیام - وضعیت فعلی پیام در فرآیند ارسال
    /// </summary>
    [DisplayName("وضعیت پیام")]
    [SBVR(SBVRModality.Calculated, "وضعیت پیام", "وضعیت پیام بر اساس پیشرفت فرآیند ارسال محاسبه می‌شود")]
    public PromotionMessageStatus Status { get; set; }

    /// <summary>
    /// تاریخ ارسال - زمان واقعی ارسال پیام (اختیاری)
    /// </summary>

    [SBVR(SBVRModality.Calculated, "تاریخ ارسال", "تاریخ ارسال پس از ارسال موفق پیام ثبت می‌شود")]
    public DateTime? SentDate { get; set; }

    /// <summary>
    /// تعداد ارسال شده - تعداد کل پیام‌هایی که ارسال شده‌اند
    /// </summary>
    [DisplayName("تعداد ارسال شده")]
    [SBVR(SBVRModality.Calculated, "تعداد ارسال شده", "تعداد ارسال شده بر اساس تعداد گیرندگان موفق محاسبه می‌شود")]
    public int SentCount { get; set; }

    /// <summary>
    /// تعداد تحویل شده - تعداد پیام‌هایی که با موفقیت تحویل داده شده‌اند
    /// </summary>
    [DisplayName("تعداد تحویل شده")]
    [SBVR(SBVRModality.Calculated, "تعداد تحویل شده", "تعداد تحویل شده بر اساس گزارش‌های اپراتور محاسبه می‌شود")]
    public int DeliveredCount { get; set; }

    /// <summary>
    /// تعداد خوانده شده - تعداد پیام‌هایی که توسط گیرندگان خوانده شده‌اند
    /// </summary>
    [DisplayName("تعداد خوانده شده")]
    [SBVR(SBVRModality.Calculated, "تعداد خوانده شده", "تعداد خوانده شده بر اساس گزارش‌های گیرندگان محاسبه می‌شود")]
    public int ReadCount { get; set; }

    /// <summary>
    /// نرخ تحویل - درصد پیام‌هایی که با موفقیت تحویل داده شده‌اند
    /// محاسبه شده: (تعداد تحویل شده / تعداد ارسال شده) * 100
    /// </summary>

    [SBVR(SBVRModality.Calculated, "نرخ تحویل", "نرخ تحویل = (تعداد تحویل شده / تعداد ارسال شده) × 100")]
    [SBVR(SBVRModality.Recommended, "نرخ تحویل", "نرخ تحویل بالای 95% نشان‌دهنده کیفیت خوب پیام است")]
    public decimal DeliveryRate => SentCount > 0 ? (decimal)DeliveredCount / SentCount : 0;

    /// <summary>
    /// نرخ خواندن - درصد پیام‌هایی که توسط گیرندگان خوانده شده‌اند
    /// محاسبه شده: (تعداد خوانده شده / تعداد تحویل شده) * 100
    /// </summary>

    [SBVR(SBVRModality.Calculated, "نرخ خواندن", "نرخ خواندن = (تعداد خوانده شده / تعداد تحویل شده) × 100")]
    [SBVR(SBVRModality.Recommended, "نرخ خواندن", "نرخ خواندن بالای 80% نشان‌دهنده جذابیت پیام است")]
    public decimal ReadRate => DeliveredCount > 0 ? (decimal)ReadCount / DeliveredCount : 0;
}