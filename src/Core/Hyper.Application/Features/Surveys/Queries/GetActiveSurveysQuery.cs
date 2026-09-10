namespace Hyper.Application.Features.Surveys.Queries;

/// <summary>
/// دریافت نظرسنجی‌های فعال (برای نمایش به مشتریان)
/// </summary>
public record GetActiveSurveysQuery : IRequest<List<ActiveSurveyDto>>
{
    [Required]
    public int TenantId { get; set; }

    public int? CustomerTenantId { get; set; }

    public SurveyType? SurveyType { get; set; }
}

public record ActiveSurveyDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public SurveyType SurveyType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int TotalParticipants { get; set; }
    public long? ParticipationPoints { get; set; }
    public long? CorrectAnswerPoints { get; set; }
    public bool HasParticipated { get; set; }
    public int ItemsCount { get; set; }
}

public class GetActiveSurveysQueryValidator : AbstractValidator<GetActiveSurveysQuery>
{
    public GetActiveSurveysQueryValidator(IMultiLingualService multiLingual)
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage(multiLingual.GetMessage("TenantIdIsRequired"));
    }
}

public class GetActiveSurveysQueryHandler(
    IHyperUnitOfWorkQuery unitOfWork
) : IRequestHandler<GetActiveSurveysQuery, List<ActiveSurveyDto>>
{
    public async Task<List<ActiveSurveyDto>> Handle(GetActiveSurveysQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var surveyRepo = unitOfWork.Repository<Survey, int>();
        var surveys = await surveyRepo.GetAllWithIncludeAsync(
            include: s => s.Items,
            cancellationToken: cancellationToken,
            predicate: s => s.Promotion.TenantId == request.TenantId &&
                     s.IsActive &&
                     (!s.StartDate.HasValue || s.StartDate.Value <= now) &&
                     (!s.EndDate.HasValue || s.EndDate.Value >= now) &&
                     (!request.SurveyType.HasValue || s.SurveyType == request.SurveyType));

        // دریافت لیست نظرسنجی‌هایی که مشتری در آنها شرکت کرده
        HashSet<int> participatedSurveyIds = [];
        if (request.CustomerTenantId.HasValue)
        {
            var participationRepo = unitOfWork.Repository<SurveyParticipation, int>();
            var participations = await participationRepo.Query()
                .Where(p => p.CustomerTenantId == request.CustomerTenantId)
                .ToListAsync(cancellationToken);

            participatedSurveyIds = participations.Select(p => p.SurveyId).ToHashSet();
        }

        return surveys.Select(s => new ActiveSurveyDto
        {
            Id = s.Id,
            Title = s.Title,
            Description = s.Description,
            SurveyType = s.SurveyType,
            StartDate = s.StartDate,
            EndDate = s.EndDate,
            TotalParticipants = s.TotalParticipants,
            ParticipationPoints = s.ParticipationPoints,
            CorrectAnswerPoints = s.CorrectAnswerPoints,
            HasParticipated = participatedSurveyIds.Contains(s.Id),
            ItemsCount = s.Items.Count
        }).ToList();
    }
}
