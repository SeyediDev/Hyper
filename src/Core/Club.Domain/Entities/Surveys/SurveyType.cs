namespace Hyper.Domain.Entities.Surveys;

/// <summary>
/// نوع نظرسنجی
/// </summary>
public enum SurveyType
{
    /// <summary>
    /// نظرسنجی عادی - مشتری می‌تواند چند گزینه را انتخاب کند
    /// </summary>
    [Display(Name = "نظرسنجی عادی")]
    RegularSurvey = 1,

    /// <summary>
    /// مسابقه - مشتری فقط یک گزینه را انتخاب می‌کند و جواب درست وجود دارد
    /// </summary>
    [Display(Name = "مسابقه")]
    Contest = 2
}