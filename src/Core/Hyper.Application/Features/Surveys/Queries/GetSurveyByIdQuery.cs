using Hyper.Domain.Entities.Tenants;

namespace Hyper.Application.Features.Surveys.Queries;

/// <summary>
/// دریافت جزئیات نظرسنجی با شناسه
/// </summary>
public record GetSurveyByIdQuery : IRequest<SurveyDetailDto>
{
    [Required]
    public int Id { get; set; }

    public int? CustomerTenantId { get; set; } // برای بررسی شرکت قبلی
}

public record SurveyDetailDto
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
    public bool HasParticipated { get; set; } // آیا مشتری قبلاً شرکت کرده
    public int? SelectedItemId { get; set; } // گزینه انتخاب شده قبلی
}

public class GetSurveyByIdQueryValidator : AbstractValidator<GetSurveyByIdQuery>
{
    public GetSurveyByIdQueryValidator(IMultiLingualService multiLingual)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(multiLingual.GetMessage("IdIsRequired"));
    }
}

public class GetSurveyByIdQueryHandler(
    IHyperUnitOfWorkQuery unitOfWork
) : IRequestHandler<GetSurveyByIdQuery, SurveyDetailDto>
{
    public async Task<SurveyDetailDto> Handle(GetSurveyByIdQuery request, CancellationToken cancellationToken)
    {
        var surveyRepo = unitOfWork.Repository<Survey, int>();
        var survey = await surveyRepo.GetByIdWithIncludeAsync(
            request.Id,
            cancellationToken,
            s => s.Items,
            s => s.Promotion
            );
        
        if (survey == null)
            throw new FluentValidation.ValidationException("نظرسنجی یافت نشد");

        // بررسی شرکت قبلی
        bool hasParticipated = false;
        int? selectedItemId = null;

        if (request.CustomerTenantId.HasValue)
        {
            var participationRepo = unitOfWork.Repository<SurveyParticipation, int>();
            var participation = await participationRepo.FirstOrDefaultAsync(
                p => p.SurveyId == request.Id && p.CustomerTenantId == request.CustomerTenantId,
                cancellationToken);
            if (participation != null)
            {
                hasParticipated = true;
                selectedItemId = participation.SelectedItemId;
            }
        }

        // Load Tenant separately if needed (assuming navigation property exists)
        var tenantRepo = unitOfWork.Repository<global::Hyper.Domain.Entities.Tenants.Tenant, int>();
        var tenant = survey.Promotion.TenantId > 0 
            ? await tenantRepo.GetByIdAsync(survey.Promotion.TenantId, cancellationToken)
            : null;

        return new SurveyDetailDto
        {
            Id = survey.Id,
            TenantId = survey.Promotion.TenantId,
            TenantName = tenant?.Title ?? string.Empty, // TODO: Check Tenant property name
            Title = survey.Title,
            Description = survey.Description,
            SurveyType = survey.SurveyType,
            ProductId = survey.ProductId,
            ProductName = survey.Product?.Title ?? string.Empty, // TODO: Check Product property name
            IsActive = survey.IsActive,
            StartDate = survey.StartDate,
            EndDate = survey.EndDate,
            AllowMultipleSelection = survey.AllowMultipleSelection,
            ShowResults = survey.ShowResults,
            TotalParticipants = survey.TotalParticipants,
            ParticipationPoints = survey.ParticipationPoints,
            CorrectAnswerPoints = survey.CorrectAnswerPoints,
            CreateDate = survey.CreateDate,
            Items = survey.Items.Select(i => new SurveyItemDto
            {
                Id = i.Id,
                OptionText = i.OptionText,
                DisplayOrder = i.DisplayOrder,
                IsCorrectAnswer = i.IsCorrectAnswer,
                VoteCount = i.VoteCount,
                PictureId = i.PictureId
            }).ToList(),
            HasParticipated = hasParticipated,
            SelectedItemId = selectedItemId
        };
    }
}
