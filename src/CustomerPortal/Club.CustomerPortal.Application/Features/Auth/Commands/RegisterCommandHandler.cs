using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Auth.Commands;

public class RegisterCommandHandler(
    ICustomerService customerService,
    IAuthenticationService authenticationService,
    IReferralService referralService,
    ILogger<RegisterCommandHandler> logger) : IRequestHandler<RegisterCommand, LoginCommandResponse>
{
    public async Task<LoginCommandResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // 1. بررسی وجود مشتری با ایمیل یا موبایل
        var existingCustomerByEmail = await customerService.GetCustomerByEmailAsync(request.Email, cancellationToken);
        if (existingCustomerByEmail != null)
        {
            throw new InvalidOperationException("این ایمیل قبلاً ثبت شده است");
        }

        var existingCustomerByPhone = await customerService.GetCustomerByPhoneAsync(request.PhoneNumber, cancellationToken);
        if (existingCustomerByPhone != null)
        {
            throw new InvalidOperationException("این شماره موبایل قبلاً ثبت شده است");
        }

        // 2. تایید کد معرف (در صورت وجود)
        if (!string.IsNullOrEmpty(request.ReferrerCode))
        {
            var isValidReferrerCode = await referralService.ValidateReferrerCodeAsync(request.ReferrerCode, cancellationToken);
            if (!isValidReferrerCode)
            {
                throw new InvalidOperationException("کد معرف نامعتبر است");
            }
        }

        // 3. ایجاد مشتری جدید
        var customer = await customerService.CreateCustomerAsync(request, cancellationToken);

        // 4. ثبت معرف (در صورت وجود)
        if (!string.IsNullOrEmpty(request.ReferrerCode))
        {
            await referralService.SetReferrerAsync(customer.Id, request.ReferrerCode, cancellationToken);
        }

        // 5. تولید JWT Token
        var tokenResult = await authenticationService.GenerateTokenAsync(
            customer.Id,
            customer.MobileNo ?? string.Empty,
            cancellationToken);

        logger.LogInformation("New customer registered successfully: {CustomerId}", customer.Id);

        var customerTenant = await customerService.GetCustomerTenantAsync(customer.Id, null, cancellationToken);

        // 6. بازگشت پاسخ
        return new LoginCommandResponse
        {
            Token = tokenResult.AccessToken,
            RefreshToken = tokenResult.RefreshToken,
            ExpiresIn = tokenResult.ExpiresIn,
            Customer = CustomerDtoFactory.Create(customer, customerTenant)
        };
    }
}

