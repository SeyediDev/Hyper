using Neo.Application.Exceptions;
using Neo.Domain.Features.Client.Dto;
using Neo.Domain.Features.Sms;

namespace Hyper.Application.Features.Account.Commands.VerifyLogin;

public record VerifyLoginCommand : IRequest<VerifyLoginCommandResponse>
{
    public required string Mobile { get; set; }
    public required int CountryCode { get; set; }
    public required string Code { get; set; }
    public string? DeviceCode { get; set; }
    public string? Platform { get; set; }
}

public record VerifyLoginCommandResponse
{
    public TokenResponseDto? Token { get; set; }
}

public class VerifyLoginCommandValidator : AbstractValidator<VerifyLoginCommand>
{
    public VerifyLoginCommandValidator()
    {
        _ = RuleFor(x => x.Mobile)
                  .Cascade(CascadeMode.Stop)
                  .NotEmpty().WithMessage("الزامی می باشد.")
                  .Matches(@"^0?[1-9][0-9]{9}$").WithMessage("شماره موبایل نامعتبر است.");
        _ = RuleFor(x => x.CountryCode)
                  .NotEmpty().WithMessage("الزامی می باشد.");
        _ = RuleFor(x => x.Code)
                   .NotEmpty().WithMessage("الزامی می باشد.");

        _ = RuleFor(x => x)
            .Custom((model, context) =>
            {
                var hasDeviceCode = !string.IsNullOrWhiteSpace(model.DeviceCode);
                var hasPlatform = !string.IsNullOrWhiteSpace(model.Platform);

                if (hasDeviceCode && !hasPlatform)
                {
                    context.AddFailure(nameof(model.Platform), "Platform is required when DeviceCode is provided.");
                }

                if (!hasDeviceCode && hasPlatform)
                {
                    context.AddFailure(nameof(model.DeviceCode), "DeviceCode is required when Platform is provided.");
                }
            });
    }
}

public class VerifyLoginCommandHandler(IRequesterUser requesterUser,
                                       IOtpService otpService,
                                       IIdpService idpService,
                                       ILoginUserService<UserId> userService
                                       //, IUserQueryRepository userQueryRepository,
                                       //IPublishEndpoint publisher
    )
        : IRequestHandler<VerifyLoginCommand, VerifyLoginCommandResponse>
{
    public async Task<VerifyLoginCommandResponse> Handle(VerifyLoginCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(requesterUser.AppName) || (requesterUser.AppName != "member" && requesterUser.AppName != "doctor" && requesterUser.AppName != "office"))
        {
            throw new BadRequestException("The 'x-app-name' header must be either 'member' or 'doctor'.");
        }
        request.Mobile = request.Mobile.PadLeft(11, '0');
        Neo.Domain.Entities.IUser<UserId> user = await userService.FindUser(request.Mobile, request.CountryCode, cancellationToken)
            ?? throw new BadRequestException("کاربر نامعتبر است.");
        if (user.OTPSeed is null)
        {
            throw new Exception("OTPSeed is null in database");
        }

        bool isVerified = otpService.Verify(user.OTPSeed, request.Code);
        if (isVerified)
        {
            if (!string.IsNullOrWhiteSpace(request.DeviceCode)) // Register fcm
            {
                //await publisher.Publish(new FcmRegisterDto
                //{
                //    UserId = user.Id,
                //    AppId = userType,
                //    DeviceCode = request.DeviceCode,
                //    Platform = request.Platform!
                //}, cancellationToken);
            }
            await userService.VerifyUser(request.Mobile, request.CountryCode, cancellationToken);
            VerifyLoginCommandResponse model = new()
            {
                Token = await idpService.GetUserTokenAsync(request.Mobile, cancellationToken),
            };
            return model;
        }
        throw new BadRequestException("کد نامعتبر است.");
    }
}
