using Hyper.Domain.Entities.Promotions.Enums;

namespace Hyper.CustomerPortal.Application.Interfaces;

/// <summary>
/// سرویس مدیریت کمپین‌ها و جشنواره‌ها
/// </summary>
public interface IPromotionService
{
    /// <summary>
    /// دریافت لیست کمپین‌های فعال
    /// </summary>
    Task<PaginatedList<PromotionDto>> GetActivePromotionsAsync(
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// دریافت جزئیات کمپین
    /// </summary>
    Task<PromotionDto?> GetPromotionByIdAsync(int promotionId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// شرکت در کمپین
    /// </summary>
    Task<ParticipationResultDto> ParticipateAsync(
        int customerId,
        int promotionId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// دریافت لیست کمپین‌هایی که مشتری شرکت کرده
    /// </summary>
    Task<PaginatedList<MyParticipationDto>> GetMyParticipationsAsync(
        int customerId,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
}

public record PromotionDto
{
    public int Id { get; init; }
    public string Title { get; init; } = null!;
    public string? Description { get; init; }
    public string? ShortDescription { get; init; }
    public string? Benefits { get; init; }
    public string? ParticipationGuide { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public string? ImageUrl { get; init; }
    public string? CardImageUrl { get; init; }
    public string? BannerImageUrl { get; init; }
    public string? IconUrl { get; init; }
    public string? PrimaryColor { get; init; }
    public string? RewardType { get; init; }
    public bool IsActive { get; init; }
    public bool CanParticipate { get; init; }
    public int DaysUntilEnd { get; init; }
    public string? Category { get; init; }
}

public record ParticipationResultDto
{
    public bool Success { get; init; }
    public string Message { get; init; } = null!;
}

public record MyParticipationDto
{
    public int PromotionId { get; init; }
    public string PromotionTitle { get; init; } = null!;
    public DateTime ParticipatedAt { get; init; }
    public string Status { get; init; } = null!;
    public PromotionCategory PromotionCategorty { get; set; }
}

