namespace Hyper.AdminPanel.Domain.UiDefinitions.Customers;

public class CustomerParameterValueUiDefinitions : SubCRUDDefinition<CustomerParameterValue>
{
    public override string SubjectId => "Sub";
    public override void SubIndexViewModel()
    {
        AddColumns(nameof(CustomerParameterValue.CustomerTenant), nameof(CustomerParameterValue.Parameter), nameof(CustomerParameterValue.Value));
    }

    public override void SubViewModel()
    {
        AddField(nameof(CustomerParameterValue.CustomerTenant));
        AddField(nameof(CustomerParameterValue.Parameter));
        AddField(nameof(CustomerParameterValue.Value));
        AddField(nameof(CustomerParameterValue.EventLog));
    }
}
