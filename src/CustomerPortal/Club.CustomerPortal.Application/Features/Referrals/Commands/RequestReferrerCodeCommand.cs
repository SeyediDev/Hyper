using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Referrals.Commands;

public record RequestReferrerCodeCommand : IRequest;

public class RequestReferrerCodeCommandHandler(
    IReferralService referralService,
    ICustomerRequesterUser requesterUser,
    ILogger<RequestReferrerCodeCommandHandler> logger)
    : IRequestHandler<RequestReferrerCodeCommand>
{
    public async Task Handle(RequestReferrerCodeCommand request, CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;
        var code = await referralService.RequestReferrerCodeAsync(customerId, cancellationToken);
        logger.LogInformation("Referrer code requested for customer {CustomerId}: {Code}", customerId, code);
    }
}

