using CustomerTenantUiDefinitions = Hyper.AdminPanel.Domain.UiDefinitions.Customers.CustomerTenants.CustomerTenantUiDefinitions;
using CustomerSegmentMembershipUiDefinitions = Hyper.AdminPanel.Domain.UiDefinitions.CustomerSegments.CustomerSegmentMembershipUiDefinitions;
using Hyper.Domain.Entities.CustomerSegments.Data;
using Hyper.AdminPanel.Domain.UiDefinitions.CustomerTenants.Base;
using Hyper.AdminPanel.Domain.UiDefinitions.CustomerTenants.CustomerTransactions.Base;

namespace Hyper.AdminPanel.Domain.UiDefinitions.HomePage;

public partial class HomePageEntityUiDefinitions
{
    public partial class HomePageDashboard
    {
        /// <summary>
        /// تب نمای کلی مدیریت با تمرکز بر شاخص‌های کلان کسب‌وکار
        /// </summary>
        public class ManagementOverviewDashboard : DashboardConfigDefinition
        {
            protected override string Title => "نمای کلی مدیریت";
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager];
            protected override bool IsDefault => true;
            protected override string Icon => "home-dashboard";

            public class TotalCustomersWidget : KpiWidgetCustomerTenantBase<CustomerTenantUiDefinitions.PublicReport.CustomerCountConfig>
            {
                public override string Title => "کل مشتریان";
                protected override string IconClass => "fal fa-users";
                protected override string IconId => "users-group";
            }

            public class ActiveCustomersWidget : KpiWidgetCustomerTenantBase<CustomerTenantUiDefinitions.PublicReport.ActiveCustomersConfig>
            {
                public override string Title => "مشتریان فعال";
                protected override string IconClass => "fal fa-user-check";
                protected override string IconId => "users-analysis";
            }

            public class NewCustomersWidget : KpiWidgetCustomerTenantBase<CustomerTenantUiDefinitions.PublicReport.NewCustomersConfig>
            {
                public override string Title => "مشتریان جدید";
                protected override string IconClass => "fal fa-user-plus";
                protected override string IconId => "users-network";
            }

            public class CampaignPerformanceVolumeWidget : KpiWidgetPromotionBase<PromotionUiDefinitions.PublicReport.CampaignPerformanceVolumeKpiConfig>
            {
                public override string Title => "حجم عملکرد کمپین‌ها";
                protected override string IconClass => "fal fa-chart-line";
                protected override string IconId => "chart-bar";
            }

            public class TransactionsOverviewWidget : WidgetCustomerTransactionBase<CustomerTransactionUiDefinitions.PublicReport.TransactionsByMonthConfig>
            {
                public override string Title => "نمای تراکنش‌ها";
                protected override int? MaxRecordCount => 12;
                protected override int? HeightInPixels => 260;
                public override int Width => 4;
                protected override string Icon => "chart-bar";
            }

            public class CustomerGrowthByMonthWidget : WidgetCustomerTenantBase<CustomerTenantUiDefinitions.PublicReport.CustomerGrowthByMonthConfig>
            {
                public override string Title => "روند رشد مشتریان";
                protected override int? MaxRecordCount => 12;
                protected override int? HeightInPixels => 240;
                public override int Width => 4;
                protected override string Icon => "chart-bar";
            }

            public class RfmSegmentChartWidget : WidgetCustomerTenantBase<CustomerTenantUiDefinitions.PublicReport.RfmSegmentDistributionChartConfig>
            {
                public override string Title => "نمودار توزیع RFM";
                protected override int? HeightInPixels => 240;
                public override int Width => 4;
                protected override string Icon => "users-analysis";
            }
			public class SegmentOverviewWidget : DashboardDivWidgetDefinition<CustomerSegmentMembership, CustomerSegmentMembershipUiDefinitions.PublicReport, CustomerSegmentMembershipUiDefinitions.PublicReport.SegmentCommunity>
			{
				public override string Title => "نمای کلی جوامع/بازارها";
				protected override int? HeightInPixels => 240;
				public override int Width => 12;
				protected override string Icon => "users-analysis";
			}
			public class RfmSegmentOverviewWidget : WidgetCustomerTenantBase<CustomerTenantUiDefinitions.PublicReport.RfmSegmentDistributionConfig>
            {
                public override string Title => "نمای کلی وفاداری RFM";
                protected override int? HeightInPixels => 240;
                public override int Width => 12;
                protected override string Icon => "users-analysis";
            }
            /*
            public class EventLogByDayCalendarWidget : WidgetEventLogBase<EventLogUiDefinitions.PublicReport.EventLogByDayCalendarConfig>
            {
                public override string Title => "تقویم رویدادهای روزانه";
                protected override int? HeightInPixels => 350;
                public override int Width => 12;
                protected override string Icon => "calendar-days";
            }

            public class DailyAttributeAggregationWidget : WidgetDailyAggregationBase<TenantAttributeDailyAggregationUiDefinitions.PublicReport.DailyAggregationCalendarConfig>
            {
                public override string Title => "تقویم تجمیع‌های روزانه ویژگی‌ها";
                protected override int? HeightInPixels => 360;
                public override int Width => 12;
            }

            public class MonthlyPersianAttributeAggregationWidget : WidgetPersianMonthlyAggregationBase<TenantAttributeDailyAggregationUiDefinitions.PersianMonthlyAggregationReport.MonthlyPersianCalendarConfig>
            {
                public override string Title => "تقویم تجمیع‌های ماهانه (شمسی)";
                protected override int? HeightInPixels => 360;
                public override int Width => 12;
            }*/
		}
	}
}

