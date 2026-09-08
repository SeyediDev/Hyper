using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Plans.Commands;

/// <summary>
/// خرید طرح اشتراک توسط مشتری
/// </summary>
public record PurchasePlanCommand : IRequest<PurchasePlanCommandResponse>
{
    public string PlanId { get; set; } = null!;
}

public record PurchasePlanCommandResponse
{
    public string CustomerPlanId { get; set; } = null!;
    public string Message { get; set; } = null!;
    public DateTime ExpiryDate { get; set; }
}

public class PurchasePlanCommandHandler(
    IPlanService planService,
    ICustomerRequesterUser requesterUser) : IRequestHandler<PurchasePlanCommand, PurchasePlanCommandResponse>
{
    public async Task<PurchasePlanCommandResponse> Handle(PurchasePlanCommand request, CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;
        if (customerId <= 0)
        {
            throw new UnauthorizedAccessException("Customer not found");
        }

        var planId = int.Parse(request.PlanId);

        var result = await planService.PurchasePlanAsync(customerId, planId, cancellationToken);

        return new PurchasePlanCommandResponse
        {
            CustomerPlanId = result.CustomerPlanId.ToString(),
            Message = "طرح با موفقیت خریداری شد",
            ExpiryDate = result.ExpiryDate
        };
    }
}

