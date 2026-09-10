namespace Hyper.Domain.Entities.Promotions.Enums;

public enum PromotionCheckTime : int
{
    [Description("روزانه")]
    Daily = 1,
    [Description("هفتگی")]
    Weekly = 2,
    [Description("ماهانه")]
    Monthly = 3,
    [Description("سالانه")]
    Yearly = 4,
    [Description("انتهای دوره")]
    EndOfPeriod = 5,
}
