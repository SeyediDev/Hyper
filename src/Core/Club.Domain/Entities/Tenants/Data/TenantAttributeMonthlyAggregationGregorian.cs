namespace Hyper.Domain.Entities.Tenants.Data;

/// <summary>
/// تجمیع ماهانه مقادیر ویژگی‌ها - تقویم میلادی
/// </summary>
[EntityIndex(nameof(IsDeleted), nameof(TenantId), nameof(AttributeId), nameof(GregorianYear), nameof(GregorianMonth))]
[EntityIndex(nameof(IsDeleted), nameof(TenantId), nameof(AttributeId), nameof(CustomerTenantId), nameof(GregorianYear), nameof(GregorianMonth))]
[EntityIndex(nameof(IsDeleted), nameof(TenantId), nameof(GregorianYear), nameof(GregorianMonth))]
[EntityIndex(nameof(IsDeleted), nameof(AttributeId), nameof(GregorianYear), nameof(GregorianMonth))]
[SBVR(SBVRModality.Permitted, "تجمیع ماهانه ویژگی - میلادی", "مقادیر تجمیع شده ماهانه برای هر ویژگی بر اساس تقویم میلادی")]
[DisplayName("تجمیع ماهانه ویژگی (میلادی)")]
public class TenantAttributeMonthlyAggregationGregorian : HyperBaseCoreLogAuditableEntity<long>
{
    public int TenantId { get; set; }
    [DisplayName("اکوسیستم")]
    public Tenant Tenant { get; set; } = null!;

    public int AttributeId { get; set; }
    [DisplayName("ویژگی")]
    public TenantAttribute Attribute { get; set; } = null!;

    public int? CustomerTenantId { get; set; }
    [DisplayName("مشتری")]
    public CustomerTenant? CustomerTenant { get; set; }

    public int? SegmentId { get; set; }
    [DisplayName("جامعه/بازار")]
    public CustomerSegment? Segment { get; set; }

    public int? ProductCategoryId { get; set; }
    [DisplayName("دسته‌بندی محصول")]
    public ProductCategory? ProductCategory { get; set; }

    public int? ProductId { get; set; }
    [DisplayName("محصول")]
    public Product? Product { get; set; }

    public int? EventTypeId { get; set; }
    [DisplayName("رویداد")]
    public EventType? EventType { get; set; }

    public int? ChannelId { get; set; }
    [DisplayName("کانال")]
    public EventChannel? Channel { get; set; }

    [DisplayName("سال میلادی")]
    public int GregorianYear { get; set; }

    [DisplayName("ماه میلادی")]
    public int GregorianMonth { get; set; }

    [DisplayName("مجموع")]
    public decimal Sum { get; set; }

    [DisplayName("تعداد")]
    public long Count { get; set; }

    [DisplayName("میانگین")]
    public decimal Average { get; set; }

    [DisplayName("حداقل")]
    public decimal? MinValue { get; set; }

    [DisplayName("حداکثر")]
    public decimal? MaxValue { get; set; }

    [DisplayName("اولین مقدار")]
    [MaxLength(512)]
    public string? FirstValue { get; set; }

    [DisplayName("آخرین مقدار")]
    [MaxLength(512)]
    public string? LastValue { get; set; }

    [DisplayName("تعداد مقادیر متمایز")]
    public long DistinctCount { get; set; }

    [DisplayName("تعداد روزهای تجمیع شده")]
    public int DaysCount { get; set; }

    [DisplayName("تاریخ شروع ماه")]
    public DateTime MonthStartDate { get; set; }

    [DisplayName("تاریخ پایان ماه")]
    public DateTime MonthEndDate { get; set; }

    [DisplayName("تاریخ تجمیع")]
    public DateTime AggregationDate { get; set; }

    [DisplayName("وضعیت")]
    public AggregationStatus Status { get; set; } = AggregationStatus.Completed;
}
