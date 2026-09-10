namespace Hyper.Application.Features.Surveys.Commands;

/// <summary>
/// ویرایش نظرسنجی
/// </summary>
public record UpdateSurveyCommand : IRequest
{
    [Required]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = null!;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public int? ProductId { get; set; }

    public bool IsActive { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool AllowMultipleSelection { get; set; }

    public bool ShowResults { get; set; }

    public long? ParticipationPoints { get; set; }

    public long? CorrectAnswerPoints { get; set; }
}

public class UpdateSurveyCommandValidator : AbstractValidator<UpdateSurveyCommand>
{
    public UpdateSurveyCommandValidator(IMultiLingualService multiLingual)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(multiLingual.GetMessage("IdIsRequired"));

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(multiLingual.GetMessage("TitleIsRequired"))
            .MaximumLength(200).WithMessage(multiLingual.GetMessage("TitleMaxLength"));

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage(multiLingual.GetMessage("DescriptionMaxLength"));

        When(x => x.EndDate.HasValue && x.StartDate.HasValue, () =>
        {
            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .WithMessage("تاریخ پایان باید بعد از تاریخ شروع باشد");
        });
    }
}

public class UpdateSurveyCommandHandler(
    IHyperUnitOfWorkCommand unitOfWork
) : IRequestHandler<UpdateSurveyCommand>
{
    public async Task Handle(UpdateSurveyCommand request, CancellationToken cancellationToken)
    {
        var survey = await unitOfWork.Repository<Survey, int>()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (survey == null)
            throw new FluentValidation.ValidationException("نظرسنجی یافت نشد");

        survey.Title = request.Title;
        survey.Description = request.Description;
        survey.ProductId = request.ProductId;
        survey.IsActive = request.IsActive;
        survey.StartDate = request.StartDate;
        survey.EndDate = request.EndDate;
        survey.AllowMultipleSelection = request.AllowMultipleSelection;
        survey.ShowResults = request.ShowResults;
        survey.ParticipationPoints = request.ParticipationPoints;
        survey.CorrectAnswerPoints = request.CorrectAnswerPoints;

        unitOfWork.Repository<Survey, int>().Update(survey);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
