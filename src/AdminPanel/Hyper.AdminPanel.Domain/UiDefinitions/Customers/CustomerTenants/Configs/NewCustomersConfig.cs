namespace Hyper.AdminPanel.Domain.UiDefinitions.Customers.CustomerTenants;

public partial class CustomerTenantUiDefinitions
{
    public partial class PublicReport : CRUDDefinition.PublicReport
    {
        /// <summary>
        /// کانفیگ گزارش مشتریان جدید (30 روز اخیر)
        /// </summary>
        public class NewCustomersConfig() : ChartConfigDefinition(ChartType.MetricBox)
        {
			protected override string Name => "مشتریان جدید";
			protected override List<string> Roles => HyperRoles.RolesManager;
            protected override string WhereCondition => $"{nameof(CustomerTenant.JoinDate)} >= (ServerDateTime().AddDays(-30))";

            protected override void DefineGroupBy()
            {
                Count();
            }

            protected override void DefineSubReports()
            {
                AddSubReport<CustomerTenantDetailsListConfig>();
            }
        }
    }
}
