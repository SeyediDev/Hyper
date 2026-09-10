namespace Hyper.Domain.Entities.Referal;

/// <summary>
/// ثبت معرفی‌های موفق مشتریان
/// این موجودیت برای ردیابی دعوت‌های موفق و مدیریت پاداش‌ها استفاده می‌شود
/// </summary>
[DisplayName("معرفی مشتری")]
[EntityIndex($"{nameof(ReferrerCodeId)},{nameof(ReferredCustomerTenantId)},{nameof(IsDeleted)}")]
public class CustomerReferrer : HyperBaseCoreAuditableEntity<int>
{
    public int ReferrerCodeId { get; set; }
    [DisplayName("کد معرف")]
    public ReferrerCode ReferrerCode { get; set; } = null!;

    public int ReferrerCustomerTenantId { get; set; }
    [DisplayName("مشتری معرف")]
    public CustomerTenant ReferrerCustomerTenant { get; set; } = null!;

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
