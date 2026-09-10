using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Auth.Commands;

public record ChangePasswordCommand : IRequest
{
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
}

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty().WithMessage("رمز عبور فعلی الزامی است");
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(6).WithMessage("رمز عبور جدید باید حداقل 6 کاراکتر باشد");
    }
}


public class ChangePasswordCommandHandler(
    ICustomerService customerService,
    ICustomerRequesterUser requesterUser,
    ILogger<ChangePasswordCommandHandler> logger) : IRequestHandler<ChangePasswordCommand>
{
    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;

        await customerService.ChangePasswordAsync(
            customerId,
            request.CurrentPassword,
            request.NewPassword,
            cancellationToken);

        logger.LogInformation("Password changed successfully for customer {CustomerId}", customerId);
    }
}
