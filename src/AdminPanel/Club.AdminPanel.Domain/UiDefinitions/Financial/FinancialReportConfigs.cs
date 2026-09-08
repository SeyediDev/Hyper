namespace Hyper.AdminPanel.Domain.UiDefinitions.Promotions;

public partial class PromotionUiDefinitions
{
    public new partial class PublicReport : CRUDDefinition.PublicReport
    {
        // =====================================================
        // Revenue Reports
        // =====================================================

        /// <summary>
        /// گزارش روند درآمد ماهانه
        /// </summary>
        public class MonthlyRevenueTrendConfig() : ChartConfigDefinition(ChartType.Line)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.FinanceManager];
            protected override string Name => "روند درآمد ماهانه";

            protected override void DefineGroupBy()
            {
                GroupBy("FromDateMonth", "ماه");
                Sum("Metrics.CampaignRevenue", "درآمد");
                Sum("Metrics.CampaignCost", "هزینه");
            }
        }

        /// <summary>
        /// گزارش درآمد به تفکیک دسته کمپین
        /// </summary>
        public class RevenueByCampaignCategoryConfig() : ChartConfigDefinition(ChartType.Pie)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.FinanceManager, HyperRoles.MarketingManager];
            protected override string Name => "درآمد به تفکیک دسته کمپین";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(Promotion.Category), "دسته کمپین");
                Sum("Metrics.CampaignRevenue", "کل درآمد");
            }
        }

        // =====================================================
        // Cost Reports
        // =====================================================

        /// <summary>
        /// گزارش کل هزینه‌های بازاریابی
        /// </summary>
        public class TotalMarketingCostsConfig() : ChartConfigDefinition(ChartType.MetricBox)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.FinanceManager];
            protected override string Name => "کل هزینه‌های بازاریابی";

            protected override void DefineGroupBy()
            {
                Sum("Metrics.CampaignCost", string.Empty);
            }
        }

        /// <summary>
        /// گزارش هزینه‌های بازاریابی ماهانه
        /// </summary>
        public class MonthlyMarketingCostsConfig() : ChartConfigDefinition(ChartType.Column)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.FinanceManager];
            protected override string Name => "هزینه‌های بازاریابی ماهانه";

            protected override void DefineGroupBy()
            {
                GroupBy("FromDateMonth", "ماه");
                Sum("Metrics.CampaignCost", "هزینه");
                Count(null, "تعداد کمپین‌ها");
            }
        }

        /// <summary>
        /// گزارش هزینه به تفکیک دسته کمپین
        /// </summary>
        public class CostByCampaignCategoryConfig() : ChartConfigDefinition(ChartType.Pie)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.FinanceManager];
            protected override string Name => "هزینه به تفکیک دسته";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(Promotion.Category), "دسته کمپین");
                Sum("Metrics.CampaignCost", "کل هزینه");
            }
        }

        // =====================================================
        // Profitability Reports
        // =====================================================

        /// <summary>
        /// گزارش سودآوری کلی
        /// </summary>
        public class OverallProfitabilityConfig() : ChartConfigDefinition(ChartType.MetricBox)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.FinanceManager];
            protected override string Name => "سودآوری کلی";

            protected override void DefineGroupBy()
            {
                Sum("Metrics.CampaignRevenue", "کل درآمد");
                Sum("Metrics.CampaignCost", "کل هزینه");
            }
        }

        /// <summary>
        /// گزارش سودآوری ماهانه
        /// </summary>
        public class MonthlyProfitabilityConfig() : ChartConfigDefinition(ChartType.Line)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.FinanceManager];
            protected override string Name => "سودآوری ماهانه";

            protected override void DefineGroupBy()
            {
                GroupBy("FromDateMonth", "ماه");
                Sum("Metrics.CampaignRevenue", "درآمد");
                Sum("Metrics.CampaignCost", "هزینه");
            }
        }

        /// <summary>
        /// گزارش سودآوری مشتریان (بر اساس CLV)
        /// </summary>
        public class CustomerProfitabilityConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.FinanceManager];
            protected override string Name => "سودآوری مشتریان";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerTenant.RfmSegment), "دسته RFM");
                Average(nameof(CustomerTenant.CustomerLifetimeValue), "میانگین CLV");
                Count(null, "تعداد مشتریان");
            }
        }

        /// <summary>
        /// گزارش سودآوری جوامع مشتریان
        /// </summary>
        public class SegmentProfitabilityConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.FinanceManager];
            protected override string Name => "سودآوری جوامع";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerSegment.Title), "جامعه مشتریان");
                Sum(nameof(CustomerSegment.ActualSize), "تعداد اعضا");
            }
        }

        // =====================================================
        // ROI Reports
        // =====================================================

        /// <summary>
        /// گزارش ROI به تفکیک ماه
        /// </summary>
        public class MonthlyRoiConfig() : ChartConfigDefinition(ChartType.Line)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.FinanceManager];
            protected override string Name => "ROI ماهانه";

            protected override void DefineGroupBy()
            {
                GroupBy("FromDateMonth", "ماه");
                Average("Metrics.ReturnOnInvestment", "میانگین ROI");
            }
        }

        /// <summary>
        /// گزارش مقایسه ROI کمپین‌ها
        /// </summary>
        public class CampaignRoiComparisonConfig() : GroupByConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.FinanceManager, HyperRoles.MarketingManager];
            protected override string Name => "مقایسه ROI کمپین‌ها";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(Promotion.Title), "کمپین");
                Average("Metrics.ReturnOnInvestment", "ROI (%)");
                Sum("Metrics.CampaignRevenue", "درآمد");
                Sum("Metrics.CampaignCost", "هزینه");
            }
        }

        // =====================================================
        // Forecast Reports
        // =====================================================

        /// <summary>
        /// گزارش پیش‌بینی درآمد
        /// </summary>
        public class RevenueForecastConfig() : ChartConfigDefinition(ChartType.Line)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.FinanceManager, HyperRoles.Analyst];
            protected override string Name => "پیش‌بینی درآمد";

            protected override void DefineGroupBy()
            {
                GroupBy("FromDateMonth", "ماه");
                Sum("Metrics.CampaignRevenue", "درآمد واقعی");
            }
        }

        /// <summary>
        /// گزارش بودجه در مقابل واقعی
        /// </summary>
        public class BudgetVsActualConfig : GroupByConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.FinanceManager];
            protected override string Name => "بودجه در مقابل واقعی";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(Promotion.Title), "کمپین");
                Sum("Metrics.CampaignCost", "هزینه واقعی");
            }
        }
    }
}

