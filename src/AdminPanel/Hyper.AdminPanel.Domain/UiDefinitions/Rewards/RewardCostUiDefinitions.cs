namespace Hyper.AdminPanel.Domain.UiDefinitions.Rewards;

public class RewardCostUiDefinitions : SubCRUDDefinition<RewardCost>
{
    public override string SubjectId => "Sub";
    public override void SubIndexViewModel()
    {
        AddColumns(nameof(RewardCost.Point),
                        nameof(RewardCost.Amount),
                        nameof(RewardCost.PointLevel),
                        nameof(RewardCost.Plan));
    }

    public override void SubViewModel()
    {
        AddFields(nameof(RewardCost.Point),
                       nameof(RewardCost.Amount),
                       nameof(RewardCost.PointLevel),
                       nameof(RewardCost.Plan)
                       );
    }
}