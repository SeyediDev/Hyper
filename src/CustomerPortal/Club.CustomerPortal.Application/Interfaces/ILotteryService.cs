namespace Hyper.CustomerPortal.Application.Interfaces;

/// <summary>
/// سرویس مدیریت قرعه‌کشی‌ها
/// </summary>
public interface ILotteryService
{
    /// <summary>
    /// دریافت لیست قرعه‌کشی‌های فعال
    /// </summary>
    Task<PaginatedList<LotteryDto>> GetActiveLotteriesAsync(
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// دریافت جزئیات قرعه‌کشی
    /// </summary>
    Task<LotteryDto?> GetLotteryByIdAsync(int lotteryId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// شرکت در قرعه‌کشی
    /// </summary>
    Task<LotteryParticipationResultDto> ParticipateInLotteryAsync(
        int customerId,
        int lotteryId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// دریافت نتایج قرعه‌کشی
    /// </summary>
    Task<IEnumerable<LotteryWinnerDto>> GetLotteryResultsAsync(
        int lotteryId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// دریافت قرعه‌کشی‌هایی که مشتری شرکت کرده
    /// </summary>
    Task<PaginatedList<MyLotteryParticipationDto>> GetMyParticipationsAsync(
        int customerId,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
}

public record LotteryDto
{
    public int Id { get; init; }
    public string Title { get; init; } = null!;
    public string? Description { get; init; }
    public DateTime DrawDate { get; init; }
    public long RequiredPoints { get; init; }
    public int MaxParticipants { get; init; }
    public int CurrentParticipants { get; init; }
    public bool IsActive { get; init; }
    public bool HasDrawn { get; init; }
}

public record LotteryParticipationResultDto
{
    public bool Success { get; init; }
    public string Message { get; init; } = null!;
    public string? TicketNumber { get; init; }
}

public record LotteryWinnerDto
{
    public string CustomerName { get; init; } = null!;
    public string RewardTitle { get; init; } = null!;
    public DateTime WonAt { get; init; }
}

public record MyLotteryParticipationDto
{
    public int LotteryId { get; init; }
    public string LotteryTitle { get; init; } = null!;
    public string? TicketNumber { get; init; }
    public DateTime ParticipatedAt { get; init; }
    public bool IsWinner { get; init; }
    public string? PrizeName { get; init; }
}

