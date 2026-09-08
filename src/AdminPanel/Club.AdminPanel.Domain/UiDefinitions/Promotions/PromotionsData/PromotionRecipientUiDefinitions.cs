namespace Hyper.AdminPanel.Domain.UiDefinitions.Promotions.PromotionsData;

public class PromotionRecipientDefinitions : SubCRUDDefinition<PromotionRecipient>
{
    public override string? Icon => "fa fa-users";
    
    protected override void IndexFormViewModel()
    {
        AddColumns(nameof(PromotionRecipient.Promotion),
                        nameof(PromotionRecipient.CustomerTenant),
                        nameof(PromotionRecipient.Status),
                        nameof(PromotionRecipient.SentDate),
                        nameof(PromotionRecipient.DeliveredDate),
                        nameof(PromotionRecipient.ReadDate),
                        nameof(PromotionRecipient.EngagementScore),
                        nameof(PromotionRecipient.ClickCount),
                        nameof(PromotionRecipient.ResponseCount)
                        );
    }

    protected override void CUDFormsViewModel()
    {
        AddFields(nameof(PromotionRecipient.Promotion),
                       nameof(PromotionRecipient.CustomerTenant),
                       nameof(PromotionRecipient.Status),
                       nameof(PromotionRecipient.SentDate),
                       nameof(PromotionRecipient.DeliveredDate),
                       nameof(PromotionRecipient.ReadDate),
                       nameof(PromotionRecipient.ErrorMessage),
                       nameof(PromotionRecipient.EngagementScore),
                       nameof(PromotionRecipient.ClickCount),
                       nameof(PromotionRecipient.ResponseCount),
                       nameof(PromotionRecipient.ResponseTime),
                       nameof(PromotionRecipient.ResponseType),
                       nameof(PromotionRecipient.ResponseText)
                       );
    }

    public override string SubjectId => "Sub";
    public override void SubIndexViewModel()
    {
        AddColumns(nameof(PromotionRecipient.CustomerTenant),
                        nameof(PromotionRecipient.Status),
                        nameof(PromotionRecipient.SentDate),
                        nameof(PromotionRecipient.DeliveredDate),
                        nameof(PromotionRecipient.EngagementScore),
                        nameof(PromotionRecipient.ClickCount)
                        );
    }

    public override void SubViewModel()
    {
        AddFields(nameof(PromotionRecipient.CustomerTenant),
                       nameof(PromotionRecipient.Status),
                       nameof(PromotionRecipient.SentDate),
                       nameof(PromotionRecipient.DeliveredDate),
                       nameof(PromotionRecipient.ReadDate),
                       nameof(PromotionRecipient.ErrorMessage),
                       nameof(PromotionRecipient.EngagementScore),
                       nameof(PromotionRecipient.ClickCount),
                       nameof(PromotionRecipient.ResponseCount),
                       nameof(PromotionRecipient.ResponseTime),
                       nameof(PromotionRecipient.ResponseType),
                       nameof(PromotionRecipient.ResponseText)
                       );
    }
}
