namespace Hyper.AdminPanel.Domain.UiDefinitions.CustomerTenants.CustomerTransactions.Base;

using CustomerTransaction = CustomerTransaction;
using CustomerTransactionUiDefinitions = CustomerTransactionUiDefinitions;

public abstract class WidgetCustomerTransactionBase<TReportConfig>
    : DashboardDivWidgetDefinition<CustomerTransaction, CustomerTransactionUiDefinitions.PublicReport, TReportConfig>
    where TReportConfig : ReportConfigDefinition
{
}









