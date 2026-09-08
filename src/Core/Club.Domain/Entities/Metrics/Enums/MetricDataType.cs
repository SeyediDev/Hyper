namespace Hyper.Domain.Entities.Metrics.Enums;

/// <summary>
/// نوع داده شاخص
/// </summary>
public enum MetricDataType
{
    /// <summary>
    /// اعشاری
    /// </summary>
    Decimal = 1,

    /// <summary>
    /// عددی صحیح
    /// </summary>
    Integer = 2,

    /// <summary>
    /// رشته‌ای
    /// </summary>
    String = 3,

    /// <summary>
    /// بولی
    /// </summary>
    Boolean = 4,

    /// <summary>
    /// تاریخ
    /// </summary>
    Date = 5
}

