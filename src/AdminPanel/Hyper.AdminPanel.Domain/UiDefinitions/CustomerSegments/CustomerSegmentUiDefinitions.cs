namespace Hyper.AdminPanel.Domain.UiDefinitions.Customers.CustomerSegments;

public partial class CustomerSegmentUiDefinitions : SubCRUDDefinition<CustomerSegment>
{
    public override string? Icon => "fa fa-object-group";
    
    protected override void IndexFormViewModel()
    {
        AddColumns(nameof(CustomerSegment.Title),
                        nameof(CustomerSegment.Key),
                        nameof(CustomerSegment.Kind),
                        nameof(CustomerSegment.JoinMode),
                        nameof(CustomerSegment.IsActive),
                        nameof(CustomerSegment.EstimatedSize),
                        nameof(CustomerSegment.Tenant));
    }
    protected override void CUDFormsViewModel()
    {
        AddFields(nameof(CustomerSegment.Tenant),
                       nameof(CustomerSegment.Title),
                        nameof(CustomerSegment.Key),
                       nameof(CustomerSegment.Kind),
                       nameof(CustomerSegment.JoinMode),
                        nameof(CustomerSegment.Constraint),
                       nameof(CustomerSegment.IsActive),
                       nameof(CustomerSegment.EstimatedSize),
                       nameof(CustomerSegment.Description));
    }
    public override string SubjectId => "Sub";
    public override void SubIndexViewModel()
    {
        AddColumns(nameof(CustomerSegment.Title),
                        nameof(CustomerSegment.Key),
                        nameof(CustomerSegment.Kind),
                       nameof(CustomerSegment.JoinMode),
                        nameof(CustomerSegment.IsActive),
                        nameof(CustomerSegment.EstimatedSize));
    }

    public override void SubViewModel()
    {
        AddFields(nameof(CustomerSegment.Title),
                        nameof(CustomerSegment.Key),
                       nameof(CustomerSegment.Kind),
                       nameof(CustomerSegment.JoinMode),
                       nameof(CustomerSegment.Constraint),
                       nameof(CustomerSegment.IsActive),
                       nameof(CustomerSegment.EstimatedSize),
                       nameof(CustomerSegment.Description));
    }
}
