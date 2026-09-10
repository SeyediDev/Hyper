using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Lotteries.Queries;

public record GetLotteryByIdQuery : IRequest<GetLotteryByIdQueryResponse>
{
    public string Id { get; set; } = null!;
}

public record GetLotteryByIdQueryResponse
{
    public LotteryDto Lottery { get; set; } = null!;
}

public class GetLotteryByIdQueryHandler(ILotteryService lotteryService) : IRequestHandler<GetLotteryByIdQuery, GetLotteryByIdQueryResponse>
{
    public async Task<GetLotteryByIdQueryResponse> Handle(GetLotteryByIdQuery request, CancellationToken cancellationToken)
    {
        var lottery = await lotteryService.GetLotteryByIdAsync(int.Parse(request.Id), cancellationToken);
        
        if (lottery == null)
        {
            throw new InvalidOperationException("قرعه‌کشی یافت نشد");
        }
        
        return new GetLotteryByIdQueryResponse
        {
            Lottery = new LotteryDto
            {
                Id = lottery.Id.ToString(),
                Name = lottery.Title,
                Description = lottery.Description ?? string.Empty,
                ImageUrl = null,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),
                DrawDate = lottery.DrawDate,
                IsActive = lottery.IsActive,
                EntryCost = null,
                MaxEntries = lottery.MaxParticipants,
                MyEntries = 0,
                TotalEntries = lottery.CurrentParticipants,
                Prizes = [],
                HasParticipated = lottery.HasDrawn,
                IsWinner = null
            }
        };
    }
}

