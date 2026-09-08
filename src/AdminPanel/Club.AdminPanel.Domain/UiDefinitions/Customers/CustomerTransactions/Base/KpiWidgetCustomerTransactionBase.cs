namespace Club.AdminPanel.Domain.UiDefinitions.Customers.CustomerTransactions.Base;

using CustomerTransaction = Club.Domain.Entities.Customers.CustomerTransaction;
using CustomerTransactionUiDefinitions = Club.AdminPanel.Domain.UiDefinitions.Customers.CustomerTransactionUiDefinitions;

public abstract class KpiWidgetCustomerTransactionBase<TReportConfig>
    : KpiWidgetBase<CustomerTransaction, CustomerTransactionUiDefinitions.PublicReport, TReportConfig>
    where TReportConfig : ReportConfigDefinition
{
}









