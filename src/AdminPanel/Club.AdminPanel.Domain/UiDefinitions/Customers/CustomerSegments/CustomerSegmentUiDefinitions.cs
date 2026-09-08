namespace Club.AdminPanel.Domain.UiDefinitions.Customers.CustomerSegments;

public partial class CustomerSegmentUiDefinitions : SubCRUDDefinition<CustomerSegment>
{
    protected override void IndexFormViewModel(FormDefinition form)
    {
        form.AddColumns(nameof(CustomerSegment.Title),
                        nameof(CustomerSegment.IsActive),
                        nameof(CustomerSegment.EstimatedSize),
                        nameof(CustomerSegment.Tenant));
        form.AddSubjectColumn<CustomerSegmentConditions>();
    }
    protected override void CUDFormsViewModel(CUDForm form)
    {
        form.AddFields(nameof(CustomerSegment.Tenant),
                       nameof(CustomerSegment.Title),
                       nameof(CustomerSegment.IsActive),
                       nameof(CustomerSegment.EstimatedSize),
                       nameof(CustomerSegment.Description));
    }
    public override string SubjectId => "Sub";
    public override void SubIndexViewModel(FormDefinition form)
    {
        form.AddColumns(nameof(CustomerSegment.Title),
                        nameof(CustomerSegment.IsActive),
                        nameof(CustomerSegment.EstimatedSize));
    }

    public override void SubViewModel(FormDefinition form)
    {
        form.AddFields(nameof(CustomerSegment.Title),
                       nameof(CustomerSegment.IsActive),
                       nameof(CustomerSegment.EstimatedSize),
                       nameof(CustomerSegment.Description));
    }

    public class CustomerSegmentConditions : EditForm, ISubjectFormDefinition
    {
        public override string SubjectId => nameof(CustomerSegmentConditions);
        public override string Name => "شرط‌های جامعه‌سازی/بازار";

        protected override void ViewModel()
        {
            AddField(nameof(CustomerSegment.Title), eControlPropertyId.ReadOnly);
            AddSubTable(nameof(CustomerSegmentKindCondition), nameof(CustomerSegmentKindCondition.CustomerSegment), null,
                "شرط‌های جامعه‌سازی/بازار", null, false);
        }
    }
}
