namespace Hyper.Domain.Entities.Analytics.Enums;

/// <summary>
/// دوره تحلیل - تعیین دوره زمانی تحلیل
/// </summary>
public enum AnalysisPeriod
{
    /// <summary>
    /// روزانه
    /// </summary>
    [Display(Name = "روزانه")]
    Daily = 1,

    /// <summary>
    /// هفتگی
    /// </summary>
    [Display(Name = "هفتگی")]
    Weekly = 2,

    /// <summary>
    /// ماهانه
    /// </summary>
    [Display(Name = "ماهانه")]
    Monthly = 3,

    /// <summary>
    /// فصلی
    /// </summary>
    [Display(Name = "فصلی")]
    Quarterly = 4,

    /// <summary>
    /// سالانه
    /// </summary>
    [Display(Name = "سالانه")]
    Yearly = 5,

    /// <summary>
    /// سفارشی
    /// </summary>
    [Display(Name = "سفارشی")]
    Custom = 6
}
