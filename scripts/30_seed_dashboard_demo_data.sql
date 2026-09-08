/*
    Club dashboard demo-data seed
    --------------------------------
    اجرا: فایل را روی دیتابیس Domain برنامه در SSMS اجرا کنید.

    ویژگی‌ها:
      - برای SQL Server نوشته شده است.
      - داده‌های موجود کسب‌وکار را حذف یا تغییر نمی‌دهد.
      - فقط رویدادها/تراکنش‌ها/تجمیع‌های آزمایشی ساخته‌شده توسط همین فایل
        در اجرای مجدد بازسازی می‌شوند.
      - قابل اجرای مجدد است و مشتری، محصول، کمپین و ... را تکراری نمی‌کند.

    اگر می‌خواهید داده‌ها داخل Tenant موجود خاصی ساخته شوند، مقدار
    @TargetTenantId را از NULL به شناسه Tenant تغییر دهید.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @TargetTenantId int = NULL;
DECLARE @CustomerCount int = 360;
DECLARE @EventsPerCustomer int = 18;
DECLARE @Now datetime2(0) = SYSUTCDATETIME();
DECLARE @Today date = CONVERT(date, SYSUTCDATETIME());
DECLARE @MonthStart date = DATEFROMPARTS(YEAR(SYSUTCDATETIME()), MONTH(SYSUTCDATETIME()), 1);

/* ---------- مدل مورد انتظار ---------- */
DECLARE @RequiredTables table (ObjectName nvarchar(300) NOT NULL);
INSERT INTO @RequiredTables(ObjectName)
VALUES
    (N'[CoreConfig].[Tenants]'),
    (N'[CoreConfig].[Countries]'),
    (N'[CoreConfig].[Provinces]'),
    (N'[CoreConfig].[Cities]'),
    (N'[Core].[Customers]'),
    (N'[Core].[CustomerTenants]'),
    (N'[CoreConfig].[Points]'),
    (N'[CoreConfig].[ProductCategories]'),
    (N'[CoreConfig].[Products]'),
    (N'[CoreConfig].[EventTypes]'),
    (N'[CoreConfig].[EventChannels]'),
    (N'[CoreLog].[EventLogs]'),
    (N'[Core].[CustomerTransactions]'),
    (N'[CoreConfig].[Promotions]'),
    (N'[CoreConfig].[PromotionMetricses]'),
    (N'[Core].[PromotionMessages]'),
    (N'[Core].[Surveys]'),
    (N'[Core].[CustomerSegments]'),
    (N'[Core].[CustomerSegmentMemberships]'),
    (N'[CoreConfig].[TenantAttributes]'),
    (N'[CoreLog].[TenantAttributeDailyAggregations]');

DECLARE @MissingTables nvarchar(max);
SELECT @MissingTables = STRING_AGG(ObjectName, N', ')
FROM @RequiredTables
WHERE OBJECT_ID(ObjectName, N'U') IS NULL;

IF @MissingTables IS NOT NULL
BEGIN
    DECLARE @MissingMessage nvarchar(2048) =
        N'اسکریپت متوقف شد. ابتدا Migration دیتابیس را کامل کنید. جدول‌های موجود نیستند: '
        + LEFT(@MissingTables, 1800);
    THROW 51000, @MissingMessage, 1;
END;

IF OBJECT_ID(N'tempdb..#N') IS NOT NULL DROP TABLE #N;
SELECT TOP (1000)
       CONVERT(int, ROW_NUMBER() OVER (ORDER BY (SELECT NULL))) AS N
INTO #N
FROM sys.all_objects AS a
CROSS JOIN sys.all_objects AS b;
CREATE UNIQUE CLUSTERED INDEX IX_N ON #N(N);

BEGIN TRY
    BEGIN TRANSACTION;

    /* ---------- Tenant ---------- */
    DECLARE @TenantId int = @TargetTenantId;

    IF @TenantId IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM [CoreConfig].[Tenants] WHERE Id = @TenantId AND IsDeleted = 0)
        THROW 51001, N'@TargetTenantId در جدول Tenants پیدا نشد.', 1;

    IF @TenantId IS NULL
        SELECT @TenantId = Id
        FROM [CoreConfig].[Tenants]
        WHERE [Key] = N'__DASHBOARD_DEMO__' AND IsDeleted = 0;

    IF @TenantId IS NULL
    BEGIN
        INSERT INTO [CoreConfig].[Tenants]
            (Title, [Key], CreateDate, IsDeleted, LastModified)
        VALUES
            (N'اکوسیستم نمایشی داشبورد', N'__DASHBOARD_DEMO__', @Now, 0, @Now);

        SET @TenantId = CONVERT(int, SCOPE_IDENTITY());
    END;

    /* ---------- جغرافیا ---------- */
    DECLARE @CountryId int;
    SELECT @CountryId = Id FROM [CoreConfig].[Countries] WHERE Iso2 = N'IR' AND IsDeleted = 0;

    IF @CountryId IS NULL
    BEGIN
        INSERT INTO [CoreConfig].[Countries]
            (Title, EnglishTitle, Iso2, Iso3, IsoNumeric, PhoneCode,
             Latitude, Longitude, Pop, Tam, Sam, Som,
             CreateDate, IsDeleted, LastModified)
        VALUES
            (N'ایران', N'Iran', N'IR', N'IRN', 364, N'+98',
             32.4279, 53.6880, 86000000, 70000000, 42000000, 12000000,
             @Now, 0, @Now);
        SET @CountryId = CONVERT(int, SCOPE_IDENTITY());
    END;

    DECLARE @ProvinceData table
    (
        Slot int PRIMARY KEY,
        Title nvarchar(128) COLLATE Persian_100_CI_AI,
        EnglishTitle nvarchar(128) COLLATE Persian_100_CI_AI,
        Iso2 nvarchar(8) COLLATE Persian_100_CI_AI,
        Latitude decimal(9,5), Longitude decimal(9,5), Pop bigint
    );
    INSERT INTO @ProvinceData VALUES
        (1, N'تهران',       N'Tehran',             N'IR-07', 35.68920, 51.38900, 13500000),
        (2, N'اصفهان',      N'Isfahan',            N'IR-10', 32.65460, 51.66800,  5200000),
        (3, N'خراسان رضوی', N'Razavi Khorasan',    N'IR-30', 36.26050, 59.61680,  6800000),
        (4, N'فارس',        N'Fars',               N'IR-14', 29.59180, 52.58370,  5000000),
        (5, N'آذربایجان شرقی', N'East Azerbaijan', N'IR-03', 38.08000, 46.29190,  4100000),
        (6, N'خوزستان',     N'Khuzestan',          N'IR-06', 31.31830, 48.67060,  5000000);

    INSERT INTO [CoreConfig].[Provinces]
        (CountryId, Title, EnglishTitle, Iso2, Latitude, Longitude,
         Pop, Tam, Sam, Som, CreateDate, IsDeleted, LastModified)
    SELECT @CountryId, d.Title, d.EnglishTitle, d.Iso2, d.Latitude, d.Longitude,
           d.Pop, d.Pop * 8 / 10, d.Pop * 5 / 10, d.Pop * 2 / 10,
           @Now, 0, @Now
    FROM @ProvinceData AS d
    WHERE NOT EXISTS
    (
        SELECT 1 FROM [CoreConfig].[Provinces] AS p
        WHERE p.CountryId = @CountryId AND p.Iso2 = d.Iso2
    );

    DECLARE @CityData table
    (
        Slot int PRIMARY KEY,
        ProvinceIso2 nvarchar(8) COLLATE Persian_100_CI_AI,
        Title nvarchar(128) COLLATE Persian_100_CI_AI,
        EnglishTitle nvarchar(128) COLLATE Persian_100_CI_AI,
        Iso2 nvarchar(32) COLLATE Persian_100_CI_AI,
        Latitude decimal(9,5), Longitude decimal(9,5), Pop bigint
    );
    INSERT INTO @CityData VALUES
        (1, N'IR-07', N'تهران', N'Tehran', N'THR', 35.68920, 51.38900,  9000000),
        (2, N'IR-10', N'اصفهان', N'Isfahan', N'IFN', 32.65460, 51.66800, 2200000),
        (3, N'IR-30', N'مشهد', N'Mashhad', N'MHD', 36.26050, 59.61680, 3400000),
        (4, N'IR-14', N'شیراز', N'Shiraz', N'SYZ', 29.59180, 52.58370, 1900000),
        (5, N'IR-03', N'تبریز', N'Tabriz', N'TBZ', 38.08000, 46.29190, 1700000),
        (6, N'IR-06', N'اهواز', N'Ahvaz', N'AWZ', 31.31830, 48.67060, 1300000);

    INSERT INTO [CoreConfig].[Cities]
        (CountryId, ProvinceId, Title, EnglishTitle, Iso2, Latitude, Longitude,
         Pop, Tam, Sam, Som, CreateDate, IsDeleted, LastModified)
    SELECT @CountryId, p.Id, d.Title, d.EnglishTitle, d.Iso2, d.Latitude, d.Longitude,
           d.Pop, d.Pop * 8 / 10, d.Pop * 5 / 10, d.Pop * 2 / 10,
           @Now, 0, @Now
    FROM @CityData AS d
    JOIN [CoreConfig].[Provinces] AS p
      ON p.CountryId = @CountryId AND p.Iso2 = d.ProvinceIso2
    WHERE NOT EXISTS
    (
        SELECT 1 FROM [CoreConfig].[Cities] AS c
        WHERE c.ProvinceId = p.Id AND c.Title = d.Title
    );

    DECLARE @Geo table (Slot int PRIMARY KEY, ProvinceId int, CityId int);
    INSERT INTO @Geo(Slot, ProvinceId, CityId)
    SELECT d.Slot, p.Id, c.Id
    FROM @CityData AS d
    JOIN [CoreConfig].[Provinces] AS p
      ON p.CountryId = @CountryId AND p.Iso2 = d.ProvinceIso2
    JOIN [CoreConfig].[Cities] AS c
      ON c.ProvinceId = p.Id AND c.Title = d.Title;

    /* ---------- مشتری‌ها و شاخص‌های CLV/RFM ---------- */
    INSERT INTO [Core].[Customers]
        (FirstName, LastName, NationalCode, MobileNo, BirthDate,
         CreateDate, IsDeleted, LastModified)
    SELECT
        N'مشتری',
        CONCAT(N'نمایشی ', RIGHT(N'0000' + CONVERT(nvarchar(10), n.N), 4)),
        CONVERT(bigint, 8000000000) + n.N,
        N'0999' + RIGHT(N'0000000' + CONVERT(nvarchar(10), n.N), 7),
        DATEADD(DAY, -(7500 + (n.N * 37) % 12000), CONVERT(date, @Now)),
        DATEADD(DAY, -(n.N * 11) % 540, @Now),
        0,
        @Now
    FROM #N AS n
    WHERE n.N <= @CustomerCount
      AND NOT EXISTS
      (
          SELECT 1 FROM [Core].[Customers] AS c
          WHERE c.MobileNo = N'0999' + RIGHT(N'0000000' + CONVERT(nvarchar(10), n.N), 7)
      );

    INSERT INTO [Core].[CustomerTenants]
        (CustomerId, TenantId, FirstName, LastName,
         CountryId, ProvinceId, CityId,
         JoinDate, LeaveDate,
         LastInteractionDate, RecencyScore, TotalInteractions, FrequencyScore,
         TotalTransactionValue, MonetaryScore, RfmSegment,
         CustomerLifetimeValue, AverageOrderValue, PurchaseFrequency,
         CustomerAcquisitionCost, LtvToCacRatio, PaybackPeriodDays,
         CustomerProfitMargin, ReferralValue, RetentionRate, RepeatPurchaseRate,
         AverageTimeBetweenPurchasesDays, EngagementScore, LoyaltyScore,
         ChurnRiskScore, NpsScore, SatisfactionScore, IsActive,
         FirstInteractionDate, DaysSinceLastInteraction,
         TotalPointsEarned, TotalPointsRedeemed, CurrentPointsBalance,
         CreateDate, IsDeleted, LastModified)
    SELECT
        c.Id, @TenantId, c.FirstName, c.LastName,
        @CountryId, g.ProvinceId, g.CityId,
        DATEADD(DAY, -(n.N * 11) % 540, @Now),
        CASE WHEN n.N % 12 = 0 THEN DATEADD(DAY, -20, @Now) END,
        DATEADD(DAY, -(n.N % 120), @Now),
        1 + (n.N % 5), 8 + (n.N % 70), 1 + ((n.N * 3) % 5),
        CONVERT(decimal(18,2), 1500000 + n.N * 21750), 1 + ((n.N * 7) % 5),
        1 + ((n.N - 1) % 8),
        CONVERT(decimal(18,2), 3500000 + n.N * 82500),
        CONVERT(decimal(18,2), 180000 + (n.N % 20) * 35000),
        CONVERT(decimal(18,2), 0.50 + (n.N % 16) * 0.25),
        CONVERT(decimal(18,2), 120000 + (n.N % 15) * 18000),
        CONVERT(decimal(18,2), 2.20 + (n.N % 10) * 0.35),
        25 + (n.N % 100),
        CONVERT(decimal(18,2), 28 + (n.N % 55)),
        CONVERT(decimal(18,2), (n.N % 18) * 40000),
        CONVERT(decimal(18,2), 55 + (n.N % 42)),
        CONVERT(decimal(18,2), 18 + (n.N % 70)),
        7 + (n.N % 50),
        CONVERT(decimal(18,2), 15 + (n.N * 7) % 86),
        CONVERT(decimal(18,2), 20 + (n.N * 9) % 81),
        CONVERT(decimal(18,2), 5 + (n.N * 13) % 91),
        -100 + (n.N * 17) % 201,
        CONVERT(decimal(18,2), 25 + (n.N * 11) % 76),
        CASE WHEN n.N % 12 = 0 THEN 0 ELSE 1 END,
        DATEADD(DAY, -(200 + (n.N * 11) % 500), @Now),
        n.N % 120,
        CONVERT(bigint, 700 + (n.N * 37) % 8000),
        CONVERT(bigint, 100 + (n.N * 19) % 2200),
        CONVERT(bigint, 400 + (n.N * 23) % 5800),
        DATEADD(DAY, -(n.N * 11) % 540, @Now), 0, @Now
    FROM #N AS n
    JOIN [Core].[Customers] AS c
      ON c.MobileNo = N'0999' + RIGHT(N'0000000' + CONVERT(nvarchar(10), n.N), 7)
    JOIN @Geo AS g ON g.Slot = 1 + ((n.N - 1) % 6)
    WHERE n.N <= @CustomerCount
      AND NOT EXISTS
      (
          SELECT 1 FROM [Core].[CustomerTenants] AS ct
          WHERE ct.CustomerId = c.Id AND ct.TenantId = @TenantId
      );

    /* ---------- انواع امتیاز ---------- */
    DECLARE @PointData table
    (
        Slot int PRIMARY KEY,
        [Key] nvarchar(41) COLLATE Persian_100_CI_AI,
        Title nvarchar(41) COLLATE Persian_100_CI_AI,
        PointType int
    );
    INSERT INTO @PointData VALUES
        (1, N'__DEMO_NORMAL__', N'امتیاز وفاداری', 0),
        (2, N'__DEMO_XP__',     N'امتیاز تجربه',   1),
        (3, N'__DEMO_VALUE__',  N'امتیاز ارزشی',   2);

    INSERT INTO [CoreConfig].[Points]
        (TenantId, Title, [Key], PointType, AutoVisit, Visible,
         ShowInLeaderboard, Transferable, HasExpiration, ExpirationDays,
         AllowNegativeBalance, CreateDate, IsDeleted, LastModified)
    SELECT @TenantId, d.Title, d.[Key], d.PointType, 0, 1, 1,
           CASE WHEN d.Slot = 1 THEN 1 ELSE 0 END,
           CASE WHEN d.Slot = 2 THEN 1 ELSE 0 END,
           CASE WHEN d.Slot = 2 THEN 365 END,
           0, @Now, 0, @Now
    FROM @PointData AS d
    WHERE NOT EXISTS
    (
        SELECT 1 FROM [CoreConfig].[Points] AS p
        WHERE p.TenantId = @TenantId AND p.[Key] = d.[Key]
    );

    DECLARE @PointIds table (Slot int PRIMARY KEY, Id int);
    INSERT INTO @PointIds
    SELECT d.Slot, p.Id
    FROM @PointData AS d
    JOIN [CoreConfig].[Points] AS p
      ON p.TenantId = @TenantId AND p.[Key] = d.[Key];

    /* ---------- دسته‌ها و محصولات ---------- */
    DECLARE @CategoryData table
    (
        Slot int PRIMARY KEY,
        [Key] nvarchar(40) COLLATE Persian_100_CI_AI,
        Title nvarchar(81) COLLATE Persian_100_CI_AI,
        Description nvarchar(1000) COLLATE Persian_100_CI_AI
    );
    INSERT INTO @CategoryData VALUES
        (1, N'__DEMO_DIGITAL__', N'خدمات دیجیتال', N'محصولات و سرویس‌های دیجیتال'),
        (2, N'__DEMO_RETAIL__',  N'کالای مصرفی',   N'کالاهای پرمصرف روزانه'),
        (3, N'__DEMO_FINANCE__', N'خدمات مالی',    N'خدمات و محصولات مالی'),
        (4, N'__DEMO_LIFESTYLE__', N'سبک زندگی',   N'محصولات حوزه سبک زندگی');

    INSERT INTO [CoreConfig].[ProductCategories]
        (TenantId, Title, [Key], Description, ParentCategoryId,
         DisplayOrder, IsActive, PictureId, CreateDate, IsDeleted, LastModified)
    SELECT @TenantId, d.Title, d.[Key], d.Description, NULL,
           d.Slot, 1, NULL, @Now, 0, @Now
    FROM @CategoryData AS d
    WHERE NOT EXISTS
    (
        SELECT 1 FROM [CoreConfig].[ProductCategories] AS pc
        WHERE pc.TenantId = @TenantId AND pc.[Key] = d.[Key]
    );

    DECLARE @ProductData table
    (
        Slot int PRIMARY KEY, CategorySlot int,
        [Key] nvarchar(40) COLLATE Persian_100_CI_AI,
        Title nvarchar(81) COLLATE Persian_100_CI_AI,
        RepeatRate decimal(18,2), PurchaseCycle int, Lifetime int
    );
    INSERT INTO @ProductData VALUES
        (1,  1, N'__DEMO_PRODUCT_01', N'اشتراک طلایی',       72, 30, 900),
        (2,  1, N'__DEMO_PRODUCT_02', N'اشتراک نقره‌ای',     61, 45, 720),
        (3,  1, N'__DEMO_PRODUCT_03', N'بسته اینترنت ویژه',  48, 30, 540),
        (4,  2, N'__DEMO_PRODUCT_04', N'سبد خرید خانواده',   68, 20, 800),
        (5,  2, N'__DEMO_PRODUCT_05', N'کارت خرید فروشگاهی', 55, 35, 650),
        (6,  2, N'__DEMO_PRODUCT_06', N'بسته سلامت',         43, 60, 600),
        (7,  3, N'__DEMO_PRODUCT_07', N'کیف پول هوشمند',     78, 15, 950),
        (8,  3, N'__DEMO_PRODUCT_08', N'اعتبار خرید',        64, 40, 700),
        (9,  3, N'__DEMO_PRODUCT_09', N'بیمه سفر',           31, 180, 500),
        (10, 4, N'__DEMO_PRODUCT_10', N'باشگاه ورزشی',       58, 30, 760),
        (11, 4, N'__DEMO_PRODUCT_11', N'بسته سفر',           39, 120, 580),
        (12, 4, N'__DEMO_PRODUCT_12', N'سرگرمی آنلاین',      66, 25, 820);

    INSERT INTO [CoreConfig].[Products]
        (TenantId, Title, [Key], ProductCategoryId, Description,
         IsActive, PictureId, RequiresSerialEntry,
         ExpectedConsumptionDuration, TypicalUsageFrequency,
         AveragePurchaseCycle, ReorderThreshold, TypicalCustomerLifetime,
         AveragePurchasesPerCustomerLifetime, RepeatPurchaseRate,
         CreateDate, IsDeleted, LastModified)
    SELECT @TenantId, d.Title, d.[Key], pc.Id,
           CONCAT(N'محصول نمایشی داشبورد - ', d.Title),
           1, NULL, 0,
           d.PurchaseCycle, 6, d.PurchaseCycle, CONVERT(decimal(18,2), 25), d.Lifetime,
           CONVERT(decimal(18,2), d.Lifetime * 1.0 / NULLIF(d.PurchaseCycle, 0)),
           d.RepeatRate, @Now, 0, @Now
    FROM @ProductData AS d
    JOIN @CategoryData AS cd ON cd.Slot = d.CategorySlot
    JOIN [CoreConfig].[ProductCategories] AS pc
      ON pc.TenantId = @TenantId AND pc.[Key] = cd.[Key]
    WHERE NOT EXISTS
    (
        SELECT 1 FROM [CoreConfig].[Products] AS p
        WHERE p.TenantId = @TenantId AND p.[Key] = d.[Key]
    );

    DECLARE @ProductIds table (Slot int PRIMARY KEY, Id int, CategoryId int);
    INSERT INTO @ProductIds
    SELECT d.Slot, p.Id, p.ProductCategoryId
    FROM @ProductData AS d
    JOIN [CoreConfig].[Products] AS p
      ON p.TenantId = @TenantId AND p.[Key] = d.[Key];

    /* ---------- تحلیل تناسب محصول؛ در نسخه‌هایی که جدول آن وجود دارد ---------- */
    IF OBJECT_ID(N'[CoreConfig].[ProductFitAnalysises]', N'U') IS NOT NULL
    BEGIN
        INSERT INTO [CoreConfig].[ProductFitAnalysises]
            (TenantId, ProductId, Title, AnalysisDate,
             ProductMarketFitScore, ProductServiceFitScore,
             UniqueValueProposition, UniqueSellingProposition,
             ValueScore, CustomerSatisfactionScore, NetPromoterScore,
             CustomerSatisfactionRating, ProductQualityScore,
             ProductPerformanceScore, UsabilityScore, DesignScore, PriceScore,
             MarketDemandScore, CompetitionScore, DifferentiationScore, InnovationScore,
             AdoptionRate, RetentionRate, GrowthRate, ReferralRate, ConversionRate,
             ROIScore, ProfitabilityScore, ScalabilityScore,
             SuccessProbability, FailureProbability, GrowthProbability, CompetitionProbability,
             AnalysisAccuracy, AnalysisConfidence, DataSources, AnalysisMethod,
             LastUpdatedDate, ModelVersion, AdditionalData,
             CreateDate, IsDeleted, LastModified)
        SELECT @TenantId, p.Id, CONCAT(N'تحلیل ', d.Title), @Now,
               55 + d.Slot * 3, 52 + d.Slot * 3,
               N'ارزش پیشنهادی متمایز برای مشتری', N'تجربه بهتر و ارزش بیشتر',
               50 + d.Slot * 3, 58 + d.Slot * 2, 35 + d.Slot * 4,
               3.20 + d.Slot * 0.10, 55 + d.Slot * 3,
               54 + d.Slot * 3, 60 + d.Slot * 2, 52 + d.Slot * 3, 48 + d.Slot * 3,
               57 + d.Slot * 3, 35 + d.Slot * 2, 50 + d.Slot * 3, 48 + d.Slot * 3,
               30 + d.Slot * 4, 45 + d.Slot * 3, 20 + d.Slot * 4, 18 + d.Slot * 3, 8 + d.Slot * 2,
               40 + d.Slot * 4, 38 + d.Slot * 4, 42 + d.Slot * 3,
               45 + d.Slot * 4, 55 - d.Slot * 2, 40 + d.Slot * 4, 25 + d.Slot * 3,
               82, 78, N'Demo seed', N'Deterministic sample',
               @Now, N'1.0-demo', N'{"source":"dashboard-demo"}',
               @Now, 0, @Now
        FROM @ProductData AS d
        JOIN [CoreConfig].[Products] AS p
          ON p.TenantId = @TenantId AND p.[Key] = d.[Key]
        WHERE NOT EXISTS
        (
            SELECT 1 FROM [CoreConfig].[ProductFitAnalysises] AS a
            WHERE a.TenantId = @TenantId AND a.ProductId = p.Id
              AND a.Title = CONCAT(N'تحلیل ', d.Title)
        );
    END;

    /* ---------- کانال‌ها و نوع رویداد ---------- */
    DECLARE @ChannelData table
    (
        Slot int PRIMARY KEY,
        [Key] nvarchar(40) COLLATE Persian_100_CI_AI,
        Title nvarchar(81) COLLATE Persian_100_CI_AI
    );
    INSERT INTO @ChannelData VALUES
        (1, N'__DEMO_WEB__',      N'وب‌سایت'),
        (2, N'__DEMO_MOBILE__',   N'اپلیکیشن موبایل'),
        (3, N'__DEMO_BRANCH__',   N'شعبه حضوری'),
        (4, N'__DEMO_PARTNER__',  N'فروشگاه همکار');

    INSERT INTO [CoreConfig].[EventChannels]
        (TenantId, [Key], Title, ClientId, CreateDate, IsDeleted, LastModified)
    SELECT @TenantId, d.[Key], d.Title, CONCAT(N'demo-client-', d.Slot), @Now, 0, @Now
    FROM @ChannelData AS d
    WHERE NOT EXISTS
    (
        SELECT 1 FROM [CoreConfig].[EventChannels] AS c
        WHERE c.TenantId = @TenantId AND c.[Key] = d.[Key]
    );

    DECLARE @ChannelIds table (Slot int PRIMARY KEY, Id int);
    INSERT INTO @ChannelIds
    SELECT d.Slot, c.Id FROM @ChannelData AS d
    JOIN [CoreConfig].[EventChannels] AS c
      ON c.TenantId = @TenantId AND c.[Key] = d.[Key];

    DECLARE @EventTypeData table
    (
        Slot int PRIMARY KEY,
        [Key] nvarchar(40) COLLATE Persian_100_CI_AI,
        Title nvarchar(81) COLLATE Persian_100_CI_AI,
        BehaviorType int
    );
    INSERT INTO @EventTypeData VALUES
        (1, N'__DEMO_PURCHASE__', N'خرید محصول', 1),
        (2, N'__DEMO_LOGIN__',    N'ورود به سامانه', 10),
        (3, N'__DEMO_VIEW__',     N'مشاهده محصول', 3),
        (4, N'__DEMO_CART__',     N'افزودن به سبد', 4),
        (5, N'__DEMO_SHARE__',    N'اشتراک‌گذاری', 14),
        (6, N'__DEMO_REPEAT__',   N'خرید مجدد', 28);

    INSERT INTO [CoreConfig].[EventTypes]
        (TenantId, [Key], Title, AddNewAttributePermission,
         CustomerBehaviorType, CreateDate, IsDeleted, LastModified)
    SELECT @TenantId, d.[Key], d.Title, 1, d.BehaviorType, @Now, 0, @Now
    FROM @EventTypeData AS d
    WHERE NOT EXISTS
    (
        SELECT 1 FROM [CoreConfig].[EventTypes] AS e
        WHERE e.TenantId = @TenantId AND e.[Key] = d.[Key]
    );

    DECLARE @EventTypeIds table (Slot int PRIMARY KEY, Id int);
    INSERT INTO @EventTypeIds
    SELECT d.Slot, e.Id FROM @EventTypeData AS d
    JOIN [CoreConfig].[EventTypes] AS e
      ON e.TenantId = @TenantId AND e.[Key] = d.[Key];

    /* ---------- کمپین‌ها و متریک‌های بازاریابی ---------- */
    INSERT INTO [CoreConfig].[Promotions]
        (TenantId, Title, Category, FromDate, ToDate, FromHour, ToHour,
         Status, IsVisibleToCustomer, ShortDescription, Benefits,
         ParticipationGuide, CardImageUrl, BannerImageUrl, PrimaryColor,
         RewardType, IconUrl, CreateDate, IsDeleted, LastModified)
    SELECT @TenantId,
           CONCAT(N'__DEMO__ کمپین ', RIGHT(N'00' + CONVERT(nvarchar(2), n.N), 2)),
           1 + ((n.N - 1) % 5),
           DATEADD(MONTH, -(12 - n.N), @MonthStart),
           DATEADD(DAY, 25, DATEADD(MONTH, -(12 - n.N), @MonthStart)),
           8, 23,
           CASE WHEN n.N >= 10 THEN 1 ELSE 3 END,
           1,
           N'کمپین آزمایشی برای تکمیل نمودارهای داشبورد',
           N'امتیاز، تخفیف و تعامل بیشتر',
           N'با انجام فعالیت‌های تعریف‌شده در کمپین شرکت کنید.',
           NULL, NULL,
           CASE (n.N - 1) % 4 WHEN 0 THEN N'#2563EB' WHEN 1 THEN N'#16A34A' WHEN 2 THEN N'#EA580C' ELSE N'#9333EA' END,
           NULL, NULL,
           DATEADD(MONTH, -(12 - n.N), @MonthStart), 0, @Now
    FROM #N AS n
    WHERE n.N <= 12
      AND NOT EXISTS
      (
          SELECT 1 FROM [CoreConfig].[Promotions] AS p
          WHERE p.TenantId = @TenantId
            AND p.Title = CONCAT(N'__DEMO__ کمپین ', RIGHT(N'00' + CONVERT(nvarchar(2), n.N), 2))
      );

    DECLARE @PromotionIds table (Slot int PRIMARY KEY, Id int);
    INSERT INTO @PromotionIds
    SELECT n.N, p.Id
    FROM #N AS n
    JOIN [CoreConfig].[Promotions] AS p
      ON p.TenantId = @TenantId
     AND p.Title = CONCAT(N'__DEMO__ کمپین ', RIGHT(N'00' + CONVERT(nvarchar(2), n.N), 2))
    WHERE n.N <= 12;

    INSERT INTO [CoreConfig].[PromotionMetricses]
        (PromotionId, TargetAudienceCount, MessagesSent, MessagesDelivered,
         DeliveryRate, OpenCount, OpenRate, ClickCount, ClickThroughRate,
         ConversionCount, ConversionRate, CampaignCost, CampaignRevenue,
         ReturnOnInvestment, CustomerAcquisitionCost, CostPerConversion,
         NewCustomersAcquired, EffectivenessScore, LastMetricsCalculationDate,
         CreateDate, IsDeleted, LastModified)
    SELECT p.Id,
           4000 + p.Slot * 450,
           3500 + p.Slot * 410,
           3100 + p.Slot * 380,
           CONVERT(decimal(18,2), 86 + p.Slot * 0.65),
           1700 + p.Slot * 240,
           CONVERT(decimal(18,2), 48 + p.Slot * 1.20),
           550 + p.Slot * 95,
           CONVERT(decimal(18,2), 18 + p.Slot * 0.85),
           120 + p.Slot * 42,
           CONVERT(decimal(18,2), 3 + p.Slot * 0.72),
           CONVERT(decimal(18,2), 18000000 + p.Slot * 2100000),
           CONVERT(decimal(18,2), 30000000 + p.Slot * 7200000),
           CONVERT(decimal(18,2), 45 + p.Slot * 9.5),
           CONVERT(decimal(18,2), 420000 - p.Slot * 9000),
           CONVERT(decimal(18,2), 150000 - p.Slot * 2500),
           35 + p.Slot * 11,
           CONVERT(decimal(18,2), 52 + p.Slot * 3.5),
           @Now, @Now, 0, @Now
    FROM @PromotionIds AS p
    WHERE NOT EXISTS
    (
        SELECT 1 FROM [CoreConfig].[PromotionMetricses] AS m
        WHERE m.PromotionId = p.Id
    );

    UPDATE m
       SET m.TargetAudienceCount = 4000 + p.Slot * 450,
           m.MessagesSent = 3500 + p.Slot * 410,
           m.MessagesDelivered = 3100 + p.Slot * 380,
           m.DeliveryRate = CONVERT(decimal(18,2), 86 + p.Slot * 0.65),
           m.OpenCount = 1700 + p.Slot * 240,
           m.OpenRate = CONVERT(decimal(18,2), 48 + p.Slot * 1.20),
           m.ClickCount = 550 + p.Slot * 95,
           m.ClickThroughRate = CONVERT(decimal(18,2), 18 + p.Slot * 0.85),
           m.ConversionCount = 120 + p.Slot * 42,
           m.ConversionRate = CONVERT(decimal(18,2), 3 + p.Slot * 0.72),
           m.CampaignCost = CONVERT(decimal(18,2), 18000000 + p.Slot * 2100000),
           m.CampaignRevenue = CONVERT(decimal(18,2), 30000000 + p.Slot * 7200000),
           m.ReturnOnInvestment = CONVERT(decimal(18,2), 45 + p.Slot * 9.5),
           m.CustomerAcquisitionCost = CONVERT(decimal(18,2), 420000 - p.Slot * 9000),
           m.CostPerConversion = CONVERT(decimal(18,2), 150000 - p.Slot * 2500),
           m.NewCustomersAcquired = 35 + p.Slot * 11,
           m.EffectivenessScore = CONVERT(decimal(18,2), 52 + p.Slot * 3.5),
           m.LastMetricsCalculationDate = @Now,
           m.LastModified = @Now
    FROM [CoreConfig].[PromotionMetricses] AS m
    JOIN @PromotionIds AS p ON p.Id = m.PromotionId;

    INSERT INTO [Core].[PromotionMessages]
        (PromotionId, SendMethod, Subject, Content, Type, Priority,
         ScheduledTime, RetryCount, MaxRetries, Status, SentDate,
         SentCount, DeliveredCount, ReadCount,
         CreateDate, IsDeleted, LastModified)
    SELECT p.Id, 1,
           CONCAT(N'__DEMO_MSG__ ', RIGHT(N'00' + CONVERT(nvarchar(2), p.Slot), 2)),
           N'پیام نمایشی کمپین باشگاه مشتریان', 1, 1,
           DATEADD(HOUR, 9, CONVERT(datetime2, DATEADD(MONTH, -(12 - p.Slot), @MonthStart))),
           0, 3, 6,
           DATEADD(HOUR, 10, CONVERT(datetime2, DATEADD(MONTH, -(12 - p.Slot), @MonthStart))),
           3500 + p.Slot * 410, 3100 + p.Slot * 380, 1700 + p.Slot * 240,
           DATEADD(MONTH, -(12 - p.Slot), @MonthStart), 0, @Now
    FROM @PromotionIds AS p
    WHERE NOT EXISTS
    (
        SELECT 1 FROM [Core].[PromotionMessages] AS pm
        WHERE pm.PromotionId = p.Id AND pm.Subject = CONCAT(N'__DEMO_MSG__ ', RIGHT(N'00' + CONVERT(nvarchar(2), p.Slot), 2))
    );

    /* ---------- نظرسنجی‌ها ---------- */
    INSERT INTO [Core].[Surveys]
        (PromotionId, Title, Description, SurveyType, ProductId, IsActive,
         StartDate, EndDate, AllowMultipleSelection, ShowResults,
         TotalParticipants, ParticipationPoints, CorrectAnswerPoints,
         CreateDate, IsDeleted, LastModified)
    SELECT pr.Id,
           CONCAT(N'__DEMO__ نظرسنجی ', RIGHT(N'00' + CONVERT(nvarchar(2), n.N), 2)),
           N'نظرسنجی نمایشی برای گزارش‌های تجربه مشتری',
           CASE WHEN n.N % 3 = 0 THEN 2 ELSE 1 END,
           p.Id,
           CASE WHEN n.N >= 4 THEN 1 ELSE 0 END,
           DATEADD(MONTH, -n.N, @Now),
           CASE WHEN n.N >= 4 THEN DATEADD(MONTH, 2, @Now) ELSE DATEADD(DAY, 25, DATEADD(MONTH, -n.N, @Now)) END,
           CASE WHEN n.N % 2 = 0 THEN 1 ELSE 0 END,
           1,
           180 + n.N * 95,
           20, 50,
           DATEADD(MONTH, -n.N, @Now), 0, @Now
    FROM #N AS n
    JOIN @ProductIds AS p ON p.Slot = 1 + ((n.N - 1) % 12)
    JOIN @PromotionIds AS pr ON pr.Slot = 1 + ((n.N - 1) % 12)
    WHERE n.N <= 6
      AND NOT EXISTS
      (
          SELECT 1 FROM [Core].[Surveys] AS s
          WHERE s.PromotionId = pr.Id
            AND s.Title = CONCAT(N'__DEMO__ نظرسنجی ', RIGHT(N'00' + CONVERT(nvarchar(2), n.N), 2))
      );

    /* ---------- جامعه‌ها/بازارها ---------- */
    DECLARE @SegmentData table
    (
        Slot int PRIMARY KEY,
        [Key] nvarchar(41) COLLATE Persian_100_CI_AI,
        Title nvarchar(41) COLLATE Persian_100_CI_AI,
        Kind int,
        GrowthRate decimal(18,2), RetentionRate decimal(18,2), EngagementRate decimal(18,2)
    );
    INSERT INTO @SegmentData VALUES
        (1, N'__DEMO_CHAMPIONS__', N'قهرمانان برند',       8, 18.50, 94.20, 88.40),
        (2, N'__DEMO_LOYAL__',     N'مشتریان وفادار',     4, 14.20, 89.10, 79.60),
        (3, N'__DEMO_NEW__',       N'مشتریان جدید',       1, 27.80, 76.50, 65.30),
        (4, N'__DEMO_AT_RISK__',   N'در معرض ریزش',       8, -8.40, 51.70, 34.20),
        (5, N'__DEMO_TEHRAN__',    N'بازار تهران',        2, 21.10, 84.60, 73.80);

    INSERT INTO [Core].[CustomerSegments]
        (TenantId, Title, [Key], Kind, Description, JoinMode, [Constraint],
         IsVisibleInPortal, ImageUrl, Benefits, CountryId, ProvinceId, CityId,
         IsActive, EstimatedSize, ActualSize, LastCalculationDate,
         CalculationIntervalDays, GrowthRate, RetentionRate, EngagementRate,
         CreateDate, IsDeleted, LastModified)
    SELECT @TenantId, d.Title, d.[Key], d.Kind,
           N'جامعه نمایشی برای تحلیل داشبورد', 1, NULL,
           1, NULL, N'پیشنهادها و مزایای اختصاصی',
           CASE WHEN d.Slot = 5 THEN @CountryId END,
           CASE WHEN d.Slot = 5 THEN (SELECT ProvinceId FROM @Geo WHERE Slot = 1) END,
           CASE WHEN d.Slot = 5 THEN (SELECT CityId FROM @Geo WHERE Slot = 1) END,
           1, @CustomerCount / 3, @CustomerCount / 5,
           @Now, 1, d.GrowthRate, d.RetentionRate, d.EngagementRate,
           @Now, 0, @Now
    FROM @SegmentData AS d
    WHERE NOT EXISTS
    (
        SELECT 1 FROM [Core].[CustomerSegments] AS s
        WHERE s.TenantId = @TenantId AND s.[Key] = d.[Key]
    );

    DECLARE @SegmentIds table (Slot int PRIMARY KEY, Id int);
    INSERT INTO @SegmentIds
    SELECT d.Slot, s.Id
    FROM @SegmentData AS d
    JOIN [Core].[CustomerSegments] AS s
      ON s.TenantId = @TenantId AND s.[Key] = d.[Key];

    /* ---------- رویدادها و تراکنش‌های ۱۲ ماه اخیر ---------- */
    DELETE m
    FROM [Core].[CustomerSegmentMemberships] AS m
    JOIN @SegmentIds AS ds ON ds.Id = m.SegmentId
    JOIN [CoreLog].[EventLogs] AS el ON el.Id = m.EventLogId
    JOIN [CoreConfig].[EventTypes] AS et ON et.Id = el.EventTypeId
    WHERE el.TenantId = @TenantId
      AND LEFT(et.[Key], 7) = N'__DEMO_';

    DELETE tx
    FROM [Core].[CustomerTransactions] AS tx
    JOIN [CoreLog].[EventLogs] AS el ON el.Id = tx.EventLogId
    JOIN [CoreConfig].[EventTypes] AS et ON et.Id = el.EventTypeId
    JOIN [Core].[CustomerTenants] AS ct ON ct.Id = el.CustomerTenantId
    JOIN [Core].[Customers] AS c ON c.Id = ct.CustomerId
    JOIN #N AS customerN
      ON customerN.N <= @CustomerCount
     AND c.MobileNo = N'0999' + RIGHT(N'0000000' + CONVERT(nvarchar(10), customerN.N), 7)
    WHERE el.TenantId = @TenantId
      AND LEFT(et.[Key], 7) = N'__DEMO_';

    DELETE el
    FROM [CoreLog].[EventLogs] AS el
    JOIN [CoreConfig].[EventTypes] AS et ON et.Id = el.EventTypeId
    JOIN [Core].[CustomerTenants] AS ct ON ct.Id = el.CustomerTenantId
    JOIN [Core].[Customers] AS c ON c.Id = ct.CustomerId
    JOIN #N AS customerN
      ON customerN.N <= @CustomerCount
     AND c.MobileNo = N'0999' + RIGHT(N'0000000' + CONVERT(nvarchar(10), customerN.N), 7)
    WHERE el.TenantId = @TenantId
      AND LEFT(et.[Key], 7) = N'__DEMO_';

    INSERT INTO [CoreLog].[EventLogs]
        (TenantId, CustomerTenantId, ReceiveEventType,
         EventChannelId, EventTypeId, PromotionId,
         PointLevelId, RewardId, ProductCategoryId, ProductId,
         AssetId, ReferrerCodeId, ShamsiMonth,
         CreateDate, IsDeleted, LastModified)
    SELECT @TenantId, ct.Id, 1,
           ch.Id, et.Id, pr.Id,
           NULL, NULL, pd.CategoryId, pd.Id,
           NULL, NULL, NULL,
           DATEADD(MINUTE, (n.N * 37 + customerN.N * 13) % 1200,
               DATEADD(DAY, -((n.N * 17 + customerN.N * 3) % 365), CONVERT(datetime2, @Today))),
           0, @Now
    FROM #N AS customerN
    JOIN [Core].[Customers] AS c
      ON c.MobileNo = N'0999' + RIGHT(N'0000000' + CONVERT(nvarchar(10), customerN.N), 7)
    JOIN [Core].[CustomerTenants] AS ct
      ON ct.CustomerId = c.Id AND ct.TenantId = @TenantId
    CROSS JOIN #N AS n
    JOIN @ChannelIds AS ch ON ch.Slot = 1 + ((n.N + customerN.N - 2) % 4)
    JOIN @EventTypeIds AS et ON et.Slot = 1 + ((n.N + customerN.N - 2) % 6)
    JOIN @ProductIds AS pd ON pd.Slot = 1 + ((n.N * 3 + customerN.N - 2) % 12)
    JOIN @PromotionIds AS pr ON pr.Slot = 1 + ((n.N + customerN.N - 2) % 12)
    WHERE customerN.N <= @CustomerCount
      AND n.N <= @EventsPerCustomer;

    /* در دیتابیس فعلی EventLogId عضویت اجباری است؛ هر عضویت به یکی از
       رویدادهای همان مشتری متصل می‌شود. */
    INSERT INTO [Core].[CustomerSegmentMemberships]
        (CustomerTenantId, SegmentId, EventLogId, IsManual,
         CreateDate, IsDeleted, LastModified)
    SELECT ct.Id, s.Id, firstEvent.Id, 0, firstEvent.CreateDate, 0, @Now
    FROM #N AS n
    JOIN [Core].[Customers] AS c
      ON c.MobileNo = N'0999' + RIGHT(N'0000000' + CONVERT(nvarchar(10), n.N), 7)
    JOIN [Core].[CustomerTenants] AS ct
      ON ct.CustomerId = c.Id AND ct.TenantId = @TenantId
    JOIN @SegmentIds AS s ON s.Slot = 1 + ((n.N - 1) % 5)
    CROSS APPLY
    (
        SELECT TOP (1) el.Id, el.CreateDate
        FROM [CoreLog].[EventLogs] AS el
        JOIN [CoreConfig].[EventTypes] AS et ON et.Id = el.EventTypeId
        WHERE el.CustomerTenantId = ct.Id
          AND el.TenantId = @TenantId
          AND LEFT(et.[Key], 7) = N'__DEMO_'
        ORDER BY el.Id
    ) AS firstEvent
    WHERE n.N <= @CustomerCount
      AND NOT EXISTS
      (
          SELECT 1 FROM [Core].[CustomerSegmentMemberships] AS m
          WHERE m.CustomerTenantId = ct.Id AND m.SegmentId = s.Id AND m.IsDeleted = 0
      );

    INSERT INTO [Core].[CustomerSegmentMemberships]
        (CustomerTenantId, SegmentId, EventLogId, IsManual,
         CreateDate, IsDeleted, LastModified)
    SELECT ct.Id, s.Id, firstEvent.Id, 0, firstEvent.CreateDate, 0, @Now
    FROM #N AS n
    JOIN [Core].[Customers] AS c
      ON c.MobileNo = N'0999' + RIGHT(N'0000000' + CONVERT(nvarchar(10), n.N), 7)
    JOIN [Core].[CustomerTenants] AS ct
      ON ct.CustomerId = c.Id AND ct.TenantId = @TenantId
    JOIN @SegmentIds AS s ON s.Slot = CASE WHEN n.N % 2 = 0 THEN 1 ELSE 5 END
    CROSS APPLY
    (
        SELECT TOP (1) el.Id, el.CreateDate
        FROM [CoreLog].[EventLogs] AS el
        JOIN [CoreConfig].[EventTypes] AS et ON et.Id = el.EventTypeId
        WHERE el.CustomerTenantId = ct.Id
          AND el.TenantId = @TenantId
          AND LEFT(et.[Key], 7) = N'__DEMO_'
        ORDER BY el.Id DESC
    ) AS firstEvent
    WHERE n.N <= @CustomerCount AND n.N % 3 = 0
      AND NOT EXISTS
      (
          SELECT 1 FROM [Core].[CustomerSegmentMemberships] AS m
          WHERE m.CustomerTenantId = ct.Id AND m.SegmentId = s.Id AND m.IsDeleted = 0
      );

    UPDATE s
       SET s.ActualSize = CONVERT(int, x.MemberCount),
           s.EstimatedSize = CONVERT(int, x.MemberCount + (x.MemberCount / 5)),
           s.LastCalculationDate = @Now,
           s.LastModified = @Now
    FROM [Core].[CustomerSegments] AS s
    JOIN
    (
        SELECT m.SegmentId, COUNT_BIG(*) AS MemberCount
        FROM [Core].[CustomerSegmentMemberships] AS m
        WHERE m.IsDeleted = 0
        GROUP BY m.SegmentId
    ) AS x ON x.SegmentId = s.Id
    JOIN @SegmentIds AS ds ON ds.Id = s.Id;

    INSERT INTO [Core].[CustomerTransactions]
        (CustomerTenantId, PointId, Debit, Credit, Balance,
         TransactionType, VisitedAt, EventLogId, EventChannelId,
         ActivePlanId, RewardId, PromotionId, PromotionActionId,
         ExpirationDate, IsExpired, ExpiredDate, IsSpent,
         CreateDate, IsDeleted, LastModified)
    SELECT el.CustomerTenantId, pt.Id,
           CASE WHEN el.Id % 4 = 0 THEN 20 + (el.Id % 180) END,
           CASE WHEN el.Id % 4 <> 0 THEN 40 + (el.Id % 360) END,
           CASE WHEN el.Id % 4 = 0 THEN -(20 + (el.Id % 180)) ELSE 40 + (el.Id % 360) END,
           CASE WHEN el.Id % 4 = 0 THEN 1 ELSE 2 END,
           CASE WHEN el.Id % 3 <> 0 THEN DATEADD(HOUR, 2, el.CreateDate) END,
           el.Id, el.EventChannelId,
           NULL, NULL, el.PromotionId, NULL,
           CASE WHEN el.Id % 4 <> 0 THEN DATEADD(DAY, 365, el.CreateDate) END,
           0, NULL, CASE WHEN el.Id % 4 = 0 THEN 1 ELSE 0 END,
           el.CreateDate, 0, @Now
    FROM [CoreLog].[EventLogs] AS el
    JOIN [CoreConfig].[EventTypes] AS et ON et.Id = el.EventTypeId
    JOIN [Core].[CustomerTenants] AS ct ON ct.Id = el.CustomerTenantId
    JOIN [Core].[Customers] AS c ON c.Id = ct.CustomerId
    JOIN #N AS customerN
      ON customerN.N <= @CustomerCount
     AND c.MobileNo = N'0999' + RIGHT(N'0000000' + CONVERT(nvarchar(10), customerN.N), 7)
    JOIN @PointIds AS pt ON pt.Slot = 1 + (CONVERT(int, el.Id % 3))
    WHERE el.TenantId = @TenantId
      AND LEFT(et.[Key], 7) = N'__DEMO_';

    /* ---------- تقویم و روند ویژگی‌های روزانه ---------- */
    DECLARE @AttributeId int;
    SELECT @AttributeId = Id
    FROM [CoreConfig].[TenantAttributes]
    WHERE TenantId = @TenantId AND [Key] = N'__DEMO_DAILY_REVENUE__' AND IsDeleted = 0;

    IF @AttributeId IS NULL
    BEGIN
        INSERT INTO [CoreConfig].[TenantAttributes]
            (TenantId, Area, CustomerUsage, [Key],
             ValueType, ValueStorageType, Title, Description, IsActive,
             DefaultValue, ValidateByType, ValidateByList, IsOptional,
             CreatedBySystem,
             SegmentId, ProductCategoryId, ProductId, ChannelId, EventTypeId,
             CreateDate, IsDeleted, LastModified)
        VALUES
            (@TenantId, 1, 0, N'__DEMO_DAILY_REVENUE__',
             3, 3, N'درآمد روزانه نمایشی', N'داده آزمایشی تقویم و روند داشبورد', 1,
             N'0', 1, 0, 1,
             1,
             NULL, NULL, NULL, NULL, NULL,
             @Now, 0, @Now);
        SET @AttributeId = CONVERT(int, SCOPE_IDENTITY());
    END;

    DELETE FROM [CoreLog].[TenantAttributeDailyAggregations]
    WHERE TenantId = @TenantId AND AttributeId = @AttributeId;

    INSERT INTO [CoreLog].[TenantAttributeDailyAggregations]
        (TenantId, AttributeId, CustomerTenantId, SegmentId,
         ProductCategoryId, ProductId, EventTypeId, ChannelId,
         [Year], [Month], [Day], [Sum], [Count], Average,
         MinValue, MaxValue, FirstValue, LastValue, DistinctCount,
         AggregationDate, Status, [DateTime],
         CreateDate, IsDeleted, LastModified)
    SELECT @TenantId, @AttributeId, NULL, NULL,
           NULL, NULL, NULL, NULL,
           YEAR(d.AggregationDay), MONTH(d.AggregationDay), DAY(d.AggregationDay),
           CONVERT(decimal(18,2), 18000000 + n.N * 210000 + ((n.N * 17) % 9) * 750000),
           CONVERT(bigint, 70 + (n.N * 13) % 160),
           CONVERT(decimal(18,2), 180000 + (n.N * 1100) % 240000),
           CONVERT(decimal(18,2), 50000 + (n.N * 700) % 50000),
           CONVERT(decimal(18,2), 600000 + (n.N * 1300) % 500000),
           CONVERT(nvarchar(100), 18000000 + n.N * 210000),
           CONVERT(nvarchar(100), 19000000 + n.N * 240000),
           CONVERT(bigint, 45 + (n.N * 7) % 100),
           d.AggregationDay, 2, d.AggregationDay,
           d.AggregationDay, 0, @Now
    FROM #N AS n
    CROSS APPLY (SELECT DATEADD(DAY, -(n.N - 1), CONVERT(datetime2, @Today)) AS AggregationDay) AS d
    WHERE n.N <= 120;

    COMMIT TRANSACTION;

    /* ---------- نتیجه ---------- */
    SELECT
        DB_NAME() AS DatabaseName,
        @TenantId AS TenantId,
        (SELECT COUNT(*) FROM [Core].[CustomerTenants] WHERE TenantId = @TenantId AND IsDeleted = 0) AS Customers,
        (SELECT COUNT(*) FROM [CoreLog].[EventLogs] WHERE TenantId = @TenantId AND IsDeleted = 0) AS EventLogs,
        (SELECT COUNT(*)
           FROM [Core].[CustomerTransactions] t
           JOIN [CoreLog].[EventLogs] e ON e.Id = t.EventLogId
          WHERE e.TenantId = @TenantId AND t.IsDeleted = 0) AS Transactions,
        (SELECT COUNT(*) FROM [CoreConfig].[Promotions] WHERE TenantId = @TenantId AND IsDeleted = 0) AS Promotions,
        (SELECT COUNT(*) FROM [Core].[CustomerSegments] WHERE TenantId = @TenantId AND IsDeleted = 0) AS Segments,
        (SELECT COUNT(*)
           FROM [Core].[Surveys] s
           JOIN [CoreConfig].[Promotions] p ON p.Id = s.PromotionId
          WHERE p.TenantId = @TenantId AND s.IsDeleted = 0) AS Surveys,
        N'Dashboard demo data created successfully.' AS [Status];
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
