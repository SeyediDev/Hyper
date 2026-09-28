using System.Text.Json;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed class IntegrationProductCreationProcessor(HyperIntegrationContext db,
    IIntegrationStrategyResolver strategies, IIntegrationPlatformCatalogPort catalog) : IIntegrationProductCreationProcessor
{
    public async Task<IntegrationScenarioResult> ProcessAsync(IntegrationScenarioJob job, ExternalIntegrationConnection connection, CancellationToken ct)
    {
        var row = await db.Set<IntegrationProductCreation>().AsNoTracking().SingleOrDefaultAsync(x =>
            x.ConnectionId == connection.Id && x.JobId == job.Id, ct);
        if (row is null || job.Trigger != IntegrationSyncTrigger.Manual)
            throw new IntegrationProviderException("CreationReceiptMissing", false);
        if (row.State == 3) return new(1, []);
        if (row.AccountIdentifier != connection.AccountIdentifier) return await Attention(row, "CreationConnectionChanged", ct);
        if (row.State is 1 or 4) return await Attention(row, row.ErrorCode ?? "CreationOutcomeUnknown", ct);
        var draft = JsonSerializer.Deserialize<ExternalProductDraft>(row.PayloadJson)
            ?? throw new IntegrationProviderException("InvalidCreationPayload", false);
        if (strategies.Resolve(connection.Provider, connection.CredentialType) is not IExternalProductCreator creator)
            return await Attention(row, "ProductCreationUnsupported", ct);
        if (row.State == 0)
        {
            var source = (await catalog.GetProductsAsync(connection.ShopId, connection.TenantId, ct))
                .SingleOrDefault(x => x.ProductId == row.HyperProductId);
            if (source is null || !source.IsEnabled || source.Name != draft.Name || source.Price != draft.PrimaryPrice)
                return await Attention(row, "CreationSourceChanged", ct);
            // Known credential/refresh failures before POST retain normal queue
            // retry semantics rather than creating an uncertain send marker.
            await creator.PrepareCreationAsync(connection, ct);
            // Commit the send marker BEFORE HTTP. A crash or lost response cannot
            // cause the recovered worker to issue a second non-idempotent POST.
            await using (var tx = await db.Database.BeginTransactionAsync(ct))
            {
                var current = await db.ExternalIntegrationConnections.FromSqlInterpolated(
                    $"SELECT * FROM dbo.ExternalIntegrationConnections WITH (UPDLOCK,HOLDLOCK) WHERE Id={connection.Id}")
                    .AsNoTracking().SingleAsync(ct);
                IntegrationConnectionReadiness.Validate(current, DateTime.UtcNow);
                if (current.AccountIdentifier != row.AccountIdentifier || current.ShopId != job.ShopId || current.TenantId != job.TenantId)
                {
                    await tx.RollbackAsync(ct);
                    return await Attention(row, "CreationConnectionChanged", ct);
                }
                if (await db.ExternalProductMappings.AnyAsync(x => x.ConnectionId == connection.Id && x.HyperProductId == row.HyperProductId, ct))
                {
                    await tx.RollbackAsync(ct);
                    return await Attention(row, "ProductMappingAlreadyExists", ct);
                }
                if (await db.Set<IntegrationProductCreation>().Where(x => x.Id == row.Id && x.State == 0)
                    .ExecuteUpdateAsync(s => s.SetProperty(x => x.State, (byte)1).SetProperty(x => x.UpdatedAtUtc, DateTime.UtcNow), ct) != 1)
                    throw new IntegrationProviderException("CreationStateChanged", false);
                await tx.CommitAsync(ct);
            }
            string externalId;
            try { externalId = await creator.CreateDraftAsync(connection, draft, ct); }
            catch (Exception e) when (e is IntegrationProviderException or HttpRequestException or OperationCanceledException or JsonException or InvalidOperationException)
            {
                // No raw upstream errors/response bodies in durable records.
                return await Attention(row, "CreationOutcomeUnknown", CancellationToken.None);
            }
            if (!int.TryParse(externalId, out var numericId) || numericId <= 0 || externalId != numericId.ToString(System.Globalization.CultureInfo.InvariantCulture))
                return await Attention(row, "CreationIdentityInvalid", ct);
            await db.Set<IntegrationProductCreation>().Where(x => x.Id == row.Id && x.State == 1)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.State, (byte)2).SetProperty(x => x.ExternalProductId, externalId)
                    .SetProperty(x => x.UpdatedAtUtc, DateTime.UtcNow), ct);
            row.ExternalProductId = externalId;
        }
        // A transient read failure retries only GET, using the persisted identity.
        var remote = await creator.ReadCreatedAsync(connection, row.ExternalProductId!, ct);
        if (remote.ExternalProductId != row.ExternalProductId || remote.VariantId is not null
            || remote.Title != draft.Name || remote.Price != draft.PrimaryPrice || remote.Inventory != 0)
            return await Attention(row, "CreationReadbackMismatch", ct);
        await using var completion = await db.Database.BeginTransactionAsync(ct);
        var completionConnection = await db.ExternalIntegrationConnections.FromSqlInterpolated(
            $"SELECT * FROM dbo.ExternalIntegrationConnections WITH (UPDLOCK,HOLDLOCK) WHERE Id={connection.Id}")
            .AsNoTracking().SingleAsync(ct);
        if (completionConnection.AccountIdentifier != row.AccountIdentifier
            || completionConnection.ShopId != job.ShopId || completionConnection.TenantId != job.TenantId)
        {
            await completion.RollbackAsync(ct);
            return await Attention(row, "CreationConnectionChanged", ct);
        }
        IntegrationConnectionReadiness.Validate(completionConnection, DateTime.UtcNow);
        if (await db.ExternalProductMappings.AnyAsync(x => x.ConnectionId == connection.Id
            && (x.HyperProductId == row.HyperProductId || x.ExternalProductId == row.ExternalProductId), ct))
        {
            await completion.RollbackAsync(ct);
            return await Attention(row, "CreationMappingConflict", ct);
        }
        var mapping = new ExternalProductMapping { ConnectionId = connection.Id, ShopId = connection.ShopId,
            HyperProductId = row.HyperProductId, ExternalProductId = row.ExternalProductId!, LastExternalPrice = remote.Price,
            LastExternalInventory = remote.Inventory, LastSyncAtUtc = DateTime.UtcNow };
        db.ExternalProductMappings.Add(mapping); await db.SaveChangesAsync(ct);
        if (await db.Set<IntegrationProductCreation>().Where(x => x.Id == row.Id && x.State == 2)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.State, (byte)3).SetProperty(x => x.MappingId, mapping.Id)
                .SetProperty(x => x.ErrorCode, (string?)null).SetProperty(x => x.UpdatedAtUtc, DateTime.UtcNow), ct) != 1)
            throw new IntegrationProviderException("CreationStateChanged", false);
        await completion.CommitAsync(ct);
        return new(1, []);
    }

    private async Task<IntegrationScenarioResult> Attention(IntegrationProductCreation row, string error, CancellationToken ct)
    {
        await db.Set<IntegrationProductCreation>().Where(x => x.Id == row.Id && x.State != 3)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.State, (byte)4).SetProperty(x => x.ErrorCode, error)
                .SetProperty(x => x.UpdatedAtUtc, DateTime.UtcNow), ct);
        return new(0, [new(row.HyperProductId, row.ExternalProductId, null, error)]);
    }
}
