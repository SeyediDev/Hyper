using Hyper.AdminPanel.Domain.UiDefinitions.CustomerTenants.CustomerTransactions.Base;

namespace Hyper.AdminPanel.Domain.UiDefinitions.HomePage;

public partial class HomePageEntityUiDefinitions
{
    public partial class HomePageDashboard
    {
        /// <summary>
        /// تب وفاداری و ارزش با تمرکز بر امتیازات، پاداش‌ها و عملکرد محصولات
        /// </summary>
        public class LoyaltyAndValueDashboard : DashboardConfigDefinition
        {
            protected override string Title => "وفاداری و ارزش";
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager, HyperRoles.Analyst];
            protected override bool IsDefault => false;
            protected override string Icon => "star-badge";

            /// <summary>
            /// KPI: امتیازات اعطا شده
            /// </summary>
            public class LoyaltyPointsIssuedWidget : KpiWidgetCustomerTransactionBase<CustomerTransactionUiDefinitions.PublicReport.PointsIssuedStatsConfig>
            {
                public override string Title => "امتیازات اعطا شده";
                protected override string IconClass => "fal fa-badge-check";
                protected override string IconId => "star-badge";
            }

            /// <summary>
            /// KPI: امتیازات مصرف شده
            /// </summary>
            public class LoyaltyPointsRedeemedWidget : KpiWidgetCustomerTransactionBase<CustomerTransactionUiDefinitions.PublicReport.PointsRedeemedStatsConfig>
            {
                public override string Title => "امتیازات مصرف شده";
                protected override string IconClass => "fal fa-shopping-basket";
                protected override string IconId => "gift-present";
            }

            /// <summary>
            /// KPI: موجودی کل امتیازات
            /// </summary>
            public class LoyaltyTotalPointsBalanceWidget : KpiWidgetCustomerTransactionBase<CustomerTransactionUiDefinitions.PublicReport.TotalPointsBalanceConfig>
            {
                public override string Title => "موجودی کل امتیازات";
                protected override string IconClass => "fal fa-wallet";
                protected override string IconId => "wallet-money";
            }

            /// <summary>
            /// KPI: میانگین ارزش تراکنش امتیازی
            /// </summary>
            public class LoyaltyAverageTransactionValueWidget : KpiWidgetCustomerTransactionBase<CustomerTransactionUiDefinitions.PublicReport.AverageTransactionValueConfig>
            {
                public override string Title => "میانگین ارزش تراکنش";
                protected override string IconClass => "fal fa-chart-line";
                protected override string IconId => "coins-money";
            }

            public class PointsByTypeWidget : DashboardDivWidgetDefinition<CustomerTransaction, CustomerTransactionUiDefinitions.PublicReport, CustomerTransactionUiDefinitions.PublicReport.PointsByTypeConfig>
            {
                public override string Title => "توزیع امتیازات بر اساس نوع";
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
                protected override string Icon => "star-badge";
            }

            public class CustomerPointsBalanceWidget : DashboardDivWidgetDefinition<CustomerTransaction, CustomerTransactionUiDefinitions.PublicReport, CustomerTransactionUiDefinitions.PublicReport.CustomerPointsBalanceConfig>
            {
                public override string Title => "موجودی امتیازات مشتریان";
                protected override int? MaxRecordCount => 15;
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
                protected override string Icon => "wallet-money";
            }

		}
	}
}

