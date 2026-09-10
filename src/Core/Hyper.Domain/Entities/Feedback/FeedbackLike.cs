namespace Hyper.Domain.Entities.Feedback;

/// <summary>
/// لایک بازخورد - برای نمایش محبوبیت بازخورد
/// </summary>
[DisplayName("لایک بازخورد")]
[SBVR(SBVRModality.Permitted, "تعامل", "مشتریان می‌توانند بازخوردهای دیگران را لایک کنند")]
public class FeedbackLike : HyperBaseCoreAuditableEntity<int>
{
    /// <summary>
    /// شناسه بازخورد
    /// </summary>
    [DisplayName("شناسه بازخورد")]
    [SBVR(SBVRModality.Obligatory, "ارتباط", "هر لایک باید به یک بازخورد تعلق داشته باشد")]
    public int FeedbackId { get; set; }

    /// <summary>
    /// بازخورد
    /// </summary>
    [DisplayName("بازخورد")]
    public CustomerFeedback Feedback { get; set; } = null!;

    /// <summary>
    /// شناسه مشتری
    /// </summary>
    [DisplayName("شناسه مشتری")]
    [SBVR(SBVRModality.Obligatory, "شناسایی", "هر لایک باید به یک مشتری تعلق داشته باشد")]
    public int CustomerTenantId { get; set; }

    /// <summary>
    /// مشتری
    /// </summary>
    [DisplayName("مشتری")]
    public CustomerTenant CustomerTenant { get; set; } = null!;

    /// <summary>
    /// تاریخ لایک
    /// </summary>
    [DisplayName("تاریخ")]
    public DateTime LikedDate { get; set; } = DateTime.UtcNow;
}



