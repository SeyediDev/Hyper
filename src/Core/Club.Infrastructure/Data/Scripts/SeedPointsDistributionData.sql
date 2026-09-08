-- =====================================================
-- Script برای افزودن داده‌های نمونه برای نمودار توزیع امتیازات بر اساس نوع
-- این اسکریپت انواع مختلف امتیاز و تراکنش‌های نمونه را اضافه می‌کند
-- =====================================================

-- ابتدا بررسی می‌کنیم که آیا Points مختلف وجود دارد یا نه
-- اگر وجود ندارد، Points جدید اضافه می‌کنیم

-- توجه: این اسکریپت به صورت خودکار مقادیر TenantId, CustomerTenantId, EventLogId را از دیتابیس می‌گیرد
-- اگر می‌خواهید مقادیر خاصی استفاده کنید، می‌توانید متغیرها را به صورت دستی تنظیم کنید

-- دریافت خودکار مقادیر از دیتابیس
DECLARE @TenantId INT = (SELECT TOP 1 Id FROM Tenants WHERE IsDeleted = 0 ORDER BY Id);
DECLARE @CustomerTenantId INT = (SELECT TOP 1 Id FROM CustomerTenants WHERE IsDeleted = 0 ORDER BY Id);
DECLARE @EventLogId BIGINT = (SELECT TOP 1 Id FROM EventLogs ORDER BY Id DESC);

-- اگر مقادیر پیدا نشد، از مقادیر پیش‌فرض استفاده می‌شود
IF @TenantId IS NULL SET @TenantId = 1;
IF @CustomerTenantId IS NULL SET @CustomerTenantId = 1;
IF @EventLogId IS NULL SET @EventLogId = 1;

PRINT N'Using TenantId: ' + CAST(@TenantId AS NVARCHAR(10));
PRINT N'Using CustomerTenantId: ' + CAST(@CustomerTenantId AS NVARCHAR(10));
PRINT N'Using EventLogId: ' + CAST(@EventLogId AS NVARCHAR(20));
PRINT N'';

-- =====================================================
-- 1. اضافه کردن Points مختلف (اگر وجود ندارند)
-- =====================================================

-- Point 1: امتیاز باشگاه (XP باشگاه)
IF NOT EXISTS (SELECT 1 FROM Points WHERE Title = N'XP باشگاه' AND TenantId = @TenantId)
BEGIN
    INSERT INTO Points (TenantId, PointType, Title, AutoVisit, Visible, ShowInLeaderboard, Transferable, CreateDate, IsDeleted)
    VALUES (@TenantId, 1, N'XP باشگاه', 0, 1, 1, 0, GETDATE(), 0);
END

-- Point 2: امتیاز خرید
IF NOT EXISTS (SELECT 1 FROM Points WHERE Title = N'امتیاز خرید' AND TenantId = @TenantId)
BEGIN
    INSERT INTO Points (TenantId, PointType, Title, AutoVisit, Visible, ShowInLeaderboard, Transferable, CreateDate, IsDeleted)
    VALUES (@TenantId, 0, N'امتیاز خرید', 0, 1, 1, 1, GETDATE(), 0);
END

-- Point 3: امتیاز معرفی دوست
IF NOT EXISTS (SELECT 1 FROM Points WHERE Title = N'امتیاز معرفی دوست' AND TenantId = @TenantId)
BEGIN
    INSERT INTO Points (TenantId, PointType, Title, AutoVisit, Visible, ShowInLeaderboard, Transferable, CreateDate, IsDeleted)
    VALUES (@TenantId, 0, N'امتیاز معرفی دوست', 0, 1, 1, 0, GETDATE(), 0);
END

-- Point 4: امتیاز نظرسنجی
IF NOT EXISTS (SELECT 1 FROM Points WHERE Title = N'امتیاز نظرسنجی' AND TenantId = @TenantId)
BEGIN
    INSERT INTO Points (TenantId, PointType, Title, AutoVisit, Visible, ShowInLeaderboard, Transferable, CreateDate, IsDeleted)
    VALUES (@TenantId, 0, N'امتیاز نظرسنجی', 0, 1, 0, 0, GETDATE(), 0);
END

-- Point 5: امتیاز ارزش طول‌عمر مشتری
IF NOT EXISTS (SELECT 1 FROM Points WHERE Title = N'امتیاز ارزش طول‌عمر' AND TenantId = @TenantId)
BEGIN
    INSERT INTO Points (TenantId, PointType, Title, AutoVisit, Visible, ShowInLeaderboard, Transferable, CreateDate, IsDeleted)
    VALUES (@TenantId, 2, N'امتیاز ارزش طول‌عمر', 0, 1, 1, 0, GETDATE(), 0);
END

-- =====================================================
-- 2. اضافه کردن تراکنش‌های نمونه برای هر نوع Point
-- =====================================================

DECLARE @PointId1 INT = (SELECT TOP 1 Id FROM Points WHERE Title = N'XP باشگاه' AND TenantId = @TenantId);
DECLARE @PointId2 INT = (SELECT TOP 1 Id FROM Points WHERE Title = N'امتیاز خرید' AND TenantId = @TenantId);
DECLARE @PointId3 INT = (SELECT TOP 1 Id FROM Points WHERE Title = N'امتیاز معرفی دوست' AND TenantId = @TenantId);
DECLARE @PointId4 INT = (SELECT TOP 1 Id FROM Points WHERE Title = N'امتیاز نظرسنجی' AND TenantId = @TenantId);
DECLARE @PointId5 INT = (SELECT TOP 1 Id FROM Points WHERE Title = N'امتیاز ارزش طول‌عمر' AND TenantId = @TenantId);

-- تراکنش‌های XP باشگاه (بزرگترین بخش)
IF @PointId1 IS NOT NULL
BEGIN
    INSERT INTO CustomerTransactions (TenantId, CustomerTenantId, PointId, Credit, Debit, Balance, TransactionType, EventLogId, CreateDate, IsDeleted)
    VALUES 
        (@TenantId, @CustomerTenantId, @PointId1, 5000000, NULL, 5000000, 0, @EventLogId, DATEADD(DAY, -30, GETDATE()), 0),
        (@TenantId, @CustomerTenantId, @PointId1, 3000000, NULL, 8000000, 0, @EventLogId, DATEADD(DAY, -20, GETDATE()), 0),
        (@TenantId, @CustomerTenantId, @PointId1, 2000000, NULL, 10000000, 0, @EventLogId, DATEADD(DAY, -10, GETDATE()), 0);
END

-- تراکنش‌های امتیاز خرید
IF @PointId2 IS NOT NULL
BEGIN
    INSERT INTO CustomerTransactions (TenantId, CustomerTenantId, PointId, Credit, Debit, Balance, TransactionType, EventLogId, CreateDate, IsDeleted)
    VALUES 
        (@TenantId, @CustomerTenantId, @PointId2, 1500000, NULL, 1500000, 0, @EventLogId, DATEADD(DAY, -25, GETDATE()), 0),
        (@TenantId, @CustomerTenantId, @PointId2, 800000, NULL, 2300000, 0, @EventLogId, DATEADD(DAY, -15, GETDATE()), 0),
        (@TenantId, @CustomerTenantId, @PointId2, NULL, 500000, 1800000, 1, @EventLogId, DATEADD(DAY, -5, GETDATE()), 0);
END

-- تراکنش‌های امتیاز معرفی دوست
IF @PointId3 IS NOT NULL
BEGIN
    INSERT INTO CustomerTransactions (TenantId, CustomerTenantId, PointId, Credit, Debit, Balance, TransactionType, EventLogId, CreateDate, IsDeleted)
    VALUES 
        (@TenantId, @CustomerTenantId, @PointId3, 500000, NULL, 500000, 0, @EventLogId, DATEADD(DAY, -28, GETDATE()), 0),
        (@TenantId, @CustomerTenantId, @PointId3, 300000, NULL, 800000, 0, @EventLogId, DATEADD(DAY, -18, GETDATE()), 0);
END

-- تراکنش‌های امتیاز نظرسنجی
IF @PointId4 IS NOT NULL
BEGIN
    INSERT INTO CustomerTransactions (TenantId, CustomerTenantId, PointId, Credit, Debit, Balance, TransactionType, EventLogId, CreateDate, IsDeleted)
    VALUES 
        (@TenantId, @CustomerTenantId, @PointId4, 200000, NULL, 200000, 0, @EventLogId, DATEADD(DAY, -22, GETDATE()), 0),
        (@TenantId, @CustomerTenantId, @PointId4, 150000, NULL, 350000, 0, @EventLogId, DATEADD(DAY, -12, GETDATE()), 0),
        (@TenantId, @CustomerTenantId, @PointId4, 100000, NULL, 450000, 0, @EventLogId, DATEADD(DAY, -2, GETDATE()), 0);
END

-- تراکنش‌های امتیاز ارزش طول‌عمر
IF @PointId5 IS NOT NULL
BEGIN
    INSERT INTO CustomerTransactions (TenantId, CustomerTenantId, PointId, Credit, Debit, Balance, TransactionType, EventLogId, CreateDate, IsDeleted)
    VALUES 
        (@TenantId, @CustomerTenantId, @PointId5, 800000, NULL, 800000, 0, @EventLogId, DATEADD(DAY, -35, GETDATE()), 0),
        (@TenantId, @CustomerTenantId, @PointId5, 400000, NULL, 1200000, 0, @EventLogId, DATEADD(DAY, -20, GETDATE()), 0);
END

-- =====================================================
-- نتیجه: حالا نمودار Pie باید 5 بخش مختلف داشته باشد:
-- 1. XP باشگاه: ~10,000,000 (بزرگترین)
-- 2. امتیاز خرید: ~1,800,000
-- 3. امتیاز معرفی دوست: ~800,000
-- 4. امتیاز نظرسنجی: ~450,000
-- 5. امتیاز ارزش طول‌عمر: ~1,200,000
-- =====================================================

PRINT N'داده‌های نمونه با موفقیت اضافه شدند!';
PRINT N'نمودار Pie حالا باید 5 بخش مختلف داشته باشد.';

