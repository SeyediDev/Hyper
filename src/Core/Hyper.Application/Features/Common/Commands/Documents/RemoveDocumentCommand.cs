namespace Hyper.Application.Features.Common.Commands.Documents;

public record RemoveDocumentCommand : IRequest<Unit>
{
    public required int DocumentId { get; set; }
}

public class RemoveDocumentCommandValidator : AbstractValidator<RemoveDocumentCommand>
{
    public RemoveDocumentCommandValidator(IMultiLingualService multiLingual)
    {
        RuleFor(x => x.DocumentId)
                  .NotEmpty().WithMessage(multiLingual.GetMessage("Required"));
    }
}

public class RemoveDocumentCommandHandler : IRequestHandler<RemoveDocumentCommand, Unit>
{
    public async Task<Unit> Handle(RemoveDocumentCommand request, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        return Unit.Value;
    }
}
