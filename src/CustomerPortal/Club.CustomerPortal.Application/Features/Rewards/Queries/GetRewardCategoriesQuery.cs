using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Rewards.Queries;

public record GetRewardCategoriesQuery : IRequest<GetRewardCategoriesQueryResponse>;

public record GetRewardCategoriesQueryResponse
{
    public List<RewardCategoryDto> Categories { get; set; } = [];
}

public record RewardCategoryDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public int RewardsCount { get; set; }
}

public class GetRewardCategoriesQueryHandler(IRewardService rewardService) : IRequestHandler<GetRewardCategoriesQuery, GetRewardCategoriesQueryResponse>
{
    public async Task<GetRewardCategoriesQueryResponse> Handle(GetRewardCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await rewardService.GetRewardCategoriesAsync(cancellationToken);
        
        var categoryDtos = categories.Select(c => new RewardCategoryDto
        {
            Id = c.Id.ToString(),
            Name = c.Name,
            Description = null,
            Icon = null,
            Color = "#4CAF50",
            RewardsCount = c.RewardCount
        }).ToList();
        
        return new GetRewardCategoriesQueryResponse
        {
            Categories = categoryDtos
        };
    }
}

