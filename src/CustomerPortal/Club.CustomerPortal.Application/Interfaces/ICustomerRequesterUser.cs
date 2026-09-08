namespace Hyper.CustomerPortal.Application.Interfaces;

/// <summary>
/// اطلاعات کاربر درخواست‌کننده در پرتال مشتریان
/// </summary>
public interface ICustomerRequesterUser : IRequesterUser
{
    int CustomerId { get; }
    int? TenantId { get; }
}