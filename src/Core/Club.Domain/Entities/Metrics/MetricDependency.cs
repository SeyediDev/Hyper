namespace Hyper.Domain.Entities.Metrics;

/// <summary>
/// وابستگی شاخص - تعریف وابستگی بین شاخص‌ها
/// </summary>
[DisplayName("وابستگی شاخص")]
[SBVR(SBVRModality.Recommended, "وابستگی شاخص", "وابستگی برای تعیین ترتیب محاسبه و حل وابستگی‌ها استفاده می‌شود")]
public class MetricDependency : HyperBaseCoreAuditableEntity<int>
{
    public int MetricDefinitionId { get; set; }
    [DisplayName("شاخص")]
    [InDisplayString]
    public MetricDefinition MetricDefinition { get; set; } = null!;

    public int DependsOnMetricDefinitionId { get; set; }
    [DisplayName("وابسته به")]
    [InDisplayString]
    public MetricDefinition DependsOnMetricDefinition { get; set; } = null!;

    /// <summary>
    /// آیا وابستگی اجباری است؟
    /// </summary>
    [DisplayName("اجباری")]
    [SBVR(SBVRModality.Recommended, "وابستگی اجباری", "وابستگی اجباری باید قبل از محاسبه شاخص محاسبه شود")]
    public bool IsRequired { get; set; } = true;
}

