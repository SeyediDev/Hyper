namespace Hyper.AdminPanel.Domain.UiDefinitions.CustomerTenants;

public class CustomerReferrerUiDefinitions : SubCRUDDefinition<CustomerReferrer>
{
    public override string? Icon => "fa fa-share-alt";
    public override string SubjectId => "Sub";
    public override void SubIndexViewModel()
    {
        AddColumns(
             nameof(CustomerReferrer.ReferrerCode)
           , nameof(CustomerReferrer.ReferrerCustomerTenant)
           , nameof(CustomerReferrer.ReferredCustomerTenant));
    }

    public override void SubViewModel()
    {
        AddFields(
            nameof(CustomerReferrer.ReferrerCode),
            nameof(CustomerReferrer.ReferrerCustomerTenant),
            nameof(CustomerReferrer.ReferredCustomerTenant),
            nameof(CustomerReferrer.EventLog),
            nameof(CustomerReferrer.Promotion),
            nameof(CustomerReferrer.PromotionAction)
        );
    }
}