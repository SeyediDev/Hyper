namespace Hyper.Domain.Entities.Customers;

/// <summary>
/// ثبت معرفی‌های موفق مشتریان
/// این موجودیت برای ردیابی دعوت‌های موفق و مدیریت پاداش‌ها استفاده می‌شود
/// </summary>
[DisplayName("معرفی مشتری")]
[EntityIndex($"{nameof(ReferrerCodeId)},{nameof(ReferredCustomerId)},{nameof(IsDeleted)}")]
public class CustomerReferrer : HyperBaseCoreAuditableEntity<int>
{
    public int TenantId { get; set; }
    [DisplayName("اکوسیستم")]
    public Tenant Tenant { get; set; } = null!;

    public int ReferrerCodeId { get; set; }
    [DisplayName("کد معرف")]
    public ReferrerCode ReferrerCode { get; set; } = null!;

    /// <summary>
    /// مشتری معرف
    /// </summary>
    public int ReferrerCustomerId { get; set; }

    public int ReferrerCustomerTenantId { get; set; }
    [DisplayName("مشتری معرف")]
    public CustomerTenant ReferrerCustomerTenant { get; set; } = null!;

    /// <summary>
    /// مشتری دعوت شده
    /// </summary>
    public int ReferredCustomerId { get; set; }

    public int ReferredCustomerTenantId { get; set; }
    [DisplayName("مشتری دعوت شده")]
    public CustomerTenant ReferredCustomerTenant { get; set; } = null!;

    public long EventLogId { get; set; }
    [DisplayName("لاگ رویداد")]
    public EventLog EventLog { get; set; } = null!;

    [OldDbMap("RuleId")]
    public int? PromotionId { get; set; }
    [DisplayName("پویش")]
    public Promotion? Promotion { get; set; }

    [OldDbMap("RuleActionId")]
    public int? PromotionActionId { get; set; }
    [DisplayName("عملیات پویش")]
    public PromotionAction? PromotionAction { get; set; }
}
