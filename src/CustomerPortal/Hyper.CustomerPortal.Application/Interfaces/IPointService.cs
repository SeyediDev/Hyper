namespace Hyper.CustomerPortal.Application.Interfaces;

/// <summary>
/// سرویس مدیریت امتیازات
/// </summary>
public interface IPointService
{
    /// <summary>
    /// دریافت خلاصه امتیازات مشتری
    /// </summary>
    Task<PointsSummaryDto> GetPointsSummaryAsync(int customerId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// دریافت لیست تراکنش‌های امتیاز
    /// </summary>
    Task<PaginatedList<PointTransactionDto>> GetPointTransactionsAsync(
        int customerId, 
        int pageNumber = 1, 
        int pageSize = 20,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// تبدیل امتیازات (مثلاً از نوعی به نوع دیگر)
    /// </summary>
    Task<ConvertPointsResultDto> ConvertPointsAsync(
        int customerId, 
        int sourcePointTypeId, 
        int targetPointTypeId, 
        long amount,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// انتقال امتیاز به مشتری دیگر
    /// </summary>
    Task<TransferPointsResultDto> TransferPointsAsync(
        int fromCustomerId, 
        string toPhoneNumber, 
        long amount,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// دریافت امتیازات در حال انقضا
    /// </summary>
    Task<IEnumerable<ExpiringPointDto>> GetExpiringPointsAsync(
        int customerId, 
        int daysAhead = 30,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// دریافت نرخ‌های تبدیل امتیاز
    /// </summary>
    Task<IEnumerable<ConversionRateDto>> GetConversionRatesAsync(CancellationToken cancellationToken = default);
}

public record PointsSummaryDto
{
    public long TotalPoints { get; init; }
    public long AvailablePoints { get; init; }
    public long PendingPoints { get; init; }
    public long ExpiredPoints { get; init; }
    public string? CurrentLevelName { get; init; }
    public long PointsToNextLevel { get; init; }
}

public record PointTransactionDto
{
    public int Id { get; init; }
    public string Title { get; init; } = null!;
    public long Amount { get; init; }
    public string Type { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
    public string? Description { get; init; }
    public DateTime? ExpirationDate { get; init; }
    public bool IsExpired { get; init; }
    public bool IsSpent { get; init; }
}

public record ConvertPointsResultDto
{
    public bool Success { get; init; }
    public long ConvertedAmount { get; init; }
    public string Message { get; init; } = null!;
}

public record TransferPointsResultDto
{
    public bool Success { get; init; }
    public string ReceiverName { get; init; } = null!;
    public long TransferredAmount { get; init; }
    public string Message { get; init; } = null!;
}

public record ExpiringPointDto
{
    public long Amount { get; init; }
    public DateTime ExpiryDate { get; init; }
    public int DaysRemaining { get; init; }
}

public record ConversionRateDto
{
    public int SourcePointTypeId { get; init; }
    public string SourcePointTypeName { get; init; } = null!;
    public int TargetPointTypeId { get; init; }
    public string TargetPointTypeName { get; init; } = null!;
    public decimal Rate { get; init; }
}

