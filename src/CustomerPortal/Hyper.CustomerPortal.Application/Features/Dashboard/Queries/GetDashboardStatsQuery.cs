using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Dashboard.Queries;

public record GetDashboardStatsQuery : IRequest<GetDashboardStatsQueryResponse>;

public record GetDashboardStatsQueryResponse
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

public class GetDashboardStatsQueryHandler(IDashboardService dashboardService, ICustomerRequesterUser requesterUser) : IRequestHandler<GetDashboardStatsQuery, GetDashboardStatsQueryResponse>
{
    public async Task<GetDashboardStatsQueryResponse> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;
        var stats = await dashboardService.GetDashboardStatsAsync(customerId, cancellationToken);
        
        return new GetDashboardStatsQueryResponse
        {
            TotalPoints = (int)stats.TotalPoints,
            PointsEarnedThisMonth = (int)stats.PointsEarnedThisMonth,
            PointsUsedThisMonth = 0,
            CurrentLevel = new CustomerLevelDto
            {
                Id = "1",
                Name = stats.CurrentLevel ?? "برنزی",
                Description = "سطح فعلی شما",
                MinPoints = 0,
                MaxPoints = 1000,
                Color = "#CD7F32",
                Icon = "star"
            },
            NextLevel = null,
            PointsToNextLevel = null,
            RecentTransactions = stats.RecentActivities.Take(5).Select(a => new RecentTransactionDto
            {
                Id = Guid.NewGuid().ToString(),
                Date = a.OccurredAt,
                Description = a.Title,
                Amount = 0,
                Type = a.Type
            }).ToList(),
            AvailableRewards = stats.TotalRewardsPurchased,
            ActivePromotions = 0,
            ReferralCount = stats.TotalReferrals
        };
    }
}

