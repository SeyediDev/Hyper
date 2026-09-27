# 📊 ساختار داشبوردهای HomePage (نسخه ۳.۰)

این سند نقشه‌ی کامل داشبورد اصلی و داشبوردهای تخصصی را ارائه می‌کند. تمامی تب‌ها با حداکثر ۸ ویجت و بدون استفاده از `DashboardDivDefinition` طراحی شده‌اند تا چینش‌ها پایدار باقی بمانند.

---

## 🏠 HomePageDashboard (داشبورد کلان)

| تب | نقش‌ها | توضیح |
|----|--------|-------|
| نمای کلی مدیریت | Admin, Manager | KPI کارت‌ها، روند درآمد، رشد مشتری، نمای کلی RFM |
| سفر مشتری | Admin, Manager, Analyst | CLV، RFM، نگهداشت، ریزش، مشتریان در معرض خطر |
| وفاداری و ارزش | Admin, Manager, Analyst | امتیازات، تراکنش‌های بازدید نشده، ارزش محصولات |
| بازاریابی و تعامل | Admin, MarketingManager, Analyst | کمپین‌ها، CAC، پیام‌ها، نظرسنجی‌های فعال |
| سلامت عملیات و سامانه | Admin, Manager, Analyst | تراکنش‌ها، رویدادها، سلامت سیستم، DAU |

### جزئیات تب‌ها و ویجت‌ها

- **نمای کلی مدیریت**
  - `TotalCustomersWidget` (3) – KPI
  - `ActiveCustomersWidget` (3) – KPI
  - `NewCustomersWidget` (3) – KPI
  - `CampaignPerformanceVolumeWidget` (3) – CampaignPerformanceVolumeKpiConfig
  - `TransactionsOverviewWidget` (8) – TransactionsByMonthConfig
  - `CustomerGrowthByMonthWidget` (4) – CustomerGrowthByMonthConfig
  - `RfmSegmentOverviewWidget` (12) – RfmSegmentDistributionConfig

- **سفر مشتری**
  - `JourneyTotalCustomersWidget` (3) – KPI
  - `JourneyActiveCustomersWidget` (3) – KPI
  - `JourneyNewCustomersWidget` (3) – KPI
  - `JourneyAverageSatisfactionWidget` (3) – KPI
  - `ClvDistributionWidget` (6)
  - `AverageClvByRfmWidget` (6)
  - `RetentionRateWidget` (6)
  - `CustomersAtRiskWidget` (6)

- **وفاداری و ارزش**
  - `LoyaltyPointsIssuedWidget` (3) – KPI
  - `LoyaltyPointsRedeemedWidget` (3) – KPI
  - `LoyaltyTotalPointsBalanceWidget` (3) – KPI
  - `LoyaltyAverageTransactionValueWidget` (3) – KPI
  - `PointsByTypeWidget` (6)
  - `CustomerPointsBalanceWidget` (6)
  - `PopularProductsWidget` (6)
  - `ProductRepeatPurchaseWidget` (6)

- **بازاریابی و تعامل**
  - `MarketingPerformanceVolumeWidget` (3) – KPI
  - `MarketingTotalMarketingCostsWidget` (3) – KPI
  - `MarketingActiveCampaignsKpiWidget` (3) – KPI
  - `MarketingAverageTransactionValueWidget` (3) – KPI
  - `CampaignPerformanceVolumeTrendWidget` (6)
  - `ConversionRateWidget` (6)
  - `MessageFunnelWidget` (6)
  - `EngagementDistributionWidget` (6)

- **سلامت عملیات و سامانه**
  - `OperationsPointsIssuedWidget` (3) – KPI
  - `OperationsPointsRedeemedWidget` (3) – KPI
  - `OperationsAverageTransactionWidget` (3) – KPI
  - `OperationsTotalEventsWidget` (3) – KPI
  - `TransactionsByMonthWidget` (8)
  - `TransactionsByTypeWidget` (4)
  - `EventLogByEventTypeWidget` (6)
  - `DailyActiveUsersWidget` (6)

---

## 👥 CustomerIntelligenceDashboard

| تب | نقش‌ها | توضیح |
|----|--------|-------|
| ارزش و بخش‌بندی | Admin, Manager, Analyst | CLV، RFM، محصولات ارزش‌آفرین |
| حفظ و ریزش | Admin, Manager, Analyst | نگهداشت، ریزش، امتیازات و تعامل |
| تجربه و بازخورد | Admin, Manager, Analyst | NPS، رضایت، نظرسنجی‌ها |
| توزیع جهانی | Admin, Manager, Analyst | نقشه جهان، توزیع کشوری، درخت‌نقشه کشورها |

هر تب شامل ۷ تا ۸ ویجت است (جزئیات در `CustomerIntelligenceDashboard.cs`). همه‌ی ویجت‌ها بر پایه‌ی گزارش‌های موجود در CustomerTenant، CustomerSegment، CustomerTransaction، Product، Point و Survey ساخته شده‌اند.

---

## 📈 MarketingPerformanceDashboard

| تب | نقش‌ها | توضیح |
|----|--------|-------|
| عملکرد کمپین‌ها | Admin, MarketingManager, Analyst | کمپین‌های فعال، ROI، CAC، Top Campaigns |
| تعامل و کانال‌ها | Admin, MarketingManager, Analyst | پیام‌ها، تعامل مشتریان، کانال‌های پرکاربر |
| بازخورد و تجربه | Admin, MarketingManager, Analyst | نظرسنجی‌ها، NPS، رضایت |

ویجت‌های هر تب مستقیماً بر اساس گزارش‌های Promotion، CustomerTenant، EventLog و Survey تنظیم شده‌اند.

---

## ⚙️ OperationsCommandDashboard

| تب | نقش‌ها | توضیح |
|----|--------|-------|
| تراکنش‌ها و امتیازات | Admin, Manager, Analyst | روند تراکنش، امتیازات، تراکنش‌های بازدید نشده |
| قوانین و حاکمیت | Admin, Manager, Analyst | قوانین فعال، قوانین تازه، فعالیت محصولات |
| سلامت سامانه | Admin, Manager, Analyst | رویدادها، کانال‌ها، کاربران فعال، رویدادهای اخیر |

این داشبورد گزارش‌های CustomerTransaction، Point، ScoringRule، EventLog و Product را مصرف می‌کند.

---

## ✅ نکات اجرایی

- **حداکثر ویجت در تب:** ۸ عدد برای جلوگیری از اسکرول عمودی طولانی
- **چیدمان کارت‌ها:** عرض‌های 3، 4، 6، 8 و 12 بر اساس شبکه‌ی ۱۲ ستونه
- **کارت‌های KPI:** `HeightInPixels` در حدود 110–200 تنظیم شده تا چیدمان کارت‌ها ثابت باشد
- **گزارش‌های مورد نیاز:** در `DataSources()` هر داشبورد به صورت صریح اضافه شده است
- **نام تب‌ها:** در عناوین اجباری به‌کارگیری لفظ «داشبورد» نیست؛ عنوان‌ها بر مبنای هدف تب انتخاب شده‌اند

---

## 🧪 تست و داده

برای نمایش مناسب:
- تراکنش و کمپین: حداقل ۱۲ ماه داده
- CLV/RFM: داده برای ۵ هزار مشتری به بالا
- نظرسنجی: حداقل ۳ نظرسنجی فعال با پاسخ
- رویدادها: داده روزانه برای ۳۰ روز اخیر

در صورت نیاز به اسکریپت بارگذاری داده، تیم متادیتا را مطلع کنید.

---

## 📌 یادآوری

- از `DashboardDivDefinition` استفاده نشده؛ تنها `DashboardDivWidgetDefinition` به کار گرفته شده است.
- نقش‌ها برای هر تب/داشبورد مجزا تعریف شده تا دسترسی‌ها شفاف باشد.
- در صورت افزودن ویجت جدید، حتماً حداکثر رکورد و ارتفاع را تعیین کنید تا تعادل صفحه حفظ شود.


