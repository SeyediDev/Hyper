namespace Hyper.Integration.Domain.Features.Integrations;

public sealed record ExternalProductDraft(string Name, long PrimaryPrice, int CategoryId,
    int PreparationDays, int PackageWeight, string? Description, long? PhotoId);
public interface IExternalProductCreator
{
    Task PrepareCreationAsync(ExternalIntegrationConnection connection, CancellationToken ct);
    // Implementations must not internally retry this non-idempotent POST.
    Task<string> CreateDraftAsync(ExternalIntegrationConnection connection, ExternalProductDraft draft, CancellationToken ct);
    Task<ExternalCatalogItem> ReadCreatedAsync(ExternalIntegrationConnection connection, string productId, CancellationToken ct);
}
public interface IIntegrationProductCreationProcessor
{
    Task<IntegrationScenarioResult> ProcessAsync(IntegrationScenarioJob job, ExternalIntegrationConnection connection, CancellationToken ct);
}
