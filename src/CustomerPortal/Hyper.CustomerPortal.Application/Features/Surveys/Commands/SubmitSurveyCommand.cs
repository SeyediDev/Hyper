using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Surveys.Commands;

public record SubmitSurveyCommand : IRequest
{
    public string SurveyId { get; set; } = null!;
    public List<QuestionResponseDto> Responses { get; set; } = [];
}

public record QuestionResponseDto
{
    public string QuestionId { get; set; } = null!;
    public List<string>? SelectedOptionIds { get; set; }
    public string? TextAnswer { get; set; }
    public int? RatingValue { get; set; }
}

public class SubmitSurveyCommandValidator : AbstractValidator<SubmitSurveyCommand>
{
    public SubmitSurveyCommandValidator()
    {
        RuleFor(x => x.SurveyId).NotEmpty().WithMessage("شناسه نظرسنجی الزامی است");
        RuleFor(x => x.Responses).NotEmpty().WithMessage("پاسخ‌ها الزامی است");
    }
}

public class SubmitSurveyCommandHandler(
    ISurveyService surveyService,
    ICustomerRequesterUser requesterUser,
    ILogger<SubmitSurveyCommandHandler> logger) : IRequestHandler<SubmitSurveyCommand>
{
    public async Task Handle(SubmitSurveyCommand request, CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;
        var answers = new Dictionary<int, string>();
        
        foreach (var response in request.Responses)
        {
            var answerId = int.Parse(response.QuestionId);
            var answerValue = response.TextAnswer ?? string.Join(",", response.SelectedOptionIds ?? []);
            answers[answerId] = answerValue;
        }
        
        var result = await surveyService.SubmitSurveyAsync(customerId, int.Parse(request.SurveyId), answers, cancellationToken);
        
        if (!result.Success)
        {
            throw new InvalidOperationException(result.Message);
        }
        
        logger.LogInformation("Customer {CustomerId} submitted survey {SurveyId}", customerId, request.SurveyId);
    }
}

