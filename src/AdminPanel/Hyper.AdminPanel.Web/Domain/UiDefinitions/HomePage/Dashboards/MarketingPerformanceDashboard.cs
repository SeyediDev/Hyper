using Hyper.AdminPanel.Domain.UiDefinitions.CustomerTenants.Base;
using Hyper.Domain.Entities.Events.Data;

namespace Hyper.AdminPanel.Domain.UiDefinitions.HomePage;

public partial class HomePageEntityUiDefinitions
{
    /// <summary>
    /// داشبورد تخصصی عملکرد بازاریابی با تمرکز بر کمپین، کانال و بازخورد
    /// </summary>
    public class MarketingPerformanceDashboard : DashboardDefinition
    {
        public override List<string>? Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.MarketingManager, HyperRoles.Analyst];

        protected override Form Identify()
        {
            return DefineDashboard("داشبورد عملکرد بازاریابی");
        }

        protected override void DataSources()
        {
            base.DataSources();
            AddReport<Promotion>();
            AddReport<CustomerTenant>();
            AddReport<EventLog>();
            AddReport<Survey>();
        }

        /// <summary>
        /// تب عملکرد کمپین‌ها
        /// </summary>
        public class CampaignPerformanceConfig : DashboardConfigDefinition
        {
            protected override string Title => "عملکرد کمپین‌ها";
            protected override List<string>? Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.MarketingManager, HyperRoles.Analyst];
            protected override bool IsDefault => true;

            public class ActiveCampaignsWidget : DashboardDivWidgetDefinition<Promotion, PromotionUiDefinitions.PublicReport, PromotionUiDefinitions.PublicReport.ActiveCampaignsConfig>
            {
                public override string Title => "کمپین‌های فعال";
                protected override int? MaxRecordCount => 15;
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
            }

            public class CampaignRoiWidget : DashboardDivWidgetDefinition<Promotion, PromotionUiDefinitions.PublicReport, PromotionUiDefinitions.PublicReport.CampaignRoiAnalysisConfig>
            {
                public override string Title => "بازدهی سرمایه‌گذاری (ROI)";
                protected override int? MaxRecordCount => 10;
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
            }

            public class ConversionRateWidget : DashboardDivWidgetDefinition<Promotion, PromotionUiDefinitions.PublicReport, PromotionUiDefinitions.PublicReport.ConversionRateAnalysisConfig>
            {
                public override string Title => "نرخ تبدیل کمپین";
                protected override int? MaxRecordCount => 10;
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
            }

            public class CacWidget : DashboardDivWidgetDefinition<Promotion, PromotionUiDefinitions.PublicReport, PromotionUiDefinitions.PublicReport.CustomerAcquisitionCostConfig>
            {
                public override string Title => "هزینه جذب مشتری (CAC)";
                protected override int? MaxRecordCount => 10;
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
            }

            public class CampaignPerformanceTrendWidget : DashboardDivWidgetDefinition<Promotion, PromotionUiDefinitions.PublicReport, PromotionUiDefinitions.PublicReport.CampaignPerformanceTrendConfig>
            {
                public override string Title => "روند عملکرد کمپین‌ها";
                protected override int? MaxRecordCount => 12;
                protected override int? HeightInPixels => 260;
                public override int Width => 8;
            }

            public class TopCampaignsWidget : DashboardDivWidgetDefinition<Promotion, PromotionUiDefinitions.PublicReport, PromotionUiDefinitions.PublicReport.CampaignPerformanceOverviewConfig>
            {
                public override string Title => "کمپین‌های برتر";
                protected override int? MaxRecordCount => 10;
                protected override int? HeightInPixels => 260;
                public override int Width => 4;
            }
        }

        /// <summary>
        /// تب تعامل کانال‌ها و پیام‌ها
        /// </summary>
        public class ChannelEngagementConfig : DashboardConfigDefinition
        {
            protected override string Title => "تعامل و کانال‌ها";
            protected override List<string>? Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.MarketingManager, HyperRoles.Analyst];

            public class MessageFunnelWidget : DashboardDivWidgetDefinition<Promotion, PromotionUiDefinitions.PublicReport, PromotionUiDefinitions.PublicReport.MessageEngagementFunnelConfig>
            {
                public override string Title => "فانل تعامل پیام";
                protected override int? MaxRecordCount => 5;
                protected override int? HeightInPixels => 240;
                public override int Width => 8;
            }

            public class DeliveryRateWidget : DashboardDivWidgetDefinition<Promotion, PromotionUiDefinitions.PublicReport, PromotionUiDefinitions.PublicReport.MessageDeliveryRateConfig>
            {
                public override string Title => "نرخ تحویل پیام‌ها";
                protected override int? MaxRecordCount => 10;
                protected override int? HeightInPixels => 240;
                public override int Width => 4;
            }

            public class HighlyEngagedWidget : WidgetCustomerTenantBase<CustomerTenantUiDefinitions.PublicReport.HighlyEngagedCustomersConfig>
            {
                public override string Title => "مشتریان با تعامل بالا";
                protected override int? MaxRecordCount => 15;
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
            }

            public class EventLogByChannelWidget : DashboardDivWidgetDefinition<EventLog, EventLogUiDefinitions.PublicReport, EventLogUiDefinitions.PublicReport.EventLogByChannelConfig>
            {
                public override string Title => "تعامل به تفکیک کانال";
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
            }

            public class MostActiveCustomersWidget : DashboardDivWidgetDefinition<EventLog, EventLogUiDefinitions.PublicReport, EventLogUiDefinitions.PublicReport.MostActiveCustomersConfig>
            {
                public override string Title => "مشتریان با بیشترین تعامل";
                protected override int? MaxRecordCount => 15;
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
            }
        }

        /// <summary>
        /// تب بازخورد و تجربه
        /// </summary>
        public class FeedbackLoopConfig : DashboardConfigDefinition
        {
            protected override string Title => "بازخورد و تجربه";
            protected override List<string>? Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.MarketingManager, HyperRoles.Analyst];

            public class ActiveSurveysWidget : DashboardDivWidgetDefinition<Survey, SurveyUiDefinitions.PublicReport, SurveyUiDefinitions.PublicReport.ActiveSurveysConfig>
            {
                public override string Title => "نظرسنجی‌های فعال";
                protected override int? MaxRecordCount => 10;
                protected override int? HeightInPixels => 240;
                public override int Width => 8;
            }

            public class SurveyParticipationWidget : DashboardDivWidgetDefinition<Survey, SurveyUiDefinitions.PublicReport, SurveyUiDefinitions.PublicReport.AverageParticipationConfig>
            {
                public override string Title => "میانگین مشارکت";
                protected override int? MaxRecordCount => 1;
                protected override int? HeightInPixels => 160;
                public override int Width => 4;
            }

            public class TopSurveysWidget : DashboardDivWidgetDefinition<Survey, SurveyUiDefinitions.PublicReport, SurveyUiDefinitions.PublicReport.TopSurveysConfig>
            {
                public override string Title => "نظرسنجی‌های محبوب";
                protected override int? MaxRecordCount => 12;
                protected override int? HeightInPixels => 240;
                public override int Width => 12;
            }

            public class NpsSegmentationWidget : WidgetCustomerTenantBase<CustomerTenantUiDefinitions.PublicReport.NpsSegmentationConfig>
            {
                public override string Title => "دسته‌بندی NPS";
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
            }

            public class NpsDistributionWidget : WidgetCustomerTenantBase<CustomerTenantUiDefinitions.PublicReport.NpsDistributionConfig>
            {
                public override string Title => "توزیع امتیاز NPS";
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
            }


            public class SatisfactionDistributionWidget : WidgetCustomerTenantBase<CustomerTenantUiDefinitions.PublicReport.SatisfactionScoreDistributionConfig>
            {
                public override string Title => "توزیع رضایت مشتریان";
                protected override int? HeightInPixels => 240;
                public override int Width => 9;
            }
        }
    }
}

