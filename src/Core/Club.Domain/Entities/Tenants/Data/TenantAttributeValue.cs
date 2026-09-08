namespace Hyper.Domain.Entities.Tenants.Data;

/// <summary>
/// مقادیر خام ویژگی‌ها - هر رویداد یک رکورد
/// </summary>
[EntityIndex(nameof(IsDeleted), nameof(AttributeId), nameof(CustomerTenantId))]
[EntityIndex(nameof(IsDeleted), nameof(AttributeId), nameof(CustomerTenantId), nameof(SegmentId))]
[EntityIndex(nameof(IsDeleted), nameof(AttributeId), nameof(CustomerTenantId), nameof(ProductCategoryId))]
[EntityIndex(nameof(IsDeleted), nameof(AttributeId), nameof(CustomerTenantId), nameof(ProductId))]
[EntityIndex(nameof(IsDeleted), nameof(AttributeId), nameof(CustomerTenantId), nameof(EventTypeId))]
[EntityIndex(nameof(IsDeleted), nameof(AttributeId), nameof(CustomerTenantId), nameof(ChannelId))]
[EntityIndex(nameof(IsDeleted), nameof(AttributeId), nameof(SegmentId))]
[EntityIndex(nameof(IsDeleted), nameof(AttributeId), nameof(ProductCategoryId))]
[EntityIndex(nameof(IsDeleted), nameof(AttributeId), nameof(ProductId))]
[EntityIndex(nameof(IsDeleted), nameof(AttributeId), nameof(EventTypeId))]
[EntityIndex(nameof(IsDeleted), nameof(AttributeId), nameof(ChannelId))]
[EntityIndex(nameof(IsDeleted), nameof(CustomerTenantId))]
[EntityIndex(nameof(IsDeleted), nameof(CustomerTenantId), nameof(SegmentId))]
[EntityIndex(nameof(IsDeleted), nameof(CustomerTenantId), nameof(ProductCategoryId))]
[EntityIndex(nameof(IsDeleted), nameof(CustomerTenantId), nameof(ProductId))]
[EntityIndex(nameof(IsDeleted), nameof(CustomerTenantId), nameof(EventTypeId))]
[EntityIndex(nameof(IsDeleted), nameof(CustomerTenantId), nameof(ChannelId))]
[EntityIndex(nameof(IsDeleted), nameof(TenantId))]
[EntityIndex(nameof(IsDeleted), nameof(Area))]
[EntityIndex(nameof(IsDeleted), nameof(Area), nameof(Key))]
[EntityIndex(nameof(IsDeleted), nameof(AttributeId))]
[EntityIndex(nameof(IsDeleted), nameof(TenantId), nameof(Area), nameof(AttributeId))]
[EntityIndex(nameof(IsDeleted), nameof(SegmentId))]
[EntityIndex(nameof(IsDeleted), nameof(ProductCategoryId))]
[EntityIndex(nameof(IsDeleted), nameof(ProductId))]
[EntityIndex(nameof(IsDeleted), nameof(EventTypeId))]
[EntityIndex(nameof(IsDeleted), nameof(ChannelId))]
[EntityIndex(nameof(IsDeleted), nameof(EventDate))]
[EntityIndex(nameof(IsDeleted), nameof(AttributeId), nameof(EventDate))]
[EntityIndex(nameof(IsDeleted), nameof(TenantId), nameof(AttributeId), nameof(EventDate))]
[SBVR(SBVRModality.Permitted, "مقدار ویژگی", "مقادیر خام ویژگی های تعریف شده - هر رویداد یک رکورد جداگانه")]
[DisplayName("مقدار ویژگی")]
public class TenantAttributeValue : HyperBaseCoreAuditableEntity<long>
{
    public int TenantId { get; set; }
    [DisplayName("اکوسیستم")]
    [SBVR(SBVRModality.Calculated, "افزونگی", "جهت پرفورمنس این فیلد به صورت افزونه در اینجا نگهداری می شود")]
    public Tenant Tenant { get; set; } = null!;

    [DisplayName("ناحیه کاربرد")]
    [SBVR(SBVRModality.Calculated, "افزونگی", "جهت پرفورمنس این فیلد به صورت افزونه در اینجا نگهداری می شود")]
    public AttributeArea Area { get; set; } = AttributeArea.Tenant;

    [DisplayName("کلید")]
    [Required]
    [MaxLength(100)]
    [SBVR(SBVRModality.Calculated, "افزونگی", "جهت پرفورمنس این فیلد به صورت افزونه در اینجا نگهداری می شود")]
    public string Key { get; set; } = null!;

    [DisplayName("کلید پارامتر")]
    [MaxLength(100)]
    [SBVR(SBVRModality.Calculated, "افزونگی", "جهت پرفورمنس این فیلد به صورت افزونه در اینجا نگهداری می شود")]
    public string? ParamKey { get; set; }

    public int AttributeId { get; set; }
    [DisplayName("ویژگی")]
    public TenantAttribute Attribute { get; set; } = null!;

    [DisplayName("مقدار")]
    [MaxLength(512)]
    public string Value { get; set; } = null!;

    public int? CustomerTenantId { get; set; }
    [DisplayName("مشتری")]
    public CustomerTenant? CustomerTenant { get; set; } = null!;

    public int? SegmentId { get; set; }
    [DisplayName("جامعه/بازار")]
    [SBVR(SBVRModality.Permitted, "محدودیت", "با تعیین جامعه/بازار، این ویژگی فقط برای این جامعه/بازار تعریف می شود")]
    public CustomerSegment? Segment { get; set; }

    public int? ProductCategoryId { get; set; }
    [DisplayName("دسته‌بندی محصول")]
    [SBVR(SBVRModality.Permitted, "محدودیت", "با تعیین دسته‌بندی محصول، این ویژگی فقط برای این دسته‌بندی محصول تعریف می شود")]
    public ProductCategory? ProductCategory { get; set; } = null!;

    public int? ProductId { get; set; }
    [DisplayName("محصول")]
    [SBVR(SBVRModality.Permitted, "محدودیت", "با تعیین محصول، این ویژگی فقط برای این محصول تعریف می شود")]
    public Product? Product { get; set; } = null!;

    public int? EventTypeId { get; set; }
    [DisplayName("رویداد")]
    public EventType? EventType { get; set; } = null!;

    public int? ChannelId { get; set; }
    [DisplayName("کانال")]
    public EventChannel? Channel { get; set; } = null!;

    [FieldIndex]
    public long EventLogId { get; set; }
    [DisplayName("لاگ رویداد")]
    public EventLog EventLog { get; set; } = null!;

    [DisplayName("تاریخ رویداد")]
    public DateTime EventDate { get; set; }
}
