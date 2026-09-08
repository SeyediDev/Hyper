namespace Hyper.Domain.Entities.Promotions.Surveys;

/// <summary>
/// پاسخ به آیتم نظرسنجی
/// </summary>
[DisplayName("پاسخ آیتم نظرسنجی")]
public class SurveyItemResponse : HyperBaseCoreAuditableEntity<int>
{
    /// <summary>
    /// شناسه شرکت در نظرسنجی
    /// </summary>
    [DisplayName("شناسه شرکت")]
    public int ParticipationId { get; set; }

    /// <summary>
    /// شرکت در نظرسنجی
    /// </summary>
    [DisplayName("شرکت")]
    public SurveyParticipation Participation { get; set; } = null!;

    /// <summary>
    /// شناسه آیتم نظرسنجی
    /// </summary>
    [DisplayName("شناسه آیتم")]
    public int SurveyItemId { get; set; }

    /// <summary>
    /// آیتم نظرسنجی
    /// </summary>
    [DisplayName("آیتم نظرسنجی")]
    public SurveyItem SurveyItem { get; set; } = null!;

    /// <summary>
    /// پاسخ متنی
    /// </summary>
    [DisplayName("پاسخ")]
    [MaxLength(500)]
    public string? ResponseText { get; set; }

    /// <summary>
    /// پاسخ عددی
    /// </summary>
    [DisplayName("مقدار عددی")]
    public int? ResponseValue { get; set; }
}



