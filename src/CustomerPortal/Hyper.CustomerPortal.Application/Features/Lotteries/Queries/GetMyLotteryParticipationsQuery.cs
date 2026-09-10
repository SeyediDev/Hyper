using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Lotteries.Queries;

public record GetMyLotteryParticipationsQuery : IRequest<GetMyLotteryParticipationsQueryResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public record GetMyLotteryParticipationsQueryResponse
{
    public PaginatedList<LotteryParticipationDto> Participations { get; set; } = null!;
}

public record LotteryParticipationDto
{
    public LotteryDto Lottery { get; set; } = null!;
    public int EntryCount { get; set; }
    public DateTime ParticipatedDate { get; set; }
    public bool IsWinner { get; set; }
    public string? Prize { get; set; }
}

public class GetMyLotteryParticipationsQueryHandler(
    ILotteryService lotteryService,
    ICustomerRequesterUser requesterUser) : IRequestHandler<GetMyLotteryParticipationsQuery, GetMyLotteryParticipationsQueryResponse>
{
    public async Task<GetMyLotteryParticipationsQueryResponse> Handle(GetMyLotteryParticipationsQuery request, CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;
        var result = await lotteryService.GetMyParticipationsAsync(customerId, request.PageNumber, request.PageSize, cancellationToken);
        
        var participations = result.Items.Select(p => new LotteryParticipationDto
        {
            Lottery = new LotteryDto
            {
                Id = p.LotteryId.ToString(),
                Name = p.LotteryTitle,
                Description = string.Empty,
                ImageUrl = null,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),
                DrawDate = DateTime.UtcNow.AddDays(7),
                IsActive = true,
                EntryCost = null,
                MaxEntries = 100,
                MyEntries = 1,
                TotalEntries = 50,
                Prizes = [],
                HasParticipated = true,
                IsWinner = p.IsWinner
            },
            EntryCount = 1,
            ParticipatedDate = p.ParticipatedAt,
            IsWinner = p.IsWinner,
            Prize = p.PrizeName
        }).ToList();
        
        return new GetMyLotteryParticipationsQueryResponse
        {
            Participations = new PaginatedList<LotteryParticipationDto>(participations, result.TotalCount, request.PageNumber, request.PageSize)
        };
    }
}

