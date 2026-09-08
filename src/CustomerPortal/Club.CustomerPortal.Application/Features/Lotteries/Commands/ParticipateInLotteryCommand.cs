using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Lotteries.Commands;

public record ParticipateInLotteryCommand : IRequest<ParticipateInLotteryCommandResponse>
{
    public string LotteryId { get; set; } = null!;
    public int Entries { get; set; } = 1;
}

public record ParticipateInLotteryCommandResponse
{
    public int EntryCount { get; set; }
    public int TotalEntries { get; set; }
}

public class ParticipateInLotteryCommandValidator : AbstractValidator<ParticipateInLotteryCommand>
{
    public ParticipateInLotteryCommandValidator()
    {
        RuleFor(x => x.LotteryId).NotEmpty().WithMessage("شناسه قرعه‌کشی الزامی است");
        RuleFor(x => x.Entries).GreaterThan(0).WithMessage("تعداد شانس باید بیشتر از صفر باشد");
    }
}

public class ParticipateInLotteryCommandHandler(
    ILotteryService lotteryService,
    ICustomerRequesterUser requesterUser,
    ILogger<ParticipateInLotteryCommandHandler> logger) : IRequestHandler<ParticipateInLotteryCommand, ParticipateInLotteryCommandResponse>
{
    public async Task<ParticipateInLotteryCommandResponse> Handle(ParticipateInLotteryCommand request, CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;
        var result = await lotteryService.ParticipateInLotteryAsync(customerId, int.Parse(request.LotteryId), cancellationToken);
        
        if (!result.Success)
        {
            throw new InvalidOperationException(result.Message);
        }
        
        logger.LogInformation("Customer {CustomerId} participated in lottery {LotteryId}", customerId, request.LotteryId);
        
        return new ParticipateInLotteryCommandResponse
        {
            EntryCount = request.Entries,
            TotalEntries = request.Entries
        };
    }
}

