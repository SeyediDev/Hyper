using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Promotions.Queries;

public record GetPromotionByIdQuery : IRequest<GetPromotionByIdQueryResponse>
{
    public string Id { get; set; } = null!;
}

public record GetPromotionByIdQueryResponse
{
    public PromotionDto Promotion { get; set; } = null!;
}

public class GetPromotionByIdQueryHandler(IPromotionService promotionService) : IRequestHandler<GetPromotionByIdQuery, GetPromotionByIdQueryResponse>
{
    public async Task<GetPromotionByIdQueryResponse> Handle(GetPromotionByIdQuery request, CancellationToken cancellationToken)
    {
        var promotion = await promotionService.GetPromotionByIdAsync(int.Parse(request.Id), cancellationToken);
        
        if (promotion == null)
        {
            throw new InvalidOperationException("کمپین یافت نشد");
        }
        
        return new GetPromotionByIdQueryResponse
        {
            Promotion = new PromotionDto
            {
                Id = promotion.Id,
                Title = promotion.Title,
                Description = promotion.Description ?? string.Empty,
                ShortDescription = promotion.ShortDescription,
                Benefits = promotion.Benefits,
                ParticipationGuide = promotion.ParticipationGuide,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate,
                DaysUntilEnd = promotion.DaysUntilEnd,
                ImageUrl = promotion.ImageUrl,
                CardImageUrl = promotion.CardImageUrl,
                BannerImageUrl = promotion.BannerImageUrl,
                IconUrl = promotion.IconUrl,
                PrimaryColor = promotion.PrimaryColor,
                RewardType = promotion.RewardType,
                IsActive = promotion.IsActive,
                CanParticipate = promotion.CanParticipate,
                Category = promotion.Category
            }
        };
    }
}

