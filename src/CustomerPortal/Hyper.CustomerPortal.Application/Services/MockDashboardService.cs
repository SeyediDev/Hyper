using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Services;

public class MockDashboardService(ICustomerService customerService, ICustomerRequesterUser requesterUser) : IDashboardService
{
    public async Task<DashboardStatsDto> GetDashboardStatsAsync(int customerId, CancellationToken cancellationToken = default)
    {
        var tenantId = requesterUser.TenantId;
        var customerTenant = await customerService.GetCustomerTenantAsync(customerId, tenantId, cancellationToken);

        var totalPoints = customerTenant?.TotalPointsEarned ?? 0;
        var availablePoints = customerTenant?.CurrentPointsBalance ?? 0;

        var stats = new DashboardStatsDto
        {
            TotalPoints = totalPoints,
            AvailablePoints = availablePoints,
            CurrentLevel = customerTenant?.RfmSegment?.ToString(),
            TotalReferrals = 0,
            LeaderboardPosition = 0,
            TotalRewardsPurchased = 0,
            PointsEarnedThisMonth = totalPoints,
            RecentActivities = new List<RecentActivityDto>()
        };

        return stats;
    }
}

