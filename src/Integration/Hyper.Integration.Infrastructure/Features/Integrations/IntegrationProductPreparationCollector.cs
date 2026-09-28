using System.Globalization;
using System.Text.Json;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Integration.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed class IntegrationProductPreparationCollector(HyperIntegrationContext db) : IIntegrationProductPreparationCollector
{
    public async Task CollectAsync(OwnedIntegrationShop scope, long connectionId, int? hyperProductId, string? externalProductId, CancellationToken ct)
    {
        var source = hyperProductId?.ToString(CultureInfo.InvariantCulture) ?? externalProductId;
        if (string.IsNullOrWhiteSpace(source) || source.Length > 128 || source != source.Trim() || source.Any(char.IsControl))
            throw new IntegrationProviderException("InvalidPreparationIdentity", false);
        var direction = hyperProductId.HasValue ? ProductTransferDirection.ToPlatform : ProductTransferDirection.ToAccounting;
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        var c = await db.ExternalIntegrationConnections.FromSqlInterpolated($"SELECT * FROM dbo.ExternalIntegrationConnections WITH(UPDLOCK,HOLDLOCK) WHERE Id={connectionId}")
            .AsNoTracking().SingleOrDefaultAsync(ct);
        if (c is null || c.ShopId != scope.ShopId || c.TenantId != scope.TenantId) throw new InvalidOperationException("ConnectionScopeChanged");
        if (await db.Set<IntegrationProductPreparation>().AnyAsync(x => x.ConnectionId == connectionId && x.Direction == (byte)direction && x.SourceProductId == source, ct)) return;
        var input = new ProductPreparationInput(direction, source);
        var issue = new ProductPreparationIssue("Preparation", direction == ProductTransferDirection.ToPlatform ? "UnmappedProductRequiresMetadata" : "AccountingCreationNotAvailable");
        var row = new IntegrationProductPreparation { ConnectionId = connectionId, Direction = (byte)direction,
            SourceProductId = source, Revision = 1, InputJson = JsonSerializer.Serialize(input), PreparedJson = JsonSerializer.Serialize(input),
            IssuesJson = JsonSerializer.Serialize(new[] { issue }), UpdatedAtUtc = DateTime.UtcNow };
        db.Add(row); await db.SaveChangesAsync(ct);
        db.Add(new IntegrationProductPreparationHistory { PreparationId = row.Id, Revision = 1,
            AuditJson = JsonSerializer.Serialize(new ProductPreparationRevision(1, "catalog-discovery", row.UpdatedAtUtc,
                input, input, new(), [issue], [])) });
        await db.SaveChangesAsync(ct); await tx.CommitAsync(ct);
    }
}
