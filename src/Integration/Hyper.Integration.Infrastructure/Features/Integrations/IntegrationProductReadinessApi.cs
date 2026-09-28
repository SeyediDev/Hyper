using System.Globalization;
using System.Text.Json;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Integration.Contracts;
using Microsoft.EntityFrameworkCore;
using Provider = Hyper.Integration.Domain.Entities.Integrations.IntegrationProvider;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed class IntegrationProductReadinessApi(HyperIntegrationContext db, IIntegrationProductDraftApi drafts,
    IIntegrationPlatformCatalogPort catalog) : IIntegrationProductReadinessApi
{
    private async Task<ExternalIntegrationConnection?> Connection(IntegrationConnectionCommandRequest s, bool locked, CancellationToken ct)
    {
        IQueryable<ExternalIntegrationConnection> query = locked
            ? db.ExternalIntegrationConnections.FromSqlInterpolated($"SELECT * FROM dbo.ExternalIntegrationConnections WITH (UPDLOCK,HOLDLOCK) WHERE Id={s.ConnectionId}")
            : db.ExternalIntegrationConnections;
        return await query.AsNoTracking().SingleOrDefaultAsync(x => x.Id == s.ConnectionId && x.ShopId == s.ShopId && x.TenantId == s.TenantId, ct);
    }
    private static void Direction(ProductTransferDirection direction)
    {
        if (!Enum.IsDefined(direction)) throw new ArgumentException("InvalidDirection");
    }
    private static void Actor(string actor)
    {
        if (string.IsNullOrWhiteSpace(actor) || actor.Length > 256) throw new ArgumentException("InvalidActor");
    }
    public async Task<ProductPreparationPolicy?> PolicyAsync(IntegrationConnectionCommandRequest scope, ProductTransferDirection direction, CancellationToken ct)
    {
        Direction(direction);
        if (await Connection(scope, false, ct) is null) return null;
        return await Policy(scope.ConnectionId, direction, ct);
    }
    private async Task<ProductPreparationPolicy> Policy(long connection, ProductTransferDirection direction, CancellationToken ct)
    {
        var row = await db.Set<IntegrationProductPreparationPolicy>().AsNoTracking()
            .SingleOrDefaultAsync(x => x.ConnectionId == connection && x.Direction == (byte)direction, ct);
        return row is null ? new() : JsonSerializer.Deserialize<ProductPreparationPolicy>(row.PolicyJson)!;
    }
    public async Task<bool> SetPolicyAsync(IntegrationConnectionCommandRequest scope, ProductTransferDirection direction,
        ProductPreparationPolicy policy, string actor, CancellationToken ct)
    {
        Direction(direction); Actor(actor);
        if (!Enum.IsDefined(policy.Mode)) throw new ArgumentException("InvalidMode");
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        if (await Connection(scope, true, ct) is null) return false;
        var row = await db.Set<IntegrationProductPreparationPolicy>().SingleOrDefaultAsync(x => x.ConnectionId == scope.ConnectionId && x.Direction == (byte)direction, ct);
        if (row is null) { row = new() { ConnectionId = scope.ConnectionId, Direction = (byte)direction }; db.Add(row); }
        row.PolicyJson = JsonSerializer.Serialize(policy); row.UpdatedBy = actor; row.UpdatedAtUtc = DateTime.UtcNow;
        await db.SaveChangesAsync(ct); await tx.CommitAsync(ct); return true;
    }
    public async Task<ProductPreparationItem?> PrepareAsync(IntegrationConnectionCommandRequest scope, ProductPreparationInput input,
        int expectedRevision, string actor, CancellationToken ct)
    {
        Direction(input.Direction); Actor(actor);
        if (string.IsNullOrWhiteSpace(input.SourceProductId) || input.SourceProductId.Length > 128
            || input.SourceProductId != input.SourceProductId.Trim() || input.SourceProductId.Any(char.IsControl)
            || input.Description?.Length > 50000 || expectedRevision < 0) throw new ArgumentException("InvalidPreparationInput");
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        var connection = await Connection(scope, true, ct);
        if (connection is null) return null;
        var json = JsonSerializer.Serialize(input);
        var row = await db.Set<IntegrationProductPreparation>().SingleOrDefaultAsync(x => x.ConnectionId == scope.ConnectionId
            && x.Direction == (byte)input.Direction && x.SourceProductId == input.SourceProductId, ct);
        if (row is not null && (row.JobId is not null || expectedRevision != row.Revision))
        {
            if (row.InputJson != json || (row.JobId is null && expectedRevision != row.Revision - 1))
                throw new InvalidOperationException("PreparationRevisionConflict");
            return await Item(scope, row, ct);
        }
        if (row is null && expectedRevision != 0) throw new InvalidOperationException("PreparationRevisionConflict");
        var policy = await Policy(scope.ConnectionId, input.Direction, ct);
        var issues = new List<ProductPreparationIssue>(); var changes = new List<ProductPreparationChange>();
        var prepared = input;
        if (policy.Mode == ProductPreparationMode.Adapt && input.Direction == ProductTransferDirection.ToPlatform
            && connection.Provider == Provider.Basalam && input.Description is { } description)
        {
            if (policy.TrimDescription && description != description.Trim())
            {
                changes.Add(new("Description", "TrimWhitespace", description, description.Trim())); description = description.Trim();
            }
            if (policy.TruncateDescription && description.Length > 10000)
            {
                // Do not split a UTF-16 surrogate pair at the destination boundary.
                var length = char.IsHighSurrogate(description[9999]) ? 9999 : 10000;
                changes.Add(new("Description", "TruncateTo10000", description, description[..length])); description = description[..length];
            }
            prepared = prepared with { Description = description };
        }
        if (input.Direction == ProductTransferDirection.ToAccounting)
            issues.Add(new("Destination", "AccountingCreationNotAvailable"));
        else if (connection.Provider != Provider.Basalam)
            issues.Add(new("Destination", "PlatformCreationNotAvailable"));
        else
        {
            if (prepared.CategoryId is null or <= 0) issues.Add(new("CategoryId", "RequiredPositiveCategory"));
            if (prepared.PreparationDays is null or < 0) issues.Add(new("PreparationDays", "RequiredNonnegativeDays"));
            if (prepared.PackageWeight is null or <= 0) issues.Add(new("PackageWeight", "RequiredPositiveWeight"));
            if (prepared.PhotoId is <= 0) issues.Add(new("PhotoId", "InvalidPhoto"));
            if (prepared.Description?.Length > 10000) issues.Add(new("Description", "DescriptionTooLong"));
            if (!int.TryParse(input.SourceProductId, NumberStyles.None, CultureInfo.InvariantCulture, out var productId)
                || productId <= 0 || input.SourceProductId != productId.ToString(CultureInfo.InvariantCulture))
                issues.Add(new("SourceProductId", "InvalidAccountingIdentity"));
            else
            {
                var source = (await catalog.GetProductsAsync(scope.ShopId, scope.TenantId, ct)).SingleOrDefault(x => x.ProductId == productId);
                if (source is null || !source.IsEnabled) issues.Add(new("Source", "SourceProductUnavailable"));
                else if (new ExternalProductUpdate("draft", null, source.Name, source.Price).ValidationError() is { } error)
                    issues.Add(new("Source", error));
                if (await db.ExternalProductMappings.AnyAsync(x => x.ConnectionId == scope.ConnectionId && x.HyperProductId == productId, ct))
                    issues.Add(new("Mapping", "ProductAlreadyMapped"));
                if (await db.Set<IntegrationProductCreation>().AnyAsync(x => x.ConnectionId == scope.ConnectionId && x.HyperProductId == productId, ct))
                    issues.Add(new("Creation", "ExistingCreationRequiresReview"));
            }
            try { IntegrationConnectionReadiness.Validate(connection, DateTime.UtcNow); }
            catch (InvalidOperationException) { issues.Add(new("Connection", "ConnectionNotReady")); }
        }
        if (row is null)
        {
            row = new() { ConnectionId = scope.ConnectionId, Direction = (byte)input.Direction, SourceProductId = input.SourceProductId };
            db.Add(row);
        }
        row.Revision++; row.InputJson = json; row.PreparedJson = JsonSerializer.Serialize(prepared);
        row.IssuesJson = JsonSerializer.Serialize(issues); row.ChangesJson = JsonSerializer.Serialize(changes);
        row.WasAdapted = changes.Count > 0; row.UpdatedAtUtc = DateTime.UtcNow;
        if (issues.Count == 0)
        {
            var requestId = Guid.NewGuid();
            var status = await drafts.StartAsync(new(scope.ShopId, scope.TenantId, scope.ConnectionId, requestId,
                int.Parse(input.SourceProductId, CultureInfo.InvariantCulture), prepared.CategoryId!.Value,
                prepared.PreparationDays, prepared.PackageWeight!.Value, prepared.Description, prepared.PhotoId), ct)
                ?? throw new InvalidOperationException("ConnectionChanged");
            row.DraftRequestId = requestId; row.JobId = status.JobId;
        }
        await db.SaveChangesAsync(ct);
        db.Add(new IntegrationProductPreparationHistory { PreparationId = row.Id, Revision = row.Revision,
            AuditJson = JsonSerializer.Serialize(new ProductPreparationRevision(row.Revision, actor, row.UpdatedAtUtc,
                input, prepared, policy, issues, changes)) });
        await db.SaveChangesAsync(ct); await tx.CommitAsync(ct);
        return await Item(scope, row, ct);
    }
    private async Task<ProductPreparationItem> Item(IntegrationConnectionCommandRequest scope, IntegrationProductPreparation row, CancellationToken ct)
    {
        var status = row.DraftRequestId is { } request ? await drafts.ReadAsync(scope, request, ct) : null;
        var issues = JsonSerializer.Deserialize<List<ProductPreparationIssue>>(row.IssuesJson)!;
        if (status?.ErrorCode is { } code) issues.Add(new("Execution", code));
        var history = await db.Set<IntegrationProductPreparationHistory>().AsNoTracking().Where(x => x.PreparationId == row.Id)
            .OrderBy(x => x.Revision).Select(x => x.AuditJson).ToListAsync(ct);
        return new(row.Id, row.Revision, JsonSerializer.Deserialize<ProductPreparationInput>(row.InputJson)!,
            JsonSerializer.Deserialize<ProductPreparationInput>(row.PreparedJson)!, status?.Status ?? "NeedsAttention", issues,
            JsonSerializer.Deserialize<List<ProductPreparationChange>>(row.ChangesJson)!, row.DraftRequestId, row.JobId,
            status?.ExternalProductId, history.Select(x => JsonSerializer.Deserialize<ProductPreparationRevision>(x)!).ToList());
    }
    public async Task<ProductPreparationBoard?> BoardAsync(IntegrationConnectionCommandRequest scope, int skip, int take, CancellationToken ct)
    {
        if (skip < 0 || take is < 1 or > 100) throw new ArgumentException("InvalidPage");
        if (await Connection(scope, false, ct) is null) return null;
        var rows = db.Set<IntegrationProductPreparation>().AsNoTracking().Where(x => x.ConnectionId == scope.ConnectionId);
        var summary = from p in rows
            join creation in db.Set<IntegrationProductCreation>() on new { p.ConnectionId, RequestId = p.DraftRequestId }
                equals new { creation.ConnectionId, RequestId = (Guid?)creation.RequestId } into creations
            from c in creations.DefaultIfEmpty()
            join job in db.IntegrationScenarioJobs on p.JobId equals (long?)job.Id into jobs
            from j in jobs.DefaultIfEmpty()
            select new { p.WasAdapted, Completed = c != null && c.State == 3,
                Attention = p.JobId == null || c == null || c.State == 4 || j == null
                    || j.Status == IntegrationScenarioStatus.NeedsAttention || j.Status == IntegrationScenarioStatus.DeadLetter };
        // One aggregate query avoids mutually inconsistent totals during worker completion.
        var totals = await summary.GroupBy(x => 1).Select(g => new { Total = g.Count(),
            Completed = g.Count(x => x.Completed), Adapted = g.Count(x => x.Completed && x.WasAdapted),
            Attention = g.Count(x => !x.Completed && x.Attention) }).SingleOrDefaultAsync(ct);
        var total = totals?.Total ?? 0; var completed = totals?.Completed ?? 0;
        var adapted = totals?.Adapted ?? 0; var attention = totals?.Attention ?? 0;
        var page = await rows.OrderByDescending(x => x.UpdatedAtUtc).ThenByDescending(x => x.Id).Skip(skip).Take(take).ToListAsync(ct);
        var items = new List<ProductPreparationItem>();
        foreach (var row in page) items.Add(await Item(scope, row, ct));
        return new(total, attention, total - completed - attention, completed, adapted, items);
    }
}
