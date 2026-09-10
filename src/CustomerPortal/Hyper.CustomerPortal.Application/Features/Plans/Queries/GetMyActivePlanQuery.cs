using Hyper.CustomerPortal.Application.Common;
using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Plans.Queries;

/// <summary>
/// دریافت طرح فعال مشتری
/// </summary>
public record GetMyActivePlanQuery : IRequest<GetMyActivePlanQueryResponse>
{
}

public record GetMyActivePlanQueryResponse
{
    public CustomerPlanDto? ActivePlan { get; set; }
}

public record CustomerPlanDto
{
    public string Id { get; set; } = null!;
    public string PlanId { get; set; } = null!;
    public string PointId { get; set; } = null!;
    public string PointTypeName { get; set; } = null!;
    public string? PointTypeColor { get; set; }
    public string PlanTitle { get; set; } = null!;
    public string PlanDescription { get; set; } = null!;
    public DateTime PurchaseDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string Status { get; set; } = null!;
    public long PaidAmount { get; set; }
    public int UsageCount { get; set; }
    public long TotalDiscountReceived { get; set; }
    public string DiscountType { get; set; } = null!;
    public decimal DiscountValue { get; set; }
    public bool IsGlobalDiscount { get; set; }
    public int? PictureId { get; set; }
    public string? ImageUrl { get; set; }
    public int DaysRemaining { get; set; }
    public bool IsValid { get; set; }
}

public class GetMyActivePlanQueryHandler(
    IPlanService planService,
    ICustomerRequesterUser requesterUser) : IRequestHandler<GetMyActivePlanQuery, GetMyActivePlanQueryResponse>
{
    public async Task<GetMyActivePlanQueryResponse> Handle(GetMyActivePlanQuery request, CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;
        if (customerId <= 0)
        {
            throw new UnauthorizedAccessException("Customer not found");
        }

        var activePlan = await planService.GetActiveCustomerPlanAsync(customerId, cancellationToken);

        if (activePlan == null)
        {
            return new GetMyActivePlanQueryResponse
            {
                ActivePlan = null
            };
        }

        var daysRemaining = (activePlan.ExpiryDate - DateTime.UtcNow).Days;

        var planDto = new CustomerPlanDto
        {
            Id = activePlan.Id.ToString(),
            PlanId = activePlan.PlanId.ToString(),
            PointId = activePlan.PointId.ToString(),
            PointTypeName = activePlan.PointTypeName,
            PointTypeColor = activePlan.PointTypeColor,
            PlanTitle = activePlan.PlanTitle,
            PlanDescription = activePlan.PlanDescription ?? string.Empty,
            PurchaseDate = activePlan.PurchaseDate,
            StartDate = activePlan.StartDate,
            ExpiryDate = activePlan.ExpiryDate,
            Status = activePlan.Status.ToString(),
            PaidAmount = activePlan.PaidAmount,
            UsageCount = activePlan.UsageCount,
            TotalDiscountReceived = activePlan.TotalDiscountReceived,
            DiscountType = activePlan.DiscountType.ToString(),
            DiscountValue = activePlan.DiscountValue,
            IsGlobalDiscount = activePlan.IsGlobalDiscount,
            PictureId = activePlan.PlanPictureId,
            ImageUrl = DocumentUrlHelper.BuildDocumentUrl(activePlan.PlanPictureId),
            DaysRemaining = daysRemaining > 0 ? daysRemaining : 0,
            IsValid = activePlan.IsValid
        };

        return new GetMyActivePlanQueryResponse
        {
            ActivePlan = planDto
        };
    }
}

