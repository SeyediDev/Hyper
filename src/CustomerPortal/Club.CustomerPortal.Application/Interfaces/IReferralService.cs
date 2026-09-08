namespace Hyper.CustomerPortal.Application.Interfaces;

/// <summary>
/// سرویس مدیریت معرفی‌ها
/// </summary>
public interface IReferralService
{
    /// <summary>
    /// دریافت آمار معرفی مشتری
    /// </summary>
    Task<ReferralStatsDto> GetReferralStatsAsync(int customerId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// درخواست کد معرف
    /// </summary>
    Task<string> RequestReferrerCodeAsync(int customerId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// تایید کد معرف
    /// </summary>
    Task<bool> ValidateReferrerCodeAsync(string referrerCode, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// ثبت معرف برای مشتری
    /// </summary>
    Task SetReferrerAsync(int customerId, string referrerCode, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// دریافت لیست مشتریان معرفی شده
    /// </summary>
    Task<PaginatedList<ReferredCustomerDto>> GetReferredCustomersAsync(
        int customerId,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// دریافت لیدربورد
    /// </summary>
    Task<PaginatedList<LeaderboardEntryDto>> GetLeaderboardAsync(
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// دریافت موقعیت مشتری در لیدربورد
    /// </summary>
    Task<int> GetCustomerPositionAsync(int customerId, CancellationToken cancellationToken = default);
}

public record ReferralStatsDto
{
    public string? MyReferrerCode { get; init; }
    public int TotalReferrals { get; init; }
    public long TotalPointsEarned { get; init; }
    public int ActiveReferrals { get; init; }
}

public record ReferredCustomerDto
{
    public string Name { get; init; } = null!;
    public DateTime JoinedAt { get; init; }
    public long PointsEarned { get; init; }
    public bool IsActive { get; init; }
}

public record LeaderboardEntryDto
{
    public int Rank { get; init; }
    public string CustomerName { get; init; } = null!;
    public long TotalPoints { get; init; }
    public string? AvatarUrl { get; init; }
    public bool IsCurrentUser { get; init; }
}

