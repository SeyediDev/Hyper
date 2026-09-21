#nullable enable
namespace Hyper.Domain.Entities.Database;

[DisplayName("مغازه")]
[DbMap("TBL_Shop")]
public sealed class SqlTblShop : SqlServerEntity
{
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("نام کاربری مالک")]
    [DbMap("OWNERID_")]
    public string Ownerid { get; set; } = null!;
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("توضیحات")]
    [DbMap("DESCRIPTION_")]
    public string? Description { get; set; }
    [DisplayName("آدرس")]
    [DbMap("ADDRESSLINE_")]
    public string? Addressline { get; set; }
    [DisplayName("کد پستی")]
    [DbMap("POSTALCODE_")]
    public string? Postalcode { get; set; }
    [DisplayName("ایمیل")]
    [DbMap("EMAIL_")]
    public string? Email { get; set; }
    [DisplayName("شماره تلفن")]
    [DbMap("PHONE_")]
    public string? Phone { get; set; }
    [DisplayName("شماره اقتصادی")]
    [DbMap("ECONOMICCODE_")]
    public string? Economiccode { get; set; }
    [DisplayName("کد ملی / شناسه ملی / کد فراگیر / کد مشارکت مدنی")]
    [DbMap("IDENTIFIERNUMBER_")]
    public string? Identifiernumber { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("شناسه نوع فعالیت")]
    [DbMap("ACTIVITYTYPEID_")]
    public byte? Activitytypeid { get; set; }
    [DisplayName("آدرس نسبی فایل لوگو")]
    [DbMap("LOGORELATIVEURL_")]
    public string? Logorelativeurl { get; set; }
    [DisplayName("شماره موبایل")]
    [DbMap("MOBILE_")]
    public string? Mobile { get; set; }
    [DisplayName("کد شعبه")]
    [DbMap("BRANCHCODE_")]
    public string? Branchcode { get; set; }
    [DisplayName("کشور")]
    [DbMap("COUNTRY_")]
    public string? Country { get; set; }
    [DisplayName("وب سایت")]
    [DbMap("WEBSITE_")]
    public string? Website { get; set; }
    [DisplayName("نوع مالکیت مغازه")]
    [DbMap("TYPE_")]
    public byte Type { get; set; }
    [DisplayName("استان")]
    [DbMap("PROVINCE_")]
    public string? Province { get; set; }
    [DisplayName("شهر")]
    [DbMap("CITY_")]
    public string? City { get; set; }
    [DisplayName("وضعیت")]
    [DbMap("STATUS_")]
    public byte Status { get; set; }
}
[DisplayName("نوع فعالیت فروشگاه")]
[DbMap("TBL_ShopActivityType")]
public sealed class SqlTblShopactivitytype : SqlServerEntity
{
    [DisplayName("شناسه نوع فعالیت مغازه")]
    [DbMap("ACTIVITYTYPEID_")]
    public byte Activitytypeid { get; set; }
    [DisplayName("عنوان")]
    [DbMap("ACTIVITYTYPENAME_")]
    public string Activitytypename { get; set; } = null!;
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
