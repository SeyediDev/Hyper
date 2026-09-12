using Neo.Application.Features.GenericEntity.Commands;
using Neo.Domain.Features.ObjectStore;
using Neo.Domain.Dto;

namespace Hyper.Application.Features.Common.Commands.Documents;

public record AddDocumentCommand(string SubjectTitle, string SubjectField, long SubjectId) : IRequest<int?>
{
    public int? DocumentTypeId { get; set; }
    public string? FileType { get; set; }
    public byte[]? Content { get; set; }
    public bool SaveContentInRecord { get; set; }
}
public class AttachmentDto
{
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public string Base64 { get; set; } = null!;
}

public class AddDocumentCommandValidator : AbstractValidator<AddDocumentCommand>
{
    public AddDocumentCommandValidator(IMultiLingualService multiLingual)
    {
 
    }
}

public class AddDocumentCommandHandler(
    ICreateGenericEntityCommandHandler<DocumentDto, Document, int> createHandler, 
    ICommandRepository<Document, int> documentCommandRepository, IObjectStoreService objectStoreService)
    : IRequestHandler<AddDocumentCommand, int?>
{
    public async Task<int?> Handle(AddDocumentCommand request, CancellationToken cancellationToken)
    {
        int? documentId = null!;
        await documentCommandRepository.UnitOfWork.DoTransaction(async () =>
        {
            documentId = await createHandler.Send(new CreateGenericEntityCommand<DocumentDto, Document, int>
            {
                Dto = new DocumentDto
                {
                    DocumentTypeId = request.DocumentTypeId,
                    SubjectId = request.SubjectId,
                    SubjectTitle = request.SubjectTitle,
                    SubjectField = request.SubjectField,
                    Content = request.SaveContentInRecord ? request.Content : null,
                },
                BeforeCreate = async _ =>
                {
                    var oldDocument = await documentCommandRepository.FirstOrDefaultAsync(x => 
                                                                                    x.DocumentTypeId == request.DocumentTypeId &&
                                                                                    x.SubjectId== request.SubjectId &&
                                                                                    x.SubjectTitle == request.SubjectTitle &&
                                                                                    x.SubjectField == request.SubjectField,
                                                                                    cancellationToken);
                    if (oldDocument is not null)
                    {
                        oldDocument.ExpireDate = DateTime.UtcNow;
                        oldDocument.IsDeleted = true;
                        await documentCommandRepository.UnitOfWork.SaveChangesAsync();
                    }
                },
                AfterCreate = async entity =>
                {
                    string? checkSum;
                    if (request.SaveContentInRecord)
                    {
                        checkSum = HashCode.Combine(request.Content).ToString();
                    }
                    else
                    {
                        checkSum = await objectStoreService.UploadFileAsync(entity.ObjectStorageName, new Neo.Domain.Features.ObjectStore.Dto.ObjectStoreDto
                        {
                            Content = request.Content!,
                            Type = request.FileType
                        });
                    }
                    entity.Checksum = checkSum;
                }
            }, cancellationToken);
        });
        return documentId;
    }
}

public record DocumentDto : IDto<int>
{
    public int? Id { get; set; }
    public int? DocumentTypeId { get; set; }
    public long? SubjectId { get; set; }
    public string? SubjectTitle { get; set; }
    public string? SubjectField { get; set; }
    public byte[]? Content { get; set; }
}
