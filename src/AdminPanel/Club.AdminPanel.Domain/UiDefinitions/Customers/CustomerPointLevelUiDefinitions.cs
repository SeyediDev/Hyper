namespace Club.AdminPanel.Domain.UiDefinitions.Customers;

public class CustomerPointLevelUiDefinitions : SubCRUDDefinition<CustomerPointLevel>
{
    protected override void IndexFormViewModel(FormDefinition form)
    {
        form.AddColumns(
            nameof(CustomerPointLevel.CustomerTenant),
            nameof(CustomerPointLevel.PointLevel));
    }

    protected override void CUDFormsViewModel(CUDForm form)
    {
        form.AddFields(
            nameof(CustomerPointLevel.CustomerTenant),
            nameof(CustomerPointLevel.PointLevel),
            nameof(CustomerPointLevel.EventLog)
            );
    }
    public override string SubjectId => "Sub";
    public override void SubIndexViewModel(FormDefinition form)
    {
        form.AddColumns(nameof(CustomerPointLevel.PointLevel));
    }

    public override void SubViewModel(FormDefinition form)
    {
        form.AddFields(nameof(CustomerPointLevel.PointLevel),
                        nameof(CustomerPointLevel.EventLog)
                        );
    }
}
