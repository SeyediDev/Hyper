namespace Hyper.Domain.Entities.Tenants;

/// <summary>
/// مقادیر مجاز برای ویژگی
/// </summary>
[DisplayName("مقدار مجاز ویژگی")]
[EntityIndex($"{nameof(TenantAttributeId)},{nameof(Value)},{nameof(IsDeleted)}")]
public class TenantAttributeAllowedValue : HyperBaseCoreConfigAuditableEntity<int>
{
    public int TenantAttributeId { get; set; }
    [DisplayName("ویژگی")]
    public TenantAttribute TenantAttribute { get; set; } = null!;

    [DisplayName("مقدار مجاز")]
    [MaxLength(512)]
    public string Value { get; set; } = null!;

    [DisplayName("عنوان")]
    [MaxLength(61)]
    public string? Title { get; set; }

    [DisplayName("ترتیب نمایش")]
    public int DisplayOrder { get; set; }

    [DisplayName("فعال")]
    public bool IsActive { get; set; } = true;
}