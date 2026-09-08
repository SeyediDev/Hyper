using System.Linq.Expressions;

namespace Hyper.Application.Features.Surveys.Queries;

/// <summary>
/// دریافت لیست نظرسنجی‌ها
/// </summary>
public record GetSurveysQuery : IRequest<List<SurveyDto>>
{
    public int? TenantId { get; set; }
    public SurveyType? SurveyType { get; set; }
    public bool? IsActive { get; set; }
}

public record SurveyDto
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string TenantName { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public SurveyType SurveyType { get; set; }
    public int? ProductId { get; set; }
    public string? ProductName { get; set; }
    public bool IsActive { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool AllowMultipleSelection { get; set; }
    public bool ShowResults { get; set; }
    public int TotalParticipants { get; set; }
    public long? ParticipationPoints { get; set; }
    public long? CorrectAnswerPoints { get; set; }
    public DateTime CreateDate { get; set; }
    public List<SurveyItemDto> Items { get; set; } = [];
}

public record SurveyItemDto
{
    public int Id { get; set; }
    public string OptionText { get; set; } = null!;
    public int DisplayOrder { get; set; }
    public bool IsCorrectAnswer { get; set; }
    public int VoteCount { get; set; }
    public int? PictureId { get; set; }
}

public class GetSurveysQueryHandler(
    IHyperUnitOfWorkQuery unitOfWork
) : IRequestHandler<GetSurveysQuery, List<SurveyDto>>
{
    public async Task<List<SurveyDto>> Handle(GetSurveysQuery request, CancellationToken cancellationToken)
    {
        var surveyRepo = unitOfWork.Repository<Survey, int>();
        var includes = new List<Expression<Func<Survey, object?>>> 
        { 
            s => s.Items,
            s => s.Promotion,
            s => s.Promotion.Tenant,
            s => s.Product
        };
        var surveys = await surveyRepo.GetAllWithIncludeAsync(
            includes: includes,
            cancellationToken: cancellationToken,
            predicate: s => (!request.TenantId.HasValue || s.Promotion.TenantId == request.TenantId) &&
                     (!request.SurveyType.HasValue || s.SurveyType == request.SurveyType) &&
                     (!request.IsActive.HasValue || s.IsActive == request.IsActive.Value));

        return surveys.Select(s => new SurveyDto
        {
            Id = s.Id,
            TenantId = s.Promotion.TenantId,
            TenantName = s.Promotion.Tenant?.Title ?? string.Empty,
            Title = s.Title,
            Description = s.Description,
            SurveyType = s.SurveyType,
            ProductId = s.ProductId,
            ProductName = s.Product?.Title ?? string.Empty,
            IsActive = s.IsActive,
            StartDate = s.StartDate,
            EndDate = s.EndDate,
            AllowMultipleSelection = s.AllowMultipleSelection,
            ShowResults = s.ShowResults,
            TotalParticipants = s.TotalParticipants,
            ParticipationPoints = s.ParticipationPoints,
            CorrectAnswerPoints = s.CorrectAnswerPoints,
            CreateDate = s.CreateDate,
            Items = s.Items.Select(i => new SurveyItemDto
            {
                Id = i.Id,
                OptionText = i.OptionText,
                DisplayOrder = i.DisplayOrder,
                IsCorrectAnswer = i.IsCorrectAnswer,
                VoteCount = i.VoteCount,
                PictureId = i.PictureId
            }).ToList()
        }).ToList();
    }
}
