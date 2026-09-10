using Hyper.Domain.Entities.Events.Data;

namespace Hyper.AdminPanel.Domain.UiDefinitions.Events.Base;

using EventLog = EventLog;
using EventLogUiDefinitions = Hyper.AdminPanel.Domain.UiDefinitions.Events.EventLogUiDefinitions;

public abstract class WidgetEventLogBase<TReportConfig>
    : DashboardDivWidgetDefinition<EventLog, EventLogUiDefinitions.PublicReport, TReportConfig>
    where TReportConfig : ReportConfigDefinition
{
}









