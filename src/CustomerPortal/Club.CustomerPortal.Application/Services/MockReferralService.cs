using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Services;

public class MockReferralService(ILogger<MockReferralService> logger) : IReferralService
{
    public Task<ReferralStatsDto> GetReferralStatsAsync(int customerId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new ReferralStatsDto
        {
            MyReferrerCode = $"REF{customerId:D6}",
            TotalReferrals = 5,
            TotalPointsEarned = 5000,
            ActiveReferrals = 3
        });
    }

    public Task<string> RequestReferrerCodeAsync(int customerId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult($"REF{customerId:D6}");
    }

    public Task<bool> ValidateReferrerCodeAsync(string referrerCode, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(referrerCode.StartsWith("REF"));
    }

    public Task SetReferrerAsync(int customerId, string referrerCode, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Mock: Set referrer {Code} for customer {CustomerId}", referrerCode, customerId);
        return Task.CompletedTask;
    }

    public Task<PaginatedList<ReferredCustomerDto>> GetReferredCustomersAsync(int customerId, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var customers = new List<ReferredCustomerDto>
        {
            new() { Name = "علی محمدی", JoinedAt = DateTime.UtcNow.AddDays(-10), PointsEarned = 1000, IsActive = true }
        };

        return Task.FromResult(new PaginatedList<ReferredCustomerDto>(customers, 1, pageNumber, pageSize));
    }

    public Task<PaginatedList<LeaderboardEntryDto>> GetLeaderboardAsync(int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var entries = new List<LeaderboardEntryDto>
        {
            new() { Rank = 1, CustomerName = "محمد رضایی", TotalPoints = 50000, AvatarUrl = null, IsCurrentUser = false },
            new() { Rank = 2, CustomerName = "شما", TotalPoints = 15000, AvatarUrl = null, IsCurrentUser = true }
        };

        return Task.FromResult(new PaginatedList<LeaderboardEntryDto>(entries, 2, pageNumber, pageSize));
    }

    public Task<int> GetCustomerPositionAsync(int customerId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(2);
    }
}

