namespace Hyper.AdminPanel.Domain.UiDefinitions.Rewards;

public class RewardAssetUiDefinitions : Sub2CRUDDefinition<RewardAsset>
{
    public override string? Icon => "fa fa-ticket";
    
    protected override void IndexFormViewModel()
    {
        AddColumns(nameof(RewardAsset.Reward),
                        nameof(RewardAsset.CustomerTenant),
                        nameof(RewardAsset.Serial),
                        nameof(RewardAsset.Quantity),
                        nameof(RewardAsset.ConsumedQuantity),
                        nameof(RewardAsset.RemainingQuantity)
                        );
    }
    protected override void CUDFormsViewModel()
    {
        AddFields(nameof(RewardAsset.Reward),
                       nameof(RewardAsset.CustomerTenant),
                       nameof(RewardAsset.Serial),
                       nameof(RewardAsset.Quantity),
                       nameof(RewardAsset.ConsumedQuantity),
                       nameof(RewardAsset.RemainingQuantity),
                       nameof(RewardAsset.EventLog),
                       nameof(RewardAsset.Promotion),
                       nameof(RewardAsset.PromotionAction)
                       );
    }
    
    public override string SubjectId => "SubCustomer";
    public override void SubIndexViewModel()
    {
        AddColumns(nameof(RewardAsset.Reward),
                        nameof(RewardAsset.Serial),
                        nameof(RewardAsset.Quantity),
                        nameof(RewardAsset.ConsumedQuantity),
                        nameof(RewardAsset.RemainingQuantity)
                        );
    }

    public override void SubViewModel()
    {
        AddFields(nameof(RewardAsset.Reward),
                       nameof(RewardAsset.Serial),
                       nameof(RewardAsset.Quantity),
                       nameof(RewardAsset.ConsumedQuantity),
                       nameof(RewardAsset.RemainingQuantity),
                       nameof(RewardAsset.EventLog),
                       nameof(RewardAsset.Promotion),
                       nameof(RewardAsset.PromotionAction)
                       );
    }

    public override string SubjectId2 => "SubReward";
    public override void Sub2IndexViewModel(FormDefinition form)
    {
        AddColumns(nameof(RewardAsset.CustomerTenant),
                        nameof(RewardAsset.Serial),
                        nameof(RewardAsset.Quantity),
                        nameof(RewardAsset.ConsumedQuantity),
                        nameof(RewardAsset.RemainingQuantity)
                        );
    }

    public override void Sub2ViewModel(FormDefinition form)
    {
        AddFields(nameof(RewardAsset.CustomerTenant),
                       nameof(RewardAsset.Serial),
                       nameof(RewardAsset.Quantity),
                       nameof(RewardAsset.ConsumedQuantity),
                       nameof(RewardAsset.RemainingQuantity),
                       nameof(RewardAsset.EventLog),
                       nameof(RewardAsset.Promotion),
                       nameof(RewardAsset.PromotionAction)
                       );
    }
}
