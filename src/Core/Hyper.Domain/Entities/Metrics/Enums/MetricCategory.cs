namespace Hyper.Domain.Entities.Metrics.Enums;

/// <summary>
/// دسته‌بندی شاخص‌ها
/// </summary>
public enum MetricCategory
{
    /// <summary>
    /// شاخص‌های وفاداری
    /// </summary>
    Loyalty = 1,

    /// <summary>
    /// شاخص‌های RFM
    /// </summary>
    RFM = 2,

    /// <summary>
    /// شاخص‌های CLV
    /// </summary>
    CLV = 3,

    /// <summary>
    /// شاخص‌های اعتباری
    /// </summary>
    Credit = 4,

    /// <summary>
    /// رتبه‌بندی اعتباری
    /// </summary>
    CreditRating = 5,

    /// <summary>
    /// شاخص‌های مالی
    /// </summary>
    Financial = 6,

    /// <summary>
    /// شاخص‌های تعامل
    /// </summary>
    Engagement = 7,

    /// <summary>
    /// شاخص‌های رضایت
    /// </summary>
    Satisfaction = 8,

    /// <summary>
    /// شاخص‌های ریزش
    /// </summary>
    Churn = 9,

    /// <summary>
    /// شاخص‌های محصول
    /// </summary>
    Product = 10,

    /// <summary>
    /// شاخص‌های Tenant
    /// </summary>
    Tenant = 11,

    /// <summary>
    /// شاخص‌های جامعه
    /// </summary>
    Community = 12
}