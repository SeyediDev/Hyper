using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Infrastructure.Features.Integrations;
using Hyper.Integration.Api;
using Hyper.Integration.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Provider = Hyper.Integration.Contracts.IntegrationProvider;

var checks = new Checks();
var scope = new IntegrationScopeAuthorization();
foreach (var claimType in new[] { "integration_scope", "scope" })
{
    var principal = new ClaimsPrincipal(new[]
    {
        new ClaimsIdentity([new Claim(ClaimTypes.Name, "admin-a")], "fixture"),
        new ClaimsIdentity([new Claim(claimType, "shop:7;tenant:tenant-a")])
    });
    await checks.Test("unauthenticated identity cannot supply " + claimType,
        async () => !await scope.CanAccessAsync(principal, 7, "tenant-a"));
}
var invalidTenant = "tenant\u0001a";
await checks.Test("control characters in tenant fail closed", async () =>
    !await scope.CanAccessAsync(new(new ClaimsIdentity([new Claim("integration_scope", "shop:7;tenant:" + invalidTenant)], "fixture")), 7, invalidTenant));

// No SQL needed: the simulation service is a controlled application boundary.
await using (var context = new HyperIntegrationContext(new DbContextOptionsBuilder<HyperIntegrationContext>()
    .UseSqlServer("Server=localhost;Database=NeverOpenedIsolationChecks;Integrated Security=true").Options))
{
    var simulation = new IntegrationAdminSimulation { AdminUserId = "admin-a", ShopId = 7, TenantId = "tenant-b",
        MerchantIdentifier = "fixture", ShopName = "fixture", ExpiresAtUtc = DateTime.UtcNow.AddMinutes(10) };
    var requestCalls = 0;
    var simulations = Probe.Create<IAdminMerchantSimulationService>((method, _) => method.Name switch
    {
        "GetAsync" => Task.FromResult<IntegrationAdminSimulation?>(simulation),
        "RequestTokenAsync" => CountRequest(),
        _ => throw new InvalidOperationException("UnexpectedSimulationCall")
    });
    Task<IntegrationTokenRequest?> CountRequest()
    {
        requestCalls++;
        return Task.FromResult<IntegrationTokenRequest?>(new() { SimulationId = simulation.Id });
    }
    var tokens = new IntegrationTokenApi(context, simulations);
    var command = new IntegrationTokenRequestCommand(simulation.Id, 7, "tenant-a", Provider.Basalam, 1);
    await checks.Test("token request rejects simulation from another tenant before mutation", async () =>
        await tokens.RequestAsync("admin-a", command, default) is null && requestCalls == 0);
    simulation.TenantId = "tenant-a";
    await checks.Test("matching simulation can request token", async () =>
        await tokens.RequestAsync("admin-a", command, default) is not null);
}

var builder = WebApplication.CreateBuilder(new WebApplicationOptions { Args = [], EnvironmentName = "Testing" });
builder.Logging.ClearProviders();
builder.WebHost.UseUrls("http://127.0.0.1:0");
builder.Services.AddAuthentication("fixture").AddScheme<AuthenticationSchemeOptions, FixtureAuthentication>("fixture", _ => { });
builder.Services.AddAuthorization();
builder.Services.AddControllers().AddApplicationPart(typeof(IntegrationManagementController).Assembly);
builder.Services.AddSingleton<IIntegrationScopeAuthorization>(scope);
var calls = new List<(string Method, object?[] Args)>();
object? Respond(MethodInfo method, object?[]? arguments)
{
    calls.Add((method.Name, arguments ?? []));
    return method.Name switch
    {
        "BoardAsync" => Task.FromResult<ProductPreparationBoard?>(new(0,0,0,0,0,[])),
        "PolicyAsync" => Task.FromResult<ProductPreparationPolicy?>(new()),
        "SetPolicyAsync" => Task.FromResult(true),
        "PrepareAsync" => Task.FromResult<ProductPreparationItem?>(new(1,1,new(ProductTransferDirection.ToPlatform,"44"),new(ProductTransferDirection.ToPlatform,"44"),"NeedsAttention",[],[],null,null,null,[])),
        "ListConnectionsAsync" => Task.FromResult<IReadOnlyList<IntegrationConnectionSummary>>([]),
        "CreateConnectionAsync" => Task.FromResult<IntegrationConnectionResponse?>(new(new(41, 7, "tenant-a", Provider.Custom, "fixture", false, null, null))),
        "SetConnectionEnabledAsync" or "DeactivateProductMappingAsync" or "RevokeAsync" => Task.FromResult(true),
        "ReplayWebhookAsync" => Task.FromResult<IntegrationReplayResponse?>(new(51, "Queued")),
        "ListProductMappingsAsync" => Task.FromResult<IReadOnlyList<IntegrationProductMappingSummary>>([]),
        "CreateProductMappingAsync" => Task.FromResult<IntegrationProductMappingSummary?>(new(61, 41, 7, 1, "fixture", null, null, null, null, null, true)),
        "TriggerAsync" => Task.FromResult<IntegrationSyncTriggerResponse?>(new(71, "Accepted")),
        "StartAsync" => Task.FromResult<IntegrationProductDraftStatus?>(new(Guid.NewGuid(),74,"Pending",null,null,null)),
        "ReadAsync" when method.DeclaringType == typeof(IIntegrationProductDraftApi) =>
            Task.FromResult<IntegrationProductDraftStatus?>(new(Guid.NewGuid(),74,"Pending",null,null,null)),
        "ReconcileCatalogAsync" => Task.FromResult<IntegrationCatalogReconciliationResponse?>(new(Guid.NewGuid(),
            new(72, "Pending", null, null), new(73, "Pending", null, null))),
        "RequestAsync" => Task.FromResult<IntegrationTokenRequestStatus?>(new(Guid.NewGuid(), 0, DateTime.UtcNow)),
        "ListAsync" => Task.FromResult<IReadOnlyList<IntegrationTokenStatus>>([]),
        "GetDashboardAsync" => Task.FromResult(new IntegrationDashboardResponse(0, 0, 0, [], [], [], [])),
        "ReceiveInventoryChangedAsync" => Task.FromResult<AccountingInventoryChangedResponse?>(new(81, "Queued")),
        "ReceiveAsync" when method.DeclaringType == typeof(IIntegrationAccountingProductEventIngress) =>
            Task.FromResult(new AccountingProductChangedResponse(82, "Queued")),
        "ReadAsync" or "ChangeAsync" => Task.FromResult<IntegrationVersionSourceResult?>(
            new("Ready", Hyper.Integration.Contracts.IntegrationVersionSource.AccountingEvents, 1, 0)),
        "ReceiveAsync" => Task.FromResult(new WebhookIngressResult(WebhookIngressStatus.Invalid)),
        _ => throw new InvalidOperationException("UnexpectedApiCall")
    };
}
builder.Services.AddSingleton(Probe.Create<IIntegrationManagementApi>(Respond));
builder.Services.AddSingleton(Probe.Create<IIntegrationMappingApi>(Respond));
builder.Services.AddSingleton(Probe.Create<IIntegrationSyncApi>(Respond));
builder.Services.AddSingleton(Probe.Create<IIntegrationProductDraftApi>(Respond));
builder.Services.AddSingleton(Probe.Create<IIntegrationProductReadinessApi>(Respond));
builder.Services.AddSingleton(Probe.Create<IIntegrationTokenApi>(Respond));
builder.Services.AddSingleton(Probe.Create<IIntegrationDashboardApi>(Respond));
builder.Services.AddSingleton(Probe.Create<IIntegrationAccountingEventIngress>(Respond));
builder.Services.AddSingleton(Probe.Create<IIntegrationAccountingProductEventIngress>(Respond));
builder.Services.AddSingleton(Probe.Create<IIntegrationVersionSourceApi>(Respond));
builder.Services.AddSingleton(Probe.Create<IIntegrationWebhookIngress>(Respond));
await using var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
await app.StartAsync();
try
{
    using var client = new HttpClient { BaseAddress = new Uri(app.Urls.Single()), Timeout = TimeSpan.FromSeconds(20) };
    const string root = "/api/integrations/v1";
    EndpointCase[] cases =
    [
        new("Board","GET",root+"/connections/41/product-readiness",null,"BoardAsync"),
        new("Policy","GET",root+"/connections/41/product-readiness/policy/1",null,"PolicyAsync"),
        new("SetPolicy","PUT",root+"/connections/41/product-readiness/policy/1",new ProductPreparationPolicy(),"SetPolicyAsync"),
        new("Prepare","POST",root+"/connections/41/product-readiness",new ProductPreparationCommand(new(ProductTransferDirection.ToPlatform,"44")),"PrepareAsync"),
        new("CreateDraft", "POST", root + "/connections/41/product-drafts",
            new IntegrationProductDraftRequest(7,"tenant-a",41,Guid.NewGuid(),44,100,2,200),"StartAsync"),
        new("ReadDraft", "GET", root + "/connections/41/product-drafts/ae475351-42f0-4a2a-967f-cf78be311c64",null,"ReadAsync"),
        new("List", "GET", root + "/connections?shopId=7", null, "ListConnectionsAsync"),
        new("Create", "POST", root + "/connections", new IntegrationConnectionCreateRequest(7, "tenant-a", Provider.Custom, "fixture", "fixture", 1), "CreateConnectionAsync"),
        new("Enable", "POST", root + "/connections/41/enable", null, "SetConnectionEnabledAsync"),
        new("Disable", "POST", root + "/connections/41/disable", null, "SetConnectionEnabledAsync"),
        new("Replay", "POST", root + "/connections/41/webhooks/51/replay", null, "ReplayWebhookAsync"),
        new("ListMappings", "GET", root + "/connections/41/mappings", null, "ListProductMappingsAsync"),
        new("CreateMapping", "POST", root + "/connections/41/mappings", new IntegrationProductMappingRequest(7, "tenant-a", 41, 1, "fixture"), "CreateProductMappingAsync"),
        new("DeactivateMapping", "DELETE", root + "/connections/41/mappings/61", null, "DeactivateProductMappingAsync"),
        new("Sync", "POST", root + "/connections/41/sync", null, "TriggerAsync"),
        new("ReconcileCatalog", "POST", root + "/connections/41/catalog/reconcile",
            new IntegrationCatalogReconciliationRequest(7, "tenant-a", 41, Guid.NewGuid()), "ReconcileCatalogAsync"),
        new("RequestToken", "POST", root + "/tokens/requests", new IntegrationTokenRequestCommand(Guid.NewGuid(), 7, "tenant-a", Provider.Basalam, 1), "RequestAsync"),
        new("List", "GET", root + "/tokens", null, "ListAsync"),
        new("Revoke", "DELETE", root + "/tokens/41", null, "RevokeAsync"),
        new("Get", "GET", root + "/dashboard?shopId=7", null, "GetDashboardAsync"),
        new("InventoryChanged", "POST", root + "/accounting/events/inventory-changed", new AccountingInventoryChangedRequest(7, "tenant-a", 41, "fixture", null, 1, 1), "ReceiveInventoryChangedAsync"),
        new("ProductChanged", "POST", root + "/accounting/events/product-changed", new AccountingProductChangedRequest(7, "tenant-a", 41, "fixture", null, 1, "Fixture product", 1), "ReceiveAsync"),
        new("Read", "GET", root + "/connections/41/mappings/61/version-source", null, "ReadAsync"),
        new("Change", "PUT", root + "/connections/41/mappings/61/version-source", new IntegrationVersionSourceChange(
            Hyper.Integration.Contracts.IntegrationVersionSource.AccountingEvents,
            Hyper.Integration.Contracts.IntegrationVersionSource.Unassigned, 0), "ChangeAsync")
    ];
    var endpoints = ((IEndpointRouteBuilder)app).DataSources.SelectMany(x => x.Endpoints)
        .Where(x => x.Metadata.GetMetadata<ControllerActionDescriptor>() is not null).ToArray();
    await checks.Test("endpoint inventory covers every protected action", () => Task.FromResult(
        endpoints.Length == cases.Length + 1 && endpoints.Count(x => x.Metadata.GetMetadata<IAllowAnonymous>() is not null) == 1
        && endpoints.Where(x => x.Metadata.GetMetadata<IAllowAnonymous>() is null).All(x =>
            x.Metadata.GetMetadata<IAuthorizeData>() is not null
            && cases.Any(c => c.Action == x.Metadata.GetMetadata<ControllerActionDescriptor>()!.ActionName))));
    foreach (var endpoint in cases)
    foreach (var identity in new[] { "anonymous", "other-shop", "other-tenant", "admin", "mixed", "owner", "owner-scope" })
    {
        await checks.Test($"HTTP {endpoint.Method} {endpoint.Path} / {identity}", async () =>
        {
            calls.Clear();
            using var request = new HttpRequestMessage(new HttpMethod(endpoint.Method), endpoint.Path);
            request.Headers.Add("X-Fixture-Identity", identity);
            request.Headers.Add("X-Shop-Id", "7");
            request.Headers.Add("X-Tenant-Id", "tenant-a");
            if (endpoint.Body is not null) request.Content = JsonContent.Create(endpoint.Body, endpoint.Body.GetType());
            using var response = await client.SendAsync(request);
            if (identity is not ("owner" or "owner-scope"))
                return response.StatusCode == (identity == "anonymous" ? HttpStatusCode.Unauthorized : HttpStatusCode.Forbidden) && calls.Count == 0;
            if (!response.IsSuccessStatusCode || calls.Count != 1 || calls[0].Method != endpoint.ServiceMethod) return false;
            var requestDto = calls[0].Args.FirstOrDefault(x => x?.GetType().GetProperty("ShopId") is not null);
            return requestDto is null
                ? calls[0].Args.Contains(7) && calls[0].Args.Contains("tenant-a")
                : Equals(requestDto.GetType().GetProperty("ShopId")!.GetValue(requestDto), 7)
                    && Equals(requestDto.GetType().GetProperty("TenantId")!.GetValue(requestDto), "tenant-a");
        });
    }
    await checks.Test("route/body connection mismatch rejected before service", async () =>
    {
        calls.Clear();
        using var request = new HttpRequestMessage(HttpMethod.Post, root + "/connections/41/mappings");
        request.Headers.Add("X-Fixture-Identity", "owner");
        request.Content = JsonContent.Create(new IntegrationProductMappingRequest(7, "tenant-a", 999, 1, "fixture"));
        using var response = await client.SendAsync(request);
        return response.StatusCode == HttpStatusCode.BadRequest && calls.Count == 0;
    });
    foreach (var invalid in new[]
    {
        new IntegrationCatalogReconciliationRequest(7, "tenant-a", 999, Guid.NewGuid()),
        new IntegrationCatalogReconciliationRequest(7, "tenant-a", 41, Guid.Empty)
    })
    await checks.Test("catalog start rejects invalid route/request identity before service", async () =>
    {
        calls.Clear();
        using var request = new HttpRequestMessage(HttpMethod.Post, root + "/connections/41/catalog/reconcile");
        request.Headers.Add("X-Fixture-Identity", "owner");
        request.Content = JsonContent.Create(invalid);
        using var response = await client.SendAsync(request);
        return response.StatusCode == HttpStatusCode.BadRequest && calls.Count == 0;
    });
    await checks.Test("anonymous webhook uses verifier-backed ingress rather than user scope", async () =>
    {
        calls.Clear();
        using var request = new HttpRequestMessage(HttpMethod.Post, root + "/webhooks/custom/fixture") { Content = JsonContent.Create(new { id = "fixture", name = "product.updated" }) };
        using var response = await client.SendAsync(request);
        return response.StatusCode == HttpStatusCode.BadRequest && calls.Count == 1 && calls[0].Method == "ReceiveAsync";
    });
}
finally { await app.StopAsync(); }

if (args.Contains("--sql")) await SqlChecks.Run(checks);
else Console.WriteLine("SQL checks skipped; use --sql with ISOLATION_TEST_SQL.");
Console.WriteLine($"{checks.Passed} passed; {checks.Failed} failed.");
return checks.Failed == 0 ? 0 : 1;

record EndpointCase(string Action, string Method, string Path, object? Body, string ServiceMethod);
public class Probe : DispatchProxy
{
    public Func<MethodInfo, object?[]?, object?> Handler { get; set; } = null!;
    protected override object? Invoke(MethodInfo? method, object?[]? args) => Handler(method!, args);
    public static T Create<T>(Func<MethodInfo, object?[]?, object?> handler) where T : class
    {
        var proxy = Create<T, Probe>();
        ((Probe)(object)proxy).Handler = handler;
        return proxy;
    }
}
sealed class Checks
{
    public int Passed { get; private set; }
    public int Failed { get; private set; }
    public async Task Test(string name, Func<Task<bool>> action)
    {
        try
        {
            if (!await action()) throw new InvalidOperationException("AssertionFailed");
            Passed++;
            Console.WriteLine("PASS " + name);
        }
        catch (Exception ex) { Failed++; Console.WriteLine($"FAIL {name} ({ex.GetType().Name})"); }
    }
}
sealed class FixtureAuthentication(IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger, UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var mode = Request.Headers["X-Fixture-Identity"].ToString();
        if (mode is "anonymous" or "") return Task.FromResult(AuthenticateResult.NoResult());
        var identity = new ClaimsIdentity([new Claim(ClaimTypes.Name, "admin-a")], "fixture");
        var principal = new ClaimsPrincipal(identity);
        if (mode == "admin") identity.AddClaim(new(ClaimTypes.Role, "Admin"));
        else if (mode == "mixed") principal.AddIdentity(new ClaimsIdentity([new Claim("integration_scope", "shop:7;tenant:tenant-a")]));
        else identity.AddClaim(new(mode == "owner-scope" ? "scope" : "integration_scope", mode switch
        {
            "other-shop" => "shop:8;tenant:tenant-a", "other-tenant" => "shop:7;tenant:tenant-b", _ => "shop:7;tenant:tenant-a"
        }));
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name)));
    }
}
