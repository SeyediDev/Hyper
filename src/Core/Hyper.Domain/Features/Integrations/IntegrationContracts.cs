using Hyper.Domain.Entities.Integrations;
namespace Hyper.Domain.Features.Integrations;
public sealed record ExternalCatalogItem(string ExternalProductId,string? Sku,string Title,decimal? Price,decimal? Inventory,string? VariantId);
public interface IExternalIntegrationAdapter
{
    IntegrationProvider Provider { get; }
    bool SupportsCredentialType(IntegrationCredentialType credentialType);
    bool IsImplemented { get; }
    Task<IReadOnlyCollection<ExternalCatalogItem>> ReadCatalogAsync(ExternalIntegrationConnection connection,CancellationToken cancellationToken);
    Task PublishInventoryAsync(ExternalIntegrationConnection connection,IReadOnlyCollection<ExternalProductMapping> mappings,CancellationToken cancellationToken);
}
public interface IIntegrationSynchronizationService
{
    Task<long> SynchronizeAsync(long connectionId,CancellationToken cancellationToken=default);
}
