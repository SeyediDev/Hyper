namespace Hyper.CustomerPortal.Application.Features.Auth;

internal static class CustomerDtoFactory
{
    public static CustomerDto Create(Customer customer, CustomerTenant? customerTenant)
    {
        return new CustomerDto
        {
            Id = customer.Id.ToString(),
            FirstName = customer.FirstName ?? string.Empty,
            LastName = customer.LastName ?? string.Empty,
            Email = string.Empty,
            PhoneNumber = customer.MobileNo ?? string.Empty,
            NationalCode = customer.NationalCode?.ToString(),
            BirthDate = customer.BirthDate,
            AvatarUrl = null,
            JoinDate = customerTenant?.JoinDate ?? customer.CreateDate,
            IsActive = customerTenant?.IsActive ?? true,
            CustomerLevelName = customerTenant?.RfmSegment?.ToString(),
            TotalPoints = (int)(customerTenant?.TotalPointsEarned ?? 0),
            AvailablePoints = (int)(customerTenant?.CurrentPointsBalance ?? 0),
            ReferrerCode = null
        };
    }
}
