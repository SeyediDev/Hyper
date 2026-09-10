namespace Hyper.Domain.Entities.Feedback;

/// <summary>
/// نظر روی بازخورد - برای گفتگو و بحث درباره بازخورد
/// </summary>
[DisplayName("نظر بازخورد")]
[SBVR(SBVRModality.Permitted, "تعامل", "مشتریان و کارکنان می‌توانند درباره بازخورد نظر بدهند")]
public class FeedbackComment : HyperBaseCoreAuditableEntity<int>
{
    /// <summary>
    /// شناسه بازخورد
    /// </summary>
    [DisplayName("شناسه بازخورد")]
    [SBVR(SBVRModality.Obligatory, "ارتباط", "هر نظر باید به یک بازخورد تعلق داشته باشد")]
    public int FeedbackId { get; set; }

    /// <summary>
    /// بازخورد
    /// </summary>
    [DisplayName("بازخورد")]
    public CustomerFeedback Feedback { get; set; } = null!;

    /// <summary>
    /// شناسه مشتری (اگر مشتری نظر داده)
    /// </summary>
    [DisplayName("شناسه مشتری")]
    public int? CustomerId { get; set; }

    /// <summary>
    /// مشتری
    /// </summary>
    [DisplayName("مشتری")]
    public Customer? Customer { get; set; }

    /// <summary>
    /// شناسه کاربر (اگر کارمند نظر داده)
    /// </summary>
    [DisplayName("شناسه کاربر")]
    public UserId? UserId { get; set; }

    /// <summary>
    /// کاربر
    /// </summary>
    [DisplayName("کاربر")]
    public User? User { get; set; }

    /// <summary>
    /// متن نظر
    /// </summary>
    [DisplayName("متن نظر")]
    [MaxLength(2000)]
    [SBVR(SBVRModality.Obligatory, "محتوا", "متن نظر الزامی است")]
    public string Content { get; set; } = null!;

    /// <summary>
    /// شناسه نظر والد (برای پاسخ به نظر)
    /// </summary>
    [DisplayName("شناسه نظر والد")]
    public int? ParentCommentId { get; set; }

    /// <summary>
    /// نظر والد
    /// </summary>
    [DisplayName("نظر والد")]
    public FeedbackComment? ParentComment { get; set; }

    /// <summary>
    /// پاسخ‌ها
    /// </summary>
    [DisplayName("پاسخ‌ها")]
    public ICollection<FeedbackComment> Replies { get; set; } = [];

	/// <summary>
	/// آیا نظر رسمی است (از طرف اکوسیستم)
	/// </summary>
	[DisplayName("نظر رسمی")]
    public bool IsOfficial { get; set; } = false;
}



