namespace Hyper.Integration.Domain.Features.Integrations;

public interface IIntegrationCapturedInventoryOutbox
{
    Task<long> EnqueueCapturedInventoryAsync(long connectionId, long mappingId, long sourceVersion,
        decimal quantity, CancellationToken ct);
}
