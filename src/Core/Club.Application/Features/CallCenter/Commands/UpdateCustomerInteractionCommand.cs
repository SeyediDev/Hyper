namespace Hyper.Application.Features.CallCenter.Commands;

/// <summary>
/// دستور به‌روزرسانی تعامل با مشتری
/// </summary>
public record UpdateCustomerInteractionCommand : IRequest
{
    public long InteractionId { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public DateTime? EndTime { get; init; }
    public int? DurationMinutes { get; init; }
    public int? InteractionOutcomeId { get; init; }
    public string? OutcomeNotes { get; init; }
    public int? SatisfactionScore { get; init; }
    public string? CustomerFeedback { get; init; }
    public bool? RequiresFollowUp { get; init; }
    public DateTime? FollowUpDate { get; init; }
    public InteractionStatus? Status { get; init; }
    public PriorityLevel? Priority { get; init; }
}

public class UpdateCustomerInteractionCommandValidator : AbstractValidator<UpdateCustomerInteractionCommand>
{
    public UpdateCustomerInteractionCommandValidator()
    {
        RuleFor(x => x.InteractionId)
            .GreaterThan(0)
            .WithMessage("شناسه تعامل باید بزرگتر از صفر باشد");

        RuleFor(x => x.Title)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Title))
            .WithMessage("عنوان نمی‌تواند بیشتر از 200 کاراکتر باشد");

        RuleFor(x => x.OutcomeNotes)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.OutcomeNotes))
            .WithMessage("توضیحات نتیجه نمی‌تواند بیشتر از 1000 کاراکتر باشد");

        RuleFor(x => x.CustomerFeedback)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.CustomerFeedback))
            .WithMessage("نظرات مشتری نمی‌تواند بیشتر از 2000 کاراکتر باشد");

        RuleFor(x => x.SatisfactionScore)
            .InclusiveBetween(1, 5)
            .When(x => x.SatisfactionScore.HasValue)
            .WithMessage("نمره رضایت باید بین 1 تا 5 باشد");
    }
}

internal sealed class UpdateCustomerInteractionCommandHandler(
    ICommandRepository<CustomerInteraction, long> interactionRepository,
    IQueryRepository<InteractionOutcome, int> outcomeRepository,
    IHyperUnitOfWorkCommand unitOfWork
) : IRequestHandler<UpdateCustomerInteractionCommand>
{
    public async Task Handle(UpdateCustomerInteractionCommand request, CancellationToken cancellationToken)
    {
        var interaction = await interactionRepository.FirstOrDefaultAsync(
            x => x.Id == request.InteractionId,
            cancellationToken) ?? throw new InvalidOperationException("تعامل یافت نشد");

        if (!string.IsNullOrWhiteSpace(request.Title))
            interaction.Title = request.Title;

        if (request.Description != null)
            interaction.Description = request.Description;

        if (request.EndTime.HasValue)
        {
            interaction.EndTime = request.EndTime;
            var duration = (int)(request.EndTime.Value - interaction.StartTime).TotalMinutes;
            interaction.DurationMinutes = duration;
        }

        if (request.DurationMinutes.HasValue)
            interaction.DurationMinutes = request.DurationMinutes;

        if (request.InteractionOutcomeId.HasValue)
        {
            var outcome = await outcomeRepository.FirstOrDefaultAsync(
                x => x.Id == request.InteractionOutcomeId.Value && x.IsActive,
                cancellationToken) ?? throw new InvalidOperationException("نتیجه تعامل یافت نشد");

            interaction.InteractionOutcomeId = request.InteractionOutcomeId.Value;
            interaction.RequiresFollowUp = outcome.RequiresFollowUp;
        }

        if (!string.IsNullOrWhiteSpace(request.OutcomeNotes))
            interaction.OutcomeNotes = request.OutcomeNotes;

        if (request.SatisfactionScore.HasValue)
            interaction.SatisfactionScore = request.SatisfactionScore;

        if (request.CustomerFeedback != null)
            interaction.CustomerFeedback = request.CustomerFeedback;

        if (request.RequiresFollowUp.HasValue)
            interaction.RequiresFollowUp = request.RequiresFollowUp.Value;

        if (request.FollowUpDate.HasValue)
            interaction.FollowUpDate = request.FollowUpDate;

        if (request.Status.HasValue)
            interaction.Status = request.Status.Value;

        if (request.Priority.HasValue)
            interaction.Priority = request.Priority.Value;

        interactionRepository.Update(interaction);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

