namespace Hyper.Infrastructure.Features.Integrations;

public abstract class ExternalIntegrationAdapterBase(IHttpClientFactory clients) : IExternalIntegrationAdapter
{
    public abstract IntegrationProvider Provider { get; }
    public abstract bool IsImplemented { get; }
    public abstract bool SupportsCredentialType(IntegrationCredentialType credentialType);
    protected HttpClient Client => clients.CreateClient("ExternalIntegrations");

    protected static void ApplyCorrelation(HttpRequestMessage request, string? correlationId)
    {
        if (!string.IsNullOrWhiteSpace(correlationId))
            request.Headers.TryAddWithoutValidation("X-Correlation-ID", correlationId);
    }

    public abstract Task<IReadOnlyCollection<ExternalCatalogItem>> ReadCatalogAsync(
        ExternalIntegrationConnection connection, CancellationToken cancellationToken);
    public abstract Task PublishInventoryAsync(ExternalIntegrationConnection connection,
        IReadOnlyCollection<ExternalInventoryUpdate> updates, CancellationToken cancellationToken);
}

// An unimplemented provider must never report a successful read or write.
public abstract class UnavailableIntegrationAdapter(IHttpClientFactory clients) : ExternalIntegrationAdapterBase(clients)
{
    public sealed override bool IsImplemented => false;
    public sealed override bool SupportsCredentialType(IntegrationCredentialType credentialType) => false;
    public sealed override Task<IReadOnlyCollection<ExternalCatalogItem>> ReadCatalogAsync(
        ExternalIntegrationConnection connection, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        throw new NotSupportedException($"The {Provider} catalog adapter is not implemented.");
    }
    public sealed override Task PublishInventoryAsync(ExternalIntegrationConnection connection,
        IReadOnlyCollection<ExternalInventoryUpdate> updates, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        throw new NotSupportedException($"The {Provider} inventory adapter is not implemented.");
    }
}

public sealed class DigikalaIntegrationAdapter(IHttpClientFactory clients) : UnavailableIntegrationAdapter(clients)
{
    public override IntegrationProvider Provider => IntegrationProvider.Digikala;
}
public sealed class TorobIntegrationAdapter(IHttpClientFactory clients) : UnavailableIntegrationAdapter(clients)
{
    public override IntegrationProvider Provider => IntegrationProvider.Torob;
}
