using Hyper.CustomerPortal.Application.Common;
using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Plans.Queries;

/// <summary>
/// دریافت جزئیات یک طرح اشتراک
/// </summary>
public record GetPlanByIdQuery : IRequest<GetPlanByIdQueryResponse>
{
    public string PlanId { get; set; } = null!;
}

public record GetPlanByIdQueryResponse
{
    public PlanDetailDto Plan { get; set; } = null!;
}

public record PlanDetailDto
{
    public string Id { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public long PriceInPoints { get; set; }
    public string PointTypeName { get; set; } = null!;
    public string? PointTypeColor { get; set; }
    public int ValidityDays { get; set; }
    public string DiscountType { get; set; } = null!;
    public decimal DiscountValue { get; set; }
    public bool IsGlobalDiscount { get; set; }
    public int? PictureId { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GetPlanByIdQueryHandler(IPlanService planService) : IRequestHandler<GetPlanByIdQuery, GetPlanByIdQueryResponse>
{
    public async Task<GetPlanByIdQueryResponse> Handle(GetPlanByIdQuery request, CancellationToken cancellationToken)
    {
        var planId = int.Parse(request.PlanId);
        var plan = await planService.GetPlanByIdAsync(planId, cancellationToken);

        if (plan == null)
        {
            throw new InvalidOperationException($"Plan with id {request.PlanId} not found");
        }

        var planDto = new PlanDetailDto
        {
            Id = plan.Id.ToString(),
            Title = plan.Title,
            Description = plan.Description ?? string.Empty,
            PriceInPoints = plan.PriceInPoints,
            PointTypeName = plan.PointTypeName,
            PointTypeColor = plan.PointTypeColor,
            ValidityDays = plan.ValidityDays,
            DiscountType = plan.DiscountType.ToString(),
            DiscountValue = plan.DiscountValue,
            IsGlobalDiscount = plan.IsGlobalDiscount,
            PictureId = plan.PictureId,
            ImageUrl = DocumentUrlHelper.BuildDocumentUrl(plan.PictureId),
            IsActive = plan.IsActive,
            CreatedAt = plan.CreatedAt
        };

        return new GetPlanByIdQueryResponse
        {
            Plan = planDto
        };
    }
}

