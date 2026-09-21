CREATE OR ALTER VIEW [dbo].[vw_SqlVwMarketinggmvmonthly]
AS
WITH MonthSeries AS (
    SELECT CAST(DATEADD(month, -11, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) AS date) MonthStart
    UNION ALL
    SELECT CAST(DATEADD(month, 1, MonthStart) AS date)
    FROM MonthSeries
    WHERE MonthStart < CAST(DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1) AS date)
), Tenants AS (
    SELECT DISTINCT ISNULL(TENANT_ID_, '') TenantId FROM TBL_SaleOrder WHERE SHOPID_ IS NOT NULL
    UNION
    SELECT DISTINCT ISNULL(TENANT_ID_, '') TenantId FROM TBL_PurchaseOrder WHERE SHOPID_ IS NOT NULL
), Purchases AS (
    SELECT ISNULL(o.TENANT_ID_, '') TenantId,
           CAST(DATEFROMPARTS(YEAR(o.ISSUEDATETIME_), MONTH(o.ISSUEDATETIME_), 1) AS date) MonthStart,
           SUM(o.TOTALINVOICEAMOUNT_) Gmv, COUNT_BIG(*) InvoiceCount,
           ISNULL(SUM(i.ItemCount), 0) ItemCount, ISNULL(SUM(i.Quantity), 0) Quantity
    FROM TBL_PurchaseOrder o
    LEFT JOIN (
        SELECT PURCHASEORDERID_, COUNT_BIG(*) ItemCount, SUM(ISNULL(QUANTITY_, 0)) Quantity
        FROM TBL_PurchaseOrderItem GROUP BY PURCHASEORDERID_
    ) i ON i.PURCHASEORDERID_ = o.PURCHASEORDERID_
    WHERE o.SHOPID_ IS NOT NULL
    GROUP BY ISNULL(o.TENANT_ID_, ''), CAST(DATEFROMPARTS(YEAR(o.ISSUEDATETIME_), MONTH(o.ISSUEDATETIME_), 1) AS date)
), Sales AS (
    SELECT ISNULL(o.TENANT_ID_, '') TenantId,
           CAST(DATEFROMPARTS(YEAR(o.ISSUEDATETIME_), MONTH(o.ISSUEDATETIME_), 1) AS date) MonthStart,
           SUM(o.TOTALINVOICEAMOUNT_) Gmv, COUNT_BIG(*) InvoiceCount,
           ISNULL(SUM(i.ItemCount), 0) ItemCount, ISNULL(SUM(i.Quantity), 0) Quantity
    FROM TBL_SaleOrder o
    LEFT JOIN (
        SELECT SALEORDERID_, COUNT_BIG(*) ItemCount, SUM(ISNULL(QUANTITY_, 0)) Quantity
        FROM TBL_SaleOrderItem GROUP BY SALEORDERID_
    ) i ON i.SALEORDERID_ = o.SALEORDERID_
    WHERE o.SHOPID_ IS NOT NULL
    GROUP BY ISNULL(o.TENANT_ID_, ''), CAST(DATEFROMPARTS(YEAR(o.ISSUEDATETIME_), MONTH(o.ISSUEDATETIME_), 1) AS date)
)
SELECT m.MonthStart, t.TenantId,
       ISNULL(p.Gmv, 0) PurchaseGmv, ISNULL(p.InvoiceCount, 0) PurchaseInvoiceCount,
       ISNULL(p.ItemCount, 0) PurchaseItemCount, ISNULL(p.Quantity, 0) PurchaseQuantity,
       ISNULL(s.Gmv, 0) SaleGmv, ISNULL(s.InvoiceCount, 0) SaleInvoiceCount,
       ISNULL(s.ItemCount, 0) SaleItemCount, ISNULL(s.Quantity, 0) SaleQuantity
FROM Tenants t CROSS JOIN MonthSeries m
LEFT JOIN Purchases p ON p.TenantId = t.TenantId AND p.MonthStart = m.MonthStart
LEFT JOIN Sales s ON s.TenantId = t.TenantId AND s.MonthStart = m.MonthStart;
