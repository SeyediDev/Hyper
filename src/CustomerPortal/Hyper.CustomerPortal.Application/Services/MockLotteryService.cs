using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Services;

public class MockLotteryService : ILotteryService
{
    public Task<PaginatedList<LotteryDto>> GetActiveLotteriesAsync(int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var lotteries = new List<LotteryDto>
        {
            new() { Id = 1, Title = "قرعه‌کشی ماهانه", Description = "برنده شوید!", DrawDate = DateTime.UtcNow.AddDays(10), RequiredPoints = 1000, MaxParticipants = 100, CurrentParticipants = 45, IsActive = true, HasDrawn = false }
        };

        return Task.FromResult(new PaginatedList<LotteryDto>(lotteries, 1, pageNumber, pageSize));
    }

    public Task<LotteryDto?> GetLotteryByIdAsync(int lotteryId, CancellationToken cancellationToken = default)
    {
        var lottery = new LotteryDto
        {
            Id = lotteryId,
            Title = "قرعه‌کشی ماهانه",
            Description = "برنده شوید!",
            DrawDate = DateTime.UtcNow.AddDays(10),
            RequiredPoints = 1000,
            MaxParticipants = 100,
            CurrentParticipants = 45,
            IsActive = true,
            HasDrawn = false
        };

        return Task.FromResult<LotteryDto?>(lottery);
    }

    public Task<LotteryParticipationResultDto> ParticipateInLotteryAsync(int customerId, int lotteryId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new LotteryParticipationResultDto
        {
            Success = true,
            Message = "شما با موفقیت در قرعه‌کشی شرکت کردید",
            TicketNumber = $"TICKET-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}"
        });
    }

    public Task<IEnumerable<LotteryWinnerDto>> GetLotteryResultsAsync(int lotteryId, CancellationToken cancellationToken = default)
    {
        var winners = new List<LotteryWinnerDto>();
        return Task.FromResult<IEnumerable<LotteryWinnerDto>>(winners);
    }

    public Task<PaginatedList<MyLotteryParticipationDto>> GetMyParticipationsAsync(int customerId, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var participations = new List<MyLotteryParticipationDto>();
        return Task.FromResult(new PaginatedList<MyLotteryParticipationDto>(participations, 0, pageNumber, pageSize));
    }
}

