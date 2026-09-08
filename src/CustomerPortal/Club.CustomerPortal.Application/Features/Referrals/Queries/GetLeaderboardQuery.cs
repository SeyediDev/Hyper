using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Referrals.Queries;

public record GetLeaderboardQuery : IRequest<GetLeaderboardQueryResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? PointTypeId { get; set; }
    public string? Period { get; set; }
}

public record GetLeaderboardQueryResponse
{
    public PaginatedList<LeaderboardEntryDto> Entries { get; set; } = null!;
}

public record LeaderboardEntryDto
{
    public int Rank { get; set; }
    public string CustomerId { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public int TotalPoints { get; set; }
    public string? Level { get; set; }
    public string? Trend { get; set; }
}

public class GetLeaderboardQueryHandler(IReferralService referralService, IRequesterUser requesterUser) : IRequestHandler<GetLeaderboardQuery, GetLeaderboardQueryResponse>
{
    public async Task<GetLeaderboardQueryResponse> Handle(GetLeaderboardQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = requesterUser.GetUserId();
        var result = await referralService.GetLeaderboardAsync(request.PageNumber, request.PageSize, cancellationToken);
        
        var entries = result.Items.Select(e => new LeaderboardEntryDto
        {
            Rank = e.Rank,
            CustomerId = currentUserId.ToString(),
            CustomerName = e.CustomerName,
            AvatarUrl = e.AvatarUrl,
            TotalPoints = (int)e.TotalPoints,
            Level = null,
            Trend = null
        }).ToList();
        
        return new GetLeaderboardQueryResponse
        {
            Entries = new PaginatedList<LeaderboardEntryDto>(entries, result.TotalCount, request.PageNumber, request.PageSize)
        };
    }
}

