using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Rewards.Queries;

public record GetPurchasedRewardByIdQuery : IRequest<GetPurchasedRewardByIdQueryResponse>
{
    public string Id { get; set; } = null!;
}

public record GetPurchasedRewardByIdQueryResponse
{
    public PurchasedRewardDto PurchasedReward { get; set; } = null!;
}

public class GetPurchasedRewardByIdQueryHandler(
    IRewardService rewardService,
    ICustomerRequesterUser requesterUser)
    : IRequestHandler<GetPurchasedRewardByIdQuery, GetPurchasedRewardByIdQueryResponse>
{
    public async Task<GetPurchasedRewardByIdQueryResponse> Handle(GetPurchasedRewardByIdQuery request, CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;
        
        // دریافت لیست پاداش‌های خریداری شده و یافتن پاداش مورد نظر
        var allPurchased = await rewardService.GetPurchasedRewardsAsync(customerId, 1, 1000, cancellationToken);
        
        var purchasedReward = allPurchased.Items.FirstOrDefault(pr => pr.Id.ToString() == request.Id);
        
        if (purchasedReward == null)
        {
            throw new InvalidOperationException("پاداش خریداری شده یافت نشد");
        }
        
        return new GetPurchasedRewardByIdQueryResponse
        {
            PurchasedReward = new PurchasedRewardDto
            {
                Id = purchasedReward.Id,
                RewardId = 1,
                RewardTitle = purchasedReward.RewardTitle,
                RewardImageUrl = null,
                PurchasedAt = purchasedReward.PurchasedAt,
                Status = purchasedReward.Status,
                SerialNumber = purchasedReward.SerialNumber,
                QrCode = purchasedReward.QrCode,
                ExpirationDate = null,
                UsedDate = null,
                PointsSpent = purchasedReward.PointsSpent
            }
        };
    }
}

