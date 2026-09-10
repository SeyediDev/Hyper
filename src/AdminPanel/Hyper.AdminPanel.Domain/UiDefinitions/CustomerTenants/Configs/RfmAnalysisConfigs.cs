using Hyper.Domain.Entities.Analytics.Enums;

namespace Hyper.AdminPanel.Domain.UiDefinitions.Customers.CustomerTenants;

public partial class CustomerTenantUiDefinitions
{
    public partial class PublicReport : CRUDDefinition.PublicReport
    {
        // =====================================================
        // RFM Analysis Reports
        // =====================================================

        /// <summary>
        /// گزارش تحلیل RFM - توزیع مشتریان بر اساس دسته‌های RFM
        /// </summary>
        public class RfmSegmentDistributionConfig : GroupByConfigDefinition
        {
            protected override List<string> Roles => HyperRoles.RolesAnalyst;
            protected override string Name => "توزیع جوامع RFM";

			// رنگ‌بندی بر اساس دسته RFM
			protected override string TableSuccess => $"({nameof(CustomerTenant.RfmSegment)} == {(int)RFMSegment.Champions}) || ({nameof(CustomerTenant.RfmSegment)} == {(int)RFMSegment.LoyalCustomers})";
			protected override string TableInfo => $"({nameof(CustomerTenant.RfmSegment)} == {(int)RFMSegment.PotentialLoyalists}) || ({nameof(CustomerTenant.RfmSegment)} == {(int)RFMSegment.NewCustomers})";
            protected override string TableWarning => $"({nameof(CustomerTenant.RfmSegment)} == {(int)RFMSegment.PotentialLoyalists})";
            protected override string TableDanger => 
                $"({nameof(CustomerTenant.RfmSegment)} == {(int)RFMSegment.AtRisk}) || " +
                $"({nameof(CustomerTenant.RfmSegment)} == {(int)RFMSegment.CannotLoseThem}) || " +
                $"({nameof(CustomerTenant.RfmSegment)} == {(int)RFMSegment.Hibernating}) || " +
                $"({nameof(CustomerTenant.RfmSegment)} == {(int)RFMSegment.Lost})";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerTenant.RfmSegment), "دسته RFM");
                Count(null, "تعداد مشتریان");
				AverageDD1(nameof(CustomerTenant.RecencyScore), "میانگین نمره تازگی Recency");
				AverageDD1(nameof(CustomerTenant.FrequencyScore), "میانگین نمره تکرار Frequency");
				AverageDD1(nameof(CustomerTenant.MonetaryScore), "میانگین نمره ارزش Monetary");
				AverageDD1(nameof(CustomerTenant.CustomerLifetimeValue), "میانگین ارزش طول عمر مشتری CLV");
				AverageDD1(nameof(CustomerTenant.NpsScore), "میانگین نمره NPS");
				AverageDD1(nameof(CustomerTenant.TotalTransactionValue), "میانگین ارزش کل تراکنش‌ها");
				AverageDD1(nameof(CustomerTenant.TotalInteractions), "میانگین تعداد تعاملات");
                AverageDD1(nameof(CustomerTenant.AverageOrderValue), "میانگین ارزش سفارش");
                AverageDD1(nameof(CustomerTenant.PurchaseFrequency), "میانگین فرکانس خرید");
                AverageDD1(nameof(CustomerTenant.EngagementScore), "میانگین نمره تعامل");
                AverageDD1(nameof(CustomerTenant.LoyaltyScore), "میانگین نمره وفاداری");
                AverageDD1(nameof(CustomerTenant.ChurnRiskScore), "میانگین احتمال ریزش");
                AverageDD1(nameof(CustomerTenant.SatisfactionScore), "میانگین نمره رضایت");
            }

            protected override void DefineSubReports()
            {
                AddSubReport<CustomersWithRfmDetailsConfig>();
            }
        }

		public class RfmSegmentDistributionChartConfig() : ChartConfigDefinition(ChartType.Pie)
        {
            protected override List<string> Roles => HyperRoles.RolesAnalyst;
            protected override string Name => "توزیع جوامع RFM";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerTenant.RfmSegment), "دسته RFM");
                Count(null, "تعداد مشتریان");
            }
        }

        /// <summary>
        /// گزارش نمودار پراکندگی RFM - Recency vs Frequency
        /// </summary>
        public class RfmScatterRecencyFrequencyConfig() : ChartConfigDefinition(ChartType.Scatter)
        {
            protected override List<string> Roles => HyperRoles.RolesAnalyst;
            protected override string Name => "نمودار پراکندگی RFM (تازگی-تکرار)";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerTenant.RecencyScore), "نمره تازگی");
                GroupBy(nameof(CustomerTenant.FrequencyScore), "نمره تکرار");
                Average(nameof(CustomerTenant.MonetaryScore), "میانگین نمره ارزش");
                Count(null, "تعداد");
            }
        }

        /// <summary>
        /// گزارش میانگین نمرات RFM به تفکیک دسته
        /// </summary>
        public class RfmScoresBySegmentConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => HyperRoles.RolesManager;
            protected override string Name => "میانگین نمرات RFM";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerTenant.RfmSegment), "دسته RFM");
                Average(nameof(CustomerTenant.RecencyScore), "میانگین تازگی");
                Average(nameof(CustomerTenant.FrequencyScore), "میانگین تکرار");
                Average(nameof(CustomerTenant.MonetaryScore), "میانگین ارزش");
                Count(null, "تعداد مشتریان");
            }
        }

        /// <summary>
        /// گزارش مشتریان به تفکیک نمره Recency
        /// </summary>
        public class CustomersByRecencyScoreConfig() : ChartConfigDefinition(ChartType.Column)
        {
            protected override List<string> Roles => HyperRoles.RolesAnalyst;
            protected override string Name => "توزیع نمره تازگی";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerTenant.RecencyScore), "نمره تازگی");
                Count(null, "تعداد مشتریان");
            }
        }

        /// <summary>
        /// گزارش مشتریان به تفکیک نمره Frequency
        /// </summary>
        public class CustomersByFrequencyScoreConfig() : ChartConfigDefinition(ChartType.Column)
        {
            protected override List<string> Roles => HyperRoles.RolesAnalyst;
            protected override string Name => "توزیع نمره تکرار";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerTenant.FrequencyScore), "نمره تکرار");
                Count(null, "تعداد مشتریان");
            }
        }

        /// <summary>
        /// گزارش مشتریان به تفکیک نمره Monetary
        /// </summary>
        public class CustomersByMonetaryScoreConfig() : ChartConfigDefinition(ChartType.Column)
        {
            protected override List<string> Roles => HyperRoles.RolesAnalyst;
            protected override string Name => "توزیع نمره ارزش";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerTenant.MonetaryScore), "نمره ارزش");
                Count(null, "تعداد مشتریان");
            }
        }

        /// <summary>
        /// گزارش لیست مشتریان با جزئیات RFM
        /// </summary>
        public class CustomersWithRfmDetailsConfig() : ReportConfigDefinition
        {
            protected override List<string> Roles => HyperRoles.RolesManager;
            protected override string Name => "لیست مشتریان با جزئیات RFM";
            

            // رنگ‌بندی بر اساس دسته RFM
            protected override string TableSuccess => $"({nameof(CustomerTenant.RfmSegment)} == {(int)RFMSegment.Champions}) || ({nameof(CustomerTenant.RfmSegment)} == {(int)RFMSegment.LoyalCustomers})";
            protected override string TableInfo => $"({nameof(CustomerTenant.RfmSegment)} == {(int)RFMSegment.PotentialLoyalists}) || ({nameof(CustomerTenant.RfmSegment)} == {(int)RFMSegment.NewCustomers})";
            protected override string TableWarning => $"({nameof(CustomerTenant.RfmSegment)} == {(int)RFMSegment.PotentialLoyalists})";
            protected override string TableDanger => $"({nameof(CustomerTenant.RfmSegment)} == {(int)RFMSegment.AtRisk}) || ({nameof(CustomerTenant.RfmSegment)} == {(int)RFMSegment.CannotLoseThem}) || ({nameof(CustomerTenant.RfmSegment)} == {(int)RFMSegment.Hibernating}) || ({nameof(CustomerTenant.RfmSegment)} == {(int)RFMSegment.Lost})";

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(CustomerTenant.Customer), "مشتری");
                DisplayColumn(nameof(CustomerTenant.Tenant), "اکوسیستم");
                DisplayColumn(nameof(CustomerTenant.RfmSegment), "دسته RFM");
                DisplayColumn(nameof(CustomerTenant.RecencyScore), "نمره تازگی");
                DisplayColumn(nameof(CustomerTenant.FrequencyScore), "نمره تکرار");
                DisplayColumn(nameof(CustomerTenant.MonetaryScore), "نمره ارزش");
                DisplayColumn(nameof(CustomerTenant.LastInteractionDate), "آخرین تعامل");
                DisplayColumn(nameof(CustomerTenant.TotalTransactionValue), "ارزش کل");
            }
        }

    }
}
