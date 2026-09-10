namespace Hyper.Domain.Entities.Promotions.Surveys.Enums;

/// <summary>
/// نوع نظرسنجی
/// </summary>
public enum SurveyType
{
    /// <summary>
    /// نظرسنجی عادی - مشتری می‌تواند چند گزینه را انتخاب کند
    /// </summary>
    [Display(Name = "نظرسنجی")]
    Survey = 1,

    /// <summary>
    /// مسابقه - مشتری می تواند چند گزینه را انتخاب می‌کند و جواب درست وجود دارد
    /// </summary>
    [Display(Name = "مسابقه")]
    Contest = 2
}