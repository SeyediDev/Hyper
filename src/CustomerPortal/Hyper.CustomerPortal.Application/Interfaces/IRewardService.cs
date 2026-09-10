namespace Hyper.CustomerPortal.Application.Interfaces;

/// <summary>
/// سرویس مدیریت پاداش‌ها
/// </summary>
public interface IRewardService
{
    /// <summary>
    /// دریافت لیست پاداش‌ها
    /// </summary>
    Task<PaginatedList<RewardDto>> GetRewardsAsync(
        int? categoryId = null,
        string? searchTerm = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// دریافت جزئیات پاداش
    /// </summary>
    Task<RewardDto?> GetRewardByIdAsync(int rewardId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// خرید پاداش
    /// </summary>
    Task<PurchaseRewardResultDto> PurchaseRewardAsync(
        int customerId, 
        int rewardId, 
        int quantity = 1,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// محاسبه قیمت نهایی ریوارد با احتساب تخفیف طرح
    /// </summary>
    Task<long> CalculateFinalRewardPriceAsync(
        int customerId,
        int rewardId,
        int pointId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// دریافت پاداش‌های خریداری شده
    /// </summary>
    Task<PaginatedList<PurchasedRewardDto>> GetPurchasedRewardsAsync(
        int customerId,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// دریافت دسته‌بندی‌های پاداش
    /// </summary>
    Task<IEnumerable<RewardCategoryDto>> GetRewardCategoriesAsync(CancellationToken cancellationToken = default);
}

public record RewardDto
{
    public int Id { get; init; }
    public string Title { get; init; } = null!;
    public string? Description { get; init; }
    public long Value { get; init; }
    public int Quantity { get; init; }
    public int? PictureId { get; init; }
    public string CategoryName { get; init; } = null!;
    public string MerchantName { get; init; } = null!;
    public bool IsAvailable { get; init; }
}

public record PurchaseRewardResultDto
{
    public bool Success { get; init; }
    public string? SerialNumber { get; init; }
    public string? QrCode { get; init; }
    public string Message { get; init; } = null!;
}

public record PurchasedRewardDto
{
    public int Id { get; init; }
    public int RewardId { get; init; }
    public string RewardTitle { get; init; } = null!;
    public string RewardName => RewardTitle; // Alias for compatibility
    public string? RewardImageUrl { get; init; }
    public string? SerialNumber { get; init; }
    public string? QrCode { get; init; }
    public DateTime PurchasedAt { get; init; }
    public DateTime PurchaseDate => PurchasedAt; // Alias for compatibility
    public long PointsSpent { get; init; }
    public string Status { get; init; } = null!;
    public DateTime? ExpirationDate { get; init; }
    public DateTime? UsedDate { get; init; }
    public List<RewardCostDto> Costs { get; init; } = [];
}

public record RewardCategoryDto
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public int RewardCount { get; init; }
}

