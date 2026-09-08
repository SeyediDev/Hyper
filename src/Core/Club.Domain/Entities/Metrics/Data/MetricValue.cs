namespace Hyper.Domain.Entities.Metrics.Data;

/// <summary>
/// مقدار شاخص - مقدار محاسبه شده یک شاخص برای یک موجودیت خاص
/// </summary>
[DisplayName("مقدار شاخص")]
[SBVR(SBVRModality.Calculated, "مقدار شاخص", "مقدار شاخص بر اساس فرمول تعریف شده محاسبه می‌شود")]
public class MetricValue : HyperBaseCoreAuditableEntity<long>
{
    public int MetricDefinitionId { get; set; }
    [DisplayName("تعریف شاخص")]
    [InDisplayString]
    public MetricDefinition MetricDefinition { get; set; } = null!;

    public int TenantId { get; set; }
    [DisplayName("اکوسیستم")]
    public Tenant Tenant { get; set; } = null!;

    /// <summary>
    /// شناسه مشتری (برای سطح Customer)
    /// </summary>
    [DisplayName("مشتری")]
    public int? CustomerTenantId { get; set; }
    [DisplayName("مشتری")]
    public CustomerTenant? CustomerTenant { get; set; }

    /// <summary>
    /// شناسه محصول (برای سطح Customer-Product)
    /// </summary>
    [DisplayName("محصول")]
    public int? ProductId { get; set; }
    [DisplayName("محصول")]
    public Product? Product { get; set; }

    /// <summary>
    /// مقدار اعشاری
    /// </summary>
    [DisplayName("مقدار اعشاری")]
    public decimal? DecimalValue { get; set; }

    /// <summary>
    /// مقدار عددی صحیح
    /// </summary>
    [DisplayName("مقدار عددی")]
    public long? IntegerValue { get; set; }

    /// <summary>
    /// مقدار رشته‌ای
    /// </summary>
    [MaxLength(500)]
    [DisplayName("مقدار رشته‌ای")]
    public string? StringValue { get; set; }

    /// <summary>
    /// مقدار بولین
    /// </summary>
    [DisplayName("مقدار بولین")]
    public bool? BooleanValue { get; set; }

    /// <summary>
    /// تاریخ محاسبه
    /// </summary>
    [DisplayName("تاریخ محاسبه")]
    [SBVR(SBVRModality.Calculated, "تاریخ محاسبه", "تاریخ محاسبه برای ردیابی به‌روزرسانی‌ها استفاده می‌شود")]
    public DateTime CalculatedAt { get; set; }

    /// <summary>
    /// شروع دوره محاسبه
    /// </summary>
    [DisplayName("شروع دوره")]
    public DateTime? PeriodStart { get; set; }

    /// <summary>
    /// پایان دوره محاسبه
    /// </summary>
    [DisplayName("پایان دوره")]
    public DateTime? PeriodEnd { get; set; }

    /// <summary>
    /// Metadata محاسبه (JSON)
    /// </summary>
    [MaxLength(2000)]
    [DisplayName("اطلاعات محاسبه")]
    public string? CalculationMetadata { get; set; }
}

