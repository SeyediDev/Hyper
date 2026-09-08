using Hyper.AdminPanel.Domain.UiDefinitions.CustomerTenants.CustomerTransactions.Base;
using Hyper.AdminPanel.Domain.UiDefinitions.CustomerTenants.Base;

namespace Hyper.AdminPanel.Domain.UiDefinitions.HomePage;

public partial class HomePageEntityUiDefinitions
{
    public partial class HomePageDashboard
    {
        /// <summary>
        /// تب بازاریابی و تعامل با تاکید بر عملکرد کمپین‌ها و بازخورد مشتریان
        /// </summary>
        public class MarketingAndEngagementDashboard : DashboardConfigDefinition
        {
            protected override string Title => "بازاریابی و تعامل";
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.MarketingManager, HyperRoles.Analyst];
            protected override bool IsDefault => false;
            protected override string Icon => "gift-present";

            /// <summary>
            /// KPI: حجم عملکرد کمپین‌ها
            /// </summary>
            public class MarketingPerformanceVolumeWidget : KpiWidgetPromotionBase<PromotionUiDefinitions.PublicReport.CampaignPerformanceVolumeKpiConfig>
            {
                public override string Title => "حجم عملکرد کمپین‌ها";
                protected override string IconClass => "fal fa-chart-line";
                protected override string IconId => "chart-bar";
            }

            /// <summary>
            /// KPI: هزینه کل بازاریابی
            /// </summary>
            public class MarketingTotalMarketingCostsWidget : KpiWidgetPromotionBase<PromotionUiDefinitions.PublicReport.TotalMarketingCostsConfig>
            {
                public override string Title => "هزینه کل بازاریابی";
                protected override string IconClass => "fal fa-wallet";
                protected override string IconId => "wallet-money";
            }

            /// <summary>
            /// KPI: کمپین‌های فعال
            /// </summary>
            public class MarketingActiveCampaignsKpiWidget : KpiWidgetPromotionBase<PromotionUiDefinitions.PublicReport.ActiveCampaignsCountConfig>
            {
                public override string Title => "کمپین‌های فعال";
                protected override string IconClass => "fal fa-bullhorn";
                protected override string IconId => "rss-signal";
            }

            /// <summary>
            /// KPI: میانگین ارزش تراکنش
            /// </summary>
            public class MarketingAverageTransactionValueWidget : KpiWidgetCustomerTransactionBase<CustomerTransactionUiDefinitions.PublicReport.AverageTransactionValueConfig>
            {
                public override string Title => "میانگین ارزش تراکنش";
                protected override string IconClass => "fal fa-coins";
                protected override string IconId => "coins-money";
            }

            public class CampaignPerformanceVolumeTrendWidget : WidgetPromotionBase<PromotionUiDefinitions.PublicReport.CampaignPerformanceVolumeTrendConfig>
            {
                public override string Title => "روند حجم عملکرد کمپین‌ها";
                protected override int? MaxRecordCount => 12;
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
                protected override string Icon => "chart-bar";
            }

            public class ConversionRateWidget : WidgetPromotionBase<PromotionUiDefinitions.PublicReport.ConversionRateAnalysisConfig>
            {
                public override string Title => "نرخ تبدیل کمپین‌ها";
                protected override int? MaxRecordCount => 12;
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
                protected override string Icon => "chart-bar";
            }

            public class MessageFunnelWidget : WidgetPromotionBase<PromotionUiDefinitions.PublicReport.MessageEngagementFunnelConfig>
            {
                public override string Title => "فانل تعامل پیام‌های کمپین";
                protected override int? MaxRecordCount => 6;
                protected override int? HeightInPixels => 240;
                public override int Width => 12;
                protected override string Icon => "list-checklist";
            }

            public class CustomerJoinCalendarWidget : WidgetCustomerTenantBase<CustomerTenantUiDefinitions.PublicReport.CustomerJoinCalendarConfig>
            {
                public override string Title => "تقویم جذب مشتریان";
                protected override int? HeightInPixels => 360;
                public override int Width => 12;
                protected override string Icon => "calendar-days";
            }
            /*
            public class CampaignActivationCalendarWidget : WidgetPromotionBase<PromotionUiDefinitions.PublicReport.ActivationCalendarConfig>
            {
                public override string Title => "تقویم کمپین‌های فعال";
                protected override int? HeightInPixels => 350;
                public override int Width => 12;
                protected override string Icon => "calendar-days";
            }*/
        }
    }
}

