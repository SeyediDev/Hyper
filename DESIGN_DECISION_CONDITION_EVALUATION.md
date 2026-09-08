# تصمیم‌گیری طراحی: سیستم ارزیابی شرط‌های PromotionTrigger

## تاریخ: 2024
## موضوع: جایگزینی LegacyFilter با سیستم شرط‌نویسی حرفه‌ای

---

## جدول مقایسه گزینه‌ها

| معیار | گزینه A: جدول PromotionTriggerCondition | گزینه B: رشته قابل خواندن + ابزار شرط‌ساز | امتیاز |
|------|------------------------------------------|--------------------------------------------|--------|
| **انعطاف‌پذیری** | ⭐⭐⭐⭐⭐<br>هر شرط به صورت مستقل قابل ویرایش | ⭐⭐⭐<br>نیاز به ویرایش کل رشته | A |
| **قابلیت نگهداری** | ⭐⭐⭐⭐⭐<br>ساختار واضح، قابل جستجو و گزارش‌گیری | ⭐⭐<br>نیاز به Parser و مدیریت پیچیده | A |
| **پشتیبانی AND/OR** | ⭐⭐⭐⭐⭐<br>با ConditionGroup به صورت طبیعی | ⭐⭐⭐<br>نیاز به ساختار در رشته | A |
| **قابلیت توسعه** | ⭐⭐⭐⭐⭐<br>افزودن شرط جدید = رکورد جدید | ⭐⭐<br>نیاز به تغییر Parser و Builder | A |
| **Performance** | ⭐⭐⭐⭐<br>Query بهینه با Index | ⭐⭐⭐⭐⭐<br>بدون Query اضافی | B |
| **قابلیت Debug** | ⭐⭐⭐⭐⭐<br>هر شرط قابل بررسی مستقل | ⭐⭐<br>نیاز به Parse برای Debug | A |
| **UI/UX** | ⭐⭐⭐⭐<br>SubTable موجود، نیاز به بهبود | ⭐⭐⭐⭐⭐<br>Builder حرفه‌ای با Visual Editor | B |
| **مستندسازی** | ⭐⭐⭐⭐⭐<br>هر شرط Title و Description دارد | ⭐⭐<br>فقط در رشته | A |
| **تست‌پذیری** | ⭐⭐⭐⭐⭐<br>تست هر شرط به صورت مستقل | ⭐⭐⭐<br>نیاز به تست Parser | A |
| **Migration** | ⭐⭐⭐⭐<br>نیاز به Migration از LegacyFilter | ⭐⭐⭐⭐⭐<br>ساده‌تر (فقط تبدیل) | B |
| **استاندارد صنعتی** | ⭐⭐⭐⭐⭐<br>مشابه Rule Engine های حرفه‌ای | ⭐⭐⭐<br>مشابه Expression Builder | A |
| **پشتیبانی از فیلدهای مشتری** | ⭐⭐⭐<br>نیاز به توسعه | ⭐⭐⭐⭐<br>در Builder قابل افزودن | B |
| **پشتیبانی از مقادیر اکوسیستم** | ⭐⭐⭐<br>نیاز به توسعه | ⭐⭐⭐⭐<br>در Builder قابل افزودن | B |
| **خوانایی** | ⭐⭐⭐⭐⭐<br>ساختار واضح در دیتابیس | ⭐⭐⭐<br>بستگی به فرمت رشته | A |
| **قابلیت تبدیل به فرم شرط‌ساز** | ⭐⭐⭐⭐<br>ساختار موجود، نیاز به UI | ⭐⭐⭐⭐⭐<br>مستقیم از Builder | B |

---

## امتیاز نهایی

- **گزینه A (جدول):** 58 امتیاز
- **گزینه B (رشته + Builder):** 44 امتیاز

---

## تصمیم نهایی: گزینه A (جدول PromotionTriggerCondition) با بهبودها

### دلایل انتخاب:

1. ✅ **ساختار موجود:** جدول PromotionTriggerCondition از قبل طراحی شده
2. ✅ **قابلیت نگهداری بالا:** هر شرط مستقل و قابل مدیریت
3. ✅ **پشتیبانی طبیعی از AND/OR:** با ConditionGroup
4. ✅ **قابلیت توسعه:** افزودن قابلیت‌های جدید بدون تغییر ساختار
5. ✅ **استاندارد صنعتی:** مشابه Rule Engine های حرفه‌ای (Drools, Rules Engine)

### بهبودهای لازم:

1. **افزودن پشتیبانی از فیلدهای مشتری:**
   - افزودن `CompareWith.CustomerField` به enum
   - افزودن `CustomerFieldName` به PromotionTriggerCondition

2. **افزودن پشتیبانی از مقادیر اکوسیستم:**
   - افزودن `CompareWith.EcosystemValue` به enum
   - افزودن `EcosystemValueId` به PromotionTriggerCondition

3. **بهبود UI Builder:**
   - ایجاد Visual Condition Builder برای ساخت شرط‌ها
   - پشتیبانی از Drag & Drop
   - Preview شرط به صورت خوانا

4. **بهبود فرمول‌نویسی:**
   - پشتیبانی از توابع ریاضی پیشرفته
   - پشتیبانی از توابع تاریخ و زمان
   - Validation و Syntax Highlighting

---

## معماری پیشنهادی

### 1. ساختار داده

```csharp
public class PromotionTriggerCondition
{
    // موجود
    public int PromotionTriggerId { get; set; }
    public string? Title { get; set; }
    public TriggerGroup ConditionGroup { get; set; } // برای AND/OR
    public PromotionTriggerFilterKind FilterKind { get; set; }
    public string? FilterConstraint { get; set; } // برای Formula
    public PromotionTriggerCompareWith? CompareWith { get; set; }
    public int? EventTypeParameterId { get; set; }
    public int? PointId { get; set; }
    public string? Value { get; set; }
    
    // پیشنهادی - افزودن
    public string? CustomerFieldName { get; set; } // برای CompareWith.CustomerField
    public int? EcosystemValueId { get; set; } // برای CompareWith.EcosystemValue
    public int Order { get; set; }
    public string? Description { get; set; }
}
```

### 2. سرویس ارزیابی شرط

```csharp
public interface IPromotionTriggerConditionEvaluator
{
    /// <summary>
    /// ارزیابی تمام شرط‌های یک Trigger با پشتیبانی از AND/OR
    /// </summary>
    Task<bool> EvaluateConditionsAsync(
        int promotionTriggerId,
        PromotionProcessingRequest request,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// ارزیابی یک شرط واحد
    /// </summary>
    Task<bool> EvaluateConditionAsync(
        PromotionTriggerCondition condition,
        PromotionProcessingRequest request,
        CancellationToken cancellationToken = default);
}
```

### 3. منطق AND/OR

```
شرایط با ConditionGroup یکسان → AND (همه باید true باشند)
گروه‌های مختلف → OR (حداقل یک گروه باید true باشد)

مثال:
- Group1: (Condition1 AND Condition2)
- Group2: (Condition3)
- نتیجه: (Group1) OR (Group2)
  = (Condition1 AND Condition2) OR (Condition3)
```

### 4. پشتیبانی از انواع مقایسه

| CompareWith | استفاده | مثال |
|-------------|---------|------|
| Parameter | پارامتر رویداد | EventTypeParameterId → Amount |
| Point | موجودی امتیاز | PointId → Balance |
| CustomerField | فیلد مشتری | CustomerFieldName → "Age" |
| EcosystemValue | مقدار اکوسیستم | EcosystemValueId → Config Value |
| Constant | مقدار ثابت | Value → "1000" |

---

## مراحل پیاده‌سازی

### Phase 1: توسعه ساختار داده
- [ ] افزودن CustomerFieldName به PromotionTriggerCondition
- [ ] افزودن EcosystemValueId به PromotionTriggerCondition
- [ ] افزودن CompareWith.CustomerField به enum
- [ ] افزودن CompareWith.EcosystemValue به enum

### Phase 2: پیاده‌سازی سرویس ارزیابی
- [ ] ایجاد IPromotionTriggerConditionEvaluator
- [ ] پیاده‌سازی منطق AND/OR با ConditionGroup
- [ ] پشتیبانی از تمام انواع CompareWith
- [ ] پشتیبانی از Formula با NCalc

### Phase 3: جایگزینی LegacyFilter
- [ ] حذف استفاده از LegacyFilter در PromotionService
- [ ] استفاده از PromotionTriggerConditionEvaluator
- [ ] Migration داده‌های LegacyFilter به PromotionTriggerCondition

### Phase 4: تست‌ها
- [ ] Unit Tests برای هر نوع شرط
- [ ] Integration Tests برای AND/OR منطق
- [ ] Performance Tests
- [ ] Scenario Tests (سناریوهای گفته شده)

### Phase 5: UI Builder (اختیاری - برای آینده)
- [ ] Visual Condition Builder
- [ ] Drag & Drop Interface
- [ ] Preview و Validation

---

## سناریوهای تست

### سناریو 1: شرط ساده با پارامتر رویداد
```
Condition: Amount >= 1000000
CompareWith: Parameter
EventTypeParameter: Amount
FilterKind: GreaterThanOrEqualTo
Value: "1000000"
```

### سناریو 2: شرط با AND
```
Group1:
  - Condition1: AccountType == "Premium"
  - Condition2: Balance >= 3000000
نتیجه: (AccountType == "Premium") AND (Balance >= 3000000)
```

### سناریو 3: شرط با OR
```
Group1: AccountType == "Premium"
Group2: AccountType == "Gold"
نتیجه: (AccountType == "Premium") OR (AccountType == "Gold")
```

### سناریو 4: شرط پیچیده با AND و OR
```
Group1: (AccountType == "Premium") AND (Balance >= 3000000)
Group2: (AccountType == "Gold")
نتیجه: ((AccountType == "Premium") AND (Balance >= 3000000)) OR (AccountType == "Gold")
```

### سناریو 5: شرط با فیلد مشتری
```
Condition: Customer.Age >= 25
CompareWith: CustomerField
CustomerFieldName: "Age"
FilterKind: GreaterThanOrEqualTo
Value: "25"
```

### سناریو 6: شرط با مقدار اکوسیستم
```
Condition: Amount >= Config.MinimumPurchaseAmount
CompareWith: EcosystemValue
EcosystemValueId: 123
FilterKind: GreaterThanOrEqualTo
```

### سناریو 7: شرط با فرمول
```
Condition: (Amount * 0.1) + Bonus >= 100000
FilterKind: Formula
FilterConstraint: "(Amount * 0.1) + Bonus >= 100000"
```

---

## نتیجه‌گیری

**گزینه A (جدول PromotionTriggerCondition)** انتخاب می‌شود زیرا:
- ✅ ساختار موجود و آماده
- ✅ قابلیت نگهداری و توسعه بالا
- ✅ پشتیبانی طبیعی از AND/OR
- ✅ استاندارد صنعتی
- ✅ قابلیت تست و Debug بالا

**بهبودهای لازم:**
- افزودن پشتیبانی از CustomerField و EcosystemValue
- بهبود UI برای ساخت شرط‌ها
- ایجاد Visual Builder (برای آینده)


