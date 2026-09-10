using Hyper.CustomerPortal.Application.Common;
using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Plans.Queries;

/// <summary>
/// دریافت لیست طرح‌های اشتراک
/// </summary>
public record GetPlansQuery : IRequest<GetPlansQueryResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool? IsActive { get; set; }
    public string? Search { get; set; }
}

public record GetPlansQueryResponse
{
    public PaginatedList<PlanDto> Plans { get; set; } = null!;
}

public record PlanDto
{
    public string Id { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public long PriceInPoints { get; set; }
    public string PointId { get; set; } = null!;
    public string PointTypeName { get; set; } = null!;
    public string? PointTypeColor { get; set; }
    public int ValidityDays { get; set; }
    public string DiscountType { get; set; } = null!;
    public decimal DiscountValue { get; set; }
    public bool IsGlobalDiscount { get; set; }
    public int? PictureId { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
}

public class GetPlansQueryHandler(IPlanService planService) : IRequestHandler<GetPlansQuery, GetPlansQueryResponse>
{
    public async Task<GetPlansQueryResponse> Handle(GetPlansQuery request, CancellationToken cancellationToken)
    {
        var result = await planService.GetAvailablePlansAsync(
            request.Search,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var plans = result.Items.Select(p => new PlanDto
        {
            Id = p.Id.ToString(),
            Title = p.Title,
            Description = p.Description ?? string.Empty,
            PriceInPoints = p.PriceInPoints,
            PointId = p.PointId.ToString(),
            PointTypeName = p.PointTypeName,
            PointTypeColor = p.PointTypeColor,
            ValidityDays = p.ValidityDays,
            DiscountType = p.DiscountType.ToString(),
            DiscountValue = p.DiscountValue,
            IsGlobalDiscount = p.IsGlobalDiscount,
            PictureId = p.PictureId,
            ImageUrl = DocumentUrlHelper.BuildDocumentUrl(p.PictureId),
            IsActive = p.IsActive
        }).ToList();

        return new GetPlansQueryResponse
        {
            Plans = new PaginatedList<PlanDto>(
                plans,
                result.TotalCount,
                request.PageNumber,
                request.PageSize)
        };
    }
}

