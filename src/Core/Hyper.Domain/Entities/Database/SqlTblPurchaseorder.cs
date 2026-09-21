#nullable enable
namespace Hyper.Domain.Entities.Database;

[DisplayName("فاکتور خرید")]
[DbMap("TBL_PurchaseOrder")]
public sealed class SqlTblPurchaseorder : SqlServerEntity
{
    [DisplayName("شناسه سفارش خرید")]
    [DbMap("PURCHASEORDERID_")]
    public long Purchaseorderid { get; set; }
    [DisplayName("مجموع مبلغ قبل از کسر تخفیف")]
    [DbMap("TOTALAMOUNTBEFOREDISCOUNT_")]
    public decimal Totalamountbeforediscount { get; set; }
    [DisplayName("مجموع مالیات بر ارزش افزوده")]
    [DbMap("TOTALVATAMOUNT_")]
    public decimal Totalvatamount { get; set; }
    [DisplayName("مجموع صورتحساب")]
    [DbMap("TOTALINVOICEAMOUNT_")]
    public decimal Totalinvoiceamount { get; set; }
    [DisplayName("مجموع تخفیفات")]
    [DbMap("TOTALDISCOUNTAMOUNT_")]
    public decimal Totaldiscountamount { get; set; }
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
    [DisplayName("نوع پرواز")]
    [DbMap("FLIGHTTYPE_")]
    public byte? Flighttype { get; set; }
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
    [DisplayName("شماره منحصر به فرد مالیاتی")]
    [DbMap("TAXUNIQUEID_")]
    public string? Taxuniqueid { get; set; }
    [DisplayName("مابه‌التفاوت اضافات و کسورات")]
    [DbMap("ADJUSTMENTSAMOUNT_")]
    public decimal Adjustmentsamount { get; set; }
    [DisplayName("مجموع سایر مالیات، عوارض و وجوه قانونی")]
    [DbMap("TOTALOTHERTAXESANDCHARGESAMOUNT_")]
    public decimal? Totalothertaxesandchargesamount { get; set; }
    [DisplayName("کد گمرک محل اظهار فروشنده")]
    [DbMap("SELLERCUSTOMSOFFICECODE_")]
    public string? Sellercustomsofficecode { get; set; }
    [DisplayName("اضافات و کسورات")]
    [DbMap("ADJUSTMENTS_")]
    public string? Adjustments { get; set; }
    [DisplayName("تاریخ سر رسید")]
    [DbMap("DUEDATE_")]
    public DateOnly? Duedate { get; set; }
    [DisplayName("شماره پروانه گمرکی")]
    [DbMap("CUSTOMSPERMITNUMBER_")]
    public string? Customspermitnumber { get; set; }
    [DisplayName("توضیحات")]
    [DbMap("DESCRIPTION_")]
    public string? Description { get; set; }
    [DisplayName("الگوی صورتحساب")]
    [DbMap("INVOICEFORMAT_")]
    public byte Invoiceformat { get; set; }
    [DisplayName("سریال صورتحساب داخلی حافظه مالیاتی")]
    [DbMap("TAXINVOICEINTERNALSERIAL_")]
    public string? Taxinvoiceinternalserial { get; set; }
    [DisplayName("مبلغ قابل پرداخت")]
    [DbMap("TOTALAMOUNT_")]
    public decimal Totalamount { get; set; }
    [DisplayName("مبلغ پرداخت شده")]
    [DbMap("PAIDAMOUNT_")]
    public decimal Paidamount { get; set; }
    [DisplayName("نوع صورتحساب")]
    [DbMap("INVOICETYPE_")]
    public byte? Invoicetype { get; set; }
    [DisplayName("مجموع مبلغ پس از کسر تخفیف")]
    [DbMap("TOTALAMOUNTAFTERDISCOUNT_")]
    public decimal Totalamountafterdiscount { get; set; }
    [DisplayName("وضعیت پرداخت")]
    [DbMap("PAYMENTSTATUS_")]
    public byte Paymentstatus { get; set; }
    [DisplayName("وضعیت صدور رسید انبار")]
    [DbMap("INVENTORYSTATUS_")]
    public byte? Inventorystatus { get; set; }
    [DisplayName("وضعیت تحویل")]
    [DbMap("DELIVERYSTATUS_")]
    public byte? Deliverystatus { get; set; }
    [DisplayName("شناسه شخص تامین‌کننده")]
    [DbMap("SUPPLIERID_")]
    public int Supplierid { get; set; }
    [DisplayName("وضعیت صورتحساب")]
    [DbMap("STATUS_")]
    public byte Status { get; set; }
    [DisplayName("تاریخ و زمان صدور صورتحساب")]
    [DbMap("ISSUEDATETIME_")]
    public DateTime Issuedatetime { get; set; }
    [DisplayName("شماره صورتحساب")]
    [DbMap("INVOICENUMBER_")]
    public int Invoicenumber { get; set; }
    [DisplayName("نرخ تبدیل ارز")]
    [DbMap("EXCHANGERATE_")]
    public decimal Exchangerate { get; set; }
    [DisplayName("ارز")]
    [DbMap("CURRENCY_")]
    public string Currency { get; set; } = null!;
    [DisplayName("شناسه پروژه")]
    [DbMap("PROJECTID_")]
    public int? Projectid { get; set; }
    [DisplayName("شناسه انبار")]
    [DbMap("WAREHOUSEID_")]
    public int? Warehouseid { get; set; }
    [DisplayName("شناسه دوره مالی")]
    [DbMap("FISCALPERIODID_")]
    public int Fiscalperiodid { get; set; }
    [DisplayName("مبلغ باقیمانده")]
    [DbMap("REMAINEDAMOUNT_")]
    public decimal? Remainedamount { get; set; }
    [DisplayName("ایا رسید انبار به صورت خودکار ایجاد شده است")]
    [DbMap("ISSTOCKCARDAUTOCREATED_")]
    public bool Isstockcardautocreated { get; set; }
}
[DisplayName("اقلام فاکتور خرید")]
[DbMap("TBL_PurchaseOrderItem")]
public sealed class SqlTblPurchaseorderitem : SqlServerEntity
{
    [DisplayName("شناسه آیتم سفارش خرید")]
    [DbMap("PURCHASEORDERITEMID_")]
    public long Purchaseorderitemid { get; set; }
    [DisplayName("شناسه سفارش خرید")]
    [DbMap("PURCHASEORDERID_")]
    public long Purchaseorderid { get; set; }
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
    [DisplayName("نرخ مالیات بر ارزش افزوده")]
    [DbMap("VATRATE_")]
    public decimal Vatrate { get; set; }
    [DisplayName("ضریب تبدیل واحد")]
    [DbMap("UNITCONVERSIONFACTOR_")]
    public decimal? Unitconversionfactor { get; set; }
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
    [DisplayName("ارزش ریالی کالا")]
    [DbMap("LINERIALVALUE_")]
    public decimal? Linerialvalue { get; set; }
    [DisplayName("وزن خالص")]
    [DbMap("NETWEIGHT_")]
    public decimal? Netweight { get; set; }
    [DisplayName("مبلغ بعد از تخفیف")]
    [DbMap("LINEAMOUNTAFTERDISCOUNT_")]
    public decimal Lineamountafterdiscount { get; set; }
    [DisplayName("شماره قرارداد حق العمل کاری")]
    [DbMap("COMMISSIONCONTRACTNUMBER_")]
    public string? Commissioncontractnumber { get; set; }
    [DisplayName("مبلغ تخفیف")]
    [DbMap("LINEDISCOUNTAMOUNT_")]
    public decimal Linediscountamount { get; set; }
    [DisplayName("مبلغ قبل از تخفیف")]
    [DbMap("LINEAMOUNTBEFOREDISCOUNT_")]
    public decimal Lineamountbeforediscount { get; set; }
    [DisplayName("میزان ارز")]
    [DbMap("LINECURRENCYAMOUNT_")]
    public decimal? Linecurrencyamount { get; set; }
    [DisplayName("مبلغ مالیات بر ارزش افزوده")]
    [DbMap("VATAMOUNT_")]
    public decimal Vatamount { get; set; }
    [DisplayName("نرخ تبدیل ارز")]
    [DbMap("LINEEXCHANGERATE_")]
    public decimal? Lineexchangerate { get; set; }
    [DisplayName("ارز")]
    [DbMap("LINECURRENCY_")]
    public string? Linecurrency { get; set; }
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
}
[DisplayName("فاکتور برگشت از خرید")]
[DbMap("TBL_PurchaseOrderReturn")]
public sealed class SqlTblPurchaseorderreturn : SqlServerEntity
{
    [DisplayName("شناسه صورتحساب برگشتی")]
    [DbMap("PURCHASEORDERRETURNID_")]
    public long Purchaseorderreturnid { get; set; }
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
    [DbMap("REFERENCEPURCHASEORDERID_")]
    public long Referencepurchaseorderid { get; set; }
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
    [DisplayName("وضعیت دریافت")]
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
    [DisplayName("مجموع مبلغ برگشتی صورتحساب")]
    [DbMap("TOTALINVOICERETURNAMOUNT_")]
    public decimal Totalinvoicereturnamount { get; set; }
    [DisplayName("مبلغ برگشتی دریافت شده")]
    [DbMap("PAIDRETURNAMOUNT_")]
    public decimal Paidreturnamount { get; set; }
    [DisplayName("مبلغ برگشتی قابل دریافت")]
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
[DisplayName("اقلام فاکتور برگشت از خرید")]
[DbMap("TBL_PurchaseOrderReturnItem")]
public sealed class SqlTblPurchaseorderreturnitem : SqlServerEntity
{
    [DisplayName("شناسه آیتم برگشتی")]
    [DbMap("PURCHASEORDERRETURNITEMID_")]
    public long Purchaseorderreturnitemid { get; set; }
    [DisplayName("شناسه آیتم مرجع")]
    [DbMap("REFERENCEPURCHASEORDERITEMID_")]
    public long Referencepurchaseorderitemid { get; set; }
    [DisplayName("شناسه صورتحساب برگشتی")]
    [DbMap("PURCHASEORDERRETURNID_")]
    public long Purchaseorderreturnid { get; set; }
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
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("پروژه")]
[DbMap("TBL_Project")]
public sealed class SqlTblProject : SqlServerEntity
{
    [DisplayName("شناسه پروژه")]
    [DbMap("PROJECTID_")]
    public int Projectid { get; set; }
    [DisplayName("شناسه مغازه")]
    [DbMap("SHOPID_")]
    public int Shopid { get; set; }
    [DisplayName("نام پروژه")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("پروژه پیش‌فرض")]
    [DbMap("ISDEFAULT_")]
    public bool Isdefault { get; set; }
    [DisplayName("پروژه فعال")]
    [DbMap("ISENABLED_")]
    public bool Isenabled { get; set; }
    [DisplayName("توضیحات")]
    [DbMap("DESCRIPTION_")]
    public string? Description { get; set; }
    [DisplayName("کد پروژه")]
    [DbMap("CODE_")]
    public byte Code { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
