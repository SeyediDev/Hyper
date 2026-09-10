using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Surveys.Queries;

public record GetSurveysQuery : IRequest<GetSurveysQueryResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool? IsActive { get; set; }
}

public record GetSurveysQueryResponse
{
    public PaginatedList<SurveyDto> Surveys { get; set; } = null!;
}

public record SurveyDto
{
    public string Id { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsCompleted { get; set; }
    public List<RewardCostDto>? Reward { get; set; }
    public List<SurveyQuestionDto> Questions { get; set; } = [];
}

public record SurveyQuestionDto
{
    public string Id { get; set; } = null!;
    public string QuestionText { get; set; } = null!;
    public string QuestionType { get; set; } = null!;
    public bool IsRequired { get; set; }
    public List<SurveyQuestionOptionDto>? Options { get; set; }
    public int Order { get; set; }
}

public record SurveyQuestionOptionDto
{
    public string Id { get; set; } = null!;
    public string OptionText { get; set; } = null!;
    public int Order { get; set; }
}

public class GetSurveysQueryHandler(ISurveyService surveyService) : IRequestHandler<GetSurveysQuery, GetSurveysQueryResponse>
{
    public async Task<GetSurveysQueryResponse> Handle(GetSurveysQuery request, CancellationToken cancellationToken)
    {
        var result = await surveyService.GetActiveSurveysAsync(request.PageNumber, request.PageSize, cancellationToken);
        
        var surveys = result.Items.Select(s => new SurveyDto
        {
            Id = s.Id.ToString(),
            Title = s.Title,
            Description = s.Description ?? string.Empty,
            StartDate = s.StartDate,
            EndDate = s.EndDate,
            IsActive = s.IsActive,
            IsCompleted = s.HasResponded,
            Reward = null,
            Questions = []
        }).ToList();
        
        return new GetSurveysQueryResponse
        {
            Surveys = new PaginatedList<SurveyDto>(surveys, result.TotalCount, request.PageNumber, request.PageSize)
        };
    }
}

