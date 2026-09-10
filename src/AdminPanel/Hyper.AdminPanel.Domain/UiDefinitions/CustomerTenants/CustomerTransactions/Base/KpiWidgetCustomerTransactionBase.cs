namespace Hyper.AdminPanel.Domain.UiDefinitions.CustomerTenants.CustomerTransactions.Base;

using CustomerTransaction = CustomerTransaction;
using CustomerTransactionUiDefinitions = CustomerTransactionUiDefinitions;

public abstract class KpiWidgetCustomerTransactionBase<TReportConfig>
    : KpiWidgetBase<CustomerTransaction, CustomerTransactionUiDefinitions.PublicReport, TReportConfig>
    where TReportConfig : ReportConfigDefinition
{
}









