using Hyper.Infrastructure.Features.Integrations;
using Hyper.Integration.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

internal sealed class QueueCases(QueueFixture fixture)
{
    internal int Checks { get; private set; }
    private static readonly TimeSpan Watchdog = TimeSpan.FromSeconds(15);
    private void Check(bool condition, string description)
    {
        if (!condition) throw new QueueCheckFailure(description);
        Console.WriteLine($"PASS {++Checks}: {description}");
    }

    internal async Task RunAsync()
    {
        await EnqueueIdentityAsync();
        await InboxIdentityAsync();
        await ShopExclusionAsync();
        await RetryOrderingAsync();
        await ExhaustionAsync();
        await CancellationRecoveryAsync();
        await StaleAcknowledgementAsync();
        await AcknowledgementRollbackAsync();
        await UnknownExpiryAsync();
        await NeedsAttentionAsync();
    }

    private async Task EnqueueIdentityAsync()
    {
        Console.WriteLine("CASE production enqueue identity under concurrent SQL callers");
        var connection = (await fixture.ConnectionsAsync())[0];
        const string eventId = "enqueue-concurrent";
        var ids = await Task.WhenAll(fixture.EnqueueAsync(connection, eventId), fixture.EnqueueAsync(connection, eventId));
        Check(ids[0] == ids[1], "concurrent exact enqueues return the same durable job");
        await using (var db = fixture.Open())
            Check(await db.IntegrationScenarioJobs.CountAsync(x => x.ConnectionId == connection.Id && x.EventId == eventId) == 1,
                "enqueue serializes one persisted job for a connection/event");
        var conflicted = false;
        try { await fixture.EnqueueAsync(connection, eventId, IntegrationSyncItem.Chat); }
        catch (InvalidOperationException error) when (error.Message == "EventIdentityConflict") { conflicted = true; }
        Check(conflicted, "same enqueue identity with a different scenario is rejected");
        await fixture.AddInboxAsync(connection, eventId);
        Check(await fixture.ProcessAsync(connection), "actual processor dispatches the enqueued review");
        var job = await fixture.JobAsync(ids[0]);
        Check(job.Status == IntegrationScenarioStatus.Completed && job.Attempts == 1 && job.LeaseId is null
            && job.LeaseExpiresAtUtc is null && job.CompletedAtUtc is not null,
            "successful job ACK clears the lease and records one completed attempt");
        var received = fixture.Owner.Received.Single(x => x.ConnectionId == connection.Id && x.EventId == eventId);
        Check(received.ShopId == connection.ShopId && received.TenantId == connection.TenantId,
            "actual dispatcher preserves shop/tenant/connection/event identity");
        Check(!await fixture.ProcessAsync(connection) && fixture.Owner.Calls(connection, eventId) == 1,
            "completed job is not dispatched again");
    }

    private async Task InboxIdentityAsync()
    {
        Console.WriteLine("CASE real webhook ingress duplicate and conflicting content");
        var connection = (await fixture.ConnectionsAsync())[0];
        const string eventId = "inbox-concurrent";
        var results = await Task.WhenAll(fixture.ReceiveAsync(connection, eventId), fixture.ReceiveAsync(connection, eventId));
        Check(results.Count(x => x.Status == WebhookIngressStatus.Accepted) == 1
            && results.Count(x => x.Status == WebhookIngressStatus.Duplicate) == 1
            && results[0].InboxId == results[1].InboxId,
            "concurrent authenticated deliveries create one inbox identity");
        var id = results.Single(x => x.Status == WebhookIngressStatus.Accepted).ScenarioJobId!.Value;
        await using (var db = fixture.Open())
            Check(await db.IntegrationWebhookInbox.CountAsync(x => x.ConnectionId == connection.Id && x.ExternalEventId == eventId) == 1
                && await db.IntegrationScenarioJobs.CountAsync(x => x.ConnectionId == connection.Id && x.EventId == eventId) == 1
                && await db.IntegrationEventAudits.CountAsync(x => x.ConnectionId == connection.Id && x.ExternalEventId == eventId) == 1,
                "ingress atomically retains one inbox, scenario and accepted audit");
        var conflict = await fixture.ReceiveAsync(connection, eventId, rating: 1);
        Check(conflict.Status == WebhookIngressStatus.Invalid && conflict.ErrorCode == "EventIdentityConflict",
            "same delivery ID with different content is rejected without another job");
        Check(await fixture.ProcessAsync(connection), "production queue consumes the accepted inbox");
        var job = await fixture.JobAsync(id);
        var inbox = await fixture.InboxAsync(connection, eventId);
        Check(job.Status == IntegrationScenarioStatus.Completed && inbox.Status == 1 && inbox.ProcessedAtUtc is not null && inbox.Error is null,
            "job and matching inbox receive successful completion");
        Check((await fixture.ReceiveAsync(connection, eventId)).Status == WebhookIngressStatus.Duplicate
            && !await fixture.ProcessAsync(connection) && fixture.Owner.Calls(connection, eventId) == 1,
            "post-completion delivery retry does not create a second dispatch");
    }

    private async Task ShopExclusionAsync()
    {
        Console.WriteLine("CASE live SQL session exclusion, cross-connection FIFO and parallel shops");
        var sameShop = await fixture.ConnectionsAsync(2);
        var otherShop = (await fixture.ConnectionsAsync())[0];
        var first = await fixture.EventAsync(sameShop[0], "lock-first");
        var second = await fixture.EventAsync(sameShop[1], "lock-second");
        await fixture.EventAsync(otherShop, "lock-other-shop");
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        fixture.Owner.Behavior["lock-first"] = async (command, ct) =>
        {
            entered.TrySetResult();
            await release.Task.WaitAsync(ct);
            return fixture.Owner.Acknowledge(command);
        };
        using var shutdown = new CancellationTokenSource(TimeSpan.FromSeconds(45));
        var firstWorker = fixture.ProcessAsync(sameShop[0], shutdown.Token);
        try
        {
            await entered.Task.WaitAsync(Watchdog);
            var running = await fixture.JobAsync(first);
            Check(running.Status == IntegrationScenarioStatus.Running && running.LeaseId is not null && running.Attempts == 1,
                "worker persists its lease before entering the owner port");
            await fixture.ExpireAsync(first);
            Check(!await fixture.ProcessAsync(sameShop[0])
                && fixture.Owner.Calls(sameShop[0], "lock-first") == 1,
                "expired lease cannot bypass the still-live SQL session lock");
            Check(!await fixture.ProcessAsync(sameShop[1])
                && fixture.Owner.Calls(sameShop[1], "lock-second") == 0,
                "another connection cannot bypass its shop's active head");
            Check(await fixture.ProcessAsync(otherShop)
                && fixture.Owner.Calls(otherShop, "lock-other-shop") == 1,
                "a different shop completes while the first shop remains locked");
            Check((await fixture.JobAsync(second)).Status == IntegrationScenarioStatus.Pending,
                "same-shop successor remains pending during the first dispatch");
        }
        finally
        {
            release.TrySetResult();
            await firstWorker;
            fixture.Owner.Behavior.TryRemove("lock-first", out _);
        }
        Check(await fixture.ProcessAsync(sameShop[1]), "released shop lock allows the next connection to advance");
        Check(fixture.Owner.Received.Where(x => x.ShopId == sameShop[0].ShopId).Select(x => x.EventId)
            .SequenceEqual(["lock-first", "lock-second"]), "actual SQL dispatch order follows durable shop FIFO");
    }

    private async Task RetryOrderingAsync()
    {
        Console.WriteLine("CASE durable retry schedule retains inbox identity and blocks shop tail");
        var connections = await fixture.ConnectionsAsync(2);
        const string eventId = "retry-first";
        var id = await fixture.EventAsync(connections[0], eventId);
        await fixture.EventAsync(connections[1], "retry-tail");
        fixture.Owner.Behavior[eventId] = (_, _) => throw new IntegrationProviderException("FixtureTransient", true, TimeSpan.FromMinutes(90));
        var before = DateTime.UtcNow;
        Check(await fixture.ProcessAsync(connections[0]), "retryable owner error is handled by the real queue");
        var pending = await fixture.JobAsync(id);
        var inbox = await fixture.InboxAsync(connections[0], eventId);
        Check(pending.Status == IntegrationScenarioStatus.Pending && pending.Attempts == 1
            && pending.ErrorCode == "FixtureTransient" && pending.NextAttemptAtUtc >= before.AddMinutes(90)
            && pending.LeaseId is null && pending.CompletedAtUtc is null,
            "retry persists Retry-After, attempt count and released lease without completion");
        Check(inbox.Status == 0 && inbox.Error == "FixtureTransient" && inbox.ProcessedAtUtc is null,
            "transient failure leaves its inbox pending with the same safe code");
        Check(!await fixture.ProcessAsync(connections[0]) && !await fixture.ProcessAsync(connections[1])
            && fixture.Owner.Calls(connections[0], eventId) == 1,
            "scheduled retry prevents early resend and cross-connection overtaking");
        fixture.Owner.Behavior.TryRemove(eventId, out _);
        await fixture.DueAsync(id);
        Check(await fixture.ProcessAsync(connections[0]), "fresh worker context retries the original durable job");
        var completed = await fixture.JobAsync(id);
        Check(completed.Status == IntegrationScenarioStatus.Completed && completed.Attempts == 2
            && completed.EventId == eventId && (await fixture.InboxAsync(connections[0], eventId)).Status == 1,
            "successful retry completes the same event and inbox identity");
        Check(await fixture.ProcessAsync(connections[1]), "shop tail advances only after head completion");
    }

    private async Task ExhaustionAsync()
    {
        Console.WriteLine("CASE finite retry exhaustion and terminal head advancement");
        var connection = (await fixture.ConnectionsAsync())[0];
        const string eventId = "exhausted-head";
        var id = await fixture.EventAsync(connection, eventId);
        await fixture.EventAsync(connection, "exhausted-tail");
        fixture.Owner.Behavior[eventId] = (_, _) => throw new IntegrationProviderException("FixtureUnavailable", true);
        for (var attempt = 1; attempt <= IntegrationRetryPolicy.MaxAttempts; attempt++)
        {
            await fixture.DueAsync(id);
            Check(await fixture.ProcessAsync(connection), $"production queue executes bounded retry attempt {attempt}");
            var state = await fixture.JobAsync(id);
            Check(state.Attempts == attempt && state.Status == (attempt < IntegrationRetryPolicy.MaxAttempts
                ? IntegrationScenarioStatus.Pending : IntegrationScenarioStatus.DeadLetter),
                $"attempt {attempt} has the expected durable status and count");
        }
        var inbox = await fixture.InboxAsync(connection, eventId);
        Check(inbox.Status == 2 && inbox.ProcessedAtUtc is not null && inbox.Error == "FixtureUnavailable"
            && fixture.Owner.Calls(connection, eventId) == IntegrationRetryPolicy.MaxAttempts,
            "exhaustion records terminal inbox outcome without an extra owner call");
        Check(await fixture.ProcessAsync(connection) && fixture.Owner.Calls(connection, "exhausted-tail") == 1,
            "dead letter releases its shop tail for processing");
    }

    private async Task CancellationRecoveryAsync()
    {
        Console.WriteLine("CASE host cancellation and fresh-context expired-lease recovery");
        var connection = (await fixture.ConnectionsAsync())[0];
        const string eventId = "cancelled-owner";
        var id = await fixture.EventAsync(connection, eventId);
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        fixture.Owner.Behavior[eventId] = async (_, ct) =>
        {
            entered.TrySetResult();
            await new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously).Task.WaitAsync(ct);
            throw new QueueCheckFailure("Cancelled owner unexpectedly continued.");
        };
        using var shutdown = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        var worker = fixture.ProcessAsync(connection, shutdown.Token);
        var cancelled = false;
        try { await entered.Task.WaitAsync(Watchdog); }
        finally
        {
            await shutdown.CancelAsync();
            try { await worker; }
            catch (OperationCanceledException) when (shutdown.IsCancellationRequested) { cancelled = true; }
            fixture.Owner.Behavior.TryRemove(eventId, out _);
        }
        var running = await fixture.JobAsync(id);
        Check(cancelled && running.Status == IntegrationScenarioStatus.Running && running.LeaseId is not null
            && running.LeaseExpiresAtUtc > DateTime.UtcNow && running.Attempts == 1,
            "host cancellation preserves unacknowledged running lease for recovery");
        Check((await fixture.InboxAsync(connection, eventId)).Status == 0 && !await fixture.ProcessAsync(connection),
            "fresh context cannot recover a cancelled job before lease expiry");
        await fixture.ExpireAsync(id);
        Check(await fixture.ProcessAsync(connection), "expired cancelled job is recovered through production claim logic");
        var done = await fixture.JobAsync(id);
        Check(done.Status == IntegrationScenarioStatus.Completed && done.Attempts == 2 && done.LeaseId is null
            && fixture.Owner.Calls(connection, eventId) == 2 && fixture.Owner.Effects(connection, eventId) == 1,
            "recovery releases prior session lock and preserves original identity");
    }

    private async Task StaleAcknowledgementAsync()
    {
        Console.WriteLine("CASE stale lease ACK cannot overwrite replacement ownership");
        var connection = (await fixture.ConnectionsAsync())[0];
        const string eventId = "stale-ack";
        var id = await fixture.EventAsync(connection, eventId);
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        fixture.Owner.Behavior[eventId] = async (command, ct) =>
        {
            entered.TrySetResult();
            await release.Task.WaitAsync(ct);
            return fixture.Owner.Acknowledge(command);
        };
        using var shutdown = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        var worker = fixture.ProcessAsync(connection, shutdown.Token);
        var replacement = Guid.NewGuid();
        try
        {
            await entered.Task.WaitAsync(Watchdog);
            await using var db = fixture.Open();
            // Fixture-only fault: cooperating workers cannot replace a live session's lease.
            await db.IntegrationScenarioJobs.Where(x => x.Id == id).ExecuteUpdateAsync(set => set
                .SetProperty(x => x.LeaseId, replacement).SetProperty(x => x.ErrorCode, "ReplacementOwner"));
        }
        finally
        {
            release.TrySetResult();
            await worker;
            fixture.Owner.Behavior.TryRemove(eventId, out _);
        }
        var state = await fixture.JobAsync(id);
        var inbox = await fixture.InboxAsync(connection, eventId);
        Check(state.Status == IntegrationScenarioStatus.Running && state.LeaseId == replacement
            && state.ErrorCode == "ReplacementOwner" && state.CompletedAtUtc is null,
            "old lease result cannot overwrite replacement job state");
        Check(inbox.Status == 0 && inbox.ProcessedAtUtc is null && inbox.Error is null,
            "zero-row job ACK leaves the associated inbox untouched");
        await fixture.ExpireAsync(id);
        Check(await fixture.ProcessAsync(connection) && (await fixture.JobAsync(id)).Status == IntegrationScenarioStatus.Completed,
            "replacement lease later recovers after actual session release");
        Check(fixture.Owner.Calls(connection, eventId) == 2 && fixture.Owner.Effects(connection, eventId) == 1,
            "recovery redelivers identity to an explicitly idempotent fixture owner");
    }

    private async Task AcknowledgementRollbackAsync()
    {
        Console.WriteLine("CASE real completion transaction rolls back job when inbox ACK fails");
        var connection = (await fixture.ConnectionsAsync())[0];
        const string eventId = "ack-rollback";
        var id = await fixture.EventAsync(connection, eventId);
        await fixture.FixtureSqlAsync("""
            CREATE TRIGGER dbo.tr_QueueSqlChecks_RejectInboxAck
            ON dbo.IntegrationWebhookInbox AFTER UPDATE AS
            BEGIN
                SET NOCOUNT ON;
                IF EXISTS (SELECT 1 FROM inserted WHERE ExternalEventId = N'ack-rollback')
                    THROW 51000, 'Queue fixture ACK failure', 1;
            END;
            """);
        var faultObserved = false;
        try
        {
            try { await fixture.ProcessAsync(connection); }
            catch (SqlException error) when (error.Number == 51000) { faultObserved = true; }
        }
        finally { await fixture.FixtureSqlAsync("DROP TRIGGER dbo.tr_QueueSqlChecks_RejectInboxAck;"); }
        Check(faultObserved, "injected inbox update failure propagates from production completion");
        var interrupted = await fixture.JobAsync(id);
        var inbox = await fixture.InboxAsync(connection, eventId);
        Check(interrupted.Status == IntegrationScenarioStatus.Running && interrupted.LeaseId is not null
            && interrupted.CompletedAtUtc is null && interrupted.Attempts == 1,
            "failed inbox ACK rolls back the preceding job completion in the same transaction");
        Check(inbox.Status == 0 && inbox.ProcessedAtUtc is null && fixture.Owner.Effects(connection, eventId) == 1,
            "owner effect can precede ACK loss while inbox stays unprocessed");
        await fixture.ExpireAsync(id);
        Check(await fixture.ProcessAsync(connection), "new context recovers ACK failure without a new event");
        var completed = await fixture.JobAsync(id);
        inbox = await fixture.InboxAsync(connection, eventId);
        Check(completed.Status == IntegrationScenarioStatus.Completed && completed.Attempts == 2
            && inbox.Status == 1 && inbox.ProcessedAtUtc is not null,
            "owner Duplicate acknowledgement completes recovered job and inbox");
        Check(fixture.Owner.Calls(connection, eventId) == 2 && fixture.Owner.Effects(connection, eventId) == 1,
            "queue is at-least-once; fixture owner deduplicates the replayed effect");
    }

    private async Task UnknownExpiryAsync()
    {
        Console.WriteLine("CASE unknown running lease expiry blocks same-shop successors");
        var connections = await fixture.ConnectionsAsync(2);
        var id = await fixture.EventAsync(connections[0], "unknown-expiry");
        await fixture.EventAsync(connections[1], "unknown-expiry-tail");
        await using (var db = fixture.Open())
            await db.IntegrationScenarioJobs.Where(x => x.Id == id).ExecuteUpdateAsync(set => set
                .SetProperty(x => x.Status, IntegrationScenarioStatus.Running)
                .SetProperty(x => x.LeaseId, Guid.NewGuid()).SetProperty(x => x.LeaseExpiresAtUtc, (DateTime?)null));
        Check(!await fixture.ProcessAsync(connections[0]) && !await fixture.ProcessAsync(connections[1]),
            "production SQL does not reclaim unknown expiry or bypass its shop head");
        Check(fixture.Owner.Calls(connections[0], "unknown-expiry") == 0
            && fixture.Owner.Calls(connections[1], "unknown-expiry-tail") == 0,
            "blocked unknown lease performs no owner dispatch");
        await fixture.ExpireAsync(id);
        Check(await fixture.ProcessAsync(connections[0]) && await fixture.ProcessAsync(connections[1]),
            "explicit fixture expiry repair restores normal head-then-tail processing");
    }

    private async Task NeedsAttentionAsync()
    {
        Console.WriteLine("CASE actionable owner dependency is terminal attention, not false success");
        var connection = (await fixture.ConnectionsAsync())[0];
        const string eventId = "pending-owner";
        var id = await fixture.EventAsync(connection, eventId);
        await fixture.EventAsync(connection, "pending-owner-tail");
        fixture.Owner.Behavior[eventId] = (_, _) => Task.FromResult(
            new EngagementCommandResult(EngagementCommandStatus.PendingDependency, "FixtureOwnerPending"));
        Check(await fixture.ProcessAsync(connection), "owner dependency result returns through real dispatcher");
        var job = await fixture.JobAsync(id);
        var inbox = await fixture.InboxAsync(connection, eventId);
        Check(job.Status == IntegrationScenarioStatus.NeedsAttention && job.ErrorCode == "DifferencesDetected"
            && job.ResultJson!.Contains("FixtureOwnerPending", StringComparison.Ordinal) && inbox.Status == 2,
            "unapplied owner result persists actionable job and inbox outcome");
        Check(await fixture.ProcessAsync(connection) && fixture.Owner.Calls(connection, "pending-owner-tail") == 1,
            "NeedsAttention is terminal for scheduling and does not starve successors");
    }
}
