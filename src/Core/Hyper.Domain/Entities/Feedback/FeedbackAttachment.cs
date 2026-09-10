namespace Hyper.Domain.Entities.Feedback;

/// <summary>
/// پیوست بازخورد - تصاویر و فایل‌های مرتبط با بازخورد
/// </summary>
[DisplayName("پیوست بازخورد")]
[SBVR(SBVRModality.Permitted, "مستندسازی", "مشتری می‌تواند تصویر یا فایل برای بازخورد ارسال کند")]
public class FeedbackAttachment : HyperBaseCoreAuditableEntity<int>
{
    /// <summary>
    /// شناسه بازخورد
    /// </summary>
    [DisplayName("شناسه بازخورد")]
    [SBVR(SBVRModality.Obligatory, "ارتباط", "هر پیوست باید به یک بازخورد تعلق داشته باشد")]
    public int FeedbackId { get; set; }

    /// <summary>
    /// بازخورد
    /// </summary>
    [DisplayName("بازخورد")]
    public CustomerFeedback Feedback { get; set; } = null!;

    /// <summary>
    /// شناسه سند
    /// </summary>
    [DisplayName("شناسه سند")]
    [SBVR(SBVRModality.Obligatory, "فایل", "هر پیوست باید یک فایل داشته باشد")]
    public int DocumentId { get; set; }

    /// <summary>
    /// سند
    /// </summary>
    [DisplayName("سند")]
    public Document Document { get; set; } = null!;

    /// <summary>
    /// توضیحات پیوست
    /// </summary>
    [DisplayName("توضیحات")]
    [MaxLength(500)]
    public string? Description { get; set; }
}



