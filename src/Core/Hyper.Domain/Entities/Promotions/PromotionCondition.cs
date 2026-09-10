namespace Hyper.Domain.Entities.Promotions;

/// <summary>
/// شرط پویش - شرایطی که باید برقرار شوند تا پویش فعال شود
/// برای Campaign: رویدادهای مورد نیاز با تقدم و تأخر
/// برای Scoring: شرایط فعال‌سازی
/// </summary>
[DisplayName("شرط پویش")]
[SBVR(SBVRModality.Obligatory, "شرایط پویش", "هر شرط پویش باید برای تعیین زمان و شرایط فعال‌سازی پویش قابل شناسایی باشد")]
public class PromotionCondition : HyperBaseCoreConfigAuditableEntity<int>
{
    public int PromotionId { get; set; }
    
    [DisplayName("پویش")]
    [SBVR(SBVRModality.Obligatory, "رابطه شرط-پویش", "هر شرط باید به یک پویش مشخص تعلق داشته باشد")]
    public Promotion Promotion { get; set; } = null!;

    [DisplayName("عنوان")]
    [InDisplayString]
    [MaxLength(41)]
    [SBVR(SBVRModality.Obligatory, "شناسایی شرط", "عنوان شرط باید برای مدیریت و ردیابی شرط‌ها واضح و قابل فهم باشد")]
    public string Title { get; set; } = null!;

    [DisplayName("نوع شرط")]
    [SBVR(SBVRModality.Obligatory, "نوع شرط", "نوع شرط تعیین می‌کند که شرط بر اساس چه چیزی فعال می‌شود")]
    public PromotionConditionType Type { get; set; }

    // ===== برای Event Conditions =====
    
    /// <summary>
    /// For PromotionConditionType.Event
    /// </summary>
    public int? EventChannelId { get; set; }
    
    [DisplayName("صادرکننده رویداد")]
    public EventChannel? EventChannel { get; set; }

    /// <summary>
    /// For PromotionConditionType.Event
    /// </summary>
    public int? EventTypeId { get; set; }
    
    [DisplayName("رویداد")]
    [SBVR(SBVRModality.Permitted, "شرط رویداد", "رویداد برای تعریف شرایطی که بر اساس دریافت رویداد فعال می‌شود")]
    public EventType? EventType { get; set; }

    /// <summary>
    /// حداقل تعداد رویداد مورد نیاز (برای Campaign)
    /// </summary>
    [DisplayName("حداقل تعداد")]
    [SBVR(SBVRModality.Permitted, "تعداد رویداد", "حداقل تعداد برای Campaign ها - تعیین می‌کند که چند بار این رویداد باید دریافت شود")]
    public int? MinimumCount { get; set; }

    /// <summary>
    /// ترتیب در توالی (برای Campaign - مدیریت تقدم و تأخر)
    /// </summary>
    [DisplayName("ترتیب")]
    [SBVR(SBVRModality.Permitted, "ترتیب شرط", "ترتیب برای Campaign ها - تعیین می‌کند که این شرط در چه مرحله‌ای از توالی قرار دارد")]
    public int? SequenceOrder { get; set; }

    /// <summary>
    /// وابسته به کدام شرط دیگر (برای Campaign - dependency)
    /// </summary>
    [DisplayName("وابسته به شرط")]
    [SBVR(SBVRModality.Permitted, "وابستگی شرط", "وابستگی برای Campaign ها - تعیین می‌کند که این شرط بعد از کدام شرط دیگر باید برقرار شود")]
    public int? DependencyConditionId { get; set; }
    
    [DisplayName("شرط وابسته")]
    public PromotionCondition? DependencyCondition { get; set; }

    /// <summary>
    /// آیا با شرط دیگر موازی است (برای Campaign - parallel execution)
    /// </summary>
    [DisplayName("موازی")]
    [SBVR(SBVRModality.Permitted, "موازی بودن شرط", "موازی بودن برای Campaign ها - تعیین می‌کند که آیا این شرط می‌تواند همزمان با شرط دیگر برقرار شود")]
    public bool IsParallel { get; set; }

    // ===== برای سایر Types =====

    /// <summary>
    /// For PromotionConditionType.UpgradePointLevel
    /// </summary>
    public int? PointLevelId { get; set; }
    
    [DisplayName("سطح‌امتیاز")]
    [SBVR(SBVRModality.Permitted, "شرط سطح امتیاز", "سطح امتیاز برای تعریف شرایطی که بر اساس ارتقاء سطح فعال می‌شود")]
    public PointLevel? PointLevel { get; set; }

    /// <summary>
    /// For PromotionConditionType.PurchaseAward or PromotionConditionType.ConsumeAward
    /// </summary>
    public int? AwardId { get; set; }
    
    [DisplayName("پاداش")]
    [SBVR(SBVRModality.Permitted, "شرط پاداش", "پاداش برای تعریف شرایطی که بر اساس خرید یا استفاده از پاداش فعال می‌شود")]
    public Reward? Award { get; set; }

    /// <summary>
    /// For PromotionConditionType.PurchaseProduct - محصولات 
    /// </summary>
    public int? ProductId { get; set; }
    
    [DisplayName("محصول")]
    [SBVR(SBVRModality.Permitted, "شرط محصول", "محصول سازمان برای تعریف شرایطی که بر اساس خرید محصولات سازمانی فعال می‌شود")]
    public Product? Product { get; set; }

    // ===== شرایط =====

    /// <summary>
    /// شرط های هم گروه با هم And می شوند
    /// </summary>
    [DisplayName("گروه شرط")]
    [SBVR(SBVRModality.Permitted, "گروه‌بندی شرط", "گروه شرط برای ترکیب چند شرط با AND")]
    public ConditionGroup? ConditionGroup { get; set; }

    [DisplayName("نوع شرط")]
    [SBVR(SBVRModality.Obligatory, "نوع ارزیابی شرط", "نوع شرط تعیین می‌کند که چگونه شرط ارزیابی می‌شود")]
    public PromotionConditionKind Kind { get; set; }

    [DisplayName("فرمول شرط")]
    [MaxLength(512)]
    [SBVR(SBVRModality.Permitted, "فرمول شرط", "فرمول شرط برای ارزیابی شرط‌های پیچیده")]
    public string? Constraint { get; set; }

    [DisplayName("مقایسه شود با")]
    [SBVR(SBVRModality.Permitted, "مقایسه شرط", "مقایسه شرط تعیین می‌کند که شرط با چه چیزی مقایسه می‌شود")]
    public PromotionConditionCompareWith? CompareWith { get; set; }

    public int? PointId { get; set; }
    
    [DisplayName("امتیاز")]
    [SBVR(SBVRModality.Permitted, "امتیاز شرط", "امتیاز برای مقایسه در شرط")]
    public Point? Point { get; set; }

    public int? EventTypeParameterId { get; set; }
    
    [DisplayName("پارامتر رویداد")]
    [SBVR(SBVRModality.Permitted, "پارامتر رویداد", "پارامتر رویداد برای مقایسه در شرط")]
    public EventTypeParameter? EventTypeParameter { get; set; }

    [DisplayName("مقدار")]
    [MaxLength(512)]
    [SBVR(SBVRModality.Permitted, "مقدار شرط", "مقدار برای مقایسه در شرط")]
    public string? Value { get; set; }
}

