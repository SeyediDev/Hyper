namespace Hyper.Domain.Entities.Tenants.Enums;

public enum AttributeArea
{
    [Description("اکوسیستم")]
    Tenant = 1,
    [Description("جامعه")]
    Segment = 2,
    [Description("محصول")]
    Product = 4,
    [Description("کانال")]
    Channel = 5,
    [Description("رویداد")]
    Event = 6
}