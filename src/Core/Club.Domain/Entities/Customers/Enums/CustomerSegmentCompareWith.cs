namespace Hyper.Domain.Entities.Customers.Enums;

/// <summary>
/// مقایسه شود با - تعیین نوع مقایسه
/// </summary>
public enum CustomerSegmentCompareWith
{
    /// <summary>
    /// مقدار ثابت
    /// </summary>

    ConstantValue = 1,

    /// <summary>
    /// پارامتر مشتری
    /// </summary>

    CustomerParameter = 2,

    /// <summary>
    /// امتیاز مشتری
    /// </summary>

    CustomerPoint = 3,

    /// <summary>
    /// سطح امتیاز مشتری
    /// </summary>

    CustomerPointLevel = 4,

    /// <summary>
    /// تاریخ عضویت مشتری
    /// </summary>

    CustomerJoinDate = 5,

    /// <summary>
    /// آخرین فعالیت مشتری
    /// </summary>

    CustomerLastActivity = 6,

    /// <summary>
    /// تعداد خرید مشتری
    /// </summary>

    CustomerPurchaseCount = 7,

    /// <summary>
    /// مبلغ کل خرید مشتری
    /// </summary>

    CustomerTotalPurchase = 8,

    /// <summary>
    /// میانگین خرید مشتری
    /// </summary>

    CustomerAveragePurchase = 9,

    /// <summary>
    /// سن مشتری
    /// </summary>

    CustomerAge = 10,

    /// <summary>
    /// جنسیت مشتری
    /// </summary>

    CustomerGender = 11,

    /// <summary>
    /// شهر مشتری
    /// </summary>

    CustomerCity = 12,

    /// <summary>
    /// فرمول محاسباتی
    /// </summary>

    Formula = 13
}
