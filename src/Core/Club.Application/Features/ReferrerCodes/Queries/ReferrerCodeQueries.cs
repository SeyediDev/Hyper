namespace Hyper.Application.Features.ReferrerCodes.Queries;

/// <summary>
/// کوئری دریافت کد معرف بر اساس شناسه
/// </summary>
public class GetReferrerCodeByIdQuery : IRequest<Result<ReferrerCodeDto>>
{
    public int Id { get; set; }
}

/// <summary>
/// کوئری دریافت کد معرف بر اساس کد
/// </summary>
public class GetReferrerCodeByCodeQuery : IRequest<Result<ReferrerCodeDto>>
{
    public string Code { get; set; } = null!;
    public string TenantKey { get; set; } = null!;
}

/// <summary>
/// کوئری دریافت لیست کدهای معرف مشتری
/// </summary>
public class GetReferrerCodesByCustomerQuery : IRequest<Result<List<ReferrerCodeDto>>>
{
    public int CustomerId { get; set; }
    public string TenantKey { get; set; } = null!;
}

/// <summary>
/// کوئری دریافت لیست کدهای معرف فعال
/// </summary>
public class GetActiveReferrerCodesQuery : IRequest<Result<List<ReferrerCodeDto>>>
{
    public string TenantKey { get; set; } = null!;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

/// <summary>
/// کوئری اعتبارسنجی کد معرف
/// </summary>
public class ValidateReferrerCodeQuery : IRequest<Result<bool>>
{
    public string Code { get; set; } = null!;
    public string TenantKey { get; set; } = null!;
}

/// <summary>
/// کوئری دریافت آمار کد معرف
/// </summary>
public class GetReferrerCodeStatsQuery : IRequest<Result<ReferrerCodeStatsDto>>
{
    public int Id { get; set; }
}

/// <summary>
/// DTO کد معرف
/// </summary>
public class ReferrerCodeDto
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;
    public string Code { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime? UsedAt { get; set; }
    public int UsageCount { get; set; }
    public int? MaxUsageCount { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}

/// <summary>
/// DTO آمار کد معرف
/// </summary>
public class ReferrerCodeStatsDto
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public int TotalUsage { get; set; }
    public int SuccessfulReferrals { get; set; }
    public int TotalEarnedPoints { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public bool IsExpired { get; set; }
    public bool IsMaxUsageReached { get; set; }
}
