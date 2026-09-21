#nullable enable
namespace Hyper.Domain.Entities.Database;

[DisplayName("محصول")]
[DbMap("TBL_Product")]
public sealed class SqlTblProduct : SqlServerEntity<int>
{
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("شناسه محصول هایپریک")]
    [DbMap("GLOBALID_")]
    public int? Globalid { get; set; }
    [DisplayName("شناسه یکتای مالیاتی")]
    [DbMap("TAXCODE_")]
    public string? Taxcode { get; set; }
    [DisplayName("نام محصول")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("فعال/غیرفعال بودن")]
    [DbMap("ISENABLED_")]
    public bool Isenabled { get; set; }
    [DisplayName("کد واحد اول")]
    [DbMap("BASEUNIT_")]
    public short Baseunit { get; set; }
    [DisplayName("کد واحد دوم")]
    [DbMap("SECONDARYUNIT_")]
    public short? Secondaryunit { get; set; }
    [DisplayName("ضریب تبدیل واحد")]
    [DbMap("UNITCONVERSIONFACTOR_")]
    public decimal? Unitconversionfactor { get; set; }
    [DisplayName("قیمت خرید")]
    [DbMap("PURCHASEPRICE_")]
    public decimal Purchaseprice { get; set; }
    [DisplayName("نرخ مالیات خرید")]
    [DbMap("PURCHASETAXRATE_")]
    public decimal Purchasetaxrate { get; set; }
    [DisplayName("قیمت فروش")]
    [DbMap("SALEPRICE_")]
    public decimal Saleprice { get; set; }
    [DisplayName("نرخ مالیات فروش")]
    [DbMap("SALETAXRATE_")]
    public decimal Saletaxrate { get; set; }
    [DisplayName("تخفیف درصدی")]
    [DbMap("PERCENTDISCOUNT_")]
    public decimal Percentdiscount { get; set; }
    [DisplayName("تخفیف ثابت")]
    [DbMap("FIXEDDISCOUNT_")]
    public decimal Fixeddiscount { get; set; }
    [DisplayName("آیا قابل مصرف است")]
    [DbMap("ISCONSUMABLE_")]
    public bool? Isconsumable { get; set; }
    [DisplayName("آیا قابل خرید است")]
    [DbMap("ISPURCHASABLE_")]
    public bool Ispurchasable { get; set; }
    [DisplayName("آیا قابل انبار است")]
    [DbMap("ISSTOCKABLE_")]
    public bool Isstockable { get; set; }
    [DisplayName("آیا قابل فروش است")]
    [DbMap("ISSELLABLE_")]
    public bool Issellable { get; set; }
    [DisplayName("آیا به‌صورت اینترنتی قابل فروش است")]
    [DbMap("ISONLINESELLABLE_")]
    public bool Isonlinesellable { get; set; }
    [DisplayName("آیا خدمت است")]
    [DbMap("ISSERVICE_")]
    public bool Isservice { get; set; }
    [DisplayName("آیا کالا سریال پذیر است")]
    [DbMap("ISSERIALIZED_")]
    public bool? Isserialized { get; set; }
    [DisplayName("شناسه حساب تفصیلی")]
    [DbMap("DETAILACCOUNTID_")]
    public long Detailaccountid { get; set; }
    [DisplayName("مسیر نسبی تصویر")]
    [DbMap("IMAGERELATIVEURL_")]
    public string? Imagerelativeurl { get; set; }
    [DisplayName("حداکثر مقدار فروش")]
    [DbMap("MAXSALESQUANTITY_")]
    public decimal? Maxsalesquantity { get; set; }
    [DisplayName("توضیحات")]
    [DbMap("DESCRIPTION_")]
    public string? Description { get; set; }
    [DisplayName("موجودی حسابداری")]
    [DbMap("ACCOUNTINGSTOCK_")]
    public decimal Accountingstock { get; set; }
    [DisplayName("حداقل موجودی ایمنی")]
    [DbMap("MINIMUMSTOCK_")]
    public decimal? Minimumstock { get; set; }
    [DisplayName("شناسه گروه")]
    [DbMap("GROUPID_")]
    public int? Groupid { get; set; }
    [DisplayName("نقطه سفارش مجدد")]
    [DbMap("REORDERPOINT_")]
    public decimal? Reorderpoint { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("بارکد محصول")]
[DbMap("TBL_ProductBarcode")]
public sealed class SqlTblProductbarcode : SqlServerEntity
{
    [DisplayName("شناسه بارکد محصول")]
    [DbMap("BARCODEID_")]
    public int Barcodeid { get; set; }
    [DisplayName("شناسه محصول")]
    [DbMap("PRODUCTID_")]
    public int Productid { get; set; }
    [DisplayName("بارکد")]
    [DbMap("BARCODE_")]
    public string Barcode { get; set; } = null!;
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("برند محصول")]
[DbMap("TBL_ProductBrand")]
public sealed class SqlTblProductbrand : SqlServerEntity
{
    [DisplayName("شناسه برند محصول")]
    [DbMap("BRANDID_")]
    public short Brandid { get; set; }
    [DisplayName("نام")]
    [DbMap("BRANDNAME_")]
    public string Brandname { get; set; } = null!;
    [DisplayName("توضیحات")]
    [DbMap("BRANDDESCRIPTION_")]
    public string? Branddescription { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("آدرس نسبی فایل لوگو")]
    [DbMap("LOGORELATIVEURL_")]
    public string? Logorelativeurl { get; set; }
}
[DisplayName("وضعیت فراوری محصول")]
[DbMap("TBL_ProductReviewStatus")]
public sealed class SqlTblProductreviewstatus : SqlServerEntity
{
    [DisplayName("شناسه وضعیت فراوری محصول")]
    [DbMap("PRODUCTREVIEWSTATUSID_")]
    public byte Productreviewstatusid { get; set; }
    [DisplayName("عنوان")]
    [DbMap("PRODUCTREVIEWSTATUSNAME_")]
    public string Productreviewstatusname { get; set; } = null!;
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("واحد اندازه‌گیری محصول")]
[DbMap("TBL_ProductUnit")]
public sealed class SqlTblProductunit : SqlServerEntity
{
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("دقت اعشار")]
    [DbMap("DECIMALPRECISION_")]
    public byte Decimalprecision { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("شناسه واحد اندازه‌گیری محصول")]
    [DbMap("UNITCODE_")]
    public short Unitcode { get; set; }
    [DisplayName("نوع واحد اندازه‌گیری")]
    [DbMap("USAGETYPE_")]
    public string Usagetype { get; set; } = null!;
    [DisplayName("شمارش به عنوان یک آیتم")]
    [DbMap("COUNTASSINGLEITEM_")]
    public bool Countassingleitem { get; set; }
    [DisplayName("تبدیل‌ها")]
    [DbMap("CONVERSIONS_")]
    public string? Conversions { get; set; }
}
[DisplayName("گروه محصول هایپریک")]
[DbMap("TBL_GlobalProductGroup")]
public sealed class SqlTblGlobalproductgroup : SqlServerEntity
{
    [DisplayName("شناسه گروه محصول")]
    [DbMap("GROUPID_")]
    public short Groupid { get; set; }
    [DisplayName("شناسه والد")]
    [DbMap("PARENTID_")]
    public short? Parentid { get; set; }
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("رسانه محصول")]
[DbMap("TBL_GlobalProductMedia")]
public sealed class SqlTblGlobalproductmedia : SqlServerEntity
{
    [DisplayName("شناسه رسانه محصول")]
    [DbMap("MEDIAID_")]
    public int Mediaid { get; set; }
    [DisplayName("شناسه محصول هایپریک")]
    [DbMap("PRODUCTID_")]
    public int Productid { get; set; }
    [DisplayName("آدرس نسبی فایل")]
    [DbMap("FILERELATIVEURL_")]
    public string? Filerelativeurl { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("محصول هایپریک")]
[DbMap("TBL_GlobalProduct")]
public sealed class SqlTblGlobalproduct : SqlServerEntity
{
    [DisplayName("شناسه محصول هایپریک")]
    [DbMap("GLOBALPRODUCTID_")]
    public int Globalproductid { get; set; }
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("بارکد")]
    [DbMap("BARCODE_")]
    public string? Barcode { get; set; }
    [DisplayName("توضیحات")]
    [DbMap("DESCRIPTION_")]
    public string? Description { get; set; }
    [DisplayName("منتشر شده")]
    [DbMap("ISPUBLISHED_")]
    public bool Ispublished { get; set; }
    [DisplayName("شناسه برند")]
    [DbMap("BRANDID_")]
    public short? Brandid { get; set; }
    [DisplayName("شناسه واحد اندازه‌گیری")]
    [DbMap("UNITCODE_")]
    public short Unitcode { get; set; }
    [DisplayName("فعال است")]
    [DbMap("ISENABLED_")]
    public bool Isenabled { get; set; }
    [DisplayName("تایید شده")]
    [DbMap("ISAPPROVED_")]
    public bool Isapproved { get; set; }
    [DisplayName("شناسه وضعیت فراوری")]
    [DbMap("REVIEWSTATUSID_")]
    public byte Reviewstatusid { get; set; }
    [DisplayName("زمان ایجاد")]
    [DbMap("CREATETIME_")]
    public DateTime Createtime { get; set; }
    [DisplayName("کاربر ایجاد کننده")]
    [DbMap("CREATORUSER_")]
    public string? Creatoruser { get; set; }
    [DisplayName("زمان آخرین ویرایش")]
    [DbMap("UPDATETIME_")]
    public DateTime Updatetime { get; set; }
    [DisplayName("کاربر ویرایش کننده")]
    [DbMap("UPDATERUSER_")]
    public string? Updateruser { get; set; }
    [DisplayName("نرخ مالیات فروش")]
    [DbMap("SALETAXRATE_")]
    public decimal Saletaxrate { get; set; }
    [DisplayName("نرخ مالیات خرید")]
    [DbMap("PURCHASETAXRATE_")]
    public decimal Purchasetaxrate { get; set; }
    [DisplayName("خدمات است")]
    [DbMap("ISSERVICE_")]
    public bool Isservice { get; set; }
    [DisplayName("شناسه گروه")]
    [DbMap("GROUPID_")]
    public short? Groupid { get; set; }
    [DisplayName("آدرس نسبی فایل تصویر")]
    [DbMap("IMAGERELATIVEURL_")]
    public string? Imagerelativeurl { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}