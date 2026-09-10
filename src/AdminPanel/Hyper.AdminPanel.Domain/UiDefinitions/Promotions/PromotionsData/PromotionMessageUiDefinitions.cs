namespace Hyper.AdminPanel.Domain.UiDefinitions.Promotions.PromotionsData;

public class PromotionMessageDefinitions : CRUDDefinition<PromotionMessage>
{
    public override string? Icon => "fa fa-envelope";
    
    protected override void IndexFormViewModel()
    {
        AddColumns(nameof(PromotionMessage.Promotion),
                        nameof(PromotionMessage.Subject),
                        nameof(PromotionMessage.Type),
                        nameof(PromotionMessage.Status),
                        nameof(PromotionMessage.SentDate),
                        nameof(PromotionMessage.SentCount),
                        nameof(PromotionMessage.DeliveredCount),
                        nameof(PromotionMessage.ReadCount),
                        nameof(PromotionMessage.DeliveryRate),
                        nameof(PromotionMessage.ReadRate)
                        );
    }

    protected override void CUDFormsViewModel()
    {
        AddFields(nameof(PromotionMessage.Promotion),
                       nameof(PromotionMessage.Subject),
                       nameof(PromotionMessage.Content),
                       nameof(PromotionMessage.SendMethod),
                       nameof(PromotionMessage.Type),
                       nameof(PromotionMessage.Status),
                       nameof(PromotionMessage.Priority),
                       nameof(PromotionMessage.ScheduledTime),
                       nameof(PromotionMessage.MaxRetries)
                       );
    }
}
