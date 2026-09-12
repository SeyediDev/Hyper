using Neo.Application.Exceptions;
using Neo.Domain.Features.ObjectStore;
using Neo.Domain.Features.ObjectStore.Dto;

namespace Hyper.Application.Features.Common.Queries.Documents;

public record GetDocumentQuery() : IRequest<DocumentQueryResponse?>
{
    public int SubjectId { get; set; }
    public string SubjectTitle { get; set; } = null!;
    public string? SubjectField { get; set; }
    public Domain.Enums.DocumentType? DocumentType { get; set; }
}

public record DocumentQueryResponse(int DocumentId, ObjectStoreDto Dto)
{
    public string? DocumentType { get; set; }
    public string? SubjectField { get; internal set; }
    public DateTimeOffset? CreateDate { get; set; }
}


public class GetDocumentQueryHandler(IQueryRepository<Document, int> queryRepository, IObjectStoreService objectStoreService)
    : IRequestHandler<GetDocumentQuery, DocumentQueryResponse?>
{
    public async Task<DocumentQueryResponse?> Handle(GetDocumentQuery request, CancellationToken cancellationToken)
    {
        var document = await queryRepository.FirstOrDefaultWithIncludeAsync(
            x => x.DocumentType,
            x =>
            x.SubjectTitle == request.SubjectTitle &&
            x.SubjectId == request.SubjectId &&
            (request.DocumentType == null || x.DocumentTypeId == (int)request.DocumentType) &&
            (request.SubjectField == null || x.SubjectField == request.SubjectField)
            , cancellationToken);
        if (document is null)
            return null;
        var result = document.Content == null ?
            await objectStoreService.DownloadFileAsync(document.ObjectStorageName) :
            new() { Content = document.Content, Type = document.DocumentType?.Title };
        if (result is null) throw new BadRequestException("سندی یافت نشد.");
        return new DocumentQueryResponse(document.Id, result)
        {
            DocumentId = document.Id,
            DocumentType = document.DocumentType?.Title,
            SubjectField = document.SubjectField,
            CreateDate = document.CreateDate,
        };
    }
}
