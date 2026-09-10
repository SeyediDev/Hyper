-- =====================================================
-- Script برای افزودن داده‌های نمونه برای RewardAsset
-- این اسکریپت دارایی‌های پاداش را برای پاداش‌های موجود اضافه می‌کند
-- =====================================================

-- دریافت خودکار مقادیر از دیتابیس
DECLARE @TenantId INT = (SELECT TOP 1 Id FROM Tenants WHERE IsDeleted = 0 ORDER BY Id);
DECLARE @EventLogId BIGINT = (SELECT TOP 1 Id FROM EventLogs ORDER BY Id DESC);

-- اگر مقادیر پیدا نشد، از مقادیر پیش‌فرض استفاده می‌شود
IF @TenantId IS NULL SET @TenantId = 1;
IF @EventLogId IS NULL SET @EventLogId = 1;

PRINT N'Using TenantId: ' + CAST(@TenantId AS NVARCHAR(10));
PRINT N'Using EventLogId: ' + CAST(@EventLogId AS NVARCHAR(20));
PRINT N'';

-- =====================================================
-- 1. اضافه کردن RewardAsset برای پاداش‌های موجود
-- =====================================================

-- دریافت لیست پاداش‌های موجود
DECLARE @RewardCursor CURSOR;
DECLARE @RewardId INT;
DECLARE @RewardTitle NVARCHAR(100);
DECLARE @CustomerId INT;
DECLARE @AssetCounter INT = 1;

-- دریافت اولین مشتری موجود (برای تخصیص برخی دارایی‌ها)
SET @CustomerId = (SELECT TOP 1 Id FROM Customers WHERE IsDeleted = 0 ORDER BY Id);

-- ایجاد دارایی‌های عمومی (بدون تخصیص به مشتری) برای هر پاداش
SET @RewardCursor = CURSOR FOR
    SELECT Id, Title 
    FROM Rewards 
    WHERE TenantId = @TenantId AND IsDeleted = 0
    ORDER BY Id;

OPEN @RewardCursor;
FETCH NEXT FROM @RewardCursor INTO @RewardId, @RewardTitle;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- بررسی اینکه آیا برای این پاداش دارایی وجود دارد یا نه
    IF NOT EXISTS (SELECT 1 FROM RewardAssets WHERE RewardId = @RewardId AND CustomerId IS NULL)
    BEGIN
        -- ایجاد دارایی عمومی با موجودی مختلف
        DECLARE @Quantity INT;
        DECLARE @ConsumedQuantity INT;
        
        -- تعیین مقدار موجودی بر اساس ID پاداش (برای تنوع)
        SET @Quantity = CASE 
            WHEN @RewardId % 3 = 0 THEN 100  -- موجودی بالا
            WHEN @RewardId % 3 = 1 THEN 50   -- موجودی متوسط
            ELSE 25                           -- موجودی پایین
        END;
        
        -- تعیین مقدار مصرف شده (برخی دارایی‌ها مصرف شده‌اند)
        SET @ConsumedQuantity = CASE 
            WHEN @RewardId % 4 = 0 THEN @Quantity / 4  -- 25% مصرف شده
            WHEN @RewardId % 4 = 1 THEN @Quantity / 2  -- 50% مصرف شده
            ELSE 0                                      -- مصرف نشده
        END;
        
        INSERT INTO RewardAssets (
            TenantId,
            RewardId,
            CustomerId,
            Serial,
            Quantity,
            ConsumedQuantity,
            EventLogId,
            CreateDate,
            IsDeleted
        )
        VALUES (
            @TenantId,
            @RewardId,
            NULL,  -- دارایی عمومی (تخصیص نیافته)
            NULL,  -- سریال در صورت نیاز می‌تواند بعداً اضافه شود
            @Quantity,
            @ConsumedQuantity,
            @EventLogId,
            GETDATE(),
            0
        );
        
        PRINT N'Created general asset for Reward: ' + @RewardTitle + N' (ID: ' + CAST(@RewardId AS NVARCHAR(10)) + N', Quantity: ' + CAST(@Quantity AS NVARCHAR(10)) + N')';
    END
    
    FETCH NEXT FROM @RewardCursor INTO @RewardId, @RewardTitle;
END

CLOSE @RewardCursor;
DEALLOCATE @RewardCursor;

-- =====================================================
-- 2. اضافه کردن دارایی‌های تخصیص یافته به مشتریان
-- =====================================================

-- اگر مشتری وجود دارد، برای برخی پاداش‌ها دارایی تخصیص می‌دهیم
IF @CustomerId IS NOT NULL
BEGIN
    SET @RewardCursor = CURSOR FOR
        SELECT TOP 5 Id, Title 
        FROM Rewards 
        WHERE TenantId = @TenantId AND IsDeleted = 0
        ORDER BY Id;

    OPEN @RewardCursor;
    FETCH NEXT FROM @RewardCursor INTO @RewardId, @RewardTitle;
    SET @AssetCounter = 1;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- ایجاد دارایی تخصیص یافته به مشتری
        IF NOT EXISTS (SELECT 1 FROM RewardAssets WHERE RewardId = @RewardId AND CustomerId = @CustomerId)
        BEGIN
            DECLARE @CustomerQuantity INT = 10;  -- مقدار تخصیص یافته به مشتری
            DECLARE @CustomerConsumed INT = CASE 
                WHEN @AssetCounter % 2 = 0 THEN 3  -- برخی مصرف شده
                ELSE 0                              -- مصرف نشده
            END;
            
            DECLARE @SerialNumber NVARCHAR(61) = N'CUST-' + CAST(@CustomerId AS NVARCHAR(10)) + N'-' + CAST(@RewardId AS NVARCHAR(10)) + N'-' + CAST(@AssetCounter AS NVARCHAR(10));
            
            INSERT INTO RewardAssets (
                TenantId,
                RewardId,
                CustomerId,
                Serial,
                Quantity,
                ConsumedQuantity,
                EventLogId,
                CreateDate,
                IsDeleted
            )
            VALUES (
                @TenantId,
                @RewardId,
                @CustomerId,
                @SerialNumber,
                @CustomerQuantity,
                @CustomerConsumed,
                @EventLogId,
                DATEADD(DAY, -@AssetCounter, GETDATE()),  -- تاریخ‌های مختلف
                0
            );
            
            PRINT N'Created customer asset for Reward: ' + @RewardTitle + N' (Customer ID: ' + CAST(@CustomerId AS NVARCHAR(10)) + N', Quantity: ' + CAST(@CustomerQuantity AS NVARCHAR(10)) + N')';
        END
        
        SET @AssetCounter = @AssetCounter + 1;
        FETCH NEXT FROM @RewardCursor INTO @RewardId, @RewardTitle;
    END

    CLOSE @RewardCursor;
    DEALLOCATE @RewardCursor;
END
ELSE
BEGIN
    PRINT N'No customers found. Skipping customer-assigned assets.';
END

-- =====================================================
-- 3. نمایش خلاصه داده‌های اضافه شده
-- =====================================================

DECLARE @TotalAssets INT = (SELECT COUNT(*) FROM RewardAssets WHERE TenantId = @TenantId AND IsDeleted = 0);
DECLARE @GeneralAssets INT = (SELECT COUNT(*) FROM RewardAssets WHERE TenantId = @TenantId AND CustomerId IS NULL AND IsDeleted = 0);
DECLARE @CustomerAssets INT = (SELECT COUNT(*) FROM RewardAssets WHERE TenantId = @TenantId AND CustomerId IS NOT NULL AND IsDeleted = 0);
DECLARE @TotalQuantity INT = (SELECT SUM(Quantity) FROM RewardAssets WHERE TenantId = @TenantId AND IsDeleted = 0);
DECLARE @TotalConsumed INT = (SELECT SUM(ConsumedQuantity) FROM RewardAssets WHERE TenantId = @TenantId AND IsDeleted = 0);
DECLARE @TotalRemaining INT = @TotalQuantity - @TotalConsumed;

PRINT N'';
PRINT N'================================================';
PRINT N'Summary of RewardAsset Data:';
PRINT N'================================================';
PRINT N'Total Assets: ' + CAST(@TotalAssets AS NVARCHAR(10));
PRINT N'General Assets (unassigned): ' + CAST(@GeneralAssets AS NVARCHAR(10));
PRINT N'Customer Assets (assigned): ' + CAST(@CustomerAssets AS NVARCHAR(10));
PRINT N'Total Quantity: ' + CAST(@TotalQuantity AS NVARCHAR(10));
PRINT N'Total Consumed: ' + CAST(@TotalConsumed AS NVARCHAR(10));
PRINT N'Total Remaining: ' + CAST(@TotalRemaining AS NVARCHAR(10));
PRINT N'================================================';
PRINT N'';
PRINT N'داده‌های RewardAsset با موفقیت اضافه شدند!';


































