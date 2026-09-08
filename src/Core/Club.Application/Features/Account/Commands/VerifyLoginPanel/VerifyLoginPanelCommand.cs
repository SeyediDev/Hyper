//using Neo.Application.Exceptions;
//using Neo.Domain.Features;
//using Neo.Domain.Features.Idp;
//using Neo.Domain.Features.Idp.Dto;
//using Neo.Domain.Features.Recaptcha;
//using Neo.Domain.Repository;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.Hosting;

//namespace Hyper.Application.Features.Account.Commands.VerifyLoginPanel;

//public record VerifyLoginPanelCommand : IRequest<VerifyLoginPanelCommandResponse>
//{
//    public required string Mobile { get; set; }
//    public required int CountryCode { get; set; }
//    public required string Otp { get; set; }
//    public required string Token { get; set; }
//}

//public record VerifyLoginPanelCommandResponse
//{
//    public TokenResponseDto? Token { get; set; }
//}

//public class VerifyLoginCommandValidator : AbstractValidator<VerifyLoginPanelCommand>
//{
//    public VerifyLoginCommandValidator()
//    {
//        _ = RuleFor(x => x.Mobile)
//                  .Cascade(CascadeMode.Stop)
//                  .NotEmpty().WithMessage("الزامی می باشد.")
//                  .Matches(@"^0?[1-9][0-9]{9}$").WithMessage("شماره موبایل نامعتبر است.");
//        _ = RuleFor(x => x.CountryCode)
//                  .NotEmpty().WithMessage("الزامی می باشد.");
//        _ = RuleFor(x => x.Otp)
//                   .NotEmpty().WithMessage("الزامی می باشد.");
//        _ = RuleFor(x => x.Token)
//                .NotEmpty().WithMessage("الزامی می باشد.");
//    }
//}

//public class VerifyLoginPanelCommandHandler(IWebHostEnvironment env,
//                                           IOtpService otpService,
//                                           IIdpService idpService,
//                                           IRecaptchaService recaptchaService
//                                           //, IQueryRepository<UserOffice, int> userOfficeQueryRepository
//    )
//        : IRequestHandler<VerifyLoginPanelCommand, VerifyLoginPanelCommandResponse>
//{
//    public async Task<VerifyLoginPanelCommandResponse> Handle(VerifyLoginPanelCommand request, CancellationToken cancellationToken)
//    {
//        if (env.IsProduction())
//        {
//            var recaptchaValidation = await recaptchaService.CaptchaValidation(request.Token);
//            if (!recaptchaValidation)
//            {
//                throw new UnauthorizedAccessException("درخواست نامعتبر است.");
//            }
//        }
//        var userOffice = await userOfficeQueryRepository.FirstOrDefaultWithIncludeAsync(x => x.User, x => x.User.Mobile == long.Parse(request.Mobile), cancellationToken);
//        if (userOffice is null)
//        {
//            throw new UnauthorizedAccessException("کاربری تعریف نشده است.");
//        }

//        bool isVerified = otpService.Verify(userOffice.User.OTPSeed!, request.Otp);
//        if (!isVerified)
//        {
//            throw new BadRequestException("کد نامعتبر است.");
//        }
//        VerifyLoginPanelCommandResponse model = new()
//        {
//            Token = await idpService.GetUserTokenAsync(request.Mobile, cancellationToken),
//        };
//        return model;
//    }
//}
