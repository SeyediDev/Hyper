namespace Hyper.Integration.Contracts;

public enum IntegrationVersionSource : byte { Unassigned = 0, AccountingEvents = 1, InventoryCapture = 2 }
public sealed record IntegrationVersionSourceChange(IntegrationVersionSource Source,
    IntegrationVersionSource ExpectedSource, long ExpectedLastVersion);
public sealed record IntegrationVersionSourceResult(string Status, IntegrationVersionSource Source,
    long LastVersion, long VersionFloor, string? ErrorCode = null);

public interface IIntegrationVersionSourceApi
{
    Task<IntegrationVersionSourceResult?> ReadAsync(IntegrationConnectionCommandRequest scope, long mappingId, CancellationToken ct);
    Task<IntegrationVersionSourceResult?> ChangeAsync(IntegrationConnectionCommandRequest scope, long mappingId,
        IntegrationVersionSourceChange change, CancellationToken ct);
}
