-- =====================================================
-- Script برای افزودن داده‌های اولیه درخت محصول و ویژگی‌ها
-- این اسکریپت دسته‌بندی‌ها، محصولات و ویژگی‌ها را ایجاد می‌کند
-- =====================================================

-- دریافت خودکار TenantId از دیتابیس
DECLARE @TenantId INT = (SELECT TOP 1 Id FROM CoreConfig.Tenants WHERE IsDeleted = 0 ORDER BY Id);

-- اگر TenantId پیدا نشد، از مقدار پیش‌فرض استفاده می‌شود
IF @TenantId IS NULL SET @TenantId = 1;

PRINT N'Using TenantId: ' + CAST(@TenantId AS NVARCHAR(10));
PRINT N'';
PRINT N'شروع ایجاد درخت محصول و ویژگی‌ها...';
PRINT N'';

-- =====================================================
-- 1. ایجاد دسته‌بندی‌های محصول
-- =====================================================

DECLARE @AccountServicesCategoryId INT;
DECLARE @CardServicesCategoryId INT;
DECLARE @MoneyTransferCategoryId INT;
DECLARE @FacilitiesCategoryId INT;
DECLARE @FacilityGrantCategoryId INT;
DECLARE @InstallmentRepaymentCategoryId INT;
DECLARE @ValueAddedCategoryId INT;

-- خدمات حساب
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategories WHERE TenantId = @TenantId AND [Key] = N'AccountServices' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategories (TenantId, Title, [Key], DisplayOrder, IsActive, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'خدمات حساب', N'AccountServices', 1, 1, GETDATE(), GETDATE(), 0);
    SET @AccountServicesCategoryId = SCOPE_IDENTITY();
    PRINT N'دسته‌بندی "خدمات حساب" ایجاد شد (ID: ' + CAST(@AccountServicesCategoryId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @AccountServicesCategoryId = Id FROM CoreConfig.ProductCategories WHERE TenantId = @TenantId AND [Key] = N'AccountServices' AND IsDeleted = 0;
END

-- خدمات کارت
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategories WHERE TenantId = @TenantId AND [Key] = N'CardServices' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategories (TenantId, Title, [Key], DisplayOrder, IsActive, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'خدمات کارت', N'CardServices', 2, 1, GETDATE(), GETDATE(), 0);
    SET @CardServicesCategoryId = SCOPE_IDENTITY();
    PRINT N'دسته‌بندی "خدمات کارت" ایجاد شد (ID: ' + CAST(@CardServicesCategoryId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @CardServicesCategoryId = Id FROM CoreConfig.ProductCategories WHERE TenantId = @TenantId AND [Key] = N'CardServices' AND IsDeleted = 0;
END

-- انتقال وجه
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategories WHERE TenantId = @TenantId AND [Key] = N'MoneyTransfer' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategories (TenantId, Title, [Key], DisplayOrder, IsActive, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'انتقال وجه', N'MoneyTransfer', 3, 1, GETDATE(), GETDATE(), 0);
    SET @MoneyTransferCategoryId = SCOPE_IDENTITY();
    PRINT N'دسته‌بندی "انتقال وجه" ایجاد شد (ID: ' + CAST(@MoneyTransferCategoryId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @MoneyTransferCategoryId = Id FROM CoreConfig.ProductCategories WHERE TenantId = @TenantId AND [Key] = N'MoneyTransfer' AND IsDeleted = 0;
END

-- تسهیلات (دسته‌بندی والد)
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategories WHERE TenantId = @TenantId AND [Key] = N'Facilities' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategories (TenantId, Title, [Key], DisplayOrder, IsActive, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'تسهیلات', N'Facilities', 4, 1, GETDATE(), GETDATE(), 0);
    SET @FacilitiesCategoryId = SCOPE_IDENTITY();
    PRINT N'دسته‌بندی "تسهیلات" ایجاد شد (ID: ' + CAST(@FacilitiesCategoryId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @FacilitiesCategoryId = Id FROM CoreConfig.ProductCategories WHERE TenantId = @TenantId AND [Key] = N'Facilities' AND IsDeleted = 0;
END

-- اعطای تسهیلات (زیر دسته‌بندی تسهیلات)
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategories WHERE TenantId = @TenantId AND [Key] = N'FacilityGrant' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategories (TenantId, Title, [Key], ParentCategoryId, DisplayOrder, IsActive, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'اعطای تسهیلات', N'FacilityGrant', @FacilitiesCategoryId, 1, 1, GETDATE(), GETDATE(), 0);
    SET @FacilityGrantCategoryId = SCOPE_IDENTITY();
    PRINT N'دسته‌بندی "اعطای تسهیلات" ایجاد شد (ID: ' + CAST(@FacilityGrantCategoryId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @FacilityGrantCategoryId = Id FROM CoreConfig.ProductCategories WHERE TenantId = @TenantId AND [Key] = N'FacilityGrant' AND IsDeleted = 0;
END

-- بازپرداخت اقساط (زیر دسته‌بندی تسهیلات)
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategories WHERE TenantId = @TenantId AND [Key] = N'InstallmentRepayment' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategories (TenantId, Title, [Key], ParentCategoryId, DisplayOrder, IsActive, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'بازپرداخت اقساط', N'InstallmentRepayment', @FacilitiesCategoryId, 2, 1, GETDATE(), GETDATE(), 0);
    SET @InstallmentRepaymentCategoryId = SCOPE_IDENTITY();
    PRINT N'دسته‌بندی "بازپرداخت اقساط" ایجاد شد (ID: ' + CAST(@InstallmentRepaymentCategoryId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @InstallmentRepaymentCategoryId = Id FROM CoreConfig.ProductCategories WHERE TenantId = @TenantId AND [Key] = N'InstallmentRepayment' AND IsDeleted = 0;
END

-- ارزش افزوده
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategories WHERE TenantId = @TenantId AND [Key] = N'ValueAdded' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategories (TenantId, Title, [Key], DisplayOrder, IsActive, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'ارزش افزوده', N'ValueAdded', 5, 1, GETDATE(), GETDATE(), 0);
    SET @ValueAddedCategoryId = SCOPE_IDENTITY();
    PRINT N'دسته‌بندی "ارزش افزوده" ایجاد شد (ID: ' + CAST(@ValueAddedCategoryId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @ValueAddedCategoryId = Id FROM CoreConfig.ProductCategories WHERE TenantId = @TenantId AND [Key] = N'ValueAdded' AND IsDeleted = 0;
END

PRINT N'';
PRINT N'تمام دسته‌بندی‌ها ایجاد شدند.';
PRINT N'';

-- =====================================================
-- 2. ایجاد ویژگی‌ها سطح اکوسیستم (Tenant)
-- =====================================================

DECLARE @TenantAttributeAmountId INT;
DECLARE @TenantAttributeCurrencyId INT;
DECLARE @TenantAttributeProfitId INT;
DECLARE @TenantAttributeQuantityId INT;

-- Amount
IF NOT EXISTS (SELECT 1 FROM CoreConfig.TenantProductAttributes WHERE TenantId = @TenantId AND [Key] = N'Amount' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.TenantProductAttributes (TenantId, [Key], Title, ParameterType, IsOptional, ValidateByList, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'Amount', N'مبلغ', 2, 0, 0, GETDATE(), GETDATE(), 0);  -- ParameterType.Float = 2
    SET @TenantAttributeAmountId = SCOPE_IDENTITY();
    PRINT N'صفت اکوسیستم "Amount" ایجاد شد (ID: ' + CAST(@TenantAttributeAmountId AS NVARCHAR(10)) + N')';
END
ELSE 
BEGIN
    SELECT @TenantAttributeAmountId = Id FROM CoreConfig.TenantProductAttributes WHERE TenantId = @TenantId AND [Key] = N'Amount' AND IsDeleted = 0;
END

-- Currency
IF NOT EXISTS (SELECT 1 FROM CoreConfig.TenantProductAttributes WHERE TenantId = @TenantId AND [Key] = N'Currency' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.TenantProductAttributes (TenantId, [Key], Title, ParameterType, IsOptional, DefaultValue, ValidateByList, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'Currency', N'ارز', 0, 0, N'IRR', 0, GETDATE(), GETDATE(), 0);  -- ParameterType.String = 0
    SET @TenantAttributeCurrencyId = SCOPE_IDENTITY();
    PRINT N'صفت اکوسیستم "Currency" ایجاد شد (ID: ' + CAST(@TenantAttributeCurrencyId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @TenantAttributeCurrencyId = Id FROM CoreConfig.TenantProductAttributes WHERE TenantId = @TenantId AND [Key] = N'Currency' AND IsDeleted = 0;
END

-- Profit
IF NOT EXISTS (SELECT 1 FROM CoreConfig.TenantProductAttributes WHERE TenantId = @TenantId AND [Key] = N'Profit' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.TenantProductAttributes (TenantId, [Key], Title, ParameterType, IsOptional, ValidateByList, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'Profit', N'سود', 2, 0, 0, GETDATE(), GETDATE(), 0);  -- ParameterType.Float = 2
    SET @TenantAttributeProfitId = SCOPE_IDENTITY();
    PRINT N'صفت اکوسیستم "Profit" ایجاد شد (ID: ' + CAST(@TenantAttributeProfitId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @TenantAttributeProfitId = Id FROM CoreConfig.TenantProductAttributes WHERE TenantId = @TenantId AND [Key] = N'Profit' AND IsDeleted = 0;
END

-- Quantity
IF NOT EXISTS (SELECT 1 FROM CoreConfig.TenantProductAttributes WHERE TenantId = @TenantId AND [Key] = N'Quantity' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.TenantProductAttributes (TenantId, [Key], Title, ParameterType, IsOptional, ValidateByList, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'Quantity', N'تعداد', 1, 0, 0, GETDATE(), GETDATE(), 0);  -- ParameterType.Long = 1
    SET @TenantAttributeQuantityId = SCOPE_IDENTITY();
    PRINT N'صفت اکوسیستم "Quantity" ایجاد شد (ID: ' + CAST(@TenantAttributeQuantityId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @TenantAttributeQuantityId = Id FROM CoreConfig.TenantProductAttributes WHERE TenantId = @TenantId AND [Key] = N'Quantity' AND IsDeleted = 0;
END

PRINT N'';
PRINT N'تمام ویژگی‌ها سطح اکوسیستم ایجاد شدند.';
PRINT N'';

-- =====================================================
-- 3. ایجاد ویژگی‌ها سطح دسته‌بندی
-- =====================================================

DECLARE @CategoryAttributeAccountTypeId INT;
DECLARE @CategoryAttributeMinimumBalanceId INT;
DECLARE @CategoryAttributeCardTypeId INT;
DECLARE @CategoryAttributeCardNumberId INT;
DECLARE @CategoryAttributeTransferTypeId INT;
DECLARE @CategoryAttributeDestinationAccountId INT;
DECLARE @CategoryAttributeDestinationShabaId INT;
DECLARE @CategoryAttributeInstallmentProviderId INT;
DECLARE @CategoryAttributeInstallmentAmountId INT;
DECLARE @CategoryAttributeInstallmentCountId INT;
DECLARE @CategoryAttributeInstallmentPeriodId INT;
DECLARE @CategoryAttributeThirdPartyCompanyId INT;
DECLARE @CategoryAttributeThirdPartyProductCodeId INT;
DECLARE @CategoryAttributeDueDateId INT;
DECLARE @CategoryAttributeDaysToDueDateId INT;
DECLARE @CategoryAttributeRepaymentStatusId INT;
DECLARE @CategoryAttributeInstallmentNumberId INT;
DECLARE @CategoryAttributeProviderId INT;
DECLARE @CategoryAttributeServiceTypeId INT;
DECLARE @CategoryAttributeMobileNumberId INT;

-- خدمات حساب: AccountType
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @AccountServicesCategoryId AND [Key] = N'AccountType' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributes (CategoryId, [Key], Title, ParameterType, IsOptional, ValidateByList, CreateDate, LastModified, IsDeleted)
    VALUES (@AccountServicesCategoryId, N'AccountType', N'نوع حساب', 0, 0, 1, GETDATE(), GETDATE(), 0);
    SET @CategoryAttributeAccountTypeId = SCOPE_IDENTITY();
    PRINT N'صفت دسته‌بندی "AccountType" ایجاد شد (ID: ' + CAST(@CategoryAttributeAccountTypeId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @CategoryAttributeAccountTypeId = Id FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @AccountServicesCategoryId AND [Key] = N'AccountType' AND IsDeleted = 0;
END

-- خدمات حساب: MinimumBalance
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @AccountServicesCategoryId AND [Key] = N'MinimumBalance' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributes (CategoryId, [Key], Title, ParameterType, IsOptional, ValidateByList, CreateDate, LastModified, IsDeleted)
    VALUES (@AccountServicesCategoryId, N'MinimumBalance', N'حداقل مانده', 2, 1, 0, GETDATE(), GETDATE(), 0);
    SET @CategoryAttributeMinimumBalanceId = SCOPE_IDENTITY();
    PRINT N'صفت دسته‌بندی "MinimumBalance" ایجاد شد (ID: ' + CAST(@CategoryAttributeMinimumBalanceId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @CategoryAttributeMinimumBalanceId = Id FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @AccountServicesCategoryId AND [Key] = N'MinimumBalance' AND IsDeleted = 0;
END

-- خدمات کارت: CardType
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @CardServicesCategoryId AND [Key] = N'CardType' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributes (CategoryId, [Key], Title, ParameterType, IsOptional, ValidateByList, CreateDate, LastModified, IsDeleted)
    VALUES (@CardServicesCategoryId, N'CardType', N'نوع کارت', 0, 0, 1, GETDATE(), GETDATE(), 0);
    SET @CategoryAttributeCardTypeId = SCOPE_IDENTITY();
    PRINT N'صفت دسته‌بندی "CardType" ایجاد شد (ID: ' + CAST(@CategoryAttributeCardTypeId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @CategoryAttributeCardTypeId = Id FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @CardServicesCategoryId AND [Key] = N'CardType' AND IsDeleted = 0;
END

-- خدمات کارت: CardNumber
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @CardServicesCategoryId AND [Key] = N'CardNumber' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributes (CategoryId, [Key], Title, ParameterType, IsOptional, ValidateByList, CreateDate, LastModified, IsDeleted)
    VALUES (@CardServicesCategoryId, N'CardNumber', N'شماره کارت', 0, 1, 0, GETDATE(), GETDATE(), 0);
    SET @CategoryAttributeCardNumberId = SCOPE_IDENTITY();
    PRINT N'صفت دسته‌بندی "CardNumber" ایجاد شد (ID: ' + CAST(@CategoryAttributeCardNumberId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @CategoryAttributeCardNumberId = Id FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @CardServicesCategoryId AND [Key] = N'CardNumber' AND IsDeleted = 0;
END

-- انتقال وجه: TransferType
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @MoneyTransferCategoryId AND [Key] = N'TransferType' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributes (CategoryId, [Key], Title, ParameterType, IsOptional, ValidateByList, CreateDate, LastModified, IsDeleted)
    VALUES (@MoneyTransferCategoryId, N'TransferType', N'نوع انتقال', 0, 0, 1, GETDATE(), GETDATE(), 0);
    SET @CategoryAttributeTransferTypeId = SCOPE_IDENTITY();
    PRINT N'صفت دسته‌بندی "TransferType" ایجاد شد (ID: ' + CAST(@CategoryAttributeTransferTypeId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @CategoryAttributeTransferTypeId = Id FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @MoneyTransferCategoryId AND [Key] = N'TransferType' AND IsDeleted = 0;
END

-- انتقال وجه: DestinationAccount
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @MoneyTransferCategoryId AND [Key] = N'DestinationAccount' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributes (CategoryId, [Key], Title, ParameterType, IsOptional, ValidateByList, CreateDate, LastModified, IsDeleted)
    VALUES (@MoneyTransferCategoryId, N'DestinationAccount', N'شماره حساب/کارت مقصد', 0, 1, 0, GETDATE(), GETDATE(), 0);
    SET @CategoryAttributeDestinationAccountId = SCOPE_IDENTITY();
    PRINT N'صفت دسته‌بندی "DestinationAccount" ایجاد شد (ID: ' + CAST(@CategoryAttributeDestinationAccountId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @CategoryAttributeDestinationAccountId = Id FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @MoneyTransferCategoryId AND [Key] = N'DestinationAccount' AND IsDeleted = 0;
END

-- انتقال وجه: DestinationShaba
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @MoneyTransferCategoryId AND [Key] = N'DestinationShaba' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributes (CategoryId, [Key], Title, ParameterType, IsOptional, ValidateByList, CreateDate, LastModified, IsDeleted)
    VALUES (@MoneyTransferCategoryId, N'DestinationShaba', N'شماره شبا مقصد', 0, 1, 0, GETDATE(), GETDATE(), 0);
    SET @CategoryAttributeDestinationShabaId = SCOPE_IDENTITY();
    PRINT N'صفت دسته‌بندی "DestinationShaba" ایجاد شد (ID: ' + CAST(@CategoryAttributeDestinationShabaId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @CategoryAttributeDestinationShabaId = Id FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @MoneyTransferCategoryId AND [Key] = N'DestinationShaba' AND IsDeleted = 0;
END

-- اعطای تسهیلات: InstallmentProvider
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @FacilityGrantCategoryId AND [Key] = N'InstallmentProvider' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributes (CategoryId, [Key], Title, ParameterType, IsOptional, ValidateByList, CreateDate, LastModified, IsDeleted)
    VALUES (@FacilityGrantCategoryId, N'InstallmentProvider', N'ارائه‌دهنده تسهیلات', 0, 0, 1, GETDATE(), GETDATE(), 0);
    SET @CategoryAttributeInstallmentProviderId = SCOPE_IDENTITY();
    PRINT N'صفت دسته‌بندی "InstallmentProvider" ایجاد شد (ID: ' + CAST(@CategoryAttributeInstallmentProviderId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @CategoryAttributeInstallmentProviderId = Id FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @FacilityGrantCategoryId AND [Key] = N'InstallmentProvider' AND IsDeleted = 0;
END

-- اعطای تسهیلات: InstallmentAmount
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @FacilityGrantCategoryId AND [Key] = N'InstallmentAmount' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributes (CategoryId, [Key], Title, ParameterType, IsOptional, ValidateByList, CreateDate, LastModified, IsDeleted)
    VALUES (@FacilityGrantCategoryId, N'InstallmentAmount', N'مبلغ تسهیلات', 2, 0, 0, GETDATE(), GETDATE(), 0);
    SET @CategoryAttributeInstallmentAmountId = SCOPE_IDENTITY();
    PRINT N'صفت دسته‌بندی "InstallmentAmount" ایجاد شد (ID: ' + CAST(@CategoryAttributeInstallmentAmountId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @CategoryAttributeInstallmentAmountId = Id FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @FacilityGrantCategoryId AND [Key] = N'InstallmentAmount' AND IsDeleted = 0;
END

-- اعطای تسهیلات: InstallmentCount
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @FacilityGrantCategoryId AND [Key] = N'InstallmentCount' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributes (CategoryId, [Key], Title, ParameterType, IsOptional, ValidateByList, CreateDate, LastModified, IsDeleted)
    VALUES (@FacilityGrantCategoryId, N'InstallmentCount', N'تعداد اقساط', 1, 0, 0, GETDATE(), GETDATE(), 0);
    SET @CategoryAttributeInstallmentCountId = SCOPE_IDENTITY();
    PRINT N'صفت دسته‌بندی "InstallmentCount" ایجاد شد (ID: ' + CAST(@CategoryAttributeInstallmentCountId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @CategoryAttributeInstallmentCountId = Id FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @FacilityGrantCategoryId AND [Key] = N'InstallmentCount' AND IsDeleted = 0;
END

-- اعطای تسهیلات: InstallmentPeriod
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @FacilityGrantCategoryId AND [Key] = N'InstallmentPeriod' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributes (CategoryId, [Key], Title, ParameterType, IsOptional, ValidateByList, CreateDate, LastModified, IsDeleted)
    VALUES (@FacilityGrantCategoryId, N'InstallmentPeriod', N'دوره اقساط (روز)', 1, 0, 0, GETDATE(), GETDATE(), 0);
    SET @CategoryAttributeInstallmentPeriodId = SCOPE_IDENTITY();
    PRINT N'صفت دسته‌بندی "InstallmentPeriod" ایجاد شد (ID: ' + CAST(@CategoryAttributeInstallmentPeriodId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @CategoryAttributeInstallmentPeriodId = Id FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @FacilityGrantCategoryId AND [Key] = N'InstallmentPeriod' AND IsDeleted = 0;
END

-- اعطای تسهیلات: ThirdPartyCompany (برای BNPL و کالانو)
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @FacilityGrantCategoryId AND [Key] = N'ThirdPartyCompany' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributes (CategoryId, [Key], Title, ParameterType, IsOptional, ValidateByList, CreateDate, LastModified, IsDeleted)
    VALUES (@FacilityGrantCategoryId, N'ThirdPartyCompany', N'شرکت سفارش‌دهنده تسهیلات', 0, 1, 0, GETDATE(), GETDATE(), 0);
    SET @CategoryAttributeThirdPartyCompanyId = SCOPE_IDENTITY();
    PRINT N'صفت دسته‌بندی "ThirdPartyCompany" ایجاد شد (ID: ' + CAST(@CategoryAttributeThirdPartyCompanyId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @CategoryAttributeThirdPartyCompanyId = Id FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @FacilityGrantCategoryId AND [Key] = N'ThirdPartyCompany' AND IsDeleted = 0;
END

-- اعطای تسهیلات: ThirdPartyProductCode (برای BNPL و کالانو)
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @FacilityGrantCategoryId AND [Key] = N'ThirdPartyProductCode' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributes (CategoryId, [Key], Title, ParameterType, IsOptional, ValidateByList, CreateDate, LastModified, IsDeleted)
    VALUES (@FacilityGrantCategoryId, N'ThirdPartyProductCode', N'کد محصول شرکت', 0, 1, 0, GETDATE(), GETDATE(), 0);
    SET @CategoryAttributeThirdPartyProductCodeId = SCOPE_IDENTITY();
    PRINT N'صفت دسته‌بندی "ThirdPartyProductCode" ایجاد شد (ID: ' + CAST(@CategoryAttributeThirdPartyProductCodeId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @CategoryAttributeThirdPartyProductCodeId = Id FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @FacilityGrantCategoryId AND [Key] = N'ThirdPartyProductCode' AND IsDeleted = 0;
END

-- بازپرداخت اقساط: InstallmentProvider
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @InstallmentRepaymentCategoryId AND [Key] = N'InstallmentProvider' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributes (CategoryId, [Key], Title, ParameterType, IsOptional, ValidateByList, CreateDate, LastModified, IsDeleted)
    VALUES (@InstallmentRepaymentCategoryId, N'InstallmentProvider', N'ارائه‌دهنده تسهیلات', 0, 0, 1, GETDATE(), GETDATE(), 0);
    PRINT N'صفت دسته‌بندی "InstallmentProvider" (بازپرداخت) ایجاد شد';
END

-- بازپرداخت اقساط: DueDate
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @InstallmentRepaymentCategoryId AND [Key] = N'DueDate' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributes (CategoryId, [Key], Title, ParameterType, IsOptional, ValidateByList, CreateDate, LastModified, IsDeleted)
    VALUES (@InstallmentRepaymentCategoryId, N'DueDate', N'تاریخ سررسید قسط', 4, 0, 0, GETDATE(), GETDATE(), 0);  -- ParameterType.DateTime = 4
    SET @CategoryAttributeDueDateId = SCOPE_IDENTITY();
    PRINT N'صفت دسته‌بندی "DueDate" ایجاد شد (ID: ' + CAST(@CategoryAttributeDueDateId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @CategoryAttributeDueDateId = Id FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @InstallmentRepaymentCategoryId AND [Key] = N'DueDate' AND IsDeleted = 0;
END

-- بازپرداخت اقساط: DaysToDueDate
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @InstallmentRepaymentCategoryId AND [Key] = N'DaysToDueDate' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributes (CategoryId, [Key], Title, ParameterType, IsOptional, ValidateByList, CreateDate, LastModified, IsDeleted)
    VALUES (@InstallmentRepaymentCategoryId, N'DaysToDueDate', N'تعداد روز تا سررسید', 1, 0, 0, GETDATE(), GETDATE(), 0);
    SET @CategoryAttributeDaysToDueDateId = SCOPE_IDENTITY();
    PRINT N'صفت دسته‌بندی "DaysToDueDate" ایجاد شد (ID: ' + CAST(@CategoryAttributeDaysToDueDateId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @CategoryAttributeDaysToDueDateId = Id FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @InstallmentRepaymentCategoryId AND [Key] = N'DaysToDueDate' AND IsDeleted = 0;
END

-- بازپرداخت اقساط: RepaymentStatus
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @InstallmentRepaymentCategoryId AND [Key] = N'RepaymentStatus' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributes (CategoryId, [Key], Title, ParameterType, IsOptional, ValidateByList, CreateDate, LastModified, IsDeleted)
    VALUES (@InstallmentRepaymentCategoryId, N'RepaymentStatus', N'وضعیت بازپرداخت', 0, 0, 1, GETDATE(), GETDATE(), 0);
    SET @CategoryAttributeRepaymentStatusId = SCOPE_IDENTITY();
    PRINT N'صفت دسته‌بندی "RepaymentStatus" ایجاد شد (ID: ' + CAST(@CategoryAttributeRepaymentStatusId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @CategoryAttributeRepaymentStatusId = Id FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @InstallmentRepaymentCategoryId AND [Key] = N'RepaymentStatus' AND IsDeleted = 0;
END

-- بازپرداخت اقساط: InstallmentAmount
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @InstallmentRepaymentCategoryId AND [Key] = N'InstallmentAmount' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributes (CategoryId, [Key], Title, ParameterType, IsOptional, ValidateByList, CreateDate, LastModified, IsDeleted)
    VALUES (@InstallmentRepaymentCategoryId, N'InstallmentAmount', N'مبلغ قسط', 2, 0, 0, GETDATE(), GETDATE(), 0);
    PRINT N'صفت دسته‌بندی "InstallmentAmount" (بازپرداخت) ایجاد شد';
END

-- بازپرداخت اقساط: InstallmentNumber
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @InstallmentRepaymentCategoryId AND [Key] = N'InstallmentNumber' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributes (CategoryId, [Key], Title, ParameterType, IsOptional, ValidateByList, CreateDate, LastModified, IsDeleted)
    VALUES (@InstallmentRepaymentCategoryId, N'InstallmentNumber', N'شماره قسط', 1, 0, 0, GETDATE(), GETDATE(), 0);
    SET @CategoryAttributeInstallmentNumberId = SCOPE_IDENTITY();
    PRINT N'صفت دسته‌بندی "InstallmentNumber" ایجاد شد (ID: ' + CAST(@CategoryAttributeInstallmentNumberId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @CategoryAttributeInstallmentNumberId = Id FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @InstallmentRepaymentCategoryId AND [Key] = N'InstallmentNumber' AND IsDeleted = 0;
END

-- ارزش افزوده: Provider
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @ValueAddedCategoryId AND [Key] = N'Provider' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributes (CategoryId, [Key], Title, ParameterType, IsOptional, ValidateByList, CreateDate, LastModified, IsDeleted)
    VALUES (@ValueAddedCategoryId, N'Provider', N'اپراتور', 0, 0, 1, GETDATE(), GETDATE(), 0);
    SET @CategoryAttributeProviderId = SCOPE_IDENTITY();
    PRINT N'صفت دسته‌بندی "Provider" ایجاد شد (ID: ' + CAST(@CategoryAttributeProviderId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @CategoryAttributeProviderId = Id FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @ValueAddedCategoryId AND [Key] = N'Provider' AND IsDeleted = 0;
END

-- ارزش افزوده: ServiceType
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @ValueAddedCategoryId AND [Key] = N'ServiceType' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributes (CategoryId, [Key], Title, ParameterType, IsOptional, ValidateByList, CreateDate, LastModified, IsDeleted)
    VALUES (@ValueAddedCategoryId, N'ServiceType', N'نوع سرویس', 0, 0, 1, GETDATE(), GETDATE(), 0);
    SET @CategoryAttributeServiceTypeId = SCOPE_IDENTITY();
    PRINT N'صفت دسته‌بندی "ServiceType" ایجاد شد (ID: ' + CAST(@CategoryAttributeServiceTypeId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @CategoryAttributeServiceTypeId = Id FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @ValueAddedCategoryId AND [Key] = N'ServiceType' AND IsDeleted = 0;
END

-- ارزش افزوده: MobileNumber
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @ValueAddedCategoryId AND [Key] = N'MobileNumber' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributes (CategoryId, [Key], Title, ParameterType, IsOptional, ValidateByList, CreateDate, LastModified, IsDeleted)
    VALUES (@ValueAddedCategoryId, N'MobileNumber', N'شماره موبایل', 0, 0, 0, GETDATE(), GETDATE(), 0);
    SET @CategoryAttributeMobileNumberId = SCOPE_IDENTITY();
    PRINT N'صفت دسته‌بندی "MobileNumber" ایجاد شد (ID: ' + CAST(@CategoryAttributeMobileNumberId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @CategoryAttributeMobileNumberId = Id FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @ValueAddedCategoryId AND [Key] = N'MobileNumber' AND IsDeleted = 0;
END

PRINT N'';
PRINT N'تمام ویژگی‌ها سطح دسته‌بندی ایجاد شدند.';
PRINT N'';

-- =====================================================
-- 4. ایجاد مقادیر مجاز برای ویژگی‌ها با ValidateByList = true
-- =====================================================

-- AccountType مقادیر مجاز
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributeAllowedValues WHERE AttributeId = @CategoryAttributeAccountTypeId AND [Value] = N'Digital' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributeAllowedValues (AttributeId, [Value], DisplayTitle, DisplayOrder, IsActive, CreateDate, LastModified, IsDeleted)
    VALUES (@CategoryAttributeAccountTypeId, N'Digital', N'دیجیتال', 1, 1, GETDATE(), GETDATE(), 0);
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributeAllowedValues WHERE AttributeId = @CategoryAttributeAccountTypeId AND [Value] = N'Premium' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributeAllowedValues (AttributeId, [Value], DisplayTitle, DisplayOrder, IsActive, CreateDate, LastModified, IsDeleted)
    VALUES (@CategoryAttributeAccountTypeId, N'Premium', N'ممتاز', 2, 1, GETDATE(), GETDATE(), 0);
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributeAllowedValues WHERE AttributeId = @CategoryAttributeAccountTypeId AND [Value] = N'Normal' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributeAllowedValues (AttributeId, [Value], DisplayTitle, DisplayOrder, IsActive, CreateDate, LastModified, IsDeleted)
    VALUES (@CategoryAttributeAccountTypeId, N'Normal', N'عادی', 3, 1, GETDATE(), GETDATE(), 0);
END

-- CardType مقادیر مجاز
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributeAllowedValues WHERE AttributeId = @CategoryAttributeCardTypeId AND [Value] = N'Normal' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributeAllowedValues (AttributeId, [Value], DisplayTitle, DisplayOrder, IsActive, CreateDate, LastModified, IsDeleted)
    VALUES (@CategoryAttributeCardTypeId, N'Normal', N'عادی', 1, 1, GETDATE(), GETDATE(), 0);
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributeAllowedValues WHERE AttributeId = @CategoryAttributeCardTypeId AND [Value] = N'Gold' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributeAllowedValues (AttributeId, [Value], DisplayTitle, DisplayOrder, IsActive, CreateDate, LastModified, IsDeleted)
    VALUES (@CategoryAttributeCardTypeId, N'Gold', N'طلایی', 2, 1, GETDATE(), GETDATE(), 0);
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributeAllowedValues WHERE AttributeId = @CategoryAttributeCardTypeId AND [Value] = N'Platinum' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributeAllowedValues (AttributeId, [Value], DisplayTitle, DisplayOrder, IsActive, CreateDate, LastModified, IsDeleted)
    VALUES (@CategoryAttributeCardTypeId, N'Platinum', N'پلاتینیوم', 3, 1, GETDATE(), GETDATE(), 0);
END

-- TransferType مقادیر مجاز
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributeAllowedValues WHERE AttributeId = @CategoryAttributeTransferTypeId AND [Value] = N'CardToCard' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributeAllowedValues (AttributeId, [Value], DisplayTitle, DisplayOrder, IsActive, CreateDate, LastModified, IsDeleted)
    VALUES (@CategoryAttributeTransferTypeId, N'CardToCard', N'کارت به کارت', 1, 1, GETDATE(), GETDATE(), 0);
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributeAllowedValues WHERE AttributeId = @CategoryAttributeTransferTypeId AND [Value] = N'Purchase' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributeAllowedValues (AttributeId, [Value], DisplayTitle, DisplayOrder, IsActive, CreateDate, LastModified, IsDeleted)
    VALUES (@CategoryAttributeTransferTypeId, N'Purchase', N'خرید', 2, 1, GETDATE(), GETDATE(), 0);
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributeAllowedValues WHERE AttributeId = @CategoryAttributeTransferTypeId AND [Value] = N'Shaba' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributeAllowedValues (AttributeId, [Value], DisplayTitle, DisplayOrder, IsActive, CreateDate, LastModified, IsDeleted)
    VALUES (@CategoryAttributeTransferTypeId, N'Shaba', N'شبا', 3, 1, GETDATE(), GETDATE(), 0);
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributeAllowedValues WHERE AttributeId = @CategoryAttributeTransferTypeId AND [Value] = N'Pal' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributeAllowedValues (AttributeId, [Value], DisplayTitle, DisplayOrder, IsActive, CreateDate, LastModified, IsDeleted)
    VALUES (@CategoryAttributeTransferTypeId, N'Pal', N'پل', 4, 1, GETDATE(), GETDATE(), 0);
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributeAllowedValues WHERE AttributeId = @CategoryAttributeTransferTypeId AND [Value] = N'AccountToAccount' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributeAllowedValues (AttributeId, [Value], DisplayTitle, DisplayOrder, IsActive, CreateDate, LastModified, IsDeleted)
    VALUES (@CategoryAttributeTransferTypeId, N'AccountToAccount', N'حساب به حساب', 5, 1, GETDATE(), GETDATE(), 0);
END

-- InstallmentProvider مقادیر مجاز (اعطای تسهیلات)
DECLARE @FacilityGrantInstallmentProviderAttributeId INT;
SELECT @FacilityGrantInstallmentProviderAttributeId = Id FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @FacilityGrantCategoryId AND [Key] = N'InstallmentProvider' AND IsDeleted = 0;

IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributeAllowedValues WHERE AttributeId = @FacilityGrantInstallmentProviderAttributeId AND [Value] = N'BNPL' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributeAllowedValues (AttributeId, [Value], DisplayTitle, DisplayOrder, IsActive, CreateDate, LastModified, IsDeleted)
    VALUES (@FacilityGrantInstallmentProviderAttributeId, N'BNPL', N'BNPL', 1, 1, GETDATE(), GETDATE(), 0);
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributeAllowedValues WHERE AttributeId = @FacilityGrantInstallmentProviderAttributeId AND [Value] = N'Kalaano' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributeAllowedValues (AttributeId, [Value], DisplayTitle, DisplayOrder, IsActive, CreateDate, LastModified, IsDeleted)
    VALUES (@FacilityGrantInstallmentProviderAttributeId, N'Kalaano', N'کالانو', 2, 1, GETDATE(), GETDATE(), 0);
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributeAllowedValues WHERE AttributeId = @FacilityGrantInstallmentProviderAttributeId AND [Value] = N'Premium' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributeAllowedValues (AttributeId, [Value], DisplayTitle, DisplayOrder, IsActive, CreateDate, LastModified, IsDeleted)
    VALUES (@FacilityGrantInstallmentProviderAttributeId, N'Premium', N'ممتاز', 3, 1, GETDATE(), GETDATE(), 0);
END

-- InstallmentProvider مقادیر مجاز (بازپرداخت اقساط)
DECLARE @RepaymentInstallmentProviderAttributeId INT;
SELECT @RepaymentInstallmentProviderAttributeId = Id FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId = @InstallmentRepaymentCategoryId AND [Key] = N'InstallmentProvider' AND IsDeleted = 0;

IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributeAllowedValues WHERE AttributeId = @RepaymentInstallmentProviderAttributeId AND [Value] = N'BNPL' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributeAllowedValues (AttributeId, [Value], DisplayTitle, DisplayOrder, IsActive, CreateDate, LastModified, IsDeleted)
    VALUES (@RepaymentInstallmentProviderAttributeId, N'BNPL', N'BNPL', 1, 1, GETDATE(), GETDATE(), 0);
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributeAllowedValues WHERE AttributeId = @RepaymentInstallmentProviderAttributeId AND [Value] = N'Kalaano' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributeAllowedValues (AttributeId, [Value], DisplayTitle, DisplayOrder, IsActive, CreateDate, LastModified, IsDeleted)
    VALUES (@RepaymentInstallmentProviderAttributeId, N'Kalaano', N'کالانو', 2, 1, GETDATE(), GETDATE(), 0);
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributeAllowedValues WHERE AttributeId = @RepaymentInstallmentProviderAttributeId AND [Value] = N'Premium' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributeAllowedValues (AttributeId, [Value], DisplayTitle, DisplayOrder, IsActive, CreateDate, LastModified, IsDeleted)
    VALUES (@RepaymentInstallmentProviderAttributeId, N'Premium', N'ممتاز', 3, 1, GETDATE(), GETDATE(), 0);
END

-- RepaymentStatus مقادیر مجاز
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributeAllowedValues WHERE AttributeId = @CategoryAttributeRepaymentStatusId AND [Value] = N'OnTime' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributeAllowedValues (AttributeId, [Value], DisplayTitle, DisplayOrder, IsActive, CreateDate, LastModified, IsDeleted)
    VALUES (@CategoryAttributeRepaymentStatusId, N'OnTime', N'پرداخت در سررسید', 1, 1, GETDATE(), GETDATE(), 0);
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributeAllowedValues WHERE AttributeId = @CategoryAttributeRepaymentStatusId AND [Value] = N'Early' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributeAllowedValues (AttributeId, [Value], DisplayTitle, DisplayOrder, IsActive, CreateDate, LastModified, IsDeleted)
    VALUES (@CategoryAttributeRepaymentStatusId, N'Early', N'زودتر از موعد', 2, 1, GETDATE(), GETDATE(), 0);
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributeAllowedValues WHERE AttributeId = @CategoryAttributeRepaymentStatusId AND [Value] = N'Late' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributeAllowedValues (AttributeId, [Value], DisplayTitle, DisplayOrder, IsActive, CreateDate, LastModified, IsDeleted)
    VALUES (@CategoryAttributeRepaymentStatusId, N'Late', N'دیرکرد', 3, 1, GETDATE(), GETDATE(), 0);
END

-- Provider مقادیر مجاز
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributeAllowedValues WHERE AttributeId = @CategoryAttributeProviderId AND [Value] = N'HamrahAval' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributeAllowedValues (AttributeId, [Value], DisplayTitle, DisplayOrder, IsActive, CreateDate, LastModified, IsDeleted)
    VALUES (@CategoryAttributeProviderId, N'HamrahAval', N'همراه اول', 1, 1, GETDATE(), GETDATE(), 0);
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributeAllowedValues WHERE AttributeId = @CategoryAttributeProviderId AND [Value] = N'Irancell' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributeAllowedValues (AttributeId, [Value], DisplayTitle, DisplayOrder, IsActive, CreateDate, LastModified, IsDeleted)
    VALUES (@CategoryAttributeProviderId, N'Irancell', N'ایرانسل', 2, 1, GETDATE(), GETDATE(), 0);
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributeAllowedValues WHERE AttributeId = @CategoryAttributeProviderId AND [Value] = N'Rightel' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributeAllowedValues (AttributeId, [Value], DisplayTitle, DisplayOrder, IsActive, CreateDate, LastModified, IsDeleted)
    VALUES (@CategoryAttributeProviderId, N'Rightel', N'رایتل', 3, 1, GETDATE(), GETDATE(), 0);
END

-- ServiceType مقادیر مجاز
IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributeAllowedValues WHERE AttributeId = @CategoryAttributeServiceTypeId AND [Value] = N'Charge' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributeAllowedValues (AttributeId, [Value], DisplayTitle, DisplayOrder, IsActive, CreateDate, LastModified, IsDeleted)
    VALUES (@CategoryAttributeServiceTypeId, N'Charge', N'شارژ', 1, 1, GETDATE(), GETDATE(), 0);
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.ProductCategoryAttributeAllowedValues WHERE AttributeId = @CategoryAttributeServiceTypeId AND [Value] = N'InternetPackage' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.ProductCategoryAttributeAllowedValues (AttributeId, [Value], DisplayTitle, DisplayOrder, IsActive, CreateDate, LastModified, IsDeleted)
    VALUES (@CategoryAttributeServiceTypeId, N'InternetPackage', N'بسته اینترنت', 2, 1, GETDATE(), GETDATE(), 0);
END

PRINT N'';
PRINT N'تمام مقادیر مجاز برای ویژگی‌ها ایجاد شدند.';
PRINT N'';

-- =====================================================
-- 5. ایجاد محصولات
-- =====================================================

-- خدمات حساب
IF NOT EXISTS (SELECT 1 FROM CoreConfig.Products WHERE TenantId = @TenantId AND [Key] = N'AccountOpening' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.Products (TenantId, Title, [Key], CategoryId, IsActive, TypicalUsageFrequency, RequiresSerialEntry, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'افتتاح حساب', N'AccountOpening', @AccountServicesCategoryId, 1, 0, 0, GETDATE(), GETDATE(), 0);
    PRINT N'محصول "افتتاح حساب" ایجاد شد';
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.Products WHERE TenantId = @TenantId AND [Key] = N'AccountStatement' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.Products (TenantId, Title, [Key], CategoryId, IsActive, TypicalUsageFrequency, RequiresSerialEntry, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'صورت حساب', N'AccountStatement', @AccountServicesCategoryId, 1, 0, 0, GETDATE(), GETDATE(), 0);
    PRINT N'محصول "صورت حساب" ایجاد شد';
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.Products WHERE TenantId = @TenantId AND [Key] = N'AccountBalance' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.Products (TenantId, Title, [Key], CategoryId, IsActive, TypicalUsageFrequency, RequiresSerialEntry, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'مانده', N'AccountBalance', @AccountServicesCategoryId, 1, 0, 0, GETDATE(), GETDATE(), 0);
    PRINT N'محصول "مانده" ایجاد شد';
END

-- خدمات کارت
IF NOT EXISTS (SELECT 1 FROM CoreConfig.Products WHERE TenantId = @TenantId AND [Key] = N'CardIssuance' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.Products (TenantId, Title, [Key], CategoryId, IsActive, TypicalUsageFrequency, RequiresSerialEntry, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'صدور', N'CardIssuance', @CardServicesCategoryId, 1, 0, 0, GETDATE(), GETDATE(), 0);
    PRINT N'محصول "صدور" ایجاد شد';
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.Products WHERE TenantId = @TenantId AND [Key] = N'CardReissuance' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.Products (TenantId, Title, [Key], CategoryId, IsActive, TypicalUsageFrequency, RequiresSerialEntry, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'صدور مجدد', N'CardReissuance', @CardServicesCategoryId, 1, 0, 0, GETDATE(), GETDATE(), 0);
    PRINT N'محصول "صدور مجدد" ایجاد شد';
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.Products WHERE TenantId = @TenantId AND [Key] = N'CardCancellation' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.Products (TenantId, Title, [Key], CategoryId, IsActive, TypicalUsageFrequency, RequiresSerialEntry, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'ابطال', N'CardCancellation', @CardServicesCategoryId, 1, 0, 0, GETDATE(), GETDATE(), 0);
    PRINT N'محصول "ابطال" ایجاد شد';
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.Products WHERE TenantId = @TenantId AND [Key] = N'CardPinChange' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.Products (TenantId, Title, [Key], CategoryId, IsActive, TypicalUsageFrequency, RequiresSerialEntry, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'تغییر رمز', N'CardPinChange', @CardServicesCategoryId, 1, 0, 0, GETDATE(), GETDATE(), 0);
    PRINT N'محصول "تغییر رمز" ایجاد شد';
END

-- انتقال وجه
IF NOT EXISTS (SELECT 1 FROM CoreConfig.Products WHERE TenantId = @TenantId AND [Key] = N'CardToCard' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.Products (TenantId, Title, [Key], CategoryId, IsActive, TypicalUsageFrequency, RequiresSerialEntry, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'کارت به کارت', N'CardToCard', @MoneyTransferCategoryId, 1, 0, 0, GETDATE(), GETDATE(), 0);
    PRINT N'محصول "کارت به کارت" ایجاد شد';
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.Products WHERE TenantId = @TenantId AND [Key] = N'Purchase' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.Products (TenantId, Title, [Key], CategoryId, IsActive, TypicalUsageFrequency, RequiresSerialEntry, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'خرید', N'Purchase', @MoneyTransferCategoryId, 1, 0, 0, GETDATE(), GETDATE(), 0);
    PRINT N'محصول "خرید" ایجاد شد';
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.Products WHERE TenantId = @TenantId AND [Key] = N'Shaba' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.Products (TenantId, Title, [Key], CategoryId, IsActive, TypicalUsageFrequency, RequiresSerialEntry, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'شبا', N'Shaba', @MoneyTransferCategoryId, 1, 0, 0, GETDATE(), GETDATE(), 0);
    PRINT N'محصول "شبا" ایجاد شد';
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.Products WHERE TenantId = @TenantId AND [Key] = N'Pal' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.Products (TenantId, Title, [Key], CategoryId, IsActive, TypicalUsageFrequency, RequiresSerialEntry, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'پل', N'Pal', @MoneyTransferCategoryId, 1, 0, 0, GETDATE(), GETDATE(), 0);
    PRINT N'محصول "پل" ایجاد شد';
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.Products WHERE TenantId = @TenantId AND [Key] = N'AccountToAccount' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.Products (TenantId, Title, [Key], CategoryId, IsActive, TypicalUsageFrequency, RequiresSerialEntry, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'حساب به حساب', N'AccountToAccount', @MoneyTransferCategoryId, 1, 0, 0, GETDATE(), GETDATE(), 0);
    PRINT N'محصول "حساب به حساب" ایجاد شد';
END

-- اعطای تسهیلات
IF NOT EXISTS (SELECT 1 FROM CoreConfig.Products WHERE TenantId = @TenantId AND [Key] = N'FacilityGrantBNPL' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.Products (TenantId, Title, [Key], CategoryId, IsActive, TypicalUsageFrequency, RequiresSerialEntry, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'اعطای تسهیلات BNPL', N'FacilityGrantBNPL', @FacilityGrantCategoryId, 1, 0, 0, GETDATE(), GETDATE(), 0);
    PRINT N'محصول "اعطای تسهیلات BNPL" ایجاد شد';
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.Products WHERE TenantId = @TenantId AND [Key] = N'FacilityGrantKalaano' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.Products (TenantId, Title, [Key], CategoryId, IsActive, TypicalUsageFrequency, RequiresSerialEntry, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'اعطای تسهیلات کالانو', N'FacilityGrantKalaano', @FacilityGrantCategoryId, 1, 0, 0, GETDATE(), GETDATE(), 0);
    PRINT N'محصول "اعطای تسهیلات کالانو" ایجاد شد';
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.Products WHERE TenantId = @TenantId AND [Key] = N'FacilityGrantPremium' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.Products (TenantId, Title, [Key], CategoryId, IsActive, TypicalUsageFrequency, RequiresSerialEntry, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'اعطای تسهیلات ممتاز', N'FacilityGrantPremium', @FacilityGrantCategoryId, 1, 0, 0, GETDATE(), GETDATE(), 0);
    PRINT N'محصول "اعطای تسهیلات ممتاز" ایجاد شد';
END

-- بازپرداخت اقساط
IF NOT EXISTS (SELECT 1 FROM CoreConfig.Products WHERE TenantId = @TenantId AND [Key] = N'InstallmentRepaymentBNPL' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.Products (TenantId, Title, [Key], CategoryId, IsActive, TypicalUsageFrequency, RequiresSerialEntry, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'بازپرداخت اقساط تسهیلات BNPL', N'InstallmentRepaymentBNPL', @InstallmentRepaymentCategoryId, 1, 0, 0, GETDATE(), GETDATE(), 0);
    PRINT N'محصول "بازپرداخت اقساط تسهیلات BNPL" ایجاد شد';
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.Products WHERE TenantId = @TenantId AND [Key] = N'InstallmentRepaymentKalaano' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.Products (TenantId, Title, [Key], CategoryId, IsActive, TypicalUsageFrequency, RequiresSerialEntry, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'بازپرداخت اقساط تسهیلات کالانو', N'InstallmentRepaymentKalaano', @InstallmentRepaymentCategoryId, 1, 0, 0, GETDATE(), GETDATE(), 0);
    PRINT N'محصول "بازپرداخت اقساط تسهیلات کالانو" ایجاد شد';
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.Products WHERE TenantId = @TenantId AND [Key] = N'InstallmentRepaymentPremium' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.Products (TenantId, Title, [Key], CategoryId, IsActive, TypicalUsageFrequency, RequiresSerialEntry, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'بازپرداخت اقساط تسهیلات ممتاز', N'InstallmentRepaymentPremium', @InstallmentRepaymentCategoryId, 1, 0, 0, GETDATE(), GETDATE(), 0);
    PRINT N'محصول "بازپرداخت اقساط تسهیلات ممتاز" ایجاد شد';
END

-- ارزش افزوده
IF NOT EXISTS (SELECT 1 FROM CoreConfig.Products WHERE TenantId = @TenantId AND [Key] = N'MobileCharge' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.Products (TenantId, Title, [Key], CategoryId, IsActive, TypicalUsageFrequency, RequiresSerialEntry, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'شارژ موبایل', N'MobileCharge', @ValueAddedCategoryId, 1, 0, 0, GETDATE(), GETDATE(), 0);
    PRINT N'محصول "شارژ موبایل" ایجاد شد';
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.Products WHERE TenantId = @TenantId AND [Key] = N'InternetPackage' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.Products (TenantId, Title, [Key], CategoryId, IsActive, TypicalUsageFrequency, RequiresSerialEntry, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'بسته اینترنت', N'InternetPackage', @ValueAddedCategoryId, 1, 0, 0, GETDATE(), GETDATE(), 0);
    PRINT N'محصول "بسته اینترنت" ایجاد شد';
END

PRINT N'';
PRINT N'تمام محصولات ایجاد شدند.';
PRINT N'';

-- =====================================================
-- 6. نمایش خلاصه
-- =====================================================

DECLARE @TotalCategories INT = (SELECT COUNT(*) FROM CoreConfig.ProductCategories WHERE TenantId = @TenantId AND IsDeleted = 0);
DECLARE @TotalProducts INT = (SELECT COUNT(*) FROM CoreConfig.Products WHERE TenantId = @TenantId AND IsDeleted = 0);
DECLARE @TotalTenantAttributes INT = (SELECT COUNT(*) FROM CoreConfig.TenantProductAttributes WHERE TenantId = @TenantId AND IsDeleted = 0);
DECLARE @TotalCategoryAttributes INT = (SELECT COUNT(*) FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId IN (SELECT Id FROM CoreConfig.ProductCategories WHERE TenantId = @TenantId AND IsDeleted = 0) AND IsDeleted = 0);
DECLARE @TotalAllowedValues INT = (SELECT COUNT(*) FROM CoreConfig.ProductCategoryAttributeAllowedValues WHERE AttributeId IN (SELECT Id FROM CoreConfig.ProductCategoryAttributes WHERE CategoryId IN (SELECT Id FROM CoreConfig.ProductCategories WHERE TenantId = @TenantId AND IsDeleted = 0) AND IsDeleted = 0) AND IsDeleted = 0);

PRINT N'';
PRINT N'================================================';
PRINT N'خلاصه داده‌های ایجاد شده:';
PRINT N'================================================';
PRINT N'تعداد دسته‌بندی‌ها: ' + CAST(@TotalCategories AS NVARCHAR(10));
PRINT N'تعداد محصولات: ' + CAST(@TotalProducts AS NVARCHAR(10));
PRINT N'تعداد ویژگی‌ها سطح اکوسیستم: ' + CAST(@TotalTenantAttributes AS NVARCHAR(10));
PRINT N'تعداد ویژگی‌ها سطح دسته‌بندی: ' + CAST(@TotalCategoryAttributes AS NVARCHAR(10));
PRINT N'تعداد مقادیر مجاز: ' + CAST(@TotalAllowedValues AS NVARCHAR(10));
PRINT N'================================================';
PRINT N'';
PRINT N'داده‌های درخت محصول با موفقیت ایجاد شدند!';

