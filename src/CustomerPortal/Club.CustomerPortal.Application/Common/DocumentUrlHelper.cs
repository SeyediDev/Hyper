namespace Hyper.CustomerPortal.Application.Common;

public static class DocumentUrlHelper
{
    public static string? BuildDocumentUrl(int? documentId)
        => documentId.HasValue ? $"/api/documents/{documentId.Value}/download" : null;
}

