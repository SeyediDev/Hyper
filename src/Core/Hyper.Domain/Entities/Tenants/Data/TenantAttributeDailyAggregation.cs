namespace Hyper.Domain.Entities.Tenants.Data;

/// <summary>
/// تجمیع روزانه مقادیر ویژگی‌ها
/// اطلاعات هر روز در این موجودیت ذخیره می‌شود
/// </summary>
[EntityIndex(nameof(IsDeleted), nameof(TenantId), nameof(AttributeId), nameof(Year), nameof(Month), nameof(Day))]
[EntityIndex(nameof(IsDeleted), nameof(TenantId), nameof(AttributeId), nameof(CustomerTenantId), nameof(Year), nameof(Month), nameof(Day))]
[EntityIndex(nameof(IsDeleted), nameof(TenantId), nameof(Year), nameof(Month), nameof(Day))]
[EntityIndex(nameof(IsDeleted), nameof(AttributeId), nameof(Year), nameof(Month), nameof(Day))]
[SBVR(SBVRModality.Permitted, "تجمیع روزانه ویژگی", "مقادیر تجمیع شده روزانه برای هر ویژگی")]
[DisplayName("تجمیع روزانه ویژگی")]
public class TenantAttributeDailyAggregation : HyperBaseCoreLogAuditableEntity<long>
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

    [DisplayName("سال")]
    public int Year { get; set; }

    [DisplayName("ماه")]
    public int Month { get; set; }

    [DisplayName("روز")]
    public int Day { get; set; }

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

    [DisplayName("تاریخ تجمیع")]
    public DateTime AggregationDate { get; set; }

    [DisplayName("وضعیت")]
    public AggregationStatus Status { get; set; } = AggregationStatus.Completed;

    [DisplayName("تاریخ")]
    public DateTime DateTime { get; set; }
}
