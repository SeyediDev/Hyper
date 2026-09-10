using Hyper.CustomerPortal.Application.Common;
using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Rewards.Queries;

public record GetRewardByIdQuery : IRequest<GetRewardByIdQueryResponse>
{
    public string Id { get; set; } = null!;
}

public record GetRewardByIdQueryResponse
{
    public RewardDto Reward { get; set; } = null!;
}

public class GetRewardByIdQueryHandler(
    IRewardService rewardService,
    IPlanService planService,
    ICustomerRequesterUser requesterUser) : IRequestHandler<GetRewardByIdQuery, GetRewardByIdQueryResponse>
{
    public async Task<GetRewardByIdQueryResponse> Handle(GetRewardByIdQuery request, CancellationToken cancellationToken)
    {
        var reward = await rewardService.GetRewardByIdAsync(int.Parse(request.Id), cancellationToken);
        
        if (reward == null)
        {
            throw new InvalidOperationException("پاداش یافت نشد");
        }
        
        CustomerPlanServiceDto? activePlan = null;
        var customerId = requesterUser.CustomerId;
        var hasCustomer = customerId > 0;

        if (hasCustomer)
        {
            activePlan = await planService.GetActiveCustomerPlanAsync(customerId, cancellationToken);
        }

        var originalPrice = reward.Value;
        long finalPrice = originalPrice;

        if (hasCustomer && activePlan != null)
        {
            finalPrice = await planService.CalculateRewardPriceWithPlanDiscountAsync(
                customerId,
                reward.Id,
                activePlan.PointId,
                originalPrice,
                activePlan,
                cancellationToken);
        }

        var costDto = new RewardCostDto
        {
            PointTypeId = activePlan?.PointId.ToString() ?? reward.Id.ToString(),
            PointTypeName = activePlan?.PointTypeName ?? "امتیاز",
            PointTypeColor = activePlan?.PointTypeColor ?? "#6366F1",
            OriginalAmount = originalPrice,
            FinalAmount = finalPrice,
            HasDiscount = finalPrice < originalPrice
        };

        return new GetRewardByIdQueryResponse
        {
            Reward = new RewardDto
            {
                Id = reward.Id.ToString(),
                Name = reward.Title,
                Description = reward.Description ?? string.Empty,
                CategoryId = "1",
                CategoryName = reward.CategoryName,
                PictureId = reward.PictureId,
                ImageUrl = DocumentUrlHelper.BuildDocumentUrl(reward.PictureId),
                Costs = [costDto],
                Stock = reward.Quantity,
                IsAvailable = reward.IsAvailable,
                ValidFrom = null,
                ValidTo = null,
                TermsAndConditions = null,
                Merchant = new MerchantDto
                {
                    Id = "1",
                    Name = reward.MerchantName,
                    LogoUrl = null
                }
            }
        };
    }
}

