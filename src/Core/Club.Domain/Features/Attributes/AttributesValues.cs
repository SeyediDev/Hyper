namespace Hyper.Domain.Features.Attributes;

public class AttributesValues : Dictionary<string, object>
{
    public void AddRange(AttributesValues attributesValues)
    {
        foreach (var attributeValue in attributesValues)
        {
            TryAdd(attributeValue.Key, attributeValue.Value);
        }
    }
}