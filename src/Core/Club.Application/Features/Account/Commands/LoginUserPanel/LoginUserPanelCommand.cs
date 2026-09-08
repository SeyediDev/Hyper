//using Neo.Application.Exceptions;
//using Neo.Domain.Features;
//using Neo.Domain.Features.Recaptcha;
//using Neo.Domain.Repository;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.Hosting;

//namespace Hyper.Application.Features.Account.Commands.LoginUserPanel;

//public record LoginUserPanelCommand : IRequest<LoginUserPanelCommandResponse>
//{
//    public required string Mobile { get; set; }
//    public required int CountryCode { get; set; }
//    public required string Token { get; set; }
//}

//public record LoginUserPanelCommandResponse(int Timeout)
//{ }

//public class LoginUserPanelCommandValidator : AbstractValidator<LoginUserPanelCommand>
//{
//    public LoginUserPanelCommandValidator()
//    {
//        RuleFor(x => x.Mobile)
//                   .Cascade(CascadeMode.Stop)
//                   .NotEmpty().WithMessage("الزامی می باشد.")
//                   .Matches(@"^0?[1-9][0-9]{9}$").WithMessage("شماره موبایل نامعتبر است.");
//        RuleFor(x => x.CountryCode)
//                  .NotEmpty().WithMessage("الزامی می باشد.");
//        RuleFor(x => x.Token)
//             .NotEmpty().WithMessage("الزامی می باشد.");
//    }
//}

//public class LoginUserPanelCommandHandler(IWebHostEnvironment env,
//                                          //IQueryRepository<UserOffice, int> userOfficeQueryRepository,
//                                          IRecaptchaService recaptchaService,
//                                          IOtpService otpService,
//                                          IMultiLingual multiLingual)
//    : IRequestHandler<LoginUserPanelCommand, LoginUserPanelCommandResponse>
//{
//    public async Task<LoginUserPanelCommandResponse> Handle(LoginUserPanelCommand request, CancellationToken cancellationToken)
//    {
//        if (env.IsProduction())
//        {
//            var recaptchaValidation = await recaptchaService.CaptchaValidation(request.Token);
//            if (!recaptchaValidation)
//            {
//                throw new UnauthorizedAccessException("درخواست نامعتبر است.");
//            }
//        }
//        //var userOffice = await userOfficeQueryRepository.FirstOrDefaultWithIncludeAsync(x => x.User, x => x.User.Mobile == long.Parse(request.Mobile), cancellationToken);
//        //if (userOffice is null)
//        //{
//        //    throw new UnauthorizedAccessException("کاربری تعریف نشده است.");
//        //}
//        if (userOffice.User.OTPSeed is null)
//            throw new BadRequestException("OTPSeed is null in database");
//        var result = await otpService.SendAsync(request.Mobile, userOffice.User.OTPSeed, multiLingual.GetMessage("OtpMessageTemplate"));
//        return new LoginUserPanelCommandResponse((int)(result - DateTimeOffset.Now).TotalSeconds);
//    }
//}
