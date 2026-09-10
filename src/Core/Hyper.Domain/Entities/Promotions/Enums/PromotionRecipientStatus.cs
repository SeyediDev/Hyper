namespace Hyper.Domain.Entities.Promotions.Enums;

/// <summary>
/// وضعیت گیرنده پویش - وضعیت فعلی گیرنده
/// </summary>
public enum PromotionRecipientStatus
{
    /// <summary>
    /// در انتظار ارسال
    /// </summary>

    Pending = 1,

    /// <summary>
    /// در حال ارسال
    /// </summary>

    Sending = 2,

    /// <summary>
    /// ارسال شده
    /// </summary>

    Sent = 3,

    /// <summary>
    /// تحویل شده
    /// </summary>

    Delivered = 4,

    /// <summary>
    /// خوانده شده
    /// </summary>

    Read = 5,

    /// <summary>
    /// پاسخ داده شده
    /// </summary>

    Responded = 6,

    /// <summary>
    /// ناموفق
    /// </summary>

    Failed = 7,

    /// <summary>
    /// لغو شده
    /// </summary>

    Cancelled = 8
}
