namespace Hyper.Integration.Domain.Features.Integrations;
public sealed record ExternalCatalogItem(string ExternalProductId,string? Sku,string Title,decimal? Price,decimal? Inventory,string? VariantId);
// Absolute, authoritative Hyper stock; never an external catalog snapshot or a delta.
public sealed record ExternalInventoryUpdate(string ExternalProductId, string? VariantId, decimal Quantity);
public interface IExternalIntegrationAdapter
{
    IntegrationProvider Provider { get; }
    bool SupportsCredentialType(IntegrationCredentialType credentialType);
    bool IsImplemented { get; }
    Task<IReadOnlyCollection<ExternalCatalogItem>> ReadCatalogAsync(ExternalIntegrationConnection connection,CancellationToken cancellationToken);
    Task PublishInventoryAsync(ExternalIntegrationConnection connection,IReadOnlyCollection<ExternalInventoryUpdate> updates,CancellationToken cancellationToken);
}
public interface IIntegrationSynchronizationService
{
    Task<long> SynchronizeAsync(long connectionId,CancellationToken cancellationToken=default);
}

