namespace Hyper.Domain.Entities.Points.Enums;

public enum PointBudgetScope
{
    [Description("کل مشتریان اکوسیستم")]
    PerTenant = 1,
    [Description("هر مشتری")]
    PerCustomer = 2,
    [Description("مشتریان یک جامعه")]
    PerCustomerSegment = 3,
}
