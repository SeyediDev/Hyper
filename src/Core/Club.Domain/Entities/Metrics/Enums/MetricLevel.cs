namespace Hyper.Domain.Entities.Metrics.Enums;

/// <summary>
/// سطح محاسبه شاخص
/// </summary>
public enum MetricLevel
{
    /// <summary>
    /// سطح مشتری
    /// </summary>
    Customer = 1,

    /// <summary>
    /// سطح مشتری-محصول
    /// </summary>
    CustomerProduct = 2,

    /// <summary>
    /// سطح Tenant
    /// </summary>
    Tenant = 3,

    /// <summary>
    /// سطح جامعه
    /// </summary>
    Community = 4
}

