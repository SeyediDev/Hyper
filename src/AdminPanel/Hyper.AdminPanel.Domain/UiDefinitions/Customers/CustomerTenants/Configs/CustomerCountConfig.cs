namespace Hyper.AdminPanel.Domain.UiDefinitions.Customers.CustomerTenants;

public partial class CustomerTenantUiDefinitions
{
    public partial class PublicReport : CRUDDefinition.PublicReport
    {
        /// <summary>
        /// کانفیگ گزارش تعداد کل مشتریان
        /// </summary>
        public class CustomerCountConfig() : ChartConfigDefinition(ChartType.MetricBox)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager];
            protected override string Name => "تعداد کل مشتریان";

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

