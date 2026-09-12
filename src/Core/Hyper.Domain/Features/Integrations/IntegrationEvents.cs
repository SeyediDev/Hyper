namespace Hyper.Domain.Features.Integrations;

public sealed record IntegrationEventEnvelope(
    string EventId, string EventType, string Provider, string TenantId, int ShopId,
    string PayloadJson, string Source, string? CorrelationId, DateTime OccurredAtUtc);

public interface IIntegrationStrategyResolver
{
    IExternalIntegrationAdapter Resolve(IntegrationProvider provider, IntegrationCredentialType credentialType);
}

public sealed class IntegrationStrategyResolver(IEnumerable<IExternalIntegrationAdapter> adapters)
    : IIntegrationStrategyResolver
{
    private readonly IExternalIntegrationAdapter[] _adapters = adapters.ToArray();

    public IExternalIntegrationAdapter Resolve(IntegrationProvider provider, IntegrationCredentialType credentialType)
    {
        if (!Enum.IsDefined(provider) || !Enum.IsDefined(credentialType))
            throw new NotSupportedException("Unknown integration provider or credential type.");

        var matches = _adapters.Where(x => x.Provider == provider && x.SupportsCredentialType(credentialType)).ToArray();
        if (matches.Length == 0)
            throw new NotSupportedException($"No adapter supports {provider}/{credentialType}.");
        if (matches.Length > 1)
            throw new InvalidOperationException($"Multiple adapters support {provider}/{credentialType}.");
        if (!matches[0].IsImplemented)
            throw new NotSupportedException($"The {provider} adapter is not implemented.");
        return matches[0];
    }
}
