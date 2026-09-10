namespace Hyper.Domain.Entities.Customers.Enums;

/// <summary>
/// نحوه محاسبه جامعه - تعیین روش محاسبه جامعه
/// </summary>
public enum SegmentCalculationMode
{
    /// <summary>
    /// خودکار - بر اساس شرط‌های تعریف شده
    /// </summary>
    Automatic = 1,

    /// <summary>
    /// دستی - توسط کاربر تعریف می‌شود
    /// </summary>
    Manual = 2,

    /// <summary>
    /// ترکیبی - ترکیب خودکار و دستی
    /// </summary>
    Hybrid = 3,

    /// <summary>
    /// هوشمند - بر اساس الگوریتم‌های پیشرفته
    /// </summary>
    Intelligent = 4
}