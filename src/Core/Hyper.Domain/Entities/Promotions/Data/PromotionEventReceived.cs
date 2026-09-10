namespace Hyper.Domain.Entities.Promotions.Data;

/// <summary>
/// رویداد دریافتی در پویش - ردیابی رویدادهای دریافتی هر مشتری برای هر محرک عملیات پویش
/// </summary>
[DisplayName("رویداد دریافتی در پویش")]
[SBVR(SBVRModality.Obligatory, "ردیابی رویداد", "هر رویداد دریافتی باید برای مدیریت تعداد و توالی رویدادها قابل شناسایی باشد")]
public class PromotionEventReceived : HyperBaseCoreConfigAuditableEntity<int>
{
    public int PromotionParticipationId { get; set; }
    
    [DisplayName("شرکت در پویش")]
    [SBVR(SBVRModality.Obligatory, "رابطه رویداد-شرکت", "هر رویداد دریافتی باید به یک شرکت مشخص تعلق داشته باشد")]
    public PromotionParticipation PromotionParticipation { get; set; } = null!;
    
    [OldDbMap("PromotionConditionId")]
    public int PromotionTriggerId { get; set; }
    
    [DisplayName("محرک عملیات پویش")]
    [SBVR(SBVRModality.Obligatory, "رابطه رویداد-محرک", "هر رویداد دریافتی باید به یک محرک عملیات پویش مشخص تعلق داشته باشد")]
    [OldDbMap("PromotionCondition")]
    public PromotionTrigger PromotionTrigger { get; set; } = null!;
    
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

    [DisplayName("نوع جریان رویداد")]
    [SBVR(SBVRModality.Permitted, "نوع جریان رویداد", "به ازای نوع جریان رویداد موازی و بعد از یک رویداد، باید یک رویداد انتخاب شود")]
    public PromotionTriggerFlowType? FlowType { get; set; }
    
    /// <summary>
    /// شناسه گروه موازی (برای گروه‌بندی رویدادهای موازی)
    /// </summary>
    [DisplayName("شناسه گروه موازی")]
    [SBVR(SBVRModality.Permitted, "گروه موازی", "شناسه گروه موازی برای گروه‌بندی رویدادهای موازی")]
    public int? ParallelGroupId { get; set; }
}