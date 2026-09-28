namespace Hyper.Integration.Domain.Features.Integrations;

// Collect only; discovery of an unmapped product is never permission to create it.
public interface IIntegrationProductPreparationCollector
{
    Task CollectAsync(OwnedIntegrationShop scope, long connectionId, int? hyperProductId, string? externalProductId, CancellationToken ct);
}
