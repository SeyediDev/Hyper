namespace Hyper.Domain.Entities.Promotions.Enums;

/// <summary>
/// وضعیت کمپین - وضعیت فعلی کمپین
/// </summary>
[EnumDescription("وضعیت پویش", "PromotionStatus")]
public enum PromotionStatus
{
    /// <summary>
    /// فعال
    /// </summary>
    [Description("فعال")]
    Active = 1,

    /// <summary>
    /// متوقف شده
    /// </summary>
    [Description("متوقف شده")]
    Paused = 2,

    /// <summary>
    /// تکمیل شده
    /// </summary>
    [Description("تکمیل شده")]
    Completed = 3,

    /// <summary>
    /// لغو شده
    /// </summary>
    [Description("لغو شده")]
    Cancelled = 4
}