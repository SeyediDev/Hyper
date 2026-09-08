namespace Hyper.Domain.Entities.Points.Enums;
public enum PointBudgetKind
{
    [Description("روزانه")]
    Daily = 1,
    [Description("هفتگی")]
    Weekly = 2,
    [Description("ماهانه")]
    Monthly = 3,
    [Description("سالانه")]
    Yearly = 4,
    [Description("در طول دوره")]
    InPeriod = 5,
}
