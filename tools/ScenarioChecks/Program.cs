using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using Hyper.Integration.Domain.Entities.Integrations;
using Hyper.Integration.Domain.Features.Integrations;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Infrastructure.Features.Integrations;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var checks = 0;
void Check(bool result, string name) { if (!result) throw new Exception(name); checks++; Console.WriteLine("PASS " + name); }
async Task Reject(Func<Task> act, string name) { try { await act(); } catch (ArgumentException) { Check(true, name); return; } catch (InvalidOperationException) { Check(true, name); return; } throw new Exception(name); }

static string SignCustomWebhook(long connectionId, string secret, string timestamp, string eventId, string eventType, byte[] body)
{
    var prefix = $"{IntegrationWebhookVerifier.Scheme}\n{connectionId}\n{timestamp}\n{eventId}\n{eventType}\n";
    using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
    using var payload = new MemoryStream(Encoding.UTF8.GetBytes(prefix).Length + body.Length);
    payload.Write(Encoding.UTF8.GetBytes(prefix));
    payload.Write(body);
    return Convert.ToHexString(hmac.ComputeHash(payload.ToArray())).ToLowerInvariant();
}

var customSecret = "01234567890123456789012345678901";
var customConnection = new ExternalIntegrationConnection
{
    Id = 42, ShopId = 100, TenantId = "tenant-a", Provider = IntegrationProvider.Custom,
    AccountIdentifier = "custom-42", CredentialsJson = $"{{\"webhookSignatureScheme\":\"{IntegrationWebhookVerifier.Scheme}\",\"webhookSecret\":\"{customSecret}\"}}",
    IsEnabled = true
};
var customBody = Encoding.UTF8.GetBytes("{\"productId\":7}");
var customTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
var customEventId = "security-event-1";
var customEventType = "product.updated";
var customSignature = SignCustomWebhook(customConnection.Id, customSecret, customTimestamp, customEventId, customEventType, customBody);
var customVerifier = new IntegrationWebhookVerifier();
var customRequest = new IntegrationWebhookRequest(customBody, customEventId, customEventType, customTimestamp, customSignature, null);
Check(customVerifier.Verify(customConnection, customRequest, DateTimeOffset.UtcNow) == WebhookValidationResult.Valid,
    "custom HMAC signature accepted");
Check(customVerifier.Verify(customConnection, customRequest with { Body = Encoding.UTF8.GetBytes("{\"productId\":8}") }, DateTimeOffset.UtcNow)
    == WebhookValidationResult.Invalid, "tampered webhook body rejected");
Check(customVerifier.Verify(customConnection, customRequest with { EventId = "security-event-2" }, DateTimeOffset.UtcNow)
    == WebhookValidationResult.Invalid, "event-id replay substitution rejected");
Check(customVerifier.Verify(customConnection, customRequest with { Timestamp = (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 301).ToString() }, DateTimeOffset.UtcNow)
    == WebhookValidationResult.Invalid, "expired webhook timestamp rejected");
Check(customVerifier.Verify(new ExternalIntegrationConnection
{
    Id = 43, ShopId = customConnection.ShopId, TenantId = "tenant-b", Provider = customConnection.Provider,
    AccountIdentifier = customConnection.AccountIdentifier, CredentialsJson = customConnection.CredentialsJson, IsEnabled = true
}, customRequest, DateTimeOffset.UtcNow)
    == WebhookValidationResult.Invalid, "signature cannot cross connection or tenant boundary");
Check(customVerifier.Verify(new ExternalIntegrationConnection
{
    Id = customConnection.Id, ShopId = customConnection.ShopId, TenantId = customConnection.TenantId,
    Provider = customConnection.Provider, AccountIdentifier = customConnection.AccountIdentifier,
    CredentialsJson = customConnection.CredentialsJson, IsEnabled = false
}, customRequest, DateTimeOffset.UtcNow)
    == WebhookValidationResult.Invalid, "disabled connection webhook rejected");
Check(customVerifier.Verify(new ExternalIntegrationConnection
{
    Id = customConnection.Id, ShopId = customConnection.ShopId, TenantId = customConnection.TenantId,
    Provider = customConnection.Provider, AccountIdentifier = customConnection.AccountIdentifier,
    CredentialsJson = "{\"webhookSignatureScheme\":\"other\",\"webhookSecret\":\"01234567890123456789012345678901\"}", IsEnabled = true
}, customRequest, DateTimeOffset.UtcNow)
    == WebhookValidationResult.Unsupported, "unknown webhook scheme rejected as unsupported");
Check(customVerifier.Verify(new ExternalIntegrationConnection
{
    Id = customConnection.Id, ShopId = customConnection.ShopId, TenantId = customConnection.TenantId,
    Provider = customConnection.Provider, AccountIdentifier = customConnection.AccountIdentifier,
    CredentialsJson = "{\"webhookSignatureScheme\":\"hyper-hmac-v1\",\"webhookSecret\":\"short\"}", IsEnabled = true
}, customRequest, DateTimeOffset.UtcNow)
    == WebhookValidationResult.Invalid, "short webhook secret rejected");
Check(typeof(IntegrationConnectionCreateRequest).GetProperty("CredentialsJson") is null
    && typeof(IntegrationConnectionSummary).GetProperty("CredentialsJson") is null,
    "management contracts do not expose credentials");

var scope = new OwnedIntegrationShop(100, "shop:100");
var webhookConnection = new ExternalIntegrationConnection
{
    Id = 7, ShopId = 100, TenantId = "shop:100", Provider = IntegrationProvider.Basalam,
    AccountIdentifier = "9001", CredentialsJson = "{\"webhookSecret\":\"secret-1\"}", IsEnabled = true
};
var webhookVerifier = new IntegrationWebhookVerifier();
var validWebhook = new IntegrationWebhookRequest([], "evt-1", "VENDOR_NEW_ORDER", null, null, "Bearer secret-1");
Check(webhookVerifier.Verify(webhookConnection, validWebhook, DateTimeOffset.UtcNow) == WebhookValidationResult.Valid,
    "Basalam webhook authorization accepted");
Check(webhookVerifier.Verify(webhookConnection, validWebhook with { Authorization = "Bearer wrong" }, DateTimeOffset.UtcNow)
    == WebhookValidationResult.Invalid, "Basalam webhook authorization rejected");
var missingSecretConnection = new ExternalIntegrationConnection
{
    Id = webhookConnection.Id, ShopId = webhookConnection.ShopId, TenantId = webhookConnection.TenantId,
    Provider = webhookConnection.Provider, AccountIdentifier = webhookConnection.AccountIdentifier,
    CredentialsJson = "{}", IsEnabled = webhookConnection.IsEnabled
};
Check(webhookVerifier.Verify(missingSecretConnection, validWebhook, DateTimeOffset.UtcNow)
    == WebhookValidationResult.Invalid, "Basalam webhook without connection secret rejected");
await Reject(() => { IntegrationScenarioRules.Validate(scope, 1, new("event", (IntegrationSyncItem)99, IntegrationSyncTrigger.Manual)); return Task.CompletedTask; }, "unknown item rejected");
await Reject(() => { IntegrationScenarioRules.Validate(scope, 1, new(" event", IntegrationSyncItem.Product, IntegrationSyncTrigger.Manual)); return Task.CompletedTask; }, "noncanonical event rejected");
var local = new[] { new IntegrationLocalProduct(1, "One", 4), new IntegrationLocalProduct(2, "Two", 0) };
var remote = new[] { new ExternalCatalogItem("11", null, "Changed", null, 8, null), new ExternalCatalogItem("12", null, "Remote", null, 1, null) };
var mappings = new[] { new ExternalProductMapping { Id=1, HyperProductId=1, ExternalProductId="11" } };
var comparison = IntegrationCatalogComparison.Compare(local, remote, mappings, IntegrationSyncItem.Product);
Check(comparison.Compared == 1 && comparison.Differences.Any(x => x.Code == "ProductTitleMismatch"), "title drift found");
Check(comparison.Differences.Any(x => x.Code == "LocalProductUnmapped") && comparison.Differences.Any(x => x.Code == "ExternalProductUnmapped"), "unmapped detected in both sources");
Check(IntegrationCatalogComparison.Compare(local, remote, mappings, IntegrationSyncItem.Inventory).Differences.Any(x => x.Code == "InventoryMismatch"), "stock drift found without store change");
Check(IntegrationCatalogComparison.Compare(local, [], mappings, IntegrationSyncItem.Product).Differences.Any(x => x.Code == "ExternalProductMissing"), "remote deletion found");
Check(IntegrationCatalogComparison.Compare([], remote, mappings, IntegrationSyncItem.Product).Differences.Any(x => x.Code == "LocalProductMissing"), "local deletion found");
Check(IntegrationCatalogComparison.Compare(local, remote, [mappings[0], mappings[0]], IntegrationSyncItem.Product).Differences.Any(x => x.Code == "DuplicateMapping"), "duplicate mapping not merged");
Check(IntegrationAvailableInventory.Calculate(3,5,true)==0 && IntegrationAvailableInventory.Calculate(3,0,false)==0, "stock floor and availability applied");
Check(IntegrationRetryPolicy.Delay(1,TimeSpan.FromMinutes(4))==TimeSpan.FromMinutes(4), "provider retry-after honored");
if (args.Length > 0)
{
    using var config = JsonDocument.Parse(File.ReadAllText(args[0]));
    var cs = config.RootElement.GetProperty("ConnectionStrings").GetProperty("DomainCommandConnection").GetString();
    var builder = new SqlConnectionStringBuilder(cs) { DataSource="lpc:.", Encrypt=false };
    await using var db = new HyperIntegrationContext(new DbContextOptionsBuilder<HyperIntegrationContext>().UseSqlServer(builder.ConnectionString).Options);
    await using var tx = await db.Database.BeginTransactionAsync();
    var adapter = new FakeAdapter();
    var processor = new IntegrationScenarioProcessor(db, new IntegrationStrategyResolver([adapter]), new FakeInventory(),
        Options.Create(new IntegrationInventoryCaptureOptions()), Options.Create(new BasalamOAuthSettings()),
        null!, null!, null!);
    var queue = new IntegrationScenarioQueue(db, processor);
    var conn = new ExternalIntegrationConnection { ShopId=int.MaxValue-1, TenantId="scenario-test", Provider=IntegrationProvider.Custom,
        DisplayName="Rollback scenario test", AccountIdentifier=Guid.NewGuid().ToString("N"), CredentialType=IntegrationCredentialType.BearerToken, CredentialsJson="{}", IsEnabled=true };
    db.ExternalIntegrationConnections.Add(conn); await db.SaveChangesAsync(); db.Entry(conn).State=EntityState.Detached;
    var owned = new OwnedIntegrationShop(conn.ShopId,conn.TenantId);
    var request = new IntegrationScenarioRequest("scenario:1",IntegrationSyncItem.Product,IntegrationSyncTrigger.Manual);
    var jobId=await queue.EnqueueAsync(owned,conn.Id,request,default);
    Check(jobId==await queue.EnqueueAsync(owned,conn.Id,request,default), "SQL replay returns same job");
    await Reject(() => queue.EnqueueAsync(owned,conn.Id,request with { Item=IntegrationSyncItem.Sale },default), "SQL conflicting replay rejected");
    await Reject(() => queue.EnqueueAsync(owned with { TenantId="other" },conn.Id,request,default), "SQL cross tenant rejected");
    await Reject(() => queue.EnqueueAsync(owned with { ShopId=100 },conn.Id,request,default), "SQL cross shop rejected");
    Check((await queue.RecentAsync(new(100,"other"),default)).All(x=>x.Id!=jobId), "SQL status query tenant scoped");
    Check(await queue.ProcessConnectionAsync(conn.Id,default), "SQL worker processes request");
    var job=await db.IntegrationScenarioJobs.AsNoTracking().SingleAsync(x=>x.Id==jobId);
    Check(job.Status==IntegrationScenarioStatus.Completed && job.LeaseId is null && adapter.Calls==1, "SQL success acknowledged and lease released");
    Check(!await queue.ProcessConnectionAsync(conn.Id,default) && adapter.Calls==1, "SQL completed request not redelivered");
    var unsupported=await queue.EnqueueAsync(owned,conn.Id,request with { EventId="scenario:2",Item=IntegrationSyncItem.Sale },default);
    await queue.ProcessConnectionAsync(conn.Id,default);
    Check((await db.IntegrationScenarioJobs.AsNoTracking().SingleAsync(x=>x.Id==unsupported)).ErrorCode=="BusinessCommandNotImplemented", "SQL unsupported accounting never successful");
    var recovery=await queue.EnqueueAsync(owned,conn.Id,request with { EventId="scenario:3" },default);
    await db.IntegrationScenarioJobs.Where(x=>x.Id==recovery).ExecuteUpdateAsync(s=>s.SetProperty(x=>x.Status,IntegrationScenarioStatus.Running).SetProperty(x=>x.LeaseId,Guid.NewGuid()).SetProperty(x=>x.LeaseExpiresAtUtc,DateTime.UtcNow.AddMinutes(-10)));
    Check(await queue.ProcessConnectionAsync(conn.Id,default), "SQL abandoned lease recovered");
    adapter.Fail=true;
    var retry=await queue.EnqueueAsync(owned,conn.Id,request with { EventId="scenario:4" },default);
    await queue.ProcessConnectionAsync(conn.Id,default);
    var retryJob=await db.IntegrationScenarioJobs.AsNoTracking().SingleAsync(x=>x.Id==retry);
    Check(retryJob.Status==IntegrationScenarioStatus.Pending && retryJob.Attempts==1 && retryJob.NextAttemptAtUtc>DateTime.UtcNow, "SQL transient error scheduled for retry");
    Check(!await queue.ProcessConnectionAsync(conn.Id,default), "SQL backoff prevents early retry");
    var inventoryJob=await queue.EnqueueAsync(owned,conn.Id,request with { EventId="scenario:5",Item=IntegrationSyncItem.Inventory },default);
    await db.IntegrationScenarioJobs.Where(x=>x.Id==retry).ExecuteUpdateAsync(s=>s.SetProperty(x=>x.Status,IntegrationScenarioStatus.DeadLetter));
    await queue.ProcessConnectionAsync(conn.Id,default);
    Check((await db.IntegrationScenarioJobs.AsNoTracking().SingleAsync(x=>x.Id==inventoryJob)).ErrorCode=="AccountingStockSourceUnverified", "SQL unverified stock source blocked");
    await tx.RollbackAsync();
    Console.WriteLine("All SQL fixtures rolled back. No external HTTP calls.");
}
Console.WriteLine($"{checks} checks passed.");
sealed class FakeInventory : IIntegrationInventoryCapture
{
    public Task<int> CaptureAsync(CancellationToken ct)=>Task.FromResult(0);
    public Task<bool> ReconcileOneAsync(long connectionId,long mappingId,CancellationToken ct)=>Task.FromResult(false);
}
sealed class FakeAdapter : IExternalIntegrationAdapter
{
    public int Calls; public bool Fail;
    public IntegrationProvider Provider=>IntegrationProvider.Custom;
    public bool IsImplemented=>true;
    public bool SupportsCredentialType(IntegrationCredentialType t)=>t==IntegrationCredentialType.BearerToken;
    public Task<IReadOnlyCollection<ExternalCatalogItem>> ReadCatalogAsync(ExternalIntegrationConnection c,CancellationToken ct)
    { Calls++; if(Fail)throw new IntegrationProviderException("Http429",true); return Task.FromResult<IReadOnlyCollection<ExternalCatalogItem>>([]); }
    public Task PublishInventoryAsync(ExternalIntegrationConnection c,IReadOnlyCollection<ExternalInventoryUpdate> u,CancellationToken ct)=>throw new Exception("Unexpected external write");
}

