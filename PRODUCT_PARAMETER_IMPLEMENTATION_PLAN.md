# برنامه پیاده‌سازی صفات محصول (Product Attributes)

## 📋 خلاصه
پیاده‌سازی سیستم صفات محصول مشابه پارامترهای رویداد (EventTypeParameter) برای امکان استفاده از `Product.*` در فرمول‌های شرط و مقدار.

---

## 🎯 ساختار مشابه Event Parameters

### Event Parameters (موجود):
1. **EventTypeParameter** - تعریف پارامترهای رویداد
2. **EventLogParameter** - ذخیره مقادیر پارامترها در هر رویداد
3. **EventService** - مدیریت پارامترها
4. **FormulaContextProvider** - استفاده از `Event.*` در فرمول‌ها

### Product Attributes (باید ایجاد شود):
1. **ProductAttribute** - تعریف صفات محصول
2. **ProductAttributeValue** - ذخیره مقادیر صفات (در EventLog یا جدول جداگانه)
3. **ProductService** - مدیریت صفات
4. **FormulaContextProvider** - استفاده از `Product.*` در فرمول‌ها

---

## 📝 لیست کامل کارها

### 1. Domain Layer - Entities

#### 1.1. ProductAttribute Entity
**فایل:** `Backend/src/Core/Hyper.Domain/Entities/Products/ProductAttribute.cs`

**فیلدها (مشابه EventTypeParameter):**
- `Id` (int, PK)
- `ProductId` (int, FK → Product)
- `Product` (Product, Navigation)
- `Key` (string, MaxLength(40), Required, Unique per Product)
- `Title` (string, MaxLength(41), Required)
- `ParameterType` (ParameterType enum)
- `IsOptional` (bool?)
- `CreatedBySystem` (bool?)
- `CustomerParameterId` (int?, FK → CustomerParameter)
- `CustomerParameter` (CustomerParameter?, Navigation)
- SBVR کامل

**تغییرات در Product:**
- اضافه کردن `ProductAttributes` collection (ICollection<ProductAttribute>)

#### 1.2. ProductAttributeValue Entity
**فایل:** `Backend/src/Core/Hyper.Domain/Entities/Products/ProductAttributeValue.cs`

**گزینه 1: ذخیره در EventLog (مشابه EventLogParameter)**
- `Id` (long, PK)
- `EventLogId` (long, FK → EventLog)
- `EventLog` (EventLog, Navigation)
- `AttributeId` (int, FK → ProductAttribute)
- `Attribute` (ProductAttribute, Navigation)
- `Value` (string, MaxLength(512), Required)

**گزینه 2: جدول جداگانه (اگر نیاز به ذخیره مستقل باشد)**
- `Id` (long, PK)
- `ProductId` (int, FK → Product)
- `Product` (Product, Navigation)
- `AttributeId` (int, FK → ProductAttribute)
- `Attribute` (ProductAttribute, Navigation)
- `Value` (string, MaxLength(512), Required)
- `EventLogId` (long?, FK → EventLog, Optional)
- `CreatedAt` (DateTime)

**تصمیم:** گزینه 1 (مشابه EventLogParameter) - ذخیره در EventLog

---

### 2. Domain Layer - Services

#### 2.1. IProductService - اضافه کردن متد
**فایل:** `Backend/src/Core/Hyper.Domain/Features/ProductService.cs`

**متد جدید:**
```csharp
Task<ProductAttribute?> GetAttributeAsync(int productId, string attributeKey, bool createIfNotExists, CancellationToken cancellationToken);
```

**تغییرات در PurchaseProductOrService:**
- دریافت `Dictionary<string, string>? Attributes` در `PurchaseProductOrServiceRequest`
- ذخیره صفات محصول در `ProductAttributeValue` (یا `EventLogParameter` با نوع خاص)

#### 2.2. FormulaContextProvider - اضافه کردن Product Context
**فایل:** `Backend/src/Core/Hyper.Domain/Features/Promotions/FormulaContextProvider.cs`

**متد جدید:**
```csharp
private async Task<Dictionary<string, object>> BuildProductContextAsync(
    PromotionProcessingRequest request,
    CancellationToken cancellationToken)
```

**منطق:**
- اگر `request.ProductId` وجود داشته باشد
- دریافت صفات محصول از EventLog (از طریق EventLogId)
- تبدیل به Dictionary با کلید `Product.*`
- اضافه کردن به Context

**تغییرات در BuildContextAsync:**
- اضافه کردن `context["Product"] = await BuildProductContextAsync(...)`

**تغییرات در AdvancedFormulaEvaluator:**
- تبدیل `Product.*` به `Product_*` در ConvertToNCalcFormula
- اضافه کردن Product Context به Expression Parameters

---

### 3. Infrastructure Layer

#### 3.1. Repository Registration
**فایل:** `Backend/src/Core/Hyper.Infrastructure/...`

- ثبت `IQueryRepository<ProductAttribute, int>`
- ثبت `ICommandRepository<ProductAttribute, int>`
- ثبت `ICommandRepository<ProductAttributeValue, long>` (یا استفاده از EventLogParameter)

#### 3.2. Database Migration
- ایجاد جدول `ProductAttribute`
- ایجاد جدول `ProductAttributeValue` (یا استفاده از EventLogParameter با فیلتر)
- اضافه کردن Foreign Key به Product
- اضافه کردن Index برای ProductId + Key

---

### 4. UI Layer - Admin Panel

#### 4.1. ProductAttributeUiDefinitions
**فایل:** `Backend/src/AdminPanel/Hyper.AdminPanel.Domain/UiDefinitions/Products/ProductAttributeUiDefinitions.cs`

**مشابه EventTypeParameterUiDefinitions:**
- SubCRUDDefinition<ProductAttribute>
- SubjectId = "Sub"
- فیلدها: Title, Key, ParameterType, IsOptional, CreatedBySystem, CustomerParameter

#### 4.2. ProductUiDefinitions - اضافه کردن SubTable
**فایل:** `Backend/src/AdminPanel/Hyper.AdminPanel.Domain/UiDefinitions/Products/ProductUiDefinitions.cs`

**تغییرات:**
- اضافه کردن SubTable برای ProductAttribute (مشابه EventTypeParameter در EventTypeUiDefinitions)

#### 4.3. Menu - اضافه کردن به منو
**فایل:** `Backend/src/AdminPanel/Hyper.AdminPanel.Domain/Menu/Menu_Hyper.cs`

- اضافه کردن منوی "صفات محصول" (اگر نیاز باشد)

---

### 5. Application Layer - ProductService

#### 5.1. PurchaseProductOrServiceRequest
**تغییرات:**
```csharp
public record PurchaseProductOrServiceRequest
{
    public string CustomerId { get; set; } = null!;
    public int ProductId { get; set; }
    public int Quantity { get; set; } = 1;
    public Dictionary<string, string>? Attributes { get; set; } // ⭐ جدید
}
```

#### 5.2. LogProductAttributes
**متد جدید (مشابه LogEventParameters):**
```csharp
private async Task<List<ProductAttributeValue>> LogProductAttributes(
    PurchaseProductOrServiceRequest request,
    long eventLogId,
    CancellationToken cancellationToken)
```

**منطق:**
- دریافت صفات تعریف شده برای Product
- ایجاد ProductAttributeValue برای هر صفت
- ذخیره در دیتابیس
- اگر CustomerParameterId وجود داشت، ذخیره در CustomerParameterValue

---

### 6. Formula Context - استفاده در فرمول‌ها

#### 6.1. BuildProductContextAsync
**منطق:**
1. دریافت EventLog از request.EventLogId
2. بررسی اینکه آیا EventLog مربوط به PurchaseProduct است
3. دریافت ProductAttributeValue های مربوط به این EventLog
4. تبدیل به Dictionary با کلید Product.*
5. تبدیل نوع داده بر اساس ParameterType

**مثال استفاده:**
```csharp
// در فرمول شرط
"Product.Price > 1000000"
"Product.Quantity >= 2"
"Product.Discount > 0.1"

// در فرمول مقدار
"Product.Price * Product.Quantity"
"Product.Price * (1 - Product.Discount)"
```

---

### 7. Simulator/Test Tools

#### 7.1. بررسی شبیه‌سازها
**فایل‌های احتمالی:**
- `Backend/src/AdminPanel/Hyper.AdminPanel.Web/Controllers/SimulatorController.cs`
- `Backend/src/AdminPanel/Hyper.AdminPanel.Web/Views/Simulator/...`
- تست‌های مربوط به شبیه‌سازی

**تغییرات:**
- اضافه کردن فیلد Attributes به فرم شبیه‌سازی خرید محصول
- امکان وارد کردن صفات محصول در شبیه‌ساز
- نمایش صفات محصول در نتایج شبیه‌سازی

---

### 8. Channel API (اگر نیاز باشد)

#### 8.1. ProductService API
**فایل:** `Backend/src/Channel/Hyper.Channel.Api/Controllers/ProductsController.cs`

**تغییرات:**
- اضافه کردن Attributes به Request Body
- ارسال Attributes به ProductService

---

## 🔍 نکات مهم

### 1. ذخیره‌سازی صفات
**سوال:** آیا ProductAttributeValue باید جدول جداگانه باشد یا از EventLogParameter استفاده کنیم؟

**پاسخ:** 
- اگر ProductAttributeValue جدول جداگانه باشد: انعطاف‌پذیری بیشتر، اما پیچیدگی بیشتر
- اگر از EventLogParameter استفاده کنیم: ساده‌تر، اما نیاز به فیلتر بر اساس ProductId

**پیشنهاد:** جدول جداگانه `ProductAttributeValue` با `EventLogId` برای ارتباط با EventLog

### 2. استفاده در فرمول‌ها
**مثال‌ها:**
```csharp
// شرط
"Product.Price > 1000000 && Product.Quantity >= 2"
"Product.Discount > 0.1 || Product.IsPremium == true"

// مقدار
"Product.Price * Product.Quantity"
"Product.Price * (1 - Product.Discount)"
```

### 3. CustomerParameter Mapping
- مشابه EventTypeParameter، ProductAttribute می‌تواند به CustomerParameter مپ شود
- هنگام ذخیره ProductAttributeValue، اگر CustomerParameterId وجود داشت، در CustomerParameterValue هم ذخیره شود

---

## 📊 فایل‌های ایجاد/تغییر

### فایل‌های جدید:
1. ✅ `ProductAttribute.cs` - موجودیت تعریف صفات محصول
2. ✅ `ProductAttributeValue.cs` - موجودیت ذخیره مقادیر صفات
3. ✅ `ProductAttributeUiDefinitions.cs` - UI Definitions برای صفات محصول

### فایل‌های تغییر یافته:
1. ✅ `Product.cs` - اضافه کردن `ProductAttributes` collection
2. ✅ `ProductService.cs` - اضافه کردن `GetAttributeAsync` و `LogProductAttributes`
3. ✅ `PurchaseProductOrServiceRequest.cs` - اضافه کردن `Attributes` field
4. ✅ `FormulaContextProvider.cs` - اضافه کردن `BuildProductContextAsync`
5. ✅ `AdvancedFormulaEvaluator.cs` - پشتیبانی از `Product.*` در ConvertToNCalcFormula
6. ✅ `ProductUiDefinitions.cs` - اضافه کردن SubTable برای ProductAttribute
7. ✅ `Menu_Hyper.cs` - اضافه کردن منوی "صفات محصول" (اختیاری)
8. ✅ `DependencyInjection.cs` - ثبت Repository های جدید
9. ✅ `PromotionProcessingRequest.cs` - بررسی نیاز به ProductId (احتمالاً موجود است)
10. ✅ `EventService.cs` - بررسی نیاز به تغییر (احتمالاً نیاز نیست)
11. ✅ Channel API Controllers - اضافه کردن Attributes به Request
12. ✅ Simulator Views/Controllers - اضافه کردن فیلد Attributes

---

## ✅ چک‌لیست نهایی

- [ ] ProductAttribute entity ایجاد شد
- [ ] ProductAttributeValue entity ایجاد شد
- [ ] Product.ProductAttributes collection اضافه شد
- [ ] ProductAttributeUiDefinitions ایجاد شد
- [ ] ProductUiDefinitions به‌روزرسانی شد (SubTable)
- [ ] ProductService.GetAttributeAsync اضافه شد
- [ ] ProductService.LogProductAttributes اضافه شد
- [ ] PurchaseProductOrServiceRequest.Attributes اضافه شد
- [x] FormulaContextProvider.BuildProductContextAsync اضافه شد
- [x] AdvancedFormulaEvaluator از Product.* پشتیبانی می‌کند
- [x] Repository ها ثبت شدند (از طریق AddNeoDomainServices به صورت خودکار)
- [ ] Migration ایجاد شد
- [x] Menu به‌روزرسانی شد (ProductAttribute و TenantProductAttribute در منو هستند)
- [ ] Simulator به‌روزرسانی شد (Simulator پیدا نشد - احتمالاً در سیستم وجود ندارد یا نیاز به بررسی بیشتر)
- [x] تست‌ها نوشته شدند (FormulaContextProviderTests و AdvancedFormulaEvaluatorTests)
- [x] SBVR کامل اضافه شد (همه entity ها SBVR دارند)
- [x] Build موفق است (تست‌ها compile می‌شوند)

---

## 🎯 مثال استفاده نهایی

```csharp
// در PromotionTrigger.Condition
"Product.Price > 1000000 && Product.Quantity >= 2"

// در PromotionAction.AmountFormula
"Product.Price * Product.Quantity * 0.1" // 10% از کل مبلغ
```

---

**آماده برای شروع پیاده‌سازی!** 🚀


