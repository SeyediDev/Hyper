namespace Hyper.Domain.Entities.Customers.Enums;

/// <summary>
/// نوع شرط جامعه/بازار مشتریان - تعیین نحوه ارزیابی شرط
/// </summary>
public enum CustomerSegmentConditionKind
{
    /// <summary>
    /// بدون شرط اضافی
    /// </summary>

    WithoutExtraCondition = 0,

    /// <summary>
    /// فرمول شرط
    /// </summary>

    Formula = 1,

    /// <summary>
    /// برابر
    /// </summary>

    EqualTo = 2,

    /// <summary>
    /// نابرابر
    /// </summary>

    NotEqualTo = 3,

    /// <summary>
    /// بزرگتر
    /// </summary>

    GreaterThan = 4,

    /// <summary>
    /// کوچکتر
    /// </summary>

    LessThan = 5,

    /// <summary>
    /// بزرگتر مساوی
    /// </summary>

    GreaterThanOrEqualTo = 6,

    /// <summary>
    /// کوچکتر مساوی
    /// </summary>

    LessThanOrEqualTo = 7,

    /// <summary>
    /// شامل
    /// </summary>

    Contains = 8,

    /// <summary>
    /// شروع با
    /// </summary>

    StartsWith = 9,

    /// <summary>
    /// پایان با
    /// </summary>

    EndsWith = 10,

    /// <summary>
    /// در بازه
    /// </summary>

    InRange = 11,

    /// <summary>
    /// خارج از بازه
    /// </summary>

    OutOfRange = 12,

    /// <summary>
    /// در لیست
    /// </summary>

    InList = 13,

    /// <summary>
    /// خارج از لیست
    /// </summary>

    NotInList = 14,

    /// <summary>
    /// خالی
    /// </summary>

    IsNull = 15,

    /// <summary>
    /// غیرخالی
    /// </summary>

    IsNotNull = 16
}
