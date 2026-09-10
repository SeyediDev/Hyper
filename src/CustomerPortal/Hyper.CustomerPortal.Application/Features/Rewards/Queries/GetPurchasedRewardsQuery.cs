using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Rewards.Queries;

public record GetPurchasedRewardsQuery : IRequest<GetPurchasedRewardsQueryResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Status { get; set; }
}

public record GetPurchasedRewardsQueryResponse
{
    public PaginatedList<PurchasedRewardDto> PurchasedRewards { get; set; } = null!;
}

public class GetPurchasedRewardsQueryHandler(
    IRewardService rewardService,
    ICustomerRequesterUser requesterUser)
    : IRequestHandler<GetPurchasedRewardsQuery, GetPurchasedRewardsQueryResponse>
{
    public async Task<GetPurchasedRewardsQueryResponse> Handle(GetPurchasedRewardsQuery request, CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;
        
        var result = await rewardService.GetPurchasedRewardsAsync(
            customerId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
        
        var purchasedRewards = result.Items.Select(pr => new PurchasedRewardDto
        {
            Id = pr.Id,
            RewardId = 1, // TODO: get from actual data
            RewardTitle = pr.RewardTitle,
            RewardImageUrl = null,
            PurchasedAt = pr.PurchasedAt,
            Status = pr.Status,
            SerialNumber = pr.SerialNumber,
            QrCode = pr.QrCode,
            ExpirationDate = null,
            UsedDate = null,
            PointsSpent = pr.PointsSpent
        }).ToList();
        
        return new GetPurchasedRewardsQueryResponse
        {
            PurchasedRewards = new PaginatedList<PurchasedRewardDto>(
                purchasedRewards,
                result.TotalCount,
                request.PageNumber,
                request.PageSize)
        };
    }
}

