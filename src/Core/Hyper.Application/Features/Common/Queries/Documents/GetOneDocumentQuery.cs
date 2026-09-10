using Neo.Application.Exceptions;
using Neo.Domain.Features.ObjectStore;
using Hyper.Domain.Entities.Common;

namespace Hyper.Application.Features.Common.Queries.Documents;

public record GetOneDocumentQuery() : IRequest<DocumentQueryResponse?>
{
    public int? DocumentId { get; set; }
}

public class GetOneDocumentQueryHandler(IQueryRepository<Document, int> queryRepository, IObjectStoreService objectStoreService)
    : IRequestHandler<GetOneDocumentQuery, DocumentQueryResponse?>
{
    public async Task<DocumentQueryResponse?> Handle(GetOneDocumentQuery request, CancellationToken cancellationToken)
    {
        Document? document = await queryRepository.FirstOrDefaultWithIncludeAsync(
            x => x.DocumentType,
            x => x.Id == request.DocumentId, cancellationToken);
        if (document is null)
        {
            return null;
        }

        Neo.Domain.Features.ObjectStore.Dto.ObjectStoreDto? result = document.Content == null 
            ? await objectStoreService.DownloadFileAsync(document.ObjectStorageName) :
            new() { Content = document.Content, Type = document.DocumentType?.Title };
        return result is null
            ? throw new BadRequestException("سندی یافت نشد.")
            : new DocumentQueryResponse(document.Id, result)
            {
                CreateDate = document.CreateDate,
                DocumentType = document.DocumentType?.Title ?? result.Type,
                SubjectField = document.SubjectField
            };
    }
}
