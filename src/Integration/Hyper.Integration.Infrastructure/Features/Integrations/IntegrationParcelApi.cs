using System.Globalization;
using System.Text.Json;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Integration.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed class IntegrationParcelApi(HyperIntegrationContext db, IIntegrationScenarioQueue queue) : IIntegrationParcelApi
{
    public const string EventPrefix = "parcel-command:";
    public async Task<ParcelCommandStatus?> ReadAsync(IntegrationConnectionCommandRequest scope, Guid requestId, CancellationToken ct)
    {
        if (!await db.ExternalIntegrationConnections.AnyAsync(x => x.Id == scope.ConnectionId && x.ShopId == scope.ShopId && x.TenantId == scope.TenantId, ct)) return null;
        var row = await db.Set<IntegrationParcelCommand>().AsNoTracking().SingleOrDefaultAsync(x => x.ConnectionId == scope.ConnectionId && x.RequestId == requestId, ct);
        return row is null ? null : await Status(row, ct);
    }

    public async Task<ParcelCommandStatus?> StartAsync(IntegrationConnectionCommandRequest scope, ParcelCommandRequest request, string actor, CancellationToken ct)
    {
        static bool Identity(string value) => int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var n)
            && n > 0 && value == n.ToString(CultureInfo.InvariantCulture);
        if (request.RequestId == Guid.Empty || !Identity(request.ParcelId) || !Identity(request.OrderId) || !Enum.IsDefined(request.Target)
            || string.IsNullOrWhiteSpace(actor) || actor.Length > 256
            || request.TrackingCode is { } code && (string.IsNullOrWhiteSpace(code) || code != code.Trim() || code.Length > 256 || code.Any(char.IsControl))
            || request.Target == ParcelCommandTarget.Posted && request.ShippingMethod is null or <= 0
            || request.Target == ParcelCommandTarget.Preparing && (request.ShippingMethod is not null || request.TrackingCode is not null))
            throw new ArgumentException("InvalidParcelCommand");
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        var connection = await db.ExternalIntegrationConnections.FromSqlInterpolated($"SELECT * FROM dbo.ExternalIntegrationConnections WITH(UPDLOCK,HOLDLOCK) WHERE Id={scope.ConnectionId}")
            .AsNoTracking().SingleOrDefaultAsync(ct);
        if (connection is null || connection.ShopId != scope.ShopId || connection.TenantId != scope.TenantId) return null;
        var json = JsonSerializer.Serialize(request);
        var previous = await db.Set<IntegrationParcelCommand>().AsNoTracking().SingleOrDefaultAsync(x => x.ConnectionId == connection.Id && x.RequestId == request.RequestId, ct);
        if (previous is not null)
        {
            if (previous.RequestJson != json) throw new InvalidOperationException("ParcelRequestConflict");
            return await Status(previous, ct);
        }
        IntegrationConnectionReadiness.Validate(connection, DateTime.UtcNow);
        if (connection.Provider != Hyper.Integration.Domain.Entities.Integrations.IntegrationProvider.Basalam)
            throw new InvalidOperationException("ParcelProviderUnsupported");
        if (!await Bound(db, connection, request, ct)) throw new InvalidOperationException("ParcelInvoiceMappingRequired");
        if (await db.Set<IntegrationParcelCommand>().AnyAsync(x => x.ConnectionId == connection.Id && x.ParcelId == request.ParcelId && x.Target == (byte)request.Target, ct))
            throw new InvalidOperationException("ParcelCommandAlreadyExists");
        var row = new IntegrationParcelCommand { ConnectionId = connection.Id, RequestId = request.RequestId, ParcelId = request.ParcelId,
            Target = (byte)request.Target, RequestJson = json, AccountIdentifier = connection.AccountIdentifier, Actor = actor,
            CreatedAtUtc = DateTime.UtcNow, UpdatedAtUtc = DateTime.UtcNow };
        db.Add(row); await db.SaveChangesAsync(ct);
        row.JobId = await queue.EnqueueAsync(new(scope.ShopId, scope.TenantId), connection.Id,
            new(EventPrefix + request.RequestId.ToString("N"), IntegrationSyncItem.Sale, IntegrationSyncTrigger.Manual), ct);
        await db.SaveChangesAsync(ct); await tx.CommitAsync(ct);
        return await Status(row, ct);
    }

    internal static Task<bool> Bound(HyperIntegrationContext db, ExternalIntegrationConnection c, ParcelCommandRequest r, CancellationToken ct) =>
        db.ExternalOrderMappings.AnyAsync(x => x.ConnectionId == c.Id && x.ShopId == c.ShopId && x.ExternalOrderId == r.OrderId
            && x.ExternalParcelId == r.ParcelId && x.HyperSaleOrderId > 0, ct);

    private async Task<ParcelCommandStatus> Status(IntegrationParcelCommand row, CancellationToken ct)
    {
        var job = await db.IntegrationScenarioJobs.AsNoTracking().SingleOrDefaultAsync(x => x.Id == row.JobId, ct);
        var status = row.State switch { 1 => "Verifying", 2 => "Completed", 3 => "NeedsAttention", _ => "Pending" };
        if (row.State != 2 && job?.Status is IntegrationScenarioStatus.DeadLetter or IntegrationScenarioStatus.NeedsAttention) status = "NeedsAttention";
        return new(row.RequestId, row.JobId, status, row.ErrorCode ?? job?.ErrorCode);
    }
}
