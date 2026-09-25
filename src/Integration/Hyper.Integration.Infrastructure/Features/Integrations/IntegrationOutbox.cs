using System.Data;
using System.Text.Json;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed class IntegrationOutbox(HyperIntegrationContext db, IIntegrationStrategyResolver strategies) : IIntegrationOutbox
{
    private const string InventoryOperation = "inventory.set.v1";

    public async Task<long> EnqueueInventoryAsync(long connectionId, long mappingId, long sourceVersion, decimal quantity, CancellationToken ct)
    {
        if (sourceVersion <= 0 || quantity < 0) throw new ArgumentException("Positive source version and nonnegative absolute stock are required.");
        await using var transaction = db.Database.CurrentTransaction is null ? await db.Database.BeginTransactionAsync(ct) : null;
        // Lock the mapping as the serialization point for concurrent source versions/replays.
        var mapping = await db.ExternalProductMappings.FromSqlInterpolated($"SELECT * FROM dbo.ExternalProductMappings WITH (UPDLOCK,HOLDLOCK) WHERE Id={mappingId}")
            .AsNoTracking().SingleOrDefaultAsync(ct) ?? throw new InvalidOperationException("MappingUnavailable");
        var connection = await db.ExternalIntegrationConnections.AsNoTracking().SingleAsync(x => x.Id == connectionId, ct);
        IntegrationConnectionReadiness.Validate(connection, DateTime.UtcNow);
        strategies.Resolve(connection.Provider, connection.CredentialType);
        if (mapping.ConnectionId != connectionId || mapping.ShopId != connection.ShopId || !mapping.IsActive || mapping.HyperProductId <= 0)
            throw new InvalidOperationException("MappingUnavailable");
        var payload = JsonSerializer.Serialize(new ExternalInventoryUpdate(mapping.ExternalProductId, mapping.ExternalVariantId, quantity));
        var previous = await db.IntegrationOutbox.AsNoTracking().Where(x => x.MappingId == mappingId && x.SourceVersion == sourceVersion).SingleOrDefaultAsync(ct);
        if (previous is not null)
        {
            if (previous.PayloadJson != payload || previous.ConnectionId != connectionId)
                throw new InvalidOperationException("SourceVersionConflict");
            if (transaction is not null) await transaction.CommitAsync(ct);
            return previous.Id;
        }
        if (await db.IntegrationOutbox.AnyAsync(x => x.MappingId == mappingId && x.SourceVersion > sourceVersion, ct))
            throw new InvalidOperationException("StaleSourceVersion");
        var message = new IntegrationOutboxMessage
        {
            ConnectionId = connectionId, MappingId = mappingId, SourceVersion = sourceVersion,
            Operation = InventoryOperation, PayloadJson = payload, CreatedAtUtc = DateTime.UtcNow, NextAttemptAtUtc = DateTime.UtcNow
        };
        db.IntegrationOutbox.Add(message);
        await db.SaveChangesAsync(ct);
        if (transaction is not null) await transaction.CommitAsync(ct);
        return message.Id;
    }

    public Task<bool> ProcessNextAsync(CancellationToken ct) => ProcessNextCore(null, ct);

    public async Task<bool> RetryAsync(long connectionId, long messageId, CancellationToken ct)
    {
        var message = await db.IntegrationOutbox.AsNoTracking().SingleOrDefaultAsync(x => x.Id == messageId && x.ConnectionId == connectionId, ct);
        if (message is null || message.Status != 3) return false;
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        // Same lock as enqueue: a newer source version must not race a replay decision.
        var mapping = await db.ExternalProductMappings.FromSqlInterpolated($"SELECT * FROM dbo.ExternalProductMappings WITH (UPDLOCK,HOLDLOCK) WHERE Id={message.MappingId}")
            .AsNoTracking().SingleOrDefaultAsync(ct);
        if (mapping is null || !mapping.IsActive || mapping.ConnectionId != connectionId
            || await db.IntegrationOutbox.AnyAsync(x => x.MappingId == message.MappingId && x.SourceVersion > message.SourceVersion, ct))
            return false;
        var connection = await db.ExternalIntegrationConnections.AsNoTracking().SingleAsync(x => x.Id == connectionId, ct);
        IntegrationConnectionReadiness.Validate(connection, DateTime.UtcNow);
        var changed = await db.IntegrationOutbox.Where(x => x.Id == messageId && x.ConnectionId == connectionId && x.Status == 3)
            .ExecuteUpdateAsync(set => set.SetProperty(x => x.Status, (byte)0).SetProperty(x => x.Attempts, 0)
                .SetProperty(x => x.NextAttemptAtUtc, DateTime.UtcNow).SetProperty(x => x.CompletedAtUtc, (DateTime?)null)
                .SetProperty(x => x.LastError, (string?)null).SetProperty(x => x.LeaseId, (Guid?)null)
                .SetProperty(x => x.LeaseExpiresAtUtc, (DateTime?)null), ct);
        await transaction.CommitAsync(ct);
        return changed == 1;
    }

    public Task<bool> ProcessConnectionAsync(long connectionId, CancellationToken ct) => ProcessNextCore(connectionId, ct);

    private async Task<bool> ProcessNextCore(long? onlyConnection, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var candidates = await db.IntegrationOutbox.AsNoTracking()
            .Where(x => !onlyConnection.HasValue || x.ConnectionId == onlyConnection.Value)
            .Where(x => x.Status == 0 && x.NextAttemptAtUtc <= now || x.Status == 1 && x.LeaseExpiresAtUtc <= now)
            .GroupBy(x => x.ConnectionId).Select(g => new { ConnectionId = g.Key, FirstId = g.Min(x => x.Id) })
            .OrderBy(x => x.FirstId).Take(50).Select(x => x.ConnectionId).ToListAsync(ct);
        foreach (var connectionId in candidates.Distinct())
        {
            await db.Database.OpenConnectionAsync(ct);
            var resource = $"Hyper.Integration.Outbox:{connectionId}";
            var locked = false;
            try
            {
                locked = await SessionLock(resource, true, ct);
                if (!locked) continue;
                // The session lock spans HTTP as well as persistence. An expired lease does
                // not allow another worker to overtake a still-active sender on this connection.
                var message = await db.IntegrationOutbox.AsNoTracking()
                    .Where(x => x.ConnectionId == connectionId && (x.Status == 0 || x.Status == 1))
                    .OrderBy(x => x.Id).FirstOrDefaultAsync(ct);
                now = DateTime.UtcNow;
                if (message is null || message.Status == 0 && message.NextAttemptAtUtc > now
                    || message.Status == 1 && message.LeaseExpiresAtUtc > now) continue;
                var lease = Guid.NewGuid();
                var expires = now.AddMinutes(2);
                await db.IntegrationOutbox.Where(x => x.Id == message.Id).ExecuteUpdateAsync(set => set
                    .SetProperty(x => x.Status, (byte)1).SetProperty(x => x.LeaseId, lease)
                    .SetProperty(x => x.LeaseExpiresAtUtc, expires).SetProperty(x => x.Attempts, x => x.Attempts + 1), ct);
                message.Attempts++;
                await Deliver(message, lease, ct);
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

    private async Task Deliver(IntegrationOutboxMessage message, Guid lease, CancellationToken ct)
    {
        string? errorCode = null;
        var retry = false;
        TimeSpan? retryAfter = null;
        try
        {
            if (message.Attempts > IntegrationRetryPolicy.MaxAttempts)
                throw new IntegrationProviderException("AttemptsExhausted", false);
            var connection = await db.ExternalIntegrationConnections.AsNoTracking().SingleAsync(x => x.Id == message.ConnectionId, ct);
            IntegrationConnectionReadiness.Validate(connection, DateTime.UtcNow);
            var mapping = await db.ExternalProductMappings.AsNoTracking().SingleOrDefaultAsync(x => x.Id == message.MappingId, ct);
            var update = JsonSerializer.Deserialize<ExternalInventoryUpdate>(message.PayloadJson)
                ?? throw new IntegrationProviderException("InvalidPayload", false);
            if (message.Operation != InventoryOperation || mapping is null || !mapping.IsActive
                || mapping.ConnectionId != connection.Id || mapping.ShopId != connection.ShopId || mapping.HyperProductId <= 0
                || mapping.ExternalProductId != update.ExternalProductId || mapping.ExternalVariantId != update.VariantId)
                throw new IntegrationProviderException("MappingChanged", false);
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
            timeout.CancelAfter(TimeSpan.FromSeconds(60));
            await strategies.Resolve(connection.Provider, connection.CredentialType).PublishInventoryAsync(connection, [update], timeout.Token);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; } // lease recovers after shutdown
        catch (IntegrationProviderException exception) { errorCode = exception.Code; retry = exception.Retryable; retryAfter = exception.RetryAfter; }
        catch (OperationCanceledException) { errorCode = "ProviderTimeout"; retry = true; }
        catch (HttpRequestException) { errorCode = "ProviderTransport"; retry = true; }
        catch (NotSupportedException) { errorCode = "UnsupportedStrategy"; }
        catch (InvalidOperationException) { errorCode = "ConnectionOrMappingUnavailable"; }
        catch (JsonException) { errorCode = "InvalidPayloadOrCredentials"; }
        var finished = DateTime.UtcNow;
        var status = errorCode is null ? (byte)2 : retry && message.Attempts < IntegrationRetryPolicy.MaxAttempts ? (byte)0 : (byte)3;
        var next = status == 0 ? finished + IntegrationRetryPolicy.Delay(message.Attempts, retryAfter) : finished;
        await db.IntegrationOutbox.Where(x => x.Id == message.Id && x.Status == 1 && x.LeaseId == lease)
            .ExecuteUpdateAsync(set => set.SetProperty(x => x.Status, status)
                .SetProperty(x => x.NextAttemptAtUtc, next).SetProperty(x => x.LastError, errorCode)
                .SetProperty(x => x.CompletedAtUtc, status == 0 ? (DateTime?)null : finished)
                .SetProperty(x => x.LeaseId, (Guid?)null).SetProperty(x => x.LeaseExpiresAtUtc, (DateTime?)null), CancellationToken.None);
    }

    private async Task<bool> SessionLock(string resource, bool acquire, CancellationToken ct)
    {
        await using var command = db.Database.GetDbConnection().CreateCommand();
        command.CommandText = acquire
            ? "DECLARE @result int; EXEC @result=sys.sp_getapplock @Resource=@resource,@LockMode='Exclusive',@LockOwner='Session',@LockTimeout=0; SELECT @result;"
            : "DECLARE @result int; EXEC @result=sys.sp_releaseapplock @Resource=@resource,@LockOwner='Session'; SELECT @result;";
        var parameter = command.CreateParameter();
        parameter.ParameterName = "@resource"; parameter.DbType = DbType.String; parameter.Value = resource;
        command.Parameters.Add(parameter);
        return Convert.ToInt32(await command.ExecuteScalarAsync(ct)) >= 0;
    }
}
