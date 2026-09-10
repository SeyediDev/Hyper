namespace Hyper.AdminPanel.Domain.UiDefinitions.Events;

public class EventTypeParameterUiDefinitions : SubCRUDDefinition<EventTypeParameter>
{
    public override string SubjectId => "Sub";
    public override void SubIndexViewModel()
    {
        AddColumns(nameof(EventTypeParameter.Title),
                        nameof(EventTypeParameter.Key),
                        nameof(EventTypeParameter.IsOptional),
                        nameof(EventTypeParameter.CustomerParameter)
                        );
    }

    public override void SubViewModel()
    {
        AddFields(nameof(EventTypeParameter.Title),
                        nameof(EventTypeParameter.Key),
                        nameof(EventTypeParameter.IsOptional),
                        nameof(EventTypeParameter.CreatedBySystem),
                        nameof(EventTypeParameter.CustomerParameter)
                        );
    }
}
