using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Referrals.Queries;

public record GetReferralStatsQuery : IRequest<GetReferralStatsQueryResponse>;

public record GetReferralStatsQueryResponse
{
    public int TotalReferrals { get; set; }
    public int ActiveReferrals { get; set; }
    public int TotalPointsEarned { get; set; }
    public string ReferralCode { get; set; } = null!;
    public string ReferralLink { get; set; } = null!;
}

public class GetReferralStatsQueryHandler(IReferralService referralService, ICustomerRequesterUser requesterUser)
    : IRequestHandler<GetReferralStatsQuery, GetReferralStatsQueryResponse>
{
    public async Task<GetReferralStatsQueryResponse> Handle(GetReferralStatsQuery request, CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;
        var stats = await referralService.GetReferralStatsAsync(customerId, cancellationToken);
        
        return new GetReferralStatsQueryResponse
        {
            TotalReferrals = stats.TotalReferrals,
            ActiveReferrals = stats.ActiveReferrals,
            TotalPointsEarned = (int)stats.TotalPointsEarned,
            ReferralCode = stats.MyReferrerCode ?? string.Empty,
            ReferralLink = $"https://portal.Hyper.com/register?ref={stats.MyReferrerCode}"
        };
    }
}

