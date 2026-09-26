namespace Hyper.Integration.Domain.Features.Integrations;

public sealed record ExternalProductUpdate(string ExternalProductId, string? VariantId,
    string? Title, decimal? PrimaryPrice)
{
    public string? ValidationError() => string.IsNullOrWhiteSpace(ExternalProductId) ? "InvalidExternalIdentifier"
        : VariantId is not null ? "ProductVariantUpdateUnsupported"
        : Title is null && PrimaryPrice is null ? "EmptyProductChange"
        : Title is not null && (string.IsNullOrWhiteSpace(Title) || Title.Length > 500) ? "InvalidProductTitle"
        : PrimaryPrice is { } price && (price < 0 || price > long.MaxValue || decimal.Truncate(price) != price)
            ? "InvalidPrimaryPrice" : null;
}

// Optional capability: existing inventory-only providers need not implement product publishing.
public interface IExternalProductPublisher
{
    Task PublishProductAsync(ExternalIntegrationConnection connection, ExternalProductUpdate update, CancellationToken ct);
}

public interface IIntegrationProductOutbox
{
    Task<long> EnqueueProductAsync(long connectionId, long mappingId, long sourceVersion,
        ExternalProductUpdate update, CancellationToken ct);
}
