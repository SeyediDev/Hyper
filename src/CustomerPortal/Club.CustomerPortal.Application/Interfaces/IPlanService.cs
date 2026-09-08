using Hyper.Domain.Entities.Promotions.Plans.Data;
using Hyper.Domain.Entities.Promotions.Plans.Enums;

namespace Hyper.CustomerPortal.Application.Interfaces;

/// <summary>
/// سرویس مدیریت طرح‌های اشتراک
/// </summary>
public interface IPlanService
{
    /// <summary>
    /// دریافت لیست طرح‌های موجود
    /// </summary>
    Task<PaginatedList<PlanServiceDto>> GetAvailablePlansAsync(
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت جزئیات یک طرح
    /// </summary>
    Task<PlanServiceDto?> GetPlanByIdAsync(int planId, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت طرح فعال مشتری
    /// </summary>
    Task<CustomerPlanServiceDto?> GetActiveCustomerPlanAsync(int customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// منقضی کردن طرح‌هایی که زمان آن‌ها به پایان رسیده است
    /// </summary>
    Task<int> ExpireExpiredPlansAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// خرید طرح توسط مشتری
    /// </summary>
    Task<PurchasePlanResult> PurchasePlanAsync(int customerId, int planId, CancellationToken cancellationToken = default);

    /// <summary>
    /// محاسبه قیمت ریوارد با احتساب تخفیف طرح
    /// </summary>
    Task<long> CalculateRewardPriceWithPlanDiscountAsync(
        int customerId,
        int rewardId,
        int? pointId,
        long originalPrice,
        CustomerPlanServiceDto? activePlan = null,
        CancellationToken cancellationToken = default);
}

public class PlanServiceDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public long PriceInPoints { get; set; }
    public string PointTypeName { get; set; } = null!;
    public string? PointTypeColor { get; set; }
    public int PointId { get; set; }
    public int ValidityDays { get; set; }
    public PlanDiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public bool IsGlobalDiscount { get; set; }
    public int? PictureId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CustomerPlanServiceDto
{
    public int Id { get; set; }
    public int CustomerTenantId { get; set; }
    public int PlanId { get; set; }
    public int PointId { get; set; }
    public string PointTypeName { get; set; } = null!;
    public string? PointTypeColor { get; set; }
    public string PlanTitle { get; set; } = null!;
    public string? PlanDescription { get; set; }
    public int? PlanPictureId { get; set; }
    public DateTime PurchaseDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public CustomerPlanStatus Status { get; set; }
    public long PaidAmount { get; set; }
    public int UsageCount { get; set; }
    public long TotalDiscountReceived { get; set; }
    public PlanDiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public bool IsGlobalDiscount { get; set; }
    public bool IsValid { get; set; }
}

public class PurchasePlanResult
{
    public int CustomerPlanId { get; set; }
    public DateTime ExpiryDate { get; set; }
}

