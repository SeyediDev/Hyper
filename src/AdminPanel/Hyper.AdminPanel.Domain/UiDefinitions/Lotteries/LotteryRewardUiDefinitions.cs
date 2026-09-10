using Hyper.Domain.Entities.Lotteries;

namespace Hyper.AdminPanel.Domain.UiDefinitions.Lotteries;

public class LotteryRewardDefinitions : CRUDDefinition<LotteryReward>
{
    public override string? Icon => "fa fa-trophy";
    
    protected override void IndexFormViewModel()
    {
        AddColumns(nameof(LotteryReward.Lottery),
            nameof(LotteryReward.Reward),
            nameof(LotteryReward.Amount),
            nameof(LotteryReward.WinRate),
            nameof(LotteryReward.MaxDistributionCount),
            nameof(LotteryReward.DistributedCount),
            nameof(LotteryReward.DisplayOrder),
            nameof(LotteryReward.IsActive)
            );
    }

    protected override void CUDFormsViewModel()
    {
        AddFields(nameof(LotteryReward.Lottery),
            nameof(LotteryReward.Reward),
            nameof(LotteryReward.Amount),
            nameof(LotteryReward.WinRate),
            nameof(LotteryReward.MaxDistributionCount),
            nameof(LotteryReward.DistributedCount),
            nameof(LotteryReward.DisplayOrder),
            nameof(LotteryReward.IsActive)
            );
    }
}