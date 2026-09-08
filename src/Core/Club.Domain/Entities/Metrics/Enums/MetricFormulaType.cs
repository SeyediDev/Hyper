namespace Hyper.Domain.Entities.Metrics.Enums;

/// <summary>
/// نوع فرمول محاسبه
/// </summary>
public enum MetricFormulaType
{
    /// <summary>
    /// فرمول SQL
    /// </summary>
    SQL = 1,

    /// <summary>
    /// فرمول C#
    /// </summary>
    CSharp = 2,

    /// <summary>
    /// فرمول Python
    /// </summary>
    Python = 3
}

