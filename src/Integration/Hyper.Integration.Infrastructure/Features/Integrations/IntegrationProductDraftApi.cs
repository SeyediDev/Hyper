using System.Text.Json;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Integration.Contracts;
using Microsoft.EntityFrameworkCore;
using Provider = Hyper.Integration.Domain.Entities.Integrations.IntegrationProvider;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed class IntegrationProductDraftApi(HyperIntegrationContext db, IIntegrationScenarioQueue queue,
    IIntegrationPlatformCatalogPort catalog) : IIntegrationProductDraftApi
{
    public async Task<IntegrationProductDraftStatus?> ReadAsync(IntegrationConnectionCommandRequest scope, Guid requestId, CancellationToken ct)
    {
        if (!await db.ExternalIntegrationConnections.AnyAsync(x => x.Id == scope.ConnectionId
            && x.ShopId == scope.ShopId && x.TenantId == scope.TenantId, ct)) return null;
        var row = await db.Set<IntegrationProductCreation>().AsNoTracking()
            .SingleOrDefaultAsync(x => x.ConnectionId == scope.ConnectionId && x.RequestId == requestId, ct);
        return row is null ? null : await Status(row, ct);
    }

    public async Task<IntegrationProductDraftStatus?> StartAsync(IntegrationProductDraftRequest request, CancellationToken ct)
    {
        if (request.ShopId <= 0 || request.ConnectionId <= 0 || request.RequestId == Guid.Empty
            || request.HyperProductId <= 0 || string.IsNullOrWhiteSpace(request.TenantId) || request.TenantId.Length > 30
            || request.CategoryId <= 0 || request.PreparationDays is null or < 0 || request.PackageWeight <= 0
            || request.PhotoId is <= 0 || request.Description?.Length > 10000)
            throw new ArgumentException("InvalidProductDraft");
        // The readiness workflow may own the transaction so preparation, audit,
        // receipt and queue are committed together. Standalone callers still own one.
        await using var tx = db.Database.CurrentTransaction is null ? await db.Database.BeginTransactionAsync(ct) : null;
        var connection = await db.ExternalIntegrationConnections.FromSqlInterpolated(
            $"SELECT * FROM dbo.ExternalIntegrationConnections WITH (UPDLOCK,HOLDLOCK) WHERE Id={request.ConnectionId}")
            .AsNoTracking().SingleOrDefaultAsync(ct);
        if (connection is null || connection.ShopId != request.ShopId || connection.TenantId != request.TenantId) return null;
        var requestJson = JsonSerializer.Serialize(request);
        var existing = await db.Set<IntegrationProductCreation>().AsNoTracking().SingleOrDefaultAsync(x =>
            x.ConnectionId == connection.Id && x.RequestId == request.RequestId, ct);
        if (existing is not null)
        {
            if (existing.RequestJson != requestJson) throw new InvalidOperationException("DraftRequestConflict");
            var status = await Status(existing, ct);
            if (tx is not null) await tx.CommitAsync(ct);
            return status;
        }
        IntegrationConnectionReadiness.Validate(connection, DateTime.UtcNow);
        if (connection.Provider != Provider.Basalam) throw new InvalidOperationException("DraftProviderUnsupported");
        if (await db.Set<IntegrationProductCreation>().AnyAsync(x => x.ConnectionId == connection.Id
                && x.HyperProductId == request.HyperProductId, ct)
            || await db.ExternalProductMappings.AnyAsync(x => x.ConnectionId == connection.Id && x.HyperProductId == request.HyperProductId, ct))
            throw new InvalidOperationException("ProductCreationAlreadyExists");
        var source = (await catalog.GetProductsAsync(request.ShopId, request.TenantId, ct))
            .SingleOrDefault(x => x.ProductId == request.HyperProductId);
        if (source is null || !source.IsEnabled) throw new InvalidOperationException("SourceProductUnavailable");
        if (new ExternalProductUpdate("draft", null, source.Name, source.Price).ValidationError() is not null)
            throw new ArgumentException("InvalidSourceProduct");
        var draft = new ExternalProductDraft(source.Name, checked((long)source.Price), request.CategoryId,
            request.PreparationDays.Value, request.PackageWeight, request.Description, request.PhotoId);
        var row = new IntegrationProductCreation { ConnectionId = connection.Id, RequestId = request.RequestId,
            HyperProductId = request.HyperProductId, AccountIdentifier = connection.AccountIdentifier,
            RequestJson = requestJson, PayloadJson = JsonSerializer.Serialize(draft),
            CreatedAtUtc = DateTime.UtcNow, UpdatedAtUtc = DateTime.UtcNow };
        db.Add(row); await db.SaveChangesAsync(ct);
        row.JobId = await queue.EnqueueAsync(new(request.ShopId, request.TenantId), connection.Id,
            new("product-create:" + request.RequestId.ToString("N"), IntegrationSyncItem.ProductCreation, IntegrationSyncTrigger.Manual), ct);
        await db.SaveChangesAsync(ct);
        if (tx is not null) await tx.CommitAsync(ct);
        return await Status(row, ct);
    }

    private async Task<IntegrationProductDraftStatus> Status(IntegrationProductCreation row, CancellationToken ct)
    {
        var job = row.JobId is null ? null : await db.IntegrationScenarioJobs.AsNoTracking().SingleOrDefaultAsync(x => x.Id == row.JobId, ct);
        var state = row.State switch { 1 => "Sending", 2 => "Verifying", 3 => "Completed", 4 => "NeedsAttention", _ => "Pending" };
        if (job?.Status is IntegrationScenarioStatus.DeadLetter or IntegrationScenarioStatus.NeedsAttention) state = "NeedsAttention";
        return new(row.RequestId, row.JobId, state, row.ErrorCode ?? job?.ErrorCode, row.ExternalProductId, row.MappingId);
    }
}
