using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Integration.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed class IntegrationVersionSourceApi(HyperIntegrationContext db) : IIntegrationVersionSourceApi
{
    public Task<IntegrationVersionSourceResult?> ReadAsync(IntegrationConnectionCommandRequest scope, long mappingId, CancellationToken ct) =>
        ExecuteAsync(scope, mappingId, null, ct);

    public Task<IntegrationVersionSourceResult?> ChangeAsync(IntegrationConnectionCommandRequest scope, long mappingId,
        IntegrationVersionSourceChange change, CancellationToken ct) => ExecuteAsync(scope, mappingId, change, ct);

    private async Task<IntegrationVersionSourceResult?> ExecuteAsync(IntegrationConnectionCommandRequest scope, long mappingId,
        IntegrationVersionSourceChange? change, CancellationToken ct)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        var mapping = await db.ExternalProductMappings.FromSqlInterpolated($"SELECT * FROM dbo.ExternalProductMappings WITH (UPDLOCK,HOLDLOCK) WHERE Id={mappingId}")
            .AsNoTracking().SingleOrDefaultAsync(ct);
        if (mapping is null || !mapping.IsActive || mapping.ConnectionId != scope.ConnectionId || mapping.ShopId != scope.ShopId
            || !await db.ExternalIntegrationConnections.AnyAsync(c => c.Id == scope.ConnectionId && c.ShopId == scope.ShopId
                && c.TenantId == scope.TenantId && c.IsEnabled, ct)) return null;
        var owner = await db.Set<IntegrationVersionOwnership>().AsNoTracking().SingleOrDefaultAsync(x => x.MappingId == mappingId, ct);
        var source = owner is null ? IntegrationVersionSource.Unassigned : (IntegrationVersionSource)owner.Source;
        var lastVersion = await db.IntegrationOutbox.Where(x => x.MappingId == mappingId).MaxAsync(x => (long?)x.SourceVersion, ct) ?? 0;
        lastVersion = Math.Max(lastVersion, owner?.VersionFloor ?? 0);
        IntegrationVersionSourceResult Result(string status, string? error = null) => new(status, source, lastVersion, owner?.VersionFloor ?? 0, error);
        if (change is null) return Result("Current");
        if (change.Source is not (IntegrationVersionSource.AccountingEvents or IntegrationVersionSource.InventoryCapture)
            || !Enum.IsDefined(change.ExpectedSource) || change.ExpectedLastVersion < 0) return Result("Rejected", "InvalidVersionSource");
        if (change.ExpectedSource != source || change.ExpectedLastVersion != lastVersion)
            return Result("Conflict", "VersionSourceSnapshotChanged");
        if (change.Source == source) return Result("Unchanged");
        if (await db.IntegrationOutbox.AnyAsync(x => x.MappingId == mappingId && (x.Status == 0 || x.Status == 1), ct))
            return Result("Conflict", "VersionSourceHasPendingMessages");
        if (owner is null)
            db.Set<IntegrationVersionOwnership>().Add(new() { MappingId = mappingId, Source = (byte)change.Source,
                VersionFloor = lastVersion, UpdatedAtUtc = DateTime.UtcNow });
        else
            await db.Set<IntegrationVersionOwnership>().Where(x => x.MappingId == mappingId).ExecuteUpdateAsync(s =>
                s.SetProperty(x => x.Source, (byte)change.Source).SetProperty(x => x.VersionFloor, lastVersion)
                 .SetProperty(x => x.UpdatedAtUtc, DateTime.UtcNow), ct);
        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return new("Changed", change.Source, lastVersion, lastVersion);
    }
}

internal static class IntegrationVersionSourceGuard
{
    // Caller must own a transaction and the mapping's UPDLOCK/HOLDLOCK.
    public static async Task<long> RequireAsync(HyperIntegrationContext db, long mappingId,
        IntegrationVersionSource source, bool claim, CancellationToken ct)
    {
        if (db.Database.CurrentTransaction is null) throw new InvalidOperationException("VersionSourceTransactionRequired");
        var owner = await db.Set<IntegrationVersionOwnership>().AsNoTracking().SingleOrDefaultAsync(x => x.MappingId == mappingId, ct);
        if (owner is not null)
        {
            if (owner.Source != (byte)source) throw new InvalidOperationException("VersionSourceConflict");
            return owner.VersionFloor;
        }
        // Do not guess which old producer wrote an existing stream.
        if (await db.IntegrationOutbox.AnyAsync(x => x.MappingId == mappingId, ct))
            throw new InvalidOperationException("VersionSourceUnassigned");
        if (claim) db.Set<IntegrationVersionOwnership>().Add(new() { MappingId = mappingId, Source = (byte)source,
            UpdatedAtUtc = DateTime.UtcNow });
        return 0;
    }
}
