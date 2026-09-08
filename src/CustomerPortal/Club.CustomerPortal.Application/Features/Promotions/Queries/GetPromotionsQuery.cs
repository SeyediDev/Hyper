using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Promotions.Queries;

public record GetPromotionsQuery : IRequest<GetPromotionsQueryResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? CategoryId { get; set; }
    public bool? IsActive { get; set; }
    public string? PromotionType { get; set; }
}

public record GetPromotionsQueryResponse
{
    public PaginatedList<PromotionDto> Promotions { get; set; } = null!;
}

/// <summary>
/// PromotionDto - DTO برای نمایش کمپین به مشتریان
/// </summary>
public record PromotionDto
{
    /// <summary>
    /// شناسه کمپین
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// عنوان کمپین
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// توضیح کامل کمپین
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// توضیح کوتاه برای نمایش سریع
    /// </summary>
    public string? ShortDescription { get; set; }

    /// <summary>
    /// مزایای شرکت در کمپین
    /// </summary>
    public string? Benefits { get; set; }

    /// <summary>
    /// راهنمای شرکت و رفتار موردعلاقه
    /// </summary>
    public string? ParticipationGuide { get; set; }

    /// <summary>
    /// تاریخ شروع
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// تاریخ پایان
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// تعداد روزهای باقی‌مانده تا پایان کمپین
    /// </summary>
    public int DaysUntilEnd { get; set; }

    /// <summary>
    /// تصویر اصلی کمپین
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// تصویر کارت کمپین (نسبت 1:1)
    /// </summary>
    public string? CardImageUrl { get; set; }

    /// <summary>
    /// تصویر بنر کمپین (نسبت 16:9 یا 2:1)
    /// </summary>
    public string? BannerImageUrl { get; set; }

    /// <summary>
    /// آیکن یا نماد کمپین
    /// </summary>
    public string? IconUrl { get; set; }

    /// <summary>
    /// رنگ اصلی کمپین (Hex)
    /// </summary>
    public string? PrimaryColor { get; set; }

    /// <summary>
    /// نوع جایزه
    /// </summary>
    public string? RewardType { get; set; }

    /// <summary>
    /// آیا کمپین فعال است
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// آیا مشتری می‌تواند شرکت کند
    /// </summary>
    public bool CanParticipate { get; set; }

    /// <summary>
    /// دسته‌بندی کمپین
    /// </summary>
    public string? Category { get; set; }
}

public class GetPromotionsQueryHandler(IPromotionService promotionService) : IRequestHandler<GetPromotionsQuery, GetPromotionsQueryResponse>
{
    public async Task<GetPromotionsQueryResponse> Handle(GetPromotionsQuery request, CancellationToken cancellationToken)
    {
        var result = await promotionService.GetActivePromotionsAsync(request.PageNumber, request.PageSize, cancellationToken);
        
        var promotions = result.Items.Select(p => new PromotionDto
        {
            Id = p.Id,
            Title = p.Title,
            Description = p.Description ?? string.Empty,
            ShortDescription = p.ShortDescription,
            Benefits = p.Benefits,
            ParticipationGuide = p.ParticipationGuide,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            DaysUntilEnd = (int)Math.Ceiling((p.EndDate - DateTime.UtcNow).TotalDays),
            ImageUrl = p.ImageUrl,
            CardImageUrl = p.CardImageUrl,
            BannerImageUrl = p.BannerImageUrl,
            IconUrl = p.IconUrl,
            PrimaryColor = p.PrimaryColor,
            RewardType = p.RewardType?.ToString(),
            IsActive = p.IsActive,
            CanParticipate = p.CanParticipate,
            Category = p.Category
        }).ToList();
        
        return new GetPromotionsQueryResponse
        {
            Promotions = new PaginatedList<PromotionDto>(promotions, result.TotalCount, request.PageNumber, request.PageSize)
        };
    }
}
