using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Surveys.Queries;

public record GetMySurveyResponsesQuery : IRequest<GetMySurveyResponsesQueryResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public record GetMySurveyResponsesQueryResponse
{
    public PaginatedList<SurveyResponseDto> Responses { get; set; } = null!;
}

public record SurveyResponseDto
{
    public SurveyDto Survey { get; set; } = null!;
    public DateTime SubmittedDate { get; set; }
    public bool IsCompleted { get; set; }
}

public class GetMySurveyResponsesQueryHandler(
    ISurveyService surveyService,
    ICustomerRequesterUser requesterUser)
    : IRequestHandler<GetMySurveyResponsesQuery, GetMySurveyResponsesQueryResponse>
{
    public async Task<GetMySurveyResponsesQueryResponse> Handle(GetMySurveyResponsesQuery request, CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;
        var result = await surveyService.GetMyResponsesAsync(customerId, request.PageNumber, request.PageSize, cancellationToken);
        
        var responses = result.Items.Select(r => new SurveyResponseDto
        {
            Survey = new SurveyDto
            {
                Id = r.SurveyId.ToString(),
                Title = r.SurveyTitle,
                Description = string.Empty,
                StartDate = DateTime.UtcNow.AddDays(-30),
                EndDate = DateTime.UtcNow,
                IsActive = false,
                IsCompleted = true,
                Reward = null,
                Questions = []
            },
            SubmittedDate = r.RespondedAt,
            IsCompleted = true
        }).ToList();
        
        return new GetMySurveyResponsesQueryResponse
        {
            Responses = new PaginatedList<SurveyResponseDto>(responses, result.TotalCount, request.PageNumber, request.PageSize)
        };
    }
}

