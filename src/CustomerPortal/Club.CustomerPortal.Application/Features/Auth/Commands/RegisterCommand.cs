namespace Hyper.CustomerPortal.Application.Features.Auth.Commands;

public record RegisterCommand : IRequest<LoginCommandResponse>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
    public string? NationalCode { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? ReferrerCode { get; set; }
}

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("نام الزامی است");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("نام خانوادگی الزامی است");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("ایمیل نامعتبر است");
        RuleFor(x => x.PhoneNumber).NotEmpty().Matches(@"^09\d{9}$").WithMessage("شماره موبایل نامعتبر است");
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6).WithMessage("رمز عبور باید حداقل 6 کاراکتر باشد");
        RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("رمز عبور و تکرار آن مطابقت ندارند");
    }
}
