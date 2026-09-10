namespace Hyper.Domain.Features.Attributes;

public interface IAttributeService
{
    public Task<AttributeDto?> GetAttributeAsync(
        int tenantId, AttributeArea area, string key, int? paramId, int? paramCategoryId, CancellationToken cancellationToken);
    public Task<AttributeDto> AddAttributeAsync(
        int tenantId, AttributeArea area, string key, int? paramId, int? paramCategoryId, string? title, object? sampleValue, CancellationToken cancellationToken);
    public Task<AttributeDto> GetOrAddAttributeAsync(
        int tenantId, AttributeArea area, string key, int? paramId, int? paramCategoryId, string? title, object? sampleValue, CancellationToken cancellationToken);
    public Task<List<AttributeDto>> GetAttributesAsync(
        int tenantId, AttributeArea area, int? paramId, int? paramCategoryId, CancellationToken cancellationToken);
}

public record AttributeDto
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public AttributeArea Area { get; set; } = AttributeArea.Tenant;
    public string Key { get; set; } = null!;
    public TenantAttributeType ValueType { get; set; } = TenantAttributeType.String;
    public AttributeValueStorageType ValueStorageType { get; set; } = AttributeValueStorageType.Last;
    public AttributeCustomerUsage CustomerUsage { get; set; }
    public string? Title { get; set; }
    public string? DefaultValue { get; set; }
    public bool ValidateByType { get; set; } = true;
    public bool ValidateByList { get; set; } = false;
    public bool? IsOptional { get; set; }
    public int? SegmentId { get; set; }
    public int? ProductCategoryId { get; set; }
    public int? ProductId { get; set; }
    public int? ChannelId { get; set; }
    public int? EventTypeId { get; set; }
}

internal class AttributeService(
      IQueryRepository<TenantAttribute> attributeQuery
    , IQueryRepository<Product> productQuery
    , IQueryRepository<ProductCategory> productCategoryQuery
    , ICommandRepository<TenantAttribute> attributeCommand
    ) : IAttributeService
{
    public async Task<AttributeDto> GetOrAddAttributeAsync(
        int tenantId, AttributeArea area, string key, int? paramId, int? paramCategoryId, string? title, object? sampleValue, CancellationToken cancellationToken)
    {
        AttributeDto? tenantAttributeDto =
               await GetAttributeAsync(tenantId, area, key, paramId, paramCategoryId, cancellationToken)
            ?? await AddAttributeAsync(tenantId, area, key, paramId, paramCategoryId, title, sampleValue, cancellationToken);
        return tenantAttributeDto;
    }

    public async Task<AttributeDto?> GetAttributeAsync(
        int tenantId, AttributeArea area, string key, int? paramId, int? paramCategoryId, CancellationToken cancellationToken)
    {
        TenantAttribute? tenantAttribute = await attributeCommand.FirstOrDefaultAsync(
            x => x.TenantId == tenantId && x.Area == area && x.Key == key && x.IsActive && !x.IsDeleted &&
            (
            (area == AttributeArea.Tenant) ||
            (area == AttributeArea.Segment && (x.SegmentId == null || x.SegmentId == paramId)) ||
            (area == AttributeArea.Product &&
                (paramId == null || x.ProductId == paramId || (x.ProductId == null)) &&
                (paramCategoryId == null || x.ProductCategoryId == paramCategoryId)) ||
            (area == AttributeArea.Event && (x.EventTypeId == null || x.EventTypeId == paramId)) ||
            (area == AttributeArea.Channel && (x.ChannelId == null || x.ChannelId == paramId))
            )
            , cancellationToken);
        return tenantAttribute?.Adapt<AttributeDto>();
    }

    public async Task<List<AttributeDto>> GetAttributesAsync(
        int tenantId, AttributeArea area, int? paramId, int? paramCategoryId, CancellationToken cancellationToken)
    {
        return await GetAttributesAsync(tenantId, area, paramId, paramCategoryId, false, cancellationToken);
    }

    private async Task<List<AttributeDto>> GetAttributesAsync(
        int tenantId, AttributeArea area, int? paramId, int? paramCategoryId, bool onlyParamId, CancellationToken cancellationToken)
    {
        var attributes = await attributeQuery.GetAllAsync<AttributeDto>(
            cancellationToken, x => x.TenantId == tenantId && x.Area == area &&
            (
            (area == AttributeArea.Tenant) ||
            (area == AttributeArea.Segment && (x.SegmentId == paramId || (!onlyParamId && x.SegmentId == null))) ||
            (area == AttributeArea.Product &&
                (paramId == null || x.ProductId == paramId || (!onlyParamId && x.ProductId == null)) &&
                (paramCategoryId == null || x.ProductCategoryId == paramCategoryId)) ||
            (area == AttributeArea.Event && (x.EventTypeId == paramId || (!onlyParamId && x.EventTypeId == null))) ||
            (area == AttributeArea.Channel && (x.ChannelId == paramId || (!onlyParamId && x.ChannelId == null)))
            ));
        if (area == AttributeArea.Product)
        {
            if (paramCategoryId != null)
            {
                var productCategory = await productCategoryQuery.GetByIdAsync(paramCategoryId.Value, cancellationToken);
                if (productCategory != null && productCategory.ParentCategoryId != null)
                {
                    attributes.AddRange([.. await GetAttributesAsync(
                                    tenantId, area, null, productCategory.ParentCategoryId, true, cancellationToken)]);
                }
            }
            else if (paramId != null)
            {
                var product = await productQuery.GetByIdAsync(paramId.Value, cancellationToken);
                if (product != null && product.ProductCategoryId != null)
                {
                    attributes.AddRange([.. await GetAttributesAsync(
                                    tenantId, area, null, product.ProductCategoryId, true, cancellationToken)]);
                }
            }
        }
        return attributes;
    }

    public async Task<AttributeDto> AddAttributeAsync(
        int tenantId, AttributeArea area, string key, int? paramId, int? paramCategoryId, string? title, object? sampleValue, CancellationToken cancellationToken)
    {
        TenantAttribute tenantAttribute = new()
        {
            Area = area,
            Key = key,
            Title = title,
            TenantId = tenantId,
            CreatedBySystem = true,
            IsActive = true,
            IsOptional = true,
            ValueType = TenantAttributeType.String,
        };
        switch (area)
        {
            case AttributeArea.Segment:
                tenantAttribute.SegmentId = paramId;
                break;
            case AttributeArea.Product:
                tenantAttribute.ProductId = paramId;
                tenantAttribute.ProductCategoryId = paramCategoryId;
                break;
            case AttributeArea.Event:
                tenantAttribute.EventTypeId = paramId;
                break;
            case AttributeArea.Channel:
                tenantAttribute.ChannelId = paramId;
                break;
        }
        var type = sampleValue?.GetType();
        if (type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(byte) ||
            type == typeof(uint) || type == typeof(ulong) || type == typeof(ushort))
            tenantAttribute.ValueType = TenantAttributeType.Integer;
        else if (type == typeof(double) || type == typeof(float) || type == typeof(decimal))
            tenantAttribute.ValueType = TenantAttributeType.Decimal;
        else if (type == typeof(string))
            tenantAttribute.ValueType = TenantAttributeType.String;
        else if (type == typeof(bool))
            tenantAttribute.ValueType = TenantAttributeType.Boolean;
        else if (type == typeof(DateTime))
            tenantAttribute.ValueType = TenantAttributeType.DateTime;
        else if (type == typeof(DateOnly))
            tenantAttribute.ValueType = TenantAttributeType.DateOnly;
        else if (type == typeof(TimeSpan))
            tenantAttribute.ValueType = TenantAttributeType.TimeOnly;
        attributeCommand.Add(tenantAttribute);
        await attributeCommand.UnitOfWork.SaveChangesAsync(cancellationToken);
        return tenantAttribute.Adapt<AttributeDto>();
    }
}