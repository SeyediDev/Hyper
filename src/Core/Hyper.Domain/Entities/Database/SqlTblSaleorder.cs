#nullable enable
namespace Hyper.Domain.Entities.Database;

[DisplayName("فاکتور فروش")]
[DbMap("TBL_SaleOrder")]
public sealed class SqlTblSaleorder : SqlServerEntity
{
    [DisplayName("شناسه سفارش فروش")]
    [DbMap("SALEORDERID_")]
    public long Saleorderid { get; set; }
    [DisplayName("مجموع مبلغ قبل از کسر تخفیف")]
    [DbMap("TOTALAMOUNTBEFOREDISCOUNT_")]
    public decimal Totalamountbeforediscount { get; set; }
    [DisplayName("مجموع مالیات بر ارزش افزوده")]
    [DbMap("TOTALVATAMOUNT_")]
    public decimal Totalvatamount { get; set; }
    [DisplayName("مجموع تخفیفات")]
    [DbMap("TOTALDISCOUNTAMOUNT_")]
    public decimal Totaldiscountamount { get; set; }
    [DisplayName("مجموع صورتحساب")]
    [DbMap("TOTALINVOICEAMOUNT_")]
    public decimal Totalinvoiceamount { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("توضیحات حمل و نقل")]
    [DbMap("SHIPPINGDESCRIPTION_")]
    public string? Shippingdescription { get; set; }
    [DisplayName("مبلغ حمل و نقل")]
    [DbMap("SHIPPINGAMOUNT_")]
    public decimal? Shippingamount { get; set; }
    [DisplayName("شناسه شخص حمل کننده")]
    [DbMap("COURIERID_")]
    public int? Courierid { get; set; }
    [DisplayName("پاسخ استعلام سامانه مودیان")]
    [DbMap("TAXPAYERPORTALINQUIRYRESPONSE_")]
    public string? Taxpayerportalinquiryresponse { get; set; }
    [DisplayName("شناسه یکتای الحاقیه")]
    [DbMap("ENDORSEMENTID_")]
    public string? Endorsementid { get; set; }
    [DisplayName("شناسه یکتای بیمه نامه")]
    [DbMap("INSURANCEID_")]
    public string? Insuranceid { get; set; }
    [DisplayName("تاریخ اعلامیه فروش")]
    [DbMap("SALESNOTICEDATE_")]
    public DateOnly? Salesnoticedate { get; set; }
    [DisplayName("شماره اعلامیه فروش")]
    [DbMap("SALESNOTICENUMBER_")]
    public string? Salesnoticenumber { get; set; }
    [DisplayName("کالاهای حمل شده")]
    [DbMap("SHIPPEDPRODUCTS_")]
    public string? Shippedproducts { get; set; }
    [DisplayName("شماره ملی/کد فراگیر اتباع غیر ایرانی راننده درحمل ونقل جاده‌ای")]
    [DbMap("DRIVERIDENTIFICATIONNUMBER_")]
    public string? Driveridentificationnumber { get; set; }
    [DisplayName("شماره ناوگان")]
    [DbMap("FLEETNUMBER_")]
    public string? Fleetnumber { get; set; }
    [DisplayName("نوع بارنامه/نوع حمل")]
    [DbMap("WAYBILLTYPE_")]
    public byte? Waybilltype { get; set; }
    [DisplayName("شناسه ملی/ شماره ملی/ شناسه مشارکت مدنی/ کد فراگیر اتباع غیر ایرانی گیرنده")]
    [DbMap("RECEIVERIDENTIFICATIONNUMBER_")]
    public string? Receiveridentificationnumber { get; set; }
    [DisplayName("شناسه ملی/ شماره ملی/ شناسه مشارکت مدنی/ کد فراگیر اتباع غیر ایرانی فرستنده")]
    [DbMap("SENDERIDENTIFICATIONNUMBER_")]
    public string? Senderidentificationnumber { get; set; }
    [DisplayName("شهر مقصد")]
    [DbMap("DESTINATIONCITY_")]
    public string? Destinationcity { get; set; }
    [DisplayName("کشور مقصد")]
    [DbMap("DESTINATIONCOUNTRY_")]
    public string? Destinationcountry { get; set; }
    [DisplayName("شهر مبدا")]
    [DbMap("ORIGINCITY_")]
    public string? Origincity { get; set; }
    [DisplayName("کشور مبدا")]
    [DbMap("ORIGINCOUNTRY_")]
    public string? Origincountry { get; set; }
    [DisplayName("شماره بارنامه مرجع")]
    [DbMap("REFERENCEWAYBILLNUMBER_")]
    public string? Referencewaybillnumber { get; set; }
    [DisplayName("شماره بارنامه")]
    [DbMap("WAYBILLNUMBER_")]
    public string? Waybillnumber { get; set; }
    [DisplayName("شماره اقتصادی آژانس")]
    [DbMap("AGENCYECONOMICCODE_")]
    public string? Agencyeconomiccode { get; set; }
    [DisplayName("مجموع ارزش ارزی")]
    [DbMap("TOTALCURRENCYAMOUNT_")]
    public decimal? Totalcurrencyamount { get; set; }
    [DisplayName("مجموع ارزش ریالی")]
    [DbMap("TOTALRIALAMOUNT_")]
    public decimal? Totalrialamount { get; set; }
    [DisplayName("مجموع وزن خالص")]
    [DbMap("TOTALNETWEIGHT_")]
    public decimal? Totalnetweight { get; set; }
    [DisplayName("شماره اشتراک/ شناسه قبض بهره بردار")]
    [DbMap("SUBSCRIBERNUMBER_")]
    public string? Subscribernumber { get; set; }
    [DisplayName("تاریخ کوتاژ اظهارنامه گمرکی")]
    [DbMap("CUSTOMSDECLARATIONDATE_")]
    public DateOnly? Customsdeclarationdate { get; set; }
    [DisplayName("شماره کوتاژ اظهارنامه گمرکی")]
    [DbMap("CUSTOMSDECLARATIONNUMBER_")]
    public string? Customsdeclarationnumber { get; set; }
    [DisplayName("شماره گذرنامه خریدار")]
    [DbMap("CUSTOMERPASSPORTNUMBER_")]
    public string? Customerpassportnumber { get; set; }
    [DisplayName("نوع پرواز")]
    [DbMap("FLIGHTTYPE_")]
    public byte? Flighttype { get; set; }
    [DisplayName("مابه‌التفاوت اضافات و کسورات")]
    [DbMap("ADJUSTMENTSAMOUNT_")]
    public decimal Adjustmentsamount { get; set; }
    [DisplayName("اضافات و کسورات")]
    [DbMap("ADJUSTMENTS_")]
    public string? Adjustments { get; set; }
    [DisplayName("تاریخ سر رسید")]
    [DbMap("DUEDATE_")]
    public DateOnly? Duedate { get; set; }
    [DisplayName("توضیحات")]
    [DbMap("DESCRIPTION_")]
    public string? Description { get; set; }
    [DisplayName("وضعیت صورتحساب در سامانه مودیان")]
    [DbMap("TAXPAYERPORTALSTATUS_")]
    public byte Taxpayerportalstatus { get; set; }
    [DisplayName("مالیات موضوع ماده 17")]
    [DbMap("TOTALARTICLE17TAXAMOUNT_")]
    public decimal? Totalarticle17taxamount { get; set; }
    [DisplayName("مجموع سهم مالیات بر ارزش افزوده از پرداخت")]
    [DbMap("TOTALVATPAIDAMOUNT_")]
    public decimal? Totalvatpaidamount { get; set; }
    [DisplayName("مبلغ نسیه")]
    [DbMap("TOTALCREDITAMOUNT_")]
    public decimal? Totalcreditamount { get; set; }
    [DisplayName("مبلغ پرداختی نقدی")]
    [DbMap("TOTALCASHPAIDAMOUNT_")]
    public decimal? Totalcashpaidamount { get; set; }
    [DisplayName("روش تسویه")]
    [DbMap("SETTLEMENTTYPEID_")]
    public byte? Settlementtypeid { get; set; }
    [DisplayName("مبلغ قابل دریافت")]
    [DbMap("TOTALAMOUNT_")]
    public decimal Totalamount { get; set; }
    [DisplayName("مبلغ دریافت شده")]
    [DbMap("PAIDAMOUNT_")]
    public decimal Paidamount { get; set; }
    [DisplayName("مجموع مبلغ پس از کسر تخفیف")]
    [DbMap("TOTALAMOUNTAFTERDISCOUNT_")]
    public decimal Totalamountafterdiscount { get; set; }
    [DisplayName("مجموع سایر مالیات، عوارض و وجوه قانونی")]
    [DbMap("TOTALOTHERTAXESANDCHARGESAMOUNT_")]
    public decimal? Totalothertaxesandchargesamount { get; set; }
    [DisplayName("وضعیت دریافت")]
    [DbMap("PAYMENTSTATUS_")]
    public byte Paymentstatus { get; set; }
    [DisplayName("وضعیت صدور حواله انبار")]
    [DbMap("INVENTORYSTATUS_")]
    public byte? Inventorystatus { get; set; }
    [DisplayName("وضعیت تحویل")]
    [DbMap("DELIVERYSTATUS_")]
    public byte? Deliverystatus { get; set; }
    [DisplayName("وضعیت صورتحساب")]
    [DbMap("STATUS_")]
    public byte Status { get; set; }
    [DisplayName("شماره قرارداد پیمانکاری")]
    [DbMap("CONTRACTNUMBER_")]
    public string? Contractnumber { get; set; }
    [DisplayName("کد گمرک محل اظهار فروشنده")]
    [DbMap("SELLERCUSTOMSOFFICECODE_")]
    public string? Sellercustomsofficecode { get; set; }
    [DisplayName("شماره پروانه گمرکی")]
    [DbMap("CUSTOMSPERMITNUMBER_")]
    public string? Customspermitnumber { get; set; }
    [DisplayName("کد شعبه خریدار")]
    [DbMap("CUSTOMERBRANCHCODE_")]
    public string? Customerbranchcode { get; set; }
    [DisplayName("کد پستی خریدار")]
    [DbMap("CUSTOMERPOSTALCODE_")]
    public string? Customerpostalcode { get; set; }
    [DisplayName("کد شعبه فروشنده")]
    [DbMap("SELLERBRANCHCODE_")]
    public string? Sellerbranchcode { get; set; }
    [DisplayName("شماره اقتصادی خریدار")]
    [DbMap("CUSTOMERECONOMICCODE_")]
    public string? Customereconomiccode { get; set; }
    [DisplayName("شناسه ملی /شماره ملی/شناسه مشارکت مدنی/کد فراگیر اتباع غیرایرانی خریدار")]
    [DbMap("CUSTOMERIDENTIFICATIONNUMBER_")]
    public string? Customeridentificationnumber { get; set; }
    [DisplayName("نوع شخص خریدار")]
    [DbMap("CUSTOMERPERSONTYPE_")]
    public byte? Customerpersontype { get; set; }
    [DisplayName("شماره اقتصادی فروشنده")]
    [DbMap("SELLERECONOMICCODE_")]
    public string? Sellereconomiccode { get; set; }
    [DisplayName("شناسه شخص خریدار")]
    [DbMap("CUSTOMERID_")]
    public int Customerid { get; set; }
    [DisplayName("شناسه شخص فروشنده")]
    [DbMap("SALESPERSONID_")]
    public int? Salespersonid { get; set; }
    [DisplayName("موضوع صورتحساب")]
    [DbMap("INVOICESUBJECT_")]
    public byte Invoicesubject { get; set; }
    [DisplayName("الگوی صورتحساب")]
    [DbMap("INVOICEFORMAT_")]
    public byte Invoiceformat { get; set; }
    [DisplayName("شماره منحصر به فرد مالیاتی صورتحساب مرجع")]
    [DbMap("REFERENCETAXUNIQUEID_")]
    public string? Referencetaxuniqueid { get; set; }
    [DisplayName("سریال صورتحساب داخلی حافظه مالیاتی")]
    [DbMap("TAXINVOICEINTERNALSERIAL_")]
    public string? Taxinvoiceinternalserial { get; set; }
    [DisplayName("نوع صورتحساب")]
    [DbMap("INVOICETYPE_")]
    public byte? Invoicetype { get; set; }
    [DisplayName("تاریخ و زمان ایجاد صورتحساب")]
    [DbMap("CREATIONDATETIME_")]
    public DateTime Creationdatetime { get; set; }
    [DisplayName("تاریخ و زمان صدور صورتحساب")]
    [DbMap("ISSUEDATETIME_")]
    public DateTime Issuedatetime { get; set; }
    [DisplayName("شماره منحصر به فرد مالیاتی")]
    [DbMap("TAXUNIQUEID_")]
    public string? Taxuniqueid { get; set; }
    [DisplayName("شماره صورتحساب")]
    [DbMap("INVOICENUMBER_")]
    public int Invoicenumber { get; set; }
    [DisplayName("نرخ تبدیل ارز")]
    [DbMap("EXCHANGERATE_")]
    public decimal Exchangerate { get; set; }
    [DisplayName("ارز")]
    [DbMap("CURRENCY_")]
    public string Currency { get; set; } = null!;
    [DisplayName("شناسه دوره مالی")]
    [DbMap("FISCALPERIODID_")]
    public int Fiscalperiodid { get; set; }
    [DisplayName("شناسه پروژه")]
    [DbMap("PROJECTID_")]
    public int? Projectid { get; set; }
    [DisplayName("شناسه انبار")]
    [DbMap("WAREHOUSEID_")]
    public int? Warehouseid { get; set; }
    [DisplayName("شناسه صورتحساب مرجع")]
    [DbMap("REFERENCESALEORDERID_")]
    public long? Referencesaleorderid { get; set; }
    [DisplayName("مبلغ باقیمانده")]
    [DbMap("REMAINEDAMOUNT_")]
    public decimal? Remainedamount { get; set; }
    [DisplayName("ایا رسید انبار به صورت خودکار ایجاد شده است")]
    [DbMap("ISSTOCKCARDAUTOCREATED_")]
    public bool Isstockcardautocreated { get; set; }
}
[DisplayName("تنظیمات فروش")]
[DbMap("TBL_SaleOrderConfig")]
public sealed class SqlTblSaleorderconfig : SqlServerEntity
{
    [DisplayName("شناسه تنظیمات")]
    [DbMap("CONFIGID_")]
    public int Configid { get; set; }
    [DisplayName("به‌روزرسانی قیمت فروش بعد از ثبت فاکتور فروش")]
    [DbMap("UPDATESALEPRICEONINVOICESAVE_")]
    public bool Updatesalepriceoninvoicesave { get; set; }
    [DisplayName("به‌روزرسانی قیمت خرید بعد از ثبت فاکتور خرید")]
    [DbMap("UPDATEPURCHASEPRICEONINVOICESAVE_")]
    public bool Updatepurchasepriceoninvoicesave { get; set; }
    [DisplayName("نمایش پیغام به‌روزرسانی قیمت")]
    [DbMap("NOTIFYUSERAFTERPRICEUPDATE_")]
    public bool Notifyuserafterpriceupdate { get; set; }
    [DisplayName("فروش بیشتر از موجودی حسابداری")]
    [DbMap("ALLOWLOWSTOCKSALE_")]
    public bool Allowlowstocksale { get; set; }
    [DisplayName("نمایش کالاهای ناموجود")]
    [DbMap("SHOWZERONEGATIVESTOCKITEMS_")]
    public bool Showzeronegativestockitems { get; set; }
    [DisplayName("ثبت ردیف تکراری در فاکتور")]
    [DbMap("ALLOWDUPLICATEITEMSININVOICE_")]
    public bool Allowduplicateitemsininvoice { get; set; }
    [DisplayName("ثبت فاکتور در صورت تجاوز از سقف اعتبار خریدار")]
    [DbMap("CHECKCUSTOMERCREDITONSALE_")]
    public bool Checkcustomercreditonsale { get; set; }
    [DisplayName("نمایش هشدار در صورت فروش با قیمتی کمتر از قیمت خرید")]
    [DbMap("WARNSALEBELOWPURCHASEPRICE_")]
    public bool Warnsalebelowpurchaseprice { get; set; }
    [DisplayName("نمایش سود در فاکتور")]
    [DbMap("SHOWPROFITININVOICE_")]
    public bool Showprofitininvoice { get; set; }
    [DisplayName("خریدار پیش‌فرض")]
    [DbMap("DEFAULTCUSTOMER_")]
    public int? Defaultcustomer { get; set; }
    [DisplayName("اجباری بودن انتخاب فروشنده")]
    [DbMap("SALESPERSONREQUIRED_")]
    public bool Salespersonrequired { get; set; }
    [DisplayName("ثبت خودکار حواله انبار")]
    [DbMap("ENABLEAUTOWAREHOUSEISSUE_")]
    public bool Enableautowarehouseissue { get; set; }
    [DisplayName("نحوه چاپ فاکتور")]
    [DbMap("INVOICEPRINTMETHOD_")]
    public string Invoiceprintmethod { get; set; } = null!;
    [DisplayName("فعال بودن ترازو")]
    [DbMap("ENABLESCALE_")]
    public bool Enablescale { get; set; }
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("نحوۀ باز شدن صفحۀ دریافت")]
    [DbMap("PAYMENTPAGEOPENINGMODE_")]
    public byte Paymentpageopeningmode { get; set; }
}
[DisplayName("اقلام فاکتور فروش")]
[DbMap("TBL_SaleOrderItem")]
public sealed class SqlTblSaleorderitem : SqlServerEntity
{
    [DisplayName("شناسه آیتم سفارش فروش")]
    [DbMap("SALEORDERITEMID_")]
    public long Saleorderitemid { get; set; }
    [DisplayName("شناسه سفارش فروش")]
    [DbMap("SALEORDERID_")]
    public long Saleorderid { get; set; }
    [DisplayName("شناسه محصول")]
    [DbMap("PRODUCTID_")]
    public int Productid { get; set; }
    [DisplayName("تعداد/مقدار")]
    [DbMap("QUANTITY_")]
    public decimal? Quantity { get; set; }
    [DisplayName("واحد اندازه‌گیری")]
    [DbMap("UNIT_")]
    public short? Unit { get; set; }
    [DisplayName("قیمت واحد")]
    [DbMap("PRICE_")]
    public decimal? Price { get; set; }
    [DisplayName("تخفیف درصدی")]
    [DbMap("LINEPERCENTDISCOUNT_")]
    public decimal? Linepercentdiscount { get; set; }
    [DisplayName("مبلغ کل کالا/خدمت")]
    [DbMap("LINETOTALAMOUNT_")]
    public decimal Linetotalamount { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("بهای تمام شده")]
    [DbMap("COST_")]
    public decimal? Cost { get; set; }
    [DisplayName("نرخ مالیات بر ارزش افزوده")]
    [DbMap("VATRATE_")]
    public decimal Vatrate { get; set; }
    [DisplayName("ضریب تبدیل واحد به واحد مرجع")]
    [DbMap("UNITCONVERSIONFACTOR_")]
    public decimal? Unitconversionfactor { get; set; }
    [DisplayName("مبلغ مالیات بر ارزش افزوده")]
    [DbMap("VATAMOUNT_")]
    public decimal Vatamount { get; set; }
    [DisplayName("شرح کالا/خدمت")]
    [DbMap("PRODUCTDESCRIPTION_")]
    public string? Productdescription { get; set; }
    [DisplayName("شناسه مالیاتی کالا/خدمت")]
    [DbMap("PRODUCTTAXCODE_")]
    public string? Producttaxcode { get; set; }
    [DisplayName("شناسه پروژه")]
    [DbMap("PROJECTID_")]
    public int? Projectid { get; set; }
    [DisplayName("شناسه انبار")]
    [DbMap("LINEWAREHOUSEID_")]
    public int? Linewarehouseid { get; set; }
    [DisplayName("شناسه دوره مالی")]
    [DbMap("FISCALPERIODID_")]
    public int Fiscalperiodid { get; set; }
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("مبلغ پایه مالیات بر ارزش افزوده")]
    [DbMap("VATBASEAMOUNT_")]
    public decimal? Vatbaseamount { get; set; }
    [DisplayName("ماخذ مالیات بر ارزش افزوده در الگوی فروش ارز")]
    [DbMap("VATCALCULATIONBASE_")]
    public decimal? Vatcalculationbase { get; set; }
    [DisplayName("نرخ خرید ارز")]
    [DbMap("CURRENCYBUYRATE_")]
    public decimal? Currencybuyrate { get; set; }
    [DisplayName("عیار")]
    [DbMap("PURITY_")]
    public decimal? Purity { get; set; }
    [DisplayName("جمع کل اجرت، حق‌العمل و سود")]
    [DbMap("TOTALMAKINGCOMMISSIONPROFIT_")]
    public decimal? Totalmakingcommissionprofit { get; set; }
    [DisplayName("حق‌العمل")]
    [DbMap("COMMISSION_")]
    public decimal? Commission { get; set; }
    [DisplayName("سود فروشنده")]
    [DbMap("SELLERPROFIT_")]
    public decimal? Sellerprofit { get; set; }
    [DisplayName("اجرت ساخت")]
    [DbMap("MAKINGCHARGE_")]
    public decimal? Makingcharge { get; set; }
    [DisplayName("ارزش ارزی کالا")]
    [DbMap("LINECURRENCYVALUE_")]
    public decimal? Linecurrencyvalue { get; set; }
    [DisplayName("شماره قرارداد حق العمل کاری")]
    [DbMap("COMMISSIONCONTRACTNUMBER_")]
    public string? Commissioncontractnumber { get; set; }
    [DisplayName("ارزش ریالی کالا")]
    [DbMap("LINERIALVALUE_")]
    public decimal? Linerialvalue { get; set; }
    [DisplayName("وزن خالص")]
    [DbMap("NETWEIGHT_")]
    public decimal? Netweight { get; set; }
    [DisplayName("سهم مالیات بر ارزش افزوده از پرداخت")]
    [DbMap("LINEVATPAIDAMOUNT_")]
    public decimal? Linevatpaidamount { get; set; }
    [DisplayName("سهم نقدی از پرداخت")]
    [DbMap("LINECASHPAIDAMOUNT_")]
    public decimal? Linecashpaidamount { get; set; }
    [DisplayName("مبلغ سایر وجوه قانونی")]
    [DbMap("OTHERLEGALDUESAMOUNT_")]
    public decimal? Otherlegalduesamount { get; set; }
    [DisplayName("نرخ سایر وجوه قانونی")]
    [DbMap("OTHERLEGALDUESRATE_")]
    public decimal? Otherlegalduesrate { get; set; }
    [DisplayName("موضوع سایر وجوه قانونی")]
    [DbMap("OTHERLEGALDUESSUBJECT_")]
    public string? Otherlegalduessubject { get; set; }
    [DisplayName("مبلغ سایر مالیات و عوارض")]
    [DbMap("OTHERTAXESANDCHARGESAMOUNT_")]
    public decimal? Othertaxesandchargesamount { get; set; }
    [DisplayName("نرخ سایر مالیات و عوارض")]
    [DbMap("OTHERTAXESANDCHARGESRATE_")]
    public decimal? Othertaxesandchargesrate { get; set; }
    [DisplayName("موضوع سایر مالیات و عوارض")]
    [DbMap("OTHERTAXESANDCHARGESSUBJECT_")]
    public string? Othertaxesandchargessubject { get; set; }
    [DisplayName("مبلغ بعد از تخفیف")]
    [DbMap("LINEAMOUNTAFTERDISCOUNT_")]
    public decimal Lineamountafterdiscount { get; set; }
    [DisplayName("مبلغ تخفیف")]
    [DbMap("LINEDISCOUNTAMOUNT_")]
    public decimal Linediscountamount { get; set; }
    [DisplayName("مبلغ قبل از تخفیف")]
    [DbMap("LINEAMOUNTBEFOREDISCOUNT_")]
    public decimal Lineamountbeforediscount { get; set; }
    [DisplayName("میزان ارز")]
    [DbMap("LINECURRENCYAMOUNT_")]
    public decimal? Linecurrencyamount { get; set; }
    [DisplayName("نرخ تبدیل ارز")]
    [DbMap("LINEEXCHANGERATE_")]
    public decimal? Lineexchangerate { get; set; }
    [DisplayName("ارز")]
    [DbMap("LINECURRENCY_")]
    public string? Linecurrency { get; set; }
}
[DisplayName("فاکتور برگشت از فروش")]
[DbMap("TBL_SaleOrderReturn")]
public sealed class SqlTblSaleorderreturn : SqlServerEntity
{
    [DisplayName("شناسه صورتحساب برگشتی")]
    [DbMap("SALEORDERRETURNID_")]
    public long Saleorderreturnid { get; set; }
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("شناسه دوره مالی")]
    [DbMap("FISCALPERIODID_")]
    public int Fiscalperiodid { get; set; }
    [DisplayName("شناسه انبار")]
    [DbMap("WAREHOUSEID_")]
    public int? Warehouseid { get; set; }
    [DisplayName("شناسه پروژه")]
    [DbMap("PROJECTID_")]
    public int? Projectid { get; set; }
    [DisplayName("شناسه صورتحساب مرجع")]
    [DbMap("REFERENCESALEORDERID_")]
    public long Referencesaleorderid { get; set; }
    [DisplayName("شماره صورتحساب برگشتی")]
    [DbMap("INVOICENUMBER_")]
    public int Invoicenumber { get; set; }
    [DisplayName("تاریخ و زمان صدور صورتحساب برگشتی")]
    [DbMap("ISSUEDATETIME_")]
    public DateTime Issuedatetime { get; set; }
    [DisplayName("تاریخ و زمان ایجاد صورتحساب برگشتی")]
    [DbMap("CREATIONDATETIME_")]
    public DateTime Creationdatetime { get; set; }
    [DisplayName("وضعیت صورتحساب برگشتی")]
    [DbMap("STATUS_")]
    public byte Status { get; set; }
    [DisplayName("وضعیت تحویل")]
    [DbMap("DELIVERYSTATUS_")]
    public byte? Deliverystatus { get; set; }
    [DisplayName("وضعیت انبار")]
    [DbMap("INVENTORYSTATUS_")]
    public byte? Inventorystatus { get; set; }
    [DisplayName("وضعیت پرداخت")]
    [DbMap("PAYMENTSTATUS_")]
    public byte Paymentstatus { get; set; }
    [DisplayName("مجموع مبلغ برگشتی قبل از کسر تخفیف")]
    [DbMap("TOTALRETURNAMOUNTBEFOREDISCOUNT_")]
    public decimal Totalreturnamountbeforediscount { get; set; }
    [DisplayName("مجموع مبلغ برگشتی تخفیفات")]
    [DbMap("TOTALDISCOUNTRETURNAMOUNT_")]
    public decimal Totaldiscountreturnamount { get; set; }
    [DisplayName("مجموع مبلغ برگشتی پس از کسر تخفیف")]
    [DbMap("TOTALRETURNAMOUNTAFTERDISCOUNT_")]
    public decimal Totalreturnamountafterdiscount { get; set; }
    [DisplayName("مجموع مبلغ برگشتی مالیات بر ارزش افزوده")]
    [DbMap("TOTALVATRETURNAMOUNT_")]
    public decimal Totalvatreturnamount { get; set; }
    [DisplayName("مجموع مبلغ برگشتی سایر مالیات، عوارض و وجوه قانونی")]
    [DbMap("TOTALOTHERTAXESANDCHARGESRETURNAMOUNT_")]
    public decimal? Totalothertaxesandchargesreturnamount { get; set; }
    [DisplayName("مجموع مبلغ برگشتی صورتحساب")]
    [DbMap("TOTALINVOICERETURNAMOUNT_")]
    public decimal Totalinvoicereturnamount { get; set; }
    [DisplayName("مبلغ برگشتی پرداخت شده")]
    [DbMap("PAIDRETURNAMOUNT_")]
    public decimal Paidreturnamount { get; set; }
    [DisplayName("مبلغ برگشتی قابل پرداخت")]
    [DbMap("TOTALRETURNAMOUNT_")]
    public decimal Totalreturnamount { get; set; }
    [DisplayName("توضیحات")]
    [DbMap("DESCRIPTION_")]
    public string? Description { get; set; }
    [DisplayName("اضافات و کسورات")]
    [DbMap("ADJUSTMENTS_")]
    public string? Adjustments { get; set; }
    [DisplayName("مابه‌التفاوت اضافات، کسورات")]
    [DbMap("ADJUSTMENTSAMOUNT_")]
    public decimal Adjustmentsamount { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("اقلام فاکتور برگشت از فروش")]
[DbMap("TBL_SaleOrderReturnItem")]
public sealed class SqlTblSaleorderreturnitem : SqlServerEntity
{
    [DisplayName("شناسه آیتم برگشتی")]
    [DbMap("SALEORDERRETURNITEMID_")]
    public long Saleorderreturnitemid { get; set; }
    [DisplayName("شناسه آیتم مرجع")]
    [DbMap("REFERENCESALEORDERITEMID_")]
    public long Referencesaleorderitemid { get; set; }
    [DisplayName("شناسه صورتحساب برگشتی")]
    [DbMap("SALEORDERRETURNID_")]
    public long Saleorderreturnid { get; set; }
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("شناسه دوره مالی")]
    [DbMap("FISCALPERIODID_")]
    public int Fiscalperiodid { get; set; }
    [DisplayName("شناسه انبار")]
    [DbMap("LINEWAREHOUSEID_")]
    public int? Linewarehouseid { get; set; }
    [DisplayName("شناسه پروژه")]
    [DbMap("PROJECTID_")]
    public int? Projectid { get; set; }
    [DisplayName("شناسه محصول")]
    [DbMap("PRODUCTID_")]
    public int Productid { get; set; }
    [DisplayName("تعداد/مقدار برگشتی")]
    [DbMap("RETURNQUANTITY_")]
    public decimal Returnquantity { get; set; }
    [DisplayName("واحد اندازه‌گیری")]
    [DbMap("UNIT_")]
    public short Unit { get; set; }
    [DisplayName("ضریب تبدیل واحد")]
    [DbMap("UNITCONVERSIONFACTOR_")]
    public decimal? Unitconversionfactor { get; set; }
    [DisplayName("قیمت واحد")]
    [DbMap("PRICE_")]
    public decimal Price { get; set; }
    [DisplayName("مبلغ برگشتی قبل از تخفیف")]
    [DbMap("LINERETURNAMOUNTBEFOREDISCOUNT_")]
    public decimal Linereturnamountbeforediscount { get; set; }
    [DisplayName("تخفیف درصدی")]
    [DbMap("LINEPERCENTDISCOUNT_")]
    public decimal? Linepercentdiscount { get; set; }
    [DisplayName("مبلغ برگشتی تخفیف")]
    [DbMap("LINEDISCOUNTRETURNAMOUNT_")]
    public decimal Linediscountreturnamount { get; set; }
    [DisplayName("مبلغ برگشتی بعد از تخفیف")]
    [DbMap("LINERETURNAMOUNTAFTERDISCOUNT_")]
    public decimal Linereturnamountafterdiscount { get; set; }
    [DisplayName("نرخ مالیات بر ارزش افزوده")]
    [DbMap("VATRATE_")]
    public decimal Vatrate { get; set; }
    [DisplayName("مبلغ برگشتی مالیات بر ارزش افزوده")]
    [DbMap("VATRETURNAMOUNT_")]
    public decimal Vatreturnamount { get; set; }
    [DisplayName("مبلغ کل برگشتی کالا/خدمت")]
    [DbMap("LINETOTALRETURNAMOUNT_")]
    public decimal Linetotalreturnamount { get; set; }
    [DisplayName("مبلغ برگشتی سایر مالیات و عوارض")]
    [DbMap("OTHERTAXESANDCHARGESRETURNAMOUNT_")]
    public decimal? Othertaxesandchargesreturnamount { get; set; }
    [DisplayName("مبلغ برگشتی سایر وجوه قانونی")]
    [DbMap("OTHERLEGALDUESRETURNAMOUNT_")]
    public decimal? Otherlegalduesreturnamount { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}