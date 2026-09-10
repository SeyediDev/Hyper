namespace Hyper.Domain.Features;

public interface IAwardService
{
    Task<Reward?> GetAward(int awardId, CancellationToken cancellationToken);
    Task<AwardDto?> GetAwardDto(int awardId, CancellationToken cancellationToken);
    Task<List<AwardDto>> GetAwardDtos(int tenantId, CancellationToken cancellationToken);
}

public class AwardService(
    IQueryRepository<Reward, int> awardRepository,
    IQueryRepository<RewardAsset, int> assetRepository
    ) : IAwardService
{
    public async Task<Reward?> GetAward(int awardId, CancellationToken cancellationToken)
    { 
        var award = await awardRepository.FirstOrDefaultAsync(x=>x.Id==awardId, cancellationToken);
        // If award is lottery-only, return null for Hyper usage
        if (award?.IsLotteryOnly == true)
        {
            return null;
        }
        return award;
    }
    public async Task<AwardDto?> GetAwardDto(int awardId, CancellationToken cancellationToken)
    {
        ConfigMap();

        var award = await awardRepository.FirstOrDefaultAsync(x => x.Id == awardId, cancellationToken);
        // If award is lottery-only, return null for Hyper usage
        if (award?.IsLotteryOnly == true)
        {
            return null;
        }

        AwardDto? awardDto = await awardRepository.GetByIdAsync<AwardDto>(awardId, cancellationToken);

        return awardDto;
    }

    public async Task<List<AwardDto>> GetAwardDtos(int tenantId, CancellationToken cancellationToken)
    {
        ConfigMap();
        // Asset بررسی و حذفِ پاداش‌ها با وضعیت ControlAsset
        var awardIdsHavingAssets = assetRepository.Query()
            .Where(x => 
                (x.Reward.ControlAsset==true )
                && x.Reward.TenantId == tenantId && 
                x.Reward.Visible==true)
            .Select(a => a.RewardId)
            .Distinct()
            .ToList();
        
        List<AwardDto> awards = await awardRepository.GetAllAsync<AwardDto>(
            cancellationToken,
            x => x.TenantId == tenantId && x.Visible==true && 
            (x.ControlAsset==null || x.ControlAsset==false || awardIdsHavingAssets.Contains(x.Id)) &&
            x.IsLotteryOnly == false, // Exclude lottery-only rewards from Hyper
            o => o.OrderByDescending(x => x.OrderId));

        return awards;
    }
    private static void ConfigMap()
    {
        _ = TypeAdapterConfig<Reward, AwardDto>.NewConfig()
            .Map(dest => dest.PointLevel, src => src.PointLevel != null ? src.PointLevel.Level : 0)
            .Map(dest => dest.PointPointId, src => src.PointLevel != null ? src.PointLevel.PointId : 0)
            .Map(dest => dest.PointLevelTitle, src => src.PointLevel != null ? src.PointLevel.Title : "")
            .Map(dest => dest.CategoryTitle, src => src.RewardCategory != null ? src.RewardCategory.Title : "")
            .Map(dest => dest.MerchantTitle, src => src.Merchant.Title)
            .Map(dest => dest.PictureId, src => src.PictureId)
            .Map(dest => dest.Costs, src => src.Costs != null ? src.Costs.Select(c => c.Adapt<AwardCostDto>()) : new List<AwardCostDto>())
            ;
        _ = TypeAdapterConfig<RewardCost, AwardCostDto>.NewConfig()
            .Map(dest => dest.Point, src => src.Point != null ? src.Point.Title : "")
            .Map(dest => dest.PointLevel, src => src.PointLevel != null ? src.PointLevel.Level : 0)
            .Map(dest => dest.PointPointId, src => src.PointLevel != null ? src.PointLevel.PointId : 0)
            .Map(dest => dest.PointLevelTitle, src => src.PointLevel != null ? src.PointLevel.Title : "")
            ;
    }
}

public record AwardDto
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
    public List<AwardCostDto>? Costs { get; set; }
}

public record AwardCostDto
{
    public int? PointLevelId { get; set; }
    public int? PointLevel { get; set; }
    public int? PointPointId { get; set; }
    public string? PointLevelTitle { get; set; } = null!;
    public int PointId { get; set; }
    public string Point { get; set; } = null!;
    public int Amount { get; set; }
}
