using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Services;

/// <summary>
/// Mock implementation برای تست - باید با implementation واقعی جایگزین شود
/// </summary>
public class MockCustomerService(ILogger<MockCustomerService> logger) : ICustomerService
{
    private static readonly Dictionary<int, Customer> _customers = [];
    private static readonly Dictionary<(int customerId, int tenantId), CustomerTenant> _customerTenants = [];
    private const int DefaultTenantId = 1;
    private static int _nextId = 1;

    public Task<Customer> CreateCustomerAsync(RegisterCommand command, CancellationToken cancellationToken = default)
    {
        var customer = new Customer
        {
            Id = _nextId++,
            FirstName = command.FirstName,
            LastName = command.LastName,
            MobileNo = command.PhoneNumber,
            NationalCode = string.IsNullOrEmpty(command.NationalCode) ? null : long.Parse(command.NationalCode),
            BirthDate = command.BirthDate
        };

        _customers[customer.Id] = customer;

        _customerTenants[(customer.Id, DefaultTenantId)] = new CustomerTenant
        {
            CustomerId = customer.Id,
            TenantId = DefaultTenantId,
            JoinDate = DateTime.UtcNow,
            IsActive = true,
            CurrentPointsBalance = 0,
            TotalPointsEarned = 0
        };
        logger.LogInformation("Mock: Created customer {CustomerId}", customer.Id);

        return Task.FromResult(customer);
    }

    public Task<Customer?> GetCustomerByIdAsync(int customerId, CancellationToken cancellationToken = default)
    {
        _customers.TryGetValue(customerId, out var customer);
        return Task.FromResult(customer);
    }

    public Task<Customer?> GetCustomerByPhoneAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        var customer = _customers.Values.FirstOrDefault(c => c.MobileNo == phoneNumber);
        return Task.FromResult(customer);
    }

    public Task<Customer?> GetCustomerByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        // Email field doesn't exist in Customer entity from main project
        // Return null for now
        return Task.FromResult<Customer?>(null);
    }

    public Task<Customer> UpdateCustomerAsync(int customerId, UpdateProfileCommand command, CancellationToken cancellationToken = default)
    {
        if (!_customers.TryGetValue(customerId, out var customer))
        {
            throw new InvalidOperationException("Customer not found");
        }

        if (!string.IsNullOrEmpty(command.FirstName)) customer.FirstName = command.FirstName;
        if (!string.IsNullOrEmpty(command.LastName)) customer.LastName = command.LastName;
        if (!string.IsNullOrEmpty(command.PhoneNumber)) customer.MobileNo = command.PhoneNumber;
        if (!string.IsNullOrEmpty(command.NationalCode)) customer.NationalCode = long.Parse(command.NationalCode);
        if (command.BirthDate.HasValue) customer.BirthDate = command.BirthDate;

        logger.LogInformation("Mock: Updated customer {CustomerId}", customerId);
        return Task.FromResult(customer);
    }

    public Task ChangePasswordAsync(int customerId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Mock: Password changed for customer {CustomerId}", customerId);
        return Task.CompletedTask;
    }

    public Task<bool> ValidatePasswordAsync(int customerId, string password, CancellationToken cancellationToken = default)
    {
        // Mock: همیشه true برمی‌گرداند
        return Task.FromResult(true);
    }

    public Task<string> GeneratePasswordResetTokenAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        var token = Guid.NewGuid().ToString("N").Substring(0, 6);
        logger.LogInformation("Mock: Generated password reset token {Token} for {Phone}", token, phoneNumber);
        return Task.FromResult(token);
    }

    public Task ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Mock: Password reset with token {Token}", token);
        return Task.CompletedTask;
    }

    public Task<CustomerTenant?> GetCustomerTenantAsync(int customerId, int? tenantId = null, CancellationToken cancellationToken = default)
    {
        if (tenantId.HasValue)
        {
            _customerTenants.TryGetValue((customerId, tenantId.Value), out var customerTenant);
            return Task.FromResult(customerTenant);
        }

        var tenantEntry = _customerTenants
            .Where(kvp => kvp.Key.customerId == customerId)
            .Select(kvp => kvp.Value)
            .FirstOrDefault();

        return Task.FromResult(tenantEntry);
    }
}

