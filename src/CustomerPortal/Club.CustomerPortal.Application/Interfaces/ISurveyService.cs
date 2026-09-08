namespace Hyper.CustomerPortal.Application.Interfaces;

/// <summary>
/// سرویس مدیریت نظرسنجی‌ها
/// </summary>
public interface ISurveyService
{
    /// <summary>
    /// دریافت لیست نظرسنجی‌های فعال
    /// </summary>
    Task<PaginatedList<SurveyDto>> GetActiveSurveysAsync(
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// دریافت جزئیات نظرسنجی
    /// </summary>
    Task<SurveyDto?> GetSurveyByIdAsync(int surveyId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// ثبت پاسخ نظرسنجی
    /// </summary>
    Task<SurveySubmissionResultDto> SubmitSurveyAsync(
        int customerId,
        int surveyId,
        Dictionary<int, string> answers,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// دریافت نظرسنجی‌هایی که مشتری پاسخ داده
    /// </summary>
    Task<PaginatedList<MySurveyResponseDto>> GetMyResponsesAsync(
        int customerId,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
}

public record SurveyDto
{
    public int Id { get; init; }
    public string Title { get; init; } = null!;
    public string? Description { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public long RewardPoints { get; init; }
    public bool IsActive { get; init; }
    public bool HasResponded { get; init; }
}

public record SurveySubmissionResultDto
{
    public bool Success { get; init; }
    public string Message { get; init; } = null!;
    public long PointsEarned { get; init; }
}

public record MySurveyResponseDto
{
    public int SurveyId { get; init; }
    public string SurveyTitle { get; init; } = null!;
    public DateTime RespondedAt { get; init; }
    public long PointsEarned { get; init; }
}

