using Neo.Application.Exceptions;
using Neo.Domain.Features.ObjectStore;
using Neo.Domain.Features.ObjectStore.Dto;

namespace Hyper.Application.Features.Common.Queries.Documents;

public record GetDocumentsQuery() : IRequest<List<DocumentItemResponse>>
{
    public string SubjectTitle { get; set; } = null!;
    public string? SubjectField { get; set; }
    public int SubjectId { get; set; }
    public bool LoadData { get; set; }
}

public record DocumentItemResponse(int DocumentId, ObjectStoreDto? Dto): DocumentQueryResponse(DocumentId, Dto!)
{
    public int? DocumentTypeId { get; set; }
    public long? SubjectId { get; set; }
}


public class GetDocumentsQueryHandler(IQueryRepository<Document, int> queryRepository, IObjectStoreService objectStoreService) 
    : IRequestHandler<GetDocumentsQuery, List<DocumentItemResponse>>
{
    public async Task<List<DocumentItemResponse>> Handle(GetDocumentsQuery request, CancellationToken cancellationToken)
    {
        var documents = await queryRepository.GetAllWithIncludeAsync(x=>x.DocumentType, cancellationToken, 
            x =>
            x.SubjectTitle == request.SubjectTitle &&
            x.SubjectId == request.SubjectId &&
            (request.SubjectField==null || x.SubjectField == request.SubjectField));
        List<DocumentItemResponse> responses = [];
        foreach (var document in documents)
        {
            ObjectStoreDto? result=null;
            if (request.LoadData)
            {
                if (document.Content == null)
                {
                    result = await objectStoreService.DownloadFileAsync(document.ObjectStorageName);
                    if (result is null) throw new BadRequestException("سندی یافت نشد.");
                }
                else
                {
                    result = new() { Content = document.Content, Type= document.DocumentType?.Title };
                }
            }
            responses.Add( new DocumentItemResponse(document.Id, result)
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
