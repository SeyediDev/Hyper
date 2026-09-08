namespace Hyper.CustomerPortal.Application.Interfaces;

/// <summary>
/// سرویس داشبورد مشتری
/// </summary>
public interface IDashboardService
{
    /// <summary>
    /// دریافت آمار داشبورد مشتری
    /// </summary>
    Task<DashboardStatsDto> GetDashboardStatsAsync(int customerId, CancellationToken cancellationToken = default);
}

public record DashboardStatsDto
{
    public long TotalPoints { get; init; }
    public long AvailablePoints { get; init; }
    public string? CurrentLevel { get; init; }
    public int TotalReferrals { get; init; }
    public int LeaderboardPosition { get; init; }
    public int TotalRewardsPurchased { get; init; }
    public long PointsEarnedThisMonth { get; init; }
    public IEnumerable<RecentActivityDto> RecentActivities { get; init; } = [];
}

public record RecentActivityDto
{
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
    public DateTime OccurredAt { get; init; }
    public string Type { get; init; } = null!;
}

