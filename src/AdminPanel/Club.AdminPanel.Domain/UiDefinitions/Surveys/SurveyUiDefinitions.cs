namespace Hyper.AdminPanel.Domain.UiDefinitions.Surveys;

public partial class SurveyUiDefinitions : CRUDDefinition<Survey>
{
    private static readonly List<string> DefaultRoles =
    [
        Neo.Domain.Constants.Roles.Admin,
        HyperRoles.Manager,
        HyperRoles.MarketingManager,
        HyperRoles.Analyst
    ];

    public override List<string>? Roles => DefaultRoles;
    public override string? Icon => "fa fa-clipboard";

    protected override void IndexFormViewModel()
    {
        AddColumns(
            nameof(Survey.Promotion),
            nameof(Survey.Title),
            nameof(Survey.SurveyType),
            nameof(Survey.IsActive),
            nameof(Survey.StartDate),
            nameof(Survey.EndDate),
            nameof(Survey.TotalParticipants),
            nameof(Survey.ParticipationPoints),
            nameof(Survey.CorrectAnswerPoints)
        );
        AddSubjectColumn<SurveyItems>();
    }

    protected override void CUDFormsViewModel()
    {
        AddFields(
            nameof(Survey.Promotion),
            nameof(Survey.Title),
            nameof(Survey.Description),
            nameof(Survey.SurveyType),
            nameof(Survey.Product),
            nameof(Survey.IsActive),
            nameof(Survey.StartDate),
            nameof(Survey.EndDate),
            nameof(Survey.AllowMultipleSelection),
            nameof(Survey.ShowResults),
            nameof(Survey.ParticipationPoints),
            nameof(Survey.CorrectAnswerPoints)
        );
    }

    public class SurveyItems : EditForm, ISubjectFormDefinition
    {
        public override string SubjectId => nameof(SurveyItems);
        public override string Name => "گزینه‌های نظرسنجی";
        public override List<string>? Roles => DefaultRoles;

        protected override void ViewModel()
        {
            AddSubTable(nameof(SurveyItem), nameof(SurveyItem.Survey), "Sub",
                "گزینه‌های نظرسنجی", null, false, ContainerControl.MultiTab);
        }
    }
}
