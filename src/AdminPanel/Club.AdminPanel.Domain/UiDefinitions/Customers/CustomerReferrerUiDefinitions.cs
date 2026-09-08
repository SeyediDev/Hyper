namespace Club.AdminPanel.Domain.UiDefinitions.Customers;

/// <summary>
/// تعاریف UI برای موجودیت معرفی مشتری
/// </summary>
public class CustomerReferrerUiDefinitions : SubCRUDDefinition<CustomerReferrer>
{
    public override string SubjectId => "Sub";
    public override void SubIndexViewModel(FormDefinition form)
    {
        form.AddColumns(nameof(CustomerReferrer.Tenant),
        nameof(CustomerReferrer.ReferrerCode),
        nameof(CustomerReferrer.ReferrerCustomerTenant),
        nameof(CustomerReferrer.ReferredCustomerTenant));
    }

    public override void SubViewModel(FormDefinition form)
    {
        form.AddFields(nameof(CustomerReferrer.Tenant),
        nameof(CustomerReferrer.ReferrerCode),
        nameof(CustomerReferrer.ReferrerCustomerTenant),
        nameof(CustomerReferrer.ReferredCustomerTenant),
        nameof(CustomerReferrer.EventLog),
        nameof(CustomerReferrer.Promotion),
        nameof(CustomerReferrer.PromotionAction)
        );
    }
}