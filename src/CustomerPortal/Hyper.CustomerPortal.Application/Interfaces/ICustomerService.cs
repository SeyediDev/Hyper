namespace Hyper.CustomerPortal.Application.Interfaces;

/// <summary>
/// سرویس مدیریت مشتریان
/// </summary>
public interface ICustomerService
{
    /// <summary>
    /// ایجاد مشتری جدید
    /// </summary>
    Task<Customer> CreateCustomerAsync(RegisterCommand command, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// دریافت مشتری با شناسه
    /// </summary>
    Task<Customer?> GetCustomerByIdAsync(int customerId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// دریافت مشتری با شماره موبایل
    /// </summary>
    Task<Customer?> GetCustomerByPhoneAsync(string phoneNumber, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// دریافت مشتری با ایمیل
    /// </summary>
    Task<Customer?> GetCustomerByEmailAsync(string email, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// به‌روزرسانی پروفایل مشتری
    /// </summary>
    Task<Customer> UpdateCustomerAsync(int customerId, UpdateProfileCommand command, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// تغییر رمز عبور
    /// </summary>
    Task ChangePasswordAsync(int customerId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// تایید رمز عبور
    /// </summary>
    Task<bool> ValidatePasswordAsync(int customerId, string password, CancellationToken cancellationToken = default);

	/// <summary>
	/// دریافت ارتباط مشتری با اکوسیستم (CustomerTenant) برای دسترسی به شاخص‌های تحلیلی
	/// </summary>
	Task<CustomerTenant?> GetCustomerTenantAsync(int customerId, int? tenantId = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// ایجاد توکن بازیابی رمز عبور
    /// </summary>
    Task<string> GeneratePasswordResetTokenAsync(string phoneNumber, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// بازیابی رمز عبور با token
    /// </summary>
    Task ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken = default);
}