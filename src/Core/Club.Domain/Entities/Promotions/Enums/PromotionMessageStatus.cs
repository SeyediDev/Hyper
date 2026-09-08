namespace Hyper.Domain.Entities.Promotions.Enums;

/// <summary>
/// وضعیت پیام پویش - وضعیت فعلی پیام
/// </summary>
public enum PromotionMessageStatus
{
    /// <summary>
    /// پیش‌نویس
    /// </summary>

    Draft = 1,

    /// <summary>
    /// آماده ارسال
    /// </summary>

    ReadyToSend = 2,

    /// <summary>
    /// در حال ارسال
    /// </summary>

    Sending = 3,

    /// <summary>
    /// ارسال شده
    /// </summary>

    Sent = 4,

    /// <summary>
    /// تحویل شده
    /// </summary>

    Delivered = 5,

    /// <summary>
    /// خوانده شده
    /// </summary>

    Read = 6,

    /// <summary>
    /// ناموفق
    /// </summary>

    Failed = 7,

    /// <summary>
    /// لغو شده
    /// </summary>

    Cancelled = 8
}
