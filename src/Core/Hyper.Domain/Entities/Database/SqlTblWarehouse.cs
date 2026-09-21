#nullable enable
namespace Hyper.Domain.Entities.Database;

[DisplayName("انبار")]
[DbMap("TBL_Warehouse")]
public sealed class SqlTblWarehouse : SqlServerEntity<int>
{
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("پیش فرض بودن")]
    [DbMap("ISDEFAULT_")]
    public bool Isdefault { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("فعال بودن")]
    [DbMap("ISENABLED_")]
    public bool Isenabled { get; set; }
    [DisplayName("شناسه حساب تفصیلی")]
    [DbMap("DETAILACCOUNTID_")]
    public long Detailaccountid { get; set; }
    [DisplayName("توضیحات")]
    [DbMap("DESCRIPTION_")]
    public string? Description { get; set; }
    [DisplayName("شناسه انباردار")]
    [DbMap("PERSONID_")]
    public int Personid { get; set; }
    [DisplayName("شماره تماس")]
    [DbMap("PHONE_")]
    public string? Phone { get; set; }
    [DisplayName("آدرس")]
    [DbMap("ADDRESS_")]
    public string? Address { get; set; }
}