#nullable enable
namespace Hyper.Domain.Entities.Database;
[DisplayName("حساب")]
[DbMap("TBL_Account")]
public sealed class SqlTblAccount : SqlServerEntity
{
    [DisplayName("شناسه حساب")]
    [DbMap("ACCOUNTID_")]
    public int Accountid { get; set; }
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int? Shopid { get; set; }
    [DisplayName("نوع حساب")]
    [DbMap("TYPE_")]
    public bool? Type { get; set; }
    [DisplayName("نوع تفصیل")]
    [DbMap("DETAILTYPE_")]
    public byte? Detailtype { get; set; }
    [DisplayName("ماهیت حساب")]
    [DbMap("NATURE_")]
    public bool? Nature { get; set; }
    [DisplayName("شناسه حساب والد")]
    [DbMap("PARENTID_")]
    public int? Parentid { get; set; }
    [DisplayName("کد حساب")]
    [DbMap("CODE_")]
    public string? Code { get; set; }
    [DisplayName("نام حساب")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("سطح حساب")]
    [DbMap("LEVEL_")]
    public byte Level { get; set; }
    [DisplayName("شناسه حساب مرجع")]
    [DbMap("REFERENCEID_")]
    public int? Referenceid { get; set; }
    [DisplayName("امکان ثبت سند")]
    [DbMap("ISPOSTABLE_")]
    public bool Ispostable { get; set; }
    [DisplayName("کلید حساب در سند خودکار")]
    [DbMap("GLOBALACCOUNTKEY_")]
    public short? Globalaccountkey { get; set; }
    [DisplayName("توضیحات")]
    [DbMap("DESCRIPTION_")]
    public string? Description { get; set; }
    [DisplayName("شناسه دوره مالی")]
    [DbMap("FISCALPERIODID_")]
    public int? Fiscalperiodid { get; set; }
    [DisplayName("نوع گزارش")]
    [DbMap("REPORTTYPE_")]
    public byte? Reporttype { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("آرتیکل حسابداری")]
[DbMap("TBL_AccountingArticle")]
public sealed class SqlTblAccountingarticle : SqlServerEntity<long>
{
    [DisplayName("شناسه سند")]
    [DbMap("DOCUMENTID_")]
    public long Documentid { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("شناسه حساب تفصیلی")]
    [DbMap("DETAILACCOUNTID_")]
    public long? Detailaccountid { get; set; }
    [DisplayName("شرح آتیکل")]
    [DbMap("DESCRIPTION_")]
    public string? Description { get; set; }
    [DisplayName("شناسه حساب")]
    [DbMap("ACCOUNTID_")]
    public int Accountid { get; set; }
    [DisplayName("مبلغ")]
    [DbMap("AMOUNT_")]
    public decimal Amount { get; set; }
    [DisplayName("ارز")]
    [DbMap("CURRENCY_")]
    public string Currency { get; set; } = null!;
    [DisplayName("شناسه چک")]
    [DbMap("CHECKID_")]
    public int? Checkid { get; set; }
    [DisplayName("نرخ تبدیل ارز")]
    [DbMap("EXCHANGERATE_")]
    public decimal Exchangerate { get; set; }
    [DisplayName("نوع آرتیکل (بستانکار)")]
    [DbMap("TYPE_")]
    public bool Type { get; set; }
    [DisplayName("مبلغ به ارز پایه")]
    [DbMap("BASEAMOUNT_")]
    public decimal Baseamount { get; set; }
}
[DisplayName("سند حسابداری")]
[DbMap("TBL_AccountingDocument")]
public sealed class SqlTblAccountingdocument : SqlServerEntity<long>
{
    [DisplayName("توضیحات")]
    [DbMap("DESCRIPTION_")]
    public string? Description { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("شناسه دوره مالی")]
    [DbMap("FISCALPERIODID_")]
    public int Fiscalperiodid { get; set; }
    [DisplayName("مبلغ به ارز پایه")]
    [DbMap("TOTALBASEAMOUNT_")]
    public decimal Totalbaseamount { get; set; }
    [DisplayName("شماره عطف")]
    [DbMap("REFERENCENUMBER_")]
    public int? Referencenumber { get; set; }
    [DisplayName("وضعیت سند")]
    [DbMap("STATUS_")]
    public byte Status { get; set; }
    [DisplayName("شماره سند")]
    [DbMap("NUMBER_")]
    public int? Number { get; set; }
    [DisplayName("تاریخ سند")]
    [DbMap("DATE_")]
    public DateOnly Date { get; set; }
    [DisplayName("نوع سند")]
    [DbMap("PRIMARYTYPE_")]
    public byte Primarytype { get; set; }
    [DisplayName("نوع مرجع رویداد مالی")]
    [DbMap("REFERENCETYPE_")]
    public byte Referencetype { get; set; }
    [DisplayName("شناسه مرجع رویداد مالی")]
    [DbMap("REFERENCEID_")]
    public long? Referenceid { get; set; }
    [DisplayName("زمان سند")]
    [DbMap("DATETIME_")]
    public DateTime Datetime { get; set; }
    [DisplayName("ماه شمسی")]
    [DbMap("MONTH_")]
    public int Month { get; set; }
    [DisplayName("شناسه پروژه")]
    [DbMap("PROJECTID_")]
    public int? Projectid { get; set; }
    [DisplayName("تفکیک اسناد")]
    [DbMap("SECONDARYTYPE_")]
    public byte Secondarytype { get; set; }
}
[DisplayName("لاگ فرایند خودکار")]
[DbMap("TBL_AutoProcessLog")]
public sealed class SqlTblAutoprocesslog : SqlServerEntity
{
    [DisplayName("شناسه لاگ فرایند خودکار")]
    [DbMap("LOGID_")]
    public long Logid { get; set; }
    [DisplayName("نام فرایند")]
    [DbMap("PROCESSNAME_")]
    public string? Processname { get; set; }
    [DisplayName("کلید فرایند")]
    [DbMap("PROCESSKEY_")]
    public string? Processkey { get; set; }
    [DisplayName("نسخه فرایند")]
    [DbMap("PROCESSVERSION_")]
    public short? Processversion { get; set; }
    [DisplayName("تنظیم اجرا")]
    [DbMap("TRIGGERCONFIG_")]
    public string? Triggerconfig { get; set; }
    [DisplayName("زمان پایان")]
    [DbMap("ENDTIME_")]
    public DateTime? Endtime { get; set; }
    [DisplayName("زمان شروع")]
    [DbMap("STARTTIME_")]
    public DateTime? Starttime { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("حساب بانکی")]
[DbMap("TBL_BankAccount")]
public sealed class SqlTblBankaccount : SqlServerEntity<int>
{
    [DisplayName("نام حساب بانکی")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("شماره حساب")]
    [DbMap("NUMBER_")]
    public string Number { get; set; } = null!;
    [DisplayName("شماره شبا")]
    [DbMap("IBAN_")]
    public string? Iban { get; set; }
    [DisplayName("نام شعبه")]
    [DbMap("BANKBRANCHNAME_")]
    public string? Bankbranchname { get; set; }
    [DisplayName("صاحب حساب")]
    [DbMap("OWNERNAME_")]
    public string? Ownername { get; set; }
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("شماره کارت")]
    [DbMap("CARDNUMBER_")]
    public string? Cardnumber { get; set; }
    [DisplayName("فعال بودن")]
    [DbMap("ISENABLED_")]
    public bool Isenabled { get; set; }
    [DisplayName("پیش فرض بودن")]
    [DbMap("ISDEFAULT_")]
    public bool Isdefault { get; set; }
    [DisplayName("توضیحات")]
    [DbMap("DESCRIPTION_")]
    public string? Description { get; set; }
    [DisplayName("ارز")]
    [DbMap("CURRENCY_")]
    public string Currency { get; set; } = null!;
    [DisplayName("بانک")]
    [DbMap("BANK_")]
    public string Bank { get; set; } = null!;
    [DisplayName("شناسه حساب تفصیلی")]
    [DbMap("DETAILACCOUNTID_")]
    public long Detailaccountid { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("صندوق نقدی")]
[DbMap("TBL_CashFund")]
public sealed class SqlTblCashfund : SqlServerEntity<int>
{
    [DisplayName("نام صندوق نقدی")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("توضیحات")]
    [DbMap("DESCRIPTION_")]
    public string? Description { get; set; }
    [DisplayName("فعال بودن")]
    [DbMap("ISENABLED_")]
    public bool Isenabled { get; set; }
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("پیش‌فرض بودن")]
    [DbMap("ISDEFAULT_")]
    public bool Isdefault { get; set; }
    [DisplayName("ارز")]
    [DbMap("CURRENCY_")]
    public string Currency { get; set; } = null!;
    [DisplayName("شناسه حساب تفصیلی")]
    [DbMap("DETAILACCOUNTID_")]
    public long Detailaccountid { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("چک")]
[DbMap("TBL_Check")]
public sealed class SqlTblCheck : SqlServerEntity
{
    [DisplayName("شناسه چک")]
    [DbMap("CHECKID_")]
    public int Checkid { get; set; }
    [DisplayName("شماره چک")]
    [DbMap("SERIALNUMBER_")]
    public string Serialnumber { get; set; } = null!;
    [DisplayName("تاریخ سررسید چک")]
    [DbMap("DUEDATE_")]
    public DateOnly Duedate { get; set; }
    [DisplayName("مبلغ چک")]
    [DbMap("AMOUNT_")]
    public decimal Amount { get; set; }
    [DisplayName("وضعیت چک")]
    [DbMap("STATUS_")]
    public byte Status { get; set; }
    [DisplayName("تاریخ و زمان آخرین تغییر وضعیت")]
    [DbMap("STATUSDATETIME_")]
    public DateTime Statusdatetime { get; set; }
    [DisplayName("شماره صیاد")]
    [DbMap("SAYADNUMBER_")]
    public string? Sayadnumber { get; set; }
    [DisplayName("توضیحات")]
    [DbMap("DESCRIPTION_")]
    public string? Description { get; set; }
    [DisplayName("طرف حساب")]
    [DbMap("PAYEE_")]
    public string? Payee { get; set; }
    [DisplayName("شناسه ملی طرف حساب")]
    [DbMap("PAYEENATIONALID_")]
    public string? Payeenationalid { get; set; }
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("شناسه حساب بانکی")]
    [DbMap("BANKACCOUNTID_")]
    public int? Bankaccountid { get; set; }
    [DisplayName("تاریخ و زمان ثبت چک")]
    [DbMap("ISSUEDATE_")]
    public DateTime Issuedate { get; set; }
    [DisplayName("شعبه بانک")]
    [DbMap("BANKBRANCH_")]
    public string? Bankbranch { get; set; }
    [DisplayName("بانک")]
    [DbMap("BANK_")]
    public string? Bank { get; set; }
    [DisplayName("شناسه شخص")]
    [DbMap("PERSONID_")]
    public int Personid { get; set; }
    [DisplayName("نوع تراکنش")]
    [DbMap("ISRECEIPT_")]
    public bool Isreceipt { get; set; }
    [DisplayName("ارز")]
    [DbMap("CURRENCY_")]
    public string Currency { get; set; } = null!;
    [DisplayName("نرخ تبدیل ارز")]
    [DbMap("EXCHANGERATE_")]
    public decimal Exchangerate { get; set; }
    [DisplayName("شناسه پروژه")]
    [DbMap("PROJECTID_")]
    public int? Projectid { get; set; }
    [DisplayName("شناسه دوره مالی")]
    [DbMap("FISCALPERIODID_")]
    public int Fiscalperiodid { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("تراکنش‌های کیف پول")]
[DbMap("TBL_CreditTransaction")]
public sealed class SqlTblCredittransaction : SqlServerEntity<int>
{
    [DisplayName("نام کاربری")]
    [DbMap("USERID_")]
    public string Userid { get; set; } = null!;
    [DisplayName("مبلغ")]
    [DbMap("AMOUNT_")]
    public decimal Amount { get; set; }
    [DisplayName("نوع تراکنش")]
    [DbMap("TYPE_")]
    public byte Type { get; set; }
    [DisplayName("شناسه مرجع")]
    [DbMap("REFERENCEID_")]
    public int Referenceid { get; set; }
    [DisplayName("توضیحات")]
    [DbMap("DESCRIPTION_")]
    public string? Description { get; set; }
    [DisplayName("زمان ایجاد")]
    [DbMap("DATETIME_")]
    public DateTime Datetime { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("حساب تفصیلی")]
[DbMap("TBL_DetailAccount")]
public sealed class SqlTblDetailaccount : SqlServerEntity
{
    [DisplayName("شناسه حساب تفصیلی")]
    [DbMap("DETAILACCOUNTID_")]
    public long Detailaccountid { get; set; }
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("نوع موجودیت")]
    [DbMap("ENTITYTYPE_")]
    public string Entitytype { get; set; } = null!;
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("شناسه مرجع تفصیل")]
    [DbMap("REFERENCEID_")]
    public int? Referenceid { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("فایل‌ها")]
[DbMap("TBL_EntityFile")]
public sealed class SqlTblEntityfile : SqlServerEntity<int>
{
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int? Shopid { get; set; }
    [DisplayName("نوع موجودیت")]
    [DbMap("ENTITYTYPE_")]
    public byte Entitytype { get; set; }
    [DisplayName("شناسه موجودیت")]
    [DbMap("ENTITYID_")]
    public int Entityid { get; set; }
    [DisplayName("نام اصلی فایل")]
    [DbMap("ORIGINALNAME_")]
    public string Originalname { get; set; } = null!;
    [DisplayName("نام منحصر به فرد")]
    [DbMap("UNIQUENAME_")]
    public string Uniquename { get; set; } = null!;
    [DisplayName("اندازه فایل به بایت")]
    [DbMap("SIZE_")]
    public long Size { get; set; }
    [DisplayName("مسیر نسبی دانلود فایل")]
    [DbMap("RELATIVEURL_")]
    public string Relativeurl { get; set; } = null!;
    [DisplayName("نوع محتوای فایل")]
    [DbMap("TYPE_")]
    public string Type { get; set; } = null!;
    [DisplayName("زمان ایجاد")]
    [DbMap("CREATEDAT_")]
    public DateTime Createdat { get; set; }
    [DisplayName("توضیحات")]
    [DbMap("DESCRIPTION_")]
    public string? Description { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("یادداشت موجودیت")]
[DbMap("TBL_EntityNote")]
public sealed class SqlTblEntitynote : SqlServerEntity<long>
{
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("شناسه موجودیت")]
    [DbMap("ENTITYID_")]
    public long Entityid { get; set; }
    [DisplayName("نوع موجودیت")]
    [DbMap("ENTITYTYPE_")]
    public byte Entitytype { get; set; }
    [DisplayName("یادداشت")]
    [DbMap("NOTE_")]
    public string Note { get; set; } = null!;
    [DisplayName("یادداشت عمومی")]
    [DbMap("ISPUBLIC_")]
    public bool Ispublic { get; set; }
    [DisplayName("کاربر ایجاد کننده")]
    [DbMap("CREATEDBY_")]
    public string Createdby { get; set; } = null!;
    [DisplayName("زمان ایجاد")]
    [DbMap("CREATEDTIME_")]
    public DateTime Createdtime { get; set; }
    [DisplayName("زمان آخرین ویرایش")]
    [DbMap("UPDATEDTIME_")]
    public DateTime? Updatedtime { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("تنظیمات کلی فروشگاه")]
[DbMap("TBL_GeneralConfig")]
public sealed class SqlTblGeneralconfig : SqlServerEntity
{
    [DisplayName("شناسه تنظیمات")]
    [DbMap("CONFIGID_")]
    public int Configid { get; set; }
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("ارز پایه")]
    [DbMap("BASECURRENCY_")]
    public string Basecurrency { get; set; } = null!;
    [DisplayName("منطقه زمانی")]
    [DbMap("TIMEZONE_")]
    public string Timezone { get; set; } = null!;
    [DisplayName("نوع تقویم")]
    [DbMap("CALENDARTYPE_")]
    public string Calendartype { get; set; } = null!;
    [DisplayName("نرخ مالیات بر ارزش افزوده پیشفرض")]
    [DbMap("DEFAULTVATRATE_")]
    public decimal? Defaultvatrate { get; set; }
    [DisplayName("چند ارزی")]
    [DbMap("ISMULTICURRENCYENABLED_")]
    public bool Ismulticurrencyenabled { get; set; }
    [DisplayName("سایر ارزها")]
    [DbMap("OTHERCURRENCIES_")]
    public string? Othercurrencies { get; set; }
    [DisplayName("ویرایش تراز افتتاحیه")]
    [DbMap("ALLOWEDITOPENINGBALANCE_")]
    public bool Alloweditopeningbalance { get; set; }
    [DisplayName("تایید خودکار اسناد سیستمی")]
    [DbMap("AUTOAPPROVESYSTEMGENERATEDDOCUMENTS_")]
    public bool Autoapprovesystemgenerateddocuments { get; set; }
    [DisplayName("فعال بودن انبار")]
    [DbMap("ISWAREHOUSEENABLED_")]
    public bool Iswarehouseenabled { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("ثبت حواله بیش از موجودی انبار")]
    [DbMap("ALLOWNEGATIVEPHYSICALSTOCK_")]
    public bool? Allownegativephysicalstock { get; set; }
    [DisplayName("روش نگهداری موجودی کالا")]
    [DbMap("TRACKINGMETHOD_")]
    public string? Trackingmethod { get; set; }
    [DisplayName("روش ارزیابی موجودی انبار")]
    [DbMap("VALUATIONMETHOD_")]
    public string? Valuationmethod { get; set; }
    [DisplayName("تاریخ انقضاء ذخیره‌سازی")]
    [DbMap("STORAGEEXPIRATIONDATE_")]
    public DateOnly? Storageexpirationdate { get; set; }
    [DisplayName("حجم ذخیره (بر اساس مگابایت)")]
    [DbMap("STORAGESIZE_")]
    public int Storagesize { get; set; }
}

[DisplayName("گروه‌ها")]
[DbMap("TBL_Groups")]
public sealed class SqlTblGroups : SqlServerEntity
{
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("شناسه گروه")]
    [DbMap("GROUPID_")]
    public int Groupid { get; set; }
    [DisplayName("نوع موجودیت")]
    [DbMap("ENTITYTYPE_")]
    public byte Entitytype { get; set; }
    [DisplayName("شناسه گروه والد")]
    [DbMap("PARENTID_")]
    public int? Parentid { get; set; }
    [DisplayName("کد")]
    [DbMap("CODE_")]
    public int Code { get; set; }
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("توضیحات")]
    [DbMap("DESCRIPTION_")]
    public string? Description { get; set; }
    [DisplayName("مسیر")]
    [DbMap("FULLPATH_")]
    public string? Fullpath { get; set; }
    [DisplayName("نام گروه‌های مسیر")]
    [DbMap("NAMEFULLPATH_")]
    public string? Namefullpath { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("مصرف لایه‌های FIFO")]
[DbMap("TBL_InventoryFifoConsumption")]
public sealed class SqlTblInventoryfifoconsumption : SqlServerEntity<int>
{
    [DisplayName("شناسه آیتم حواله")]
    [DbMap("STOCKCARDITEMID_")]
    public int Stockcarditemid { get; set; }
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("مقدار مصرف شده به واحد اصلی")]
    [DbMap("CONSUMEDQUANTITY_")]
    public decimal Consumedquantity { get; set; }
    [DisplayName("بهای تمام شده در لحظه مصرف")]
    [DbMap("UNITCOST_")]
    public decimal Unitcost { get; set; }
    [DisplayName("تاریخ و زمان مصرف")]
    [DbMap("CONSUMPTIONDATETIME_")]
    public DateTime Consumptiondatetime { get; set; }
    [DisplayName("مبلغ مصرف به ارز پایه")]
    [DbMap("AMOUNT_")]
    public decimal? Amount { get; set; }
    [DisplayName("شناسه کالا")]
    [DbMap("PRODUCTID_")]
    public int Productid { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("لایه موجودی FIFO")]
[DbMap("TBL_InventoryFifoLayer")]
public sealed class SqlTblInventoryfifolayer : SqlServerEntity<int>
{
    [DisplayName("شناسه آیتم رسید")]
    [DbMap("STOCKCARDITEMID_")]
    public int Stockcarditemid { get; set; }
    [DisplayName("شناسه کالا")]
    [DbMap("PRODUCTID_")]
    public int Productid { get; set; }
    [DisplayName("شناسه انبار")]
    [DbMap("WAREHOUSEID_")]
    public int Warehouseid { get; set; }
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("مقدار اولیه به واحد اصلی")]
    [DbMap("INITIALQUANTITY_")]
    public decimal Initialquantity { get; set; }
    [DisplayName("مقدار باقیمانده به واحد اصلی")]
    [DbMap("REMAININGQUANTITY_")]
    public decimal Remainingquantity { get; set; }
    [DisplayName("بهای تمام شده واحد")]
    [DbMap("UNITCOST_")]
    public decimal Unitcost { get; set; }
    [DisplayName("تاریخ و زمان")]
    [DbMap("RECEIPTDATETIME_")]
    public DateTime Receiptdatetime { get; set; }
    [DisplayName("مبلغ کل لایه به ارز پایه")]
    [DbMap("AMOUNT_")]
    public decimal? Amount { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("شهرهای ایران")]
[DbMap("TBL_IranCities")]
public sealed class SqlTblIrancities : SqlServerEntity
{
    [DisplayName("شناسه شهر")]
    [DbMap("CITYID_")]
    public short Cityid { get; set; }
    [DisplayName("شناسه استان")]
    [DbMap("STATEID_")]
    public byte Stateid { get; set; }
    [DisplayName("نام")]
    [DbMap("CITYNAME_")]
    public string Cityname { get; set; } = null!;
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("استان‌های ایران")]
[DbMap("TBL_IranStates")]
public sealed class SqlTblIranstates : SqlServerEntity
{
    [DisplayName("شناسه استان‌")]
    [DbMap("STATEID_")]
    public byte Stateid { get; set; }
    [DisplayName("نام")]
    [DbMap("STATENAME_")]
    public string Statename { get; set; } = null!;
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("تاریخچه پیامک‌ها")]
[DbMap("TBL_MessageHistory")]
public sealed class SqlTblMessagehistory : SqlServerEntity
{
    [DisplayName("شناسه تاریخچه پیامک‌")]
    [DbMap("MESSAGEID_")]
    public int Messageid { get; set; }
    [DisplayName("تلفن همراه")]
    [DbMap("MOBILENUMBER_")]
    public string? Mobilenumber { get; set; }
    [DisplayName("زمان ارسال")]
    [DbMap("SENDTIME_")]
    public DateTime? Sendtime { get; set; }
    [DisplayName("متن پیامک")]
    [DbMap("MESSAGETEXT_")]
    public string? Messagetext { get; set; }
    [DisplayName("کد خطا")]
    [DbMap("ERRORCODE_")]
    public string? Errorcode { get; set; }
    [DisplayName("موضوع")]
    [DbMap("SUBJECT_")]
    public string? Subject { get; set; }
    [DisplayName("پیغام خطا")]
    [DbMap("ERRORMESSAGE_")]
    public string? Errormessage { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("وضعیت تحویل")]
    [DbMap("DELIVERYSTATUS_")]
    public string? Deliverystatus { get; set; }
}
[DisplayName("تنظیمات اعلانات")]
[DbMap("TBL_NotificationConfig")]
public sealed class SqlTblNotificationconfig : SqlServerEntity
{
    [DisplayName("شناسه تنظیمات")]
    [DbMap("CONFIGID_")]
    public int Configid { get; set; }
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("روش ارسال اعلان")]
    [DbMap("NOTIFICATIONMETHOD_")]
    public string Notificationmethod { get; set; } = null!;
    [DisplayName("سررسید فاکتور فروش")]
    [DbMap("NOTIFYSALEINVOICEDUEDATE_")]
    public bool Notifysaleinvoiceduedate { get; set; }
    [DisplayName("زمان اعلان سررسید فاکتور فروش")]
    [DbMap("SALEINVOICEDUENOTIFICATIONTHRESHOLD_")]
    public byte? Saleinvoiceduenotificationthreshold { get; set; }
    [DisplayName("سررسید فاکتور خرید")]
    [DbMap("NOTIFYPURCHASEINVOICEDUEDATE_")]
    public bool Notifypurchaseinvoiceduedate { get; set; }
    [DisplayName("زمان اعلان سررسید فاکتور خرید")]
    [DbMap("PURCHASEINVOICEDUENOTIFICATIONTHRESHOLD_")]
    public byte? Purchaseinvoiceduenotificationthreshold { get; set; }
    [DisplayName("سررسید چک دریافتی")]
    [DbMap("NOTIFYRECEIVEDCHEQUEDUEDATE_")]
    public bool Notifyreceivedchequeduedate { get; set; }
    [DisplayName("زمان اعلان سررسید چک دریافتی")]
    [DbMap("RECEIVEDCHEQUEDUENOTIFICATIONTHRESHOLD_")]
    public byte? Receivedchequeduenotificationthreshold { get; set; }
    [DisplayName("سررسید چک پرداختی")]
    [DbMap("NOTIFYPAIDCHEQUEDUEDATE_")]
    public bool Notifypaidchequeduedate { get; set; }
    [DisplayName("زمان اعلان سررسید چک پرداختی")]
    [DbMap("PAIDCHEQUEDUENOTIFICATIONTHRESHOLD_")]
    public byte? Paidchequeduenotificationthreshold { get; set; }
    [DisplayName("سررسید اقساط")]
    [DbMap("NOTIFYINSTALLMENTDUEDATE_")]
    public bool Notifyinstallmentduedate { get; set; }
    [DisplayName("زمان اعلان سررسید اقساط")]
    [DbMap("INSTALLMENTDUENOTIFICATIONTHRESHOLD_")]
    public byte? Installmentduenotificationthreshold { get; set; }
    [DisplayName("حد سفارش کالا")]
    [DbMap("NOTIFYREORDERPOINT_")]
    public bool Notifyreorderpoint { get; set; }
    [DisplayName("سررسید مهلت ارسال فاکتور به سامانه مودیان")]
    [DbMap("NOTIFYTAXINVOICESUBMISSIONDUEDATE_")]
    public bool Notifytaxinvoicesubmissionduedate { get; set; }
    [DisplayName("زمان اعلان سررسید مهلت ارسال فاکتور به سامانه مودیان")]
    [DbMap("TAXINVOICESUBMISSIONDUENOTIFICATIONTHRESHOLD_")]
    public byte? Taxinvoicesubmissionduenotificationthreshold { get; set; }
    [DisplayName("اتمام تعداد اسناد قابل ثبت")]
    [DbMap("NOTIFYDOCUMENTLIMITREACHED_")]
    public bool Notifydocumentlimitreached { get; set; }
    [DisplayName("تعداد اسناد مانده در زمان اعلان")]
    [DbMap("DOCUMENTLIMITNOTIFICATIONTHRESHOLD_")]
    public byte? Documentlimitnotificationthreshold { get; set; }
    [DisplayName("اتمام تعداد فاکتور آنلاین قابل صدور")]
    [DbMap("NOTIFYONLINEINVOICELIMITREACHED_")]
    public bool Notifyonlineinvoicelimitreached { get; set; }
    [DisplayName("تعداد فاکتور آنلاین مانده در زمان اعلان")]
    [DbMap("ONLINEINVOICENOTIFICATIONTHRESHOLD_")]
    public byte? Onlineinvoicenotificationthreshold { get; set; }
    [DisplayName("اتمام تعداد فاکتور قابل ارسال به سامانه مودیان")]
    [DbMap("NOTIFYTAXSUBMISSIONLIMITREACHED_")]
    public bool Notifytaxsubmissionlimitreached { get; set; }
    [DisplayName("تعداد مانده از فاکتورهای قابل ارسال به سامانه مودیان در زمان اعلان")]
    [DbMap("TAXSUBMISSIONNOTIFICATIONTHRESHOLD_")]
    public byte? Taxsubmissionnotificationthreshold { get; set; }
    [DisplayName("اتمام فضای ذخیره‌سازی")]
    [DbMap("NOTIFYSTORAGELIMITREACHED_")]
    public bool Notifystoragelimitreached { get; set; }
    [DisplayName("میزان فضای مانده در زمان اعلان")]
    [DbMap("STORAGENOTIFICATIONTHRESHOLD_")]
    public int? Storagenotificationthreshold { get; set; }
    [DisplayName("کمبود موجودی کیف پول")]
    [DbMap("NOTIFYWALLETLOWBALANCE_")]
    public bool Notifywalletlowbalance { get; set; }
    [DisplayName("حداقل موجودی")]
    [DbMap("WALLETLOWBALANCETHRESHOLD_")]
    public decimal? Walletlowbalancethreshold { get; set; }
    [DisplayName("ارسال آمار فروش روزانه")]
    [DbMap("SENDDAILYSALESREPORT_")]
    public bool Senddailysalesreport { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("تنخواه")]
[DbMap("TBL_PettyCash")]
public sealed class SqlTblPettycash : SqlServerEntity
{
    [DisplayName("شناسه تنخواه")]
    [DbMap("PETTYCASHID_")]
    public int Pettycashid { get; set; }
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("شناسه حساب تفصیلی")]
    [DbMap("DETAILACCOUNTID_")]
    public long? Detailaccountid { get; set; }
    [DisplayName("شناسه تنخواه گردان")]
    [DbMap("PERSONID_")]
    public int Personid { get; set; }
    [DisplayName("پیش‌فرض")]
    [DbMap("ISDEFAULT_")]
    public bool Isdefault { get; set; }
    [DisplayName("فعال")]
    [DbMap("ISENABLED_")]
    public bool Isenabled { get; set; }
    [DisplayName("توضیحات")]
    [DbMap("DESCRIPTION_")]
    public string? Description { get; set; }
    [DisplayName("کد تنخواه")]
    [DbMap("CODE_")]
    public int Code { get; set; }
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("ارز")]
    [DbMap("CURRENCY_")]
    public string Currency { get; set; } = null!;
}
[DisplayName("دستگاه کارتخوان")]
[DbMap("TBL_PosDevice")]
public sealed class SqlTblPosdevice : SqlServerEntity<int>
{
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("فعال بودن")]
    [DbMap("ISENABLED_")]
    public bool Isenabled { get; set; }
    [DisplayName("شناسه حساب بانکی")]
    [DbMap("BANKACCOUNTID_")]
    public int Bankaccountid { get; set; }
    [DisplayName("توضیحات")]
    [DbMap("DESCRIPTION_")]
    public string? Description { get; set; }
    [DisplayName("نام کارتخوان")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("آدرس IP")]
    [DbMap("IPADDRESS_")]
    public string Ipaddress { get; set; } = null!;
    [DisplayName("شماره پورت")]
    [DbMap("PORTNUMBER_")]
    public int? Portnumber { get; set; }
    [DisplayName("شماره پایانه")]
    [DbMap("TERMINALNUMBER_")]
    public string? Terminalnumber { get; set; }
    [DisplayName("شماره پذیرنده")]
    [DbMap("MERCHANTNUMBER_")]
    public string? Merchantnumber { get; set; }
    [DisplayName("شماره سریال")]
    [DbMap("SERIALNUMBER_")]
    public string? Serialnumber { get; set; }
    [DisplayName("پیش فرض بودن")]
    [DbMap("ISDEFAULT_")]
    public bool Isdefault { get; set; }
    [DisplayName("نوع کارتخوان")]
    [DbMap("PSP_")]
    public string Psp { get; set; } = null!;
    [DisplayName("شناسه حساب تفصیلی")]
    [DbMap("DETAILACCOUNTID_")]
    public long Detailaccountid { get; set; }
    [DisplayName("ارز")]
    [DbMap("CURRENCY_")]
    public string Currency { get; set; } = null!;
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("تنظیمات چاپ")]
[DbMap("TBL_PrintConfig")]
public sealed class SqlTblPrintconfig : SqlServerEntity
{
    [DisplayName("شناسه تنظیمات")]
    [DbMap("CONFIGID_")]
    public int Configid { get; set; }
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("چاپ لوگو")]
    [DbMap("PRINTLOGO_")]
    public bool Printlogo { get; set; }
    [DisplayName("عنوان صورتحساب")]
    [DbMap("INVOICETITLE_")]
    public string Invoicetitle { get; set; } = null!;
    [DisplayName("چاپ اطلاعات کسب‌وکار")]
    [DbMap("PRINTSELLERINFO_")]
    public bool Printsellerinfo { get; set; }
    [DisplayName("چاپ اطلاعات خریدار")]
    [DbMap("PRINTCUSTOMERINFO_")]
    public bool Printcustomerinfo { get; set; }
    [DisplayName("چاپ محل امضاها")]
    [DbMap("PRINTSIGNATUREAREA_")]
    public bool Printsignaturearea { get; set; }
    [DisplayName("عنوان امضای یک")]
    [DbMap("SIGNATURETITLE1_")]
    public string? Signaturetitle1 { get; set; }
    [DisplayName("عنوان امضای دو")]
    [DbMap("SIGNATURETITLE2_")]
    public string? Signaturetitle2 { get; set; }
    [DisplayName("عنوان امضای سه")]
    [DbMap("SIGNATURETITLE3_")]
    public string? Signaturetitle3 { get; set; }
    [DisplayName("عنوان امضای چهار")]
    [DbMap("SIGNATURETITLE4_")]
    public string? Signaturetitle4 { get; set; }
    [DisplayName("عنوان امضای پنج")]
    [DbMap("SIGNATURETITLE5_")]
    public string? Signaturetitle5 { get; set; }
    [DisplayName("چاپ تاریخ سررسید")]
    [DbMap("PRINTDUEDATE_")]
    public bool Printduedate { get; set; }
    [DisplayName("چاپ ستون مالیات")]
    [DbMap("TAXPRINTMETHOD_")]
    public string Taxprintmethod { get; set; } = null!;
    [DisplayName("چاپ ستون تخفیف")]
    [DbMap("DISCOUNTPRINTMETHOD_")]
    public string Discountprintmethod { get; set; } = null!;
    [DisplayName("چاپ اطلاعات پرداخت")]
    [DbMap("PRINTPAYMENTINFO_")]
    public bool Printpaymentinfo { get; set; }
    [DisplayName("چاپ مانده حساب")]
    [DbMap("PRINTACCOUNTBALANCE_")]
    public bool Printaccountbalance { get; set; }
    [DisplayName("چاپ تاریخ و زمان چاپ")]
    [DbMap("PRINTCURRENTDATETIME_")]
    public bool Printcurrentdatetime { get; set; }
    [DisplayName("متن انتهای فاکتور")]
    [DbMap("INVOICEFOOTERTEXT_")]
    public string? Invoicefootertext { get; set; }
    [DisplayName("تعداد چاپ")]
    [DbMap("PRINTCOPYCOUNT_")]
    public byte Printcopycount { get; set; }
    [DisplayName("چاپ تعداد اقلام داخل فاکتور")]
    [DbMap("PRINTINVOICEITEMCOUNT_")]
    public bool Printinvoiceitemcount { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}

[DisplayName("تخفیف اشتراک")]
[DbMap("TBL_ServiceDiscount")]
public sealed class SqlTblServicediscount : SqlServerEntity<short>
{
    [DisplayName("عنوان")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("کد تخفیف")]
    [DbMap("CODE_")]
    public string Code { get; set; } = null!;
    [DisplayName("تاریخ شروع")]
    [DbMap("STARTDATE_")]
    public DateOnly? Startdate { get; set; }
    [DisplayName("تاریخ پایان")]
    [DbMap("ENDDATE_")]
    public DateOnly? Enddate { get; set; }
    [DisplayName("عمومی بودن")]
    [DbMap("ISPUBLIC_")]
    public bool Ispublic { get; set; }
    [DisplayName("مقدار درصدی تخفیف")]
    [DbMap("PERCENTAMOUNT_")]
    public decimal Percentamount { get; set; }
    [DisplayName("مقدار ثابت تخفیف")]
    [DbMap("FIXEDAMOUNT_")]
    public decimal Fixedamount { get; set; }
    [DisplayName("توضیحات")]
    [DbMap("DESCRIPTION_")]
    public string? Description { get; set; }
    [DisplayName("فعال بودن")]
    [DbMap("ISENABLED_")]
    public bool Isenabled { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("پرداخت سرویس")]
[DbMap("TBL_ServicePayment")]
public sealed class SqlTblServicepayment : SqlServerEntity
{
    [DisplayName("شناسه پرداخت سرویس")]
    [DbMap("PAYMENTID_")]
    public int Paymentid { get; set; }
    [DisplayName("شماره فاکتور")]
    [DbMap("INVOICENUMBER_")]
    public string? Invoicenumber { get; set; }
    [DisplayName("تاریخ فاکتور")]
    [DbMap("INVOICEDATE_")]
    public string? Invoicedate { get; set; }
    [DisplayName("شناسه مرجع تراکنش")]
    [DbMap("TRANSACTIONREFERENCEID_")]
    public string? Transactionreferenceid { get; set; }
    [DisplayName("مبلغ")]
    [DbMap("AMOUNT_")]
    public decimal? Amount { get; set; }
    [DisplayName("تاریخ تراکنش")]
    [DbMap("TRANSACTIONDATE_")]
    public DateTime? Transactiondate { get; set; }
    [DisplayName("شماره مرجع")]
    [DbMap("REFERENCENUMBER_")]
    public long? Referencenumber { get; set; }
    [DisplayName("شماره کارت")]
    [DbMap("MASKEDCARDNUMBER_")]
    public string? Maskedcardnumber { get; set; }
    [DisplayName("شماره مرجع شاپرک")]
    [DbMap("SHAPARAKREFNUMBER_")]
    public long? Shaparakrefnumber { get; set; }
    [DisplayName("موفق بودن")]
    [DbMap("ISSUCCESS_")]
    public bool? Issuccess { get; set; }
    [DisplayName("پیغام")]
    [DbMap("MESSAGE_")]
    public string? Message { get; set; }
    [DisplayName("شماره پیگیری")]
    [DbMap("TRACENUMBER_")]
    public long? Tracenumber { get; set; }
    [DisplayName("زمان ثبت")]
    [DbMap("REGISTERTIME_")]
    public DateTime? Registertime { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("نوع دسترسی اشتراک")]
[DbMap("TBL_ServicePlan")]
public sealed class SqlTblServiceplan : SqlServerEntity
{
    [DisplayName("شناسه نوع دسترسی اشتراک")]
    [DbMap("PLANID_")]
    public byte Planid { get; set; }
    [DisplayName("عنوان")]
    [DbMap("PLANNAME_")]
    public string Planname { get; set; } = null!;
    [DisplayName("توضیحات")]
    [DbMap("PLANDESCRIPTION_")]
    public string? Plandescription { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("درخواست اشتراک")]
[DbMap("TBL_ServiceRequest")]
public sealed class SqlTblServicerequest : SqlServerEntity<int>
{
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int? Shopid { get; set; }
    [DisplayName("شناسه نوع اشتراک")]
    [DbMap("SUBSCRIPTIONID_")]
    public byte? Subscriptionid { get; set; }
    [DisplayName("وضعیت اشتراک")]
    [DbMap("STATUS_")]
    public byte Status { get; set; }
    [DisplayName("زمان درخواست")]
    [DbMap("REQUESTTIME_")]
    public DateTime Requesttime { get; set; }
    [DisplayName("شناسه پرداخت")]
    [DbMap("PAYMENTID_")]
    public int? Paymentid { get; set; }
    [DisplayName("قیمت فروش")]
    [DbMap("PRICE_")]
    public decimal Price { get; set; }
    [DisplayName("شناسه تخفیف اشتراک")]
    [DbMap("DISCOUNTID_")]
    public short? Discountid { get; set; }
    [DisplayName("مبلغ تخفیف")]
    [DbMap("DISCOUNTAMOUNT_")]
    public decimal Discountamount { get; set; }
    [DisplayName("مبلغ قابل پرداخت")]
    [DbMap("PAYABLEAMOUNT_")]
    public decimal Payableamount { get; set; }
    [DisplayName("تاریخ شروع اشتراک")]
    [DbMap("SUBSCRIPTIONSTARTDATE_")]
    public DateOnly? Subscriptionstartdate { get; set; }
    [DisplayName("تاریخ پایان اشتراک")]
    [DbMap("SUBSCRIPTIONENDDATE_")]
    public DateOnly? Subscriptionenddate { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("نام کاربری")]
    [DbMap("USERID_")]
    public string Userid { get; set; } = null!;
    [DisplayName("مبلغ مالیات")]
    [DbMap("TAXAMOUNT_")]
    public decimal Taxamount { get; set; }
}
[DisplayName("نوع اشتراک")]
[DbMap("TBL_ServiceSubscription")]
public sealed class SqlTblServicesubscription : SqlServerEntity
{
    [DisplayName("شناسه نوع اشتراک")]
    [DbMap("SUBSCRIPTIONID_")]
    public byte Subscriptionid { get; set; }
    [DisplayName("عنوان")]
    [DbMap("SUBSCRIPTIONNAME_")]
    public string Subscriptionname { get; set; } = null!;
    [DisplayName("شناسه نوع دسترسی")]
    [DbMap("PLANID_")]
    public byte Planid { get; set; }
    [DisplayName("قیمت پایه")]
    [DbMap("SUBSCRIPTIONPRICE_")]
    public decimal Subscriptionprice { get; set; }
    [DisplayName("توضیحات")]
    [DbMap("SUBSCRIPTIONDESCRIPTION_")]
    public string? Subscriptiondescription { get; set; }
    [DisplayName("فعال بودن")]
    [DbMap("SUBSCRIPTIONENABLE_")]
    public bool Subscriptionenable { get; set; }
    [DisplayName("پیش فرض بودن")]
    [DbMap("SUBSCRIPTIONDEFAULT_")]
    public bool Subscriptiondefault { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("تخفیف درصدی")]
    [DbMap("PERCENTDISCOUNT_")]
    public decimal Percentdiscount { get; set; }
    [DisplayName("تخفیف ثابت")]
    [DbMap("FIXEDDISCOUNT_")]
    public decimal Fixeddiscount { get; set; }
    [DisplayName("نرخ مالیات")]
    [DbMap("SUBSCRIPTIONTAXRATE_")]
    public decimal Subscriptiontaxrate { get; set; }
    [DisplayName("تعداد روزهای اشتراک")]
    [DbMap("SUBSCRIPTIONDAYS_")]
    public short Subscriptiondays { get; set; }
    [DisplayName("مبلغ قابل پرداخت")]
    [DbMap("SUBSCRIPTIONPAYABLEAMOUNT_")]
    public decimal? Subscriptionpayableamount { get; set; }
    [DisplayName("مبلغ مالیات")]
    [DbMap("SUBSCRIPTIONTAXAMOUNT_")]
    public decimal? Subscriptiontaxamount { get; set; }
}
[DisplayName("سهامدار")]
[DbMap("TBL_Shareholder")]
public sealed class SqlTblShareholder : SqlServerEntity
{
    [DisplayName("شناسه سهامدار")]
    [DbMap("SHAREHOLDERID_")]
    public int Shareholderid { get; set; }
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("شناسه دوره مالی")]
    [DbMap("FISCALPERIODID_")]
    public int Fiscalperiodid { get; set; }
    [DisplayName("شناسه شخص")]
    [DbMap("PERSONID_")]
    public int Personid { get; set; }
    [DisplayName("درصد سهام")]
    [DbMap("SHAREPERCENT_")]
    public decimal Sharepercent { get; set; }
    [DisplayName("توضیحات")]
    [DbMap("DESCRIPTION_")]
    public string? Description { get; set; }
    [DisplayName("سهم سرمایه اولیه")]
    [DbMap("INITIALCAPITALSHARE_")]
    public decimal? Initialcapitalshare { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("دوره مالی فروشگاه")]
[DbMap("TBL_ShopFiscalPeriod")]
public sealed class SqlTblShopfiscalperiod : SqlServerEntity
{
    [DisplayName("شناسه دوره مالی")]
    [DbMap("FISCALPERIODID_")]
    public int Fiscalperiodid { get; set; }
    [DisplayName("عنوان دوره مالی")]
    [DbMap("FISCALPERIODNAME_")]
    public string Fiscalperiodname { get; set; } = null!;
    [DisplayName("توضیحات")]
    [DbMap("FISCALPERIODDESCRIPTION_")]
    public string? Fiscalperioddescription { get; set; }
    [DisplayName("تاریخ شروع")]
    [DbMap("STARTDATE_")]
    public DateOnly Startdate { get; set; }
    [DisplayName("تاریخ پایان")]
    [DbMap("ENDDATE_")]
    public DateOnly Enddate { get; set; }
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("شناسه وضعیت دوره مالی")]
    [DbMap("FISCALPERIODSTATUSID_")]
    public byte Fiscalperiodstatusid { get; set; }
    [DisplayName("اولین دوره مالی")]
    [DbMap("FIRSTPERIOD_")]
    public bool Firstperiod { get; set; }
    [DisplayName("آخرین شماره سند")]
    [DbMap("LASTDOCUMENTNUMBER_")]
    public int Lastdocumentnumber { get; set; }
    [DisplayName("آخرین شماره عطف")]
    [DbMap("LASTREFERENCENUMBER_")]
    public int Lastreferencenumber { get; set; }
    [DisplayName("تاریخ آخرین سند")]
    [DbMap("LASTDOCUMENTDATETIME_")]
    public DateTime? Lastdocumentdatetime { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("اعلان فروشگاه")]
[DbMap("TBL_ShopNotification")]
public sealed class SqlTblShopnotification : SqlServerEntity
{
    [DisplayName("شناسه اعلان مغازه")]
    [DbMap("NOTIFICATIONID_")]
    public int Notificationid { get; set; }
    [DisplayName("شناسه نوع اعلان")]
    [DbMap("NOTIFICATIONTYPEID_")]
    public byte Notificationtypeid { get; set; }
    [DisplayName("عنوان")]
    [DbMap("NOTIFICATIONTITLE_")]
    public string? Notificationtitle { get; set; }
    [DisplayName("متن")]
    [DbMap("NOTIFICATIONTEXT_")]
    public string Notificationtext { get; set; } = null!;
    [DisplayName("مجاز به بستن")]
    [DbMap("NOTIFICATIONCLOSEALLOWED_")]
    public bool Notificationcloseallowed { get; set; }
    [DisplayName("زمان شروع")]
    [DbMap("NOTIFICATIONSTARTTIME_")]
    public DateTime Notificationstarttime { get; set; }
    [DisplayName("زمان پایان")]
    [DbMap("NOTIFICATIONENDTIME_")]
    public DateTime Notificationendtime { get; set; }
    [DisplayName("همه مغازه‌ها")]
    [DbMap("NOTIFICATIONALLSHOPS_")]
    public bool Notificationallshops { get; set; }
    [DisplayName("همه نقش‌های اعضاء مغازه")]
    [DbMap("NOTIFICATIONALLMEMBERROLES_")]
    public bool Notificationallmemberroles { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("بنر بودن")]
    [DbMap("ISBANNER_")]
    public bool Isbanner { get; set; }
}
[DisplayName("اعلان فروشگاه در نقش عضو فروشگاه")]
[DbMap("TBL_ShopNotificationInMemberRole")]
public sealed class SqlTblShopnotificationinmemberrole : SqlServerEntity
{
    [DisplayName("شناسه اعلان مغازه در نقش عضو مغازه")]
    [DbMap("NOTIFICATIONINMEMBERROLEID_")]
    public int Notificationinmemberroleid { get; set; }
    [DisplayName("شناسه اعلان")]
    [DbMap("NOTIFICATIONID_")]
    public int Notificationid { get; set; }
    [DisplayName("شناسه نقش عضو مغازه")]
    [DbMap("MEMBERROLEID_")]
    public byte Memberroleid { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("اعلان فروشگاه در فروشگاه")]
[DbMap("TBL_ShopNotificationInShop")]
public sealed class SqlTblShopnotificationinshop : SqlServerEntity
{
    [DisplayName("شناسه اعلان مغازه در مغازه")]
    [DbMap("NOTIFICATIONINSHOPID_")]
    public int Notificationinshopid { get; set; }
    [DisplayName("شناسه اعلان")]
    [DbMap("NOTIFICATIONID_")]
    public int Notificationid { get; set; }
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("نوع اعلان فروشگاه")]
[DbMap("TBL_ShopNotificationType")]
public sealed class SqlTblShopnotificationtype : SqlServerEntity
{
    [DisplayName("شناسه نوع اعلان مغازه")]
    [DbMap("NOTIFICATIONTYPEID_")]
    public byte Notificationtypeid { get; set; }
    [DisplayName("عنوان")]
    [DbMap("NOTIFICATIONTYPENAME_")]
    public string Notificationtypename { get; set; } = null!;
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("کاردکس انبار")]
[DbMap("TBL_StockCard")]
public sealed class SqlTblStockcard : SqlServerEntity<int>
{
    [DisplayName("شناسه انبار")]
    [DbMap("WAREHOUSEID_")]
    public int Warehouseid { get; set; }
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("شناسه دوره مالی")]
    [DbMap("FISCALPERIODID_")]
    public int Fiscalperiodid { get; set; }
    [DisplayName("شناسه پروژه")]
    [DbMap("PROJECTID_")]
    public int? Projectid { get; set; }
    [DisplayName("تاریخ و زمان کاردکس")]
    [DbMap("DATETIME_")]
    public DateTime Datetime { get; set; }
    [DisplayName("شماره")]
    [DbMap("NUMBER_")]
    public int Number { get; set; }
    [DisplayName("توضیحات")]
    [DbMap("DESCRIPTION_")]
    public string? Description { get; set; }
    [DisplayName("طرف حساب")]
    [DbMap("PERSONID_")]
    public int? Personid { get; set; }
    [DisplayName("جهت ورود/خروج")]
    [DbMap("DIRECTION_")]
    public bool Direction { get; set; }
    [DisplayName("نوع مرجع رویداد مالی")]
    [DbMap("REFERENCETYPE_")]
    public byte Referencetype { get; set; }
    [DisplayName("شناسه مرجع رویداد مالی")]
    [DbMap("REFERENCEID_")]
    public long? Referenceid { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("آیتم کاردکس انبار")]
[DbMap("TBL_StockCardItem")]
public sealed class SqlTblStockcarditem : SqlServerEntity<int>
{
    [DisplayName("شناسه کاردکس")]
    [DbMap("STOCKCARDID_")]
    public int Stockcardid { get; set; }
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("شناسه کالا")]
    [DbMap("PRODUCTID_")]
    public int Productid { get; set; }
    [DisplayName("تعداد/مقدار")]
    [DbMap("QUANTITY_")]
    public decimal Quantity { get; set; }
    [DisplayName("نمایش به واحد اندازه‌گیری دوم")]
    [DbMap("SHOWINSECONDUNIT_")]
    public bool Showinsecondunit { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("انبارگردانی")]
[DbMap("TBL_StockTaking")]
public sealed class SqlTblStocktaking : SqlServerEntity<int>
{
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("شناسه انبار")]
    [DbMap("WAREHOUSEID_")]
    public int Warehouseid { get; set; }
    [DisplayName("شناسه دوره مالی")]
    [DbMap("FISCALPERIODID_")]
    public int Fiscalperiodid { get; set; }
    [DisplayName("شناسه پروژه")]
    [DbMap("PROJECTID_")]
    public int? Projectid { get; set; }
    [DisplayName("توضیحات")]
    [DbMap("DESCRIPTION_")]
    public string? Description { get; set; }
    [DisplayName("وضعیت انبارگردانی")]
    [DbMap("STATUS_")]
    public byte Status { get; set; }
    [DisplayName("تاریخ و زمان انبارگردانی")]
    [DbMap("DATETIME_")]
    public DateTime Datetime { get; set; }
    [DisplayName("شماره")]
    [DbMap("NUMBER_")]
    public int? Number { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("آیتم انبارگردانی")]
[DbMap("TBL_StockTakingItem")]
public sealed class SqlTblStocktakingitem : SqlServerEntity<int>
{
    [DisplayName("شناسه انبارگردانی")]
    [DbMap("STOCKTAKINGID_")]
    public int Stocktakingid { get; set; }
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("شناسه کالا")]
    [DbMap("PRODUCTID_")]
    public int Productid { get; set; }
    [DisplayName("مقدار اولیه")]
    [DbMap("INITIALQUANTITY_")]
    public decimal Initialquantity { get; set; }
    [DisplayName("مقدار نهایی")]
    [DbMap("FINALQUANTITY_")]
    public decimal? Finalquantity { get; set; }
    [DisplayName("نمایش به واحد اندازه‌گیری دوم")]
    [DbMap("SHOWINSECONDUNIT_")]
    public bool Showinsecondunit { get; set; }
    [DisplayName("بهای تمام شده واحد")]
    [DbMap("UNITCOST_")]
    public decimal? Unitcost { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("انتقال")]
[DbMap("TBL_Transfer")]
public sealed class SqlTblTransfer : SqlServerEntity
{
    [DisplayName("شناسه انتقال")]
    [DbMap("TRANSFERID_")]
    public int Transferid { get; set; }
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("شناسه دوره مالی")]
    [DbMap("FISCALPERIODID_")]
    public int Fiscalperiodid { get; set; }
    [DisplayName("شناسه پروژه")]
    [DbMap("PROJECTID_")]
    public int? Projectid { get; set; }
    [DisplayName("توضیحات")]
    [DbMap("DESCRIPTION_")]
    public string? Description { get; set; }
    [DisplayName("مبلغ")]
    [DbMap("TOTALAMOUNT_")]
    public decimal Totalamount { get; set; }
    [DisplayName("تاریخ و زمان")]
    [DbMap("DATETIME_")]
    public DateTime Datetime { get; set; }
    [DisplayName("ارز")]
    [DbMap("CURRENCY_")]
    public string Currency { get; set; } = null!;
    [DisplayName("نرخ تبدیل ارز")]
    [DbMap("EXCHANGERATE_")]
    public decimal Exchangerate { get; set; }
    [DisplayName("نوع انتقال")]
    [DbMap("TRANSFERTYPE_")]
    public byte Transfertype { get; set; }
    [DisplayName("نوع مرجع رویداد مالی")]
    [DbMap("REFERENCETYPE_")]
    public byte? Referencetype { get; set; }
    [DisplayName("شناسه مرجع رویداد مالی")]
    [DbMap("REFERENCEID_")]
    public long? Referenceid { get; set; }
    [DisplayName("شماره")]
    [DbMap("NUMBER_")]
    public int Number { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("آیتم انتقال")]
[DbMap("TBL_TransferItem")]
public sealed class SqlTblTransferitem : SqlServerEntity
{
    [DisplayName("شناسه آیتم انتقال")]
    [DbMap("TRANSFERITEMID_")]
    public int Transferitemid { get; set; }
    [DisplayName("شناسه انتقال")]
    [DbMap("TRANSFERID_")]
    public int Transferid { get; set; }
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("نوع انتقال")]
    [DbMap("TRANSFERTYPE_")]
    public byte Transfertype { get; set; }
    [DisplayName("حساب مقصد")]
    [DbMap("ISDESTINATION_")]
    public bool Isdestination { get; set; }
    [DisplayName("مبلغ")]
    [DbMap("AMOUNT_")]
    public decimal Amount { get; set; }
    [DisplayName("کارمزد")]
    [DbMap("COMMISSION_")]
    public decimal Commission { get; set; }
    [DisplayName("توضیحات")]
    [DbMap("DESCRIPTION_")]
    public string? Description { get; set; }
    [DisplayName("شناسه حساب")]
    [DbMap("ACCOUNTID_")]
    public int Accountid { get; set; }
    [DisplayName("شناسه موجودیت")]
    [DbMap("ENTITYID_")]
    public int? Entityid { get; set; }
    [DisplayName("شناسه چک")]
    [DbMap("CHECKID_")]
    public int? Checkid { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("بارگذاری فایل‌ها")]
[DbMap("TBL_UploadFile")]
public sealed class SqlTblUploadfile : SqlServerEntity<int>
{
    [DisplayName("نام اصلی")]
    [DbMap("ORIGINALNAME_")]
    public string Originalname { get; set; } = null!;
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("پسوند")]
    [DbMap("EXTENSION_")]
    public string Extension { get; set; } = null!;
    [DisplayName("اندازه")]
    [DbMap("SIZE_")]
    public long Size { get; set; }
    [DisplayName("شناسه یکتا")]
    [DbMap("UID_")]
    public string Uid { get; set; } = null!;
    [DisplayName("نشانی نسبی")]
    [DbMap("RELATIVEURL_")]
    public string Relativeurl { get; set; } = null!;
    [DisplayName("نوع فایل")]
    [DbMap("FILETYPE_")]
    public string Filetype { get; set; } = null!;
    [DisplayName("زمان ایجاد")]
    [DbMap("CREATETIME_")]
    public DateTime Createtime { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("تاریخچه عملیات کاربران")]
[DbMap("TBL_UserActionLog")]
public sealed class SqlTblUseractionlog : SqlServerEntity<long>
{
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("شناسه کاربر")]
    [DbMap("USERID_")]
    public string Userid { get; set; } = null!;
    [DisplayName("نوع عملیات")]
    [DbMap("ACTIONTYPE_")]
    public byte Actiontype { get; set; }
    [DisplayName("نوع موجودیت")]
    [DbMap("ENTITYTYPE_")]
    public byte? Entitytype { get; set; }
    [DisplayName("شناسه موجودیت")]
    [DbMap("ENTITYID_")]
    public long? Entityid { get; set; }
    [DisplayName("جزئیات")]
    [DbMap("DETAILS_")]
    public string? Details { get; set; }
    [DisplayName("زمان عملیات")]
    [DbMap("DATETIME_")]
    public DateTime Datetime { get; set; }
    [DisplayName("شناسه لاگ والد")]
    [DbMap("PARENTID_")]
    public long? Parentid { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
