namespace Hyper.AdminPanel.Domain.UiDefinitions.CustomerTenants;

public class CustomerPointLevelUiDefinitions : SubCRUDDefinition<CustomerPointLevel>
{
    public override string? Icon => "fa fa-signal";
    
    protected override void IndexFormViewModel()
    {
        AddColumns(
            nameof(CustomerPointLevel.CustomerTenant),
            nameof(CustomerPointLevel.PointLevel));
    }

    protected override void CUDFormsViewModel()
    {
        AddFields(
            nameof(CustomerPointLevel.CustomerTenant),
            nameof(CustomerPointLevel.PointLevel),
            nameof(CustomerPointLevel.EventLog)
            );
    }
    public override string SubjectId => "Sub";
    public override void SubIndexViewModel()
    {
        AddColumns(nameof(CustomerPointLevel.PointLevel));
    }

    public override void SubViewModel()
    {
        AddFields(nameof(CustomerPointLevel.PointLevel),
                        nameof(CustomerPointLevel.EventLog)
                        );
    }
}
