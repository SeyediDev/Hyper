using Neo.Bpms.Domain.Features.Cmmn.ObjectStorage.Dto;
using Neo.Common.Utility;
using Hyper.Application.Features.Common.Commands.Documents;
using Hyper.Application.Features.Common.Queries.Documents;
using MassTransit.Initializers;
using MediatR;
using System.Text;
using Neo.Bpms.Domain.Models.Cmmn.Entities;

namespace Hyper.AdminPanel.Web.Infrastructure.Cmmn;
public class CmmnDocument(ISender sender) : ICmmnDocument
{
    public async Task DeleteFileData(Entity entity, string subjectField, string recordId,
        string oldDocumentId, CancellationToken cancellationToken)
    {
        _ = int.TryParse(oldDocumentId, out int old_record_id);
        RemoveDocumentCommand doc = new()
        {
            DocumentId = old_record_id,
        };
        _ = await sender.Send(doc, cancellationToken);
    }

    public async Task<string> MoveAndSaveFile(Entity entity, string subjectField, int? documentTypeId, string recordId,
        string oldDocumentId, object fileData, CancellationToken cancellationToken)
    {
        _ = long.TryParse(recordId, out long subjectId);
        AddDocumentCommand doc = GetAddDocumentCommandDto(entity, subjectField, documentTypeId, fileData, subjectId);

        int? documentId = await sender.Send(doc, cancellationToken);
        return documentId?.ToString()!;
    }

    public async Task<string> SaveFileData(Entity entity, string subjectField, int? documentTypeId,
        string recordId, object fileData, CancellationToken cancellationToken)
    {
        _ = long.TryParse(recordId, out long subjectId);
        AddDocumentCommand doc = GetAddDocumentCommandDto(entity, subjectField, documentTypeId, fileData, subjectId);
        int? documentId = await sender.Send(doc, cancellationToken);
        return documentId?.ToString()!;
    }

    public async Task<List<DocumentView>> GetDocuments(Entity entity, string? subjectField,
        int subjectId, bool loadData, CancellationToken cancellationToken)
    {
        GetDocumentsQuery query = new()
        {
            SubjectTitle = entity.Id,
            SubjectId = subjectId,
            SubjectField = subjectField,
            LoadData = true
        };
        List<DocumentItemResponse> response = await sender.Send(query, cancellationToken);
        List<DocumentView> result = [.. response.Select(GetDocumentView)];
        return result;
    }

    public async Task<DocumentView> GetDocumentData(int id, CancellationToken cancellationToken)
    {
        GetOneDocumentQuery query = new()
        {
            DocumentId = id
        };
        DocumentQueryResponse? doc = await sender.Send(query, cancellationToken);
        DocumentView result = GetDocumentView(doc!);
        return result;
    }

    private DocumentView GetDocumentView(DocumentQueryResponse doc)
    {
        string base64 = Encoding.UTF8.GetString(doc?.Dto?.Content!);
        string mimeType = doc?.Dto?.Content is not null ? NeoMimeTypes.GetMimeType(doc.Dto.Content, base64) : "application/octet-stream";
        return new DocumentView(
            doc?.DocumentId??0,
            doc?.SubjectField,
            doc?.DocumentType ?? doc?.Dto?.Type,
            doc?.Dto,
            base64,
            mimeType,
            NeoMimeTypes.GetExtension(mimeType),
            doc?.CreateDate);
    }

    public string UploadedFilesPath()
    {
        //TODO
        throw new NotImplementedException();
    }

    public PaintableFileInfo GetPaintableFileInfo(DocumentView document)
    {
        //TODO
        throw new NotImplementedException();
    }
    private AddDocumentCommand GetAddDocumentCommandDto(Entity entity, string subjectField, 
        int? documentTypeId, object fileData, long subjectId)
    {
        AddDocumentCommand doc = new(entity.Id, subjectField, subjectId)
        {
            DocumentTypeId = documentTypeId,
        };
        if (fileData is AttachmentDto attachmentDto)
        {
            doc.Content = Convert.FromBase64String(attachmentDto.Base64);
            doc.FileType = attachmentDto.ContentType;
            //todo doc.OriginalFileName = attachmentDto.FileName;
        }
        else
        {
            doc.Content = Encoding.UTF8.GetBytes(fileData?.ToString()!);
        }

        return doc;
    }
}
