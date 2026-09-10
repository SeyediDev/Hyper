-- =====================================================
-- SQL Script for Promotion Scenarios Test Data
-- =====================================================
-- این اسکریپت داده‌های تست برای دو سناریو Promotion را ایجاد می‌کند
-- تاریخ: 2024-12-XX
-- =====================================================

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

-- =====================================================
-- Step 1: ایجاد EventType های مورد نیاز
-- =====================================================

SET IDENTITY_INSERT [CoreConfig].[EventTypes] ON;

-- EventType 1: افتتاح حساب ممتاز
IF NOT EXISTS (SELECT 1 FROM [CoreConfig].[EventTypes] WHERE Id = 1)
BEGIN
    INSERT INTO [CoreConfig].[EventTypes] (Id, TenantId, [Key], Title, IsDeleted, CreateDate, LastModified)
    VALUES (1, 1, N'AccountOpeningPremium', N'افتتاح حساب ممتاز', 0, GETDATE(), GETDATE());
END

-- EventType 2: خرید شارژ
IF NOT EXISTS (SELECT 1 FROM [CoreConfig].[EventTypes] WHERE Id = 2)
BEGIN
    INSERT INTO [CoreConfig].[EventTypes] (Id, TenantId, [Key], Title, IsDeleted, CreateDate, LastModified)
    VALUES (2, 1, N'ChargePurchase', N'خرید شارژ', 0, GETDATE(), GETDATE());
END

-- EventType 3: دعوت دوست
IF NOT EXISTS (SELECT 1 FROM [CoreConfig].[EventTypes] WHERE Id = 3)
BEGIN
    INSERT INTO [CoreConfig].[EventTypes] (Id, TenantId, [Key], Title, IsDeleted, CreateDate, LastModified)
    VALUES (3, 1, N'FriendInvitation', N'دعوت دوست', 0, GETDATE(), GETDATE());
END

-- EventType 4: بازپرداخت BNPL در موعد
IF NOT EXISTS (SELECT 1 FROM [CoreConfig].[EventTypes] WHERE Id = 4)
BEGIN
    INSERT INTO [CoreConfig].[EventTypes] (Id, TenantId, [Key], Title, IsDeleted, CreateDate, LastModified)
    VALUES (4, 1, N'BNPLPaymentOnTime', N'بازپرداخت BNPL در موعد', 0, GETDATE(), GETDATE());
END

-- EventType 5: بازپرداخت کالانو زودتر از موعد
IF NOT EXISTS (SELECT 1 FROM [CoreConfig].[EventTypes] WHERE Id = 5)
BEGIN
    INSERT INTO [CoreConfig].[EventTypes] (Id, TenantId, [Key], Title, IsDeleted, CreateDate, LastModified)
    VALUES (5, 1, N'KalanoPaymentEarly', N'بازپرداخت کالانو زودتر از موعد', 0, GETDATE(), GETDATE());
END

-- EventType 6: دیرکرد در بازپرداخت
IF NOT EXISTS (SELECT 1 FROM [CoreConfig].[EventTypes] WHERE Id = 6)
BEGIN
    INSERT INTO [CoreConfig].[EventTypes] (Id, TenantId, [Key], Title, IsDeleted, CreateDate, LastModified)
    VALUES (6, 1, N'PaymentLate', N'دیرکرد در بازپرداخت', 0, GETDATE(), GETDATE());
END

SET IDENTITY_INSERT [CoreConfig].[EventTypes] OFF;

-- =====================================================
-- Step 2: ایجاد Point Types (سکه نقره و طلا)
-- =====================================================

-- Point 1: سکه نقره
IF NOT EXISTS (SELECT 1 FROM [CoreConfig].[Points] WHERE Id = 1)
BEGIN
    INSERT INTO [CoreConfig].[Points] (Id, TenantId, Title, IsDeleted, CreateDate, LastModified)
    VALUES (1, 1, N'سکه نقره', 0, GETDATE(), GETDATE());
END

-- Point 2: سکه طلا
IF NOT EXISTS (SELECT 1 FROM [CoreConfig].[Points] WHERE Id = 2)
BEGIN
    INSERT INTO [CoreConfig].[Points] (Id, TenantId, Title, IsDeleted, CreateDate, LastModified)
    VALUES (2, 1, N'سکه طلا', 0, GETDATE(), GETDATE());
END

SET IDENTITY_INSERT [CoreConfig].[Points] OFF;

-- =====================================================
-- Step 3: ایجاد Customer Segment (جامعه x و VIP)
-- =====================================================

SET IDENTITY_INSERT [Core].[CustomerSegments] ON;

-- Segment 1: جامعه x
IF NOT EXISTS (SELECT 1 FROM [Core].[CustomerSegments] WHERE Id = 1)
BEGIN
    INSERT INTO [Core].[CustomerSegments] (Id, TenantId, Title, IsActive, EstimatedSize, IsDeleted, CreateDate, LastModified)
    VALUES (1, 1, N'جامعه x', 1, 0, 0, GETDATE(), GETDATE());
END

-- Segment 2: VIP
IF NOT EXISTS (SELECT 1 FROM [Core].[CustomerSegments] WHERE Id = 2)
BEGIN
    INSERT INTO [Core].[CustomerSegments] (Id, TenantId, Title, IsActive, EstimatedSize, IsDeleted, CreateDate, LastModified)
    VALUES (2, 1, N'VIP', 1, 0, 0, GETDATE(), GETDATE());
END

SET IDENTITY_INSERT [Core].[CustomerSegments] OFF;

-- =====================================================
-- Step 4: ایجاد Lottery (قرعه‌کشی x)
-- =====================================================

-- Lottery 1: قرعه‌کشی x
IF NOT EXISTS (SELECT 1 FROM [CoreConfig].[Lotteries] WHERE Id = 1)
BEGIN
    INSERT INTO [CoreConfig].[Lotteries] (Id, TenantId, Title, IsDeleted, CreateDate, LastModified)
    VALUES (1, 1, N'قرعه‌کشی x', 0, GETDATE(), GETDATE());
END

SET IDENTITY_INSERT [CoreConfig].[Lotteries] OFF;

-- =====================================================
-- Step 5: ایجاد Customer Parameter (سطح VIP)
-- =====================================================

SET IDENTITY_INSERT [Core].[CustomerParameters] ON;

-- Parameter 1: سطح VIP
IF NOT EXISTS (SELECT 1 FROM [Core].[CustomerParameters] WHERE Id = 1)
BEGIN
    INSERT INTO [Core].[CustomerParameters] (Id, TenantId, [Key], Title, IsDeleted, CreateDate, LastModified)
    VALUES (1, 1, N'VIPLevel', N'سطح VIP', 0, GETDATE(), GETDATE());
END

SET IDENTITY_INSERT [Core].[CustomerParameters] OFF;

-- =====================================================
-- Step 6: ایجاد Promotion برای سناریو 1
-- =====================================================

DECLARE @Scenario1PromotionId INT = 1;
DECLARE @FromDate DATETIME = '2024-10-03 19:00:00'; -- 3 دی 1403 ساعت 19
DECLARE @ToDate DATETIME = '2024-10-07 19:00:00'; -- 7 دی 1403 ساعت 19

IF NOT EXISTS (SELECT 1 FROM [CoreConfig].[Promotions] WHERE Id = @Scenario1PromotionId)
BEGIN
    INSERT INTO [CoreConfig].[Promotions] (
        Id, TenantId, Title, Category, FromDate, ToDate, 
        ScheduledHour, ScheduledMinute, Status, IsDeleted, CreateDate, LastModified
    )
    VALUES (
        @Scenario1PromotionId, 1, N'پویش 3 اقدام متوالی', 8, -- SpecialOffers
        @FromDate, @ToDate, 19, 0, 1, 0, GETDATE(), GETDATE() -- Status = Active
    );
END

SET IDENTITY_INSERT [CoreConfig].[Promotions] OFF;

-- PromotionCustomerSegment برای جامعه x
IF NOT EXISTS (SELECT 1 FROM [CoreConfig].[PromotionCustomerSegments] 
               WHERE PromotionId = @Scenario1PromotionId AND CustomerSegmentId = 1)
BEGIN
    INSERT INTO [CoreConfig].[PromotionCustomerSegments] (PromotionId, CustomerSegmentId, IsDeleted, CreateDate, LastModified)
    VALUES (@Scenario1PromotionId, 1, 0, GETDATE(), GETDATE());
END

-- Condition 1: افتتاح حساب ممتاز
IF NOT EXISTS (SELECT 1 FROM [CoreConfig].[PromotionConditions] WHERE Id = 1)
BEGIN
    INSERT INTO [CoreConfig].[PromotionConditions] (
        Id, PromotionId, Title, Type, EventTypeId, SequenceOrder,
        Kind, [Constraint], IsDeleted, CreateDate, LastModified
    )
    VALUES (
        1, @Scenario1PromotionId, N'افتتاح حساب ممتاز', 1, -- Event
        1, 1, 1, -- Formula, Constraint
        N'AccountType == ''Premium'' && Balance >= 3000000', 0, GETDATE(), GETDATE()
    );
END

-- Condition 2: خرید دو شارژ
IF NOT EXISTS (SELECT 1 FROM [CoreConfig].[PromotionConditions] WHERE Id = 2)
BEGIN
    INSERT INTO [CoreConfig].[PromotionConditions] (
        Id, PromotionId, Title, Type, EventTypeId, SequenceOrder,
        DependencyConditionId, MinimumCount, Kind, [Constraint], IsDeleted, CreateDate, LastModified
    )
    VALUES (
        2, @Scenario1PromotionId, N'خرید دو شارژ', 1, -- Event
        2, 2, 1, -- DependencyConditionId = 1
        2, 1, -- Formula, Constraint
        N'Amount == 5000 || Amount == 10000', 0, GETDATE(), GETDATE()
    );
END

-- Condition 3: دعوت دوست
IF NOT EXISTS (SELECT 1 FROM [CoreConfig].[PromotionConditions] WHERE Id = 3)
BEGIN
    INSERT INTO [CoreConfig].[PromotionConditions] (
        Id, PromotionId, Title, Type, EventTypeId, SequenceOrder,
        DependencyConditionId, MinimumCount, IsDeleted, CreateDate, LastModified
    )
    VALUES (
        3, @Scenario1PromotionId, N'دعوت دوست', 1, -- Event
        3, 3, 2, -- DependencyConditionId = 2
        1, 0, GETDATE(), GETDATE()
    );
END

SET IDENTITY_INSERT [CoreConfig].[PromotionConditions] OFF;

SET IDENTITY_INSERT [CoreConfig].[PromotionActions] ON;

-- Action 1: با انجام مرحله 1 - 2 سکه نقره (OnCondition)
IF NOT EXISTS (SELECT 1 FROM [CoreConfig].[PromotionActions] WHERE Id = 1)
BEGIN
    INSERT INTO [CoreConfig].[PromotionActions] (
        Id, PromotionId, PromotionConditionId, ActionType, ActionKind,
        PointId, AmountMethod, Amount, ActionOnWho, IsDeleted, CreateDate, LastModified
    )
    VALUES (
        1, @Scenario1PromotionId, 1, 2, -- OnCondition
        1, -- CreditPoint
        1, 1, N'2', 1, 0, GETDATE(), GETDATE() -- PointId = SilverCoin, FixAmount, Customer
    );
END

-- Action 2: با انجام هر 3 مرحله - 10 سکه طلا (OnCompletion)
IF NOT EXISTS (SELECT 1 FROM [CoreConfig].[PromotionActions] WHERE Id = 2)
BEGIN
    INSERT INTO [CoreConfig].[PromotionActions] (
        Id, PromotionId, PromotionConditionId, ActionType, ActionKind,
        PointId, AmountMethod, Amount, ActionOnWho, IsDeleted, CreateDate, LastModified
    )
    VALUES (
        2, @Scenario1PromotionId, NULL, 3, -- OnCompletion
        1, -- CreditPoint
        2, 1, N'10', 1, 0, GETDATE(), GETDATE() -- PointId = GoldCoin, FixAmount, Customer
    );
END

-- Action 3: شرکت در قرعه‌کشی (OnCompletion)
IF NOT EXISTS (SELECT 1 FROM [CoreConfig].[PromotionActions] WHERE Id = 3)
BEGIN
    INSERT INTO [CoreConfig].[PromotionActions] (
        Id, PromotionId, PromotionConditionId, ActionType, ActionKind,
        LotteryId, ActionOnWho, IsDeleted, CreateDate, LastModified
    )
    VALUES (
        3, @Scenario1PromotionId, NULL, 3, -- OnCompletion
        22, -- JoinLottery
        1, 1, 0, GETDATE(), GETDATE() -- LotteryId = 1, Customer
    );
END

-- Action 4: عضویت در سطح VIP (OnCompletion)
IF NOT EXISTS (SELECT 1 FROM [CoreConfig].[PromotionActions] WHERE Id = 4)
BEGIN
    INSERT INTO [CoreConfig].[PromotionActions] (
        Id, PromotionId, PromotionConditionId, ActionType, ActionKind,
        CustomerSegmentId, ActionOnWho, IsDeleted, CreateDate, LastModified
    )
    VALUES (
        4, @Scenario1PromotionId, NULL, 3, -- OnCompletion
        12, -- JoinInCustomerSegment
        2, 1, 0, GETDATE(), GETDATE() -- CustomerSegmentId = VIP, Customer
    );
END

SET IDENTITY_INSERT [CoreConfig].[PromotionActions] OFF;
SET IDENTITY_INSERT [CoreConfig].[Promotions] OFF;

-- =====================================================
-- Step 7: ایجاد Promotion برای سناریو 2
-- =====================================================

SET IDENTITY_INSERT [CoreConfig].[Promotions] ON;

DECLARE @Scenario2PromotionId INT = 2;

IF NOT EXISTS (SELECT 1 FROM [CoreConfig].[Promotions] WHERE Id = @Scenario2PromotionId)
BEGIN
    INSERT INTO [CoreConfig].[Promotions] (
        Id, TenantId, Title, Category, Status, IsDeleted, CreateDate, LastModified
    )
    VALUES (
        @Scenario2PromotionId, 1, N'پویش بازپرداخت اقساط', 1, -- RewardsAndPointsPrograms
        1, 0, GETDATE(), GETDATE() -- Status = Active
    );
END

-- Condition 1: بازپرداخت BNPL در موعد
IF NOT EXISTS (SELECT 1 FROM [CoreConfig].[PromotionConditions] WHERE Id = 4)
BEGIN
    INSERT INTO [CoreConfig].[PromotionConditions] (
        Id, PromotionId, Title, Type, EventTypeId, Kind, IsDeleted, CreateDate, LastModified
    )
    VALUES (
        4, @Scenario2PromotionId, N'بازپرداخت BNPL در موعد', 1, -- Event
        4, 0, 0, GETDATE(), GETDATE() -- WithoutExtraCondition
    );
END

SET IDENTITY_INSERT [CoreConfig].[Promotions] OFF;
SET IDENTITY_INSERT [CoreConfig].[PromotionConditions] OFF;

SET IDENTITY_INSERT [CoreConfig].[PromotionActions] ON;

-- Action 1: 1 سکه نقره (Immediate)
IF NOT EXISTS (SELECT 1 FROM [CoreConfig].[PromotionActions] WHERE Id = 5)
BEGIN
    INSERT INTO [CoreConfig].[PromotionActions] (
        Id, PromotionId, PromotionConditionId, ActionType, ActionKind,
        PointId, AmountMethod, Amount, ActionOnWho, IsDeleted, CreateDate, LastModified
    )
    VALUES (
        5, @Scenario2PromotionId, NULL, 1, -- Immediate
        1, -- CreditPoint
        1, 1, N'1', 1, 0, GETDATE(), GETDATE() -- PointId = SilverCoin, FixAmount, Customer
    );
END

-- Condition 2: بازپرداخت کالانو زودتر از موعد
IF NOT EXISTS (SELECT 1 FROM [CoreConfig].[PromotionConditions] WHERE Id = 5)
BEGIN
    INSERT INTO [CoreConfig].[PromotionConditions] (
        Id, PromotionId, Title, Type, EventTypeId, Kind, IsDeleted, CreateDate, LastModified
    )
    VALUES (
        5, @Scenario2PromotionId, N'بازپرداخت کالانو زودتر از موعد', 1, -- Event
        5, 0, 0, GETDATE(), GETDATE() -- WithoutExtraCondition
    );
END

-- Action 2: 2 سکه طلا (Immediate)
IF NOT EXISTS (SELECT 1 FROM [CoreConfig].[PromotionActions] WHERE Id = 6)
BEGIN
    INSERT INTO [CoreConfig].[PromotionActions] (
        Id, PromotionId, PromotionConditionId, ActionType, ActionKind,
        PointId, AmountMethod, Amount, ActionOnWho, IsDeleted, CreateDate, LastModified
    )
    VALUES (
        6, @Scenario2PromotionId, NULL, 1, -- Immediate
        1, -- CreditPoint
        2, 1, N'2', 1, 0, GETDATE(), GETDATE() -- PointId = GoldCoin, FixAmount, Customer
    );
END

-- Condition 3: دیرکرد در بازپرداخت
IF NOT EXISTS (SELECT 1 FROM [CoreConfig].[PromotionConditions] WHERE Id = 6)
BEGIN
    INSERT INTO [CoreConfig].[PromotionConditions] (
        Id, PromotionId, Title, Type, EventTypeId, Kind, IsDeleted, CreateDate, LastModified
    )
    VALUES (
        6, @Scenario2PromotionId, N'دیرکرد در بازپرداخت', 1, -- Event
        6, 0, 0, GETDATE(), GETDATE() -- WithoutExtraCondition
    );
END

SET IDENTITY_INSERT [CoreConfig].[PromotionConditions] OFF;

-- Action 3: کاهش 2 سکه نقره (Immediate)
IF NOT EXISTS (SELECT 1 FROM [CoreConfig].[PromotionActions] WHERE Id = 7)
BEGIN
    INSERT INTO [CoreConfig].[PromotionActions] (
        Id, PromotionId, PromotionConditionId, ActionType, ActionKind,
        PointId, AmountMethod, Amount, ActionOnWho, IsDeleted, CreateDate, LastModified
    )
    VALUES (
        7, @Scenario2PromotionId, NULL, 1, -- Immediate
        2, -- DebitPoint
        1, 1, N'2', 1, 0, GETDATE(), GETDATE() -- PointId = SilverCoin, FixAmount, Customer
    );
END

-- Action 4: نزول از طلایی به نقره‌ای (Immediate)
IF NOT EXISTS (SELECT 1 FROM [CoreConfig].[PromotionActions] WHERE Id = 8)
BEGIN
    INSERT INTO [CoreConfig].[PromotionActions] (
        Id, PromotionId, PromotionConditionId, ActionType, ActionKind,
        CustomerParameterId, AmountMethod, Amount, ActionOnWho, IsDeleted, CreateDate, LastModified
    )
    VALUES (
        8, @Scenario2PromotionId, NULL, 1, -- Immediate
        11, -- SetCustomerParameterValue
        1, 1, N'Silver', 1, 0, GETDATE(), GETDATE() -- CustomerParameterId = VIPLevel, FixAmount, Customer
    );
END

COMMIT TRANSACTION;

PRINT N'✅ داده‌های تست برای Promotion Scenarios با موفقیت ایجاد شدند.';
PRINT N'';
PRINT N'خلاصه:';
PRINT N'  - Promotion 1: پویش 3 اقدام متوالی (سناریو 1)';
PRINT N'  - Promotion 2: پویش بازپرداخت اقساط (سناریو 2)';
PRINT N'  - EventTypes: 6 نوع رویداد';
PRINT N'  - Points: 2 نوع سکه (نقره و طلا)';
PRINT N'  - Segments: 2 جامعه (x و VIP)';
PRINT N'  - Lottery: 1 قرعه‌کشی';
PRINT N'  - CustomerParameter: 1 پارامتر (سطح VIP)';

