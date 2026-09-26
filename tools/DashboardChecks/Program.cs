using System.Text.Json;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Infrastructure.Features.Integrations;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

try
{
var connectionString = Environment.GetEnvironmentVariable("DASHBOARD_CHECK_CONNECTION")
    ?? "Server=localhost;Database=tempdb;Integrated Security=true;TrustServerCertificate=true";
await using var connection = new SqlConnection(connectionString);
await connection.OpenAsync();
await using var db = new HyperIntegrationContext(new DbContextOptionsBuilder<HyperIntegrationContext>()
    .UseSqlServer(connection).Options);
// Run DDL on the open connection (not sp_executesql) to retain local temp tables.
foreach (var batch in System.Text.RegularExpressions.Regex.Split(db.Database.GenerateCreateScript(), @"^\s*GO\s*$",
    System.Text.RegularExpressions.RegexOptions.Multiline | System.Text.RegularExpressions.RegexOptions.IgnoreCase))
{
    if (string.IsNullOrWhiteSpace(batch)) continue;
    await using var ddl = connection.CreateCommand();
    ddl.CommandText = batch;
    await ddl.ExecuteNonQueryAsync();
}
var now = DateTime.UtcNow;
const string payloadSecret = "not-for-dashboard-payload";
for (var id = 1; id <= 3; id++)
    db.ExternalIntegrationConnections.Add(new()
    {
        Id = id, ShopId = id == 3 ? 11 : 10, TenantId = id == 2 ? "other-tenant" : "tenant",
        DisplayName = $"connection-{id}", AccountIdentifier = $"account-{id}",
        Provider = IntegrationProvider.Basalam, CredentialsJson = "not-for-dashboard-credentials"
    });

void Inbox(long id, long conn, byte status, DateTime received) => db.IntegrationWebhookInbox.Add(new()
{
    Id = id, ConnectionId = conn, ExternalEventId = $"event-{id}", EventType = "product.updated",
    Status = status, ReceivedAtUtc = received, ProcessedAtUtc = status == 0 ? null : received.AddSeconds(1),
    PayloadJson = payloadSecret, Error = status == 2 ? "InvalidProduct" : null
});
void Outbox(long id, long conn, byte status) => db.IntegrationOutbox.Add(new()
{
    Id = id, ConnectionId = conn, MappingId = 1, Operation = "inventory.set.v1", PayloadJson = payloadSecret,
    Status = status, Attempts = status == 3 ? 5 : 1, CreatedAtUtc = now, NextAttemptAtUtc = now,
    LastError = status == 3 ? "AttemptsExhausted" : null
});
Inbox(1, 1, 2, now.AddDays(-10));
Outbox(1, 1, 3);
for (var id = 2; id <= 36; id++) { Inbox(id, 1, 1, now); Outbox(id, 1, 2); }
Inbox(100, 2, 2, now.AddDays(1)); Inbox(101, 3, 2, now.AddDays(1));
Outbox(100, 2, 3); Outbox(101, 3, 3);
await db.SaveChangesAsync();
var checks = 0;
void Check(bool condition, string name)
{
    if (!condition) throw new InvalidOperationException(name);
    Console.WriteLine($"PASS {++checks}: {name}");
}
var query = new IntegrationDashboardQuery(db, new Catalog());
var snapshot = await query.GetAsync(10, "tenant", default);
Check(snapshot.Connections == 1 && snapshot.WebhookCount(1) == 35 && snapshot.WebhookCount(2) == 1,
    "aggregates scoped by both shop and tenant");
Check(snapshot.RecentInbox.Count == 30 && snapshot.RecentInbox.Select(x => x.Id)
    .SequenceEqual(Enumerable.Range(7, 30).Reverse().Select(x => (long)x)), "inbox bounded with deterministic time/ID ordering");
Check(snapshot.FailedInbox is [{ Id: 1, Error: "InvalidProduct" }], "old failed inbox survives newer successes and excludes foreign scope");
Check(snapshot.DeadLetterOutbox is [{ Id: 1, Attempts: 5, LastError: "AttemptsExhausted" }], "old dead letter survives newer successes and excludes foreign scope");
Check(snapshot.RecentOutbox.Count == 30 && snapshot.RecentOutbox.All(x => x.ConnectionName == "connection-1"), "outbox bounded and scoped");
Check(snapshot.OutboxCount(3) == 1 && snapshot.OutboxCount(2) == 35, "outbox totals independent of recent row limit");
var api = new IntegrationDashboardApi(query);
var response = await api.GetDashboardAsync(10, "tenant");
Check(response.RecentInbox.Count == 30 && response.FailedInbox.Single().ExternalEventId == "event-1"
    && response.DeadLetterOutbox.Single().Attempts == 5, "API preserves new lists and diagnostic fields");
var json = JsonSerializer.Serialize(response);
Check(!json.Contains(payloadSecret) && !json.Contains("not-for-dashboard-credentials") && !json.Contains("PayloadJson"),
    "API omits raw payload and credentials");
var empty = await query.GetAsync(99, "empty", default);
Check(empty.RecentInbox.Count == 0 && empty.FailedInbox.Count == 0 && empty.DeadLetterOutbox.Count == 0,
    "empty scope returns empty lists");
for (var id = 200; id <= 234; id++) { Inbox(id, 1, 2, now); Outbox(id, 1, 3); }
await db.SaveChangesAsync();
snapshot = await query.GetAsync(10, "tenant", default);
Check(snapshot.WebhookCount(2) == 36 && snapshot.FailedInbox.Count == 30 && snapshot.FailedInbox[0].Id == 234,
    "failed inbox bounded independently of total failures");
Check(snapshot.OutboxCount(3) == 36 && snapshot.DeadLetterOutbox.Count == 30 && snapshot.DeadLetterOutbox[0].Id == 234,
    "dead letters bounded independently of total failures");
Console.WriteLine($"{checks} dashboard checks passed; temporary tables disappear on disconnect.");
return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex);
    return 1;
}

sealed class Catalog : IIntegrationPlatformCatalogPort
{
    public Task<IReadOnlyList<IntegrationPlatformProduct>> GetProductsAsync(int shopId, string tenantId, CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<IntegrationPlatformProduct>>([]);
}
