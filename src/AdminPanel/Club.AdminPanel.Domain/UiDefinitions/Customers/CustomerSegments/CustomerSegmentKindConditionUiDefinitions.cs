namespace Club.AdminPanel.Domain.UiDefinitions.Customers.CustomerSegments;

public class CustomerSegmentKindConditionDefinitions : SubCRUDDefinition<CustomerSegmentKindCondition>
{
    protected override void IndexFormViewModel(FormDefinition form)
    {
        form.AddColumns(nameof(CustomerSegmentKindCondition.CustomerSegment),
                        nameof(CustomerSegmentKindCondition.Kind),
                        nameof(CustomerSegmentKindCondition.Title),
                        nameof(CustomerSegmentKindCondition.Priority),
                        nameof(CustomerSegmentKindCondition.ConditionGroup),
                        nameof(CustomerSegmentKindCondition.CompareWith),
                        nameof(CustomerSegmentKindCondition.Value)
                        );
    }

    protected override void CUDFormsViewModel(CUDForm form)
    {
        form.AddFields(nameof(CustomerSegmentKindCondition.CustomerSegment),
                       nameof(CustomerSegmentKindCondition.Kind),
                       nameof(CustomerSegmentKindCondition.Title),
                       nameof(CustomerSegmentKindCondition.Description),
                       nameof(CustomerSegmentKindCondition.Priority),
                       nameof(CustomerSegmentKindCondition.ConditionGroup),
                       nameof(CustomerSegmentKindCondition.Constraint),
                       nameof(CustomerSegmentKindCondition.CompareWith),
                       nameof(CustomerSegmentKindCondition.Value),
                       nameof(CustomerSegmentKindCondition.MinScore),
                       nameof(CustomerSegmentKindCondition.MaxScore),
                       nameof(CustomerSegmentKindCondition.ErrorMessage)
                       );
    }

    public override string SubjectId => "Sub";
    public override void SubIndexViewModel(FormDefinition form)
    {
        form.AddColumns(nameof(CustomerSegmentKindCondition.Kind),
                        nameof(CustomerSegmentKindCondition.Title),
                        nameof(CustomerSegmentKindCondition.Priority),
                        nameof(CustomerSegmentKindCondition.CompareWith),
                        nameof(CustomerSegmentKindCondition.Value)
                        );
    }

    public override void SubViewModel(FormDefinition form)
    {
        form.AddFields(nameof(CustomerSegmentKindCondition.Kind),
                       nameof(CustomerSegmentKindCondition.Title),
                       nameof(CustomerSegmentKindCondition.Description),
                       nameof(CustomerSegmentKindCondition.Priority),
                       nameof(CustomerSegmentKindCondition.ConditionGroup),
                       nameof(CustomerSegmentKindCondition.Constraint),
                       nameof(CustomerSegmentKindCondition.CompareWith),
                       nameof(CustomerSegmentKindCondition.Value),
                       nameof(CustomerSegmentKindCondition.MinScore),
                       nameof(CustomerSegmentKindCondition.MaxScore),
                       nameof(CustomerSegmentKindCondition.ErrorMessage)
                       );
    }
}
