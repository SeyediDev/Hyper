namespace Hyper.Domain.Features.Customers;

public static class ReservedAttributeExtension
{
    public static object? GetReservedAttribute(this AttributesValues? attributes, CustomerReservedAttribute reservedAttribute)
    {
        if (attributes != null && attributes.TryGetValue(reservedAttribute.ToString(), out object? value))
        {
            return value;
        }
        return null;
    }

    public static object? GetReservedAttribute(this AttributesValues? attributes, CustomerTenantReservedAttribute reservedAttribute)
    {
        if (attributes != null && attributes.TryGetValue(reservedAttribute.ToString(), out object? value))
        {
            return value;
        }
        return null;
    }

    public static object? GetReservedAttribute(this AttributesValues? attributes, ProductReservedAttribute reservedAttribute)
    {
        if (attributes != null && attributes.TryGetValue(reservedAttribute.ToString(), out object? value))
        {
            return value;
        }
        return null;
    }

    public static void Add(this AttributesValues? attributes, CustomerReservedAttribute reservedAttribute, object value)
    {
        if (attributes != null)
        {
            if (!attributes.TryAdd(reservedAttribute.ToString(), value))
            {
                attributes[reservedAttribute.ToString()] = value;
            }
        }
    }

    public static void Add(this AttributesValues? attributes, CustomerTenantReservedAttribute reservedAttribute, object value)
    {
        if (attributes != null)
        {
            if (!attributes.TryAdd(reservedAttribute.ToString(), value))
            {
                attributes[reservedAttribute.ToString()] = value;
            }
        }
    }

    public static void Add(this AttributesValues? attributes, ProductReservedAttribute reservedAttribute, object value)
    {
        if (attributes != null)
        {
            if (!attributes.TryAdd(reservedAttribute.ToString(), value))
            {
                attributes[reservedAttribute.ToString()] = value;
            }
        }
    }
}