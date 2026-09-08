namespace Hyper.Domain.Features.Attributes;

public record EventAttributeValueDto(AttributeDto Attribute)
{
    public object Value { get; set; } = null!;
    public int? SegmentId { get; set; }
    public int? ProductCategoryId { get; set; }
    public int? ProductId { get; set; }
    public int? EventTypeId { get; set; }
    public int? ChannelId { get; set; }
    public string? ParamKey { get; set; }
    public int? ParamId => Attribute.Area switch
    {
        AttributeArea.Segment => SegmentId,
        AttributeArea.Product => ProductId,
        AttributeArea.Event => EventTypeId,
        AttributeArea.Channel => ChannelId,
        _ => null,
    };
    public int? ParamCategoryId => Attribute.Area switch
    {
        AttributeArea.Product => ProductCategoryId,
        _ => null,
    };
    public bool Compare(TenantAttributeValue x, bool forCustomer, int? customerTenantId)
    {
        if (Attribute.Id != x.AttributeId)
            return false;
        if (Attribute.ValueStorageType == AttributeValueStorageType.CountOfDistinctOfValues)
        {
            if (x.Value != Value?.ToString())
            {
                return false;
            }
        }
        switch (x.Attribute.CustomerUsage)
        {
            case AttributeCustomerUsage.None:
                if (forCustomer)
                {
                    return false;
                }
                break;
            case AttributeCustomerUsage.AlsoForCustomers:
                if (forCustomer)
                {
                    if (customerTenantId != x.CustomerTenantId)
                    {
                        return false;
                    }
                }
                else
                {
                    if (x.CustomerTenantId > 0)
                    {
                        return false;
                    }
                }
                break;
            case AttributeCustomerUsage.OnlyForCustomers:
                if (forCustomer)
                {
                    if (customerTenantId != x.CustomerTenantId)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
                break;
        }
        return true;
    }
}
