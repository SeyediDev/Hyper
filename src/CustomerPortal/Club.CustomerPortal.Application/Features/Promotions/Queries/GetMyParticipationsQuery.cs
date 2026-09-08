using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Promotions.Queries;

public record GetMyParticipationsQuery : IRequest<GetMyParticipationsQueryResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Status { get; set; }
}

public record GetMyParticipationsQueryResponse
{
    public PaginatedList<PromotionParticipationDto> Participations { get; set; } = null!;
}

public record PromotionParticipationDto
{
    public PromotionDto Promotion { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime ParticipatedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
}

public class GetMyParticipationsQueryHandler(
    IPromotionService promotionService,
    ICustomerRequesterUser requesterUser) : IRequestHandler<GetMyParticipationsQuery, GetMyParticipationsQueryResponse>
{
    public async Task<GetMyParticipationsQueryResponse> Handle(GetMyParticipationsQuery request, CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;
        var result = await promotionService.GetMyParticipationsAsync(customerId, request.PageNumber, request.PageSize, cancellationToken);
        
        var participations = result.Items.Select(p => new PromotionParticipationDto
        {
            Promotion = new PromotionDto
            {
                Id = p.PromotionId,
                Title = p.PromotionTitle,
                Category = p.PromotionCategorty.ToString(),
                Description = string.Empty,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),
                DaysUntilEnd = 30,
                IsActive = true,
                CanParticipate = false
            },
            Status = p.Status,
            ParticipatedDate = p.ParticipatedAt,
            CompletedDate = null
        }).ToList();
        
        return new GetMyParticipationsQueryResponse
        {
            Participations = new PaginatedList<PromotionParticipationDto>(participations, result.TotalCount, request.PageNumber, request.PageSize)
        };
    }
}

