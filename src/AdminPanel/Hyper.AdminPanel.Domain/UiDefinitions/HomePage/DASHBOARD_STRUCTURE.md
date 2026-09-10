# 📋 ساختار داشبوردهای HomePage

این فایل ساختار کامل و نقشه هر یک از ۴ داشبورد جدید را نشان می‌دهد.

---

## 📈 ExecutiveDashboard (داشبورد مدیریتی)
**👥 Roles:** Admin, Manager | **⭐ Default:** YES

```
┌─────────────────────────────────────────────────────────────┐
│ ExecutiveDashboard (داشبورد مدیریتی)                       │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓  │
│  ┃ 🎯 شاخص‌های کلیدی عملکرد (KPI) - Row Width: 12    ┃  │
│  ┣━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┫  │
│  ┃ [کل مشتریان]  [فعال]  [جدید]  [درآمد کل]          ┃  │
│  ┃   Width: 3      W:3     W:3       W:3               ┃  │
│  ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛  │
│                                                             │
│  ┌─────────────────────────┐  ┌───────────────────────┐   │
│  │ 💰 عملکرد مالی          │  │ 📊 رشد مشتریان       │   │
│  │ Width: 6                │  │ Width: 6              │   │
│  ├─────────────────────────┤  ├───────────────────────┤   │
│  │ • ROI کلی              │  │ • روند رشد ماهانه    │   │
│  │ • روند درآمد           │  │                       │   │
│  └─────────────────────────┘  └───────────────────────┘   │
│                                                             │
│  ┌─────────────────────────────────┐  ┌─────────────────┐  │
│  │ 🎨 تحلیل RFM مشتریان           │  │ 👥 جوامع        │  │
│  │ Width: 8                        │  │ Width: 4        │  │
│  ├─────────────────────────────────┤  ├─────────────────┤  │
│  │ • توزیع دسته‌بندی RFM           │  │ • توزیع جوامع   │  │
│  └─────────────────────────────────┘  └─────────────────┘  │
│                                                             │
│  ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓  │
│  ┃ 🏆 کمپین‌های برتر - Width: 12                       ┃  │
│  ┣━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┫  │
│  ┃ • عملکرد کمپین‌های برتر (Top 10)                   ┃  │
│  ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛  │
└─────────────────────────────────────────────────────────────┘
```

### ویجت‌ها (10 ویجت):
1. TotalCustomersWidget → CustomerCountConfig
2. ActiveCustomersWidget → ActiveCustomersConfig
3. NewCustomersWidget → NewCustomersConfig
4. TotalRevenueWidget → TotalRevenueConfig
5. RevenueOverallRoiWidget → OverallRoiConfig
6. RevenueTrendWidget → CampaignPerformanceTrendConfig
7. CustomerGrowthByMonthWidget → CustomerGrowthByMonthConfig
8. RfmSegmentDistributionWidget → RfmSegmentDistributionConfig
9. SegmentDistributionWidget → SegmentDistributionConfig
10. TopCampaignsWidget → CampaignPerformanceOverviewConfig

---

## 📢 MarketingDashboard (داشبورد بازاریابی)
**👥 Roles:** MarketingManager, Admin | **⭐ Default:** NO

```
┌─────────────────────────────────────────────────────────────┐
│ MarketingDashboard (داشبورد بازاریابی)                     │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓  │
│  ┃ 📣 کمپین‌های فعال - Width: 12                       ┃  │
│  ┣━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┫  │
│  ┃ • لیست کمپین‌های فعال (15 رکورد)                   ┃  │
│  ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛  │
│                                                             │
│  ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓  │
│  ┃ 💹 عملکرد و بازدهی - Row Width: 12                ┃  │
│  ┣━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┫  │
│  ┃ [تحلیل ROI کمپین‌ها]    [نرخ تبدیل]                ┃  │
│  ┃      Width: 6                Width: 6                ┃  │
│  ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛  │
│                                                             │
│  ┌─────────────────────────┐  ┌───────────────────────┐   │
│  │ ✉️ اثربخشی پیام‌ها      │  │ 🎯 جذب مشتری         │   │
│  │ Width: 6                │  │ Width: 6              │   │
│  ├─────────────────────────┤  ├───────────────────────┤   │
│  │ • فانل تعامل           │  │ • هزینه جذب (CAC)    │   │
│  │ • نرخ تحویل پیام‌ها     │  │                       │   │
│  └─────────────────────────┘  └───────────────────────┘   │
│                                                             │
│  ┌─────────────────────────┐  ┌───────────────────────┐   │
│  │ 🤝 تعامل مشتریان        │  │ 📋 نظرسنجی‌ها        │   │
│  │ Width: 6                │  │ Width: 6              │   │
│  ├─────────────────────────┤  ├───────────────────────┤   │
│  │ • توزیع نمره تعامل     │  │ • فعال (5 رکورد)     │   │
│  │ • با تعامل بالا        │  │ • میانگین مشارکت     │   │
│  │                         │  │ • محبوب (10 رکورد)   │   │
│  └─────────────────────────┘  └───────────────────────┘   │
│                                                             │
│  ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓  │
│  ┃ 😊 رضایت مشتری - Row Width: 12                     ┃  │
│  ┣━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┫  │
│  ┃ [NPS]  [میانگین رضایت]  [توزیع رضایت]              ┃  │
│  ┃  W:6        W:3              W:3                     ┃  │
│  ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛  │
└─────────────────────────────────────────────────────────────┘
```

### ویجت‌ها (13 ویجت):
1. ActiveCampaignsWidget → ActiveCampaignsConfig
2. CampaignRoiWidget → CampaignRoiAnalysisConfig
3. ConversionRateWidget → ConversionRateAnalysisConfig
4. MessageFunnelWidget → MessageEngagementFunnelConfig
5. DeliveryRateWidget → MessageDeliveryRateConfig
6. CacWidget → CustomerAcquisitionCostConfig
7. EngagementDistributionWidget → EngagementScoreDistributionConfig
8. HighlyEngagedWidget → HighlyEngagedCustomersConfig
9. ActiveSurveysWidget → ActiveSurveysConfig ✅
10. SurveyParticipationWidget → AverageParticipationConfig ✅
11. TopSurveysWidget → TopSurveysConfig ✅
12. NpsSegmentationWidget → NpsSegmentationConfig
13. AverageSatisfactionWidget → AverageSatisfactionScoreConfig
14. SatisfactionTrendWidget → SatisfactionScoreDistributionConfig

**✅ رفع مشکل:** نظرسنجی‌ها حالا با Roles مناسب تعریف شدند

---

## 🔍 AnalyticsDashboard (داشبورد تحلیلی)
**👥 Roles:** Analyst, Admin | **⭐ Default:** NO

```
┌─────────────────────────────────────────────────────────────┐
│ AnalyticsDashboard (داشبورد تحلیلی)                        │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓  │
│  ┃ 💎 تحلیل CLV - Row Width: 12                        ┃  │
│  ┣━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┫  │
│  ┃ [توزیع CLV]    [میانگین CLV به تفکیک RFM]          ┃  │
│  ┃   Width: 6            Width: 6                       ┃  │
│  ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛  │
│                                                             │
│  ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓  │
│  ┃ 🎨 تحلیل پیشرفته RFM - Width: 12                   ┃  │
│  ┣━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┫  │
│  ┃ • توزیع دسته‌بندی RFM                               ┃  │
│  ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛  │
│                                                             │
│  ┌─────────────────────────┐  ┌───────────────────────┐   │
│  │ 🔄 نرخ حفظ مشتری        │  │ ⚠️ تحلیل ریزش        │   │
│  │ Width: 6                │  │ Width: 6              │   │
│  ├─────────────────────────┤  ├───────────────────────┤   │
│  │ • نرخ حفظ ماهانه       │  │ • در معرض خطر        │   │
│  │   (12 ماه)              │  │ • توزیع احتمال ریزش  │   │
│  └─────────────────────────┘  └───────────────────────┘   │
│                                                             │
│  ┌─────────────────────────┐  ┌───────────────────────┐   │
│  │ 📊 تحلیل NPS            │  │ 😊 رضایت مشتری       │   │
│  │ Width: 6                │  │ Width: 6              │   │
│  ├─────────────────────────┤  ├───────────────────────┤   │
│  │ • دسته‌بندی NPS         │  │ • میانگین رضایت      │   │
│  │ • توزیع نمره            │  │ • توزیع نمره         │   │
│  └─────────────────────────┘  └───────────────────────┘   │
│                                                             │
│  ┌─────────────────────────┐  ┌───────────────────────┐   │
│  │ 📦 تحلیل محصولات        │  │ 🔁 تحلیل وفاداری     │   │
│  │ Width: 6                │  │ Width: 6              │   │
│  ├─────────────────────────┤  ├───────────────────────┤   │
│  │ • محبوب‌ترین (15)       │  │ • نرخ تکرار خرید     │   │
│  │                         │  │ • محصولات CLV         │   │
│  └─────────────────────────┘  └───────────────────────┘   │
│                                                             │
│  ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓  │
│  ┃ 🏷️ دسته‌بندی محصولات - Row Width: 12               ┃  │
│  ┣━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┫  │
│  ┃ [به تفکیک دسته]    [به تفکیک نوع]                  ┃  │
│  ┃     Width: 6             Width: 6                    ┃  │
│  ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛  │
└─────────────────────────────────────────────────────────────┘
```

### ویجت‌ها (14 ویجت):
1. ClvDistributionWidget → ClvDistributionConfig
2. AverageClvByRfmWidget → AverageClvByRfmSegmentConfig
3. RfmSegmentDistributionWidget → RfmSegmentDistributionConfig
4. RetentionRateWidget → RetentionRateByMonthConfig
5. CustomersAtRiskWidget → CustomersAtRiskConfig
6. ChurnRiskDistributionWidget → ChurnRiskDistributionConfig
7. NpsSegmentationWidget → NpsSegmentationConfig
8. NpsDistributionWidget → NpsDistributionConfig
9. AverageSatisfactionWidget → AverageSatisfactionScoreConfig
10. SatisfactionDistributionWidget → SatisfactionScoreDistributionConfig
11. PopularProductsWidget → PopularProductsConfig
12. ProductRepeatPurchaseWidget → ProductRepeatPurchaseConfig
13. ProductCLVWidget → ProductCLVConfig
14. ProductsByCategoryWidget → ProductsByCategoryConfig
15. ProductsByTypeWidget → ProductsByTypeConfig

---

## ⚙️ OperationsDashboard (داشبورد عملیاتی)
**👥 Roles:** Admin, Manager, Analyst | **⭐ Default:** NO

```
┌─────────────────────────────────────────────────────────────┐
│ OperationsDashboard (داشبورد عملیاتی)                      │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓  │
│  ┃ 💳 نمای کلی تراکنش‌ها - Row Width: 12              ┃  │
│  ┣━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┫  │
│  ┃ [روند ماهانه]          [به تفکیک نوع]              ┃  │
│  ┃   Width: 8                 Width: 4                  ┃  │
│  ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛  │
│                                                             │
│  ┌─────────────────────────┐  ┌───────────────────────┐   │
│  │ ⭐ مدیریت امتیازات      │  │ 🔔 بازدید نشده       │   │
│  │ Width: 6                │  │ Width: 6              │   │
│  ├─────────────────────────┤  ├───────────────────────┤   │
│  │ • به تفکیک نوع          │  │ • امتیازات جدید      │   │
│  │ • بازدید خودکار         │  │ • موجودی مشتریان     │   │
│  └─────────────────────────┘  └───────────────────────┘   │
│                                                             │
│  ┌─────────────────────────┐  ┌───────────────────────┐   │
│  │ 📜 قوانین امتیازدهی    │  │ 📊 تحلیل قوانین      │   │
│  │ Width: 6                │  │ Width: 6              │   │
│  ├─────────────────────────┤  ├───────────────────────┤   │
│  │ • قوانین فعال           │  │ • تفکیک قانون         │   │
│  │ • جدیدترین قوانین       │  │ • تفکیک اکوسیستم        │   │
│  └─────────────────────────┘  └───────────────────────┘   │
│                                                             │
│  ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓  │
│  ┃ 🎯 رویدادهای سیستم - Row Width: 12                 ┃  │
│  ┣━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┫  │
│  ┃ [روند ماهانه]          [به تفکیک نوع]              ┃  │
│  ┃   Width: 8                 Width: 4                  ┃  │
│  ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛  │
│                                                             │
│  ┌───────────────────────────────┐  ┌─────────────────┐   │
│  │ 💚 سلامت سیستم                │  │ 🕐 اخیر         │   │
│  │ Width: 8                      │  │ Width: 4        │   │
│  ├───────────────────────────────┤  ├─────────────────┤   │
│  │ • فعالیت روزانه (30 روز)     │  │ • رویدادهای     │   │
│  │ • کاربران فعال (DAU)          │  │   اخیر (20)     │   │
│  └───────────────────────────────┘  └─────────────────┘   │
│                                                             │
│  ┌─────────────────────────┐  ┌───────────────────────┐   │
│  │ 📦 محصولات فعال         │  │ 📡 کانال‌های رویداد   │   │
│  │ Width: 6                │  │ Width: 6              │   │
│  ├─────────────────────────┤  ├───────────────────────┤   │
│  │ • لیست فعال (15)        │  │ • تفکیک کانال         │   │
│  │ • فعالیت محصولات        │  │ • فعال‌ترین مشتریان  │   │
│  └─────────────────────────┘  └───────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
```

### ویجت‌ها (18 ویجت):
1. TransactionsByMonthWidget → TransactionsByMonthConfig
2. TransactionsByTypeWidget → TransactionsByTypeConfig
3. PointsByTypeWidget → PointsByTypeConfig
4. AutoVisitPointsWidget → AutoVisitPointsConfig
5. UnvisitedTransactionsWidget → UnvisitedTransactionsConfig
6. CustomerPointsBalanceWidget → CustomerPointsBalanceConfig
7. ActiveScoringRulesWidget → ActiveScoringRulesConfig
8. RecentScoringRulesWidget → RecentScoringRulesConfig
9. TransactionsByScoringRuleWidget → TransactionsByScoringRuleConfig
10. ScoringRulesByTenantWidget → ScoringRulesByTenantConfig
11. EventLogByMonthWidget → EventLogByMonthConfig
12. EventLogByEventTypeWidget → EventLogByEventTypeConfig
13. SystemActivityWidget → SystemActivityDailyConfig
14. DailyActiveUsersWidget → DailyActiveUsersConfig
15. RecentEventsWidget → RecentEventsConfig
16. ActiveProductsWidget → ActiveProductsConfig
17. EventLogByProductWidget → EventLogByProductConfig
18. EventLogByChannelWidget → EventLogByChannelConfig
19. MostActiveCustomersWidget → MostActiveCustomersConfig

---

## 📊 خلاصه آمار

| داشبورد | تعداد Div | تعداد Widget | Default | Roles |
|---------|-----------|--------------|---------|-------|
| ExecutiveDashboard | 6 | 10 | ✅ YES | Admin, Manager |
| MarketingDashboard | 7 | 14 | ❌ NO | MarketingManager, Admin |
| AnalyticsDashboard | 9 | 15 | ❌ NO | Analyst, Admin |
| OperationsDashboard | 10 | 19 | ❌ NO | Admin, Manager, Analyst |
| **مجموع** | **32** | **58** | - | - |

---

## ✅ اطمینان از صحت کانفیگ‌ها

همه ۵۸ ویجت به ReportConfig‌های معتبر اشاره می‌کنند:
- ✅ همه در فایل‌های ReportConfig تعریف شده‌اند
- ✅ همه Roles مناسب دارند
- ✅ هیچ خطای "ویجتی تنظیم نشده است" وجود ندارد

---

**📅 آخرین بروزرسانی:** 1404/08/14  
**✨ وضعیت:** آماده به کارگیری



