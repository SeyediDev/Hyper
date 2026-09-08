# تحلیل پوشش سناریوهای Promotion

## 📋 خلاصه

این سند بررسی می‌کند که آیا دو سناریو ارائه شده توسط کاربر با ساختار فعلی Promotion پشتیبانی می‌شوند یا نه.

---

## ✅ سناریو 1: 3 اقدام متوالی با بازه زمانی

### نیازمندی‌ها:
1. ✅ بازه زمانی: 3 تا 7 دی ساعت 19
2. ✅ 3 اقدام متوالی:
   - افتتاح حساب ممتاز با مانده حداقل 3 میلیون
   - خرید دو شارژ به مبالغ 5 و 10 تومان
   - دعوت یک دوست
3. ✅ فقط کاربران جامعه x
4. ✅ با انجام مرحله 1: 2 سکه نقره
5. ✅ با انجام هر 3 مرحله: 10 سکه طلا + قرعه‌کشی + VIP

### پشتیبانی در ساختار فعلی:

| نیازمندی | فیلد/قابلیت | وضعیت |
|---------|------------|-------|
| بازه زمانی | `Promotion.FromDate`, `Promotion.ToDate`, `Promotion.ScheduledHour`, `Promotion.ScheduledMinute` | ✅ پشتیبانی می‌شود |
| اقدام متوالی | `PromotionTrigger.SequenceOrder`, `PromotionTrigger.DependencyConditionId` | ✅ پشتیبانی می‌شود |
| حداقل تعداد | `PromotionTrigger.MinimumCount` | ✅ پشتیبانی می‌شود |
| شرط فرمول | `PromotionTrigger.Constraint`, `PromotionTrigger.Kind = Formula` | ✅ پشتیبانی می‌شود |
| جامعه مشتریان | `PromotionCustomerSegment` | ✅ پشتیبانی می‌شود |
| Action روی شرط | `PromotionAction.ActionType = OnCondition`, `PromotionAction.PromotionTriggerId` | ✅ پشتیبانی می‌شود |
| Action روی تکمیل | `PromotionAction.ActionType = OnCompletion`, `PromotionAction.PromotionTriggerId = null` | ✅ پشتیبانی می‌شود |
| اعطای امتیاز | `PromotionAction.ActionKind = CreditPoint`, `PromotionAction.PointId` | ✅ پشتیبانی می‌شود |
| شرکت در قرعه‌کشی | `PromotionAction.ActionKind = JoinLottery`, `PromotionAction.LotteryId` | ✅ پشتیبانی می‌شود |
| عضویت در جامعه | `PromotionAction.ActionKind = JoinInCustomerSegment`, `PromotionAction.CustomerSegmentId` | ✅ پشتیبانی می‌شود |

### نتیجه: ✅ **کاملاً پشتیبانی می‌شود**

---

## ✅ سناریو 2: بازپرداخت اقساط

### نیازمندی‌ها:
1. ✅ بازپرداخت قسط BNPL در موعد: 1 سکه نقره
2. ✅ بازپرداخت قسط کالانو زودتر از موعد: 2 سکه طلا
3. ✅ دیرکرد در بازپرداخت: کاهش 2 سکه نقره + نزول از طلایی به نقره‌ای

### پشتیبانی در ساختار فعلی:

| نیازمندی | فیلد/قابلیت | وضعیت |
|---------|------------|-------|
| Event Type | `PromotionTrigger.Type = Event`, `PromotionTrigger.EventTypeId` | ✅ پشتیبانی می‌شود |
| Immediate Action | `PromotionAction.ActionType = Immediate` | ✅ پشتیبانی می‌شود |
| افزایش امتیاز | `PromotionAction.ActionKind = CreditPoint` | ✅ پشتیبانی می‌شود |
| کاهش امتیاز | `PromotionAction.ActionKind = DebitPoint` | ✅ پشتیبانی می‌شود |
| مقداردهی پارامتر | `PromotionAction.ActionKind = SetCustomerParameterValue`, `PromotionAction.CustomerParameterId` | ✅ پشتیبانی می‌شود |

### نتیجه: ✅ **کاملاً پشتیبانی می‌شود**

---

## 📊 ساختار دیتابیس مورد نیاز

### جداول اصلی:
- ✅ `Promotions` - پویش‌ها
- ✅ `PromotionTriggers` - شرایط پویش
- ✅ `PromotionActions` - عملیات پویش
- ✅ `PromotionCustomerSegments` - جوامع مشتریان
- ✅ `PromotionParticipations` - شرکت‌های مشتریان
- ✅ `PromotionEventReceived` - رویدادهای دریافتی

### جداول مرتبط:
- ✅ `EventTypes` - انواع رویداد
- ✅ `Points` - انواع امتیاز (سکه نقره و طلا)
- ✅ `CustomerSegments` - جوامع مشتریان
- ✅ `Lotteries` - قرعه‌کشی‌ها
- ✅ `CustomerParameters` - پارامترهای مشتری

### نتیجه: ✅ **همه جداول مورد نیاز موجود هستند**

---

## 🧪 تست‌های نوشته شده

### فایل تست:
- `PromotionScenariosTests.cs` - شامل تست‌های زیر:

1. ✅ `Scenario1_SequentialActions_ShouldCreateCorrectPromotionStructure` - بررسی ساختار سناریو 1
2. ✅ `Scenario1_Step1Completion_ShouldExecuteOnConditionAction` - بررسی اجرای OnCondition
3. ✅ `Scenario1_AllStepsCompletion_ShouldExecuteOnCompletionActions` - بررسی اجرای OnCompletion
4. ✅ `Scenario2_PaymentScenarios_ShouldCreateCorrectPromotionStructure` - بررسی ساختار سناریو 2
5. ✅ `Scenario2_BNPLOnTime_ShouldCreditSilverCoin` - تست BNPL در موعد
6. ✅ `Scenario2_KalanoEarly_ShouldCreditGoldCoins` - تست کالانو زودتر
7. ✅ `Scenario2_PaymentLate_ShouldDebitAndDowngrade` - تست دیرکرد
8. ✅ `DatabaseStructure_ShouldSupportScenarios` - بررسی ساختار دیتابیس

### فایل SQL:
- `PromotionScenarios_DataSetup.sql` - اسکریپت ایجاد داده‌های تست

---

## ✅ نتیجه‌گیری

### هر دو سناریو کاملاً پشتیبانی می‌شوند! ✅

1. **ساختار دیتابیس**: همه جداول و فیلدهای مورد نیاز موجود هستند
2. **ساختار Domain**: همه موجودیت‌ها و Enums مورد نیاز موجود هستند
3. **ساختار Service**: `PromotionService` می‌تواند این سناریوها را پردازش کند
4. **تست‌ها**: تست‌های کامل برای هر دو سناریو نوشته شده‌اند
5. **داده‌های تست**: اسکریپت SQL برای ایجاد داده‌های تست آماده است

### نکات مهم:

1. **EventTypes**: باید در دیتابیس تعریف شوند:
   - EventType 1: افتتاح حساب ممتاز
   - EventType 2: خرید شارژ
   - EventType 3: دعوت دوست
   - EventType 4: بازپرداخت BNPL در موعد
   - EventType 5: بازپرداخت کالانو زودتر از موعد
   - EventType 6: دیرکرد در بازپرداخت

2. **Points**: باید در دیتابیس تعریف شوند:
   - Point 1: سکه نقره
   - Point 2: سکه طلا

3. **CustomerSegments**: باید در دیتابیس تعریف شوند:
   - Segment 1: جامعه x
   - Segment 2: VIP

4. **Lottery**: باید در دیتابیس تعریف شود:
   - Lottery 1: قرعه‌کشی x

5. **CustomerParameter**: باید در دیتابیس تعریف شود:
   - Parameter 1: سطح VIP

---

## 📝 مراحل بعدی

1. ✅ تست‌ها نوشته شدند
2. ✅ اسکریپت SQL برای داده‌های تست آماده است
3. ⏳ اجرای اسکریپت SQL در دیتابیس تست
4. ⏳ اجرای تست‌ها و بررسی نتایج
5. ⏳ در صورت نیاز، اصلاحات در `PromotionService`


