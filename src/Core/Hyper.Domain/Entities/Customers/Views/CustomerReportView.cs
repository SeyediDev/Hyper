namespace Hyper.Domain.Entities.Customers.Views;

/// <summary>
/// ویو گزارشات مشتری - ترکیب اطلاعات مشتری با اکوسیستم
/// </summary>
[DontAudit]
[View(@"SELECT c.*, 
ct.TenantId,
t.Title AS TenantTitle
FROM Core.Customers c
LEFT JOIN Core.CustomerTenants ct ON ct.CustomerId = c.Id AND ct.IsActive = 1
LEFT JOIN CoreConfig.Tenants t ON t.Id = ct.TenantId", true)]
[DisplayName("گزارش مشتری")]
public class CustomerReportView : Customer
{
    /// <summary>
    /// شناسه اکوسیستم
    /// </summary>
    [DisplayName("شناسه اکوسیستم")]
    public int TenantId { get; set; }

    /// <summary>
    /// عنوان اکوسیستم
    /// </summary>
    [DisplayName("عنوان اکوسیستم")]
    [InDisplayString]
    [MaxLength(41)]
    public string? TenantTitle { get; set; }
}

