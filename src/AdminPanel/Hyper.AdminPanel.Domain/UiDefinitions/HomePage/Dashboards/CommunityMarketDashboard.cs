using Hyper.AdminPanel.Domain.UiDefinitions.CustomerSegments;
using Hyper.AdminPanel.Domain.UiDefinitions.CustomerSegments.Base;
using Hyper.Domain.Entities.CustomerSegments.Data;

namespace Hyper.AdminPanel.Domain.UiDefinitions.HomePage;

public partial class HomePageEntityUiDefinitions
{
    public partial class HomePageDashboard
    {
        /// <summary>
        /// داشبورد تخصصی جامعه/بازار
        /// نمایش اطلاعات به تفکیک جامعه‌ها/بازارها و موقعیت جغرافیایی (شهر/استان/کشور)
        /// </summary>
        public class CommunityMarketDashboard : DashboardConfigDefinition
        {
            protected override string Title => "جامعه / بازار";
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.MarketingManager, HyperRoles.Analyst, HyperRoles.Manager];
            protected override bool IsDefault => false;
            protected override string Icon => "users-group";

            // =====================================================
            // KPI Widgets - Overview Metrics
            // =====================================================

            /// <summary>
            /// KPI: تعداد کل جوامع/بازارهای فعال
            /// </summary>
            public class TotalActiveCommunitiesWidget : KpiWidgetCustomerSegmentBase<CustomerSegmentUiDefinitions.PublicReport.TotalActiveCommunitiesConfig>
            {
                public override string Title => "تعداد جوامع/بازارهای فعال";
                protected override string IconClass => "fal fa-users";
                protected override string IconId => "users-group";
            }

            /// <summary>
            /// KPI: تعداد کل اعضای جوامع/بازارها
            /// </summary>
            public class TotalCommunityMembersWidget : KpiWidgetCustomerSegmentBase<CustomerSegmentUiDefinitions.PublicReport.TotalCommunityMembersConfig>
            {
                public override string Title => "تعداد کل اعضای جوامع/بازارها";
                protected override string IconClass => "fal fa-user-friends";
                protected override string IconId => "users-group";
            }

            /// <summary>
            /// KPI: میانگین اندازه جوامع/بازارها
            /// </summary>
            public class AverageCommunitySizeWidget : KpiWidgetCustomerSegmentBase<CustomerSegmentUiDefinitions.PublicReport.AverageCommunitySizeConfig>
            {
                public override string Title => "میانگین اندازه جوامع/بازارها";
                protected override string IconClass => "fal fa-chart-bar";
                protected override string IconId => "chart-bar";
            }

            /// <summary>
            /// KPI: میانگین نرخ رشد جوامع/بازارها
            /// </summary>
            public class AverageGrowthRateWidget : KpiWidgetCustomerSegmentBase<CustomerSegmentUiDefinitions.PublicReport.AverageGrowthRateConfig>
            {
                public override string Title => "میانگین نرخ رشد جوامع/بازارها";
                protected override string IconClass => "fal fa-chart-line";
                protected override string IconId => "chart-line";
            }

            // =====================================================
            // Segment Distribution Widgets
            // =====================================================

            /// <summary>
            /// نمودار دایره‌ای: توزیع جوامع/بازارها
            /// </summary>
            public class SegmentDistributionWidget : DashboardDivWidgetDefinition<CustomerSegment, CustomerSegmentUiDefinitions.PublicReport, CustomerSegmentUiDefinitions.PublicReport.SegmentDistributionConfig>
            {
                public override string Title => "توزیع جوامع/بازارهای مشتریان";
                protected override int? HeightInPixels => 360;
                public override int Width => 6;
                protected override string Icon => "chart-pie";
            }

            /// <summary>
            /// نمودار میله‌ای: نرخ رشد جوامع/بازارها
            /// </summary>
            public class SegmentGrowthRateWidget : DashboardDivWidgetDefinition<CustomerSegment, CustomerSegmentUiDefinitions.PublicReport, CustomerSegmentUiDefinitions.PublicReport.SegmentGrowthRateConfig>
            {
                public override string Title => "نرخ رشد جوامع/بازارها";
                protected override int? HeightInPixels => 360;
                public override int Width => 6;
                protected override string Icon => "chart-bar";
            }

            /// <summary>
            /// نمودار میله‌ای: نرخ حفظ مشتریان جوامع/بازارها
            /// </summary>
            public class SegmentRetentionRateWidget : DashboardDivWidgetDefinition<CustomerSegment, CustomerSegmentUiDefinitions.PublicReport, CustomerSegmentUiDefinitions.PublicReport.SegmentRetentionRateConfig>
            {
                public override string Title => "نرخ حفظ مشتریان جوامع/بازارها";
                protected override int? HeightInPixels => 360;
                public override int Width => 12;
                protected override string Icon => "chart-bar";
            }

            public class SegmentEngagementRateWidget : DashboardDivWidgetDefinition<CustomerSegmentMembership, CustomerSegmentMembershipUiDefinitions.PublicReport, CustomerSegmentMembershipUiDefinitions.PublicReport.SegmentCommunity>
			{
                public override string Title => "نرخ تعامل جوامع/بازارها";
                protected override int? HeightInPixels => 360;
                public override int Width => 12;
                protected override string Icon => "chart-bar";
            }

			public class RfmSegmentCommunityPivotWidget : DashboardDivWidgetDefinition<CustomerSegmentMembership, CustomerSegmentMembershipUiDefinitions.PublicReport, CustomerSegmentMembershipUiDefinitions.PublicReport.RfmSegmentCommunityPivotConfig>
			{
				public override string Title => "تحلیل تقاطع «جامعه» با «دسته RFM» بر اساس «تعداد مشتریان / مجموع CLV / میانگین CLV / میانگین نمره تعامل / میانگین NPS / جمع هزینه جذب (CAC) / میانگین هزینه جذب (CAC) / میانگین حاشیه سود»";
				protected override int? HeightInPixels => 400;
				public override int Width => 12;
			}
			// =====================================================
			// Geographic Analysis Widgets
			// =====================================================

			/// <summary>
			/// نقشه ایران: توزیع اعضای جوامع/بازارها بر اساس استان
			/// </summary>
			public class SegmentMembersIranMapWidget : DashboardDivWidgetDefinition<CustomerSegmentMembership, CustomerSegmentMembershipUiDefinitions.PublicReport, CustomerSegmentMembershipUiDefinitions.PublicReport.SegmentMembersIranMapConfig>
            {
                public override string Title => "نقشه ایران: توزیع اعضای جوامع/بازارها";
                protected override int? HeightInPixels => 400;
                public override int Width => 12;
                protected override string Icon => "map-marked-alt";
            }

            /// <summary>
            /// نمودار میله‌ای: اعضای جوامع/بازارها به تفکیک استان
            /// </summary>
            public class SegmentMembersByProvinceWidget : DashboardDivWidgetDefinition<CustomerSegmentMembership, CustomerSegmentMembershipUiDefinitions.PublicReport, CustomerSegmentMembershipUiDefinitions.PublicReport.SegmentMembersByProvinceConfig>
            {
                public override string Title => "اعضای جوامع/بازارها به تفکیک استان";
                protected override int? HeightInPixels => 400;
                public override int Width => 6;
                protected override string Icon => "chart-bar";
            }

            /// <summary>
            /// نمودار میله‌ای: اعضای جوامع/بازارها به تفکیک شهر
            /// </summary>
            public class SegmentMembersByCityWidget : DashboardDivWidgetDefinition<CustomerSegmentMembership, CustomerSegmentMembershipUiDefinitions.PublicReport, CustomerSegmentMembershipUiDefinitions.PublicReport.SegmentMembersByCityConfig>
            {
                public override string Title => "اعضای جوامع/بازارها به تفکیک شهر";
                protected override int? HeightInPixels => 400;
                public override int Width => 6;
                protected override string Icon => "chart-bar";
            }

            /// <summary>
            /// گزارش: اعضای جوامع/بازارها به تفکیک موقعیت جغرافیایی
            /// </summary>
            public class SegmentMembersGeographyOverviewWidget : DashboardDivWidgetDefinition<CustomerSegmentMembership, CustomerSegmentMembershipUiDefinitions.PublicReport, CustomerSegmentMembershipUiDefinitions.PublicReport.SegmentMembersGeographyOverviewConfig>
            {
                public override string Title => "اعضای جوامع/بازارها به تفکیک موقعیت جغرافیایی";
                protected override int? HeightInPixels => 400;
                public override int Width => 12;
                protected override string Icon => "table";
            }

            // =====================================================
            // Segment Performance Widgets
            // =====================================================

            /// <summary>
            /// گزارش: عملکرد کلی جوامع/بازارها
            /// </summary>
            public class SegmentPerformanceOverviewWidget : DashboardDivWidgetDefinition<CustomerSegment, CustomerSegmentUiDefinitions.PublicReport, CustomerSegmentUiDefinitions.PublicReport.SegmentPerformanceOverviewConfig>
            {
                public override string Title => "عملکرد کلی جوامع/بازارها";
                protected override int? HeightInPixels => 400;
                public override int Width => 12;
                protected override string Icon => "table";
            }

            /// <summary>
            /// گزارش: مقایسه اندازه تخمینی و واقعی جوامع/بازارها
            /// </summary>
            public class SegmentSizeComparisonWidget : DashboardDivWidgetDefinition<CustomerSegment, CustomerSegmentUiDefinitions.PublicReport, CustomerSegmentUiDefinitions.PublicReport.SegmentSizeComparisonConfig>
            {
                public override string Title => "مقایسه اندازه جوامع/بازارها (تخمینی vs واقعی)";
                protected override int? HeightInPixels => 400;
                public override int Width => 12;
                protected override string Icon => "chart-bar";
            }
		}
	}
}

