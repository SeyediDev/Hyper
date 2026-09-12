using Neo.Application.Exceptions;
using Neo.Domain.Entities.Common;

namespace Hyper.Application.Features.Account.Commands.LoginUser;

public record LoginUserCommand : IRequest<LoginUserCommandResponse>
{
    public required string Mobile { get; set; }
    public required int CountryCode { get; set; }
    public Func<Neo.Domain.Entities.IUser<UserId>,Task>? SetUseParametersInRegistration { get; set; }
}

public record LoginUserCommandResponse(DateTimeOffset Timeout)
{ }

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        _ = RuleFor(x => x.Mobile)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("الزامی می باشد.")
                   .Matches(@"^0?[1-9][0-9]{9}$").WithMessage("شماره موبایل نامعتبر است.");
        _ = RuleFor(x => x.CountryCode)
                  .NotEmpty().WithMessage("الزامی می باشد.");
    }
}

public class LoginUserCommandHandler(ILoginUserService<UserId> userService, IOtpService otpService, IRequesterUser requesterUser, IMultiLingualService multiLingual)
    : IRequestHandler<LoginUserCommand, LoginUserCommandResponse>
{
    public async Task<LoginUserCommandResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        Neo.Domain.Entities.IUser<UserId> user = await userService.FindUser(request.Mobile, request.CountryCode, cancellationToken) ??
            await userService.RegisterUser(request.Mobile, request.CountryCode, otpService.GetNewOtpSeed(), 
                                           request.SetUseParametersInRegistration, cancellationToken);
        requesterUser.Id = user.Id;
        if (user.OTPSeed is null)
        {
            throw new BadRequestException("OTPSeed is null in database");
        }

        DateTimeOffset result = await otpService.SendAsync(request.Mobile, user.OTPSeed, multiLingual.GetMessage("OtpMessageTemplate"));
        return new LoginUserCommandResponse(result);
    }
}
