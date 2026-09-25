using System.Data;
using System.Text.Json;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed class IntegrationScenarioQueue(HyperIntegrationContext db, IntegrationScenarioProcessor processor) : IIntegrationScenarioQueue
{
    public async Task<long> EnqueueAsync(OwnedIntegrationShop shop, long connectionId, IntegrationScenarioRequest request, CancellationToken ct)
    {
        IntegrationScenarioRules.Validate(shop, connectionId, request);
        await using var tx = db.Database.CurrentTransaction is null ? await db.Database.BeginTransactionAsync(ct) : null;
        // Connection row serializes concurrent event replays. No legacy accounting row is changed.
        var connection = await db.ExternalIntegrationConnections.FromSqlInterpolated($"SELECT * FROM dbo.ExternalIntegrationConnections WITH (UPDLOCK,HOLDLOCK) WHERE Id={connectionId}")
            .AsNoTracking().SingleOrDefaultAsync(ct);
        if (connection is null || connection.ShopId != shop.ShopId || connection.TenantId != shop.TenantId)
            throw new InvalidOperationException("ConnectionUnavailable");
        var existing = await db.IntegrationScenarioJobs.AsNoTracking().SingleOrDefaultAsync(x => x.ConnectionId == connectionId && x.EventId == request.EventId, ct);
        if (existing is not null)
        {
            if (existing.Item != request.Item || existing.Trigger != request.Trigger || existing.ShopId != shop.ShopId || existing.TenantId != shop.TenantId)
                throw new InvalidOperationException("EventIdentityConflict");
            if (tx is not null) await tx.CommitAsync(ct);
            return existing.Id;
        }
        var job = new IntegrationScenarioJob
        {
            ConnectionId = connectionId, ShopId = shop.ShopId, TenantId = shop.TenantId,
            EventId = request.EventId, Item = request.Item, Trigger = request.Trigger,
            CreatedAtUtc = DateTime.UtcNow, NextAttemptAtUtc = DateTime.UtcNow
        };
        db.IntegrationScenarioJobs.Add(job);
        await db.SaveChangesAsync(ct);
        if (tx is not null) await tx.CommitAsync(ct);
        db.Entry(job).State = EntityState.Detached;
        return job.Id;
    }

    public Task<bool> ProcessNextAsync(CancellationToken ct) => ProcessNextCore(null, ct);
    public Task<bool> ProcessConnectionAsync(long connectionId, CancellationToken ct) => ProcessNextCore(connectionId, ct);

    private async Task<bool> ProcessNextCore(long? onlyConnection, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var candidates = await db.IntegrationScenarioJobs.AsNoTracking()
            .Where(x => !onlyConnection.HasValue || x.ConnectionId == onlyConnection.Value)
            .Where(x => x.Status == IntegrationScenarioStatus.Pending && x.NextAttemptAtUtc <= now
                || x.Status == IntegrationScenarioStatus.Running && x.LeaseExpiresAtUtc <= now)
            .OrderBy(x => x.Id).Take(50).Select(x => x.ConnectionId).ToListAsync(ct);
        foreach (var connectionId in candidates.Distinct())
        {
            await db.Database.OpenConnectionAsync(ct);
            var resource = $"Hyper.Integration.Scenarios:{connectionId}";
            var locked = false;
            try
            {
                locked = await SessionLock(resource, true, ct);
                if (!locked) continue;
                var job = await db.IntegrationScenarioJobs.AsNoTracking().Where(x => x.ConnectionId == connectionId
                    && (x.Status == IntegrationScenarioStatus.Pending || x.Status == IntegrationScenarioStatus.Running))
                    .OrderBy(x => x.Id).FirstOrDefaultAsync(ct);
                now = DateTime.UtcNow;
                if (job is null || job.Status == IntegrationScenarioStatus.Pending && job.NextAttemptAtUtc > now
                    || job.Status == IntegrationScenarioStatus.Running && job.LeaseExpiresAtUtc > now) continue;
                var lease = Guid.NewGuid();
                await db.IntegrationScenarioJobs.Where(x => x.Id == job.Id).ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.Status, IntegrationScenarioStatus.Running).SetProperty(x => x.LeaseId, lease)
                    .SetProperty(x => x.LeaseExpiresAtUtc, now.AddMinutes(5)).SetProperty(x => x.Attempts, x => x.Attempts + 1), ct);
                job.Attempts++;
                await Deliver(job, lease, ct);
                return true;
            }
            finally
            {
                try { if (locked) await SessionLock(resource, false, CancellationToken.None); }
                finally { await db.Database.CloseConnectionAsync(); }
            }
        }
        return false;
    }

    private async Task Deliver(IntegrationScenarioJob job, Guid lease, CancellationToken ct)
    {
        var status = IntegrationScenarioStatus.Completed;
        string? resultJson = null, errorCode = null;
        TimeSpan? retryAfter = null;
        var retry = false;
        try
        {
            if (job.Attempts > IntegrationRetryPolicy.MaxAttempts) throw new IntegrationProviderException("AttemptsExhausted", false);
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
            timeout.CancelAfter(TimeSpan.FromMinutes(3));
            var result = await processor.ProcessAsync(job, timeout.Token);
            // Bound the persisted summary. A detailed page/export can be added separately.
            resultJson = JsonSerializer.Serialize(new { result.Compared, result.Enqueued,
                DifferenceCount = result.Differences.Count, Differences = result.Differences.Take(500) });
            if (result.Differences.Count != 0) { status = IntegrationScenarioStatus.NeedsAttention; errorCode = "DifferencesDetected"; }
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
        catch (IntegrationProviderException ex) { errorCode = ex.Code; retry = ex.Retryable; retryAfter = ex.RetryAfter; }
        catch (NotSupportedException) { errorCode = "UnsupportedStrategy"; }
        catch (HttpRequestException) { errorCode = "ProviderTransport"; retry = true; }
        catch (OperationCanceledException) { errorCode = "ProviderTimeout"; retry = true; }
        catch (JsonException) { errorCode = "InvalidProviderPayload"; }
        catch (ArgumentException) { errorCode = "DuplicateOrInvalidSourceData"; }
        catch (InvalidOperationException) { errorCode = "ConnectionOrSourceUnavailable"; }
        var finished = DateTime.UtcNow;
        if (errorCode is not null && resultJson is null)
            status = retry && job.Attempts < IntegrationRetryPolicy.MaxAttempts ? IntegrationScenarioStatus.Pending : IntegrationScenarioStatus.DeadLetter;
        var next = status == IntegrationScenarioStatus.Pending ? finished + IntegrationRetryPolicy.Delay(job.Attempts, retryAfter) : finished;
        await db.IntegrationScenarioJobs.Where(x => x.Id == job.Id && x.Status == IntegrationScenarioStatus.Running && x.LeaseId == lease)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.Status, status).SetProperty(x => x.ErrorCode, errorCode)
                .SetProperty(x => x.ResultJson, resultJson).SetProperty(x => x.NextAttemptAtUtc, next)
                .SetProperty(x => x.CompletedAtUtc, status == IntegrationScenarioStatus.Pending ? (DateTime?)null : finished)
                .SetProperty(x => x.LeaseId, (Guid?)null).SetProperty(x => x.LeaseExpiresAtUtc, (DateTime?)null), CancellationToken.None);
    }

    private async Task<bool> SessionLock(string resource, bool acquire, CancellationToken ct)
    {
        await using var command = db.Database.GetDbConnection().CreateCommand();
        command.Transaction = db.Database.CurrentTransaction?.GetDbTransaction();
        command.CommandText = acquire
            ? "DECLARE @r int; EXEC @r=sys.sp_getapplock @Resource=@resource,@LockMode='Exclusive',@LockOwner='Session',@LockTimeout=0; SELECT @r;"
            : "DECLARE @r int; EXEC @r=sys.sp_releaseapplock @Resource=@resource,@LockOwner='Session'; SELECT @r;";
        var p = command.CreateParameter(); p.ParameterName = "@resource"; p.DbType = DbType.String; p.Value = resource; command.Parameters.Add(p);
        return Convert.ToInt32(await command.ExecuteScalarAsync(ct)) >= 0;
    }
    public async Task<IReadOnlyList<IntegrationScenarioConnection>> ConnectionsAsync(OwnedIntegrationShop shop, CancellationToken ct) =>
        await db.ExternalIntegrationConnections.AsNoTracking().Where(x => x.ShopId == shop.ShopId && x.TenantId == shop.TenantId)
            .OrderBy(x => x.Provider).Select(x => new IntegrationScenarioConnection(x.Id, x.DisplayName, x.Provider, x.IsEnabled)).ToListAsync(ct);
    public async Task<IReadOnlyList<IntegrationScenarioJob>> RecentAsync(OwnedIntegrationShop shop, CancellationToken ct) =>
        await db.IntegrationScenarioJobs.AsNoTracking().Where(x => x.ShopId == shop.ShopId && x.TenantId == shop.TenantId)
            .OrderByDescending(x => x.Id).Take(20).ToListAsync(ct);
}
