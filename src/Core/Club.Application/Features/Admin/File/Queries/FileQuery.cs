using Hyper.Application.Features.Common.Queries.Documents;

namespace Hyper.Application.Features.Admin.File.Queries;
public record FileQuery : IRequest<FileQueryResponse>
{
    public required int SubjectId { get; set; }
    public required string SubjectTitle { get; set; }
    public string? SubjectField { get; set; }
    public int? DocumentTypeId { get; set; }
}

public record FileQueryResponse(List<FileDto> Files)
{
}

public record FileDto(int DocumentId, string? Content)
{
}

public class SubjectFileQueryHandler(ISender sender) : IRequestHandler<FileQuery, FileQueryResponse>
{
    public async Task<FileQueryResponse> Handle(FileQuery request, CancellationToken cancellationToken)
    {
        var files = await sender.Send(new GetListDocumentQuery()
        {
            SubjectId = request.SubjectId,
            SubjectTitle = request.SubjectTitle,
            DocumentTypeId = request.DocumentTypeId,
            SubjectField = request.SubjectField
        }, cancellationToken);
        var fileDto = files.Select(x => new FileDto(x.DocumentId, x.Content)).ToList();
        return new FileQueryResponse(fileDto);
    }
}
