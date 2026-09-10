using Hyper.CustomerPortal.Application.Common;
using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Rewards.Queries;

public record GetRewardsQuery : IRequest<GetRewardsQueryResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? CategoryId { get; set; }
    public bool? IsAvailable { get; set; }
    public string? Search { get; set; }
}

public record GetRewardsQueryResponse
{
    public PaginatedList<RewardDto> Rewards { get; set; } = null!;
}

public record RewardDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string CategoryId { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public int? PictureId { get; set; }
    public string? ImageUrl { get; set; }
    public List<RewardCostDto> Costs { get; set; } = [];
    public int? Stock { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public string? TermsAndConditions { get; set; }
    public MerchantDto? Merchant { get; set; }
}

public record RewardCostDto
{
    public string PointTypeId { get; set; } = null!;
    public string PointTypeName { get; set; } = null!;
    public string PointTypeColor { get; set; } = null!;
    public long OriginalAmount { get; set; }
    public long FinalAmount { get; set; }
    public bool HasDiscount { get; set; }
    public long Amount => FinalAmount;
}

public record MerchantDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? LogoUrl { get; set; }
}

public class GetRewardsQueryHandler(
    IRewardService rewardService,
    IPlanService planService,
    ICustomerRequesterUser requesterUser) : IRequestHandler<GetRewardsQuery, GetRewardsQueryResponse>
{
    public async Task<GetRewardsQueryResponse> Handle(GetRewardsQuery request, CancellationToken cancellationToken)
    {
        var categoryId = string.IsNullOrEmpty(request.CategoryId) ? null : (int?)int.Parse(request.CategoryId);
        
        var result = await rewardService.GetRewardsAsync(
            categoryId,
            request.Search,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        CustomerPlanServiceDto? activePlan = null;
        var customerId = requesterUser.CustomerId;
        var hasCustomer = customerId > 0;

        if (hasCustomer)
        {
            activePlan = await planService.GetActiveCustomerPlanAsync(customerId, cancellationToken);
        }
        
        var rewards = new List<RewardDto>();

        foreach (var reward in result.Items)
        {
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

            rewards.Add(new RewardDto
            {
                Id = reward.Id.ToString(),
                Name = reward.Title,
                Description = reward.Description ?? string.Empty,
                CategoryId = "1", // TODO: دریافت شناسه واقعی دسته‌بندی
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
            });
        }
        
        return new GetRewardsQueryResponse
        {
            Rewards = new PaginatedList<RewardDto>(
                rewards,
                result.TotalCount,
                request.PageNumber,
                request.PageSize)
        };
    }
}

