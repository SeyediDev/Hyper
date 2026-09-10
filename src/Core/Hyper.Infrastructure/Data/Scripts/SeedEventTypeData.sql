-- =====================================================
-- Script برای افزودن داده‌های اولیه EventType و EventTypeParameter
-- این اسکریپت بر اساس سه سناریوی MVP طراحی شده است:
-- 1. Earn/Spend Points
-- 2. Multi-stage Campaign
-- 3. Installment Repayment
-- =====================================================

-- دریافت خودکار TenantId از دیتابیس
DECLARE @TenantId INT = (SELECT TOP 1 Id FROM CoreConfig.Tenants WHERE IsDeleted = 0 ORDER BY Id);

-- اگر TenantId پیدا نشد، از مقدار پیش‌فرض استفاده می‌شود
IF @TenantId IS NULL SET @TenantId = 1;

PRINT N'Using TenantId: ' + CAST(@TenantId AS NVARCHAR(10));
PRINT N'';
PRINT N'شروع ایجاد EventType و EventTypeParameter...';
PRINT N'';

-- =====================================================
-- 1. EventType ها - Scenario 1: Earn/Spend Points
-- =====================================================

DECLARE @EventTypeAccountOpeningId INT;
DECLARE @EventTypeFriendInvitationId INT;
DECLARE @EventTypeFriendRegistrationId INT;
DECLARE @EventTypeProductPurchaseId INT;
DECLARE @EventTypeProductPurchaseWithPointsId INT;

-- AccountOpening - افتتاح حساب
IF NOT EXISTS (SELECT 1 FROM CoreConfig.EventTypes WHERE TenantId = @TenantId AND [Key] = N'AccountOpening' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.EventTypes (TenantId, [Key], Title, CustomerBehaviorType, AllowNegativeBalance, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'AccountOpening', N'افتتاح حساب', 12, 0, GETDATE(), GETDATE(), 0);  -- Registration = 12
    SET @EventTypeAccountOpeningId = SCOPE_IDENTITY();
    PRINT N'EventType "AccountOpening" ایجاد شد (ID: ' + CAST(@EventTypeAccountOpeningId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @EventTypeAccountOpeningId = Id FROM CoreConfig.EventTypes WHERE TenantId = @TenantId AND [Key] = N'AccountOpening' AND IsDeleted = 0;
END

-- FriendInvitation - دعوت دوست
IF NOT EXISTS (SELECT 1 FROM CoreConfig.EventTypes WHERE TenantId = @TenantId AND [Key] = N'FriendInvitation' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.EventTypes (TenantId, [Key], Title, CustomerBehaviorType, AllowNegativeBalance, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'FriendInvitation', N'دعوت دوست', 29, 0, GETDATE(), GETDATE(), 0);  -- Referral = 29
    SET @EventTypeFriendInvitationId = SCOPE_IDENTITY();
    PRINT N'EventType "FriendInvitation" ایجاد شد (ID: ' + CAST(@EventTypeFriendInvitationId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @EventTypeFriendInvitationId = Id FROM CoreConfig.EventTypes WHERE TenantId = @TenantId AND [Key] = N'FriendInvitation' AND IsDeleted = 0;
END

-- FriendRegistration - ثبت‌نام دوست دعوت شده
IF NOT EXISTS (SELECT 1 FROM CoreConfig.EventTypes WHERE TenantId = @TenantId AND [Key] = N'FriendRegistration' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.EventTypes (TenantId, [Key], Title, CustomerBehaviorType, AllowNegativeBalance, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'FriendRegistration', N'ثبت‌نام دوست دعوت شده', 12, 0, GETDATE(), GETDATE(), 0);  -- Registration = 12
    SET @EventTypeFriendRegistrationId = SCOPE_IDENTITY();
    PRINT N'EventType "FriendRegistration" ایجاد شد (ID: ' + CAST(@EventTypeFriendRegistrationId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @EventTypeFriendRegistrationId = Id FROM CoreConfig.EventTypes WHERE TenantId = @TenantId AND [Key] = N'FriendRegistration' AND IsDeleted = 0;
END

-- ProductPurchase - خرید محصول
IF NOT EXISTS (SELECT 1 FROM CoreConfig.EventTypes WHERE TenantId = @TenantId AND [Key] = N'ProductPurchase' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.EventTypes (TenantId, [Key], Title, CustomerBehaviorType, AllowNegativeBalance, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'ProductPurchase', N'خرید محصول', 1, 0, GETDATE(), GETDATE(), 0);  -- Purchase = 1
    SET @EventTypeProductPurchaseId = SCOPE_IDENTITY();
    PRINT N'EventType "ProductPurchase" ایجاد شد (ID: ' + CAST(@EventTypeProductPurchaseId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @EventTypeProductPurchaseId = Id FROM CoreConfig.EventTypes WHERE TenantId = @TenantId AND [Key] = N'ProductPurchase' AND IsDeleted = 0;
END

-- ProductPurchaseWithPoints - خرید محصول با امتیاز
IF NOT EXISTS (SELECT 1 FROM CoreConfig.EventTypes WHERE TenantId = @TenantId AND [Key] = N'ProductPurchaseWithPoints' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.EventTypes (TenantId, [Key], Title, CustomerBehaviorType, AllowNegativeBalance, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'ProductPurchaseWithPoints', N'خرید محصول با امتیاز', 1, 0, GETDATE(), GETDATE(), 0);  -- Purchase = 1
    SET @EventTypeProductPurchaseWithPointsId = SCOPE_IDENTITY();
    PRINT N'EventType "ProductPurchaseWithPoints" ایجاد شد (ID: ' + CAST(@EventTypeProductPurchaseWithPointsId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @EventTypeProductPurchaseWithPointsId = Id FROM CoreConfig.EventTypes WHERE TenantId = @TenantId AND [Key] = N'ProductPurchaseWithPoints' AND IsDeleted = 0;
END

-- =====================================================
-- 2. EventType ها - Scenario 2: Multi-stage Campaign
-- =====================================================

DECLARE @EventTypeCommunityMembershipId INT;
DECLARE @EventTypeCampaignRewardReceivedId INT;
DECLARE @EventTypeLotteryEntryId INT;
DECLARE @EventTypeVIPStatusChangeId INT;

-- CommunityMembership - عضویت در جامعه
IF NOT EXISTS (SELECT 1 FROM CoreConfig.EventTypes WHERE TenantId = @TenantId AND [Key] = N'CommunityMembership' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.EventTypes (TenantId, [Key], Title, CustomerBehaviorType, AllowNegativeBalance, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'CommunityMembership', N'عضویت در جامعه', 31, 0, GETDATE(), GETDATE(), 0);  -- CampaignParticipation = 31
    SET @EventTypeCommunityMembershipId = SCOPE_IDENTITY();
    PRINT N'EventType "CommunityMembership" ایجاد شد (ID: ' + CAST(@EventTypeCommunityMembershipId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @EventTypeCommunityMembershipId = Id FROM CoreConfig.EventTypes WHERE TenantId = @TenantId AND [Key] = N'CommunityMembership' AND IsDeleted = 0;
END

-- CampaignRewardReceived - دریافت پاداش از کمپین
IF NOT EXISTS (SELECT 1 FROM CoreConfig.EventTypes WHERE TenantId = @TenantId AND [Key] = N'CampaignRewardReceived' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.EventTypes (TenantId, [Key], Title, CustomerBehaviorType, AllowNegativeBalance, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'CampaignRewardReceived', N'دریافت پاداش از کمپین', 45, 0, GETDATE(), GETDATE(), 0);  -- CampaignRewardClaim = 45
    SET @EventTypeCampaignRewardReceivedId = SCOPE_IDENTITY();
    PRINT N'EventType "CampaignRewardReceived" ایجاد شد (ID: ' + CAST(@EventTypeCampaignRewardReceivedId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @EventTypeCampaignRewardReceivedId = Id FROM CoreConfig.EventTypes WHERE TenantId = @TenantId AND [Key] = N'CampaignRewardReceived' AND IsDeleted = 0;
END

-- LotteryEntry - ورود به قرعه‌کشی
IF NOT EXISTS (SELECT 1 FROM CoreConfig.EventTypes WHERE TenantId = @TenantId AND [Key] = N'LotteryEntry' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.EventTypes (TenantId, [Key], Title, CustomerBehaviorType, AllowNegativeBalance, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'LotteryEntry', N'ورود به قرعه‌کشی', 34, 0, GETDATE(), GETDATE(), 0);  -- LotteryParticipation = 34
    SET @EventTypeLotteryEntryId = SCOPE_IDENTITY();
    PRINT N'EventType "LotteryEntry" ایجاد شد (ID: ' + CAST(@EventTypeLotteryEntryId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @EventTypeLotteryEntryId = Id FROM CoreConfig.EventTypes WHERE TenantId = @TenantId AND [Key] = N'LotteryEntry' AND IsDeleted = 0;
END

-- VIPStatusChange - تغییر وضعیت VIP
IF NOT EXISTS (SELECT 1 FROM CoreConfig.EventTypes WHERE TenantId = @TenantId AND [Key] = N'VIPStatusChange' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.EventTypes (TenantId, [Key], Title, CustomerBehaviorType, AllowNegativeBalance, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'VIPStatusChange', N'تغییر وضعیت VIP', 0, 0, GETDATE(), GETDATE(), 0);  -- Custom = 0
    SET @EventTypeVIPStatusChangeId = SCOPE_IDENTITY();
    PRINT N'EventType "VIPStatusChange" ایجاد شد (ID: ' + CAST(@EventTypeVIPStatusChangeId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @EventTypeVIPStatusChangeId = Id FROM CoreConfig.EventTypes WHERE TenantId = @TenantId AND [Key] = N'VIPStatusChange' AND IsDeleted = 0;
END

-- =====================================================
-- 3. EventType ها - Scenario 3: Installment Repayment
-- =====================================================

DECLARE @EventTypeInstallmentRepaymentId INT;

-- InstallmentRepayment - بازپرداخت اقساط
IF NOT EXISTS (SELECT 1 FROM CoreConfig.EventTypes WHERE TenantId = @TenantId AND [Key] = N'InstallmentRepayment' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.EventTypes (TenantId, [Key], Title, CustomerBehaviorType, AllowNegativeBalance, CreateDate, LastModified, IsDeleted)
    VALUES (@TenantId, N'InstallmentRepayment', N'بازپرداخت اقساط', 1, 1, GETDATE(), GETDATE(), 0);  -- Purchase = 1, AllowNegativeBalance = 1 (برای جریمه دیرکرد)
    SET @EventTypeInstallmentRepaymentId = SCOPE_IDENTITY();
    PRINT N'EventType "InstallmentRepayment" ایجاد شد (ID: ' + CAST(@EventTypeInstallmentRepaymentId AS NVARCHAR(10)) + N')';
END
ELSE
BEGIN
    SELECT @EventTypeInstallmentRepaymentId = Id FROM CoreConfig.EventTypes WHERE TenantId = @TenantId AND [Key] = N'InstallmentRepayment' AND IsDeleted = 0;
END

PRINT N'';
PRINT N'تمام EventType ها ایجاد شدند.';
PRINT N'';

-- =====================================================
-- 4. EventTypeParameter ها - Scenario 1: Earn/Spend Points
-- =====================================================

-- AccountOpening Parameters
IF NOT EXISTS (SELECT 1 FROM CoreConfig.EventTypeParameters WHERE EventTypeId = @EventTypeAccountOpeningId AND [Key] = N'AccountType' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.EventTypeParameters (EventTypeId, [Key], Title, ParameterType, IsOptional, CreatedBySystem, CreateDate, LastModified, IsDeleted)
    VALUES (@EventTypeAccountOpeningId, N'AccountType', N'نوع حساب', 0, 0, 0, GETDATE(), GETDATE(), 0);  -- String = 0
    PRINT N'Parameter "AccountType" برای AccountOpening ایجاد شد';
END

-- FriendInvitation Parameters
IF NOT EXISTS (SELECT 1 FROM CoreConfig.EventTypeParameters WHERE EventTypeId = @EventTypeFriendInvitationId AND [Key] = N'InvitedFriendMobile' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.EventTypeParameters (EventTypeId, [Key], Title, ParameterType, IsOptional, CreatedBySystem, CreateDate, LastModified, IsDeleted)
    VALUES (@EventTypeFriendInvitationId, N'InvitedFriendMobile', N'شماره موبایل دوست دعوت شده', 0, 0, 0, GETDATE(), GETDATE(), 0);  -- String = 0
    PRINT N'Parameter "InvitedFriendMobile" برای FriendInvitation ایجاد شد';
END

-- FriendRegistration Parameters
IF NOT EXISTS (SELECT 1 FROM CoreConfig.EventTypeParameters WHERE EventTypeId = @EventTypeFriendRegistrationId AND [Key] = N'ReferrerCustomerId' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.EventTypeParameters (EventTypeId, [Key], Title, ParameterType, IsOptional, CreatedBySystem, CreateDate, LastModified, IsDeleted)
    VALUES (@EventTypeFriendRegistrationId, N'ReferrerCustomerId', N'شناسه مشتری معرف', 1, 0, 0, GETDATE(), GETDATE(), 0);  -- Long = 1
    PRINT N'Parameter "ReferrerCustomerId" برای FriendRegistration ایجاد شد';
END

-- ProductPurchase Parameters
IF NOT EXISTS (SELECT 1 FROM CoreConfig.EventTypeParameters WHERE EventTypeId = @EventTypeProductPurchaseId AND [Key] = N'ProductKey' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.EventTypeParameters (EventTypeId, [Key], Title, ParameterType, IsOptional, CreatedBySystem, CreateDate, LastModified, IsDeleted)
    VALUES (@EventTypeProductPurchaseId, N'ProductKey', N'کلید محصول', 0, 0, 0, GETDATE(), GETDATE(), 0);  -- String = 0
    PRINT N'Parameter "ProductKey" برای ProductPurchase ایجاد شد';
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.EventTypeParameters WHERE EventTypeId = @EventTypeProductPurchaseId AND [Key] = N'Amount' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.EventTypeParameters (EventTypeId, [Key], Title, ParameterType, IsOptional, CreatedBySystem, CreateDate, LastModified, IsDeleted)
    VALUES (@EventTypeProductPurchaseId, N'Amount', N'مبلغ', 2, 0, 0, GETDATE(), GETDATE(), 0);  -- Float = 2
    PRINT N'Parameter "Amount" برای ProductPurchase ایجاد شد';
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.EventTypeParameters WHERE EventTypeId = @EventTypeProductPurchaseId AND [Key] = N'Provider' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.EventTypeParameters (EventTypeId, [Key], Title, ParameterType, IsOptional, CreatedBySystem, CreateDate, LastModified, IsDeleted)
    VALUES (@EventTypeProductPurchaseId, N'Provider', N'اپراتور', 0, 1, 0, GETDATE(), GETDATE(), 0);  -- String = 0, Optional = 1
    PRINT N'Parameter "Provider" برای ProductPurchase ایجاد شد';
END

-- ProductPurchaseWithPoints Parameters
IF NOT EXISTS (SELECT 1 FROM CoreConfig.EventTypeParameters WHERE EventTypeId = @EventTypeProductPurchaseWithPointsId AND [Key] = N'ProductKey' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.EventTypeParameters (EventTypeId, [Key], Title, ParameterType, IsOptional, CreatedBySystem, CreateDate, LastModified, IsDeleted)
    VALUES (@EventTypeProductPurchaseWithPointsId, N'ProductKey', N'کلید محصول', 0, 0, 0, GETDATE(), GETDATE(), 0);  -- String = 0
    PRINT N'Parameter "ProductKey" برای ProductPurchaseWithPoints ایجاد شد';
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.EventTypeParameters WHERE EventTypeId = @EventTypeProductPurchaseWithPointsId AND [Key] = N'Amount' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.EventTypeParameters (EventTypeId, [Key], Title, ParameterType, IsOptional, CreatedBySystem, CreateDate, LastModified, IsDeleted)
    VALUES (@EventTypeProductPurchaseWithPointsId, N'Amount', N'مبلغ', 2, 0, 0, GETDATE(), GETDATE(), 0);  -- Float = 2
    PRINT N'Parameter "Amount" برای ProductPurchaseWithPoints ایجاد شد';
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.EventTypeParameters WHERE EventTypeId = @EventTypeProductPurchaseWithPointsId AND [Key] = N'PointsUsed' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.EventTypeParameters (EventTypeId, [Key], Title, ParameterType, IsOptional, CreatedBySystem, CreateDate, LastModified, IsDeleted)
    VALUES (@EventTypeProductPurchaseWithPointsId, N'PointsUsed', N'امتیاز استفاده شده', 1, 0, 0, GETDATE(), GETDATE(), 0);  -- Long = 1
    PRINT N'Parameter "PointsUsed" برای ProductPurchaseWithPoints ایجاد شد';
END

-- =====================================================
-- 5. EventTypeParameter ها - Scenario 2: Multi-stage Campaign
-- =====================================================

-- AccountOpening Parameters (برای Scenario 2 - Premium account با MinimumBalance)
IF NOT EXISTS (SELECT 1 FROM CoreConfig.EventTypeParameters WHERE EventTypeId = @EventTypeAccountOpeningId AND [Key] = N'MinimumBalance' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.EventTypeParameters (EventTypeId, [Key], Title, ParameterType, IsOptional, CreatedBySystem, CreateDate, LastModified, IsDeleted)
    VALUES (@EventTypeAccountOpeningId, N'MinimumBalance', N'حداقل مانده', 2, 1, 0, GETDATE(), GETDATE(), 0);  -- Float = 2, Optional = 1
    PRINT N'Parameter "MinimumBalance" برای AccountOpening ایجاد شد';
END

-- CommunityMembership Parameters
IF NOT EXISTS (SELECT 1 FROM CoreConfig.EventTypeParameters WHERE EventTypeId = @EventTypeCommunityMembershipId AND [Key] = N'CommunityId' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.EventTypeParameters (EventTypeId, [Key], Title, ParameterType, IsOptional, CreatedBySystem, CreateDate, LastModified, IsDeleted)
    VALUES (@EventTypeCommunityMembershipId, N'CommunityId', N'شناسه جامعه', 0, 0, 0, GETDATE(), GETDATE(), 0);  -- String = 0
    PRINT N'Parameter "CommunityId" برای CommunityMembership ایجاد شد';
END

-- CampaignRewardReceived Parameters
IF NOT EXISTS (SELECT 1 FROM CoreConfig.EventTypeParameters WHERE EventTypeId = @EventTypeCampaignRewardReceivedId AND [Key] = N'RewardType' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.EventTypeParameters (EventTypeId, [Key], Title, ParameterType, IsOptional, CreatedBySystem, CreateDate, LastModified, IsDeleted)
    VALUES (@EventTypeCampaignRewardReceivedId, N'RewardType', N'نوع پاداش', 0, 0, 0, GETDATE(), GETDATE(), 0);  -- String = 0 (Silver, Gold, etc.)
    PRINT N'Parameter "RewardType" برای CampaignRewardReceived ایجاد شد';
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.EventTypeParameters WHERE EventTypeId = @EventTypeCampaignRewardReceivedId AND [Key] = N'Quantity' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.EventTypeParameters (EventTypeId, [Key], Title, ParameterType, IsOptional, CreatedBySystem, CreateDate, LastModified, IsDeleted)
    VALUES (@EventTypeCampaignRewardReceivedId, N'Quantity', N'تعداد', 1, 0, 0, GETDATE(), GETDATE(), 0);  -- Long = 1
    PRINT N'Parameter "Quantity" برای CampaignRewardReceived ایجاد شد';
END

-- LotteryEntry Parameters
IF NOT EXISTS (SELECT 1 FROM CoreConfig.EventTypeParameters WHERE EventTypeId = @EventTypeLotteryEntryId AND [Key] = N'LotteryId' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.EventTypeParameters (EventTypeId, [Key], Title, ParameterType, IsOptional, CreatedBySystem, CreateDate, LastModified, IsDeleted)
    VALUES (@EventTypeLotteryEntryId, N'LotteryId', N'شناسه قرعه‌کشی', 0, 0, 0, GETDATE(), GETDATE(), 0);  -- String = 0
    PRINT N'Parameter "LotteryId" برای LotteryEntry ایجاد شد';
END

-- VIPStatusChange Parameters
IF NOT EXISTS (SELECT 1 FROM CoreConfig.EventTypeParameters WHERE EventTypeId = @EventTypeVIPStatusChangeId AND [Key] = N'NewStatus' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.EventTypeParameters (EventTypeId, [Key], Title, ParameterType, IsOptional, CreatedBySystem, CreateDate, LastModified, IsDeleted)
    VALUES (@EventTypeVIPStatusChangeId, N'NewStatus', N'وضعیت جدید', 0, 0, 0, GETDATE(), GETDATE(), 0);  -- String = 0 (VIP, etc.)
    PRINT N'Parameter "NewStatus" برای VIPStatusChange ایجاد شد';
END

-- =====================================================
-- 6. EventTypeParameter ها - Scenario 3: Installment Repayment
-- =====================================================

-- InstallmentRepayment Parameters
IF NOT EXISTS (SELECT 1 FROM CoreConfig.EventTypeParameters WHERE EventTypeId = @EventTypeInstallmentRepaymentId AND [Key] = N'InstallmentProvider' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.EventTypeParameters (EventTypeId, [Key], Title, ParameterType, IsOptional, CreatedBySystem, CreateDate, LastModified, IsDeleted)
    VALUES (@EventTypeInstallmentRepaymentId, N'InstallmentProvider', N'ارائه‌دهنده تسهیلات', 0, 0, 0, GETDATE(), GETDATE(), 0);  -- String = 0 (BNPL, Kalaano, Premium)
    PRINT N'Parameter "InstallmentProvider" برای InstallmentRepayment ایجاد شد';
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.EventTypeParameters WHERE EventTypeId = @EventTypeInstallmentRepaymentId AND [Key] = N'RepaymentStatus' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.EventTypeParameters (EventTypeId, [Key], Title, ParameterType, IsOptional, CreatedBySystem, CreateDate, LastModified, IsDeleted)
    VALUES (@EventTypeInstallmentRepaymentId, N'RepaymentStatus', N'وضعیت بازپرداخت', 0, 0, 0, GETDATE(), GETDATE(), 0);  -- String = 0 (OnTime, Early, Late)
    PRINT N'Parameter "RepaymentStatus" برای InstallmentRepayment ایجاد شد';
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.EventTypeParameters WHERE EventTypeId = @EventTypeInstallmentRepaymentId AND [Key] = N'DaysToDueDate' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.EventTypeParameters (EventTypeId, [Key], Title, ParameterType, IsOptional, CreatedBySystem, CreateDate, LastModified, IsDeleted)
    VALUES (@EventTypeInstallmentRepaymentId, N'DaysToDueDate', N'تعداد روز تا سررسید', 1, 0, 0, GETDATE(), GETDATE(), 0);  -- Long = 1
    PRINT N'Parameter "DaysToDueDate" برای InstallmentRepayment ایجاد شد';
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.EventTypeParameters WHERE EventTypeId = @EventTypeInstallmentRepaymentId AND [Key] = N'InstallmentAmount' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.EventTypeParameters (EventTypeId, [Key], Title, ParameterType, IsOptional, CreatedBySystem, CreateDate, LastModified, IsDeleted)
    VALUES (@EventTypeInstallmentRepaymentId, N'InstallmentAmount', N'مبلغ قسط', 2, 0, 0, GETDATE(), GETDATE(), 0);  -- Float = 2
    PRINT N'Parameter "InstallmentAmount" برای InstallmentRepayment ایجاد شد';
END

IF NOT EXISTS (SELECT 1 FROM CoreConfig.EventTypeParameters WHERE EventTypeId = @EventTypeInstallmentRepaymentId AND [Key] = N'InstallmentNumber' AND IsDeleted = 0)
BEGIN
    INSERT INTO CoreConfig.EventTypeParameters (EventTypeId, [Key], Title, ParameterType, IsOptional, CreatedBySystem, CreateDate, LastModified, IsDeleted)
    VALUES (@EventTypeInstallmentRepaymentId, N'InstallmentNumber', N'شماره قسط', 1, 1, 0, GETDATE(), GETDATE(), 0);  -- Long = 1, Optional = 1
    PRINT N'Parameter "InstallmentNumber" برای InstallmentRepayment ایجاد شد';
END

PRINT N'';
PRINT N'تمام EventTypeParameter ها ایجاد شدند.';
PRINT N'';

-- =====================================================
-- 7. نمایش خلاصه
-- =====================================================

DECLARE @TotalEventTypes INT = (SELECT COUNT(*) FROM CoreConfig.EventTypes WHERE TenantId = @TenantId AND IsDeleted = 0);
DECLARE @TotalEventTypeParameters INT = (SELECT COUNT(*) FROM CoreConfig.EventTypeParameters WHERE EventTypeId IN (SELECT Id FROM CoreConfig.EventTypes WHERE TenantId = @TenantId AND IsDeleted = 0) AND IsDeleted = 0);

PRINT N'';
PRINT N'================================================';
PRINT N'خلاصه داده‌های ایجاد شده:';
PRINT N'================================================';
PRINT N'تعداد EventType ها: ' + CAST(@TotalEventTypes AS NVARCHAR(10));
PRINT N'تعداد EventTypeParameter ها: ' + CAST(@TotalEventTypeParameters AS NVARCHAR(10));
PRINT N'================================================';
PRINT N'';
PRINT N'داده‌های EventType و EventTypeParameter با موفقیت ایجاد شدند!';

