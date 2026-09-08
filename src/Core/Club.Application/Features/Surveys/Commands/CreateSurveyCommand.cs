namespace Hyper.Application.Features.Surveys.Commands;

/// <summary>
/// ایجاد نظرسنجی جدید
/// </summary>
public record CreateSurveyCommand : IRequest<int>
{
    [Required]
    public int PromotionId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = null!;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public SurveyType SurveyType { get; set; } = SurveyType.Survey;

    public int? ProductId { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool AllowMultipleSelection { get; set; } = false;

    public bool ShowResults { get; set; } = true;

    public long? ParticipationPoints { get; set; }

    public long? CorrectAnswerPoints { get; set; }

    /// <summary>
    /// گزینه‌های نظرسنجی
    /// </summary>
    public List<CreateSurveyItemDto> Items { get; set; } = [];
}

public record CreateSurveyItemDto
{
    [Required]
    [MaxLength(500)]
    public string OptionText { get; set; } = null!;

    public int DisplayOrder { get; set; }

    public bool IsCorrectAnswer { get; set; } = false;

    public int? PictureId { get; set; }
}

public class CreateSurveyCommandValidator : AbstractValidator<CreateSurveyCommand>
{
    public CreateSurveyCommandValidator(IMultiLingualService multiLingual)
    {
        RuleFor(x => x.PromotionId)
            .NotEmpty().WithMessage(multiLingual.GetMessage("TenantIdIsRequired"));

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(multiLingual.GetMessage("TitleIsRequired"))
            .MaximumLength(200).WithMessage(multiLingual.GetMessage("TitleMaxLength"));

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage(multiLingual.GetMessage("DescriptionMaxLength"));

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("نظرسنجی باید حداقل یک گزینه داشته باشد")
            .Must(items => items.Count >= 2).WithMessage("نظرسنجی باید حداقل دو گزینه داشته باشد");

        RuleFor(x => x)
            .Must(x => x.SurveyType != SurveyType.Contest || x.Items.Any(i => i.IsCorrectAnswer))
            .WithMessage("در مسابقات باید حداقل یک پاسخ صحیح مشخص شود");

        When(x => x.EndDate.HasValue && x.StartDate.HasValue, () =>
        {
            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .WithMessage("تاریخ پایان باید بعد از تاریخ شروع باشد");
        });
    }
}

public class CreateSurveyCommandHandler(
    IHyperUnitOfWorkCommand unitOfWork
) : IRequestHandler<CreateSurveyCommand, int>
{
    public async Task<int> Handle(CreateSurveyCommand request, CancellationToken cancellationToken)
    {
        var survey = new Survey
        {
            PromotionId = request.PromotionId,
            Title = request.Title,
            Description = request.Description,
            SurveyType = request.SurveyType,
            ProductId = request.ProductId,
            IsActive = request.IsActive,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            AllowMultipleSelection = request.AllowMultipleSelection,
            ShowResults = request.ShowResults,
            ParticipationPoints = request.ParticipationPoints,
            CorrectAnswerPoints = request.CorrectAnswerPoints,
            TotalParticipants = 0
        };

        // افزودن گزینه‌ها
        foreach (var itemDto in request.Items)
        {
            survey.Items.Add(new SurveyItem
            {
                OptionText = itemDto.OptionText,
                DisplayOrder = itemDto.DisplayOrder,
                IsCorrectAnswer = itemDto.IsCorrectAnswer,
                PictureId = itemDto.PictureId,
                VoteCount = 0
            });
        }

        unitOfWork.Repository<Survey, int>().Add(survey);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return survey.Id;
    }
}
