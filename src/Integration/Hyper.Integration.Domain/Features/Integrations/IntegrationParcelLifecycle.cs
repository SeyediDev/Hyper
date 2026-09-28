namespace Hyper.Integration.Domain.Features.Integrations;

// Provider codes and transport JSON remain in the adapter, never accounting.
public sealed record ExternalParcelState(string ParcelId, string OrderId, string? State,
    string? TrackingCode, int ShippingMethod, string? AttentionCode = null);

public interface IExternalParcelLifecycle
{
    Task<ExternalParcelState> ReadParcelAsync(ExternalIntegrationConnection connection, string parcelId, CancellationToken ct);
    Task SendParcelAsync(ExternalIntegrationConnection connection, string parcelId, bool posted, int? shippingMethod, string? trackingCode, CancellationToken ct);
}

public interface IIntegrationParcelCommandProcessor
{
    Task<IntegrationScenarioResult> ProcessAsync(IntegrationScenarioJob job, ExternalIntegrationConnection connection, CancellationToken ct);
}
