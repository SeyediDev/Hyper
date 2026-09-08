# Migration برای ویژگی طرح‌های اشتراک

## به‌روزرسانی تجربه چرخونه مشتریان (Loyalty Wheel) – 25 Nov 2025

### تغییرات جداول

1. **Lotteries**
   - ستون‌های جدید: `Description`, `WheelTheme`, `WheelSubtitle`, `WheelButtonLabel`, `WheelCallToAction`, `WheelCelebrationMessage`, `WheelBackgroundColor`, `WheelCenterIcon`, `SpinDurationSeconds`, `MaxDailySpins`, `MaxTotalSpins`, `IsActive`

2. **LotteryRewards**
   - ستون‌های جدید: `SegmentLabel`, `SegmentMessage`, `SegmentColor`, `SegmentTextColor`, `SegmentIcon`, `IsJackpot`

3. **LotteryParticipants**
   - ستون جدید: `LotteryRewardId` (به همراه ایندکس و Foreign Key به `LotteryRewards`)

### اسکریپت

فایل `scripts/2025-11-25_loyalty_wheel.sql` شامل دستورات `ALTER TABLE` و داده نمونه برای یک چرخونه فعال است.  
اسکریپت باید با ConnectionString با نام `Domain` اجرا شود.

### مراحل اجرا

```powershell
cd D:\Projects\Hyper
sqlcmd -S <ServerName> -d <DomainDatabase> -i .\scripts\2025-11-25_loyalty_wheel.sql
```

> **نکته:** شناسه‌های پاداش در بخش Seed با مقدارهای نمونه `101/102/103` درج شده‌اند. قبل از اجرا، آن‌ها را با `RewardId`های موجود در محیط مقصد جایگزین کنید.

## جداول جدید

### 1. Plans (طرح‌های اشتراک)
```sql
CREATE TABLE Plans (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TenantId INT NOT NULL,
    Title NVARCHAR(100) NOT NULL,
    Description NVARCHAR(1000),
    PriceInPoints BIGINT NOT NULL,
    PointId INT NOT NULL,
    ValidityDays INT NOT NULL,
    DiscountType INT NOT NULL, -- 1: Percentage, 2: FixedAmount
    DiscountValue DECIMAL(18,2) NOT NULL,
    CustomerSegmentId INT,
    IsActive BIT NOT NULL DEFAULT 1,
    IsGlobalDiscount BIT NOT NULL DEFAULT 0,
    OrderId INT,
    PictureId INT NULL,
    CreatedAt DATETIME2 NOT NULL,
    CreatedBy NVARCHAR(256),
    UpdatedAt DATETIME2,
    UpdatedBy NVARCHAR(256),
    
    CONSTRAINT FK_Plans_Tenant FOREIGN KEY (TenantId) REFERENCES Tenants(Id),
    CONSTRAINT FK_Plans_Point FOREIGN KEY (PointId) REFERENCES Points(Id),
    CONSTRAINT FK_Plans_CustomerSegment FOREIGN KEY (CustomerSegmentId) REFERENCES CustomerSegments(Id),
    CONSTRAINT FK_Plans_Picture FOREIGN KEY (PictureId) REFERENCES Documents(Id)
);

CREATE INDEX IX_Plans_TenantId ON Plans(TenantId);
CREATE INDEX IX_Plans_IsActive ON Plans(IsActive);
CREATE INDEX IX_Plans_TenantId_IsActive ON Plans(TenantId, IsActive);
```

### 2. CustomerPlans (طرح‌های فعال مشتری)
```sql
CREATE TABLE CustomerPlans (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CustomerId INT NOT NULL,
    PlanId INT NOT NULL,
    PurchaseDate DATETIME2 NOT NULL,
    StartDate DATETIME2 NOT NULL,
    ExpiryDate DATETIME2 NOT NULL,
    Status INT NOT NULL, -- 1: Active, 2: Expired, 3: Cancelled, 4: Suspended
    PaidAmount BIGINT NOT NULL,
    CustomerTransactionId BIGINT,
    UsageCount INT NOT NULL DEFAULT 0,
    TotalDiscountReceived BIGINT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    Notes NVARCHAR(500),
    CreatedAt DATETIME2 NOT NULL,
    CreatedBy NVARCHAR(256),
    UpdatedAt DATETIME2,
    UpdatedBy NVARCHAR(256),
    
    CONSTRAINT FK_CustomerPlans_Customer FOREIGN KEY (CustomerId) REFERENCES Customers(Id),
    CONSTRAINT FK_CustomerPlans_Plan FOREIGN KEY (PlanId) REFERENCES Plans(Id),
    CONSTRAINT FK_CustomerPlans_Transaction FOREIGN KEY (CustomerTransactionId) REFERENCES CustomerTransactions(Id),
    CONSTRAINT FK_CustomerPlans_Picture FOREIGN KEY (PictureId) REFERENCES Documents(Id)
);

CREATE INDEX IX_CustomerPlans_CustomerId ON CustomerPlans(CustomerId);
CREATE INDEX IX_CustomerPlans_PlanId ON CustomerPlans(PlanId);
CREATE INDEX IX_CustomerPlans_CustomerId_Status ON CustomerPlans(CustomerId, Status);
CREATE INDEX IX_CustomerPlans_CustomerId_Status_IsActive_ExpiryDate 
    ON CustomerPlans(CustomerId, Status, IsActive, ExpiryDate);
```

### 3. تغییر در جدول RewardCosts
```sql
ALTER TABLE RewardCosts
ADD PlanId INT NULL;

ALTER TABLE RewardCosts
ADD CONSTRAINT FK_RewardCosts_Plan FOREIGN KEY (PlanId) REFERENCES Plans(Id);

CREATE INDEX IX_RewardCosts_PlanId ON RewardCosts(PlanId);
```

### 4. تغییرات در ستون‌های تصویر
برای یکسان‌سازی ذخیره‌سازی تصاویر با موجودیت `Document`، ستون‌های زیر را اضافه کنید:

```sql
-- اگر ستون Picture قبلاً وجود دارد، ابتدا ستون جدید را اضافه کنید
ALTER TABLE Plans ADD PictureId INT NULL;
ALTER TABLE Rewards ADD PictureId INT NULL;

-- ثبت رابطه با جدول اسناد
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Plans_Picture')
BEGIN
    ALTER TABLE Plans
    ADD CONSTRAINT FK_Plans_Picture FOREIGN KEY (PictureId) REFERENCES Documents(Id);
END

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Rewards_Picture')
BEGIN
    ALTER TABLE Rewards
    ADD CONSTRAINT FK_Rewards_Picture FOREIGN KEY (PictureId) REFERENCES Documents(Id);
END

-- پس از انتقال داده‌های قبلی به جدول Documents، ستون قدیمی را حذف کنید
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'Picture' AND Object_ID = Object_ID('Plans'))
BEGIN
    ALTER TABLE Plans DROP COLUMN Picture;
END

IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'Picture' AND Object_ID = Object_ID('Rewards'))
BEGIN
    ALTER TABLE Rewards DROP COLUMN Picture;
END
```

> نکته: برای مهاجرت داده‌های موجود، ابتدا فایل‌های تصاویر را در جدول `Documents` یا Object Storage ثبت کنید و سپس مقدار `PictureId` را با `Id` سند مرتبط به‌روزرسانی نمایید.

## دستورات EF Core Migration

### ایجاد Migration

از دایرکتوری `Hyper.Infrastructure` اجرا کنید:

```bash
cd D:\Projects\Hyper\Backend\src\Core\Hyper.Infrastructure

# ایجاد Migration
dotnet ef migrations add AddPlansTables --startup-project ..\..\CustomerPortal\Hyper.CustomerPortal.Api\Hyper.CustomerPortal.Api.csproj --context HyperContextCommand

# اعمال به دیتابیس
dotnet ef database update --startup-project ..\..\CustomerPortal\Hyper.CustomerPortal.Api\Hyper.CustomerPortal.Api.csproj --context HyperContextCommand
```

### در صورت نیاز به حذف Migration

```bash
# حذف آخرین Migration
dotnet ef migrations remove --startup-project ..\..\CustomerPortal\Hyper.CustomerPortal.Api\Hyper.CustomerPortal.Api.csproj --context HyperContextCommand
```

### مشاهده Migration SQL

```bash
# تولید SQL Script
dotnet ef migrations script --startup-project ..\..\CustomerPortal\Hyper.CustomerPortal.Api\Hyper.CustomerPortal.Api.csproj --context HyperContextCommand --output migration.sql
```

## داده‌های نمونه (Optional)

پس از اجرای Migration، می‌توانید داده‌های نمونه اضافه کنید:

```sql
-- نمونه طرح با تخفیف درصدی
INSERT INTO Plans (TenantId, Title, Description, PriceInPoints, PointId, ValidityDays, DiscountType, DiscountValue, IsActive, IsGlobalDiscount, CreatedAt)
VALUES 
(1, 'طرح نقره‌ای', 'دریافت 10% تخفیف در تمام ریوارد‌ها به مدت 30 روز', 5000, 1, 30, 1, 10, 1, 1, GETUTCDATE()),
(1, 'طرح طلایی', 'دریافت 20% تخفیف در تمام ریوارد‌ها به مدت 60 روز', 8000, 1, 60, 1, 20, 1, 1, GETUTCDATE()),
(1, 'طرح پلاتینیوم', 'دریافت 30% تخفیف در تمام ریوارد‌ها به مدت 90 روز', 12000, 1, 90, 1, 30, 1, 1, GETUTCDATE());

-- نمونه طرح با تخفیف مقداری
INSERT INTO Plans (TenantId, Title, Description, PriceInPoints, PointId, ValidityDays, DiscountType, DiscountValue, IsActive, IsGlobalDiscount, CreatedAt)
VALUES 
(1, 'طرح استارتر', 'دریافت 1000 امتیاز تخفیف در هر خرید به مدت 30 روز', 3000, 1, 30, 2, 1000, 1, 1, GETUTCDATE());
```

## نکات مهم

1. **Backup**: قبل از اجرای Migration، حتماً از دیتابیس Backup بگیرید
2. **ConnectionString**: مطمئن شوید ConnectionString در `appsettings.json` صحیح است
3. **Testing**: ابتدا در محیط Development تست کنید
4. **Rollback**: در صورت بروز مشکل، از `dotnet ef database update {PreviousMigrationName}` برای بازگشت استفاده کنید

## بررسی Migration

بعد از اجرای Migration، موارد زیر را بررسی کنید:

1. ✅ جدول `Plans` ایجاد شده است
2. ✅ جدول `CustomerPlans` ایجاد شده است
3. ✅ ستون `PlanId` به جدول `RewardCosts` اضافه شده است
4. ✅ تمام Foreign Key ها صحیح تعریف شده‌اند
5. ✅ Index های مورد نیاز ایجاد شده‌اند

## Troubleshooting

### خطای Context
اگر خطای مربوط به Context دریافت کردید:
```bash
dotnet ef dbcontext list --startup-project ..\..\CustomerPortal\Hyper.CustomerPortal.Api\Hyper.CustomerPortal.Api.csproj
```

### خطای Build
اگر پروژه Build نمی‌شود:
```bash
cd D:\Projects\Hyper\Backend
dotnet build
```

### خطای ConnectionString
مطمئن شوید در `appsettings.Development.json` فایل CustomerPortal API، ConnectionString صحیح تنظیم شده است.


