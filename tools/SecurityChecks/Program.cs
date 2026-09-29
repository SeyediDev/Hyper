using System.Data.Common;
using System.Globalization;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Infrastructure.Features.Integrations;
using Hyper.Integration.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ContractProvider = Hyper.Integration.Contracts.IntegrationProvider;
using DomainProvider = Hyper.Integration.Domain.Entities.Integrations.IntegrationProvider;

var passed = 0;
var failures = new List<string>();
async Task Test(string name, Func<Task<bool>> action)
{
    try
    {
        if (!await action()) throw new InvalidOperationException("AssertionFailed");
        Console.WriteLine("PASS " + name);
        passed++;
    }
    catch (Exception ex)
    {
        // Never print credentials, payloads, or exception messages from providers.
        failures.Add(name);
        Console.WriteLine($"FAIL {name} ({ex.GetType().Name})");
    }
}
Task Check(string name, Func<bool> action) => Test(name, () => Task.FromResult(action()));
var now = DateTimeOffset.FromUnixTimeSeconds(1_800_000_000);
const string secret = "fixture-key-0123456789-ABCDEFGHIJK";
var verifier = new IntegrationWebhookVerifier(FixtureProtection.LegacyVault());
ExternalIntegrationConnection Connection(long id = 42) => new()
{
    Id = id, ShopId = 7, TenantId = "tenant-a", Provider = DomainProvider.Custom,
    AccountIdentifier = "fixture-" + id, DisplayName = "Security fixture", IsEnabled = true,
    CredentialsJson = JsonSerializer.Serialize(new { webhookSignatureScheme = "hyper-hmac-v1", webhookSecret = secret })
};
IntegrationWebhookRequest Sign(long id, DateTimeOffset time, string eventId = "event-1", string type = "product.updated", string body = "{}")
{
    var timestamp = time.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
    var bytes = Encoding.UTF8.GetBytes(body);
    var prefix = Encoding.UTF8.GetBytes($"hyper-hmac-v1\n{id.ToString(CultureInfo.InvariantCulture)}\n{timestamp}\n{eventId}\n{type}\n");
    byte[] message = [.. prefix, .. bytes];
    var signature = Convert.ToHexString(HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), message));
    return new(bytes, eventId, type, timestamp, signature);
}
var connection = Connection();
var signed = Sign(connection.Id, now);
await Check("valid independently signed HMAC", () => verifier.Verify(connection, signed, now) == WebhookValidationResult.Valid);
foreach (var seconds in new[] { -301, -300, 0, 300, 301 })
    await Check($"signed timestamp offset {seconds}", () => verifier.Verify(connection, Sign(connection.Id, now.AddSeconds(seconds)), now)
        == (Math.Abs(seconds) <= 300 ? WebhookValidationResult.Valid : WebhookValidationResult.Invalid));
foreach (var altered in new[]
{
    signed with { Body = Encoding.UTF8.GetBytes("{\"changed\":true}") },
    signed with { EventId = "event-2" }, signed with { EventType = "inventory.updated" },
    signed with { Timestamp = now.AddSeconds(1).ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture) }
}.Select((request, index) => (request, index)))
    await Check($"signed field tampering {altered.index}", () => verifier.Verify(connection, altered.request, now) == WebhookValidationResult.Invalid);
await Check("signature bound to connection", () => verifier.Verify(Connection(43), signed, now) == WebhookValidationResult.Invalid);
foreach (var timestamp in new[] { "-1", " 1800000000", "9223372036854775807", "9223372036854775808", "abc" })
    await Check($"invalid timestamp {timestamp}", () => verifier.Verify(connection, signed with { Timestamp = timestamp }, now) == WebhookValidationResult.Invalid);
foreach (var signature in new string?[] { null, "", "00", new('Z', 64) })
    await Check("malformed signature", () => verifier.Verify(connection, signed with { Signature = signature }, now) == WebhookValidationResult.Invalid);
await Check("control characters in event identity", () => verifier.Verify(connection, Sign(42, now, "event\n2"), now) == WebhookValidationResult.Invalid);
await Check("oversized custom body", () => verifier.Verify(connection, Sign(42, now, body: new string('x', 1024 * 1024 + 1)), now) == WebhookValidationResult.Invalid);
connection.IsEnabled = false;
await Check("disabled connection rejected", () => verifier.Verify(connection, signed, now) == WebhookValidationResult.Invalid);
connection.IsEnabled = true;

foreach (var credentials in new[] { "[]", "null", "42", "\"text\"", "{", "{}", "{\"webhookSecret\":\"\"}", "{\"webhookSecret\":\"   \"}" })
{
    var basalam = Connection();
    basalam.Provider = DomainProvider.Basalam;
    basalam.CredentialsJson = credentials;
    await Check("Basalam invalid credential shape or empty secret", () => verifier.Verify(basalam,
        signed with { Authorization = credentials.Contains("   ") ? "Bearer    " : "Bearer " }, now) == WebhookValidationResult.Invalid);
}
var bearerConnection = Connection();
bearerConnection.Provider = DomainProvider.Basalam;
bearerConnection.CredentialsJson = "{\"webhookSecret\":\"fixture-bearer\"}";
await Check("Basalam configured bearer accepted", () => verifier.Verify(bearerConnection, signed with { Authorization = "Bearer fixture-bearer" }, now) == WebhookValidationResult.Valid);
await Check("Basalam wrong bearer rejected", () => verifier.Verify(bearerConnection, signed with { Authorization = "Bearer other" }, now) == WebhookValidationResult.Invalid);

foreach (var body in new[] { "[]", "null", "42", "\"text\"", "true" })
    await Check("source extraction tolerates non-object JSON " + body, () =>
    {
        using var doc = JsonDocument.Parse(body);
        return IntegrationSourceRules.ReadSource(doc.RootElement) is null;
    });
var noSqlOptions = new DbContextOptionsBuilder<HyperIntegrationContext>()
    .UseSqlServer("Server=localhost;Database=NeverOpenedSecurityChecks;Integrated Security=true")
    .AddInterceptors(new RejectDatabaseConnection()).Options;
await using (var db = new HyperIntegrationContext(noSqlOptions))
{
    foreach (var body in new[] { "[]", "null", "42", "\"text\"", "true", "{", "" })
        await Test("invalid payload rejected before persistence " + body, async () =>
            (await new IntegrationWebhookIngress(db, verifier).ReceiveAsync(new(ContractProvider.Custom,
                "fixture-42", "event-1", "product.updated", signed.Timestamp, signed.Signature, Encoding.UTF8.GetBytes(body))))
            .Status == WebhookIngressStatus.Invalid);
}
var authorization = new IntegrationScopeAuthorization();
ClaimsPrincipal Principal(string claim, bool authenticated = true) =>
    new(new ClaimsIdentity([new Claim("integration_scope", claim)], authenticated ? "fixture" : null));
await Test("authenticated matching tenant/shop allowed", () => authorization.CanAccessAsync(Principal("shop:7;tenant:tenant-a"), 7, "tenant-a"));
foreach (var candidate in new[] { Principal("shop:8;tenant:tenant-a"), Principal("shop:7;tenant:tenant-b"),
    Principal("shop:7;tenant:tenant-a", false), Principal("shop:*;tenant:tenant-a"), Principal("shop:7;tenant:tenant-a extra") })
    await Test("mismatched or unauthenticated scope denied", async () => !await authorization.CanAccessAsync(candidate, 7, "tenant-a"));
await Test("admin role does not grant arbitrary scope", async () => !await authorization.CanAccessAsync(
    new(new ClaimsIdentity([new Claim(ClaimTypes.Role, "Admin")], "fixture")), 7, "tenant-a"));
foreach (var type in new[] { typeof(IntegrationConnectionSummary), typeof(IntegrationConnectionCreateRequest), typeof(IntegrationTokenStatus) })
    await Check(type.Name + " excludes secret fields", () => !type.GetProperties().Any(p =>
        new[] { "CredentialsJson", "AccessToken", "RefreshToken", "WebhookSecret", "Password" }.Contains(p.Name)));

if (args.Contains("--sql"))
{
    var input = Environment.GetEnvironmentVariable("SECURITY_TEST_SQL") ?? throw new InvalidOperationException("SECURITY_TEST_SQL required");
    var database = "HyperSecurityChecks_" + Guid.NewGuid().ToString("N");
    var cs = new SqlConnectionStringBuilder(input) { InitialCatalog = database };
    var options = new DbContextOptionsBuilder<HyperIntegrationContext>().UseSqlServer(cs.ConnectionString)
        .ReplaceService<IModelCustomizer, FixtureSchema>().Options;
    await using var setup = new HyperIntegrationContext(options);
    try
    {
        await setup.Database.EnsureCreatedAsync();
        var first = Connection(0);
        first.AccountIdentifier = "first";
        var second = Connection(0);
        second.AccountIdentifier = "second"; second.ShopId = 8; second.TenantId = "tenant-b";
        setup.AddRange(first, second);
        await setup.SaveChangesAsync();
        WebhookIngressRequest Request(ExternalIntegrationConnection c, string eventId, string body = "{}")
        {
            var value = Sign(c.Id, DateTimeOffset.UtcNow, eventId, body: body);
            return new(ContractProvider.Custom, c.AccountIdentifier, value.EventId, value.EventType, value.Timestamp, value.Signature, value.Body);
        }
        async Task<WebhookIngressResult> Receive(WebhookIngressRequest request)
        {
            await using var db = new HyperIntegrationContext(options);
            return await new IntegrationWebhookIngress(db, verifier).ReceiveAsync(request);
        }
        var request = Request(first, "sql-event", "{\"sensitive\":\"fixture-private-body\"}");
        await Test("SQL accepted event creates inbox audit and scoped job", async () =>
        {
            var result = await Receive(request);
            var job = await setup.IntegrationScenarioJobs.SingleAsync();
            return result.Status == WebhookIngressStatus.Accepted && job.ShopId == 7 && job.TenantId == "tenant-a"
                && await setup.IntegrationWebhookInbox.CountAsync() == 1 && await setup.IntegrationEventAudits.CountAsync() == 1;
        });
        await Test("SQL replay creates no extra effects", async () =>
            (await Receive(request)).Status == WebhookIngressStatus.Duplicate
            && await setup.IntegrationScenarioJobs.CountAsync() == 1 && await setup.IntegrationEventAudits.CountAsync() == 1);
        await Test("SQL connection substitution cannot reuse signature", async () =>
            (await Receive(request with { ConnectionKey = second.AccountIdentifier })).Status == WebhookIngressStatus.Invalid
            && !await setup.IntegrationScenarioJobs.AnyAsync(x => x.ConnectionId == second.Id));
        await Test("SQL event ID is scoped per connection", async () =>
            (await Receive(Request(second, "sql-event"))).Status == WebhookIngressStatus.Accepted
            && await setup.IntegrationScenarioJobs.AnyAsync(x => x.ConnectionId == second.Id && x.ShopId == 8 && x.TenantId == "tenant-b"));
        await Test("SQL changed replay content is rejected", async () =>
            (await Receive(Request(first, "sql-event", "{\"changed\":true}"))).Status == WebhookIngressStatus.Invalid
            && await setup.IntegrationScenarioJobs.CountAsync(x => x.ConnectionId == first.Id && x.EventId == "sql-event") == 1);
        await Test("SQL shared account route selects credential-owned tenant", async () =>
        {
            var shared = Connection(0);
            shared.AccountIdentifier = first.AccountIdentifier;
            shared.ShopId = 9; shared.TenantId = "tenant-c";
            setup.Add(shared);
            await setup.SaveChangesAsync();
            var result = await Receive(Request(shared, "shared-route"));
            return result.Status == WebhookIngressStatus.Accepted
                && await setup.IntegrationScenarioJobs.AnyAsync(x => x.EventId == "shared-route" && x.ConnectionId == shared.Id
                    && x.ShopId == 9 && x.TenantId == "tenant-c")
                && !await setup.IntegrationScenarioJobs.AnyAsync(x => x.EventId == "shared-route" && x.ConnectionId == first.Id);
        });
        await Test("SQL ambiguous Basalam credentials fail closed", async () =>
        {
            var candidates = new[] { Connection(0), Connection(0) };
            for (var i = 0; i < candidates.Length; i++)
            {
                candidates[i].AccountIdentifier = "shared-basalam";
                candidates[i].ShopId = 20 + i; candidates[i].TenantId = "ambiguous-" + i;
                candidates[i].Provider = DomainProvider.Basalam;
                candidates[i].CredentialsJson = bearerConnection.CredentialsJson;
            }
            setup.AddRange(candidates);
            await setup.SaveChangesAsync();
            var result = await Receive(new(ContractProvider.Basalam, "shared-basalam", "ambiguous-event", "product.updated",
                null, null, Encoding.UTF8.GetBytes("{}"), Authorization: "Bearer fixture-bearer"));
            return result.Status == WebhookIngressStatus.Invalid && result.ErrorCode == "ConnectionKeyAmbiguous"
                && !await setup.IntegrationWebhookInbox.AnyAsync(x => x.ExternalEventId == "ambiguous-event");
        });
        await Test("SQL audit stores digest without payload or credential", async () =>
        {
            var audit = await setup.IntegrationEventAudits.SingleAsync(x => x.ConnectionId == first.Id);
            var serialized = JsonSerializer.Serialize(audit);
            return audit.PayloadHash == Convert.ToHexString(SHA256.HashData(request.Body))
                && audit.SignatureValid && !serialized.Contains(secret) && !serialized.Contains("fixture-private-body");
        });
        await Test("SQL malformed signed body produces no rows", async () =>
        {
            var before = await setup.IntegrationWebhookInbox.CountAsync();
            var result = await Receive(Request(first, "scalar", "[]"));
            return result.Status == WebhookIngressStatus.Invalid && await setup.IntegrationWebhookInbox.CountAsync() == before;
        });
        await Test("SQL loopback retained without a business job", async () =>
        {
            var result = await Receive(Request(first, "loopback", "{\"metadata\":{\"sync_source\":\"Hyperyek\"}}"));
            return result.Status == WebhookIngressStatus.Accepted && result.ScenarioJobId is null
                && await setup.IntegrationWebhookInbox.AnyAsync(x => x.ExternalEventId == "loopback")
                && await setup.IntegrationEventAudits.AnyAsync(x => x.ExternalEventId == "loopback")
                && !await setup.IntegrationScenarioJobs.AnyAsync(x => x.EventId == "loopback");
        });
        await Test("SQL concurrent duplicate delivers exactly once", async () =>
        {
            var concurrent = Request(first, "race");
            var results = await Task.WhenAll(Receive(concurrent), Receive(concurrent));
            return results.Count(x => x.Status == WebhookIngressStatus.Accepted) == 1
                && results.Count(x => x.Status == WebhookIngressStatus.Duplicate) == 1
                && await setup.IntegrationWebhookInbox.CountAsync(x => x.ExternalEventId == "race") == 1
                && await setup.IntegrationEventAudits.CountAsync(x => x.ExternalEventId == "race") == 1
                && await setup.IntegrationScenarioJobs.CountAsync(x => x.EventId == "race") == 1;
        });
    }
    finally
    {
        if (setup.Database.GetDbConnection().Database != database || !database.StartsWith("HyperSecurityChecks_", StringComparison.Ordinal))
            throw new InvalidOperationException("FixtureCleanupTargetMismatch");
        await setup.Database.EnsureDeletedAsync();
        Console.WriteLine("Disposable security database removed.");
    }
}
else Console.WriteLine("SQL checks skipped; use --sql with SECURITY_TEST_SQL.");
Console.WriteLine($"{passed} passed; {failures.Count} failed.");
return failures.Count == 0 ? 0 : 1;

sealed class RejectDatabaseConnection : DbConnectionInterceptor
{
    public override ValueTask<InterceptionResult> ConnectionOpeningAsync(DbConnection connection, ConnectionEventData eventData,
        InterceptionResult result, CancellationToken cancellationToken = default) => throw new InvalidOperationException("UnexpectedDatabaseAccess");
}
sealed class FixtureSchema(ModelCustomizerDependencies dependencies) : ModelCustomizer(dependencies)
{
    public override void Customize(ModelBuilder builder, DbContext context)
    {
        base.Customize(builder, context);
        foreach (var entity in builder.Model.GetEntityTypes()) entity.SetIsTableExcludedFromMigrations(false);
    }
}
