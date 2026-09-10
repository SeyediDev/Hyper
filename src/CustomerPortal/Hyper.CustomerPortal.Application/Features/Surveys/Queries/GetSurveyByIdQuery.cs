using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Surveys.Queries;

public record GetSurveyByIdQuery : IRequest<GetSurveyByIdQueryResponse>
{
    public string Id { get; set; } = null!;
}

public record GetSurveyByIdQueryResponse
{
    public SurveyDto Survey { get; set; } = null!;
}

public class GetSurveyByIdQueryHandler(ISurveyService surveyService) : IRequestHandler<GetSurveyByIdQuery, GetSurveyByIdQueryResponse>
{
    public async Task<GetSurveyByIdQueryResponse> Handle(GetSurveyByIdQuery request, CancellationToken cancellationToken)
    {
        var survey = await surveyService.GetSurveyByIdAsync(int.Parse(request.Id), cancellationToken);
        
        if (survey == null)
        {
            throw new InvalidOperationException("نظرسنجی یافت نشد");
        }
        
        return new GetSurveyByIdQueryResponse
        {
            Survey = new SurveyDto
            {
                Id = survey.Id.ToString(),
                Title = survey.Title,
                Description = survey.Description ?? string.Empty,
                StartDate = survey.StartDate,
                EndDate = survey.EndDate,
                IsActive = survey.IsActive,
                IsCompleted = survey.HasResponded,
                Reward = null,
                Questions = []
            }
        };
    }
}

