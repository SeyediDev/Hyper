using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Services;

/// <summary>
/// Mock implementation برای تست
/// </summary>
public class MockRewardService(ILogger<MockRewardService> logger) : IRewardService
{
    private static readonly List<PurchasedRewardDto> _purchasedRewards = [];

    public Task<PaginatedList<Interfaces.RewardDto>> GetRewardsAsync(int? categoryId = null, string? searchTerm = null, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        // NOTE: This is a Mock implementation. 
        // When implementing real RewardService, ensure to filter out rewards where IsLotteryOnly = true
        // Example: .Where(r => r.IsLotteryOnly == false || r.IsLotteryOnly == null)
        var rewards = new List<Interfaces.RewardDto>
        {
            new() { Id = 1, Title = "کارت هدیه 50 هزار تومانی", Description = "کارت هدیه دیجیتال", Value = 5000, Quantity = 100, PictureId = null, CategoryName = "کارت هدیه", MerchantName = "فروشگاه آنلاین", IsAvailable = true },
            new() { Id = 2, Title = "تخفیف 20 درصدی", Description = "کد تخفیف خرید", Value = 2000, Quantity = 50, PictureId = null, CategoryName = "تخفیف", MerchantName = "فروشگاه", IsAvailable = true }
        };

        return Task.FromResult(new PaginatedList<Interfaces.RewardDto>(rewards, 2, pageNumber, pageSize));
    }

    public Task<Interfaces.RewardDto?> GetRewardByIdAsync(int rewardId, CancellationToken cancellationToken = default)
    {
        // NOTE: This is a Mock implementation.
        // When implementing real RewardService, ensure to check IsLotteryOnly field
        // If reward.IsLotteryOnly == true, return null (not available in Hyper)
        var reward = new Interfaces.RewardDto
        {
            Id = rewardId,
            Title = "پاداش نمونه",
            Description = "توضیحات پاداش",
            Value = 1000,
            Quantity = 10,
            PictureId = null,
            CategoryName = "عمومی",
            MerchantName = "فروشگاه",
            IsAvailable = true
        };

        return Task.FromResult<Interfaces.RewardDto?>(reward);
    }

    public Task<PurchaseRewardResultDto> PurchaseRewardAsync(int customerId, int rewardId, int quantity = 1, CancellationToken cancellationToken = default)
    {
        var serial = $"SN-{Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper()}";
        var qrCode = $"QR-{customerId}-{rewardId}-{DateTime.UtcNow.Ticks}";

        var purchasedReward = new PurchasedRewardDto
        {
            Id = _purchasedRewards.Count + 1,
            RewardId = rewardId,
            RewardTitle = "پاداش خریداری شده",
            SerialNumber = serial,
            QrCode = qrCode,
            PurchasedAt = DateTime.UtcNow,
            PointsSpent = 1000,
            Status = "Active"
        };

        _purchasedRewards.Add(purchasedReward);

        logger.LogInformation("Mock: Purchased reward {RewardId} for customer {CustomerId}", rewardId, customerId);

        return Task.FromResult(new PurchaseRewardResultDto
        {
            Success = true,
            SerialNumber = serial,
            QrCode = qrCode,
            Message = "پاداش با موفقیت خریداری شد"
        });
    }

    public Task<PaginatedList<PurchasedRewardDto>> GetPurchasedRewardsAsync(int customerId, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new PaginatedList<PurchasedRewardDto>(_purchasedRewards, _purchasedRewards.Count, pageNumber, pageSize));
    }

    public Task<IEnumerable<Interfaces.RewardCategoryDto>> GetRewardCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var categories = new List<Interfaces.RewardCategoryDto>
        {
            new() { Id = 1, Name = "کارت هدیه", RewardCount = 10 },
            new() { Id = 2, Name = "تخفیف", RewardCount = 5 },
            new() { Id = 3, Name = "محصولات", RewardCount = 15 }
        };

        return Task.FromResult<IEnumerable<Interfaces.RewardCategoryDto>>(categories);
    }

    public Task<long> CalculateFinalRewardPriceAsync(int customerId, int rewardId, int pointId, CancellationToken cancellationToken = default)
    {
        // Mock implementation - returns base price
        return Task.FromResult(1000L);
    }
}

