#nullable enable
namespace Hyper.Domain.Entities.Database;

[DisplayName("تنظیمات سامانه مودیان")]
[DbMap("TBL_TaxpayerPortalConfig")]
public sealed class SqlTblTaxpayerportalconfig : SqlServerEntity
{
    [DisplayName("شناسه تنظیمات")]
    [DbMap("CONFIGID_")]
    public int Configid { get; set; }
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("اتصال به سامانه مودیان")]
    [DbMap("ISTAXPAYERPORTALENABLED_")]
    public bool Istaxpayerportalenabled { get; set; }
    [DisplayName("شناسه یکتای حافظه مالیاتی")]
    [DbMap("TAXMEMORYUNIQUEID_")]
    public string? Taxmemoryuniqueid { get; set; }
    [DisplayName("کلید خصوصی")]
    [DbMap("PRIVATEKEY_")]
    public string? Privatekey { get; set; }
    [DisplayName("گواهی امضای دیجیتال")]
    [DbMap("DIGITALSIGNATURECERTIFICATE_")]
    public string? Digitalsignaturecertificate { get; set; }
    [DisplayName("الگوی صورتحساب")]
    [DbMap("INVOICEFORMAT_")]
    public byte Invoiceformat { get; set; }
    [DisplayName("نوع صورتحساب پیشفرض")]
    [DbMap("INVOICETYPE_")]
    public byte? Invoicetype { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
