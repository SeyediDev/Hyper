namespace Hyper.AdminPanel.Domain.UiDefinitions.Promotions;

public class PromotionMessageDefinitions : SubCRUDDefinition<PromotionMessage>
{
    protected override void IndexFormViewModel(FormDefinition form)
    {
        form.AddColumns(nameof(PromotionMessage.Promotion),
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
                       nameof(PromotionMessage.Type),
                       nameof(PromotionMessage.Status),
                       nameof(PromotionMessage.Priority),
                       nameof(PromotionMessage.ScheduledTime),
                       nameof(PromotionMessage.MaxRetries)
                       );
    }

    public override string SubjectId => "Sub";
    public override void SubIndexViewModel()
    {
        AddColumns(nameof(PromotionMessage.Subject),
                        nameof(PromotionMessage.Type),
                        nameof(PromotionMessage.Status),
                        nameof(PromotionMessage.SentDate),
                        nameof(PromotionMessage.SentCount),
                        nameof(PromotionMessage.DeliveryRate)
                        );
    }

    public override void SubViewModel()
    {
        AddFields(nameof(PromotionMessage.Subject),
                       nameof(PromotionMessage.Content),
                       nameof(PromotionMessage.Type),
                       nameof(PromotionMessage.Status),
                       nameof(PromotionMessage.Priority),
                       nameof(PromotionMessage.ScheduledTime),
                       nameof(PromotionMessage.MaxRetries)
                       );
    }
}
