using Hyper.Domain.Entities.Channels;

namespace Hyper.AdminPanel.Domain.UiDefinitions.Events;

public class EventChannelValidEventTypeUiDefinitions : SubCRUDDefinition<EventChannelValidEvent>
{
    public override string SubjectId => "Sub";
    public override void SubIndexViewModel()
    {
        AddColumns(nameof(EventChannelValidEvent.EventType));
    }

    public override void SubViewModel()
    {
        AddFields(nameof(EventChannelValidEvent.EventType));
    }
}
