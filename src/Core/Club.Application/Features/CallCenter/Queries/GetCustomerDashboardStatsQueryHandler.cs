namespace Hyper.Application.Features.CallCenter.Queries;

public class GetCustomerDashboardStatsQueryHandler : IRequestHandler<GetCustomerDashboardStatsQuery, GetCustomerDashboardStatsQueryResponse>
{
    private readonly IQueryRepository<Domain.Entities.Customers.CustomerTenant, int> _customerTenantRepository;

    public GetCustomerDashboardStatsQueryHandler(
        IQueryRepository<Domain.Entities.Customers.CustomerTenant, int> customerTenantRepository)
    {
        _customerTenantRepository = customerTenantRepository;
    }

    public async Task<GetCustomerDashboardStatsQueryResponse> Handle(
        GetCustomerDashboardStatsQuery request,
        CancellationToken cancellationToken)
    {
        var customerTenant = await _customerTenantRepository
            .Query()
            .FirstOrDefaultAsync(ct => ct.Id == request.CustomerTenantId, cancellationToken);

        if (customerTenant == null)
        {
            // Return default stats if customer not found
            return new GetCustomerDashboardStatsQueryResponse
            {
                TotalPoints = 0,
                PointsEarnedThisMonth = 0,
                PointsUsedThisMonth = 0,
                CurrentLevel = new CustomerLevelDto
                {
                    Id = "1",
                    Name = "عادی",
                    Description = "سطح فعلی",
                    MinPoints = 0,
                    Color = "#6B7280",
                    Icon = "user"
                },
                RecentTransactions = [],
                AvailableRewards = 0,
                ActivePromotions = 0,
                ReferralCount = 0
            };
        }

        var totalPoints = (int)(customerTenant.TotalPointsEarned ?? 0);
        var currentBalance = (int)(customerTenant.CurrentPointsBalance ?? 0);
        var pointsUsed = totalPoints - currentBalance;

        // Calculate points earned this month (simplified - you may need to query transactions)
        var pointsEarnedThisMonth = totalPoints; // TODO: Calculate from transactions

        return new GetCustomerDashboardStatsQueryResponse
        {
            TotalPoints = totalPoints,
            PointsEarnedThisMonth = pointsEarnedThisMonth,
            PointsUsedThisMonth = pointsUsed,
            CurrentLevel = new CustomerLevelDto
            {
                Id = "1",
                Name = customerTenant.RfmSegment?.ToString() ?? "عادی",
                Description = "سطح فعلی مشتری",
                MinPoints = 0,
                Color = "#3B82F6",
                Icon = "star"
            },
            NextLevel = null,
            PointsToNextLevel = null,
            RecentTransactions = [], // TODO: Load from CustomerTransaction
            AvailableRewards = 0, // TODO: Load from RewardPurchase
            ActivePromotions = 0, // TODO: Load from PromotionRecipient
            ReferralCount = 0 // TODO: Load from CustomerReferrer
        };
    }
}

