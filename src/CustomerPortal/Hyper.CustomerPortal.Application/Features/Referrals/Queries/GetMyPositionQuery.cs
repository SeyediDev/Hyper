using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Referrals.Queries;

public record GetMyPositionQuery : IRequest<GetMyPositionQueryResponse>
{
    public string? PointTypeId { get; set; }
}

public record GetMyPositionQueryResponse
{
    public int Rank { get; set; }
    public int TotalPoints { get; set; }
    public LeaderboardEntryDto Position { get; set; } = null!;
}

public class GetMyPositionQueryHandler(
    IReferralService referralService,
    ICustomerRequesterUser requesterUser)
    : IRequestHandler<GetMyPositionQuery, GetMyPositionQueryResponse>
{
    public async Task<GetMyPositionQueryResponse> Handle(GetMyPositionQuery request, CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;
        var rank = await referralService.GetCustomerPositionAsync(customerId, cancellationToken);
        
        return new GetMyPositionQueryResponse
        {
            Rank = rank,
            TotalPoints = 0,
            Position = new LeaderboardEntryDto
            {
                Rank = rank,
                CustomerId = customerId.ToString(),
                CustomerName = "شما",
                AvatarUrl = null,
                TotalPoints = 0,
                Level = null,
                Trend = null
            }
        };
    }
}

