using System.Text.Json;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Integration.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed class IntegrationParcelCommandProcessor(HyperIntegrationContext db, IIntegrationStrategyResolver strategies) : IIntegrationParcelCommandProcessor
{
    public async Task<IntegrationScenarioResult> ProcessAsync(IntegrationScenarioJob job, ExternalIntegrationConnection connection, CancellationToken ct)
    {
        var row = await db.Set<IntegrationParcelCommand>().AsNoTracking().SingleOrDefaultAsync(x => x.ConnectionId == connection.Id && x.JobId == job.Id, ct)
            ?? throw new IntegrationProviderException("ParcelCommandReceiptMissing", false);
        if (row.State == 2) return new(1, []);
        if (row.State == 3) return new(1, [new(null, null, null, row.ErrorCode ?? "ParcelCommandNeedsReview")]);
        var request = JsonSerializer.Deserialize<ParcelCommandRequest>(row.RequestJson)!;
        if (!await ScopeValid(row, job, request, ct)) return await Attention(row, "ParcelCommandScopeChanged", ct);
        if (strategies.Resolve(connection.Provider, connection.CredentialType) is not IExternalParcelLifecycle adapter)
            return await Attention(row, "ParcelLifecycleUnavailable", ct);
        var remote = await adapter.ReadParcelAsync(connection, row.ParcelId, ct);
        if (remote.OrderId != request.OrderId || remote.ParcelId != request.ParcelId)
            return await Attention(row, "ParcelOrderMismatch", ct);
        if (Matches(remote, request)) return await Complete(row, job, request, ct);
        // A recovery/readback never repeats POST, even with a new worker lease.
        if (row.State == 1) throw new IntegrationProviderException("ParcelCommandNotConfirmed", true);
        if (request.Target == ParcelCommandTarget.Preparing && remote.State != "new"
            || request.Target == ParcelCommandTarget.Posted && remote.State != "preparing")
            return await Attention(row, "ParcelTransitionNeedsReview", ct);
        if (request.Target == ParcelCommandTarget.Posted && remote.ShippingMethod != request.ShippingMethod)
            return await Attention(row, "ParcelShippingMethodChanged", ct);
        await using (var tx = await db.Database.BeginTransactionAsync(ct))
        {
            // Same connection serialization point as intake and scope changes.
            await db.ExternalIntegrationConnections.FromSqlInterpolated($"SELECT * FROM dbo.ExternalIntegrationConnections WITH(UPDLOCK,HOLDLOCK) WHERE Id={connection.Id}")
                .AsNoTracking().SingleAsync(ct);
            if (!await ScopeValid(row, job, request, ct))
            { await tx.RollbackAsync(ct); return await Attention(row, "ParcelCommandScopeChanged", ct); }
            if (await db.Set<IntegrationParcelCommand>().Where(x => x.Id == row.Id && x.State == 0)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.State, (byte)1).SetProperty(x => x.UpdatedAtUtc, DateTime.UtcNow), ct) != 1)
                throw new IntegrationProviderException("ParcelCommandStateChanged", false);
            await tx.CommitAsync(ct);
        }
        try { await adapter.SendParcelAsync(connection, row.ParcelId, request.Target == ParcelCommandTarget.Posted, request.ShippingMethod, request.TrackingCode, ct); }
        catch (Exception e) when (e is IntegrationProviderException or HttpRequestException or OperationCanceledException or JsonException)
        { throw new IntegrationProviderException("ParcelCommandVerificationPending", true); }
        remote = await adapter.ReadParcelAsync(connection, row.ParcelId, ct);
        if (!Matches(remote, request)) throw new IntegrationProviderException("ParcelCommandNotConfirmed", true);
        return await Complete(row, job, request, ct);
    }

    private async Task<bool> ScopeValid(IntegrationParcelCommand row, IntegrationScenarioJob job, ParcelCommandRequest request, CancellationToken ct)
    {
        var c = await db.ExternalIntegrationConnections.AsNoTracking().SingleOrDefaultAsync(x => x.Id == row.ConnectionId, ct);
        if (c is null || c.Provider != Hyper.Integration.Domain.Entities.Integrations.IntegrationProvider.Basalam
            || c.AccountIdentifier != row.AccountIdentifier || c.ShopId != job.ShopId || c.TenantId != job.TenantId) return false;
        IntegrationConnectionReadiness.Validate(c, DateTime.UtcNow);
        return await IntegrationParcelApi.Bound(db, c, request, ct);
    }
    private static bool Matches(ExternalParcelState remote, ParcelCommandRequest request) =>
        remote.OrderId == request.OrderId && remote.ParcelId == request.ParcelId &&
        (request.Target == ParcelCommandTarget.Preparing ? remote.State is "preparing" or "shipped" or "delivered"
            : remote.State is "shipped" or "delivered" && remote.ShippingMethod == request.ShippingMethod
                && (request.TrackingCode is null || remote.TrackingCode == request.TrackingCode));

    private async Task<IntegrationScenarioResult> Complete(IntegrationParcelCommand row, IntegrationScenarioJob job, ParcelCommandRequest request, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        await db.ExternalIntegrationConnections.FromSqlInterpolated($"SELECT * FROM dbo.ExternalIntegrationConnections WITH(UPDLOCK,HOLDLOCK) WHERE Id={row.ConnectionId}")
            .AsNoTracking().SingleAsync(ct);
        if (!await ScopeValid(row, job, request, ct))
        { await tx.RollbackAsync(ct); return await Attention(row, "ParcelCommandScopeChanged", ct); }
        await db.Set<IntegrationParcelCommand>().Where(x => x.Id == row.Id && x.State < 2)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.State, (byte)2).SetProperty(x => x.ErrorCode, (string?)null)
                .SetProperty(x => x.UpdatedAtUtc, DateTime.UtcNow), ct);
        await tx.CommitAsync(ct); return new(1, []);
    }
    private async Task<IntegrationScenarioResult> Attention(IntegrationParcelCommand row, string code, CancellationToken ct)
    {
        await db.Set<IntegrationParcelCommand>().Where(x => x.Id == row.Id && x.State != 2)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.State, (byte)3).SetProperty(x => x.ErrorCode, code)
                .SetProperty(x => x.UpdatedAtUtc, DateTime.UtcNow), ct);
        return new(1, [new(null, null, null, code)]);
    }
}
