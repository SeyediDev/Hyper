using System.Globalization;

namespace Hyper.Domain.Features;

/// <summary>
/// سرویس مدیریت جامعه/بازار مشتریان
/// شامل ارزیابی شرایط عضویت و مدیریت عضویت‌ها
/// </summary>
public interface ICustomerSegmentService
{
    /// <summary>
    /// بررسی واجد شرایط بودن مشتری برای عضویت در جامعه/بازار
    /// </summary>
    /// <param name="customerId">شناسه مشتری</param>
    /// <param name="segmentId">شناسه جامعه/بازار</param>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>آیا مشتری واجد شرایط است</returns>
    [Telemetry]
    Task<CustomerSegmentEligibilityResult> CheckCustomerEligibilityAsync(
        int customerId, 
        int segmentId, 
        CancellationToken cancellationToken);

    /// <summary>
    /// عضو کردن مشتری در جامعه/بازار
    /// </summary>
    /// <param name="customerId">شناسه مشتری</param>
    /// <param name="segmentId">شناسه جامعه/بازار</param>
    /// <param name="eventLogId">شناسه لاگ رویداد (اختیاری)</param>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>شناسه عضویت ایجاد شده</returns>
    [Telemetry]
    Task<CustomerSegmentMembershipResult> JoinCustomerToSegmentAsync(
        int customerId, 
        int segmentId, 
        long? eventLogId,
        CancellationToken cancellationToken);

    /// <summary>
    /// دریافت جامعه‌ها/بازارهایی که مشتری واجد شرایط عضویت در آن‌ها است
    /// </summary>
    /// <param name="customerId">شناسه مشتری</param>
    /// <param name="onlyVisibleInPortal">فقط جامعه‌ها/بازارهای قابل نمایش در پرتال</param>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>لیست جامعه‌ها/بازارهای واجد شرایط</returns>
    [Telemetry]
    Task<List<CustomerSegmentEligibilityInfo>> GetEligibleSegmentsForCustomerAsync(
        int customerId, 
        bool onlyVisibleInPortal,
        CancellationToken cancellationToken);

    /// <summary>
    /// بررسی خودکار و عضویت مشتری در جامعه‌ها/بازارهای مناسب پس از رویداد
    /// </summary>
    /// <param name="customerId">شناسه مشتری</param>
    /// <param name="eventLogId">شناسه لاگ رویداد</param>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>تعداد جامعه‌ها/بازارهایی که مشتری به آن‌ها اضافه شد</returns>
    [Telemetry]
    Task<int> AutoJoinCustomerToEligibleSegmentsAsync(
        int customerId, 
        long eventLogId,
        CancellationToken cancellationToken);
}

/// <summary>
/// نتیجه بررسی واجد شرایط بودن برای عضویت در جامعه/بازار
/// </summary>
public record CustomerSegmentEligibilityResult
{
    /// <summary>
    /// آیا مشتری واجد شرایط است
    /// </summary>
    public bool IsEligible { get; init; }

    /// <summary>
    /// آیا مشتری از قبل عضو است
    /// </summary>
    public bool IsAlreadyMember { get; init; }

    /// <summary>
    /// پیام توضیحی (در صورت عدم واجد شرایط بودن)
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    /// شرایطی که برقرار نشده‌اند
    /// </summary>
    public List<string>? FailedConditions { get; init; }
}

/// <summary>
/// نتیجه عملیات عضویت در جامعه/بازار
/// </summary>
public record CustomerSegmentMembershipResult
{
    /// <summary>
    /// آیا عملیات موفق بود
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// شناسه عضویت ایجاد شده
    /// </summary>
    public int? MembershipId { get; init; }

    /// <summary>
    /// پیام
    /// </summary>
    public string? Message { get; init; }
}

/// <summary>
/// اطلاعات جامعه/بازار واجد شرایط
/// </summary>
public record CustomerSegmentEligibilityInfo
{
    /// <summary>
    /// شناسه جامعه/بازار
    /// </summary>
    public int SegmentId { get; init; }

    /// <summary>
    /// عنوان جامعه/بازار
    /// </summary>
    public string Title { get; init; } = null!;

    /// <summary>
    /// توضیحات جامعه/بازار
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// آدرس تصویر
    /// </summary>
    public string? ImageUrl { get; init; }

    /// <summary>
    /// تعداد اعضا
    /// </summary>
    public int MemberCount { get; init; }

    /// <summary>
    /// آیا مشتری عضو است
    /// </summary>
    public bool IsMember { get; init; }

    /// <summary>
    /// نحوه عضویت
    /// </summary>
    public CustomerSegmentJoinMode JoinMode { get; init; }

    /// <summary>
    /// مزایا (JSON array deserialize شده)
    /// </summary>
    public List<string>? Benefits { get; init; }
}

/// <summary>
/// پیاده‌سازی سرویس مدیریت جامعه/بازار مشتریان
/// </summary>
internal class CustomerSegmentService(
    IQueryRepository<CustomerSegment, int> segmentRepo,
    IQueryRepository<CustomerSegmentKindCondition, int> conditionRepo,
    IQueryRepository<CustomerSegmentMembership, int> membershipQueryRepo,
    ICommandRepository<CustomerSegmentMembership, int> membershipCmdRepo,
    IQueryRepository<CustomerTenant, int> customerTenantQueryRepo,
    IQueryRepository<CustomerParameter, int> customerParameterQueryRepo,
    IQueryRepository<CustomerParameterValue, int> customerParameterValueQueryRepo,
    IQueryRepository<Customer, int> customerRepo,
    ICustomerService customerService
    // IEvaluateFormulaService evaluateFormulaService - REMOVED to break circular dependency
) : ICustomerSegmentService
{
    public async Task<CustomerSegmentEligibilityResult> CheckCustomerEligibilityAsync(
        int customerId,
        int segmentId,
        CancellationToken cancellationToken)
    {
        // دریافت جامعه/بازار
        var segment = await segmentRepo.GetByIdAsync(segmentId, cancellationToken);
        if (segment == null)
        {
            return new CustomerSegmentEligibilityResult
            {
                IsEligible = false,
                IsAlreadyMember = false,
                Message = "جامعه/بازار مورد نظر یافت نشد"
            };
        }

		// دریافت رابطه مشتری-اکوسیستم
		CustomerTenant? customerTenant = await customerTenantQueryRepo.FirstOrDefaultAsync(
            x => x.CustomerId == customerId && x.TenantId == segment.TenantId, cancellationToken);

        if (customerTenant == null)
        {
            return new CustomerSegmentEligibilityResult
            {
                IsEligible = false,
                IsAlreadyMember = false,
                Message = "مشتری در این اکوسیستم عضو نیست"
			};
        }

        // بررسی عضویت فعلی
        var existingMembership = await membershipQueryRepo.FirstOrDefaultAsync(
            m => m.CustomerTenantId == customerTenant.Id && m.SegmentId == segmentId,
            cancellationToken);

        if (existingMembership != null)
        {
            return new CustomerSegmentEligibilityResult
            {
                IsEligible = false,
                IsAlreadyMember = true,
                Message = "مشتری از قبل عضو این جامعه/بازار است"
            };
        }

        // بررسی فعال بودن جامعه/بازار
        if (!segment.IsActive)
        {
            return new CustomerSegmentEligibilityResult
            {
                IsEligible = false,
                IsAlreadyMember = false,
                Message = "جامعه/بازار غیرفعال است"
            };
        }

        // دریافت مشتری
        var customerQuery = await customerRepo.GetAllAsync(cancellationToken, c => c.Id == customerId);
        var customer = customerQuery.FirstOrDefault();
        if (customer == null)
        {
            return new CustomerSegmentEligibilityResult
            {
                IsEligible = false,
                IsAlreadyMember = false,
                Message = "مشتری یافت نشد"
            };
        }

        // دریافت شرایط جامعه/بازار
        var conditions = await conditionRepo.GetAllAsync(cancellationToken);
        var segmentConditions = conditions
            .Where(c => c.CustomerSegmentId == segmentId)
            .OrderBy(c => c.Priority)
            .ToList();

        // اگر شرطی وجود ندارد، مشتری واجد شرایط است
        if (!segmentConditions.Any())
        {
            return new CustomerSegmentEligibilityResult
            {
                IsEligible = true,
                IsAlreadyMember = false,
                Message = "مشتری واجد شرایط عضویت است"
            };
        }

        // بررسی شرایط با منطق AND/OR
        var evaluationResult = await EvaluateConditionsAsync(
            customerTenant,
            customer, 
            segmentConditions, 
            cancellationToken);
        var isEligible = evaluationResult.Item1;
        var failedConditions = evaluationResult.Item2;

        return new CustomerSegmentEligibilityResult
        {
            IsEligible = isEligible,
            IsAlreadyMember = false,
            Message = isEligible ? "مشتری واجد شرایط عضویت است" : "مشتری واجد شرایط عضویت نیست",
            FailedConditions = failedConditions
        };
    }

    public async Task<CustomerSegmentMembershipResult> JoinCustomerToSegmentAsync(
        int customerId,
        int segmentId,
        long? eventLogId,
        CancellationToken cancellationToken)
    {
        // دریافت جامعه/بازار
        var segment = await segmentRepo.GetByIdAsync(segmentId, cancellationToken);
        if (segment == null)
        {
            return new CustomerSegmentMembershipResult
            {
                Success = false,
                Message = "جامعه/بازار مورد نظر یافت نشد"
            };
        }

		// دریافت رابطه مشتری-اکوسیستم
		CustomerTenant? customerTenant = await customerTenantQueryRepo.FirstOrDefaultAsync(
            x => x.CustomerId == customerId && x.TenantId == segment.TenantId, cancellationToken);

        if (customerTenant == null)
        {
            return new CustomerSegmentMembershipResult
            {
                Success = false,
                Message = "مشتری در این اکوسیستم عضو نیست"
			};
        }

        // بررسی عضویت قبلی
        var existingMembership = await membershipQueryRepo.FirstOrDefaultAsync(
            m => m.CustomerTenantId == customerTenant.Id && m.SegmentId == segmentId,
            cancellationToken);

        if (existingMembership != null)
        {
            return new CustomerSegmentMembershipResult
            {
                Success = false,
                Message = "مشتری از قبل عضو این جامعه/بازار است"
            };
        }

        // ایجاد عضویت جدید
        var membership = new CustomerSegmentMembership
        {
            CustomerTenantId = customerTenant.Id,
            CustomerTenant = customerTenant,
            SegmentId = segmentId,
            EventLogId = eventLogId ?? 0 // اگر eventLogId null بود، 0 قرار می‌دهیم (برای عضویت‌های دستی)
        };

        membershipCmdRepo.Add(membership);
        await membershipCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);

        return new CustomerSegmentMembershipResult
        {
            Success = true,
            MembershipId = membership.Id,
            Message = $"عضویت در جامعه/بازار {segment.Title} با موفقیت انجام شد"
        };
    }

    public async Task<List<CustomerSegmentEligibilityInfo>> GetEligibleSegmentsForCustomerAsync(
        int customerId,
        bool onlyVisibleInPortal,
        CancellationToken cancellationToken)
    {
        var result = new List<CustomerSegmentEligibilityInfo>();

        // دریافت مشتری
        var customerResult = await customerRepo.GetAllAsync(cancellationToken, c => c.Id == customerId);
        var customer = customerResult.FirstOrDefault();
        if (customer == null)
        {
            return result;
        }

		// دریافت اکوسیستم‌های فعال مشتری
		var customerTenants = (await customerTenantQueryRepo.GetAllAsync(
            cancellationToken,
            ct => ct.CustomerId == customerId)).ToList();

        if (!customerTenants.Any())
        {
            return result;
        }

        var tenantIds = customerTenants.Select(ct => ct.TenantId).ToHashSet();

        // دریافت جامعه‌ها/بازارهای فعال در سازمان‌های مشتری
        var segments = await segmentRepo.GetAllAsync(
            cancellationToken,
            s => tenantIds.Contains(s.TenantId));
        var activeSegments = segments
            .Where(s => s.IsActive)
            .Where(s => !onlyVisibleInPortal || s.IsVisibleInPortal)
            .ToList();

        // دریافت عضویت‌های فعلی مشتری
        var customerTenantIds = customerTenants.Select(ct => ct.Id).ToList();
        var memberships = await membershipQueryRepo.GetAllAsync(
            cancellationToken,
            m => customerTenantIds.Contains(m.CustomerTenantId));
        var customerMembershipIds = memberships
            .Select(m => m.SegmentId)
            .ToHashSet();

        foreach (var segment in activeSegments)
        {
            var isMember = customerMembershipIds.Contains(segment.Id);
            
            // برای جامعه‌ها/بازارهای SystemOnly که کاربر عضو نیست، نمایش نمی‌دهیم
            if (segment.JoinMode == CustomerSegmentJoinMode.SystemOnly && !isMember)
            {
                continue;
            }

            var eligibilityInfo = new CustomerSegmentEligibilityInfo
            {
                SegmentId = segment.Id,
                Title = segment.Title,
                Description = segment.Description,
                ImageUrl = segment.ImageUrl,
                MemberCount = segment.ActualSize,
                IsMember = isMember,
                JoinMode = segment.JoinMode,
                Benefits = ParseBenefits(segment.Benefits)
            };

            result.Add(eligibilityInfo);
        }

        return result;
    }

    public async Task<int> AutoJoinCustomerToEligibleSegmentsAsync(
        int customerId,
        long eventLogId,
        CancellationToken cancellationToken)
    {
        int joinedCount = 0;

        // دریافت مشتری
        var customerResult = await customerRepo.GetAllAsync(cancellationToken, c => c.Id == customerId);
        var customer = customerResult.FirstOrDefault();
        if (customer == null)
        {
            return joinedCount;
        }

        // دریافت جامعه‌ها/بازارهای فعال با JoinMode = SystemOnly یا WithConditionCheck
        // TODO: باید TenantId از CustomerTenant گرفته شود
        var segments = await segmentRepo.GetAllAsync(cancellationToken);
        var autoJoinSegments = segments
            .Where(s => s.IsActive &&
                       (s.JoinMode == CustomerSegmentJoinMode.SystemOnly || 
                        s.JoinMode == CustomerSegmentJoinMode.WithConditionCheck))
            .ToList();

        foreach (var segment in autoJoinSegments)
        {
            // بررسی واجد شرایط بودن
            var eligibility = await CheckCustomerEligibilityAsync(customerId, segment.Id, cancellationToken);
            
            if (eligibility.IsEligible && !eligibility.IsAlreadyMember)
            {
                // عضو کردن خودکار
                var result = await JoinCustomerToSegmentAsync(customerId, segment.Id, eventLogId, cancellationToken);
                if (result.Success)
                {
                    joinedCount++;
                }
            }
        }

        return joinedCount;
    }

    /// <summary>
    /// ارزیابی شرایط جامعه/بازار با منطق AND/OR
    /// شرایط یک گروه با AND و گروه‌های مختلف با OR بررسی می‌شوند
    /// </summary>
    private async Task<(bool isEligible, List<string>? failedConditions)> EvaluateConditionsAsync(
        CustomerTenant customerTenant,
        Customer customer,
        List<CustomerSegmentKindCondition> conditions,
        CancellationToken cancellationToken)
    {
        var failedConditions = new List<string>();

        // گروه‌بندی بر اساس ConditionGroup
        var groupedConditions = conditions.GroupBy(c => c.ConditionGroup ?? 0);

        foreach (var group in groupedConditions)
        {
            bool passGroup = true;
            var groupFailedConditions = new List<string>();

            // بررسی همه شرایط یک گروه (AND)
            foreach (var condition in group)
            {
                var conditionResult = await EvaluateSingleConditionAsync(customerTenant, customer, condition, cancellationToken);
                if (!conditionResult)
                {
                    passGroup = false;
                    groupFailedConditions.Add(condition.Title);
                }
            }

            // اگر حداقل یک گروه پاس شد (OR)
            if (passGroup)
            {
                return (true, null);
            }

            failedConditions.AddRange(groupFailedConditions);
        }

        // هیچ گروهی پاس نشد
        return (false, failedConditions);
    }

    /// <summary>
    /// ارزیابی یک شرط منفرد
    /// </summary>
    private async Task<bool> EvaluateSingleConditionAsync(
        CustomerTenant customerTenant,
        Customer customer,
        CustomerSegmentKindCondition condition,
        CancellationToken cancellationToken)
    {
        // اگر نوع شرط WithoutExtraCondition است، همیشه true
        if (condition.ConditionKind == CustomerSegmentConditionKind.WithoutExtraCondition)
        {
            return true;
        }

        // اگر نوع شرط Formula است، فرمول را ارزیابی می‌کنیم
        if (condition.ConditionKind == CustomerSegmentConditionKind.Formula)
        {
            return await EvaluateFormulaConditionAsync(customer, condition, cancellationToken);
        }

        // محاسبه مقدار مقایسه
        var compareValue = await CalculateCompareValueAsync(customerTenant, customer, condition, cancellationToken);
        
        // مقایسه بر اساس نوع شرط
        return CompareValues(compareValue, condition);
    }

    /// <summary>
    /// ارزیابی شرط فرمولی
    /// </summary>
    private Task<bool> EvaluateFormulaConditionAsync(
        Customer customer,
        CustomerSegmentKindCondition condition,
        CancellationToken cancellationToken)
    {
        // TODO: Formula evaluation temporarily disabled to break circular dependency
        // IEvaluateFormulaService -> IEventService -> ICustomerSegmentService -> IEvaluateFormulaService
        return Task.FromResult(false);
    }

    /// <summary>
    /// محاسبه مقدار برای مقایسه
    /// </summary>
    private async Task<object?> CalculateCompareValueAsync(
        CustomerTenant customerTenant,
        Customer customer,
        CustomerSegmentKindCondition condition,
        CancellationToken cancellationToken)
    {
        if (condition.CompareWith == null)
        {
            return condition.Value;
        }

        return condition.CompareWith switch
        {
            CustomerSegmentCompareWith.ConstantValue => condition.Value,
            CustomerSegmentCompareWith.CustomerParameter => await GetCustomerParameterValueAsync(
                customerTenant,
                condition,
                cancellationToken),
            CustomerSegmentCompareWith.CustomerPoint => await customerService.GetPointBalanceAsync(
                customerTenant.TenantId,
                1, // TODO: باید PointId از شرط یا جای دیگر بیاید
                customer.Id,
                cancellationToken),
            CustomerSegmentCompareWith.CustomerPointLevel => (await customerService
                .GetCustomerPointLevels(customer.Id, customerTenant.TenantId, cancellationToken))
                .Select(x => x.PointLevelId)
                .FirstOrDefault(),
            CustomerSegmentCompareWith.CustomerJoinDate => customerTenant.JoinDate,
            CustomerSegmentCompareWith.CustomerLastActivity => customerTenant.LastInteractionDate,
            CustomerSegmentCompareWith.CustomerPurchaseCount => customerTenant.TotalInteractions,
            CustomerSegmentCompareWith.CustomerTotalPurchase => customerTenant.TotalTransactionValue,
            CustomerSegmentCompareWith.CustomerAveragePurchase => customerTenant.AverageOrderValue,
            CustomerSegmentCompareWith.CustomerAge => CalculateAge(customer),
            CustomerSegmentCompareWith.CustomerGender => null, // Gender field removed from Customer entity
            _ => condition.Value
        };
    }

    /// <summary>
    /// مقایسه مقادیر بر اساس نوع شرط
    /// </summary>
    private bool CompareValues(object? actualValue, CustomerSegmentKindCondition condition)
    {
        if (actualValue == null)
        {
            return condition.ConditionKind == CustomerSegmentConditionKind.IsNull;
        }

        var expectedValue = condition.Value;

        return condition.ConditionKind switch
        {
            CustomerSegmentConditionKind.IsNull => actualValue == null,
            CustomerSegmentConditionKind.IsNotNull => actualValue != null,
            CustomerSegmentConditionKind.EqualTo => actualValue.ToString() == expectedValue,
            CustomerSegmentConditionKind.NotEqualTo => actualValue.ToString() != expectedValue,
            CustomerSegmentConditionKind.GreaterThan => CompareNumeric(actualValue, expectedValue, (a, b) => a > b),
            CustomerSegmentConditionKind.LessThan => CompareNumeric(actualValue, expectedValue, (a, b) => a < b),
            CustomerSegmentConditionKind.GreaterThanOrEqualTo => CompareNumeric(actualValue, expectedValue, (a, b) => a >= b),
            CustomerSegmentConditionKind.LessThanOrEqualTo => CompareNumeric(actualValue, expectedValue, (a, b) => a <= b),
            CustomerSegmentConditionKind.Contains => actualValue.ToString()?.Contains(expectedValue ?? "", StringComparison.OrdinalIgnoreCase) ?? false,
            CustomerSegmentConditionKind.StartsWith => actualValue.ToString()?.StartsWith(expectedValue ?? "", StringComparison.OrdinalIgnoreCase) ?? false,
            CustomerSegmentConditionKind.EndsWith => actualValue.ToString()?.EndsWith(expectedValue ?? "", StringComparison.OrdinalIgnoreCase) ?? false,
            CustomerSegmentConditionKind.InList => CompareList(actualValue, expectedValue, true),
            CustomerSegmentConditionKind.NotInList => CompareList(actualValue, expectedValue, false),
            CustomerSegmentConditionKind.InRange => CompareRange(actualValue, expectedValue, true),
            CustomerSegmentConditionKind.OutOfRange => CompareRange(actualValue, expectedValue, false),
            _ => false
        };
    }

    /// <summary>
    /// مقایسه عددی
    /// </summary>
    private bool CompareNumeric(object? actualValue, string? expectedValue, Func<decimal, decimal, bool> comparer)
    {
        if (actualValue == null || expectedValue == null)
        {
            return false;
        }

        if (decimal.TryParse(actualValue.ToString(), out var actual) &&
            decimal.TryParse(expectedValue, out var expected))
        {
            return comparer(actual, expected);
        }

        return false;
    }

    private bool CompareList(object? actualValue, string? expectedValue, bool shouldContain)
    {
        if (actualValue == null || string.IsNullOrWhiteSpace(expectedValue))
        {
            return false;
        }

        var tokens = expectedValue
            .Split([',', ';', '|'], StringSplitOptions.RemoveEmptyEntries)
            .Select(token => token.Trim())
            .Where(token => token.Length > 0)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (tokens.Count == 0)
        {
            return false;
        }

        var actualText = actualValue.ToString()?.Trim();
        if (string.IsNullOrEmpty(actualText))
        {
            return false;
        }

        var contains = tokens.Contains(actualText);
        return shouldContain ? contains : !contains;
    }

    private bool CompareRange(object? actualValue, string? expectedValue, bool shouldBeInside)
    {
        if (actualValue == null || string.IsNullOrWhiteSpace(expectedValue))
        {
            return false;
        }

        var parts = expectedValue
            .Split([',', ';', '|'], StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Trim())
            .ToArray();

        if (parts.Length < 2)
        {
            return false;
        }

        if (!decimal.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var min))
        {
            return false;
        }

        if (!decimal.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var max))
        {
            return false;
        }

        if (!decimal.TryParse(actualValue.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var actual))
        {
            return false;
        }

        var lowerBound = Math.Min(min, max);
        var upperBound = Math.Max(min, max);
        var inside = actual >= lowerBound && actual <= upperBound;

        return shouldBeInside ? inside : !inside;
    }

    private async Task<string?> GetCustomerParameterValueAsync(
        CustomerTenant customerTenant,
        CustomerSegmentKindCondition condition,
        CancellationToken cancellationToken)
    {
        int? parameterId = condition.CustomerParameterId;

        if (parameterId == null && !string.IsNullOrWhiteSpace(condition.CustomerParameterKey))
        {
            var parameter = await customerParameterQueryRepo.FirstOrDefaultAsync(
                x => x.TenantId == customerTenant.TenantId &&
                     x.Key == condition.CustomerParameterKey,
                cancellationToken);

            parameterId = parameter?.Id;
        }

        if (parameterId == null)
        {
            return null;
        }

        var value = await customerParameterValueQueryRepo.FirstOrDefaultAsync(
            x => x.CustomerTenantId == customerTenant.Id &&
                 x.ParameterId == parameterId.Value,
            cancellationToken);

        return value?.Value;
    }

    /// <summary>
    /// محاسبه سن مشتری
    /// </summary>
    private int? CalculateAge(Customer customer)
    {
        // TODO: باید فیلد تاریخ تولد در Customer وجود داشته باشد
        // در حال حاضر null برمی‌گردانیم
        return null;
    }

    /// <summary>
    /// تجزیه مزایا از JSON
    /// </summary>
    private List<string>? ParseBenefits(string? benefitsJson)
    {
        if (string.IsNullOrEmpty(benefitsJson))
        {
            return null;
        }

        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<string>>(benefitsJson);
        }
        catch
        {
            return null;
        }
    }
}

