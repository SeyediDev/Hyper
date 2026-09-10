using Hyper.Domain.Features.Rewards;

namespace Hyper.Application.Features.Hyper.Commands;

public record PurchaseRewardCommand : IRequest<PurchaseRewardCommandResponse>
{
    public string CustomerId { get; set; } = null!;
    public int RewardId { get; set; }
}
public record PurchaseRewardCommandResponse
{
    public string CustomerId { get; set; } = null!;
    public int RewardId { get; set; }
}

public class PurchaseRewardCommandValidator : AbstractValidator<PurchaseRewardCommand>
{
    public PurchaseRewardCommandValidator(IMultiLingualService multiLingual)
    {
    }
}

public class PurchaseRewardCommandHandler(IRewardAssetService rewardAssetService)
    : IRequestHandler<PurchaseRewardCommand, PurchaseRewardCommandResponse>
{
    public async Task<PurchaseRewardCommandResponse> Handle(PurchaseRewardCommand request, CancellationToken cancellationToken)
    {
        PurchaseRewardResponse response = await rewardAssetService.PurchaseReward(
            new()
            { 
                Customer= request.CustomerId, 
                RewardId= request.RewardId 
            }, cancellationToken);
        return response.Adapt<PurchaseRewardCommandResponse>();
    }
}
