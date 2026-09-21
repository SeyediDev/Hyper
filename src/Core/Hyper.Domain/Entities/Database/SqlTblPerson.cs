#nullable enable
namespace Hyper.Domain.Entities.Database;

[DisplayName("شخص")]
[DbMap("TBL_Person")]
public sealed class SqlTblPerson : SqlServerEntity<int>
{
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("شناسه حساب تفصیلی")]
    [DbMap("DETAILACCOUNTID_")]
    public long Detailaccountid { get; set; }
    [DisplayName("نام شخص")]
    [DbMap("NAME_")]
    public string? Name { get; set; }
    [DisplayName("نام خانوادگی شخص")]
    [DbMap("LASTNAME_")]
    public string? Lastname { get; set; }
    [DisplayName("نام کسب‌وکار")]
    [DbMap("COMPANYNAME_")]
    public string? Companyname { get; set; }
    [DisplayName("نام مستعار")]
    [DbMap("NICKNAME_")]
    public string Nickname { get; set; } = null!;
    [DisplayName("نوع شخص")]
    [DbMap("TYPE_")]
    public byte Type { get; set; }
    [DisplayName("فعال بودن")]
    [DbMap("ISENABLED_")]
    public bool Isenabled { get; set; }
    [DisplayName("کد ملی / شناسه ملی / کد فراگیر / کد مشارکت مدنی")]
    [DbMap("IDENTIFIERNUMBER_")]
    public string? Identifiernumber { get; set; }
    [DisplayName("شماره اقتصادی")]
    [DbMap("ECONOMICCODE_")]
    public string? Economiccode { get; set; }
    [DisplayName("کد شعبه")]
    [DbMap("BRANCHCODE_")]
    public string? Branchcode { get; set; }
    [DisplayName("میزان اعتبار")]
    [DbMap("CREDITLIMIT_")]
    public decimal? Creditlimit { get; set; }
    [DisplayName("مسیر تصویر پروفایل")]
    [DbMap("PROFILEIMAGEURL_")]
    public string? Profileimageurl { get; set; }
    [DisplayName("اطلاعات تماس")]
    [DbMap("CONTACTINFO_")]
    public string? Contactinfo { get; set; }
    [DisplayName("تاریخ‌های خاص")]
    [DbMap("SPECIALDATES_")]
    public string? Specialdates { get; set; }
    [DisplayName("اطلاعات حساب‌های بانکی")]
    [DbMap("BANKACCOUNTSINFO_")]
    public string? Bankaccountsinfo { get; set; }
    [DisplayName("اطلاعات آدرس‌ها")]
    [DbMap("ADDRESSESINFO_")]
    public string? Addressesinfo { get; set; }
    [DisplayName("نقش‌ها")]
    [DbMap("ROLES_")]
    public string? Roles { get; set; }
    [DisplayName("شماره موبایل")]
    [DbMap("MOBILENUMBER_")]
    public string? Mobilenumber { get; set; }
    [DisplayName("شماره گذرنامه")]
    [DbMap("PASSPORTNUMBER_")]
    public string? Passportnumber { get; set; }
    [DisplayName("شماره قرارداد پیمانکاری")]
    [DbMap("CONTRACTNUMBER_")]
    public string? Contractnumber { get; set; }
    [DisplayName("شماره اشتراک/شناسه قبض")]
    [DbMap("SUBSCRIPTIONNUMBER_")]
    public string? Subscriptionnumber { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
