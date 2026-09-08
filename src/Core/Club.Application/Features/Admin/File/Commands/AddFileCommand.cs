using Hyper.Application.Features.Common.Commands.Documents;
using System.Text;

namespace Hyper.Application.Features.Admin.File.Commands;
public record AddFileCommand(string SubjectTitle, string SubjectField) : IRequest<Unit>
{
    public int SubjectId { get; set; }
    public string Content { get; set; } = null!;
}

public class AddFileCommandValidator : AbstractValidator<AddFileCommand>
{
    public AddFileCommandValidator(IMultiLingualService multiLingual)
    {
        RuleFor(x => x.SubjectId)
                   .NotEmpty().WithMessage(multiLingual.GetMessage("Required"));
        RuleFor(x => x.SubjectTitle)
           .NotEmpty().WithMessage(multiLingual.GetMessage("Required"));
        RuleFor(x => x.SubjectField)
          .NotEmpty().WithMessage(multiLingual.GetMessage("Required"));
        RuleFor(x => x.Content)
           .NotEmpty().WithMessage(multiLingual.GetMessage("Required"));
    }
}

public class AddFileCommandHandler(ISender sender) : IRequestHandler<AddFileCommand, Unit>
{
    public async Task<Unit> Handle(AddFileCommand request, CancellationToken cancellationToken)
    {
        var doc = new AddDocumentCommand(request.SubjectTitle, request.SubjectField, request.SubjectId)
        {
            Content = Encoding.UTF8.GetBytes(request.Content),
        };
        await sender.Send(doc, cancellationToken);
        return Unit.Value;
    }
}
