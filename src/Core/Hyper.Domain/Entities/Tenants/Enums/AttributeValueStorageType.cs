namespace Hyper.Domain.Entities.Tenants.Enums;

public enum AttributeValueStorageType
{
    [Description("بدون ذخیره")]
    None = 0,
    [Description("ذخیره اولین مقدار")]
    First = 1,
    [Description("ذخیره آخرین مقدار")]
    Last = 2,
    [Description("محاسبه مجموع/تعداد/متوسط")]
    SumCountAverage = 3,
    [Description("محاسبه تعداد مقادیر متمایز")]
    CountOfDistinctOfValues = 4,
}