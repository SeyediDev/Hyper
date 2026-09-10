namespace Hyper.CustomerPortal.Application.Interfaces;

/// <summary>
/// سرویس مدیریت تجربه چرخونه وفاداری
/// </summary>
public interface IWheelService
{
    Task<WheelExperienceDto?> GetActiveWheelAsync(
        int customerId,
        CancellationToken cancellationToken = default);

    Task<WheelSpinResultDto> SpinAsync(
        int customerId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WheelSpinHistoryDto>> GetRecentSpinsAsync(
        int customerId,
        int take = 10,
        CancellationToken cancellationToken = default);
}

public record WheelExperienceDto
{
    public int Id { get; init; }
    public string Title { get; init; } = null!;
    public string? Description { get; init; }
    public string? Subtitle { get; init; }
    public string Theme { get; init; } = "sunset";
    public string ButtonLabel { get; init; } = "شروع چرخش";
    public string BackgroundColor { get; init; } = "#FDF2F8";
    public string? CenterIcon { get; init; }
    public string? CelebrationMessage { get; init; }
    public string? CallToAction { get; init; }
    public int SpinDurationSeconds { get; init; }
    public int MaxDailySpins { get; init; }
    public int? MaxTotalSpins { get; init; }
    public int SpinsUsedToday { get; init; }
    public int TotalSpins { get; init; }
    public IReadOnlyList<WheelSegmentDto> Segments { get; init; } = Array.Empty<WheelSegmentDto>();
}

public record WheelSegmentDto
{
    public int Id { get; init; }
    public string Label { get; init; } = null!;
    public string Color { get; init; } = "#f97316";
    public string TextColor { get; init; } = "#ffffff";
    public string? Icon { get; init; }
    public string? Message { get; init; }
    public bool IsJackpot { get; init; }
    public int WinRate { get; init; }
    public decimal Probability { get; init; }
    public RewardSummaryDto? Reward { get; init; }
}

public record RewardSummaryDto
{
    public int Id { get; init; }
    public string Title { get; init; } = null!;
    public int? PictureId { get; init; }
}

public record WheelSpinResultDto
{
    public WheelSegmentDto Segment { get; init; } = null!;
    public DateTime OccurredAt { get; init; }
    public int SpinsUsedToday { get; init; }
    public int RemainingDailySpins { get; init; }
    public int TotalSpins { get; init; }
    public string? TicketNumber { get; init; }
    public bool HasReward => Segment.Reward is not null;
}

public record WheelSpinHistoryDto
{
    public string SegmentLabel { get; init; } = null!;
    public string SegmentColor { get; init; } = "#f97316";
    public string? SegmentIcon { get; init; }
    public bool IsWinner { get; init; }
    public string? RewardTitle { get; init; }
    public DateTime SpunAt { get; init; }
    public string? TicketNumber { get; init; }
}





