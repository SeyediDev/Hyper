using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Lotteries.Queries;

public record GetLotteriesQuery : IRequest<GetLotteriesQueryResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool? IsActive { get; set; }
}

public record GetLotteriesQueryResponse
{
    public PaginatedList<LotteryDto> Lotteries { get; set; } = null!;
}

public record LotteryDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime DrawDate { get; set; }
    public bool IsActive { get; set; }
    public List<RewardCostDto>? EntryCost { get; set; }
    public int? MaxEntries { get; set; }
    public int? MyEntries { get; set; }
    public int TotalEntries { get; set; }
    public List<LotteryPrizeDto> Prizes { get; set; } = [];
    public bool HasParticipated { get; set; }
    public bool? IsWinner { get; set; }
}

public record LotteryPrizeDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int Quantity { get; set; }
    public int Rank { get; set; }
}

public class GetLotteriesQueryHandler(ILotteryService lotteryService) : IRequestHandler<GetLotteriesQuery, GetLotteriesQueryResponse>
{
    public async Task<GetLotteriesQueryResponse> Handle(GetLotteriesQuery request, CancellationToken cancellationToken)
    {
        var result = await lotteryService.GetActiveLotteriesAsync(request.PageNumber, request.PageSize, cancellationToken);
        
        var lotteries = result.Items.Select(l => new LotteryDto
        {
            Id = l.Id.ToString(),
            Name = l.Title,
            Description = l.Description ?? string.Empty,
            ImageUrl = null,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30),
            DrawDate = l.DrawDate,
            IsActive = l.IsActive,
            EntryCost = null,
            MaxEntries = l.MaxParticipants,
            MyEntries = 0,
            TotalEntries = l.CurrentParticipants,
            Prizes = [],
            HasParticipated = false,
            IsWinner = null
        }).ToList();
        
        return new GetLotteriesQueryResponse
        {
            Lotteries = new PaginatedList<LotteryDto>(lotteries, result.TotalCount, request.PageNumber, request.PageSize)
        };
    }
}

