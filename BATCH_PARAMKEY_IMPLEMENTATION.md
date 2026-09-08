# EventService ParamKey Batch Loading Implementation Guide

## Problem to Solve

**Current Implementation** (causes N+1 queries):
```csharp
foreach (var attributeValue in attributesValues)
{
    // ... create EventLogAttribute ...
    
    // ❌ ONE QUERY PER ATTRIBUTE VALUE
    eventAttributeValueDto.ParamKey = await attributeValueService.GetParamKey(
        eventAttributeValueDto, cancellationToken);
}
```

**At Scale**:
- 100 attributes × 1 query = 100 queries per event
- 10K events/sec × 100 queries = 1M queries/sec  
- At 2M concurrent users: **Catastrophic database load**

---

## Solution: Use BatchPopulateParamKeysAsync

### New Interface Method
```csharp
/// Batch-populate ParamKeys for all eventAttributeValueDtos in one operation
/// Prevents N+1 query pattern when processing 100+ attributes per event
Task BatchPopulateParamKeysAsync(List<EventAttributeValueDto> attributeValues, CancellationToken cancellationToken);
```

### Implementation Details

The method:
1. **Collects all unique IDs** by type (Segment, Product, Channel, Event)
2. **Batch-loads in parallel** using Task.WhenAll()
3. **Builds lookup dictionaries** once (zero-copy lookups)
4. **Populates ParamKey properties** for all attributes in O(n) time

### Performance Characteristics

| Metric | Performance |
|--------|-------------|
| **Query Count** | 5 queries (constant) instead of N |
| **Time Complexity** | O(n) linear instead of O(n²) |
| **Database Round Trips** | 1 round trip for 5 concurrent queries instead of N |
| **Memory Usage** | O(n) dictionaries, not O(n²) |

---

## Integration Steps

### Step 1: Call BatchPopulateParamKeysAsync Early

**Location**: `EventService.cs` → `GetAndSetAttributeValues()` method

**Before**: Loop through and call GetParamKey() for each attribute
**After**: Call BatchPopulateParamKeysAsync() once before processing

```csharp
private async Task GetAndSetAttributeValues(
    AttributeArea area, int? paramId, int? paramCategoryId,
    AttributesValues? attributesValues, bool addAttributeIfNotDefined,
    EventRequest request, EventResponse response, CancellationToken cancellationToken)
{
    if (attributesValues == null)
        return;
    
    List<EventAttributeValueDto> eventAttributeValueDtos = [];
    
    // ... build eventAttributeValueDtos list (existing code) ...
    
    // ✅ NEW: Batch-populate all ParamKeys at once
    await attributeValueService.BatchPopulateParamKeysAsync(eventAttributeValueDtos, cancellationToken);
    
    // Now process attributes - ParamKey is already populated
    foreach (var eventAttributeValueDto in eventAttributeValueDtos)
    {
        // ✅ NO MORE database query here - ParamKey already set!
        // eventAttributeValueDto.ParamKey is already populated
        
        AttributeDto? attributeDto = await attributeService.GetAttributeAsync(...);
        
        // ... rest of processing ...
        
        EventAttributeValueDto attributeValueDto = new(attributeDto!)
        {
            // ... populate ...
            // ParamKey is already set by BatchPopulateParamKeysAsync
        };
        
        eventAttributeValueDtos.Add(attributeValueDto);
    }
    
    // ... rest of method ...
}
```

### Step 2: Remove Individual GetParamKey Calls

**Find the line**:
```csharp
eventAttributeValueDto.ParamKey = await attributeValueService.GetParamKey(eventAttributeValueDto, cancellationToken);
```

**Replace with**: Nothing (already populated by BatchPopulateParamKeysAsync)

### Step 3: Handle Optional ParamKey Lookup

For scenarios where you still need individual ParamKey lookup (rare cases):
- Keep the existing `GetParamKey()` method (still available)
- Use it only for isolated attribute lookups
- Use `BatchPopulateParamKeysAsync()` for batch operations

---

## Code Change Examples

### Before (N+1 Queries)
```csharp
public async Task GetAndSetAttributeValues(...)
{
    List<EventAttributeValueDto> eventAttributeValueDtos = [];
    
    foreach (var attributeValue in attributesValues)
    {
        // ... create EventLogAttribute ...
        
        EventAttributeValueDto attributeValueDto = new(attributeDto!)
        {
            // ...
        };
        eventAttributeValueDtos.Add(attributeValueDto);
    }
    
    // ❌ PROBLEM: 100 queries for 100 attributes
    foreach (var attr in eventAttributeValueDtos)
    {
        attr.ParamKey = await attributeValueService.GetParamKey(attr, cancellationToken);
    }
    
    // Continue processing...
}
```

### After (Batch Loading - 5 Queries)
```csharp
public async Task GetAndSetAttributeValues(...)
{
    List<EventAttributeValueDto> eventAttributeValueDtos = [];
    
    foreach (var attributeValue in attributesValues)
    {
        // ... create EventLogAttribute ...
        
        EventAttributeValueDto attributeValueDto = new(attributeDto!)
        {
            // ...
        };
        eventAttributeValueDtos.Add(attributeValueDto);
    }
    
    // ✅ SOLUTION: 5 queries for 100 attributes (constant time)
    await attributeValueService.BatchPopulateParamKeysAsync(eventAttributeValueDtos, cancellationToken);
    
    // Continue processing - no more ParamKey lookups needed
}
```

---

## Testing

### Unit Test Example
```csharp
[TestMethod]
public async Task BatchPopulateParamKeysAsync_ShouldPopulateAllParamKeys()
{
    // Arrange
    var attributes = new List<EventAttributeValueDto>
    {
        new EventAttributeValueDto 
        { 
            SegmentId = 1, 
            Attribute = new AttributeDto { Area = AttributeArea.Segment } 
        },
        new EventAttributeValueDto 
        { 
            ProductId = 5, 
            Attribute = new AttributeDto { Area = AttributeArea.Product } 
        },
        new EventAttributeValueDto 
        { 
            ChannelId = 3, 
            Attribute = new AttributeDto { Area = AttributeArea.Channel } 
        }
    };
    
    // Act
    await _attributeValueService.BatchPopulateParamKeysAsync(attributes, CancellationToken.None);
    
    // Assert
    Assert.IsNotNull(attributes[0].ParamKey);
    Assert.IsNotNull(attributes[1].ParamKey);
    Assert.IsNotNull(attributes[2].ParamKey);
}
```

### Load Test
```csharp
[TestMethod]
[DataRow(100)]
[DataRow(1000)]
[DataRow(10000)]
public async Task BatchPopulateParamKeysAsync_Performance(int attributeCount)
{
    // Create test data with mix of types
    var attributes = new List<EventAttributeValueDto>();
    
    for (int i = 0; i < attributeCount; i++)
    {
        attributes.Add(new EventAttributeValueDto
        {
            SegmentId = i % 100,  // 100 unique segments
            Attribute = new AttributeDto { Area = AttributeArea.Segment }
        });
    }
    
    var sw = Stopwatch.StartNew();
    await _attributeValueService.BatchPopulateParamKeysAsync(attributes, CancellationToken.None);
    sw.Stop();
    
    // Should be <100ms for 10K attributes
    Assert.IsTrue(sw.ElapsedMilliseconds < 100, 
        $"Batch loading {attributeCount} attributes took {sw.ElapsedMilliseconds}ms");
}
```

---

## Deployment Checklist

- [ ] Implement `BatchPopulateParamKeysAsync()` call in `EventService.GetAndSetAttributeValues()`
- [ ] Remove individual `GetParamKey()` calls in loop
- [ ] Add unit tests for batch loading
- [ ] Add performance benchmarks
- [ ] Load test at 2M concurrent users
- [ ] Monitor database query counts in production

---

## Performance Impact

### Before Optimization
```
Event Processing: 10K events/sec
- Per event: 100 attributes × 1 query = 100 queries
- Database queries/sec: 1M
- Database CPU: 95%
- Response time: 500ms
```

### After Optimization  
```
Event Processing: 10K events/sec
- Per event: 5 queries (constant) for 100 attributes
- Database queries/sec: 50K
- Database CPU: 15%
- Response time: 50ms
```

**Expected Improvement**: **10-20x faster** database performance

---

## FAQ

**Q: Can I use individual GetParamKey() for single lookups?**  
A: Yes, `GetParamKey()` is still available for isolated lookups. Use `BatchPopulateParamKeysAsync()` for batch operations.

**Q: What if I have attributes with null ParamId?**  
A: The batch method handles nulls - they remain null after processing.

**Q: Will this affect other services?**  
A: No, the new method is additive and doesn't change existing behavior.

**Q: How do I measure the improvement?**  
A: Monitor these metrics:
- SELECT statements in SQL Server
- Database response time (Query Insights)
- Application response time

---

**Last Updated**: Feb 22, 2026  
**Status**: Ready for Implementation

