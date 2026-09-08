using Hyper.Domain.Features.Rewards;

namespace Hyper.Application.Features.Hyper.Commands;

public record PurchaseAwardCommand : IRequest<PurchaseAwardCommandResponse>
{
    public string CustomerId { get; set; } = null!;
    public int AwardId { get; set; }
}
public record PurchaseAwardCommandResponse
{
    public string CustomerId { get; set; } = null!;
    public int AwardId { get; set; }
}

public class PurchaseAwardCommandValidator : AbstractValidator<PurchaseAwardCommand>
{
    public PurchaseAwardCommandValidator(IMultiLingualService multiLingual)
    {
    }
}

public class PurchaseAwardCommandHandler(IRewardAssetService awardAssetService)
    : IRequestHandler<PurchaseAwardCommand, PurchaseAwardCommandResponse>
{
    public async Task<PurchaseAwardCommandResponse> Handle(PurchaseAwardCommand request, CancellationToken cancellationToken)
    {
        PurchaseRewardResponse response = await awardAssetService.PurchaseReward(
            new()
            { 
                Customer= request.CustomerId, 
                RewardId= request.AwardId 
            }, cancellationToken);
        return response.Adapt<PurchaseAwardCommandResponse>();
    }
}
