namespace Hyper.AdminPanel.Domain.UiDefinitions.Customers.CustomerTenants;
public partial class CustomerTenantUiDefinitions
{
    public partial class PublicReport : CRUDDefinition.PublicReport
    {
        /// <summary>
        /// کانفیگ گزارش مشتریان فعال
        /// </summary>
        public class ActiveCustomersConfig() : ChartConfigDefinition(ChartType.MetricBox)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager];
            protected override string WhereCondition => $"{nameof(CustomerTenant.IsActive)} == true";
            protected override string Name => "مشتریان فعال";

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

