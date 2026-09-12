using Neo.Domain.Features.ObjectStore;
using System.Text;

namespace Hyper.Application.Features.Common.Queries.Documents;

public record GetListDocumentQuery() : IRequest<List<GetListDocumentQueryResponse>>
{
    public int? SubjectId { get; set; }
    public string? SubjectTitle { get; set; }
    public string? SubjectField { get; set; }
    public int? DocumentTypeId { get; set; }
}

public record GetListDocumentQueryResponse
{
    public int DocumentId { get; set; }
    public string? Content { get; set; }
}

public class GetListDocumentQueryHandler(IQueryRepository<Document, int> queryRepository, IObjectStoreService objectStoreService)
    : IRequestHandler<GetListDocumentQuery, List<GetListDocumentQueryResponse>>
{
    public async Task<List<GetListDocumentQueryResponse>> Handle(GetListDocumentQuery request, CancellationToken cancellationToken)
    {
        var documents = await queryRepository.GetAllAsync(cancellationToken,
                                                            x => x.DocumentTypeId == request.DocumentTypeId &&
                                                            x.SubjectId == request.SubjectId &&
                                                            x.SubjectTitle == request.SubjectTitle &&
                                                            x.SubjectField == request.SubjectField);

        var documentResponses = await Task.WhenAll(documents.Select(async d => new GetListDocumentQueryResponse
        {
            DocumentId = d.Id,
            Content = Encoding.UTF8.GetString(d.Content?? await DownloadFileAsync(d.ObjectStorageName))
                
        }));
        return [.. documentResponses];
    }

    private async Task<byte[]> DownloadFileAsync(string objectId)
    {
        var result = await objectStoreService.DownloadFileAsync(objectId);
        if (result is null) return [];
        return result.Content;
    }
}