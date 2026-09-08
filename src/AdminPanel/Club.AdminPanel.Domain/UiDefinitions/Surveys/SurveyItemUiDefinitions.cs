namespace Hyper.AdminPanel.Domain.UiDefinitions.Surveys;

public class SurveyItemUiDefinitions : SubCRUDDefinition<SurveyItem>
{
    public override string SubjectId => "Sub";

    public override void SubIndexViewModel()
    {
        AddColumns(
            nameof(SurveyItem.OptionText),
            nameof(SurveyItem.DisplayOrder),
            nameof(SurveyItem.IsCorrectAnswer),
            nameof(SurveyItem.VoteCount),
            nameof(SurveyItem.Picture)
        );
    }

    public override void SubViewModel()
    {
        AddFields(
            nameof(SurveyItem.OptionText),
            nameof(SurveyItem.DisplayOrder),
            nameof(SurveyItem.IsCorrectAnswer),
            nameof(SurveyItem.Picture)
        );
    }
}
