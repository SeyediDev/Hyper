namespace Hyper.Domain.Features.Attributes;

/// <summary>
/// سرویس کوئری مقادیر تجمیع شده ویژگی‌ها
/// </summary>
public interface IAttributeAggregationQueryService
{
    /// <summary>
    /// دریافت مقادیر تجمیع شده برای استفاده در فرمول‌ها و شرط‌ها
    /// </summary>
    Task<AttributesValues> GetAggregatedValuesAsync(
        int tenantId, int? customerTenantId, AttributeArea area, int? paramId, int? paramCategoryId,
        CancellationToken cancellationToken);

    /// <summary>
    /// محاسبه تجمیع روزانه برای یک ویژگی خاص
    /// </summary>
    Task<TenantAttributeDailyAggregation?> GetDailyAggregationAsync(
        int tenantId, int attributeId, int? customerTenantId, DateTime date,
        CancellationToken cancellationToken);

    /// <summary>
    /// محاسبه تجمیع ماهانه شمسی
    /// </summary>
    Task<TenantAttributeMonthlyAggregationPersian?> GetMonthlyAggregationPersianAsync(
        int tenantId, int attributeId, int? customerTenantId, int shamsiYear, int shamsiMonth,
        CancellationToken cancellationToken);

    /// <summary>
    /// محاسبه تجمیع ماهانه میلادی
    /// </summary>
    Task<TenantAttributeMonthlyAggregationGregorian?> GetMonthlyAggregationGregorianAsync(
        int tenantId, int attributeId, int? customerTenantId, int gregorianYear, int gregorianMonth,
        CancellationToken cancellationToken);

    /// <summary>
    /// محاسبه آمار یک ویژگی از روی داده‌های خام (On-Demand)
    /// </summary>
    Task<AttributeStatistics> CalculateStatisticsAsync(
        int tenantId, int attributeId, int? customerTenantId, DateTime? fromDate, DateTime? toDate,
        CancellationToken cancellationToken);
}

/// <summary>
/// آمار محاسبه شده یک ویژگی
/// </summary>
public class AttributeStatistics
{
    public long Count { get; set; }
    public decimal Sum { get; set; }
    public decimal Average { get; set; }
    public decimal? MinValue { get; set; }
    public decimal? MaxValue { get; set; }
    public string? FirstValue { get; set; }
    public string? LastValue { get; set; }
    public long DistinctCount { get; set; }
}
internal class AttributeAggregationQueryService(
    IQueryRepositoryL<TenantAttributeDailyAggregation> dailyAggregationRepository,
    IQueryRepositoryL<TenantAttributeMonthlyAggregationPersian> monthlyPersianRepository,
    IQueryRepositoryL<TenantAttributeMonthlyAggregationGregorian> monthlyGregorianRepository,
    IQueryRepositoryL<TenantAttributeValue> attributeValueRepository) : IAttributeAggregationQueryService
{
    /// <summary>
    /// دریافت مقادیر تجمیع شده برای استفاده در فرمول‌ها و شرط‌ها
    /// اولویت: روز -> ماه -> مقدار آخر
    /// </summary>
    public async Task<AttributesValues> GetAggregatedValuesAsync(
        int tenantId, int? customerTenantId, AttributeArea area, int? paramId, int? paramCategoryId,
        CancellationToken cancellationToken)
    {
        var result = new AttributesValues();

        // دریافت ویژگی‌های تعریف شده برای این ناحیه
        var lastDailyAggregations = await dailyAggregationRepository.GetAllAsync(
            cancellationToken,
            x => x.IsDeleted == false &&
                 x.TenantId == tenantId &&
                 (customerTenantId == null || x.CustomerTenantId == customerTenantId) &&
                 x.Attribute.Area == area
        );

        // دریافت جدیدترین رکورد برای هر ویژگی (بر اساس تاریخ تجمیع)
        var latestAggregations = lastDailyAggregations
            .GroupBy(x => x.AttributeId)
            .Select(g => g.OrderByDescending(x => x.AggregationDate).FirstOrDefault())
            .Where(x => x != null)
            .ToList();

        // اضافه کردن مقادیر تجمیع شده
        foreach (var agg in latestAggregations!)
        {
            var attributeKey = agg!.Attribute.Key;
            
            // تبدیل مقدار بر اساس نوع داده
            object convertedValue = ConvertAttributeValue(agg.Average, agg.Attribute.ValueType);
            result.TryAdd(attributeKey, convertedValue);
        }

        return result;
    }

    /// <summary>
    /// محاسبه تجمیع روزانه برای یک ویژگی خاص
    /// </summary>
    public async Task<TenantAttributeDailyAggregation?> GetDailyAggregationAsync(
        int tenantId, int attributeId, int? customerTenantId, DateTime date,
        CancellationToken cancellationToken)
    {
        var targetDate = date.Date;
        
        var aggregation = await dailyAggregationRepository.FirstOrDefaultAsync(
            x => x.IsDeleted == false &&
                 x.TenantId == tenantId &&
                 x.AttributeId == attributeId &&
                 (customerTenantId == null || x.CustomerTenantId == customerTenantId) &&
                 x.Year == targetDate.Year &&
                 x.Month == targetDate.Month &&
                 x.Day == targetDate.Day,
            cancellationToken);

        return aggregation;
    }

    /// <summary>
    /// محاسبه تجمیع ماهانه شمسی
    /// </summary>
    public async Task<TenantAttributeMonthlyAggregationPersian?> GetMonthlyAggregationPersianAsync(
        int tenantId, int attributeId, int? customerTenantId, int shamsiYear, int shamsiMonth,
        CancellationToken cancellationToken)
    {
        var aggregation = await monthlyPersianRepository.FirstOrDefaultAsync(
            x => x.IsDeleted == false &&
                 x.TenantId == tenantId &&
                 x.AttributeId == attributeId &&
                 (customerTenantId == null || x.CustomerTenantId == customerTenantId) &&
                 x.ShamsiYear == shamsiYear &&
                 x.ShamsiMonth == shamsiMonth,
            cancellationToken);

        return aggregation;
    }

    /// <summary>
    /// محاسبه تجمیع ماهانه میلادی
    /// </summary>
    public async Task<TenantAttributeMonthlyAggregationGregorian?> GetMonthlyAggregationGregorianAsync(
        int tenantId, int attributeId, int? customerTenantId, int gregorianYear, int gregorianMonth,
        CancellationToken cancellationToken)
    {
        var aggregation = await monthlyGregorianRepository.FirstOrDefaultAsync(
            x => x.IsDeleted == false &&
                 x.TenantId == tenantId &&
                 x.AttributeId == attributeId &&
                 (customerTenantId == null || x.CustomerTenantId == customerTenantId) &&
                 x.GregorianYear == gregorianYear &&
                 x.GregorianMonth == gregorianMonth,
            cancellationToken);

        return aggregation;
    }

    /// <summary>
    /// محاسبه آمار یک ویژگی از روی داده‌های خام (On-Demand)
    /// استفاده در مواردی که تجمیع از پیش انجام نشده است
    /// </summary>
    public async Task<AttributeStatistics> CalculateStatisticsAsync(
        int tenantId, int attributeId, int? customerTenantId, DateTime? fromDate, DateTime? toDate,
        CancellationToken cancellationToken)
    {
        // دریافت داده‌های خام فیلتر شده
        var query = attributeValueRepository.GetAllAsync(
            cancellationToken,
            x => x.IsDeleted == false &&
                 x.TenantId == tenantId &&
                 x.AttributeId == attributeId &&
                 (customerTenantId == null || x.CustomerTenantId == customerTenantId) &&
                 (fromDate == null || x.EventLog.CreateDate >= fromDate) &&
                 (toDate == null || x.EventLog.CreateDate <= toDate)
        );

        var values = await query;

        // اگر هیچ داده‌ای وجود نداشت
        if (!values.Any())
        {
            return new AttributeStatistics
            {
                Count = 0,
                Sum = 0,
                Average = 0,
                MinValue = null,
                MaxValue = null,
                FirstValue = null,
                LastValue = null,
                DistinctCount = 0
            };
        }

        // تبدیل مقادیر رشته‌ای به اعداد برای محاسبات
        var numericValues = values
            .Select(v => TryParseDecimal(v.Value))
            .Where(v => v.HasValue)
            .Select(v => v!.Value)
            .ToList();

        var stringValues = values.Select(v => v.Value).ToList();

        return new AttributeStatistics
        {
            Count = stringValues.Count,
            Sum = numericValues.Any() ? numericValues.Sum() : 0,
            Average = numericValues.Any() ? numericValues.Average() : 0,
            MinValue = numericValues.Any() ? numericValues.Min() : null,
            MaxValue = numericValues.Any() ? numericValues.Max() : null,
            FirstValue = stringValues.FirstOrDefault(),
            LastValue = stringValues.LastOrDefault(),
            DistinctCount = stringValues.Distinct().Count()
        };
    }

    /// <summary>
    /// تبدیل مقدار اعشاری به نوع داده مناسب
    /// </summary>
    private static object ConvertAttributeValue(decimal value, TenantAttributeType valueType)
    {
        return valueType switch
        {
            TenantAttributeType.Integer => (long)value,
            TenantAttributeType.String => value.ToString("F2"),
            TenantAttributeType.Boolean => value != 0,
            TenantAttributeType.DateTime => value.ToString("F2"),
            TenantAttributeType.DateOnly => value.ToString("F2"),
            _ => value // Decimal
        };
    }

    /// <summary>
    /// تلاش برای تبدیل رشته‌ی مقدار به اعشار
    /// </summary>
    private static decimal? TryParseDecimal(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (decimal.TryParse(value.Trim(), out var result))
            return result;

        // سعی برای تبدیل بولی
        if (value.Equals("true", StringComparison.OrdinalIgnoreCase))
            return 1;
        if (value.Equals("false", StringComparison.OrdinalIgnoreCase))
            return 0;

        return null;
    }
}
