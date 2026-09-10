using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Lotteries.Queries;

public record GetLotteryResultsQuery : IRequest<GetLotteryResultsQueryResponse>
{
    public string LotteryId { get; set; } = null!;
}

public record GetLotteryResultsQueryResponse
{
    public List<LotteryWinnerDto> Winners { get; set; } = [];
    public DateTime DrawDate { get; set; }
}

public record LotteryWinnerDto
{
    public int Rank { get; set; }
    public string Prize { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
    public int EntryCount { get; set; }
}

public class GetLotteryResultsQueryHandler(ILotteryService lotteryService) : IRequestHandler<GetLotteryResultsQuery, GetLotteryResultsQueryResponse>
{
    public async Task<GetLotteryResultsQueryResponse> Handle(GetLotteryResultsQuery request, CancellationToken cancellationToken)
    {
        var winners = await lotteryService.GetLotteryResultsAsync(int.Parse(request.LotteryId), cancellationToken);
        
        var winnerDtos = winners.Select((w, index) => new LotteryWinnerDto
        {
            Rank = index + 1,
            Prize = w.RewardTitle,
            CustomerName = w.CustomerName,
            EntryCount = 1
        }).ToList();
        
        return new GetLotteryResultsQueryResponse
        {
            Winners = winnerDtos,
            DrawDate = DateTime.UtcNow
        };
    }
}

