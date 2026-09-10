namespace Hyper.AdminPanel.Domain.UiDefinitions.Promotions.PromotionsData;

public class PromotionParticipationUiDefinitions : CRUDDefinition<PromotionParticipation>
{
    public override string? Icon => "fa fa-user-plus";
    
    protected override void IndexFormViewModel()
    {
        AddColumns(
            nameof(PromotionParticipation.Promotion),
            nameof(PromotionParticipation.CustomerTenant),
            nameof(PromotionParticipation.Status),
            nameof(PromotionParticipation.StartDate),
            nameof(PromotionParticipation.ParticipationCount),
            nameof(PromotionParticipation.IsCompleted)
        );
    }
    
    protected override void CUDFormsViewModel()
    {
        AddField(nameof(PromotionParticipation.Promotion), eControlPropertyId.ReadOnly);
        AddField(nameof(PromotionParticipation.CustomerTenant), eControlPropertyId.ReadOnly);
        AddField(nameof(PromotionParticipation.Status));
        AddField(nameof(PromotionParticipation.StartDate), eControlPropertyId.ReadOnly);
        AddField(nameof(PromotionParticipation.EndDate), eControlPropertyId.ReadOnly);
        AddField(nameof(PromotionParticipation.ParticipationCount), eControlPropertyId.ReadOnly);
        AddField(nameof(PromotionParticipation.IsCompleted), eControlPropertyId.ReadOnly);
        AddField(nameof(PromotionParticipation.CompletedDate), eControlPropertyId.ReadOnly);
    }
}

