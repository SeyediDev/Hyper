namespace Hyper.AdminPanel.Domain.UiDefinitions.CustomerTenants.Base;

public abstract class WidgetCustomerTenantBase<TReportConfig>
    : DashboardDivWidgetDefinition<CustomerTenant, CustomerTenantUiDefinitions.PublicReport, TReportConfig>
    where TReportConfig : ReportConfigDefinition
{
}
