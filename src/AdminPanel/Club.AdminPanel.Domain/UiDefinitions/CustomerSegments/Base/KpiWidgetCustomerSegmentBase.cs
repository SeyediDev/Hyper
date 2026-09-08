namespace Hyper.AdminPanel.Domain.UiDefinitions.CustomerSegments.Base;

public abstract class KpiWidgetCustomerSegmentBase<TReportConfig>
    : KpiWidgetBase<CustomerSegment, CustomerSegmentUiDefinitions.PublicReport, TReportConfig>
    where TReportConfig : ReportConfigDefinition
{ 
}

