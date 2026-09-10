namespace Hyper.Domain.Entities.Promotions;

/// <summary>
/// شرکت مشتری در پویش - ردیابی وضعیت شرکت هر مشتری در یک پویش
/// </summary>
[DisplayName("شرکت مشتری در پویش")]
[SBVR(SBVRModality.Obligatory, "ردیابی شرکت", "هر شرکت مشتری در پویش باید برای مدیریت وضعیت و محدودیت‌های شرکت قابل شناسایی باشد")]
public class PromotionParticipation : HyperBaseCoreConfigAuditableEntity<int>
{
    public int PromotionId { get; set; }
    
    [DisplayName("پویش")]
    [SBVR(SBVRModality.Obligatory, "رابطه شرکت-پویش", "هر شرکت باید به یک پویش مشخص تعلق داشته باشد")]
    public Promotion Promotion { get; set; } = null!;
    
    public int CustomerTenantId { get; set; }
    
    [DisplayName("مشتری")]
    [SBVR(SBVRModality.Obligatory, "رابطه شرکت-مشتری", "هر شرکت باید به یک مشتری مشخص تعلق داشته باشد")]
    public CustomerTenant CustomerTenant { get; set; } = null!;
    
    /// <summary>
    /// تاریخ شروع شرکت در پویش
    /// </summary>
    [DisplayName("تاریخ شروع")]
    [SBVR(SBVRModality.Permitted, "زمان شروع شرکت", "تاریخ شروع برای محاسبه بازه زمانی شرکت")]
    public DateTime StartDate { get; set; }
    
    /// <summary>
    /// تاریخ پایان شرکت در پویش (در صورت تکمیل یا انصراف)
    /// </summary>
    [DisplayName("تاریخ پایان")]
    [SBVR(SBVRModality.Permitted, "زمان پایان شرکت", "تاریخ پایان برای محاسبه مدت زمان شرکت")]
    public DateTime? EndDate { get; set; }
    
    /// <summary>
    /// وضعیت شرکت (در حال انجام، تکمیل شده، لغو شده)
    /// </summary>
    [DisplayName("وضعیت")]
    [SBVR(SBVRModality.Obligatory, "وضعیت شرکت", "وضعیت شرکت برای مدیریت و گزارش‌گیری")]
    public PromotionParticipationStatus Status { get; set; }
    
    /// <summary>
    /// تعداد دفعات شرکت در این پویش (در بازه زمانی پویش)
    /// </summary>
    [DisplayName("تعداد دفعات شرکت")]
    [SBVR(SBVRModality.Permitted, "تعداد شرکت", "تعداد دفعات شرکت برای مدیریت محدودیت‌های شرکت")]
    public int ParticipationCount { get; set; }
    
    /// <summary>
    /// آیا پویش تکمیل شده است (همه شرط‌ها برقرار شده‌اند)
    /// </summary>
    [DisplayName("تکمیل شده")]
    [SBVR(SBVRModality.Permitted, "تکمیل پویش", "تکمیل پویش برای تعیین اینکه آیا همه شرط‌ها برقرار شده‌اند")]
    public bool IsCompleted { get; set; }
    
    /// <summary>
    /// تاریخ تکمیل پویش
    /// </summary>
    [DisplayName("تاریخ تکمیل")]
    [SBVR(SBVRModality.Permitted, "زمان تکمیل", "تاریخ تکمیل برای گزارش‌گیری و تحلیل")]
    public DateTime? CompletedDate { get; set; }
    
    /// <summary>
    /// رویدادهای دریافتی این مشتری در این پویش
    /// </summary>
    [DisplayName("رویدادهای دریافتی")]
    [SBVR(SBVRModality.Permitted, "رویدادهای شرکت", "رویدادهای دریافتی برای ردیابی پیشرفت در پویش")]
    public ICollection<PromotionEventReceived> EventsReceived { get; set; } = [];
}