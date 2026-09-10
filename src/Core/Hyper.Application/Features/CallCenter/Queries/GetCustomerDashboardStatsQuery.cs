namespace Hyper.Application.Features.CallCenter.Queries;

public record GetCustomerDashboardStatsQuery : IRequest<GetCustomerDashboardStatsQueryResponse>
{
    public int CustomerTenantId { get; init; }
}

public record GetCustomerDashboardStatsQueryResponse
{
    public int TotalPoints { get; set; }
    public int PointsEarnedThisMonth { get; set; }
    public int PointsUsedThisMonth { get; set; }
    public CustomerLevelDto CurrentLevel { get; set; } = null!;
    public CustomerLevelDto? NextLevel { get; set; }
    public int? PointsToNextLevel { get; set; }
    public List<RecentTransactionDto> RecentTransactions { get; set; } = [];
    public int AvailableRewards { get; set; }
    public int ActivePromotions { get; set; }
    public int ReferralCount { get; set; }
}

public record CustomerLevelDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int MinPoints { get; set; }
    public int? MaxPoints { get; set; }
    public string Color { get; set; } = null!;
    public string? Icon { get; set; }
}

public record RecentTransactionDto
{
    public string Id { get; set; } = null!;
    public DateTime Date { get; set; }
    public string Description { get; set; } = null!;
    public int Amount { get; set; }
    public string Type { get; set; } = null!;
}

