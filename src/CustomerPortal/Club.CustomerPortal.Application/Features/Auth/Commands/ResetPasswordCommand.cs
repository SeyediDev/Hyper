using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Auth.Commands;

public record ResetPasswordCommand : IRequest
{
    public required string Token { get; set; }
    public required string NewPassword { get; set; }
}

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Token).NotEmpty().WithMessage("Token الزامی است");
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(6).WithMessage("رمز عبور باید حداقل 6 کاراکتر باشد");
    }
}

public class ResetPasswordCommandHandler(
    ICustomerService customerService,
    ILogger<ResetPasswordCommandHandler> logger) : IRequestHandler<ResetPasswordCommand>
{
    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        await customerService.ResetPasswordAsync(request.Token, request.NewPassword, cancellationToken);
        
        logger.LogInformation("Password reset successfully");
    }
}

