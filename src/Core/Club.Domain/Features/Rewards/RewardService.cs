namespace Hyper.Domain.Features.Rewards;

public interface IRewardService
{
    Task<Reward?> GetReward(int rewardId, CancellationToken cancellationToken);
    Task<RewardDto?> GetRewardDto(int rewardId, CancellationToken cancellationToken);
    Task<List<RewardDto>> GetRewardDtos(int tenantId, CancellationToken cancellationToken);
}

public class RewardService(
    IQueryRepository<Reward, int> rewardRepository,
    IQueryRepository<RewardAsset, int> assetRepository
    ) : IRewardService
{
    public async Task<Reward?> GetReward(int rewardId, CancellationToken cancellationToken)
    { 
        var reward = await rewardRepository.FirstOrDefaultAsync(x=>x.Id==rewardId, cancellationToken);
        // If reward is lottery-only, return null for Hyper usage
        if (reward?.IsLotteryOnly == true)
        {
            return null;
        }
        return reward;
    }
    public async Task<RewardDto?> GetRewardDto(int rewardId, CancellationToken cancellationToken)
    {
        ConfigMap();

        var reward = await rewardRepository.FirstOrDefaultAsync(x => x.Id == rewardId, cancellationToken);
        // If reward is lottery-only, return null for Hyper usage
        if (reward?.IsLotteryOnly == true)
        {
            return null;
        }

        RewardDto? rewardDto = await rewardRepository.GetByIdAsync<RewardDto>(rewardId, cancellationToken);

        return rewardDto;
    }

    public async Task<List<RewardDto>> GetRewardDtos(int tenantId, CancellationToken cancellationToken)
    {
        ConfigMap();
        // Asset بررسی و حذفِ پاداش‌ها با وضعیت ControlAsset
        var rewardIdsHavingAssets = assetRepository.Query()
            .Where(x => 
                (x.Reward.ControlAsset==true )
                && x.Reward.TenantId == tenantId && 
                x.Reward.Visible==true)
            .Select(a => a.RewardId)
            .Distinct()
            .ToList();
        
        List<RewardDto> rewards = await rewardRepository.GetAllAsync<RewardDto>(
            cancellationToken,
            x => x.TenantId == tenantId && x.Visible==true && 
            (x.ControlAsset==null || x.ControlAsset==false || rewardIdsHavingAssets.Contains(x.Id)) &&
            x.IsLotteryOnly == false, // Exclude lottery-only rewards from Hyper
            o => o.OrderByDescending(x => x.OrderId));

        return rewards;
    }
    private static void ConfigMap()
    {
        _ = TypeAdapterConfig<Reward, RewardDto>.NewConfig()
            .Map(dest => dest.PointLevel, src => src.PointLevel != null ? src.PointLevel.Level : 0)
            .Map(dest => dest.PointPointId, src => src.PointLevel != null ? src.PointLevel.PointId : 0)
            .Map(dest => dest.PointLevelTitle, src => src.PointLevel != null ? src.PointLevel.Title : "")
            .Map(dest => dest.CategoryTitle, src => src.RewardCategory != null ? src.RewardCategory.Title : "")
            .Map(dest => dest.MerchantTitle, src => src.Merchant.Title)
            .Map(dest => dest.PictureId, src => src.PictureId)
            .Map(dest => dest.Costs, src => src.Costs != null ? src.Costs.Select(c => c.Adapt<RewardCostDto>()) : new List<RewardCostDto>())
            ;
        _ = TypeAdapterConfig<RewardCost, RewardCostDto>.NewConfig()
            .Map(dest => dest.Point, src => src.Point != null ? src.Point.Title : "")
            .Map(dest => dest.PointLevel, src => src.PointLevel != null ? src.PointLevel.Level : 0)
            .Map(dest => dest.PointPointId, src => src.PointLevel != null ? src.PointLevel.PointId : 0)
            .Map(dest => dest.PointLevelTitle, src => src.PointLevel != null ? src.PointLevel.Title : "")
            ;
    }
}

public record RewardDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    
    public string Title { get; set; } = null!;

    public int? PointLevelId { get; set; }
    public int? PointLevel { get; set; }
    public int? PointPointId { get; set; }
    public string? PointLevelTitle { get; set; } = null!;

    public int CategoryId { get; set; }
    public string CategoryTitle { get; set; } = null!;

    public int MerchantId { get; set; }
    public string MerchantTitle { get; set; } = null!;
    public bool Visible { get; set; }
    public bool ControlAsset { get; set; }
    public int Quantity { get; set; }
    public int? PictureId { get; set; }
    public List<RewardCostDto>? Costs { get; set; }
}

public record RewardCostDto
{
    public int? PointLevelId { get; set; }
    public int? PointLevel { get; set; }
    public int? PointPointId { get; set; }
    public string? PointLevelTitle { get; set; } = null!;
    public int PointId { get; set; }
    public string Point { get; set; } = null!;
    public int Amount { get; set; }
}
