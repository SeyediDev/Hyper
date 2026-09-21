#nullable enable
namespace Hyper.Domain.Entities.Database;

[DisplayName("برچسب")]
[DbMap("TBL_Tag")]
public sealed class SqlTblTag : SqlServerEntity<int>
{
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("مقدار برچسب")]
    [DbMap("VALUE_")]
    public string Value { get; set; } = null!;
    [DisplayName("نوع موجودیت")]
    [DbMap("ENTITYTYPE_")]
    public byte Entitytype { get; set; }
    [DisplayName("توضیحات")]
    [DbMap("DESCRIPTION_")]
    public string? Description { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
