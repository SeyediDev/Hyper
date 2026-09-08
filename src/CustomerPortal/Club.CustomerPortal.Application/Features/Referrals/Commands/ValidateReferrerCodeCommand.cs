using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Referrals.Commands;

public record ValidateReferrerCodeCommand : IRequest<ValidateReferrerCodeCommandResponse>
{
    public required string Code { get; set; }
}

public record ValidateReferrerCodeCommandResponse
{
    public bool IsValid { get; set; }
    public string? ReferrerName { get; set; }
}

public class ValidateReferrerCodeCommandValidator : AbstractValidator<ValidateReferrerCodeCommand>
{
    public ValidateReferrerCodeCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().WithMessage("کد معرف الزامی است");
    }
}

public class ValidateReferrerCodeCommandHandler(IReferralService referralService) : IRequestHandler<ValidateReferrerCodeCommand, ValidateReferrerCodeCommandResponse>
{
    public async Task<ValidateReferrerCodeCommandResponse> Handle(ValidateReferrerCodeCommand request, CancellationToken cancellationToken)
    {
        var isValid = await referralService.ValidateReferrerCodeAsync(request.Code, cancellationToken);
        return new ValidateReferrerCodeCommandResponse
        {
            IsValid = isValid,
            ReferrerName = isValid ? "مشتری گرامی" : null
        };
    }
}

