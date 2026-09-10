namespace Hyper.AdminPanel.Domain.UiDefinitions.Customers.CustomerTransactions.Base;

using CustomerTransaction = Hyper.Domain.Entities.Customers.CustomerTransaction;
using CustomerTransactionUiDefinitions = Hyper.AdminPanel.Domain.UiDefinitions.Customers.CustomerTransactionUiDefinitions;

public abstract class WidgetCustomerTransactionBase<TReportConfig>
    : DashboardDivWidgetDefinition<CustomerTransaction, CustomerTransactionUiDefinitions.PublicReport, TReportConfig>
    where TReportConfig : ReportConfigDefinition
{
}









