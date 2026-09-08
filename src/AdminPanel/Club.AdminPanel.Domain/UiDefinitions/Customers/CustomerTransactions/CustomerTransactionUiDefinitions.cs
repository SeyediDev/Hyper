namespace Club.AdminPanel.Domain.UiDefinitions.Customers;

public partial class CustomerTransactionUiDefinitions : SubCRUDDefinition<CustomerTransaction>
{
    public override string SubjectId => "Sub";
    public override void SubIndexViewModel(FormDefinition form)
    {
        form.AddColumns(nameof(CustomerTransaction.Point),
                        nameof(CustomerTransaction.Balance),
                        nameof(CustomerTransaction.CreateDate),
                        nameof(CustomerTransaction.Tenant)
                        );
    }

    public override void SubViewModel(FormDefinition form)
    {
        form.AddFields(nameof(CustomerTransaction.Point),
                       nameof(CustomerTransaction.Balance),
                       nameof(CustomerTransaction.CreateDate),
                       nameof(CustomerTransaction.Tenant),
                       nameof(CustomerTransaction.TransactionType),
                       nameof(CustomerTransaction.Debit),
                       nameof(CustomerTransaction.Credit),
                       nameof(CustomerTransaction.VisitedAt),
                       nameof(CustomerTransaction.EventLog),
                       nameof(CustomerTransaction.Promotion),
                       nameof(CustomerTransaction.PromotionAction)
                       );
    }
}
