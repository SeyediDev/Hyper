namespace Hyper.Domain.Entities.CallCenter;

/// <summary>
/// پیگیری تعامل - ثبت پیگیری‌های بعدی برای تعاملات
/// </summary>
[DisplayName("پیگیری تعامل")]
[SBVR(SBVRModality.Recommended, "مدیریت پیگیری", "پیگیری برای اطمینان از حل شدن مسائل مشتری و بهبود خدمات استفاده می‌شود")]
public class InteractionFollowUp : HyperBaseCoreAuditableEntity<long>
{
    /// <summary>
    /// شناسه تعامل اصلی
    /// </summary>
    [DisplayName("تعامل")]
    [SBVR(SBVRModality.Obligatory, "ارتباط", "هر پیگیری باید به یک تعامل مشخص مرتبط باشد")]
    public long CustomerInteractionId { get; set; }

    [DisplayName("تعامل")]
    public CustomerInteraction CustomerInteraction { get; set; } = null!;

    /// <summary>
    /// شناسه کاربر کال سنتر (پشتیبان)
    /// </summary>
    [DisplayName("پشتیبان")]
    [SBVR(SBVRModality.Obligatory, "مسئولیت", "هر پیگیری باید به یک پشتیبان مشخص اختصاص داده شود")]
    public UserId? AgentUserId { get; set; }

    [DisplayName("پشتیبان")]
    public User? AgentUser { get; set; }

    /// <summary>
    /// تاریخ و زمان پیگیری
    /// </summary>
    [DisplayName("تاریخ پیگیری")]
    [SBVR(SBVRModality.Obligatory, "زمان‌بندی", "تاریخ پیگیری برای برنامه‌ریزی و ردیابی استفاده می‌شود")]
    public DateTime FollowUpDate { get; set; }

    /// <summary>
    /// توضیحات پیگیری
    /// </summary>
    [DisplayName("توضیحات")]
    [Column(TypeName = "nvarchar(max)")]
    [SBVR(SBVRModality.Obligatory, "مستندسازی", "توضیحات برای ثبت جزئیات پیگیری ضروری است")]
    public string? Notes { get; set; }

    /// <summary>
    /// نتیجه پیگیری
    /// </summary>
    [DisplayName("نتیجه")]
    [MaxLength(500)]
    [SBVR(SBVRModality.Recommended, "مستندسازی", "نتیجه برای ثبت نتیجه پیگیری استفاده می‌شود")]
    public string? Result { get; set; }

    /// <summary>
    /// آیا پیگیری تکمیل شده است
    /// </summary>
    [DisplayName("تکمیل شده")]
    [SBVR(SBVRModality.Recommended, "مدیریت وضعیت", "وضعیت تکمیل برای مدیریت پیگیری‌ها استفاده می‌شود")]
    public bool IsCompleted { get; set; }

    /// <summary>
    /// تاریخ تکمیل
    /// </summary>
    [DisplayName("تاریخ تکمیل")]
    [SBVR(SBVRModality.Recommended, "زمان‌بندی", "تاریخ تکمیل برای تحلیل زمان پیگیری استفاده می‌شود")]
    public DateTime? CompletedDate { get; set; }
}