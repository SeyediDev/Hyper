namespace Hyper.AdminPanel.Domain.UiDefinitions.CustomerTenants.Base;

public abstract class KpiWidgetCustomerTenantBase<TReportConfig>
    : KpiWidgetBase<CustomerTenant, CustomerTenantUiDefinitions.PublicReport, TReportConfig>
    where TReportConfig : ReportConfigDefinition
{ 
}
