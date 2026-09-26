using Hyper.Infrastructure.Features.Integrations;
using Hyper.Integration.Domain.Entities.Integrations;
using Hyper.Integration.Domain.Features.Integrations;
using Microsoft.EntityFrameworkCore;

var checks = 0;
void Check(bool value, string description)
{
    if (!value) throw new InvalidOperationException(description);
    Console.WriteLine($"PASS {++checks}: {description}");
}

var now = new DateTime(2026, 9, 26, 12, 0, 0, DateTimeKind.Utc);
IntegrationScenarioJob Job(long id, int shop, long connection) => new()
{
    Id = id, ShopId = shop, ConnectionId = connection, TenantId = "fixture",
    EventId = "event-" + id, Status = IntegrationScenarioStatus.Pending,
    NextAttemptAtUtc = now, CreatedAtUtc = now
};
long[] Ready(IEnumerable<IntegrationScenarioJob> jobs, long? connection = null) =>
    IntegrationScenarioScheduling.ReadyHeads(jobs.AsQueryable(), now, connection).Select(x => x.Id).ToArray();

var first = Job(1, 10, 100);
var second = Job(2, 10, 101);
var other = Job(3, 20, 200);
IntegrationScenarioJob[] sequence = [other, second, first];
Check(Ready(sequence).SequenceEqual([1L, 3L]), "oldest event per shop, across connections, in durable ID order");
Check(Ready(sequence, 101).Length == 0, "connection-specific processing cannot bypass another connection's shop head");
Check(Ready(sequence, 200).SequenceEqual([3L]), "connection-specific processing selects only its own eligible head");

first.Status = IntegrationScenarioStatus.Completed;
Check(Ready(sequence).SequenceEqual([2L, 3L]), "acknowledged completion advances to next connection in the same shop");
foreach (var terminal in new[] { IntegrationScenarioStatus.NeedsAttention, IntegrationScenarioStatus.DeadLetter })
{
    first.Status = terminal;
    Check(Ready(sequence).SequenceEqual([2L, 3L]), $"{terminal} remains terminal and does not stall later work");
}
first.Status = IntegrationScenarioStatus.Pending;
first.NextAttemptAtUtc = now.AddMinutes(1);
Check(Ready(sequence).SequenceEqual([3L]), "a delayed retry blocks only its shop, not unrelated shops");
first.NextAttemptAtUtc = now;
Check(Ready(sequence).SequenceEqual([1L, 3L]), "retry becomes eligible exactly at its scheduled time");

first.Status = IntegrationScenarioStatus.Running;
first.LeaseExpiresAtUtc = now.AddMinutes(1);
Check(Ready(sequence).SequenceEqual([3L]), "live lease blocks later events of the shop");
first.LeaseExpiresAtUtc = now;
Check(Ready(sequence).SequenceEqual([1L, 3L]), "expired lease recovers the same head, not its successor");
first.LeaseExpiresAtUtc = null;
Check(Ready(sequence).SequenceEqual([3L]), "running event without expiry is not guessed to be recoverable");

var backlog = Enumerable.Range(1, 100).Select(id => Job(id, 10, 100))
    .Concat(Enumerable.Range(101, 60).Select(id => Job(id, id, id))).ToArray();
Check(Ready(backlog).SequenceEqual(new[] { 1L }.Concat(Enumerable.Range(101, 49).Select(x => (long)x))),
    "50 candidate slots represent 50 shops, not 50 events of one shop");
backlog[0].NextAttemptAtUtc = now.AddHours(1);
Check(Ready(backlog).SequenceEqual(Enumerable.Range(101, 50).Select(x => (long)x)),
    "delayed shop's entire tail is excluded before paging");
Check(Ready([]).Length == 0, "empty queue is idle");
Check(IntegrationScenarioScheduling.LockResource(first.ShopId) == IntegrationScenarioScheduling.LockResource(second.ShopId)
    && IntegrationScenarioScheduling.LockResource(first.ShopId) != IntegrationScenarioScheduling.LockResource(other.ShopId),
    "lock identity is shared across a shop's connections and separate across shops");

// SQL translation only: no connection is opened and no database/fixture is created.
using var db = new SchedulingContext(new DbContextOptionsBuilder<SchedulingContext>()
    .UseSqlServer("Server=unused.invalid;Database=QueueTranslationOnly;Integrated Security=true;Encrypt=true").Options);
foreach (long? connection in new long?[] { null, 101 })
{
    var sql = IntegrationScenarioScheduling.ReadyHeads(db.Jobs.AsNoTracking(), now, connection)
        .Select(x => x.ShopId).ToQueryString();
    Check(sql.Contains("NOT EXISTS", StringComparison.OrdinalIgnoreCase)
        && sql.Contains("TOP(", StringComparison.OrdinalIgnoreCase)
        && sql.Contains("ORDER BY", StringComparison.OrdinalIgnoreCase)
        && sql.Contains("[ShopId]", StringComparison.Ordinal),
        $"production scheduling query translates to SQL Server (connection filter: {connection.HasValue})");
}
Console.WriteLine($"{checks} queue scheduling checks passed. In-memory ordering and SQL translation only; no SQL execution, lock concurrency, HTTP or financial posting tested.");

sealed class SchedulingContext(DbContextOptions<SchedulingContext> options) : DbContext(options)
{
    public DbSet<IntegrationScenarioJob> Jobs => Set<IntegrationScenarioJob>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IntegrationScenarioJob>().ToTable("IntegrationScenarioJobs", "dbo");
    }
}
