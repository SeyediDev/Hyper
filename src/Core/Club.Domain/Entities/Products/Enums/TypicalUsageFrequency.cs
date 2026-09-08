namespace Hyper.Domain.Entities.Products.Enums;

/// <summary>
/// فرکانس استفاده معمولی - فرکانس استفاده محصول توسط مشتریان
/// </summary>
public enum TypicalUsageFrequency
{
    [Description("نامشخص")]
    Unknown = 0,
    [Description("یکبار استفاده")]
    OneTimeUse = 1,
    [Description("روزانه")]
    Daily = 2,
    [Description("چند بار در هفته")]
    SeveralTimesPerWeek = 3,
    [Description("هفتگی")]
    Weekly = 4,
    [Description("چند بار در ماه")]
    SeveralTimesPerMonth = 5,
    [Description("ماهانه")]
    Monthly = 6,
    [Description("چند بار در سال")]
    SeveralTimesPerYear = 7,
    [Description("سالانه")]
    Yearly = 8,
    [Description("طبق تقاضا")]
    OnDemand = 9
}
