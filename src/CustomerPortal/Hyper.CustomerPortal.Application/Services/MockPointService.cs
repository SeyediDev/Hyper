using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Services;

/// <summary>
/// Mock implementation برای تست
/// </summary>
public class MockPointService(ILogger<MockPointService> logger) : IPointService
{
    public Task<PointsSummaryDto> GetPointsSummaryAsync(int customerId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new PointsSummaryDto
        {
            TotalPoints = 15000,
            AvailablePoints = 12500,
            PendingPoints = 2500,
            ExpiredPoints = 500,
            CurrentLevelName = "طلایی",
            PointsToNextLevel = 5000
        });
    }

    public Task<PaginatedList<PointTransactionDto>> GetPointTransactionsAsync(int customerId, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var transactions = new List<PointTransactionDto>
        {
            new() { Id = 1, Title = "خرید محصول", Amount = 500, Type = "Earned", CreatedAt = DateTime.UtcNow.AddDays(-1), Description = "امتیاز خرید" },
            new() { Id = 2, Title = "معرفی دوست", Amount = 1000, Type = "Referral", CreatedAt = DateTime.UtcNow.AddDays(-2), Description = "امتیاز معرفی" },
            new() { Id = 3, Title = "خرید پاداش", Amount = -200, Type = "Spent", CreatedAt = DateTime.UtcNow.AddDays(-3), Description = "خرید هدیه" }
        };

        return Task.FromResult(new PaginatedList<PointTransactionDto>(transactions, 3, pageNumber, pageSize));
    }

    public Task<ConvertPointsResultDto> ConvertPointsAsync(int customerId, int sourcePointTypeId, int targetPointTypeId, long amount, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Mock: Converted {Amount} points", amount);
        return Task.FromResult(new ConvertPointsResultDto
        {
            Success = true,
            ConvertedAmount = amount,
            Message = "تبدیل با موفقیت انجام شد"
        });
    }

    public Task<TransferPointsResultDto> TransferPointsAsync(int fromCustomerId, string toPhoneNumber, long amount, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Mock: Transferred {Amount} points to {Phone}", amount, toPhoneNumber);
        return Task.FromResult(new TransferPointsResultDto
        {
            Success = true,
            ReceiverName = "مشتری گرامی",
            TransferredAmount = amount,
            Message = "انتقال با موفقیت انجام شد"
        });
    }

    public Task<IEnumerable<ExpiringPointDto>> GetExpiringPointsAsync(int customerId, int daysAhead = 30, CancellationToken cancellationToken = default)
    {
        var expiringPoints = new List<ExpiringPointDto>
        {
            new() { Amount = 500, ExpiryDate = DateTime.UtcNow.AddDays(15), DaysRemaining = 15 }
        };

        return Task.FromResult<IEnumerable<ExpiringPointDto>>(expiringPoints);
    }

    public Task<IEnumerable<ConversionRateDto>> GetConversionRatesAsync(CancellationToken cancellationToken = default)
    {
        var rates = new List<ConversionRateDto>
        {
            new() { SourcePointTypeId = 1, SourcePointTypeName = "طلایی", TargetPointTypeId = 2, TargetPointTypeName = "نقره‌ای", Rate = 1.5M }
        };

        return Task.FromResult<IEnumerable<ConversionRateDto>>(rates);
    }
}

