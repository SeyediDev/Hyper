using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using Hyper.Infrastructure.Features.Integrations;
using Hyper.Integration.Domain.Features.Integrations;
using Hyperyek.Accounting.Host;
using Hyperyek.Accounting.Api;
using Hyperyek.Accounting.Contracts;
using Hyperyek.Accounting.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

var checks = 0;
void Check(bool ok, string name) { if (!ok) throw new Exception(name); Console.WriteLine("PASS " + name); checks++; }
async Task Reject(Func<Task> action, string code)
{
    try { await action(); }
    catch (Exception ex) when (ex.Message == code) { Check(true, code); return; }
    throw new Exception("Expected " + code);
}
var values = new Dictionary<string, string?>
{
    ["IdpSetting:TokenProvider"] = "Keycloak", ["IdpSetting:Authority"] = "https://issuer.fixture.invalid/realm",
    ["IdpSetting:ClientId"] = "accounting-api", ["AccountingApiSecurity:AllowedClientIds:0"] = "integration-service"
};
IConfiguration Config(Dictionary<string, string?> data) => new ConfigurationBuilder().AddInMemoryCollection(data).Build();
IHost FixtureHost(IServiceCollection descriptors) => new HostBuilder().ConfigureServices((_, services) =>
{
    foreach (var descriptor in descriptors) services.Add(descriptor);
}).Build();
foreach (var (key, value) in new[] { ("IdpSetting:TokenProvider", "Memory"), ("IdpSetting:Authority", "http://issuer.invalid"),
    ("AccountingApiSecurity:AllowedClientIds:0", ""), ("IdpSetting:ClientId", "") })
{
    var invalid = new Dictionary<string, string?>(values) { [key] = value };
    await Reject(() => { new ServiceCollection().AddAccountingServiceSecurity(Config(invalid)); return Task.CompletedTask; },
        "AccountingAuthenticationConfigurationInvalid");
}
using var rsa = RSA.Create(2048);
var keyMaterial = new RsaSecurityKey(rsa) { KeyId = "fixture-key" };
string Issue(string scope = "hyperyek.accounting", string client = "integration-service", string audience = "accounting-api",
    string issuer = "https://issuer.fixture.invalid/realm", bool expired = false, string? conflictingClient = null)
{
    var claims = new List<Claim> { new("scope", scope), new("azp", client) };
    if (conflictingClient is not null) claims.Add(new("client_id", conflictingClient));
    return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(issuer, audience, claims,
        DateTime.UtcNow.AddMinutes(-10), DateTime.UtcNow.AddMinutes(expired ? -5 : 5),
        new SigningCredentials(keyMaterial, SecurityAlgorithms.RsaSha256)));
}
var builder = WebApplication.CreateBuilder(new WebApplicationOptions { Args = [], EnvironmentName = "Testing" });
builder.Logging.ClearProviders();
builder.WebHost.UseUrls("http://127.0.0.1:0");
builder.Services.AddAccountingServiceSecurity(Config(values));
builder.Services.AddHyperyekAccountingApi();
builder.Services.AddControllers().AddApplicationPart(typeof(AccountingFinancialPreviewController).Assembly);
builder.Services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
{
    var metadata = new OpenIdConnectConfiguration { Issuer = values["IdpSetting:Authority"] };
    metadata.SigningKeys.Add(keyMaterial);
    options.ConfigurationManager = new StaticConfigurationManager<OpenIdConnectConfiguration>(metadata);
});
await using var app = builder.Build();
app.UseAuthentication(); app.UseAuthorization();
app.MapGet("/probe", () => Results.Ok()).RequireAuthorization();
app.MapGet("/fallback", () => Results.Ok());
app.MapControllers();
await app.StartAsync();
try
{
    var url = app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.Single();
    using var http = new HttpClient { BaseAddress = new Uri(url) };
    async Task<HttpStatusCode> Status(string? token)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/probe");
        if (token is not null) request.Headers.Authorization = new("Bearer", token);
        using var response = await http.SendAsync(request); return response.StatusCode;
    }
    Check(await Status(null) == HttpStatusCode.Unauthorized, "anonymous request receives 401 rather than missing-scheme 500");
    Check(await Status("invalid") == HttpStatusCode.Unauthorized, "malformed JWT rejected");
    var signed = Issue().Split('.');
    signed[2] = (signed[2][0] == 'A' ? "B" : "A") + signed[2][1..];
    Check(await Status(string.Join('.', signed)) == HttpStatusCode.Unauthorized, "tampered JWT signature rejected");
    using (var fallback = await http.GetAsync("/fallback"))
        Check(fallback.StatusCode == HttpStatusCode.Unauthorized, "fallback policy protects newly added routes");
    Check(await Status(Issue(expired: true)) == HttpStatusCode.Unauthorized, "expired JWT rejected");
    Check(await Status(Issue(audience: "other-api")) == HttpStatusCode.Unauthorized, "wrong audience rejected");
    Check(await Status(Issue(issuer: "https://other.invalid")) == HttpStatusCode.Unauthorized, "wrong issuer rejected");
    Check(await Status(Issue(scope: "other")) == HttpStatusCode.Forbidden, "valid identity without accounting scope receives 403");
    Check(await Status(Issue(scope: "hyperyek.accounting.extra")) == HttpStatusCode.Forbidden, "scope matching is exact");
    Check(await Status(Issue(client: "other-client")) == HttpStatusCode.Forbidden, "untrusted service client receives 403");
    Check(await Status(Issue(conflictingClient: "other-client")) == HttpStatusCode.Forbidden, "conflicting client identities rejected");
    Check(await Status(Issue(scope: "profile hyperyek.accounting")) == HttpStatusCode.OK, "trusted service with accounting scope authorized");
    var configured = app.Services.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>().Get(JwtBearerDefaults.AuthenticationScheme);
    Check(configured.RequireHttpsMetadata && !configured.IncludeErrorDetails && !configured.MapInboundClaims,
        "Neo JWT integration uses HTTPS metadata and minimal error details");
    var preview = new FinancialPreviewRequest(new(7, "tenant-a"), 10, "order", "billing-group", 1,
        FinancialPreviewCalculator.PolicyVersion, FinancialSourceUnit.IRR, true,
        [new("line", 1, 10_000_000, 1_000_000, true)], 0, 600_000, 0, 0, 500_000, 9_100_000, 900_000, 0, 0, 0);
    const string previewRoute = "/api/hyperyek/v2/accounting/financial-preview";
    using (var anonymous = await http.PostAsJsonAsync(previewRoute, preview))
        Check(anonymous.StatusCode == HttpStatusCode.Unauthorized, "financial preview rejects anonymous requests");
    http.DefaultRequestHeaders.Authorization = new("Bearer", Issue(scope: "other"));
    using (var forbidden = await http.PostAsJsonAsync(previewRoute, preview))
        Check(forbidden.StatusCode == HttpStatusCode.Forbidden, "financial preview requires accounting scope");
    http.DefaultRequestHeaders.Authorization = new("Bearer", Issue());
    using (var response = await http.PostAsJsonAsync(previewRoute, preview))
    {
        var body = await response.Content.ReadFromJsonAsync<FinancialPreviewResult>();
        Check(response.StatusCode == HttpStatusCode.OK && body is { PreviewOnly: true, ExpectedSettlement: 8_700_000 }
            && body.Status == FinancialPreviewStatus.Validated, "authenticated preview computes without any registered SQL or persistence handler");
    }
    using (var response = await http.PostAsJsonAsync(previewRoute, preview with { PlatformCommission = null }))
    {
        var body = await response.Content.ReadFromJsonAsync<FinancialPreviewResult>();
        Check(response.StatusCode == HttpStatusCode.OK && body is { InvoiceTotal: 9_600_000, ExpectedSettlement: null }
            && body.Status == FinancialPreviewStatus.AwaitingEvidence, "HTTP preview preserves unknown fee rather than defaulting zero");
    }
    using (var response = await http.PostAsJsonAsync(previewRoute, preview with { BuyerPayment = 0 }))
    {
        var body = await response.Content.ReadFromJsonAsync<FinancialPreviewResult>();
        Check(response.StatusCode == HttpStatusCode.OK && body?.Status == FinancialPreviewStatus.NeedsReview,
            "HTTP 200 calculation result can contain review issues and is not posting acceptance");
    }
    using (var response = await http.PostAsync(previewRoute, new StringContent("{bad", System.Text.Encoding.UTF8, "application/json")))
        Check(response.StatusCode == HttpStatusCode.BadRequest, "malformed preview body is rejected by API binding");
    await FinancialPreviewBridgeChecks.RunAsync(http, Check);
}
finally { await app.StopAsync(); }

var clientValues = new Dictionary<string, string?>
{
    ["HyperyekAccountingApi:BaseAddress"] = "https://accounting.fixture.invalid/",
    ["HyperyekAccountingApi:TokenEndpoint"] = "https://issuer.fixture.invalid/token",
    ["HyperyekAccountingApi:ClientId"] = "integration-service",
    ["HyperyekAccountingApi:ClientSecret"] = "fixture-secret",
    ["HyperyekAccountingApi:Scope"] = "hyperyek.accounting"
};
var tokenTransport = new TokenTransport { Token = Issue() };
var apiTransport = new ApiTransport();
var services = new ServiceCollection();
services.AddLogging(); services.AddHyperyekAccountingClients(Config(clientValues));
services.AddHttpClient(AccountingClientRegistration.TokenClient).ConfigurePrimaryHttpMessageHandler(() => tokenTransport);
services.AddHttpClient(AccountingClientRegistration.ApiClient).ConfigurePrimaryHttpMessageHandler(() => apiTransport);
using var clientHost = FixtureHost(services);
var provider = clientHost.Services;
using var serviceScope = provider.CreateScope();
var catalog = serviceScope.ServiceProvider.GetRequiredService<IIntegrationPlatformCatalogPort>();
Check(serviceScope.ServiceProvider.GetRequiredService<IIntegrationFinancialPreviewPort>() is AccountingFinancialPreviewClient,
    "financial preview port resolves with the dedicated accounting service client");
await Task.WhenAll(Enumerable.Range(0, 8).Select(_ => catalog.GetProductsAsync(7, "tenant-a", default)));
Check(tokenTransport.Calls == 1 && apiTransport.Calls == 8 && apiTransport.LastToken == tokenTransport.Token,
    "worker-safe concurrent requests share one service token without HttpContext");
Check(tokenTransport.CorrectGrant, "configured client credentials and dedicated scope sent to token endpoint");
Check(ReferenceEquals(catalog, serviceScope.ServiceProvider.GetRequiredService<IIntegrationBusinessCommandPort>())
    && ReferenceEquals(catalog, serviceScope.ServiceProvider.GetRequiredService<IIntegrationAccountingPort>())
    && ReferenceEquals(catalog, serviceScope.ServiceProvider.GetRequiredService<IIntegrationPlatformShopPort>())
    && ReferenceEquals(catalog, serviceScope.ServiceProvider.GetRequiredService<IIntegrationPlatformOverviewPort>()),
    "all five accounting ports use the same authenticated client registration");
apiTransport.Status = HttpStatusCode.Unauthorized;
var callsBefore401 = apiTransport.Calls;
await Reject(() => catalog.GetProductsAsync(7, "tenant-a", default), "AccountingAuthenticationFailed");
Check(apiTransport.Calls == callsBefore401 + 1, "401 does not blindly replay an accounting request");
apiTransport.Status = HttpStatusCode.OK;
await catalog.GetProductsAsync(7, "tenant-a", default);
Check(tokenTransport.Calls == 2, "next durable attempt refreshes rejected token");
var financialPort = serviceScope.ServiceProvider.GetRequiredService<IIntegrationFinancialPreviewPort>();
var previewViaService = await financialPort.PreviewAsync(FinancialPreviewBridgeChecks.Sample);
Check(previewViaService.ExpectedSettlement == 8_700_000 && apiTransport.LastToken == tokenTransport.Token
    && tokenTransport.Calls == 2, "registered financial preview reuses service authentication, not a merchant or admin token");
apiTransport.Status = HttpStatusCode.Unauthorized;
var previewCallsBefore401 = apiTransport.Calls;
await Reject(() => financialPort.PreviewAsync(FinancialPreviewBridgeChecks.Sample), "AccountingAuthenticationFailed");
Check(apiTransport.Calls == previewCallsBefore401 + 1, "financial preview 401 invalidates token without POST replay");
apiTransport.Status = HttpStatusCode.OK;
await financialPort.PreviewAsync(FinancialPreviewBridgeChecks.Sample);
Check(tokenTransport.Calls == 3, "next explicit financial preview attempt obtains a fresh service token");
using (var named = provider.GetRequiredService<IHttpClientFactory>().CreateClient(AccountingClientRegistration.ApiClient))
    await Reject(() => named.GetAsync("https://untrusted.fixture.invalid/"), "AccountingRequestDestinationInvalid");
var tokens = provider.GetRequiredService<AccountingServiceTokenProvider>();
await tokens.InvalidateAsync(tokenTransport.Token);
tokenTransport.Status = HttpStatusCode.BadRequest;
await Reject(() => tokens.GetAsync(default), "AccountingTokenRequestFailed");
tokenTransport.Status = HttpStatusCode.OK; tokenTransport.Malformed = true;
await Reject(() => tokens.GetAsync(default), "AccountingTokenResponseInvalid");
tokenTransport.Malformed = false; tokenTransport.Lifetime = 0;
await Reject(() => tokens.GetAsync(default), "AccountingTokenResponseInvalid");
tokenTransport.Lifetime = 15;
await tokens.GetAsync(default); var callsBeforeExpiry = tokenTransport.Calls; await tokens.GetAsync(default);
Check(tokenTransport.Calls == callsBeforeExpiry + 1, "near-expiry token is not reused from cache");
var unsafeValues = new Dictionary<string, string?>(clientValues) { ["HyperyekAccountingApi:TokenEndpoint"] = "http://insecure.invalid/token" };
var unsafeServices = new ServiceCollection(); unsafeServices.AddHyperyekAccountingClients(Config(unsafeValues));
using var unsafeHost = FixtureHost(unsafeServices);
var unsafeProvider = unsafeHost.Services;
await Reject(() => unsafeProvider.GetRequiredService<IHttpClientFactory>().CreateClient(AccountingClientRegistration.ApiClient).GetAsync("probe"),
    "AccountingServiceConfigurationInvalid");
var deferredServices = new ServiceCollection(); deferredServices.AddHyperyekAccountingClients(null);
using var deferredHost = FixtureHost(deferredServices);
var deferred = deferredHost.Services;
using var deferredScope = deferred.CreateScope();
Check(deferredScope.ServiceProvider.GetRequiredService<IIntegrationPlatformCatalogPort>() is not null,
    "missing service setup does not break unrelated panel dependency activation");
var transportServices = new ServiceCollection(); transportServices.AddHyperyekAccountingClients(Config(clientValues));
using var transportHost = FixtureHost(transportServices);
var transportProvider = transportHost.Services;
foreach (var name in new[] { AccountingClientRegistration.ApiClient, AccountingClientRegistration.TokenClient })
{
    var handler = transportProvider.GetRequiredService<IHttpMessageHandlerFactory>().CreateHandler(name);
    while (handler is DelegatingHandler delegating) handler = delegating.InnerHandler!;
    Check(handler is HttpClientHandler { AllowAutoRedirect: false, ServerCertificateCustomValidationCallback: null },
        "service transport disables redirects and preserves certificate validation: " + name);
}
Console.WriteLine($"{checks} accounting authentication checks passed. Local JWT/HTTP fixtures only; no live identity provider or accounting mutation.");

sealed class TokenTransport : HttpMessageHandler
{
    public int Calls; public bool CorrectGrant; public string Token = ""; public int Lifetime = 300;
    public bool Malformed; public HttpStatusCode Status = HttpStatusCode.OK;
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        Interlocked.Increment(ref Calls);
        var form = await request.Content!.ReadAsStringAsync(ct);
        CorrectGrant = request.RequestUri!.AbsoluteUri == "https://issuer.fixture.invalid/token"
            && form.Contains("grant_type=client_credentials") && form.Contains("client_id=integration-service")
            && form.Contains("client_secret=fixture-secret") && form.Contains("scope=hyperyek.accounting");
        return new(Status) { Content = Malformed ? new StringContent("sensitive-invalid-json")
            : Status != HttpStatusCode.OK ? new StringContent("sensitive-provider-error")
            : JsonContent.Create(new { access_token = Token, token_type = "Bearer", expires_in = Lifetime }) };
    }
}
sealed class ApiTransport : HttpMessageHandler
{
    public int Calls; public string? LastToken; public HttpStatusCode Status = HttpStatusCode.OK;
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        Interlocked.Increment(ref Calls); LastToken = request.Headers.Authorization?.Parameter;
        if (Status == HttpStatusCode.OK && request.RequestUri!.AbsolutePath.EndsWith("/financial-preview", StringComparison.Ordinal))
        {
            var preview = await request.Content!.ReadFromJsonAsync<FinancialPreviewRequest>(cancellationToken: ct);
            return new(Status) { Content = JsonContent.Create(new FinancialPreviewCalculator().Preview(preview!)) };
        }
        return new(Status) { Content = new StringContent("[]", System.Text.Encoding.UTF8, "application/json") };
    }
}
