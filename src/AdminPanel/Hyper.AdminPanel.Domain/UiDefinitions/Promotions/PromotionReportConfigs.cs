namespace Hyper.AdminPanel.Domain.UiDefinitions.Promotions;

public partial class PromotionUiDefinitions
{
    public new partial class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => DefaultRoles;

        // =====================================================
        // Campaign Calendar & Timeline
        // =====================================================

        /// <summary>
        /// تقویم کمپین‌های فعال - نمایش تعداد کمپین‌های فعال برای هر روز
        /// </summary>
        public partial class ActivationCalendarConfig() : ChartConfigDefinition(ChartType.Calendar)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.MarketingManager];
            protected override string Name => "تقویم کمپین‌های فعال";
            protected override string WhereCondition => $"{nameof(Promotion.Status)} == {(int)PromotionStatus.Active}";

            protected override void DefineGroupBy()
            {
                GroupByFormula($"DATE({nameof(Promotion.FromDate)})", "تاریخ آغاز");
                Count(null, "تعداد کمپین‌ها");
            }
        }

        // =====================================================
        // Campaign Performance Reports
        // =====================================================

        /// <summary>
        /// گزارش کل درآمد
        /// </summary>
        public partial class TotalRevenueConfig() : ChartConfigDefinition(ChartType.MetricBox)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.FinanceManager];
            protected override string Name => "کل درآمد";

            protected override void DefineGroupBy()
            {
                Sum("Metrics.CampaignRevenue");
            }
        }

        /// <summary>
        /// گزارش ROI کلی
        /// </summary>
        public partial class OverallRoiConfig() : ChartConfigDefinition(ChartType.MetricBox)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.FinanceManager];
            protected override string Name => "ROI کلی";

            protected override void DefineGroupBy()
            {
                Average("Metrics.ReturnOnInvestment");
            }
        }

        /// <summary>
        /// KPI حجم عملکرد کمپین‌ها
        /// </summary>
        public partial class CampaignPerformanceVolumeKpiConfig() : ChartConfigDefinition(ChartType.MetricBox)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.MarketingManager, HyperRoles.Analyst];
            protected override string Name => "حجم عملکرد کمپین‌ها";

            protected override void DefineGroupBy()
            {
                Sum("Metrics.ConversionCount", "تعداد تبدیل‌ها");
            }
        }

        /// <summary>
        /// گزارش عملکرد کمپین‌ها - نمای کلی
        /// </summary>
        public partial class CampaignPerformanceOverviewConfig() : ReportConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.MarketingManager];
            protected override string Name => "عملکرد کلی کمپین‌ها";
            

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(Promotion.Title), "عنوان");
                DisplayColumn(nameof(Promotion.Status), "وضعیت");
                DisplayColumn("Metrics.TargetAudienceCount", "هدف");
                DisplayColumn("Metrics.ConversionCount", "تبدیل");
                DisplayColumn("Metrics.ConversionRate", "نرخ تبدیل (%)");
                DisplayColumn("Metrics.ReturnOnInvestment", "ROI (%)");
                DisplayColumn("Metrics.EffectivenessScore", "نمره اثربخشی");
                OrderByDesc("Metrics.EffectivenessScore"); // نزولی - بهترین عملکرد
            }
        }

        /// <summary>
        /// گزارش ROI کمپین‌ها
        /// </summary>
        public partial class CampaignRoiAnalysisConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.MarketingManager, HyperRoles.FinanceManager];
            protected override string Name => "تحلیل ROI کمپین‌ها";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(Promotion.Title), "عنوان");
                Sum("Metrics.CampaignCost", "هزینه");
                Sum("Metrics.CampaignRevenue", "درآمد");
                Average("Metrics.ReturnOnInvestment", "ROI (%)");
            }
        }

        /// <summary>
        /// گزارش نرخ تبدیل کمپین‌ها
        /// </summary>
        public partial class ConversionRateAnalysisConfig() : ChartConfigDefinition(ChartType.Column)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.MarketingManager];
            protected override string Name => "تحلیل نرخ تبدیل";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(Promotion.Title), "عنوان");
                Average("Metrics.ConversionRate", "نرخ تبدیل (%)");
                Sum("Metrics.TargetAudienceCount", "تعداد هدف");
                Sum("Metrics.ConversionCount", "تعداد تبدیل");
            }
        }

        /// <summary>
        /// گزارش هزینه جذب مشتری (CAC)
        /// </summary>
        public partial class CustomerAcquisitionCostConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.MarketingManager, HyperRoles.FinanceManager];
            protected override string Name => "تحلیل هزینه جذب مشتری";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(Promotion.Title), "عنوان");
                Average("Metrics.CustomerAcquisitionCost", "CAC");
                Sum("Metrics.NewCustomersAcquired", "مشتریان جدید");
                Sum("Metrics.CampaignCost", "هزینه کمپین");
            }
        }

        // =====================================================
        // Message Effectiveness Reports
        // =====================================================

        /// <summary>
        /// گزارش نرخ تحویل پیام‌ها
        /// </summary>
        public partial class MessageDeliveryRateConfig() : ChartConfigDefinition(ChartType.Column)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.MarketingManager];
            protected override string Name => "نرخ تحویل پیام‌ها";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(Promotion.Title), "عنوان");
                Sum("Metrics.MessagesSent", "ارسال شده");
                Sum("Metrics.MessagesDelivered", "تحویل شده");
                Average("Metrics.DeliveryRate", "نرخ تحویل (%)");
            }
        }

        /// <summary>
        /// گزارش نرخ باز شدن پیام‌ها
        /// </summary>
        public partial class MessageOpenRateConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.MarketingManager];
            protected override string Name => "نرخ باز شدن پیام‌ها";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(Promotion.Title), "عنوان");
                Sum("Metrics.MessagesDelivered", "تحویل شده");
                Sum("Metrics.OpenCount", "باز شده");
                Average("Metrics.OpenRate", "نرخ باز شدن (%)");
            }
        }

        /// <summary>
        /// گزارش نرخ کلیک پیام‌ها
        /// </summary>
        public partial class MessageClickThroughRateConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.MarketingManager];
            protected override string Name => "نرخ کلیک پیام‌ها";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(Promotion.Title), "عنوان");
                Sum("Metrics.OpenCount", "باز شده");
                Sum("Metrics.ClickCount", "کلیک شده");
                Average("Metrics.ClickThroughRate", "نرخ کلیک (%)");
            }
        }

        /// <summary>
        /// گزارش فانل تعامل پیام (Delivery → Open → Click → Conversion)
        /// </summary>
        public partial class MessageEngagementFunnelConfig() : ChartConfigDefinition(ChartType.Column)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.MarketingManager, HyperRoles.Analyst];
            protected override string Name => "فانل تعامل پیام";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(Promotion.Title), "عنوان");
                Sum("Metrics.MessagesDelivered", "تحویل شده");
                Sum("Metrics.OpenCount", "باز شده");
                Sum("Metrics.ClickCount", "کلیک شده");
                Sum("Metrics.ConversionCount", "تبدیل");
            }
        }

        /// <summary>
        /// گزارش مقایسه عملکرد کمپین‌ها
        /// </summary>
        public partial class CampaignPerformanceComparisonConfig() : GroupByConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.MarketingManager, HyperRoles.Analyst];
            protected override string Name => "مقایسه عملکرد کمپین‌ها";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(Promotion.Title), "عنوان");
                Sum("Metrics.TargetAudienceCount", "هدف");
                Sum("Metrics.ConversionCount", "تبدیل");
                Sum("Metrics.CampaignRevenue", "درآمد");
                Sum("Metrics.CampaignCost", "هزینه");
            }
        }

        /// <summary>
        /// گزارش کمپین‌های فعال
        /// </summary>
        public partial class ActiveCampaignsConfig : ReportConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.MarketingManager];
            protected override string WhereCondition => $"{nameof(Promotion.Status)} == {(int)PromotionStatus.Active}";
            protected override string Name => "کمپین‌های فعال";
            

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(Promotion.Title), "عنوان");
                DisplayColumn(nameof(Promotion.Status), "وضعیت");
                DisplayColumn(nameof(Promotion.FromDate), "تاریخ شروع");
                DisplayColumn(nameof(Promotion.ToDate), "تاریخ پایان");
                DisplayColumn("Metrics.TargetAudienceCount", "هدف");
            }
        }

        /// <summary>
        /// KPI تعداد کمپین‌های فعال
        /// </summary>
        public partial class ActiveCampaignsCountConfig() : ChartConfigDefinition(ChartType.MetricBox)
        {
			protected override string Name => "تعداد کمپین‌های فعال";
			protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.MarketingManager];
            protected override string WhereCondition => $"{nameof(Promotion.Status)} == {(int)PromotionStatus.Active}";

            protected override void DefineGroupBy()
            {
                Count(null, string.Empty);
            }
        }

        /// <summary>
        /// گزارش هزینه هر تبدیل
        /// </summary>
        public partial class CostPerConversionConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.MarketingManager, HyperRoles.FinanceManager];
            protected override string Name => "هزینه هر تبدیل";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(Promotion.Title), "عنوان");
                Average("Metrics.CostPerConversion", "هزینه هر تبدیل");
                Sum("Metrics.ConversionCount", "تعداد تبدیل");
            }
        }

        /// <summary>
        /// گزارش نمره اثربخشی کمپین‌ها
        /// </summary>
        public partial class CampaignEffectivenessScoreConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.MarketingManager, HyperRoles.Analyst];
            protected override string Name => "نمره اثربخشی کمپین‌ها";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(Promotion.Title), "عنوان");
                Average("Metrics.EffectivenessScore", "نمره اثربخشی");
            }
        }

        /// <summary>
        /// گزارش کمپین‌ها به تفکیک دسته
        /// </summary>
        public partial class CampaignsByCategoryConfig() : ChartConfigDefinition(ChartType.Pie)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.MarketingManager];
            protected override string Name => "کمپین‌ها به تفکیک دسته";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(Promotion.Category), "دسته");
                Count(null, "تعداد کمپین‌ها");
            }
        }

        /// <summary>
        /// گزارش روند عملکرد کمپین‌ها
        /// </summary>
        public partial class CampaignPerformanceTrendConfig() : ChartConfigDefinition(ChartType.Line)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.MarketingManager];
            protected override string Name => "روند عملکرد کمپین";

            protected override void DefineGroupBy()
            {
                GroupBy($"Month({nameof(Promotion.FromDate)})", "ماه");
                Sum("Metrics.ConversionCount", "تبدیل‌ها");
                Sum("Metrics.CampaignRevenue", "درآمد");
            }
        }

        /// <summary>
        /// گزارش روند حجم عملکرد کمپین‌ها
        /// </summary>
        public partial class CampaignPerformanceVolumeTrendConfig() : ChartConfigDefinition(ChartType.Column)
        {
			protected override string Name => "روند حجم عملکرد کمپین‌ها";
			protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.MarketingManager];

            protected override void DefineGroupBy()
            {
				GroupByFormula($"Month({nameof(Promotion.CreateDate)})", "ماه");
				Sum("Metrics.MessagesSent", "پیام‌های ارسال شده");
                Sum("Metrics.MessagesDelivered", "پیام‌های تحویل شده");
                Sum("Metrics.OpenCount", "باز شده");
                Sum("Metrics.ClickCount", "کلیک شده");
            }
        }
    }
}

