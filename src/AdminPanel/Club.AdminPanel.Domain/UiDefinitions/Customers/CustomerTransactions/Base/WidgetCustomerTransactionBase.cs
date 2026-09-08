namespace Club.AdminPanel.Domain.UiDefinitions.Customers.CustomerTransactions.Base;

using CustomerTransaction = Club.Domain.Entities.Customers.CustomerTransaction;
using CustomerTransactionUiDefinitions = Club.AdminPanel.Domain.UiDefinitions.Customers.CustomerTransactionUiDefinitions;

public abstract class WidgetCustomerTransactionBase<TReportConfig>
    : DashboardDivWidgetDefinition<CustomerTransaction, CustomerTransactionUiDefinitions.PublicReport, TReportConfig>
    where TReportConfig : ReportConfigDefinition
{
}









