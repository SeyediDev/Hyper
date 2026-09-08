namespace Hyper.Domain.Entities.Tenants.Enums;

public enum TenantAttributeType
{
    [Description("متن")]
    String = 1,

    [Description("عدد صحیح")]
    Integer = 2,

    [Description("عدد اعشاری")]
    Decimal = 3,

    [Description("بولی")]
    Boolean = 4,

    [Description("تاریخ و زمان")]
    DateTime = 5,

    [Description("تاریخ")]
    DateOnly = 6,

    [Description("زمان")]
    TimeOnly = 7
}