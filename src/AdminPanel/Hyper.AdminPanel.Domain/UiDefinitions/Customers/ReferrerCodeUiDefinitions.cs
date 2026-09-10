namespace Hyper.AdminPanel.Domain.UiDefinitions.Customers;

public class ReferrerCodeUiDefinitions : SubCRUDDefinition<ReferrerCode>
{
    public override string SubjectId => "Sub";
    public override void SubIndexViewModel()
    {
        AddColumns(
            nameof(ReferrerCode.CustomerTenant),
            nameof(ReferrerCode.Code));
    }

    public override void SubViewModel()
    {
        AddFields(
            nameof(ReferrerCode.CustomerTenant),
            nameof(ReferrerCode.Code));
    }
}