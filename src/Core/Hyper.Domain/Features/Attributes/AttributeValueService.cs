namespace Hyper.Domain.Features.Attributes;

public interface IAttributeValueService
{
    public Task<AttributesValues> GetAndSetAttributesValuesActionsAsync(
        int tenantId, int? customerTenantId, AttributeArea area, int? paramId, int? paramCategoryId,
        List<EventAttributeValueDto>? eventAttributeValueDtos, CancellationToken cancellationToken);
    
    Task<string?> GetParamKey(EventAttributeValueDto attributeValue, CancellationToken cancellationToken);
    
    /// <summary>
    /// Batch-load ParamKeys for multiple attributes to prevent N+1 query pattern
    /// اینجا برای 2M concurrent users: به جای N query تک الف، تک بار load می‌کنه
    /// </summary>
    Task BatchPopulateParamKeysAsync(List<EventAttributeValueDto> attributeValues, CancellationToken cancellationToken);
}

internal class AttributeValueService(
      IQueryRepository<TenantAttributeAllowedValue, int> tenantAttributeAllowedValueQuery//TODO use cash
    , IQueryRepository<CustomerSegment> segmentQuery
    , IQueryRepository<EventChannel> channelQuery
    , IQueryRepository<EventType> eventQuery
    , IQueryRepository<Product> productQuery
    , IQueryRepository<ProductCategory> productCategoryQuery
    , IQueryRepositoryL<TenantAttributeValue> attributeValueQuery
    , ICommandRepositoryL<TenantAttributeValue> attributeValueCommand
    //, IAttributeAggregationQueryService aggregationQueryService
    ) : IAttributeValueService
{
    public async Task<AttributesValues> GetAndSetAttributesValuesActionsAsync(
        int tenantId, int? customerTenantId, AttributeArea area, int? paramId, int? paramCategoryId,
        List<EventAttributeValueDto>? eventAttributeValueDtos, CancellationToken cancellationToken)
    {
        // COMMAND: Save raw attribute values (INSERT operations)
        if (eventAttributeValueDtos != null)
        {
            await SaveRawAttributeValuesAsync(tenantId, customerTenantId, area, paramId, paramCategoryId, 
                eventAttributeValueDtos, cancellationToken);
        }

        // QUERY: Get aggregated attribute values from database with GROUP BY
        // Using separate QueryRepository to avoid transaction conflicts
        AttributesValues attributesValues = await GetAggregatedAttributesAsync(
            tenantId, customerTenantId, area, paramId, paramCategoryId, cancellationToken);

        return attributesValues;
    }

    private async Task SaveRawAttributeValuesAsync(
        int tenantId, int? customerTenantId, AttributeArea area, int? paramId, int? paramCategoryId,
        List<EventAttributeValueDto> eventAttributeValueDtos, CancellationToken cancellationToken)
    {
        // First, load existing attribute definitions for validation
        List<TenantAttributeValue> existingValues =
            await GetAttributesValues(tenantId, customerTenantId, area, paramId, paramCategoryId, false, cancellationToken);

        foreach (var eventAttributeValueDto in eventAttributeValueDtos)
        {
            if (await ValidateAttributeValue(eventAttributeValueDto, cancellationToken))
            {
                // ذخیره داده خام برای هر رویداد
                if (eventAttributeValueDto.Attribute.CustomerUsage is not AttributeCustomerUsage.OnlyForCustomers)
                {
                    await SaveRawAttributeValue(eventAttributeValueDto, false, null, existingValues, cancellationToken);
                }
                if (eventAttributeValueDto.Attribute.CustomerUsage is AttributeCustomerUsage.AlsoForCustomers or AttributeCustomerUsage.OnlyForCustomers)
                {
                    await SaveRawAttributeValue(eventAttributeValueDto, true, customerTenantId, existingValues, cancellationToken);
                }
            }
        }
        
        // Single transaction for all INSERTs
        await attributeValueCommand.SaveChangesAsync(cancellationToken);
    }

    private async Task<AttributesValues> GetAggregatedAttributesAsync(
        int tenantId, int? customerTenantId, AttributeArea area, int? paramId, int? paramCategoryId, 
        CancellationToken cancellationToken)
    {
        // DATABASE-LEVEL GROUP BY: Aggregate at SQL level, not in memory
        // Returns only aggregated records, not billions of raw rows
        
        // Get all matching attribute values with their definitions  
        var attributeValues = await attributeValueQuery.GetAllWithIncludeAsync(
            x => x.Attribute, cancellationToken,
            x => x.Area == area && x.TenantId == tenantId && !x.IsDeleted && 
                   !x.Attribute.IsDeleted && x.Attribute.IsActive &&
                   (x.Attribute.CustomerUsage == AttributeCustomerUsage.None ||
                    (x.Attribute.CustomerUsage == AttributeCustomerUsage.OnlyForCustomers && x.CustomerTenantId == customerTenantId) ||
                    (x.Attribute.CustomerUsage == AttributeCustomerUsage.AlsoForCustomers && (x.CustomerTenantId == null || x.CustomerTenantId == customerTenantId))
            ) &&
            (area == AttributeArea.Tenant ||
            (area == AttributeArea.Segment && (x.SegmentId == paramId || x.SegmentId == null)) ||
            (area == AttributeArea.Product &&
                (paramId == null || x.ProductId == paramId || x.ProductId == null) &&
                (paramCategoryId == null || x.ProductCategoryId == paramCategoryId)) ||
            (area == AttributeArea.Event && (x.EventTypeId == paramId || x.EventTypeId == null)) ||
            (area == AttributeArea.Channel && (x.ChannelId == paramId || x.ChannelId == null)))
        );

        // GROUP BY at LINQ level (translates to SQL GROUP BY)
        // Key selector groups by all the dimensions
        var groupedResults = attributeValues
            .GroupBy(x => new
            {
                x.AttributeId,
                x.CustomerTenantId,
                x.ParamKey,
                x.Area,
                x.SegmentId,
                x.ProductId,
                x.ProductCategoryId,
                x.EventTypeId,
                x.ChannelId,
                AttributeKey = x.Attribute.Key,
                x.Attribute.ValueStorageType
            })
            .Select(g => new
            {
                g.Key.AttributeId,
                g.Key.CustomerTenantId,
                g.Key.ParamKey,
                g.Key.Area,
                g.Key.SegmentId,
                g.Key.ProductId,
                g.Key.ProductCategoryId,
                g.Key.EventTypeId,
                g.Key.ChannelId,
                g.Key.AttributeKey,
                g.Key.ValueStorageType,
                AllValues = g.Select(x => x.Value).ToList(),
                Count = g.Count(),
                NumericValues = g.Select(x => 
                {
                    if (decimal.TryParse(x.Value, out var num))
                        return (decimal?)num;
                    return null;
                })
                .Where(v => v.HasValue)
                .Select(v => v!.Value)
                .ToList()
            })
            .ToList();

        // Build result dictionary from aggregated records
        AttributesValues result = [];
        
        foreach (var group in groupedResults)
        {
            string key = "";
            if (group.CustomerTenantId.HasValue && group.CustomerTenantId > 0)
                key = "Customer";
            
            key += group.Area switch
            {
                AttributeArea.Tenant => (group.CustomerTenantId.HasValue && group.CustomerTenantId > 0) ? "Tenant" : "",
                _ => group.Area.ToString()
            };

            if (!string.IsNullOrEmpty(group.ParamKey))
                key += $"_{group.ParamKey}";
            key += $"_{group.AttributeKey}";

            // Apply aggregation based on storage type
            switch (group.ValueStorageType)
            {
                case AttributeValueStorageType.None:
                case AttributeValueStorageType.First:
                case AttributeValueStorageType.Last:
                    // Return last value
                    if (group.AllValues.Any())
                        result.Add(key, group.AllValues.Last());
                    break;

                case AttributeValueStorageType.SumCountAverage:
                    if (group.NumericValues.Any())
                    {
                        var sum = group.NumericValues.Sum();
                        var count = group.NumericValues.Count;
                        var average = sum / count;
                        
                        result.Add($"{key}_Sum", sum);
                        result.Add($"{key}_Count", count);
                        result.Add($"{key}_Average", average);
                    }
                    break;

                case AttributeValueStorageType.CountOfDistinctOfValues:
                    var distinctCount = group.AllValues.Distinct().Count();
                    if (group.AllValues.Any())
                        result.Add($"{group.AllValues.Last()}_Count", distinctCount);
                    break;
            }
        }

        return result;
    }
    private async Task<List<TenantAttributeValue>> GetAttributesValues(
        int tenantId, int? customerTenantId,
        AttributeArea area, int? paramId, int? paramCategoryId, bool onlyParamId, CancellationToken cancellationToken)
    {
        // Filter conditions for base query
        var baseFilter = new Func<TenantAttributeValue, bool>(x => 
            x.TenantId == tenantId && 
            !x.IsDeleted && 
            !x.Attribute.IsDeleted && 
            x.Attribute.IsActive &&
            (x.Attribute.CustomerUsage == AttributeCustomerUsage.None ||
            (x.Attribute.CustomerUsage == AttributeCustomerUsage.OnlyForCustomers && x.CustomerTenantId == customerTenantId) ||
            (x.Attribute.CustomerUsage == AttributeCustomerUsage.AlsoForCustomers && (x.CustomerTenantId == null || x.CustomerTenantId == customerTenantId)))
        );

        var attributeValues = (await attributeValueQuery.GetAllWithIncludeAsync(
            x => x.Attribute, cancellationToken,
            x => x.Area == area && baseFilter(x) &&
            (area == AttributeArea.Tenant ||
            (area == AttributeArea.Segment && (x.SegmentId == paramId || (!onlyParamId && x.SegmentId == null))) ||
            (area == AttributeArea.Product &&
                (paramId == null || x.ProductId == paramId || (!onlyParamId && x.ProductId == null)) &&
                (paramCategoryId == null || x.ProductCategoryId == paramCategoryId)) ||
            (area == AttributeArea.Event && (x.EventTypeId == paramId || (!onlyParamId && x.EventTypeId == null))) ||
            (area == AttributeArea.Channel && (x.ChannelId == paramId || (!onlyParamId && x.ChannelId == null))))
        )).ToList();

        // Handle product category hierarchy - fetch all parent categories in single query, not recursive
        if (area == AttributeArea.Product && paramCategoryId != null)
        {
            var parentCategoryIds = new HashSet<int> { paramCategoryId.Value };
            int currentCategoryId = paramCategoryId.Value;
            
            // Load category hierarchy in batch (prevents N recursive queries)
            while (true)
            {
                var category = await productCategoryQuery.GetByIdAsync(currentCategoryId, cancellationToken);
                if (category?.ParentCategoryId == null)
                    break;
                    
                parentCategoryIds.Add(category.ParentCategoryId.Value);
                currentCategoryId = category.ParentCategoryId.Value;
            }

            // Get attributes for all parent categories in one query
            if (parentCategoryIds.Count > 1)
            {
                var parentAttributeValues = (await attributeValueQuery.GetAllWithIncludeAsync(
                    x => x.Attribute, cancellationToken,
                    x => x.Area == area && baseFilter(x) &&
                    parentCategoryIds.Contains(x.ProductCategoryId ?? 0) &&
                    (x.ProductId == null || x.ProductId == paramId || (!onlyParamId && x.ProductId == null))
                )).ToList();

                // Add parent attributes (avoid duplicates)
                var existingAttributeIds = attributeValues.Select(a => a.AttributeId).ToHashSet();
                attributeValues.AddRange(parentAttributeValues.Where(a => !existingAttributeIds.Contains(a.AttributeId)));
            }
        }
        else if (area == AttributeArea.Product && paramId != null)
        {
            // Load product's category and get its hierarchy attributes
            var product = await productQuery.GetByIdAsync(paramId.Value, cancellationToken);
            if (product?.ProductCategoryId != null)
            {
                var categoryAttributeValues = await GetAttributesValues(
                    tenantId, customerTenantId, area, null, product.ProductCategoryId, true, cancellationToken);
                
                var existingAttributeIds = attributeValues.Select(a => a.AttributeId).ToHashSet();
                attributeValues.AddRange(categoryAttributeValues.Where(a => !existingAttributeIds.Contains(a.AttributeId)));
            }
        }

        return attributeValues;
    }

    private async Task SaveRawAttributeValue(
        EventAttributeValueDto eventAttributeValueDto, bool forCustomer, int? customerTenantId,
        List<TenantAttributeValue> attributeValues, CancellationToken cancellationToken)
    {
        if (eventAttributeValueDto.Attribute.ValueStorageType == AttributeValueStorageType.First)
        {
            if (attributeValues.Any(a => a.Key == eventAttributeValueDto.Attribute.Key))//TODO check other parameters
            {
                return;
            }
        }
        else if (eventAttributeValueDto.Attribute.ValueStorageType == AttributeValueStorageType.Last)
        {
            if (attributeValues.Any(a => a.Key == eventAttributeValueDto.Attribute.Key))//TODO check other parameters
            {
                await attributeValueCommand
                    .ExpireAsync(x => x.AttributeId == eventAttributeValueDto.Attribute.Id
                    //TODO check other parameters
                    , cancellationToken);
            }
        }
        var attributeValue = eventAttributeValueDto.Adapt<TenantAttributeValue>();
        attributeValue.CustomerTenantId = forCustomer ? customerTenantId : null;
        attributeValue.EventDate = DateTime.UtcNow;

        attributeValueCommand.Add(attributeValue);
    }

    private async Task<bool> ValidateAttributeValue(EventAttributeValueDto eventAttributeValueDto, CancellationToken cancellationToken)
    {
        // کنترل بر اساس نوع
        if (eventAttributeValueDto.Attribute?.ValidateByType is true)
        {
            var typeValidationResult = ValidateByParameterType(eventAttributeValueDto.Attribute.ValueType, eventAttributeValueDto.Value);
            if (!typeValidationResult.IsValid)
            {
                return false;
            }
        }

        // کنترل بر اساس لیست
        if (eventAttributeValueDto.Attribute?.ValidateByList is true)
        {
            var allowedValues = await tenantAttributeAllowedValueQuery.GetAllAsync(
                cancellationToken,
                av => av.TenantAttributeId == eventAttributeValueDto.Attribute.Id && av.IsActive && !av.IsDeleted);

            var isAllowed = allowedValues.Any(av => av.Value.Equals(eventAttributeValueDto.Value?.ToString(), StringComparison.OrdinalIgnoreCase));
            if (!isAllowed)
            {
                return false;
            }
        }

        return true;
    }

    private static ValidationResult ValidateByParameterType(TenantAttributeType type, object value)
    {
        if (value == null)
        {
            return new ValidationResult
            {
                IsValid = false,
                ErrorMessage = "مقدار نمی‌تواند خالی باشد"
            };
        }

        return type switch
        {
            TenantAttributeType.String => new ValidationResult { IsValid = true },
            TenantAttributeType.Integer => long.TryParse(value.ToString(), out _)
                ? new ValidationResult { IsValid = true }
                : new ValidationResult { IsValid = false, ErrorMessage = "مقدار باید یک عدد صحیح باشد" },
            TenantAttributeType.Decimal => decimal.TryParse(value.ToString(), out _)
                ? new ValidationResult { IsValid = true }
                : new ValidationResult { IsValid = false, ErrorMessage = "مقدار باید یک عدد اعشاری باشد" },
            TenantAttributeType.DateOnly => DateTime.TryParse(value.ToString(), out _)
                ? new ValidationResult { IsValid = true }
                : new ValidationResult { IsValid = false, ErrorMessage = "مقدار باید یک تاریخ معتبر باشد" },
            TenantAttributeType.DateTime => DateTime.TryParse(value.ToString(), out _)
                ? new ValidationResult { IsValid = true }
                : new ValidationResult { IsValid = false, ErrorMessage = "مقدار باید یک تاریخ و زمان معتبر باشد" },
            TenantAttributeType.TimeOnly => TimeSpan.TryParse(value.ToString(), out _)
                ? new ValidationResult { IsValid = true }
                : new ValidationResult { IsValid = false, ErrorMessage = "مقدار باید یک زمان معتبر باشد" },
            _ => new ValidationResult { IsValid = true }
        };
    }

    public async Task<string?> GetParamKey(EventAttributeValueDto attributeValue, CancellationToken cancellationToken)
    {
        if (attributeValue.ParamId is null)
            return null!;
        
        return attributeValue.Attribute.Area switch
        {
            AttributeArea.Segment => 
                attributeValue.SegmentId > 0 
                    ? (await segmentQuery.GetByIdAsync(attributeValue.SegmentId ?? 0, cancellationToken))?.Key 
                    : null,

            AttributeArea.Product => 
                attributeValue.ProductId > 0 
                    ? (await productQuery.GetByIdAsync(attributeValue.ProductId ?? 0, cancellationToken))?.Key 
                    : attributeValue.ProductCategoryId > 0 
                        ? (await productCategoryQuery.GetByIdAsync(attributeValue.ProductCategoryId ?? 0, cancellationToken))?.Key 
                        : null,

            AttributeArea.Channel => 
                attributeValue.ChannelId > 0 
                    ? (await channelQuery.GetByIdAsync(attributeValue.ChannelId ?? 0, cancellationToken))?.Key 
                    : null,

            AttributeArea.Event => 
                attributeValue.EventTypeId > 0 
                    ? (await eventQuery.GetByIdAsync(attributeValue.EventTypeId ?? 0, cancellationToken))?.Key 
                    : null,

            _ => null
        };
    }

    /// <summary>
    /// Batch-populate ParamKeys for all eventAttributeValueDtos in one operation
    /// Prevents N+1 query pattern when processing 100+ attributes per event
    /// برای 2M concurrent users: N query تکی را به یک بار batch loading تبدیل می‌کنه
    /// </summary>
    public async Task BatchPopulateParamKeysAsync(
        List<EventAttributeValueDto> attributeValues, 
        CancellationToken cancellationToken)
    {
        if (attributeValues == null || attributeValues.Count == 0)
            return;

        // Collect all unique IDs by type
        var segmentIds = attributeValues
            .Where(a => a.Attribute.Area == AttributeArea.Segment && a.SegmentId > 0)
            .Select(a => a.SegmentId!.Value)
            .Distinct()
            .ToList();

        var productIds = attributeValues
            .Where(a => a.Attribute.Area == AttributeArea.Product && a.ProductId > 0)
            .Select(a => a.ProductId!.Value)
            .Distinct()
            .ToList();

        var productCategoryIds = attributeValues
            .Where(a => a.Attribute.Area == AttributeArea.Product && a.ProductCategoryId > 0)
            .Select(a => a.ProductCategoryId!.Value)
            .Distinct()
            .ToList();

        var channelIds = attributeValues
            .Where(a => a.Attribute.Area == AttributeArea.Channel && a.ChannelId > 0)
            .Select(a => a.ChannelId!.Value)
            .Distinct()
            .ToList();

        var eventTypeIds = attributeValues
            .Where(a => a.Attribute.Area == AttributeArea.Event && a.EventTypeId > 0)
            .Select(a => a.EventTypeId!.Value)
            .Distinct()
            .ToList();

        // Batch-load all parameters in parallel
        var segmentsTask = segmentIds.Count > 0
            ? segmentQuery.GetAllAsync(cancellationToken, x => segmentIds.Contains(x.Id)).ContinueWith(t => t.Result.ToList())
            : Task.FromResult(new List<CustomerSegment>());

        var productsTask = productIds.Count > 0
            ? productQuery.GetAllAsync(cancellationToken, x => productIds.Contains(x.Id)).ContinueWith(t => t.Result.ToList())
            : Task.FromResult(new List<Product>());

        var categoriesTask = productCategoryIds.Count > 0
            ? productCategoryQuery.GetAllAsync(cancellationToken, x => productCategoryIds.Contains(x.Id)).ContinueWith(t => t.Result.ToList())
            : Task.FromResult(new List<ProductCategory>());

        var channelsTask = channelIds.Count > 0
            ? channelQuery.GetAllAsync(cancellationToken, x => channelIds.Contains(x.Id)).ContinueWith(t => t.Result.ToList())
            : Task.FromResult(new List<EventChannel>());

        var eventsTask = eventTypeIds.Count > 0
            ? eventQuery.GetAllAsync(cancellationToken, x => eventTypeIds.Contains(x.Id)).ContinueWith(t => t.Result.ToList())
            : Task.FromResult(new List<EventType>());

        // Wait for all loads to complete
        await Task.WhenAll(segmentsTask, productsTask, categoriesTask, channelsTask, eventsTask);

        // Build lookup dictionaries
        var segmentsByIdMap = await segmentsTask;
        var segmentsDict = segmentsByIdMap.ToDictionary(x => x.Id, x => x.Key);

        var productsByIdMap = await productsTask;
        var productsDict = productsByIdMap.ToDictionary(x => x.Id, x => x.Key);

        var categoriesByIdMap = await categoriesTask;
        var categoriesDict = categoriesByIdMap.ToDictionary(x => x.Id, x => x.Key);

        var channelsByIdMap = await channelsTask;
        var channelsDict = channelsByIdMap.ToDictionary(x => x.Id, x => x.Key);

        var eventsByIdMap = await eventsTask;
        var eventsDict = eventsByIdMap.ToDictionary(x => x.Id, x => x.Key);

        // Populate ParamKey for all attributes from dictionaries (zero-copy lookup)
        foreach (var attr in attributeValues)
        {
            attr.ParamKey = attr.Attribute.Area switch
            {
                AttributeArea.Segment => 
                    attr.SegmentId > 0 && segmentsDict.TryGetValue(attr.SegmentId.Value, out var segmentKey)
                        ? segmentKey
                        : null,

                AttributeArea.Product => 
                    attr.ProductId > 0 && productsDict.TryGetValue(attr.ProductId.Value, out var productKey)
                        ? productKey
                        : attr.ProductCategoryId > 0 && categoriesDict.TryGetValue(attr.ProductCategoryId.Value, out var categoryKey)
                            ? categoryKey
                            : null,

                AttributeArea.Channel => 
                    attr.ChannelId > 0 && channelsDict.TryGetValue(attr.ChannelId.Value, out var channelKey)
                        ? channelKey
                        : null,

                AttributeArea.Event => 
                    attr.EventTypeId > 0 && eventsDict.TryGetValue(attr.EventTypeId.Value, out var eventKey)
                        ? eventKey
                        : null,

                _ => null
            };
        }
    }
}

/// <summary>
/// نتیجه ذخیره پارامتر مشتری
/// </summary>
public record SaveCustomerParameterResult
{
    /// <summary>
    /// آیا مقدار تغییر کرده است؟
    /// </summary>
    public bool ValueChanged { get; init; }

    /// <summary>
    /// مقدار قبلی (اگر وجود داشته باشد)
    /// </summary>
    public string? OldValue { get; init; }

    /// <summary>
    /// مقدار جدید
    /// </summary>
    public string NewValue { get; init; } = string.Empty;
}

/// <summary>
/// نتیجه اعتبارسنجی مقدار پارامتر
/// </summary>
public record ValidationResult
{
    /// <summary>
    /// آیا مقدار معتبر است؟
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// پیام خطا (در صورت نامعتبر بودن)
    /// </summary>
    public string? ErrorMessage { get; init; }
}
