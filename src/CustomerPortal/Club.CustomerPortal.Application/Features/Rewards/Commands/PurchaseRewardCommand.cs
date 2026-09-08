using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Rewards.Commands;

public record PurchaseRewardCommand : IRequest<PurchaseRewardCommandResponse>
{
    public required string RewardId { get; set; }
    public int Quantity { get; set; } = 1;
}

public record PurchaseRewardCommandResponse
{
    public PurchasedRewardDto PurchasedReward { get; set; } = null!;
}

public class PurchaseRewardCommandValidator : AbstractValidator<PurchaseRewardCommand>
{
    public PurchaseRewardCommandValidator()
    {
        RuleFor(x => x.RewardId).NotEmpty().WithMessage("شناسه پاداش الزامی است");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("تعداد باید بیشتر از صفر باشد");
    }
}

public class PurchaseRewardCommandHandler(
    IRewardService rewardService,
    ICustomerRequesterUser requesterUser,
    ILogger<PurchaseRewardCommandHandler> logger)
    : IRequestHandler<PurchaseRewardCommand, PurchaseRewardCommandResponse>
{
    public async Task<PurchaseRewardCommandResponse> Handle(PurchaseRewardCommand request, CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;
        
        var result = await rewardService.PurchaseRewardAsync(
            customerId,
            int.Parse(request.RewardId),
            request.Quantity,
            cancellationToken);
        
        if (!result.Success)
        {
            throw new InvalidOperationException(result.Message);
        }
        
        logger.LogInformation("Reward purchased successfully by customer {CustomerId}: Reward {RewardId} x {Quantity}",
            customerId, request.RewardId, request.Quantity);
        
        return new PurchaseRewardCommandResponse
        {
            PurchasedReward = new PurchasedRewardDto
            {
                Id = 0, // Will be set by database
                RewardId = int.Parse(request.RewardId),
                RewardTitle = "پاداش",
                RewardImageUrl = null,
                PurchasedAt = DateTime.UtcNow,
                Status = "Active",
                SerialNumber = result.SerialNumber,
                QrCode = result.QrCode,
                ExpirationDate = null,
                UsedDate = null,
                PointsSpent = 0 // TODO: get from result
            }
        };
    }
}

