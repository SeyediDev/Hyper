# Icon Registry - Hyper Platform

این فایل راهنمای استفاده از `IconRegistry` برای مدیریت متمرکز آیکن‌های پلتفرم باشگاه مشتریان می‌باشد.

## 📦 ساختار

```
Hyper.Bpms/
├── Hyper.Bpms/Infrastructure/Icons/
│   ├── IconRegistry.cs           # رجیستری مرکزی mapping آیکن‌ها
│   ├── MenuIconExtensions.cs     # Extension methods (اختیاری)
│   └── INDEX.md                  # این فایل
│
└── Hyper.Bpms.Web/wwwroot/Content/custom-icons/
    ├── custom-sprite.svg         # فایل SVG اصلی
    ├── custom-menu-icons.css       # استایل‌های رنگی
    └── README.md                 # راهنمای استفاده
```

## 🎯 هدف

`IconRegistry` یک **Single Source of Truth** برای:
- Mapping entity ها به icon های مناسب
- مدیریت color scheme یکپارچه
- جلوگیری از hard-code کردن icon name ها در سراسر کد

## 📝 نحوه استفاده

### 1. در Menu Definitions

```csharp
// فایل: Menu_Hyper.cs
AddMenu<Customer>("مشتریان", "user-circle");
AddReport<Product>("گزارش محصولات", "box-package");
```

Icon name ها **مستقیماً** در `Menu_Hyper.cs` تعریف می‌شوند.

### 2. دریافت Icon از Registry

```csharp
using Hyper.Bpms.Infrastructure.Icons;

// دریافت icon ID
var iconId = IconRegistry.GetIconId("Customer"); 
// returns: "user-circle"

var iconId = IconRegistry.GetIconId("UnknownEntity"); 
// returns: "default-icon"

// دریافت رنگ
var color = IconRegistry.Colors.GetColor("Customer");
// returns: "#10B981" (Emerald)
```

### 3. استفاده از Extension Methods (اختیاری)

```csharp
using Hyper.Bpms.Infrastructure.Icons;

var menuItem = AddMenu("مشتریان", "", "Customer Management", "");
menuItem.WithHyperIcon("Customer")           // Set icon
        .WithIconColor("Customer");         // Set color

// یا به صورت ترکیبی:
menuItem.WithHyperIconAndColor("Customer");
```

**نکته**: در حال حاضر از extension methods استفاده نمی‌کنیم و icon ها مستقیماً تعریف می‌شوند.

## 🗂️ Entity to Icon Mapping

لیست کامل mapping ها در `IconRegistry.cs`:

| Entity/Menu | Icon ID | رنگ |
|-------------|---------|-----|
| **Dashboard** | | |
| HomePageEntity | home-dashboard | #6366F1 (Indigo) |
| **Customers** | | |
| Tenant | building-organization | #10B981 (Emerald) |
| Customer | user-circle | #10B981 (Emerald) |
| CustomerSegment | users-group | #10B981 (Emerald) |
| CustomerParameter | user-settings | #10B981 (Emerald) |
| CustomerSegmentMembership | user-badge | #10B981 (Emerald) |
| CustomerParameterValue | sliders-h | #10B981 (Emerald) |
| CustomerTransaction | wallet-money | #10B981 (Emerald) |
| CustomerPointLevel | trophy-star | #10B981 (Emerald) |
| **Products** | | |
| Reward | box-package | #F59E0B (Amber) |
| RewardCost | coins-money | #F59E0B (Amber) |
| RewardAsset | cube-3d | #F59E0B (Amber) |
| RewardCategory | grid-layout | #F59E0B (Amber) |
| RewardMerchant | store-shop | #F59E0B (Amber) |
| **Scoring** | | |
| Point | star-badge | #EF4444 (Red) |
| PointLevel | medal-reward | #EF4444 (Red) |
| PointBudget | piggy-bank | #EF4444 (Red) |
| ScoringRuleTriggerCondition | flag-trigger | #EF4444 (Red) |
| ScoringRuleAction | bolt-lightning | #EF4444 (Red) |
| **Events** | | |
| EventType | calendar-event | #8B5CF6 (Purple) |
| EventTypeParameter | cog-settings | #8B5CF6 (Purple) |
| EventChannel | rss-signal | #8B5CF6 (Purple) |
| EventLog | list-checklist | #8B5CF6 (Purple) |
| **Promotions** | | |
| Promotion | gift-present | #EC4899 (Pink) |
| Lottery | ticket-lottery | #EC4899 (Pink) |
| **Settings** | | |
| User | user-admin | #64748B (Slate) |
| Faq | question-circle | #64748B (Slate) |
| Help | info-circle | #64748B (Slate) |
| Document | file-document | #64748B (Slate) |
| **Reports** | | |
| report-icon | chart-bar | #0EA5E9 (Sky) |

## 🎨 Color Scheme

```csharp
IconRegistry.Colors.Dashboard   // #6366F1 - Indigo
IconRegistry.Colors.Customer    // #10B981 - Emerald
IconRegistry.Colors.Product     // #F59E0B - Amber
IconRegistry.Colors.Scoring     // #EF4444 - Red
IconRegistry.Colors.Event       // #8B5CF6 - Purple
IconRegistry.Colors.Promotion   // #EC4899 - Pink
IconRegistry.Colors.Settings    // #64748B - Slate
IconRegistry.Colors.Report      // #0EA5E9 - Sky
IconRegistry.Colors.Default     // #6B7280 - Gray
```

## 🔄 افزودن Icon جدید

1. **افزودن SVG به sprite**:
   - فایل: `Hyper.Bpms.Web/wwwroot/Content/custom-icons/custom-sprite.svg`
   - افزودن `<symbol id="new-icon-name">...</symbol>`

2. **افزودن به Registry**:
   ```csharp
   // IconRegistry.cs
   private static readonly Dictionary<string, string> EntityIcons = new()
   {
       // ...
       ["NewEntity"] = "new-icon-name",
   };
   ```

3. **افزودن به UseHyperIcon در MenuHelper**:
   ```csharp
   // MenuHelper.cs
   private static bool UseHyperIcon(string iconName)
   {
       var HyperIcons = new HashSet<string>
       {
           // ...
           "new-icon-name"
       };
       return HyperIcons.Contains(iconName);
   }
   ```

4. **افزودن رنگ به CSS**:
   ```css
   /* custom-menu-icons.css */
   .sidemenu-icon use[href*="new-icon-name"] {
       stroke: #ColorCode;
   }
   ```

5. **بروزرسانی Documentation**:
   - این فایل (INDEX.md)
   - `README.md` در wwwroot/Content/custom-icons/

## 🧪 تست

```csharp
// Test icon resolution
Assert.Equal("user-circle", IconRegistry.GetIconId("Customer"));
Assert.Equal("default-icon", IconRegistry.GetIconId("UnknownEntity"));

// Test color resolution
Assert.Equal("#10B981", IconRegistry.Colors.GetColor("Customer"));
Assert.Equal("#6B7280", IconRegistry.Colors.GetColor("UnknownEntity"));
```

## 📐 اصول طراحی

### ✅ DO
- از naming convention سازگار استفاده کنید (kebab-case)
- رنگ‌ها را از Tailwind palette انتخاب کنید
- Documentation را به‌روز نگه دارید
- Icon را semantic و معنادار انتخاب کنید

### ❌ DON'T
- Icon name را hard-code نکنید
- رنگ‌های random استفاده نکنید
- از icon های بدون معنی استفاده نکنید
- mapping ها را در چند جا تکرار نکنید

## 🔗 وابستگی‌ها

- **Frontend**: `MenuHelper.cs` (Neo.Bpms.UI.MVC)
- **Assets**: `custom-sprite.svg`, `custom-menu-icons.css`
- **Menu**: `Menu_Hyper.cs` (Hyper.Bpms)

## 📞 پشتیبانی

برای سوالات یا مشکلات:
1. این documentation را مطالعه کنید
2. فایل `README.md` در wwwroot را چک کنید
3. کد `IconRegistry.cs` را بررسی کنید
4. تیم توسعه را مطلع کنید

---

**نسخه**: 1.0.0  
**تاریخ ایجاد**: 2025-10-14  
**آخرین بروزرسانی**: 2025-10-14









