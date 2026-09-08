namespace Hyper.Domain.Entities.Promotions.Data;

/// <summary>
/// گیرنده پویش - مدیریت گیرندگان پیام‌های کمپین و تعاملات آنها
/// این موجودیت شامل اطلاعات کامل گیرندگان، وضعیت ارسال و آمار تعامل می‌باشد
/// </summary>
[DisplayName("گیرنده پویش/کمپین")]
public class PromotionRecipient : HyperBaseCoreAuditableEntity<int>
{
    /// <summary>
    /// شناسه پویش مرتبط - هر گیرنده باید به یک پویش مشخص تعلق داشته باشد
    /// </summary>
    public int PromotionId { get; set; }

    public Promotion Promotion { get; set; } = null!;

    [DisplayName("شناسه مشتری")]
    public int CustomerTenantId { get; set; }

    public CustomerTenant CustomerTenant { get; set; } = null!;

    /// <summary>
    /// وضعیت ارسال - وضعیت فعلی ارسال پیام به این گیرنده
    /// </summary>
    [DisplayName("وضعیت ارسال")]
    [SBVR(SBVRModality.Calculated, "وضعیت ارسال", "وضعیت ارسال بر اساس پیشرفت فرآیند ارسال به این گیرنده محاسبه می‌شود")]
    public PromotionRecipientStatus Status { get; set; }

    /// <summary>
    /// تاریخ ارسال - زمان واقعی ارسال پیام به این گیرنده (اختیاری)
    /// </summary>
    [DisplayName("تاریخ ارسال")]
    [SBVR(SBVRModality.Calculated, "تاریخ ارسال", "تاریخ ارسال پس از ارسال موفق پیام ثبت می‌شود")]
    public DateTime? SentDate { get; set; }

    /// <summary>
    /// تاریخ تحویل - زمان تحویل موفق پیام به گیرنده (اختیاری)
    /// </summary>
    [DisplayName("تاریخ تحویل")]
    [SBVR(SBVRModality.Calculated, "تاریخ تحویل", "تاریخ تحویل پس از تحویل موفق پیام به گیرنده ثبت می‌شود")]
    public DateTime? DeliveredDate { get; set; }

    /// <summary>
    /// تاریخ خواندن - زمان خواندن پیام توسط گیرنده (اختیاری)
    /// </summary>
    [DisplayName("تاریخ خواندن")]
    [SBVR(SBVRModality.Calculated, "تاریخ خواندن", "تاریخ خواندن پس از باز شدن پیام توسط گیرنده ثبت می‌شود")]
    public DateTime? ReadDate { get; set; }

    /// <summary>
    /// پیام خطا - متن خطای احتمالی در ارسال پیام (اختیاری)
    /// حداکثر 500 کاراکتر
    /// </summary>
    [DisplayName("پیام خطا")]
    [MaxLength(500)]
    [SBVR(SBVRModality.Calculated, "پیام خطا", "پیام خطا شامل جزئیات خطای ارسال برای عیب‌یابی استفاده می‌شود")]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// نمره تعامل - نمره تعامل گیرنده با پیام (0-100)
    /// </summary>
    [DisplayName("نمره تعامل")]
    [SBVR(SBVRModality.Recommended, "نمره تعامل", "نمره تعامل برای ارزیابی اثربخشی پیام و کیفیت گیرندگان استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نمره تعامل", "نمره تعامل بر اساس خواندن، کلیک، پاسخ و مدت زمان تعامل محاسبه می‌شود")]
    public decimal EngagementScore { get; set; }

    /// <summary>
    /// تعداد کلیک - تعداد کلیک‌های گیرنده روی لینک‌های موجود در پیام
    /// </summary>
    [DisplayName("تعداد کلیک")]
    [SBVR(SBVRModality.Calculated, "تعداد کلیک", "تعداد کلیک بر اساس تعداد کلیک‌های گیرنده روی لینک‌های موجود در پیام محاسبه می‌شود")]
    public int ClickCount { get; set; }

    /// <summary>
    /// تعداد پاسخ - تعداد پاسخ‌های دریافتی از گیرنده
    /// </summary>
    [DisplayName("تعداد پاسخ")]
    [SBVR(SBVRModality.Calculated, "تعداد پاسخ", "تعداد پاسخ بر اساس تعداد پیام‌های دریافتی از گیرنده محاسبه می‌شود")]
    public int ResponseCount { get; set; }

    /// <summary>
    /// زمان پاسخ - زمان اولین پاسخ دریافتی از گیرنده (اختیاری)
    /// </summary>
    [DisplayName("زمان پاسخ")]
    [SBVR(SBVRModality.Calculated, "زمان پاسخ", "زمان پاسخ برای ارزیابی سرعت تعامل گیرنده استفاده می‌شود")]
    public DateTime? ResponseTime { get; set; }

    /// <summary>
    /// نوع پاسخ - نوع پاسخ دریافتی از گیرنده (اختیاری)
    /// حداکثر 100 کاراکتر
    /// </summary>
    [DisplayName("نوع پاسخ")]
    [MaxLength(100)]
    [SBVR(SBVRModality.Calculated, "نوع پاسخ", "نوع پاسخ برای دسته‌بندی و تحلیل پاسخ‌های دریافتی استفاده می‌شود")]
    public string? ResponseType { get; set; }

    /// <summary>
    /// متن پاسخ - متن کامل پاسخ دریافتی از گیرنده (اختیاری)
    /// حداکثر 500 کاراکتر
    /// </summary>
    [DisplayName("متن پاسخ")]
    [MaxLength(500)]
    [SBVR(SBVRModality.Recommended, "متن پاسخ", "متن پاسخ برای تحلیل بازخورد و بهبود کمپین استفاده می‌شود")]
    public string? ResponseText { get; set; }
}

