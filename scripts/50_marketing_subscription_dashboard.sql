/*
   DASH-217 / DASH-218
   Monthly marketing funnel for the main dashboard.

   A customer is a shop.  A shop becomes a customer on its first paid
   subscription request (Price > 0).  The view deliberately keeps the
   month series dense so a month with no activity is rendered as zero.
*/
CREATE OR ALTER VIEW dbo.vw_MarketingSubscriptionMonthly
AS
WITH MonthSeries AS
(
    SELECT CAST(DATEADD(month, -11, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) AS date) AS MonthStart
    UNION ALL
    SELECT CAST(DATEADD(month, 1, MonthStart) AS date)
    FROM MonthSeries
    WHERE MonthStart < CAST(DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1) AS date)
),
TenantSeries AS
(
    SELECT DISTINCT TENANT_ID_
    FROM dbo.TBL_ServiceRequest
    WHERE TENANT_ID_ IS NOT NULL
),
EligibleShopMonths AS
(
    SELECT DISTINCT TENANT_ID_, SHOPID_,
        CAST(DATEFROMPARTS(YEAR(REQUESTTIME_), MONTH(REQUESTTIME_), 1) AS date) AS MonthStart
    FROM dbo.TBL_ServiceRequest
    WHERE TENANT_ID_ IS NOT NULL AND SHOPID_ IS NOT NULL
),
PaidRequests AS
(
    SELECT request.TENANT_ID_, request.SHOPID_, request.REQUESTTIME_
    FROM dbo.TBL_ServiceRequest request
    LEFT JOIN dbo.TBL_ServicePayment payment
        ON payment.PAYMENTID_ = request.PAYMENTID_
    WHERE request.TENANT_ID_ IS NOT NULL
        AND request.SHOPID_ IS NOT NULL
        AND request.PRICE_ > 0
        AND (request.PAYMENTID_ IS NULL OR payment.ISSUCCESS_ = 1)
),
FirstPaid AS
(
    SELECT TENANT_ID_, SHOPID_, MIN(REQUESTTIME_) AS FirstPaidAt
    FROM PaidRequests
    GROUP BY TENANT_ID_, SHOPID_
),
PaidShopMonths AS
(
    SELECT DISTINCT TENANT_ID_, SHOPID_,
        CAST(DATEFROMPARTS(YEAR(REQUESTTIME_), MONTH(REQUESTTIME_), 1) AS date) AS MonthStart
    FROM PaidRequests
),
EligibleByMonth AS
(
    SELECT TENANT_ID_, MonthStart, COUNT_BIG(*) AS EligibleShopCount
    FROM EligibleShopMonths
    GROUP BY TENANT_ID_, MonthStart
),
NewPaidByMonth AS
(
    SELECT TENANT_ID_,
        CAST(DATEFROMPARTS(YEAR(FirstPaidAt), MONTH(FirstPaidAt), 1) AS date) AS MonthStart,
        COUNT_BIG(*) AS NewPaidShopCount
    FROM FirstPaid
    GROUP BY TENANT_ID_, CAST(DATEFROMPARTS(YEAR(FirstPaidAt), MONTH(FirstPaidAt), 1) AS date)
),
ActivePaidByMonth AS
(
    SELECT TENANT_ID_, MonthStart, COUNT_BIG(*) AS ActivePaidShopCount
    FROM PaidShopMonths
    GROUP BY TENANT_ID_, MonthStart
),
RetainedByMonth AS
(
    SELECT currentMonth.TENANT_ID_, currentMonth.MonthStart, COUNT_BIG(*) AS RetainedShopCount
    FROM PaidShopMonths currentMonth
    INNER JOIN PaidShopMonths previousMonth
        ON previousMonth.TENANT_ID_ = currentMonth.TENANT_ID_
        AND previousMonth.SHOPID_ = currentMonth.SHOPID_
        AND previousMonth.MonthStart = DATEADD(month, -1, currentMonth.MonthStart)
    GROUP BY currentMonth.TENANT_ID_, currentMonth.MonthStart
)
SELECT
    months.MonthStart,
    tenants.TENANT_ID_ AS TenantId,
    CAST(ISNULL(eligible.EligibleShopCount, 0) AS bigint) AS EligibleShopCount,
    CAST(ISNULL(newPaid.NewPaidShopCount, 0) AS bigint) AS NewPaidShopCount,
    CAST(ISNULL(activePaid.ActivePaidShopCount, 0) AS bigint) AS ActivePaidShopCount,
    CAST(ISNULL(retained.RetainedShopCount, 0) AS bigint) AS RetainedShopCount,
    CAST(CASE WHEN ISNULL(eligible.EligibleShopCount, 0) = 0 THEN 0
        ELSE 100.0 * ISNULL(newPaid.NewPaidShopCount, 0) / eligible.EligibleShopCount END AS decimal(9, 2)) AS ConversionRate,
    CAST(CASE WHEN ISNULL(previousActive.ActivePaidShopCount, 0) = 0 THEN 0
        ELSE 100.0 * ISNULL(retained.RetainedShopCount, 0) / previousActive.ActivePaidShopCount END AS decimal(9, 2)) AS RetentionRate
FROM TenantSeries tenants
CROSS JOIN MonthSeries months
LEFT JOIN EligibleByMonth eligible
    ON eligible.TENANT_ID_ = tenants.TENANT_ID_ AND eligible.MonthStart = months.MonthStart
LEFT JOIN NewPaidByMonth newPaid
    ON newPaid.TENANT_ID_ = tenants.TENANT_ID_ AND newPaid.MonthStart = months.MonthStart
LEFT JOIN ActivePaidByMonth activePaid
    ON activePaid.TENANT_ID_ = tenants.TENANT_ID_ AND activePaid.MonthStart = months.MonthStart
LEFT JOIN RetainedByMonth retained
    ON retained.TENANT_ID_ = tenants.TENANT_ID_ AND retained.MonthStart = months.MonthStart
LEFT JOIN ActivePaidByMonth previousActive
    ON previousActive.TENANT_ID_ = tenants.TENANT_ID_
    AND previousActive.MonthStart = DATEADD(month, -1, months.MonthStart);
GO
