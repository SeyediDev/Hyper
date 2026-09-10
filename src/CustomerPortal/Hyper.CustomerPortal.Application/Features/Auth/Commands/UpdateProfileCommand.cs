using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Auth.Commands;

public record UpdateProfileCommand : IRequest<UpdateProfileCommandResponse>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? NationalCode { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? AvatarUrl { get; set; }
}

public record UpdateProfileCommandResponse
{
    public required CustomerDto Customer { get; set; }
}

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        When(x => !string.IsNullOrEmpty(x.Email), () =>
        {
            RuleFor(x => x.Email).EmailAddress().WithMessage("ایمیل نامعتبر است");
        });
        When(x => !string.IsNullOrEmpty(x.PhoneNumber), () =>
        {
            RuleFor(x => x.PhoneNumber).Matches(@"^09\d{9}$").WithMessage("شماره موبایل نامعتبر است");
        });
    }
}

public class UpdateProfileCommandHandler(
    ICustomerService customerService,
    ICustomerRequesterUser requesterUser,
    ILogger<UpdateProfileCommandHandler> logger) : IRequestHandler<UpdateProfileCommand, UpdateProfileCommandResponse>
{
    public async Task<UpdateProfileCommandResponse> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;
        
        var updatedCustomer = await customerService.UpdateCustomerAsync(customerId, request, cancellationToken);
        var tenantId = requesterUser.TenantId;
        var customerTenant = await customerService.GetCustomerTenantAsync(customerId, tenantId, cancellationToken);
        
        logger.LogInformation("Profile updated successfully for customer {CustomerId}", customerId);
        
        return new UpdateProfileCommandResponse
        {
            Customer = CustomerDtoFactory.Create(updatedCustomer, customerTenant)
        };
    }
}

