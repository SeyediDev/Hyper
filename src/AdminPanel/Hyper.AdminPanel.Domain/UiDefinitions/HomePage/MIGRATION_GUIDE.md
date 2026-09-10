# 🚀 راهنمای مهاجرت داشبوردها

این راهنما برای کمک به توسعه‌دهندگان در فهم تغییرات و نحوه مهاجرت کد است.

---

## 📋 جدول تبدیل داشبوردها

این جدول نشان می‌دهد که هر داشبورد قدیمی به کدام داشبورد جدید منتقل شده است:

| داشبورد قدیمی | وضعیت | جایگزین جدید | توضیحات |
|---------------|--------|--------------|---------|
| `MainOverviewDashboard` | ❌ حذف شد | `ExecutiveDashboard` | تمام ویجت‌های اصلی منتقل شدند |
| `CustomerAnalyticsDashboard` | ❌ حذف شد | `AnalyticsDashboard` | ادغام با تحلیل محصولات |
| `MarketingCampaignsDashboard` | ❌ حذف شد | `MarketingDashboard` | ادغام با نظرسنجی‌ها |
| `FinancialOverviewDashboard` | ❌ حذف شد | `ExecutiveDashboard` | ویجت‌های مالی در داشبورد مدیریتی |
| `OperationalMetricsDashboard` | ❌ حذف شد | `OperationsDashboard` | ادغام با رویدادها و تراکنش‌ها |
| `EventLogDashboard` | ❌ حذف شد | `OperationsDashboard` | بخش رویدادها |
| `PointsTransactionsDashboard` | ❌ حذف شد | `OperationsDashboard` | بخش تراکنش‌ها و امتیازات |
| `ProductsDashboard` | ❌ حذف شد | `AnalyticsDashboard` + `OperationsDashboard` | توزیع شده |
| `SurveyAnalyticsDashboard` | ❌ حذف شد | `MarketingDashboard` | ✅ با Roles تعریف شد |

---

## 🔄 نقشه انتقال ویجت‌ها

### از MainOverviewDashboard:

```
MainOverviewDashboard                    ExecutiveDashboard
├─ CustomerCountWidget         ────────→ TotalCustomersWidget
├─ ActiveCustomersWidget       ────────→ ActiveCustomersWidget
├─ NewCustomersWidget          ────────→ NewCustomersWidget
├─ TotalRevenueWidget          ────────→ TotalRevenueWidget
├─ OverallRoiWidget            ────────→ RevenueOverallRoiWidget
├─ RfmSegmentDistributionWidget ───────→ RfmSegmentDistributionWidget
├─ CustomerGrowthByMonthWidget ────────→ CustomerGrowthByMonthWidget
├─ SegmentDistributionWidget   ────────→ SegmentDistributionWidget
└─ TopCampaignsWidget          ────────→ TopCampaignsWidget
```

### از CustomerAnalyticsDashboard:

```
CustomerAnalyticsDashboard               AnalyticsDashboard
├─ ClvDistributionWidget       ────────→ ClvDistributionWidget
├─ AverageClvByRfmWidget       ────────→ AverageClvByRfmWidget
├─ RetentionRateWidget         ────────→ RetentionRateWidget
├─ CustomersAtRiskWidget       ────────→ CustomersAtRiskWidget
├─ ChurnRiskDistributionWidget ────────→ ChurnRiskDistributionWidget
├─ EngagementDistributionWidget ───────→ [Moved to MarketingDashboard]
├─ HighlyEngagedWidget         ────────→ [Moved to MarketingDashboard]
├─ NpsSegmentationWidget       ────────→ NpsSegmentationWidget
├─ NpsDistributionWidget       ────────→ NpsDistributionWidget
├─ AverageSatisfactionWidget   ────────→ AverageSatisfactionWidget
└─ SatisfactionDistributionWidget ─────→ SatisfactionDistributionWidget
```

### از MarketingCampaignsDashboard:

```
MarketingCampaignsDashboard              MarketingDashboard
├─ ActiveCampaignsWidget       ────────→ ActiveCampaignsWidget
├─ CampaignRoiWidget           ────────→ CampaignRoiWidget
├─ MessageFunnelWidget         ────────→ MessageFunnelWidget
├─ DeliveryRateWidget          ────────→ DeliveryRateWidget
├─ ConversionRateWidget        ────────→ ConversionRateWidget
└─ CacWidget                   ────────→ CacWidget
```

### از SurveyAnalyticsDashboard:

```
SurveyAnalyticsDashboard (⚠️ بدون Roles)  MarketingDashboard (✅ با Roles)
├─ ActiveSurveysWidget         ────────→ ActiveSurveysWidget
├─ AverageParticipationWidget  ────────→ SurveyParticipationWidget
└─ TopSurveysWidget            ────────→ TopSurveysWidget
```

### از PointsTransactionsDashboard:

```
PointsTransactionsDashboard              OperationsDashboard
├─ TransactionsByMonthWidget   ────────→ TransactionsByMonthWidget
├─ TransactionsByTypeWidget    ────────→ TransactionsByTypeWidget
├─ UnvisitedTransactionsWidget ────────→ UnvisitedTransactionsWidget
├─ CustomerPointsBalanceWidget ────────→ CustomerPointsBalanceWidget
├─ PointsByTypeWidget          ────────→ PointsByTypeWidget
├─ AutoVisitPointsWidget       ────────→ AutoVisitPointsWidget
├─ TransactionsByScoringRuleWidget ────→ TransactionsByScoringRuleWidget
├─ RecentScoringRulesWidget    ────────→ RecentScoringRulesWidget
└─ ScoringRulesByTenantWidget  ────────→ ScoringRulesByTenantWidget
```

### از EventLogDashboard:

```
EventLogDashboard                        OperationsDashboard
├─ EventLogByMonthWidget       ────────→ EventLogByMonthWidget
├─ EventLogByEventTypeWidget   ────────→ EventLogByEventTypeWidget
├─ EventLogByChannelWidget     ────────→ EventLogByChannelWidget
├─ MostActiveCustomersWidget   ────────→ MostActiveCustomersWidget
├─ DailyActiveUsersWidget      ────────→ DailyActiveUsersWidget
├─ SystemActivityWidget        ────────→ SystemActivityWidget
└─ RecentEventsWidget          ────────→ RecentEventsWidget
```

### از ProductsDashboard:

```
ProductsDashboard                        توزیع شده
├─ ProductsByCategoryWidget    ────────→ AnalyticsDashboard
├─ ProductsByTypeWidget        ────────→ AnalyticsDashboard
├─ ActiveProductsWidget        ────────→ OperationsDashboard
├─ PopularProductsWidget       ────────→ AnalyticsDashboard
├─ EventLogByProductWidget     ────────→ OperationsDashboard
├─ ProductRepeatPurchaseWidget ────────→ AnalyticsDashboard
└─ ProductCLVWidget            ────────→ AnalyticsDashboard
```

---

## 🔑 تغییرات کلیدی

### 1. تغییر نام‌گذاری

برخی ویجت‌ها نام‌گذاری بهتری پیدا کردند:

```csharp
// قبل
MainOverviewDashboard.KPISectionDiv.CustomerCountWidget

// بعد
ExecutiveDashboard.KeyPerformanceIndicatorsDiv.TotalCustomersWidget
```

### 2. گروه‌بندی بهتر Div‌ها

```csharp
// قبل - پراکنده
MainOverviewDashboard
├─ KPISectionDiv
├─ FeedbackOverviewDiv (commented out)
├─ RFMAnalysisDiv
├─ CustomerGrowthDiv
└─ ...

// بعد - ساختارمند
ExecutiveDashboard
├─ KeyPerformanceIndicatorsDiv (Row)
├─ FinancialPerformanceDiv
├─ CustomerGrowthDiv
├─ RFMOverviewDiv
├─ SegmentsOverviewDiv
└─ TopCampaignsDiv
```

### 3. تعریف Roles برای همه

```csharp
// قبل - SurveyAnalyticsDashboard
protected override string Title => "تحلیل نظرسنجی‌ها";
// ⚠️ هیچ Roles تعریف نشده بود!

// بعد - در MarketingDashboard
protected override string Title => "داشبورد بازاریابی";
protected override List<string> Roles => [HyperRoles.MarketingManager, Neo.Domain.Constants.Roles.Admin];
// ✅ Roles تعریف شده
```

### 4. حذف موقت ویجت‌های مالی

- `TotalRevenueWidget` و `MarketingTotalRevenueWidget` به دلیل بلوغ ناکافی محاسبات درآمدی غیرفعال شدند.
- به جای آن‌ها `CampaignPerformanceVolumeWidget` و `MarketingPerformanceVolumeWidget` اضافه شدند تا تمرکز روی حجم عملکرد (Conversions/Delivered) باشد.
- نمودار `TransactionsOverviewWidget` نیز جایگزین نمای ROI شد تا «تراکنش ویو» مستقیماً در صفحه اول در دسترس باشد.

---

## ⚙️ نحوه استفاده از داشبوردهای جدید

### برای کاربران نهایی:

1. **مدیران (Admin, Manager):**
   - به ExecutiveDashboard دسترسی دارند (پیش‌فرض)
   - می‌توانند OperationsDashboard را هم ببینند

2. **مدیر بازاریابی (MarketingManager):**
   - MarketingDashboard را می‌بیند
   - شامل کمپین‌ها، تعامل و نظرسنجی‌ها

3. **تحلیلگر (Analyst):**
   - AnalyticsDashboard برای تحلیل‌های عمیق
   - OperationsDashboard برای نظارت عملیاتی

### برای توسعه‌دهندگان:

#### افزودن ویجت جدید:

```csharp
// در ExecutiveDashboard.cs
public partial class NewKPIDiv : DashboardDivDefinition
{
    public override string Title => "شاخص جدید";
    public override int Width => 6;

    public partial class NewKPIWidget : DashboardDivWidgetDefinition<
        MyEntityUiDefinitions,
        MyEntityUiDefinitions.PublicReport,
        MyEntityUiDefinitions.PublicReport.MyReportConfig>
    {
        public override string Title => "ویجت جدید";
        protected override int? MaxRecordCount => 10;
        public override int Width => 12;
    }
}
```

#### تنظیم Roles:

```csharp
public partial class MyNewDashboard : DashboardConfigDefinition
{
    protected override string Title => "داشبورد من";
    protected override List<string> Roles => [
        Neo.Domain.Constants.Roles.Admin,
        HyperRoles.Manager
    ];
    protected override bool IsDefault => false;
}
```

---

## 🐛 رفع مشکلات رایج

### مشکل: "ویجتی تنظیم نشده است"

**علت:** Roles تعریف نشده یا ReportConfig وجود ندارد

**راه‌حل:**
1. اطمینان از وجود Roles در DashboardConfigDefinition
2. بررسی وجود ReportConfig در فایل‌های مربوطه

```csharp
// ✅ صحیح
protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin];

// ❌ غلط
// Roles تعریف نشده
```

### مشکل: ویجت نمایش داده نمی‌شود

**علت:** Width یا MaxRecordCount نامناسب

**راه‌حل:**
```csharp
// برای KPI widgets
protected override int? MaxRecordCount => 1;
protected override int? HeightInPixels => 100;

// برای لیست‌ها
protected override int? MaxRecordCount => 15;
```

---

## 📊 چک‌لیست مهاجرت

برای اطمینان از موفقیت مهاجرت:

- ✅ همه ۹ فایل قدیمی حذف شدند
- ✅ ۴ فایل جدید ایجاد شدند
- ✅ همه ویجت‌ها به ReportConfig معتبر اشاره می‌کنند
- ✅ همه داشبوردها Roles تعریف شده دارند
- ✅ یک داشبورد به عنوان IsDefault تنظیم شده (ExecutiveDashboard)
- ✅ هیچ خطای linter وجود ندارد
- ✅ مستندات کامل نوشته شده

---

## 🎯 تست

برای تست داشبوردهای جدید:

1. **تست نقش‌ها:**
   - ورود با Admin → باید ExecutiveDashboard را ببیند
   - ورود با MarketingManager → باید MarketingDashboard را ببیند
   - ورود با Analyst → باید AnalyticsDashboard را ببیند

2. **تست ویجت‌ها:**
   - همه ویجت‌ها باید بدون خطا لود شوند
   - داده‌ها باید صحیح نمایش داده شوند
   - فیلترها و محدودیت‌ها باید کار کنند

3. **تست عملکرد:**
   - سرعت بارگذاری مناسب
   - بدون خطای "ویجتی تنظیم نشده است"
   - Responsive بودن در اندازه‌های مختلف

---

## 🔗 منابع

- [DASHBOARD_REDESIGN.md](./DASHBOARD_REDESIGN.md) - توضیحات کامل بازطراحی
- [DASHBOARD_STRUCTURE.md](./DASHBOARD_STRUCTURE.md) - ساختار دقیق داشبوردها
- [Neo.Bpms Documentation](../../../../Neo.Bpms/) - مستندات فریمورک

---

**✨ موفق باشید!**

**📅 آخرین بروزرسانی:** 1404/08/14



