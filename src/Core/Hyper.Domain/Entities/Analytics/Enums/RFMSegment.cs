namespace Hyper.Domain.Entities.Analytics.Enums;

/// <summary>
/// بخش RFM - بخش‌بندی مشتری بر اساس نمره RFM
/// </summary>
public enum RFMSegment
{
    /// <summary>
    /// چمپیون - مشتریان با ارزش بالا و وفادار
    /// </summary>
    [Display(Name = "قهرمان‌ها")]
    Champions = 1,

    /// <summary>
    /// وفاداران - مشتریان وفادار با ارزش متوسط
    /// </summary>
    [Display(Name = "مشتریان وفادار")]
    LoyalCustomers = 2,

    /// <summary>
    /// پتانسیل بالا - مشتریان با پتانسیل رشد بالا
    /// </summary>
    [Display(Name = "پتانسیل وفادار شدن")]
    PotentialLoyalists = 3,

    /// <summary>
    /// جدید - مشتریان جدید
    /// </summary>
    [Display(Name = "مشتریان جدید")]
    NewCustomers = 4,

    /// <summary>
    /// در خطر - مشتریان در خطر ترک
    /// </summary>
    [Display(Name = "در معرض ریزش")]
    AtRisk = 5,

    /// <summary>
    /// نمی‌توان نگه داشت - مشتریان با ارزش پایین
    /// </summary>
    [Display(Name = "نباید از دست بروند")]
    CannotLoseThem = 6,

    /// <summary>
    /// خوابیده - مشتریان غیرفعال
    /// </summary>
    [Display(Name = "خوابیده‌ها")]
    Hibernating = 7,

    /// <summary>
    /// از دست رفته - مشتریان ترک کرده
    /// </summary>
    [Display(Name = "از دست رفته‌ها")]
    Lost = 8
}
