using Neo.Application.Exceptions;
using Neo.Domain.Features.ObjectStore;
using Neo.Domain.Features.ObjectStore.Dto;
using Hyper.Domain.Entities.Common;

namespace Hyper.Application.Features.Common.Queries.Documents;

public record DocumentsQuery() : IRequest<Dictionary<int,DocumentItemResponse>>
{
    public List<int> DocumentIds { get; set; } = null!;
}

public class DocumentsQueryHandler(IQueryRepository<Document, int> queryRepository, IObjectStoreService objectStoreService) 
    : IRequestHandler<DocumentsQuery, Dictionary<int, DocumentItemResponse>>
{
    public async Task<Dictionary<int,DocumentItemResponse>> Handle(DocumentsQuery request, CancellationToken cancellationToken)
    {
        var documents = await queryRepository.GetAllWithIncludeAsync(
            x=>x.DocumentType, cancellationToken, 
            x => request.DocumentIds.Contains(x.Id));
        Dictionary<int, DocumentItemResponse> responses = [];
        foreach (var document in documents)
        {
            ObjectStoreDto? result=null;
            if (document.Content == null)
            {
                result = await objectStoreService.DownloadFileAsync(document.ObjectStorageName);
                if (result is null) throw new BadRequestException("سندی یافت نشد.");
            }
            else
            {
                result = new() { Content = document.Content, Type= document.DocumentType?.Title };
            }
            responses.Add(document.Id, new DocumentItemResponse(document.Id, result)
            {
                DocumentTypeId = document.DocumentTypeId,
                DocumentType = document.DocumentType?.Title?? "",
                SubjectId = document.SubjectId, 
                SubjectField = document.SubjectField,
                CreateDate = document.CreateDate,
            });
        }
        return responses;
    }
}
