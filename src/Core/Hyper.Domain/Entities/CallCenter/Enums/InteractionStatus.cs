namespace Hyper.Domain.Entities.CallCenter.Enums;

/// <summary>
/// وضعیت تعامل
/// </summary>
public enum InteractionStatus
{
    /// <summary>
    /// در حال انجام
    /// </summary>
    [Display(Name = "در حال انجام")]
    InProgress = 1,

    /// <summary>
    /// تکمیل شده
    /// </summary>
    [Display(Name = "تکمیل شده")]
    Completed = 2,

    /// <summary>
    /// لغو شده
    /// </summary>
    [Display(Name = "لغو شده")]
    Cancelled = 3,

    /// <summary>
    /// در انتظار پیگیری
    /// </summary>
    [Display(Name = "در انتظار پیگیری")]
    PendingFollowUp = 4,

    /// <summary>
    /// بسته شده
    /// </summary>
    [Display(Name = "بسته شده")]
    Closed = 5
}