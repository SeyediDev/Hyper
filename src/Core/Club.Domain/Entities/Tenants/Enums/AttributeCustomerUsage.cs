namespace Hyper.Domain.Entities.Tenants.Enums;

public enum AttributeCustomerUsage
{
    [Description("کاربرد ندارد")]
    None = 0,
    [Description("برای مشتری نیز، در این ناحیه کاربرد دارد")]
    AlsoForCustomers = 1,
    [Description("فقط برای مشتریان در این ناحیه کاربرد دارد")]
    OnlyForCustomers = 2,
}
