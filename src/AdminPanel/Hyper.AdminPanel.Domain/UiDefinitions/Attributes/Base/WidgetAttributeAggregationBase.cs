namespace Hyper.AdminPanel.Domain.UiDefinitions.Attributes.Base;

/// <summary>
/// Base class برای ویجت‌های Daily Aggregation
/// </summary>
public abstract class WidgetDailyAggregationBase<TReportConfig> :
    DashboardDivWidgetDefinition<TenantAttributeDailyAggregation, TenantAttributeDailyAggregationUiDefinitions.PublicReport, TReportConfig>
    where TReportConfig : ReportConfigDefinition
{
    protected override string Icon => "calendar-days";
}

/// <summary>
/// Base class برای ویجت‌های Persian Monthly Aggregation
/// </summary>
public abstract class WidgetPersianMonthlyAggregationBase<TReportConfig> :
    DashboardDivWidgetDefinition<TenantAttributeMonthlyAggregationPersian, TenantAttributeMonthlyAggregationPersianUiDefinitions.PublicReport, TReportConfig>
    where TReportConfig : ReportConfigDefinition
{
    protected override string Icon => "calendar-alt";
}

/// <summary>
/// Base class برای ویجت‌های Gregorian Monthly Aggregation
/// </summary>
public abstract class WidgetGregorianMonthlyAggregationBase<TReportConfig> :
    DashboardDivWidgetDefinition<TenantAttributeMonthlyAggregationGregorian, TenantAttributeMonthlyAggregationGregorianUiDefinitions.PublicReport, TReportConfig>
    where TReportConfig : ReportConfigDefinition
{
    protected override string Icon => "calendar-check";
}
