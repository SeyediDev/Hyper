using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Hyper.AdminPanel.Web.Controllers;
using Hyper.AdminPanel.Web.Infrastructure;
using Hyper.AdminPanel.Web.ViewModels;
using Hyper.Infrastructure.Features.Integrations;
using Hyper.Integration.Domain.Entities.Integrations;
using Hyper.Integration.Domain.Features.Integrations;
using Hyperyek.Accounting.Contracts;
using Hyperyek.Accounting.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Neo.Domain.Entities.Common;
using Neo.Domain.Features.Client;
using BpmsIdentity = Neo.Bpms.Domain.Models.Security.Authentication.IdentityUser;

// Test executable only. Never calls the production Program, loads appsettings,
// registers workers/SQL or authenticates a real user. Binds ephemeral loopback only.
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = [], EnvironmentName = "Testing", ApplicationName = typeof(MvcFixtureMarker).Assembly.GetName().Name,
    ContentRootPath = Path.GetFullPath("tools/FinancialPreviewMvcChecks")
});
builder.Configuration.Sources.Clear();
builder.Configuration.AddInMemoryCollection();
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options => options.SingleLine = true).SetMinimumLevel(LogLevel.Warning);
builder.WebHost.UseUrls("http://127.0.0.1:0");
builder.Services.AddSingleton<IDataProtectionProvider>(new EphemeralDataProtectionProvider());
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.Cookie.Name = "Hyper.FinancialPreview.Fixture";
    options.Events.OnRedirectToLogin = context => { context.Response.StatusCode = 401; return Task.CompletedTask; };
    options.Events.OnRedirectToAccessDenied = context => { context.Response.StatusCode = 403; return Task.CompletedTask; };
});
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IRequesterUser, FixtureRequester>();
builder.Services.AddSingleton<AdminSimulationTickets>();
builder.Services.AddSingleton<FixtureContext>();
builder.Services.AddSingleton<IAdminMerchantSimulationService>(sp => sp.GetRequiredService<FixtureContext>());
builder.Services.AddSingleton<IIntegrationScenarioQueue>(sp => sp.GetRequiredService<FixtureContext>());
builder.Services.AddSingleton<PreviewTransport>();
builder.Services.AddSingleton<IIntegrationFinancialPreviewPort>(sp => new AccountingFinancialPreviewClient(
    new HttpClient(sp.GetRequiredService<PreviewTransport>()) { BaseAddress = new Uri("https://accounting.fixture.invalid/") }));
builder.Services.AddScoped<AdminFinancialPreviewWorkflow>();
builder.Services.AddControllersWithViews().AddApplicationPart(typeof(AccountingFinancialPreviewController).Assembly)
    .ConfigureApplicationPartManager(parts => parts.FeatureProviders.Add(new PreviewOnlyControllers()));
await using var app = builder.Build();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.GetFullPath("src/AdminPanel/Hyper.AdminPanel.Web/wwwroot/Content")),
    RequestPath = "/Content"
});
app.UseAuthentication(); app.UseAuthorization();
app.MapGet("/fixture/start", async (HttpContext context, AdminSimulationTickets tickets, FixtureContext data) =>
{
    var member = context.Request.Query["member"] == "1";
    var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, FixtureContext.Admin), new(ClaimTypes.Name, "Fixture") };
    if (!member) claims.Add(new(ClaimTypes.Role, "Administrator"));
    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));
    context.Response.Cookies.Append(AdminFinancialPreviewWorkflow.CookieName,
        tickets.Protect(FixtureContext.Admin, data.Selection.Id), new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.Strict });
    return Results.Redirect("/AccountingFinancialPreview/Index");
});
app.MapControllerRoute("default", "{controller=AccountingFinancialPreview}/{action=Index}/{id?}");
await app.StartAsync();
var address = app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.Single();
var count = 0;
void Check(bool condition, string name) { if (!condition) throw new Exception(name); count++; Console.WriteLine("PASS " + name); }
HttpClient Browser() => new(new HttpClientHandler { AllowAutoRedirect = false, CookieContainer = new CookieContainer() }) { BaseAddress = new Uri(address) };
const string pageRoute = "/AccountingFinancialPreview/Index";
const string postRoute = "/AccountingFinancialPreview/Preview";
var data = app.Services.GetRequiredService<FixtureContext>();
var transport = app.Services.GetRequiredService<PreviewTransport>();
string Hidden(string html, string name)
{
    var match = Regex.Match(html, "<input[^>]*name=\"" + Regex.Escape(name) + "\"[^>]*value=\"([^\"]*)\"", RegexOptions.IgnoreCase);
    if (!match.Success) throw new Exception("Missing rendered hidden field " + name);
    return WebUtility.HtmlDecode(match.Groups[1].Value);
}
try
{
    using var browser = Browser();
    using (var anonymous = await browser.GetAsync(pageRoute)) Check(anonymous.StatusCode == HttpStatusCode.Unauthorized,
        "real MVC Authorize rejects anonymous GET (received " + (int)anonymous.StatusCode + ")");
    using (var member = await browser.GetAsync("/fixture/start?member=1")) Check(member.StatusCode == HttpStatusCode.Redirect, "fixture member cookie issued only on local test host");
    using (var forbidden = await browser.GetAsync(pageRoute)) Check(forbidden.StatusCode == HttpStatusCode.Forbidden, "real controller rejects authenticated non-admin");
    using (var admin = await browser.GetAsync("/fixture/start")) Check(admin.StatusCode == HttpStatusCode.Redirect, "fixture admin context ready");
    using var page = await browser.GetAsync(pageRoute);
    var html = await page.Content.ReadAsStringAsync();
    Check(page.StatusCode == HttpStatusCode.OK, "actual compiled controller and Razor render successfully");
    Check(page.Headers.CacheControl?.NoStore == true, "financial page carries no-store header");
    var decoded = WebUtility.HtmlDecode(html);
    Check(decoded.Contains("میزبان تست محلی") && decoded.Contains("داده‌ها دستی‌اند") && decoded.Contains("fp-json"),
        "production view is rendered inside clearly marked fixture-only shell");
    var token = Hidden(html, "__RequestVerificationToken");
    var contextTicket = Hidden(html, "ContextTicket");
    Check(token.Length > 0 && contextTicket.Length > 0, "real form emits antiforgery and simulation context tokens");
    Dictionary<string, string> Fields(string draft = FinancialPreviewDraft.Sample) => new()
    {
        ["__RequestVerificationToken"] = token, ["ContextTicket"] = contextTicket,
        ["ConnectionId"] = "10", ["DraftJson"] = draft, ["UnitAcknowledged"] = "true"
    };
    var missingCsrf = Fields(); missingCsrf.Remove("__RequestVerificationToken");
    using (var rejected = await browser.PostAsync(postRoute, new FormUrlEncodedContent(missingCsrf)))
        Check(rejected.StatusCode == HttpStatusCode.BadRequest && transport.Calls == 0, "real MVC antiforgery rejects missing token before financial call");
    var invalidCsrf = Fields(); invalidCsrf["__RequestVerificationToken"] = "tampered";
    using (var rejected = await browser.PostAsync(postRoute, new FormUrlEncodedContent(invalidCsrf)))
        Check(rejected.StatusCode == HttpStatusCode.BadRequest && transport.Calls == 0, "tampered antiforgery token rejected");
    using (var result = await browser.PostAsync(postRoute, new FormUrlEncodedContent(Fields())))
    {
        var body = WebUtility.HtmlDecode(await result.Content.ReadAsStringAsync());
        Check(result.StatusCode == HttpStatusCode.OK && transport.Calls == 1 && body.Contains("9,600,000")
            && body.Contains("9,100,000") && body.Contains("8,700,000"), "form -> workflow -> production HTTP mapper -> calculator -> real Razor totals");
        Check(body.Contains("محاسبه معتبر — فقط آزمایشی"), "validated financial result remains labelled simulation");
    }
    using (var result = await browser.PostAsync(postRoute, new FormUrlEncodedContent(Fields(FinancialPreviewDraft.Sample.Replace("\"platformCommission\": 900000", "\"platformCommission\": null")))))
    {
        var body = WebUtility.HtmlDecode(await result.Content.ReadAsStringAsync());
        Check(result.StatusCode == HttpStatusCode.OK && body.Contains("در انتظار اطلاعات تکمیلی") && body.Contains("نامشخص")
            && !body.Contains("8,700,000"), "unknown commission renders evidence status and unknown settlement");
    }
    using (var result = await browser.PostAsync(postRoute, new FormUrlEncodedContent(Fields(FinancialPreviewDraft.Sample.Replace("9100000", "1")))))
    {
        var body = WebUtility.HtmlDecode(await result.Content.ReadAsStringAsync());
        Check(result.StatusCode == HttpStatusCode.OK && body.Contains("نیازمند بررسی") && !body.Contains("8,700,000"), "payment mismatch renders review without settlement");
    }
    var calls = transport.Calls;
    var foreign = Fields(); foreign["ConnectionId"] = "99";
    using (var result = await browser.PostAsync(postRoute, new FormUrlEncodedContent(foreign)))
        Check(result.StatusCode == HttpStatusCode.Conflict && transport.Calls == calls, "posted foreign connection rejected by real form path");
    data.Selection.EndedAtUtc = DateTime.UtcNow;
    using (var result = await browser.PostAsync(postRoute, new FormUrlEncodedContent(Fields())))
        Check(result.StatusCode == HttpStatusCode.Conflict && transport.Calls == calls, "expired simulation rejects valid-antiforgery stale form");
    data.Selection.EndedAtUtc = null;
    Check(data.Mutations == 0, "no simulation mutation, queue write or token request throughout MVC checks");
    Console.WriteLine($"{count} MVC/Razor checks passed. Real production controller/view/workflow/HTTP mapper; synthetic identity and transport, no SQL/Basalam.");
    if (args.Contains("--serve"))
    {
        Console.WriteLine("BROWSER_FIXTURE_URL=" + address + "/fixture/start");
        using var lifetime = new CancellationTokenSource(TimeSpan.FromMinutes(10));
        await app.WaitForShutdownAsync(lifetime.Token);
    }
}
finally { await app.StopAsync(); }

public sealed class MvcFixtureMarker;
// Keep the real target controller and compiled views, without activating unrelated
// panel/API routes whose versioning and dependencies are outside this fixture.
sealed class PreviewOnlyControllers : IApplicationFeatureProvider<ControllerFeature>
{
    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        foreach (var controller in feature.Controllers.Where(type => type.AsType() != typeof(AccountingFinancialPreviewController)).ToArray())
            feature.Controllers.Remove(controller);
    }
}
sealed class FixtureRequester : IRequesterUser
{
    public FixtureRequester(IHttpContextAccessor accessor)
    {
        Properties = new() { [typeof(BpmsIdentity).Name] = new BpmsIdentity { Id = FixtureContext.Admin, UserName = "Fixture",
            Culture = "fa", IsAdmin = accessor.HttpContext!.User.IsInRole("Administrator") } };
    }
    public UserId? Id { get; set; }
    public string Platform => "fixture";
    public string? AppName => "fixture";
    public string? Lang => "fa";
    public string? Mobile => null;
    public string? CorrelationId => null;
    public Dictionary<string, object> Properties { get; set; }
    public Task<LanguageId> GetLangIdAsync(CancellationToken cancellationToken = default) => throw new NotSupportedException();
    public List<Claim> Claims() => [];
    public bool? IsInRole(string role) => false;
}
sealed class FixtureContext : IAdminMerchantSimulationService, IIntegrationScenarioQueue
{
    public const string Admin = "financial-fixture-admin";
    public int Mutations;
    public IntegrationAdminSimulation Selection { get; } = new() { AdminUserId = Admin, ShopId = 7, TenantId = "fixture-tenant",
        ShopName = "مغازهٔ آزمایشی", MerchantIdentifier = "fixture", ExpiresAtUtc = DateTime.UtcNow.AddHours(1) };
    public Task<IntegrationAdminSimulation?> GetAsync(string adminId, Guid simulationId, CancellationToken ct) => Task.FromResult<IntegrationAdminSimulation?>(
        adminId == Admin && simulationId == Selection.Id ? Selection : null);
    public Task<IReadOnlyList<IntegrationScenarioConnection>> ConnectionsAsync(OwnedIntegrationShop shop, CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<IntegrationScenarioConnection>>(shop == new OwnedIntegrationShop(7, "fixture-tenant")
            ? [new(10, "اتصال ساختگی باسلام", IntegrationProvider.Basalam, true)] : []);
    public Task<IReadOnlyList<SimulationShop>> SearchShopsAsync(string? search, CancellationToken ct) => throw new NotSupportedException();
    public Task<IntegrationAdminSimulation?> SelectAsync(string adminId, int shopId, CancellationToken ct) { Mutations++; throw new NotSupportedException(); }
    public Task EndAsync(string adminId, Guid simulationId, CancellationToken ct) { Mutations++; throw new NotSupportedException(); }
    public Task<IntegrationTokenRequest?> RequestTokenAsync(string adminId, Guid simulationId, int displayedShopId, IntegrationProvider provider, IntegrationCredentialType credentialType, CancellationToken ct) { Mutations++; throw new NotSupportedException(); }
    public Task<long> EnqueueAsync(OwnedIntegrationShop shop, long connectionId, IntegrationScenarioRequest request, CancellationToken ct) { Mutations++; throw new NotSupportedException(); }
    public Task<bool> ProcessNextAsync(CancellationToken ct) { Mutations++; throw new NotSupportedException(); }
    public Task<IReadOnlyList<IntegrationScenarioJob>> RecentAsync(OwnedIntegrationShop shop, CancellationToken ct) => throw new NotSupportedException();
}
sealed class PreviewTransport : HttpMessageHandler
{
    public int Calls;
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        if (request.Method != HttpMethod.Post || request.RequestUri!.AbsoluteUri != "https://accounting.fixture.invalid/api/hyperyek/v2/accounting/financial-preview")
            throw new InvalidOperationException("Fixture denies any non-preview destination");
        Calls++;
        var input = await request.Content!.ReadFromJsonAsync<FinancialPreviewRequest>(cancellationToken: ct);
        return new(HttpStatusCode.OK) { Content = JsonContent.Create(new FinancialPreviewCalculator().Preview(input!)) };
    }
}
