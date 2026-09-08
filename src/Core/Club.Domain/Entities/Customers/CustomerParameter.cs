namespace Hyper.Domain.Entities.Customers;

/// <summary>
/// می خواهیم یه سری پارامتر شناور به ازای هر مشتری ذخیره کنیم
/// </summary>
[DisplayName("پارامتر مشتری")]
public class CustomerParameter : HyperBaseCoreAuditableEntity<int>
{
    [DisplayName("کلید")]
    [InDisplayString]
    [MaxLength(40)]
    public string Key { get; set; } = null!;

    [DisplayName("عنوان")]
    [InDisplayString]
    [MaxLength(41)]
    public string Title { get; set; } = null!;

    [DisplayName("نوع پارامتر")]
    public ParameterType ParameterType { get; set; }
    public int TenantId { get; set; }
    [DisplayName("اکوسیستم")]
    public Tenant Tenant { get; set; } = null!;
}
