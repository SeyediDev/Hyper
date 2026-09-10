namespace Hyper.AdminPanel.Domain.UiDefinitions.Customers;

public partial class CustomerTransactionUiDefinitions : SubCRUDDefinition<CustomerTransaction>
{
    public override string? Icon => "fa fa-exchange";
    public override string SubjectId => "Sub";
    public override void SubIndexViewModel()
    {
        AddColumns(nameof(CustomerTransaction.Point),
                        nameof(CustomerTransaction.Balance),
                        nameof(CustomerTransaction.CreateDate),
                        nameof(CustomerTransaction.CustomerTenant)
                        );
    }

    public override void SubViewModel()
    {
        AddFields(nameof(CustomerTransaction.Point),
                       nameof(CustomerTransaction.Balance),
                       nameof(CustomerTransaction.CreateDate),
                       nameof(CustomerTransaction.CustomerTenant),
                       nameof(CustomerTransaction.TransactionType),
                       nameof(CustomerTransaction.Debit),
                       nameof(CustomerTransaction.Credit),
                       nameof(CustomerTransaction.ExpirationDate),
                       nameof(CustomerTransaction.IsExpired),
                       nameof(CustomerTransaction.ExpiredDate),
                       nameof(CustomerTransaction.IsSpent),
                       nameof(CustomerTransaction.VisitedAt),
                       nameof(CustomerTransaction.EventLog),
                       nameof(CustomerTransaction.Promotion),
                       nameof(CustomerTransaction.PromotionAction)
                       );
    }
}
