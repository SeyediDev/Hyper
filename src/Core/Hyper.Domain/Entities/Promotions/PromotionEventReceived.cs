namespace Hyper.Domain.Entities.Promotions;

/// <summary>
/// رویداد دریافتی در پویش - ردیابی رویدادهای دریافتی هر مشتری برای هر شرط پویش
/// </summary>
[DisplayName("رویداد دریافتی در پویش")]
[SBVR(SBVRModality.Obligatory, "ردیابی رویداد", "هر رویداد دریافتی باید برای مدیریت تعداد و توالی رویدادها قابل شناسایی باشد")]
public class PromotionEventReceived : HyperBaseCoreConfigAuditableEntity<int>
{
    public int PromotionParticipationId { get; set; }
    
    [DisplayName("شرکت در پویش")]
    [SBVR(SBVRModality.Obligatory, "رابطه رویداد-شرکت", "هر رویداد دریافتی باید به یک شرکت مشخص تعلق داشته باشد")]
    public PromotionParticipation PromotionParticipation { get; set; } = null!;
    
    public int PromotionConditionId { get; set; }
    
    [DisplayName("شرط پویش")]
    [SBVR(SBVRModality.Obligatory, "رابطه رویداد-شرط", "هر رویداد دریافتی باید به یک شرط پویش مشخص تعلق داشته باشد")]
    public PromotionCondition PromotionCondition { get; set; } = null!;
    
    public long EventLogId { get; set; }
    
    [DisplayName("رویداد")]
    [SBVR(SBVRModality.Obligatory, "رابطه رویداد-لاگ", "هر رویداد دریافتی باید به یک لاگ رویداد مشخص تعلق داشته باشد")]
    public EventLog EventLog { get; set; } = null!;
    
    /// <summary>
    /// تاریخ و زمان دریافت رویداد
    /// </summary>
    [DisplayName("تاریخ دریافت")]
    [SBVR(SBVRModality.Obligatory, "زمان دریافت", "تاریخ دریافت برای مدیریت توالی و بازه زمانی")]
    public DateTime ReceivedDate { get; set; }
    
    /// <summary>
    /// شماره ترتیب در توالی (برای مدیریت تقدم و تأخر)
    /// </summary>
    [DisplayName("شماره ترتیب")]
    [SBVR(SBVRModality.Permitted, "ترتیب دریافت", "شماره ترتیب برای مدیریت توالی رویدادها")]
    public int? SequenceNumber { get; set; }
    
    /// <summary>
    /// آیا این رویداد بخشی از یک گروه موازی است
    /// </summary>
    [DisplayName("موازی")]
    [SBVR(SBVRModality.Permitted, "موازی بودن", "موازی بودن برای تعیین اینکه آیا این رویداد همزمان با رویدادهای دیگر دریافت شده است")]
    public bool IsParallel { get; set; }
    
    /// <summary>
    /// شناسه گروه موازی (برای گروه‌بندی رویدادهای موازی)
    /// </summary>
    [DisplayName("شناسه گروه موازی")]
    [SBVR(SBVRModality.Permitted, "گروه موازی", "شناسه گروه موازی برای گروه‌بندی رویدادهای موازی")]
    public int? ParallelGroupId { get; set; }
}

