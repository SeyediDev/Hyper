using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Auth.Commands;

public record LoginCommand : IRequest<LoginCommandResponse>
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public bool RememberMe { get; set; }
}

public record LoginCommandResponse
{
    public required string Token { get; set; }
    public required string RefreshToken { get; set; }
    public int ExpiresIn { get; set; }
    public required CustomerDto Customer { get; set; }
}

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage("نام کاربری الزامی است");
        RuleFor(x => x.Password).NotEmpty().WithMessage("رمز عبور الزامی است");
    }
}

public class LoginCommandHandler(
    ICustomerService customerService,
    IAuthenticationService authenticationService,
    ILogger<LoginCommandHandler> logger) : IRequestHandler<LoginCommand, LoginCommandResponse>
{
    public async Task<LoginCommandResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // 1. یافتن مشتری با username (phone number یا email)
        var customer = await customerService.GetCustomerByPhoneAsync(request.Username, cancellationToken)
                      ?? await customerService.GetCustomerByEmailAsync(request.Username, cancellationToken);

        if (customer == null)
        {
            logger.LogWarning("Login failed: Customer not found with username {Username}", request.Username);
            throw new UnauthorizedAccessException("نام کاربری یا رمز عبور اشتباه است");
        }

        // 2. تایید رمز عبور
        var isPasswordValid = await customerService.ValidatePasswordAsync(customer.Id, request.Password, cancellationToken);
        if (!isPasswordValid)
        {
            logger.LogWarning("Login failed: Invalid password for customer {CustomerId}", customer.Id);
            throw new UnauthorizedAccessException("نام کاربری یا رمز عبور اشتباه است");
        }

        // 3. تولید JWT Token
        var tokenResult = await authenticationService.GenerateTokenAsync(
            customer.Id,
            customer.MobileNo ?? string.Empty,
            cancellationToken);

        logger.LogInformation("Customer {CustomerId} logged in successfully", customer.Id);

        var customerTenant = await customerService.GetCustomerTenantAsync(customer.Id, null, cancellationToken);

        // 4. بازگشت پاسخ
        return new LoginCommandResponse
        {
            Token = tokenResult.AccessToken,
            RefreshToken = tokenResult.RefreshToken,
            ExpiresIn = tokenResult.ExpiresIn,
            Customer = CustomerDtoFactory.Create(customer, customerTenant)
        };
    }
}

// CustomerDto will be shared across commands/queries
public record CustomerDto
{
    public string Id { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? NationalCode { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime JoinDate { get; set; }
    public bool IsActive { get; set; }
    public string? CustomerLevelName { get; set; }
    public int TotalPoints { get; set; }
    public int AvailablePoints { get; set; }
    public string? ReferrerCode { get; set; }
}

