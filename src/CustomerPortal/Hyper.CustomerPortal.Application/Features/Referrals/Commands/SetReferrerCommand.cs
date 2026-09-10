using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Referrals.Commands;

public record SetReferrerCommand : IRequest
{
    public required string ReferrerCode { get; set; }
}

public class SetReferrerCommandValidator : AbstractValidator<SetReferrerCommand>
{
    public SetReferrerCommandValidator()
    {
        RuleFor(x => x.ReferrerCode).NotEmpty().WithMessage("کد معرف الزامی است");
    }
}

public class SetReferrerCommandHandler(
    IReferralService referralService,
    ICustomerRequesterUser requesterUser,
    ILogger<SetReferrerCommandHandler> logger)
    : IRequestHandler<SetReferrerCommand>
{
    public async Task Handle(SetReferrerCommand request, CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;
        await referralService.SetReferrerAsync(customerId, request.ReferrerCode, cancellationToken);
        logger.LogInformation("Referrer set for customer {CustomerId}: {Code}", customerId, request.ReferrerCode);
    }
}

