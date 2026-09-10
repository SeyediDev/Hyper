using Hyper.Domain.Features.Promotions;

namespace Hyper.Application.Features.Hyper.Commands;

/// <summary>
/// Command for spinning lottery wheel
/// </summary>
public record SpinLotteryCommand : IRequest<SpinLotteryCommandResponse>
{
    public int LotteryId { get; init; }
    public string CustomerId { get; init; } = null!;
}

public record SpinLotteryCommandResponse
{
    public int ParticipantId { get; init; }
    public bool IsWinner { get; init; }
    public int? RewardId { get; init; }
    public int? RewardAmount { get; init; }
    public string? Message { get; init; }
}

public class SpinLotteryCommandValidator : AbstractValidator<SpinLotteryCommand>
{
    public SpinLotteryCommandValidator()
    {
        RuleFor(x => x.LotteryId)
            .GreaterThan(0)
            .WithMessage("شناسه قرعه‌کشی معتبر نیست");

        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("شناسه مشتری باید مشخص باشد");
    }
}

public class SpinLotteryCommandHandler : IRequestHandler<SpinLotteryCommand, SpinLotteryCommandResponse>
{
    private readonly ILotteryService _lotteryService;
    private readonly ILogger<SpinLotteryCommandHandler> _logger;

    public SpinLotteryCommandHandler(ILotteryService lotteryService, ILogger<SpinLotteryCommandHandler> logger)
    {
        _lotteryService = lotteryService;
        _logger = logger;
    }

    public async Task<SpinLotteryCommandResponse> Handle(SpinLotteryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing SpinLottery command for CustomerId: {CustomerId}, LotteryId: {LotteryId}", 
            request.CustomerId, request.LotteryId);

        var spinRequest = new SpinLotteryRequest
        {
            LotteryId = request.LotteryId,
            CustomerId = request.CustomerId
        };

        var response = await _lotteryService.SpinLottery(spinRequest, cancellationToken);

        return new SpinLotteryCommandResponse
        {
            ParticipantId = response.ParticipantId,
            IsWinner = response.IsWinner,
            RewardId = response.RewardId,
            RewardAmount = response.RewardAmount,
            Message = response.Message
        };
    }
}

