namespace Hyper.Domain.Entities.Feedback;

/// <summary>
/// نوع بازخورد مشتری
/// </summary>
public enum FeedbackType
{
    /// <summary>
    /// پیشنهاد - مشتری پیشنهادی برای بهبود دارد
    /// </summary>
    [Display(Name = "پیشنهاد")]
    Suggestion = 1,

    /// <summary>
    /// انتقاد - مشتری انتقادی دارد
    /// </summary>
    [Display(Name = "انتقاد")]
    Criticism = 2,

    /// <summary>
    /// شکایت - مشتری از خدمات/محصول ناراضی است
    /// </summary>
    [Display(Name = "شکایت")]
    Complaint = 3,

    /// <summary>
    /// تشکر و قدردانی - مشتری از خدمات راضی است
    /// </summary>
    [Display(Name = "تشکر")]
    Appreciation = 4,

    /// <summary>
    /// سوال - مشتری سوالی دارد
    /// </summary>
    [Display(Name = "سوال")]
    Question = 5,

    /// <summary>
    /// درخواست ویژگی - مشتری ویژگی جدیدی می‌خواهد
    /// </summary>
    [Display(Name = "درخواست ویژگی")]
    FeatureRequest = 6
}

/// <summary>
/// وضعیت بازخورد
/// </summary>
public enum FeedbackStatus
{
    /// <summary>
    /// جدید - هنوز بررسی نشده
    /// </summary>
    [Display(Name = "جدید")]
    New = 1,

    /// <summary>
    /// در حال بررسی
    /// </summary>
    [Display(Name = "در حال بررسی")]
    UnderReview = 2,

    /// <summary>
    /// در حال اقدام
    /// </summary>
    [Display(Name = "در حال اقدام")]
    InProgress = 3,

    /// <summary>
    /// حل شده
    /// </summary>
    [Display(Name = "حل شده")]
    Resolved = 4,

    /// <summary>
    /// رد شده
    /// </summary>
    [Display(Name = "رد شده")]
    Rejected = 5,

    /// <summary>
    /// بسته شده
    /// </summary>
    [Display(Name = "بسته")]
    Closed = 6
}

/// <summary>
/// اولویت بازخورد
/// </summary>
public enum FeedbackPriority
{
    /// <summary>
    /// کم
    /// </summary>
    [Display(Name = "کم")]
    Low = 1,

    /// <summary>
    /// متوسط
    /// </summary>
    [Display(Name = "متوسط")]
    Medium = 2,

    /// <summary>
    /// بالا
    /// </summary>
    [Display(Name = "بالا")]
    High = 3,

    /// <summary>
    /// فوری
    /// </summary>
    [Display(Name = "فوری")]
    Critical = 4
}



