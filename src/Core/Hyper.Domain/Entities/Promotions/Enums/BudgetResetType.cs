namespace Hyper.Domain.Entities.Promotions.Enums;

public enum BudgetResetType
{
    [Description("هر ساعت")]
    Hourly = 1,

    [Description("روزانه")]
    Daily = 2,

    [Description("هفتگی")]
    Weekly = 3,

    [Description("ماهانه")]
    Monthly = 4,

    [Description("سالانه")]
    Yearly = 5
}
