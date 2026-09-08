namespace Hyper.AdminPanel.Domain.UiDefinitions.Promotions.PromotionsData;

public class PromotionEventReceivedUiDefinitions : CRUDDefinition<PromotionEventReceived>
{
    public override string? Icon => "fa fa-inbox";
    
    protected override void IndexFormViewModel()
    {
        AddColumns(
            nameof(PromotionEventReceived.PromotionParticipation),
            nameof(PromotionEventReceived.PromotionTrigger),
            nameof(PromotionEventReceived.EventLog),
            nameof(PromotionEventReceived.ReceivedDate),
            nameof(PromotionEventReceived.SequenceNumber)
        );
    }
    
    protected override void CUDFormsViewModel()
    {
        AddField(nameof(PromotionEventReceived.PromotionParticipation), eControlPropertyId.ReadOnly);
        AddField(nameof(PromotionEventReceived.PromotionTrigger), eControlPropertyId.ReadOnly);
        AddField(nameof(PromotionEventReceived.EventLog), eControlPropertyId.ReadOnly);
        AddField(nameof(PromotionEventReceived.ReceivedDate), eControlPropertyId.ReadOnly);
        AddField(nameof(PromotionEventReceived.SequenceNumber), eControlPropertyId.ReadOnly);
        AddField(nameof(PromotionEventReceived.FlowType), eControlPropertyId.ReadOnly);
        AddField(nameof(PromotionEventReceived.ParallelGroupId), eControlPropertyId.ReadOnly);
    }
}