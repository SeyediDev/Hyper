namespace Hyper.Domain.Entities.Rewards;

/// <summary>
/// می خواهیم دارایی های قابل فروش هر پاداش را فهرست کنیم
/// می خواهیم خریدهای هر مشتری را به عنوان دارایی ذخیره کنیم
/// </summary>
[DisplayName("دارایی پاداش")]
[SBVR(SBVRModality.Obligatory, "مدیریت دارایی‌های پاداش", "هر پاداش باید دارایی‌های قابل فروش مشخصی داشته باشد تا توزیع و مدیریت موجودی امکان‌پذیر باشد")]
[SBVR(SBVRModality.Recommended, "مدیریت دارایی‌های پاداش", "دارایی برای ردیابی موجودی پاداش‌ها، توزیع به مشتریان و تحلیل اثربخشی کاتالوگ استفاده می‌شود")]
[SBVR(SBVRModality.Calculated, "مدیریت دارایی‌های پاداش", "در زمان خرید پاداش توسط مشتری، دارایی‌های فیزیکی یا دیجیتالی به وی تخصیص داده می‌شود. " +
    "Quantity بیانگر مقدار کل، ConsumedQuantity مقدار مصرف شده و RemainingQuantity مقدار باقیمانده را نمایش می‌دهد")]
[OldDbMap("AwardAssets")]
public class RewardAsset : HyperBaseCoreAuditableEntity<int>
{
    [OldDbMap("ProductId")]
    public int RewardId { get; set; }
    [DisplayName("پاداش")]
    [SBVR(SBVRModality.Obligatory, "رابطه پاداش-دارایی", "هر دارایی باید به یک پاداش مشخص تعلق داشته باشد تا مدیریت موجودی و فروش امکان‌پذیر باشد")]
    [SBVR(SBVRModality.Recommended, "رابطه پاداش-دارایی", "رابطه پاداش-دارایی برای ردیابی موجودی، تحلیل فروش و بهینه‌سازی کاتالوگ استفاده می‌شود")]
    public Reward Reward { get; set; } = null!;
    
    public int? CustomerTenantId { get; set; }
    [DisplayName("مشتری")]
    [SBVR(SBVRModality.Permitted, "تخصیص دارایی", "مشتری برای تخصیص دارایی به مشتریان پس از خرید پاداش استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "تخصیص دارایی", "اگر مشتری مشخص باشد، دارایی به آن مشتری تخصیص داده شده است")]
    public CustomerTenant? CustomerTenant { get; set; }
    
    [DisplayName("سریال")] 
    [MaxLength(61)]
    [SBVR(SBVRModality.Permitted, "شناسه یکتا دارایی", "سریال برای تولید کدهای یکتا برای دارایی‌های فیزیکی یا دیجیتالی استفاده می‌شود")]
    public string? Serial { get; set; }

    [DisplayName("تعداد")]
    [SBVR(SBVRModality.Obligatory, "مقدار دارایی", "تعداد برای تعیین مقدار موجود دارایی ضروری است")]
    [SBVR(SBVRModality.Calculated, "مقدار دارایی", "تعداد در زمان خرید یا واگذاری دارایی تعیین می‌شود")]
    public int Quantity { get; set; }
    [DisplayName("تعداد مصرف شده")]
    [SBVR(SBVRModality.Calculated, "مصرف دارایی", "تعداد مصرف شده برای ردیابی استفاده از دارایی و مدیریت موجودی استفاده می‌شود")]
    public int ConsumedQuantity { get; set; }

    [DisplayName("تعداد باقیمانده")]
    [SBVR(SBVRModality.Calculated, "دارایی باقیمانده", "تعداد باقیمانده برای نمایش مقدار قابل استفاده دارایی محاسبه می‌شود")]
    public int RemainingQuantity => Quantity - ConsumedQuantity;

    public long? EventLogId { get; set; }
    [DisplayName("لاگ رویداد")]
    [SBVR(SBVRModality.Calculated, "ردیابی ایجاد", "لاگ رویداد برای ردیابی زمان و نحوه ایجاد دارایی استفاده می‌شود")]
    public EventLog? EventLog { get; set; }

    [OldDbMap("ScoringRuleId")]
    public int? PromotionId { get; set; }
    [DisplayName("پویش")]
    [SBVR(SBVRModality.Permitted, "منبع پویش", "پویش برای ردیابی پویشی که این پاداش از طریق آن بدست آمده استفاده می‌شود")]
    public Promotion? Promotion { get; set; }

    [OldDbMap("ScoringRuleActionId")]
    public int? PromotionActionId { get; set; }
    [DisplayName("عملیات پویش")]
    [SBVR(SBVRModality.Permitted, "عملیات پویش", "عملیات پویش برای ردیابی عملیات خاصی که این پاداش را ایجاد کرده استفاده می‌شود")]
    public PromotionAction? PromotionAction { get; set; }
}
