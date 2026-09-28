namespace Hyper.Integration.Contracts;

public enum ParcelCommandTarget : byte { Preparing = 1, Posted = 2 }
public sealed record ParcelCommandRequest(Guid RequestId, string ParcelId, string OrderId,
    ParcelCommandTarget Target, int? ShippingMethod = null, string? TrackingCode = null);
public sealed record ParcelCommandStatus(Guid RequestId, long? JobId, string Status, string? ErrorCode);
public interface IIntegrationParcelApi
{
    Task<ParcelCommandStatus?> StartAsync(IntegrationConnectionCommandRequest scope, ParcelCommandRequest request, string actor, CancellationToken ct);
    Task<ParcelCommandStatus?> ReadAsync(IntegrationConnectionCommandRequest scope, Guid requestId, CancellationToken ct);
}
