using System.Text.Json.Nodes;
using Hyper.AdminPanel.Web.Infrastructure;
using Hyper.AdminPanel.Web.ViewModels;
using Hyper.Integration.Domain.Entities.Integrations;
using Hyper.Integration.Domain.Features.Integrations;
using Microsoft.AspNetCore.DataProtection;

var checks = 0;
void Check(bool condition, string name) { if (!condition) throw new Exception(name); checks++; Console.WriteLine("PASS " + name); }
var tickets = new AdminSimulationTickets(new EphemeralDataProtectionProvider());
var simulations = new Simulations();
var connections = new Connections();
var financial = new Financial();
var workflow = new AdminFinancialPreviewWorkflow(simulations, tickets, connections, financial);
const string admin = "admin-fixture";
var selected = new IntegrationAdminSimulation
{
    AdminUserId = admin, ShopId = 7, TenantId = "tenant-a", ShopName = "Fixture shop",
    MerchantIdentifier = "fixture", ExpiresAtUtc = DateTime.UtcNow.AddMinutes(20)
};
simulations.Selected = selected;
var cookie = tickets.Protect(admin, selected.Id);
connections.Items = [new(10, "Basalam fixture", IntegrationProvider.Basalam, true),
    new(11, "Disabled", IntegrationProvider.Basalam, false),
    new(12, "Other provider", (IntegrationProvider)250, true)];
FinancialPreviewForm Form() => new() { ContextTicket = cookie, ConnectionId = 10, UnitAcknowledged = true };

var page = await workflow.PrepareAsync(admin, cookie, default);
Check(page.Selected == selected && page.Connections.Count == 1 && page.Form.ConnectionId == 10,
    "page includes only enabled Basalam connections in selected shop");
Check(connections.LastScope == new OwnedIntegrationShop(7, "tenant-a"), "connection lookup receives server-resolved tenant and shop");
Check(tickets.Read(admin, page.Form.ContextTicket) == selected.Id && !page.Form.UnitAcknowledged,
    "form is bound to current simulation and never pre-acknowledges currency");
Check((await workflow.PrepareAsync(admin, null, default)).Selected is null, "no selection renders empty state");
Check((await workflow.PrepareAsync(admin, "tampered", default)).Selected is null, "tampered ticket rejected");
Check((await workflow.PrepareAsync("another-admin", cookie, default)).Selected is null, "ticket cannot cross administrators");

var result = await workflow.PreviewAsync(admin, cookie, Form(), default);
Check(result.Result?.InvoiceTotal == 9_600_000 && result.StatusCode == 200 && financial.Calls == 1,
    "manual preview invokes financial port and returns its result");
var sent = financial.Last!;
Check(sent.ShopId == 7 && sent.TenantId == "tenant-a" && sent.ConnectionId == 10 && sent.PolicyVersion == "irr-preview-v1",
    "financial request scope and policy are supplied by server");
Check(sent.SourceUnitVerified && sent.SourceUnit == IntegrationMoneyUnit.IRR && sent.PlatformCommission == 900_000,
    "explicit manual unit acknowledgement and sample amounts preserved");
var unverified = Form(); unverified.UnitAcknowledged = false;
await workflow.PreviewAsync(admin, cookie, unverified, default);
Check(!financial.Last!.SourceUnitVerified, "unchecked currency acknowledgement remains false");
var unknown = Form(); unknown.DraftJson = unknown.DraftJson.Replace("\"platformCommission\": 900000", "\"platformCommission\": null");
await workflow.PreviewAsync(admin, cookie, unknown, default);
Check(financial.Last!.PlatformCommission is null, "null input commission is not replaced by zero");

async Task Deny(FinancialPreviewForm form, string? currentCookie, int status, string name)
{
    var before = financial.Calls;
    var denied = await workflow.PreviewAsync(admin, currentCookie, form, default);
    Check(denied.StatusCode == status && denied.Result is null && denied.Error is not null && financial.Calls == before, name);
}
await Deny(Form(), tickets.Protect(admin, Guid.NewGuid()), 409, "another-tab shop change rejects old form before HTTP");
await Deny(Form(), null, 409, "missing current cookie rejects posted ticket");
var forged = Form(); forged.ContextTicket = "tampered";
await Deny(forged, cookie, 409, "forged form ticket cannot invoke accounting");
foreach (var id in new long[] { 0, 11, 12, 999 })
{
    var form = Form(); form.ConnectionId = id;
    await Deny(form, cookie, 409, "inactive/foreign/unknown connection rejected: " + id);
}
foreach (var json in new[] { "", "null", "[]", "{bad", new string('x', 100001),
    FinancialPreviewDraft.Sample.Replace("\"sourceVersion\": 1,", "\"sourceVersion\": 1, \"shopId\": 999,"),
    FinancialPreviewDraft.Sample.Replace("\"sourceVersion\": 1,", "\"sourceVersion\": 1, \"tenantId\": \"foreign\","),
    FinancialPreviewDraft.Sample.Replace("\"sourceVersion\": 1,", "\"sourceVersion\": 1, \"sourceUnitVerified\": true,"),
    FinancialPreviewDraft.Sample.Replace("\"quantity\": 1,", "\"quantity\": 1, \"quantity\": 99,") })
{
    var form = Form(); form.DraftJson = json;
    await Deny(form, cookie, 400, "malformed/oversize/unknown/duplicate JSON cannot invoke accounting");
}
foreach (var lineJson in new[] { "null", "[]", "[null]", "[" + string.Join(',', Enumerable.Repeat("{}", 1001)) + "]" })
{
    var form = Form(); var node = JsonNode.Parse(form.DraftJson)!; node["lines"] = JsonNode.Parse(lineJson); form.DraftJson = node.ToJsonString();
    await Deny(form, cookie, 400, "invalid line collection rejected before accounting");
}
selected.ExpiresAtUtc = DateTime.UtcNow.AddMinutes(-1);
await Deny(Form(), cookie, 409, "expired simulation rejected even if simulation reader returns it");
selected.ExpiresAtUtc = DateTime.UtcNow.AddMinutes(20); selected.EndedAtUtc = DateTime.UtcNow;
await Deny(Form(), cookie, 409, "ended simulation rejected");
selected.EndedAtUtc = null; selected.AdminUserId = "other-admin";
await Deny(Form(), cookie, 409, "simulation row belonging to another admin rejected");
selected.AdminUserId = admin; selected.TenantId = "";
await Deny(Form(), cookie, 409, "empty tenant rejected");
selected.TenantId = "tenant-a";

financial.Failure = new IntegrationProviderException("secret-provider-response", true);
var unavailable = await workflow.PreviewAsync(admin, cookie, Form(), default);
Check(unavailable.StatusCode == 502 && unavailable.Result is null && !unavailable.Error!.Contains("secret"),
    "provider transient failure displays safe retry guidance without raw error");
financial.Failure = new IntegrationProviderException("secret-provider-response", false);
var rejected = await workflow.PreviewAsync(admin, cookie, Form(), default);
Check(rejected.StatusCode == 502 && !rejected.Error!.Contains("secret"), "permanent service failure is sanitized");
financial.Failure = new InvalidOperationException("secret-configuration");
var config = await workflow.PreviewAsync(admin, cookie, Form(), default);
Check(config.StatusCode == 503 && !config.Error!.Contains("secret"), "missing secure service setup produces actionable sanitized response");
financial.Failure = new OperationCanceledException();
try { await workflow.PreviewAsync(admin, cookie, Form(), default); throw new Exception("Cancellation swallowed"); }
catch (OperationCanceledException) { Check(true, "request cancellation propagates"); }
financial.Failure = null;
simulations.Failure = new InvalidOperationException("secret-missing-accounting-configuration");
var prepareConfig = await workflow.PrepareAsync(admin, cookie, default);
Check(prepareConfig.StatusCode == 503 && prepareConfig.Selected is null && !prepareConfig.Error!.Contains("secret"),
    "initial context lookup with missing service configuration renders safe guidance");
var postConfig = await workflow.PreviewAsync(admin, cookie, Form(), default);
Check(postConfig.StatusCode == 503 && postConfig.Result is null,
    "POST preserves context service failure rather than misreporting simulation expiry");
simulations.Failure = new HttpRequestException("secret-upstream-body");
var prepareUnavailable = await workflow.PrepareAsync(admin, cookie, default);
Check(prepareUnavailable.StatusCode == 502 && !prepareUnavailable.Error!.Contains("secret"),
    "upstream shop read HTTP failure is sanitized before rendering");
simulations.Failure = new OperationCanceledException();
try { await workflow.PrepareAsync(admin, cookie, default); throw new Exception("Cancellation swallowed"); }
catch (OperationCanceledException) { Check(true, "context lookup cancellation propagates"); }
simulations.Failure = null;
financial.AfterCall = () => selected.EndedAtUtc = DateTime.UtcNow;
var endedDuringCall = await workflow.PreviewAsync(admin, cookie, Form(), default);
Check(endedDuringCall.StatusCode == 409 && endedDuringCall.Result is null && endedDuringCall.Selected is null,
    "simulation ended during network call cannot display stale financial result");
Check(simulations.Writes == 0 && connections.Writes == 0, "preview never changes simulation, requests token or queues business events");
Console.WriteLine($"{checks} financial-preview panel workflow checks passed. Fake ports and ephemeral tickets; no SQL, live provider or browser test.");

sealed class Financial : IIntegrationFinancialPreviewPort
{
    public int Calls; public IntegrationFinancialPreviewRequest? Last; public Exception? Failure; public Action? AfterCall;
    public Task<IntegrationFinancialPreviewResult> PreviewAsync(IntegrationFinancialPreviewRequest request, CancellationToken ct)
    {
        Calls++; Last = request; if (Failure is not null) throw Failure; AfterCall?.Invoke();
        return Task.FromResult(new IntegrationFinancialPreviewResult(IntegrationFinancialReviewStatus.Validated, "irr-preview-v1", "IRR",
            [new("one", 10_000_000, 1_000_000, 0, 9_000_000)], 10_000_000, 1_000_000, 9_000_000, 9_600_000, 9_100_000, 8_700_000, [], null));
    }
}
sealed class Simulations : IAdminMerchantSimulationService
{
    public IntegrationAdminSimulation? Selected; public int Writes; public Exception? Failure;
    public Task<IntegrationAdminSimulation?> GetAsync(string adminId, Guid simulationId, CancellationToken ct)
    { if (Failure is not null) throw Failure; return Task.FromResult(Selected); }
    public Task<IReadOnlyList<SimulationShop>> SearchShopsAsync(string? search, CancellationToken ct) => throw new NotSupportedException();
    public Task<IntegrationAdminSimulation?> SelectAsync(string adminId, int shopId, CancellationToken ct) { Writes++; throw new NotSupportedException(); }
    public Task EndAsync(string adminId, Guid simulationId, CancellationToken ct) { Writes++; throw new NotSupportedException(); }
    public Task<IntegrationTokenRequest?> RequestTokenAsync(string adminId, Guid simulationId, int displayedShopId,
        IntegrationProvider provider, IntegrationCredentialType credentialType, CancellationToken ct) { Writes++; throw new NotSupportedException(); }
}
sealed class Connections : IIntegrationScenarioQueue
{
    public IReadOnlyList<IntegrationScenarioConnection> Items = []; public OwnedIntegrationShop? LastScope; public int Writes;
    public Task<IReadOnlyList<IntegrationScenarioConnection>> ConnectionsAsync(OwnedIntegrationShop shop, CancellationToken ct)
    { LastScope = shop; return Task.FromResult(Items); }
    public Task<long> EnqueueAsync(OwnedIntegrationShop shop, long connectionId, IntegrationScenarioRequest request, CancellationToken ct)
    { Writes++; throw new NotSupportedException(); }
    public Task<bool> ProcessNextAsync(CancellationToken ct) { Writes++; throw new NotSupportedException(); }
    public Task<IReadOnlyList<IntegrationScenarioJob>> RecentAsync(OwnedIntegrationShop shop, CancellationToken ct) => throw new NotSupportedException();
}
