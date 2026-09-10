using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Services;

public class MockSurveyService : ISurveyService
{
    public Task<PaginatedList<SurveyDto>> GetActiveSurveysAsync(int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var surveys = new List<SurveyDto>
        {
            new() { Id = 1, Title = "نظرسنجی رضایت مشتری", Description = "نظر شما برای ما مهم است", StartDate = DateTime.UtcNow.AddDays(-5), EndDate = DateTime.UtcNow.AddDays(25), RewardPoints = 500, IsActive = true, HasResponded = false }
        };

        return Task.FromResult(new PaginatedList<SurveyDto>(surveys, 1, pageNumber, pageSize));
    }

    public Task<SurveyDto?> GetSurveyByIdAsync(int surveyId, CancellationToken cancellationToken = default)
    {
        var survey = new SurveyDto
        {
            Id = surveyId,
            Title = "نظرسنجی رضایت مشتری",
            Description = "نظر شما برای ما مهم است",
            StartDate = DateTime.UtcNow.AddDays(-5),
            EndDate = DateTime.UtcNow.AddDays(25),
            RewardPoints = 500,
            IsActive = true,
            HasResponded = false
        };

        return Task.FromResult<SurveyDto?>(survey);
    }

    public Task<SurveySubmissionResultDto> SubmitSurveyAsync(int customerId, int surveyId, Dictionary<int, string> answers, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new SurveySubmissionResultDto
        {
            Success = true,
            Message = "پاسخ شما ثبت شد",
            PointsEarned = 500
        });
    }

    public Task<PaginatedList<MySurveyResponseDto>> GetMyResponsesAsync(int customerId, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var responses = new List<MySurveyResponseDto>();
        return Task.FromResult(new PaginatedList<MySurveyResponseDto>(responses, 0, pageNumber, pageSize));
    }
}

