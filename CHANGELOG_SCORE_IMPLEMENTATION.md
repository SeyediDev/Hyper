# Changelog - Scoring & Points System Implementation

**Date:** 2024-12-29  
**Version:** 1.0

## Summary

این مستند تغییرات انجام شده در سیستم Scoring & Points را شرح می‌دهد.

---

## 1. SCORE-POINT-EXPIRATION: سیستم تاریخ انقضای امتیازات

### تغییرات Entity
- **CustomerTransaction**: فیلد `ExpirationDate` از قبل وجود داشت
- فیلدهای `IsExpired`, `ExpiredDate`, `IsSpent` از قبل وجود داشتند

### تغییرات Business Logic
- **PromotionActionService.SetPointBaseAction**: 
  - هنگام اعطای امتیاز (CreditPoint و SetPointBalance)، اگر `Point.HasExpiration = true` باشد، `ExpirationDate` محاسبه و تنظیم می‌شود
  - فرمول: `ExpirationDate = DateTime.UtcNow.AddDays(Point.ExpirationDays.Value)`

### Job جدید
- **ExpirePointsJob**: 
  - Job دوره‌ای که هر روز ساعت 2 صبح اجرا می‌شود
  - تراکنش‌های Credit که `ExpirationDate <= Now` و `IsExpired = false` و `IsSpent = false` هستند را پیدا می‌کند
  - آن‌ها را به عنوان منقضی شده علامت‌گذاری می‌کند

### تغییرات UI
- **Admin Panel**: فیلدهای `ExpirationDate`, `IsExpired`, `ExpiredDate`, `IsSpent` به `CustomerTransactionUiDefinitions` اضافه شد
- **Customer Portal**: DTO ها و query handlers به‌روزرسانی شدند

### تست‌ها
- **ExpirePointsJobTests**: 4 تست برای سناریوهای مختلف انقضا

---

## 2. SCORE-POINT-LEVEL-VIP: اضافه کردن SetPointLevel

### تغییرات Entity
- **PromotionActionKind**: 
  - `SetPointLevel = 4` اضافه شد

- **PromotionAction**: 
  - `PointLevelId` (int?) اضافه شد
  - `PointLevel` (PointLevel?) اضافه شد

### تغییرات Business Logic
- **PromotionActionService**: 
  - متد جدید `SetPointLevelAction` پیاده‌سازی شد
  - منطق:
    1. بررسی می‌کند که `PointLevelId` null نباشد
    2. `CustomerTenant` را پیدا می‌کند
    3. سطح قدیمی مشتری را پیدا می‌کند
    4. اگر سطح قدیمی متفاوت باشد، آن را expire می‌کند
    5. سطح جدید را ایجاد می‌کند

### تغییرات UI
- **PromotionActionUiDefinitions**: 
  - فیلد `PointLevel` به فرم اضافه شد
  - UI Rule اضافه شد: `PointLevel` فقط زمانی نمایش داده می‌شود که `ActionKind = SetPointLevel`

### تست‌ها
- **PromotionActionServiceTests**: 3 تست برای `SetPointLevel`

### نکته مهم
VIP یک `CustomerSegment` است نه `PointLevel`. برای VIP باید از `JoinInCustomerSegment` استفاده شود.

---

## 3. SCORE-RULES-PENALTY: کنترل منفی شدن Balance

### تغییرات Entity
- **EventType**: 
  - `AllowNegativeBalance` (bool) اضافه شد (پیش‌فرض: false)
  - این فیلد تعیین می‌کند که آیا این رویداد می‌تواند باعث منفی شدن balance امتیاز شود یا نه

### تغییرات Business Logic
- **PromotionActionService.SetPointBaseAction**: 
  - در case `DebitPoint`:
    1. `currentBalance` و `newBalance` محاسبه می‌شود
    2. `EventLog` و `EventType` را می‌خواند
    3. اگر `AllowNegativeBalance = false` و `newBalance < 0` باشد:
       - Action رد می‌شود (transaction ایجاد نمی‌شود)
       - Warning log می‌شود
    4. در غیر این صورت، transaction ایجاد می‌شود

### تغییرات UI
- **EventTypeUiDefinitions**: 
  - فیلد `AllowNegativeBalance` به فرم اضافه شد

### تست‌ها
- **PromotionActionServiceTests**: 3 تست برای `DebitPoint` با سناریوهای مختلف:
  - رد شدن اگر `AllowNegativeBalance = false` و balance منفی شود
  - اجازه دادن اگر `AllowNegativeBalance = true`
  - اجازه دادن اگر balance مثبت بماند

---

## Migration Notes

طبق قوانین پروژه Hyper، نیازی به نوشتن SQL Migration Script نیست. پروژه یک ORM سفارشی دارد که با فراخوانی `migrationManager.StartMigration` (دو الی سه بار) DDL ها خودکار تولید و اجرا می‌شوند.

### فیلدهای جدید که نیاز به Migration دارند:
1. **PromotionAction.PointLevelId** (int?, nullable)
2. **EventType.AllowNegativeBalance** (bool, default: false)

---

## فایل‌های تغییر یافته

### Entities
- `Backend/src/Core/Hyper.Domain/Entities/Promotions/Enums/PromotionActionKind.cs`
- `Backend/src/Core/Hyper.Domain/Entities/Promotions/PromotionAction.cs`
- `Backend/src/Core/Hyper.Domain/Entities/Events/EventType.cs`

### Business Logic
- `Backend/src/Core/Hyper.Domain/Features/Promotions/PromotionActionService.cs`

### Jobs
- `Backend/src/Core/Hyper.Application/Features/Points/Jobs/IExpirePointsJob.cs`
- `Backend/src/Core/Hyper.Application/Features/Points/Jobs/ExpirePointsJob.cs`
- `Backend/src/Core/Hyper.Infrastructure/Features/Jobs/RegisterHyperRecurringJobs.cs`

### UI Definitions
- `Backend/src/AdminPanel/Hyper.AdminPanel.Domain/UiDefinitions/Promotions/PromotionActionUiDefinitions.cs`
- `Backend/src/AdminPanel/Hyper.AdminPanel.Domain/UiDefinitions/Events/EventTypeUiDefinitions.cs`
- `Backend/src/AdminPanel/Hyper.AdminPanel.Domain/UiDefinitions/Customers/CustomerTransactions/CustomerTransactionUiDefinitions.cs`

### Tests
- `Backend/tests/Hyper.Application.Tests/Features/Points/ExpirePointsJobTests.cs`
- `Backend/tests/Hyper.Domain.Tests/Features/Promotions/PromotionActionServiceTests.cs`

### Documentation
- `Backlog/BACKLOG_SCORE.md`

---

## Breaking Changes

هیچ breaking change وجود ندارد. همه تغییرات backward compatible هستند.

---

## Testing Checklist

- [x] تست‌های Unit برای ExpirePointsJob
- [x] تست‌های Unit برای SetPointLevel
- [x] تست‌های Unit برای DebitPoint با AllowNegativeBalance
- [ ] تست‌های Integration (اختیاری)
- [ ] تست‌های Manual در محیط Development

---

## Next Steps

1. اجرای Migration برای ایجاد فیلدهای جدید در دیتابیس
2. تست Manual در محیط Development
3. بررسی Performance برای ExpirePointsJob
4. مستندسازی برای کاربران نهایی (در صورت نیاز)










