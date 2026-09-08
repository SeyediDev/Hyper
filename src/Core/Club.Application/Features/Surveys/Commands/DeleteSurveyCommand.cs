namespace Hyper.Application.Features.Surveys.Commands;

/// <summary>
/// حذف نظرسنجی
/// </summary>
public record DeleteSurveyCommand : IRequest
{
    [Required]
    public int Id { get; set; }
}

public class DeleteSurveyCommandValidator : AbstractValidator<DeleteSurveyCommand>
{
    public DeleteSurveyCommandValidator(IMultiLingualService multiLingual)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(multiLingual.GetMessage("IdIsRequired"));
    }
}

public class DeleteSurveyCommandHandler(
    IHyperUnitOfWorkCommand unitOfWork
) : IRequestHandler<DeleteSurveyCommand>
{
    public async Task Handle(DeleteSurveyCommand request, CancellationToken cancellationToken)
    {
        var survey = await unitOfWork.Repository<Survey, int>()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (survey == null)
            throw new FluentValidation.ValidationException("نظرسنجی یافت نشد");

        // Note: EF will cascade delete Items and Participations
        unitOfWork.Repository<Survey, int>().Remove(survey);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
