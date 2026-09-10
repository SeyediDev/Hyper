namespace Hyper.Domain.Entities.Promotions.Enums;

/// <summary>
/// نوع پیام پویش - تعیین نوع پیام
/// </summary>
public enum PromotionMessageType
{
    /// <summary>
    /// تبلیغاتی
    /// </summary>
    [Description("تبلیغاتی")]
    Promotional = 1,

    /// <summary>
    /// اطلاع‌رسانی
    /// </summary>
    [Description("اطلاع‌رسانی")]
    Informational = 2,

    /// <summary>
    /// تعاملی
    /// </summary>
    [Description("تعاملی")]
    Interactive = 3,

    /// <summary>
    /// تبریک
    /// </summary>
    [Description("تبریک")]
    Congratulatory = 4,

    /// <summary>
    /// یادآوری
    /// </summary>
    [Description("یادآوری")]
    Reminder = 5,

    /// <summary>
    /// دعوت
    /// </summary>
    [Description("دعوت")]
    Invitation = 6,

    /// <summary>
    /// نظرسنجی
    /// </summary>
    [Description("نظرسنجی")]
    Survey = 7,

    /// <summary>
    /// سفارشی
    /// </summary>
    [Description("سفارشی")]
    Custom = 8
}
