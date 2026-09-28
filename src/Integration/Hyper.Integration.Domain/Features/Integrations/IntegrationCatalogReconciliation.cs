namespace Hyper.Integration.Domain.Features.Integrations;

// Optional capability of the accounting-catalog capture path. Both operations
// use the existing capture-owned mapping version stream, not a second producer.
public interface IIntegrationCatalogReconciliation
{
    Task<long?> ReconcileProductAsync(long connectionId, long mappingId, CancellationToken ct);
    Task<long?> ReconcileInventoryAsync(long connectionId, long mappingId, CancellationToken ct);
}

public interface IIntegrationCapturedProductOutbox
{
    Task<long> EnqueueCapturedProductAsync(long connectionId, long mappingId, long sourceVersion,
        ExternalProductUpdate update, CancellationToken ct);
}
