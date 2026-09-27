using CustomerTenantUiDefinitions = Hyper.AdminPanel.Domain.UiDefinitions.Customers.CustomerTenants.CustomerTenantUiDefinitions;
using CustomerSegmentMembershipUiDefinitions = Hyper.AdminPanel.Domain.UiDefinitions.CustomerSegments.CustomerSegmentMembershipUiDefinitions;
using Hyper.Domain.Entities.CustomerSegments.Data;
using Hyper.AdminPanel.Domain.UiDefinitions.CustomerTenants.Base;

namespace Hyper.AdminPanel.Domain.UiDefinitions.HomePage;

public partial class HomePageEntityUiDefinitions
{
    /// <summary>
    /// داشبورد تخصصی بینش مشتری با تمرکز بر ارزش، حفظ و تجربه مشتریان
    /// </summary>
    public class CustomerIntelligenceDashboard : DashboardDefinition
    {
        public override List<string>? Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager, HyperRoles.Analyst];

        protected override Form Identify()
        {
            return DefineDashboard("داشبورد بینش مشتری");
        }

        protected override void DataSources()
        {
            base.DataSources();
            AddReport<CustomerTenant>();
            AddReport<CustomerSegment>();
            AddReport<CustomerTransaction>();
            AddReport<Product>();
            AddReport<Point>();
            AddReport<Survey>();
        }

        /// <summary>
        /// تب ارزش و بخش‌بندی مشتریان
        /// </summary>
        public class ValueAndSegmentsConfig : DashboardConfigDefinition
        {
            protected override string Title => "ارزش و بخش‌بندی";
            protected override List<string>? Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager, HyperRoles.Analyst];

            public class ClvDistributionWidget : WidgetCustomerTenantBase< CustomerTenantUiDefinitions.PublicReport.ClvDistributionConfig>
            {
                public override string Title => "توزیع ارزش طول عمر (CLV)";
                protected override int? HeightInPixels => 260;
                public override int Width => 6;
            }

            public class AverageClvByRfmWidget : WidgetCustomerTenantBase< CustomerTenantUiDefinitions.PublicReport.AverageClvByRfmSegmentConfig>
            {
                public override string Title => "میانگین CLV بر اساس RFM";
                protected override int? HeightInPixels => 260;
                public override int Width => 6;
            }

            public class RfmSegmentDistributionWidget : WidgetCustomerTenantBase< CustomerTenantUiDefinitions.PublicReport.RfmSegmentDistributionConfig>
            {
                public override string Title => "توزیع بخش‌های RFM";
                protected override int? HeightInPixels => 260;
                public override int Width => 12;
            }

            public class SegmentDistributionWidget : DashboardDivWidgetDefinition<CustomerSegment, CustomerSegmentUiDefinitions.PublicReport, CustomerSegmentUiDefinitions.PublicReport.SegmentDistributionConfig>
            {
                public override string Title => "تقسیم‌بندی جامعه مشتریان";
                protected override int? HeightInPixels => 260;
                public override int Width => 6;
            }

            public class RfmSegmentCommunityPivotWidget : DashboardDivWidgetDefinition<CustomerSegmentMembership, CustomerSegmentMembershipUiDefinitions.PublicReport, CustomerSegmentMembershipUiDefinitions.PublicReport.RfmSegmentCommunityPivotConfig>
            {
                public override string Title => "تحلیل تقاطع «جامعه» با «دسته RFM» بر اساس «تعداد مشتریان / مجموع CLV / میانگین CLV / میانگین نمره تعامل / میانگین NPS / جمع هزینه جذب (CAC) / میانگین هزینه جذب (CAC) / میانگین حاشیه سود»";
                protected override int? HeightInPixels => 400;
                public override int Width => 12;
            }

        }

        /// <summary>
        /// تب حفظ مشتریان و پیش‌بینی ریزش
        /// </summary>
        public class RetentionAndChurnConfig : DashboardConfigDefinition
        {
            protected override string Title => "حفظ و ریزش";
            protected override List<string>? Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager, HyperRoles.Analyst];

            public class RetentionRateWidget : WidgetCustomerTenantBase< CustomerTenantUiDefinitions.PublicReport.RetentionRateByMonthConfig>
            {
                public override string Title => "نرخ حفظ ماهانه";
                protected override int? MaxRecordCount => 12;
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
            }

            public class CustomersAtRiskWidget : WidgetCustomerTenantBase< CustomerTenantUiDefinitions.PublicReport.CustomersAtRiskConfig>
            {
                public override string Title => "مشتریان در معرض ریزش";
                protected override int? MaxRecordCount => 20;
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
            }

            public class ChurnRiskDistributionWidget : WidgetCustomerTenantBase< CustomerTenantUiDefinitions.PublicReport.ChurnRiskDistributionConfig>
            {
                public override string Title => "توزیع احتمال ریزش";
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
            }

            public class CustomerPointsBalanceWidget : DashboardDivWidgetDefinition<CustomerTransaction, CustomerTransactionUiDefinitions.PublicReport, CustomerTransactionUiDefinitions.PublicReport.CustomerPointsBalanceConfig>
            {
                public override string Title => "موجودی امتیازات مشتریان";
                protected override int? MaxRecordCount => 20;
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
            }

            public class UnvisitedTransactionsWidget : DashboardDivWidgetDefinition<CustomerTransaction, CustomerTransactionUiDefinitions.PublicReport, CustomerTransactionUiDefinitions.PublicReport.UnvisitedTransactionsConfig>
            {
                public override string Title => "تراکنش‌های بازدید نشده";
                protected override int? MaxRecordCount => 30;
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
            }

            public class PointsByTypeWidget : DashboardDivWidgetDefinition<Point, PointUiDefinitions.PublicReport, PointUiDefinitions.PublicReport.PointsByTypeConfig>
            {
                public override string Title => "توزیع امتیازات";
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
            }

            public class EngagementDistributionWidget : WidgetCustomerTenantBase< CustomerTenantUiDefinitions.PublicReport.EngagementScoreDistributionConfig>
            {
                public override string Title => "توزیع نمره تعامل";
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
            }

            public class HighlyEngagedCustomersWidget : WidgetCustomerTenantBase< CustomerTenantUiDefinitions.PublicReport.HighlyEngagedCustomersConfig>
            {
                public override string Title => "مشتریان با تعامل بالا";
                protected override int? MaxRecordCount => 15;
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
            }
        }

        /// <summary>
        /// تب تجربه و بازخورد مشتریان
        /// </summary>
        public class ExperienceInsightsConfig : DashboardConfigDefinition
        {
            protected override string Title => "تجربه و بازخورد";
            protected override List<string>? Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager, HyperRoles.Analyst];

            public class NpsSegmentationWidget : WidgetCustomerTenantBase< CustomerTenantUiDefinitions.PublicReport.NpsSegmentationConfig>
            {
                public override string Title => "دسته‌بندی NPS";
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
            }

            public class NpsDistributionWidget : WidgetCustomerTenantBase< CustomerTenantUiDefinitions.PublicReport.NpsDistributionConfig>
            {
                public override string Title => "توزیع امتیاز NPS";
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
            }

            public class AverageSatisfactionWidget : WidgetCustomerTenantBase< CustomerTenantUiDefinitions.PublicReport.AverageSatisfactionScoreConfig>
            {
                public override string Title => "میانگین رضایت مشتری";
                protected override int? MaxRecordCount => 1;
                protected override int? HeightInPixels => 160;
                public override int Width => 3;
            }

            public class AverageSatisfactionGaugeWidget : WidgetCustomerTenantBase< CustomerTenantUiDefinitions.PublicReport.AverageSatisfactionScoreGaugeConfig>
            {
                public override string Title => "متوسط نمره رضایت";
                protected override int? HeightInPixels => 240;
                public override int Width => 4;
            }

            public class AverageEngagementGaugeWidget : WidgetCustomerTenantBase< CustomerTenantUiDefinitions.PublicReport.AverageEngagementScoreGaugeConfig>
            {
                public override string Title => "متوسط نمره تعامل";
                protected override int? HeightInPixels => 240;
                public override int Width => 4;
            }

            public class AverageLoyaltyGaugeWidget : WidgetCustomerTenantBase< CustomerTenantUiDefinitions.PublicReport.AverageLoyaltyScoreGaugeConfig>
            {
                public override string Title => "متوسط نمره وفاداری";
                protected override int? HeightInPixels => 240;
                public override int Width => 4;
            }

            public class SatisfactionDistributionWidget : WidgetCustomerTenantBase< CustomerTenantUiDefinitions.PublicReport.SatisfactionScoreDistributionConfig>
            {
                public override string Title => "توزیع رضایت مشتریان";
                protected override int? HeightInPixels => 240;
                public override int Width => 12;
            }

            public class ActiveSurveysWidget : DashboardDivWidgetDefinition<Survey, SurveyUiDefinitions.PublicReport, SurveyUiDefinitions.PublicReport.ActiveSurveysConfig>
            {
                public override string Title => "نظرسنجی‌های فعال";
                protected override int? MaxRecordCount => 10;
                protected override int? HeightInPixels => 240;
                public override int Width => 9;
            }

            public class SurveyParticipationWidget : DashboardDivWidgetDefinition<Survey, SurveyUiDefinitions.PublicReport, SurveyUiDefinitions.PublicReport.AverageParticipationConfig>
            {
                public override string Title => "میانگین مشارکت";
                protected override int? MaxRecordCount => 1;
                protected override int? HeightInPixels => 140;
                public override int Width => 3;
            }

            public class TopSurveysWidget : DashboardDivWidgetDefinition<Survey, SurveyUiDefinitions.PublicReport, SurveyUiDefinitions.PublicReport.TopSurveysConfig>
            {
                public override string Title => "نظرسنجی‌های محبوب";
                protected override int? MaxRecordCount => 10;
                protected override int? HeightInPixels => 240;
                public override int Width => 12;
            }
        }

        /// <summary>
        /// تب توزیع جهانی: نمایش توزیع مشتریان روی نقشه جهان با ویجت‌های تکمیلی
        /// </summary>
        public class GlobalGeographyDashboard : DashboardConfigDefinition
        {
            protected override string Title => "توزیع جهانی";
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager, HyperRoles.Analyst];
            protected override string Icon => "globe";

            public class WorldMapWidget : WidgetCustomerTenantBase<CustomerTenantUiDefinitions.PublicReport.GlobalGeographicDistributionConfig>
            {
                public override string Title => "نقشه توزیع مشتریان - جهان";
                protected override int? HeightInPixels => 320;
                public override int Width => 12;
            }

            public class CountryBarWidget : WidgetCustomerTenantBase<CustomerTenantUiDefinitions.PublicReport.CountryBarDistributionConfig>
            {
                public override string Title => "نمودار توزیع کشوری";
                protected override int? HeightInPixels => 360;
                public override int Width => 3;
                protected override int? MaxRecordCount { get; } = 32;
            }

            public class CountryTreeMapWidget : WidgetCustomerTenantBase<CustomerTenantUiDefinitions.PublicReport.CountryTreeMapConfig>
            {
                public override string Title => "درخت‌نقشه توزیع کشورها (دریل‌داون استان/شهر)";
                protected override int? HeightInPixels => 400;
                public override int Width => 9;
            }
        }
    }
}