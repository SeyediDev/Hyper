namespace Hyper.Domain.Entities.Database;

[DisplayName("حساب مانده")]
[DbMap("View_AccountBalance")]
public sealed class SqlViewAccountbalance : SqlServerEntity
{
    [DisplayName("سند تاریخ")]
    [DbMap("documentDate")]
    public DateOnly Documentdate { get; set; }
    [DisplayName("حساب شناسه")]
    [DbMap("accountId")]
    public int Accountid { get; set; }
    [DisplayName("سند نوع شناسه")]
    [DbMap("documentTypeId")]
    public byte Documenttypeid { get; set; }
    [DisplayName("مجموع بدهکار")]
    [DbMap("totalDebit")]
    public decimal? Totaldebit { get; set; }
    [DisplayName("مجموع بستانکار")]
    [DbMap("totalCredit")]
    public decimal? Totalcredit { get; set; }
    [DisplayName("تعداد")]
    [DbMap("cnt")]
    public long? Cnt { get; set; }
}
[DisplayName("تفصیلی حساب مانده")]
[DbMap("View_DetailAccountBalance")]
public sealed class SqlViewDetailaccountbalance : SqlServerEntity
{
    [DisplayName("حساب شناسه")]
    [DbMap("accountId")]
    public int Accountid { get; set; }
    [DisplayName("تفصیلی حساب شناسه")]
    [DbMap("detailAccountId")]
    public long? Detailaccountid { get; set; }
    [DisplayName("مجموع بدهکار")]
    [DbMap("totalDebit")]
    public decimal? Totaldebit { get; set; }
    [DisplayName("مجموع بستانکار")]
    [DbMap("totalCredit")]
    public decimal? Totalcredit { get; set; }
    [DisplayName("تعداد")]
    [DbMap("cnt")]
    public long? Cnt { get; set; }
}

/// <summary>
/// ماهانه‌ی قیف اشتراک برای گزارش‌های بازاریابی داشبورد اصلی.
/// مشتری جدید فقط اولین درخواست پولی هر مغازه است و retention
/// مغازه‌هایی را می‌شمارد که در ماه قبل نیز اشتراک پولی داشته‌اند.
/// </summary>
[Neo.Bpms.Domain.Models.Attributes.EntityAttributes.View(SqlVwMarketingsubscriptionmonthlyQuery.Sql, true)]
[DisplayName("تحلیل ماهانه اشتراک بازاریابی")]
[DbMap("vw_SqlVwMarketingsubscriptionmonthly")]
public sealed class SqlVwMarketingsubscriptionmonthly : SqlServerEntity
{
    [DisplayName("ماه")]
    [DbMap("MonthStart")]
    public DateOnly Monthstart { get; set; }

    [DisplayName("مستاجر شناسه")]
    [DbMap("TenantId")]
    public string Tenantid { get; set; } = null!;

    [DisplayName("مغازه‌های واجد شرایط")]
    [DbMap("EligibleShopCount")]
    public long Eligibleshopcount { get; set; }

    [DisplayName("مشتری‌های جدید پولی")]
    [DbMap("NewPaidShopCount")]
    public long Newpaidshopcount { get; set; }

    [DisplayName("مغازه‌های فعال پولی")]
    [DbMap("ActivePaidShopCount")]
    public long Activepaidshopcount { get; set; }

    [DisplayName("مغازه‌های حفظ‌شده")]
    [DbMap("RetainedShopCount")]
    public long Retainedshopcount { get; set; }

    [DisplayName("نرخ تبدیل")]
    [DbMap("ConversionRate")]
    public decimal Conversionrate { get; set; }

    [DisplayName("نرخ حفظ")]
    [DbMap("RetentionRate")]
    public decimal Retentionrate { get; set; }
}

internal static class SqlVwMarketingsubscriptionmonthlyQuery
{
    public const string Sql = """
WITH MonthSeries AS (
    SELECT CAST(DATEADD(month, -11, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) AS date) MonthStart
    UNION ALL SELECT CAST(DATEADD(month, 1, MonthStart) AS date) FROM MonthSeries
    WHERE MonthStart < CAST(DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1) AS date)
), Tenants AS (
    SELECT DISTINCT ISNULL(TENANT_ID_, '') TenantId FROM TBL_ServiceRequest WHERE SHOPID_ IS NOT NULL
), Eligible AS (
    SELECT DISTINCT ISNULL(TENANT_ID_, '') TenantId, SHOPID_ ShopId, CAST(DATEFROMPARTS(YEAR(REQUESTTIME_), MONTH(REQUESTTIME_), 1) AS date) MonthStart
    FROM TBL_ServiceRequest WHERE SHOPID_ IS NOT NULL
), Paid AS (
    SELECT ISNULL(r.TENANT_ID_, '') TenantId, r.SHOPID_ ShopId, r.REQUESTTIME_ RequestTime
    FROM TBL_ServiceRequest r LEFT JOIN TBL_ServicePayment p ON p.PAYMENTID_ = r.PAYMENTID_
    WHERE r.SHOPID_ IS NOT NULL AND r.PRICE_ > 0 AND (r.PAYMENTID_ IS NULL OR p.ISSUCCESS_ = 1)
), FirstPaid AS (
    SELECT TenantId, ShopId, MIN(RequestTime) FirstPaidAt FROM Paid GROUP BY TenantId, ShopId
), PaidMonths AS (
    SELECT DISTINCT TenantId, ShopId, CAST(DATEFROMPARTS(YEAR(RequestTime), MONTH(RequestTime), 1) AS date) MonthStart FROM Paid
), EligibleByMonth AS (
    SELECT TenantId, MonthStart, COUNT_BIG(*) EligibleShopCount FROM Eligible GROUP BY TenantId, MonthStart
), NewByMonth AS (
    SELECT TenantId, CAST(DATEFROMPARTS(YEAR(FirstPaidAt), MONTH(FirstPaidAt), 1) AS date) MonthStart, COUNT_BIG(*) NewPaidShopCount
    FROM FirstPaid GROUP BY TenantId, CAST(DATEFROMPARTS(YEAR(FirstPaidAt), MONTH(FirstPaidAt), 1) AS date)
), ActiveByMonth AS (
    SELECT TenantId, MonthStart, COUNT_BIG(*) ActivePaidShopCount FROM PaidMonths GROUP BY TenantId, MonthStart
), RetainedByMonth AS (
    SELECT c.TenantId, c.MonthStart, COUNT_BIG(*) RetainedShopCount FROM PaidMonths c
    INNER JOIN PaidMonths p ON p.TenantId = c.TenantId AND p.ShopId = c.ShopId AND p.MonthStart = DATEADD(month, -1, c.MonthStart)
    GROUP BY c.TenantId, c.MonthStart
)
SELECT m.MonthStart, t.TenantId, ISNULL(e.EligibleShopCount, 0) EligibleShopCount, ISNULL(n.NewPaidShopCount, 0) NewPaidShopCount,
       ISNULL(a.ActivePaidShopCount, 0) ActivePaidShopCount, ISNULL(r.RetainedShopCount, 0) RetainedShopCount,
       CAST(CASE WHEN ISNULL(e.EligibleShopCount, 0) = 0 THEN 0 ELSE 100.0 * ISNULL(n.NewPaidShopCount, 0) / e.EligibleShopCount END AS decimal(9,2)) ConversionRate,
       CAST(CASE WHEN ISNULL(pa.ActivePaidShopCount, 0) = 0 THEN 0 ELSE 100.0 * ISNULL(r.RetainedShopCount, 0) / pa.ActivePaidShopCount END AS decimal(9,2)) RetentionRate
FROM Tenants t CROSS JOIN MonthSeries m
LEFT JOIN EligibleByMonth e ON e.TenantId = t.TenantId AND e.MonthStart = m.MonthStart
LEFT JOIN NewByMonth n ON n.TenantId = t.TenantId AND n.MonthStart = m.MonthStart
LEFT JOIN ActiveByMonth a ON a.TenantId = t.TenantId AND a.MonthStart = m.MonthStart
LEFT JOIN RetainedByMonth r ON r.TenantId = t.TenantId AND r.MonthStart = m.MonthStart
LEFT JOIN ActiveByMonth pa ON pa.TenantId = t.TenantId AND pa.MonthStart = DATEADD(month, -1, m.MonthStart)
""";
}

[Neo.Bpms.Domain.Models.Attributes.EntityAttributes.View(SqlVwMarketinggmvmonthlyQuery.Sql, true)]
[DisplayName("گزارش ماهانه GMV خرید و فروش")]
[DbMap("vw_SqlVwMarketinggmvmonthly")]
public sealed class SqlVwMarketinggmvmonthly : SqlServerEntity
{
    [DisplayName("ماه")][DbMap("MonthStart")] public DateOnly Monthstart { get; set; }
    [DisplayName("مستاجر شناسه")][DbMap("TenantId")] public string Tenantid { get; set; } = null!;
    [DisplayName("GMV خرید")][DbMap("PurchaseGmv")] public decimal Purchasegmv { get; set; }
    [DisplayName("تعداد فاکتور خرید")][DbMap("PurchaseInvoiceCount")] public long Purchaseinvoicecount { get; set; }
    [DisplayName("تعداد اقلام خرید")][DbMap("PurchaseItemCount")] public long Purchaseitemcount { get; set; }
    [DisplayName("حجم کالای خرید")][DbMap("PurchaseQuantity")] public decimal Purchasequantity { get; set; }
    [DisplayName("GMV فروش")][DbMap("SaleGmv")] public decimal Salegmv { get; set; }
    [DisplayName("تعداد فاکتور فروش")][DbMap("SaleInvoiceCount")] public long Saleinvoicecount { get; set; }
    [DisplayName("تعداد اقلام فروش")][DbMap("SaleItemCount")] public long Saleitemcount { get; set; }
    [DisplayName("حجم کالای فروش")][DbMap("SaleQuantity")] public decimal Salequantity { get; set; }
}

internal static class SqlVwMarketinggmvmonthlyQuery
{
    public const string Sql = """
WITH MonthSeries AS (
 SELECT CAST(DATEADD(month,-11,DATEFROMPARTS(YEAR(GETDATE()),MONTH(GETDATE()),1)) AS date) MonthStart
 UNION ALL SELECT CAST(DATEADD(month,1,MonthStart) AS date) FROM MonthSeries WHERE MonthStart < CAST(DATEFROMPARTS(YEAR(GETDATE()),MONTH(GETDATE()),1) AS date)
), Tenants AS (
 SELECT DISTINCT ISNULL(TENANT_ID_,'') TenantId FROM TBL_SaleOrder WHERE SHOPID_ IS NOT NULL
 UNION SELECT DISTINCT ISNULL(TENANT_ID_,'') FROM TBL_PurchaseOrder WHERE SHOPID_ IS NOT NULL
), Purchases AS (
 SELECT ISNULL(o.TENANT_ID_,'') TenantId, CAST(DATEFROMPARTS(YEAR(o.ISSUEDATETIME_),MONTH(o.ISSUEDATETIME_),1) AS date) MonthStart, SUM(o.TOTALINVOICEAMOUNT_) Gmv, COUNT_BIG(*) InvoiceCount, ISNULL(SUM(i.ItemCount),0) ItemCount, ISNULL(SUM(i.Quantity),0) Quantity
 FROM TBL_PurchaseOrder o LEFT JOIN (SELECT PURCHASEORDERID_,COUNT_BIG(*) ItemCount,SUM(ISNULL(QUANTITY_,0)) Quantity FROM TBL_PurchaseOrderItem GROUP BY PURCHASEORDERID_) i ON i.PURCHASEORDERID_=o.PURCHASEORDERID_
 WHERE o.SHOPID_ IS NOT NULL GROUP BY ISNULL(o.TENANT_ID_,''),CAST(DATEFROMPARTS(YEAR(o.ISSUEDATETIME_),MONTH(o.ISSUEDATETIME_),1) AS date)
), Sales AS (
 SELECT ISNULL(o.TENANT_ID_,'') TenantId, CAST(DATEFROMPARTS(YEAR(o.ISSUEDATETIME_),MONTH(o.ISSUEDATETIME_),1) AS date) MonthStart, SUM(o.TOTALINVOICEAMOUNT_) Gmv, COUNT_BIG(*) InvoiceCount, ISNULL(SUM(i.ItemCount),0) ItemCount, ISNULL(SUM(i.Quantity),0) Quantity
 FROM TBL_SaleOrder o LEFT JOIN (SELECT SALEORDERID_,COUNT_BIG(*) ItemCount,SUM(ISNULL(QUANTITY_,0)) Quantity FROM TBL_SaleOrderItem GROUP BY SALEORDERID_) i ON i.SALEORDERID_=o.SALEORDERID_
 WHERE o.SHOPID_ IS NOT NULL GROUP BY ISNULL(o.TENANT_ID_,''),CAST(DATEFROMPARTS(YEAR(o.ISSUEDATETIME_),MONTH(o.ISSUEDATETIME_),1) AS date)
)
SELECT m.MonthStart,t.TenantId,ISNULL(p.Gmv,0) PurchaseGmv,ISNULL(p.InvoiceCount,0) PurchaseInvoiceCount,ISNULL(p.ItemCount,0) PurchaseItemCount,ISNULL(p.Quantity,0) PurchaseQuantity,ISNULL(s.Gmv,0) SaleGmv,ISNULL(s.InvoiceCount,0) SaleInvoiceCount,ISNULL(s.ItemCount,0) SaleItemCount,ISNULL(s.Quantity,0) SaleQuantity
FROM Tenants t CROSS JOIN MonthSeries m LEFT JOIN Purchases p ON p.TenantId=t.TenantId AND p.MonthStart=m.MonthStart LEFT JOIN Sales s ON s.TenantId=t.TenantId AND s.MonthStart=m.MonthStart
""";
}
[DisplayName("یکسان‌سازی داشبورد")]
[DbMap("vw_IntegrationDashboard")]
public sealed class SqlVwIntegrationdashboard : SqlServerEntity
{
    [DisplayName("مغازه شناسه")]
    [DbMap("ShopId")]
    public int Shopid { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TenantId")]
    public string Tenantid { get; set; } = null!;
    [DisplayName("پلتفرم")]
    [DbMap("Provider")]
    public byte Provider { get; set; }
    [DisplayName("نمایشی نام")]
    [DbMap("DisplayName")]
    public string Displayname { get; set; } = null!;
    [DisplayName("آیا فعال")]
    [DbMap("IsEnabled")]
    public bool Isenabled { get; set; }
    [DisplayName("آخرین یکسان‌سازی در زمان یوتی‌سی")]
    [DbMap("LastSyncAtUtc")]
    public DateTime? Lastsyncatutc { get; set; }
    [DisplayName("نگاشت تعداد")]
    [DbMap("MappingCount")]
    public int? Mappingcount { get; set; }
    [DisplayName("آخرین اجرا در زمان یوتی‌سی")]
    [DbMap("LastRunAtUtc")]
    public DateTime? Lastrunatutc { get; set; }
    [DisplayName("ناموفق اجراها")]
    [DbMap("FailedRuns")]
    public int? Failedruns { get; set; }
}
