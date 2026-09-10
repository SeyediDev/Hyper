namespace Hyper.AdminPanel.Domain.UiDefinitions.Customers.CustomerTenants;

public partial class CustomerTenantUiDefinitions
{
    public partial class PublicReport : CRUDDefinition.PublicReport
    {
        /// <summary>
        /// لیست جامع جزئیات مشتریان برای استفاده در زیرگزارش‌ها
        /// </summary>
        public class CustomerTenantDetailsListConfig() : ReportConfigDefinition
        {
			protected override string Name => "جزئیات مشتریان";
			protected override List<string> Roles => HyperRoles.RolesAnalyst;
            

            // رنگ‌بندی بر اساس نمره ریسک ریزش و وضعیت
            protected override string TableDanger => $"{nameof(CustomerTenant.ChurnRiskScore)} >= 70";
            protected override string TableWarning => $"{nameof(CustomerTenant.ChurnRiskScore)} >= 50 && {nameof(CustomerTenant.ChurnRiskScore)} < 70";
            protected override string TableSuccess => $"{nameof(CustomerTenant.EngagementScore)} >= 80 && {nameof(CustomerTenant.ChurnRiskScore)} < 30";
            protected override string TableInfo => $"{nameof(CustomerTenant.EngagementScore)} >= 60 && {nameof(CustomerTenant.EngagementScore)} < 80";

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(CustomerTenant.Customer), "مشتری");
                DisplayColumn(nameof(CustomerTenant.Tenant), "اکوسیستم");
                DisplayColumn(nameof(CustomerTenant.Country), "کشور");
                DisplayColumn(nameof(CustomerTenant.Province), "استان");
                DisplayColumn(nameof(CustomerTenant.City), "استان");
                DisplayColumn(nameof(CustomerTenant.JoinDate), "تاریخ عضویت");
                DisplayColumn(nameof(CustomerTenant.RfmSegment), "دسته RFM");
                DisplayColumn(nameof(CustomerTenant.CustomerLifetimeValue), "ارزش طول عمر");
                DisplayColumn(nameof(CustomerTenant.EngagementScore), "نمره تعامل");
                DisplayColumn(nameof(CustomerTenant.ChurnRiskScore), "نمره ریزش");
                DisplayColumn(nameof(CustomerTenant.TotalTransactionValue), "ارزش کل تراکنش");
                DisplayColumn(nameof(CustomerTenant.LastInteractionDate), "آخرین تعامل");
            }
        }
    }
}