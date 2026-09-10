namespace Hyper.AdminPanel.Domain.UiDefinitions.Tenants;

/// <summary>
/// Base class برای ویجت‌های TenantAttributeValue
/// </summary>
public abstract class WidgetTenantAttributeValueBase<TReportConfig> :
    DashboardDivWidgetDefinition<TenantAttributeValue, TenantAttributeValueUiDefinitions.PublicReport, TReportConfig>
    where TReportConfig : ReportConfigDefinition
{
    protected override string Icon => "calendar-days";
}