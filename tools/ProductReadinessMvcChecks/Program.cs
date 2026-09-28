using System.Net;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Hyper.AdminPanel.Web.Controllers;
using Hyper.AdminPanel.Web.Infrastructure;
using Hyper.Infrastructure.Features.Integrations;
using Hyper.Integration.Contracts;
using Hyper.Integration.Domain.Entities.Integrations;
using Hyper.Integration.Domain.Features.Integrations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Neo.Domain.Entities.Common;
using Neo.Domain.Features.Client;
using BpmsIdentity = Neo.Bpms.Domain.Models.Security.Authentication.IdentityUser;
using Provider = Hyper.Integration.Domain.Entities.Integrations.IntegrationProvider;

// Separate test-only entry point. Never load production Program/config/SQL/workers.
var builder=WebApplication.CreateBuilder(new WebApplicationOptions {Args=[],EnvironmentName="Testing",
    ApplicationName=typeof(FixtureMarker).Assembly.GetName().Name,ContentRootPath=Path.GetFullPath("tools/ProductReadinessMvcChecks")});
builder.Configuration.Sources.Clear(); builder.Configuration.AddInMemoryCollection();
builder.Logging.ClearProviders(); builder.Logging.AddSimpleConsole().SetMinimumLevel(LogLevel.Warning);
builder.WebHost.UseUrls("http://127.0.0.1:0");
builder.Services.AddSingleton<IDataProtectionProvider>(new EphemeralDataProtectionProvider());
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(o=>
{
    o.Cookie.Name="Hyper.Readiness.Fixture";
    o.Events.OnRedirectToLogin=c=>{c.Response.StatusCode=401;return Task.CompletedTask;};
    o.Events.OnRedirectToAccessDenied=c=>{c.Response.StatusCode=403;return Task.CompletedTask;};
});
builder.Services.AddAuthorization(); builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IRequesterUser,FixtureRequester>();
builder.Services.AddSingleton<AdminSimulationTickets>(); builder.Services.AddSingleton<FixtureData>();
builder.Services.AddControllersWithViews().AddApplicationPart(typeof(MerchantSimulationController).Assembly)
    .ConfigureApplicationPartManager(p=>p.FeatureProviders.Add(new OnlyTargetController())).AddControllersAsServices();
builder.Services.AddTransient<MerchantSimulationController>(sp=>new(sp.GetRequiredService<FixtureData>(),
    sp.GetRequiredService<AdminSimulationTickets>(),null!,null!,null!,sp.GetRequiredService<FixtureData>(),sp.GetRequiredService<FixtureData>()));
await using var app=builder.Build(); app.UseAuthentication(); app.UseAuthorization();
app.MapGet("/fixture/start",async(HttpContext context,AdminSimulationTickets tickets,FixtureData data)=>
{
    var claims=new List<Claim>{new(ClaimTypes.NameIdentifier,FixtureData.Admin),new(ClaimTypes.Name,"Readiness fixture")};
    if(context.Request.Query["member"]!="1")claims.Add(new(ClaimTypes.Role,"Administrator"));
    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,new ClaimsPrincipal(new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme)));
    context.Response.Cookies.Append("Hyper.AdminMerchantSimulation",tickets.Protect(FixtureData.Admin,data.Selection.Id),new CookieOptions{HttpOnly=true,SameSite=SameSiteMode.Strict});
    return Results.Redirect("/MerchantSimulation/Products");
});
app.MapControllerRoute("default","{controller=MerchantSimulation}/{action=Products}/{id?}");
await app.StartAsync(); var address=app.Urls.Single(); var data=app.Services.GetRequiredService<FixtureData>(); var checks=0;
void Check(bool value,string name){if(!value)throw new Exception(name);Console.WriteLine("PASS "+name);checks++;}
string Hidden(string html,string name)
{
    var m=Regex.Match(html,"<input[^>]*name=\""+Regex.Escape(name)+"\"[^>]*value=\"([^\"]*)\"",RegexOptions.IgnoreCase);
    if(!m.Success)throw new Exception("Missing hidden "+name);return WebUtility.HtmlDecode(m.Groups[1].Value);
}
try
{
    using var browser=new HttpClient(new HttpClientHandler{AllowAutoRedirect=false,CookieContainer=new CookieContainer()}){BaseAddress=new Uri(address)};
    const string page="/MerchantSimulation/Products",prepare="/MerchantSimulation/PrepareProduct",policy="/MerchantSimulation/ProductPolicy";
    // Neo's real base-controller exception filter redirects unauthenticated users.
    // Verify the destination below, rather than confusing this with IsAdmin403.
    using(var response=await browser.GetAsync(page))Check(response.StatusCode==HttpStatusCode.Redirect
        && response.Headers.Location?.OriginalString.Contains("Login",StringComparison.OrdinalIgnoreCase)==true,
        "anonymous redirected to login by real Neo controller ("+response.Headers.Location+")");
    await browser.GetAsync("/fixture/start?member=1");
    using(var response=await browser.GetAsync(page))Check(response.StatusCode==HttpStatusCode.Forbidden,"authenticated non-admin denied");
    await browser.GetAsync("/fixture/start");
    using var result=await browser.GetAsync(page);var html=await result.Content.ReadAsStringAsync();
    Check(result.StatusCode==HttpStatusCode.OK,"compiled production controller and Razor render");
    Check(result.Headers.CacheControl?.NoStore==true,"sensitive worklist marked no-store");
    Check(WebUtility.HtmlDecode(html).Contains("میزبان تست محلی"),"fixture visibly distinguished from live panel");
    Check(!html.Contains("<script>fixtureAttack</script>") && WebUtility.HtmlDecode(html).Contains("<script>fixtureAttack</script>"),"untrusted descriptions and history HTML encoded");
    var token=Hidden(html,"__RequestVerificationToken"); var ticket=Hidden(html,"contextTicket");
    Dictionary<string,string> Fields()=>new(){["__RequestVerificationToken"]=token,["contextTicket"]=ticket,["connectionId"]="10",
        ["expectedRevision"]="1",["input.Direction"]="1",["input.SourceProductId"]="44",["input.CategoryId"]="100",
        ["input.PreparationDays"]="0",["input.PackageWeight"]="200",["input.Description"]="Corrected by fixture",["input.PhotoId"]=""};
    var fields=Fields();fields.Remove("__RequestVerificationToken");
    using(var r=await browser.PostAsync(prepare,new FormUrlEncodedContent(fields)))Check(r.StatusCode==HttpStatusCode.BadRequest&&data.Prepares==0,"missing antiforgery blocks service");
    fields=Fields();fields["contextTicket"]="tampered";
    using(var r=await browser.PostAsync(prepare,new FormUrlEncodedContent(fields)))Check(r.StatusCode==HttpStatusCode.Conflict&&data.Prepares==0,"tampered simulation ticket blocks service");
    fields=Fields();fields["input.PreparationDays"]="not-an-int";
    using(var r=await browser.PostAsync(prepare,new FormUrlEncodedContent(fields)))Check(r.StatusCode==HttpStatusCode.BadRequest&&data.Prepares==0,"invalid numeric model binding blocks service");
    fields=Fields();fields["shopId"]="999";fields["tenantId"]="foreign";
    using(var r=await browser.PostAsync(prepare,new FormUrlEncodedContent(fields)))Check(r.StatusCode==HttpStatusCode.Redirect&&data.Prepares==1
        &&data.LastScope==new IntegrationConnectionCommandRequest(7,"fixture-tenant",10)&&data.LastInput!.PreparationDays==0&&data.LastInput.PhotoId is null,
        "real repair form binds optional fields and zero days; scope comes only from selected context");
    Check(data.LastActor==FixtureData.Admin&&data.LastRevision==1,"authenticated actor and expected revision propagated");
    using(var r=await browser.GetAsync(page))
    {var body=WebUtility.HtmlDecode(await r.Content.ReadAsStringAsync());Check(r.StatusCode==HttpStatusCode.OK&&body.Contains("در صف")&&body.Contains("درخواست ارسال‌شده قابل بازنویسی نیست"),"post-redirect-get renders queued immutable result, not completed");}
    var policyFields=new List<KeyValuePair<string,string>>{new("__RequestVerificationToken",token),new("contextTicket",ticket),new("connectionId","10"),
        new("direction","1"),new("policy.Mode","2"),new("policy.TrimDescription","true"),new("policy.TrimDescription","false"),new("policy.TruncateDescription","false")};
    using(var r=await browser.PostAsync(policy,new FormUrlEncodedContent(policyFields)))Check(r.StatusCode==HttpStatusCode.Redirect&&data.Policy.Mode==ProductPreparationMode.Adapt
        &&data.Policy.TrimDescription&&!data.Policy.TruncateDescription,"real checkbox plus hidden false binds selected policy correctly");
    using(var r=await browser.GetAsync(page+"?connectionId=999"))Check(r.StatusCode==HttpStatusCode.NotFound,"foreign connection GET denied");
    fields=Fields();fields["connectionId"]="999";
    using(var r=await browser.PostAsync(prepare,new FormUrlEncodedContent(fields)))Check(r.StatusCode==HttpStatusCode.NotFound&&data.Prepares==1,"foreign connection POST cannot mutate fixture");
    data.Selection.ExpiresAtUtc=DateTime.UtcNow.AddMinutes(-1);
    using(var r=await browser.PostAsync(prepare,new FormUrlEncodedContent(Fields())))Check(r.StatusCode==HttpStatusCode.Conflict&&data.Prepares==1,"expired simulation rejects stale valid-CSRF form");
    data.Selection.ExpiresAtUtc=DateTime.UtcNow.AddHours(1);
    data.EmptyConnections=true;
    using(var r=await browser.GetAsync(page))Check(r.StatusCode==HttpStatusCode.OK&&WebUtility.HtmlDecode(await r.Content.ReadAsStringAsync()).Contains("اتصالی ثبت نشده"),"empty connection state renders without null errors");
    data.EmptyConnections=false;data.Submitted=false;
    Console.WriteLine($"{checks} MVC/Razor checks passed. Production controller/views; synthetic services/identity, no SQL or Basalam.");
    if(args.Contains("--serve"))
    {
        Console.WriteLine("BROWSER_FIXTURE_URL="+address+"/fixture/start");
        using var stop=new CancellationTokenSource(TimeSpan.FromMinutes(10));await app.WaitForShutdownAsync(stop.Token);
    }
}
catch(Exception error){Console.Error.WriteLine(error);return 1;}
finally{await app.StopAsync();}
return 0;

public sealed class FixtureMarker;
sealed class OnlyTargetController:IApplicationFeatureProvider<ControllerFeature>
{
    public void PopulateFeature(IEnumerable<ApplicationPart> parts,ControllerFeature feature)
    {foreach(var c in feature.Controllers.Where(c=>c.AsType()!=typeof(MerchantSimulationController)).ToArray())feature.Controllers.Remove(c);}
}
sealed class FixtureRequester:IRequesterUser
{
    public FixtureRequester(IHttpContextAccessor accessor)=>Properties=new(){[typeof(BpmsIdentity).Name]=new BpmsIdentity{Id=FixtureData.Admin,UserName="Fixture",Culture="fa",IsAdmin=accessor.HttpContext!.User.IsInRole("Administrator")}};
    public UserId? Id{get;set;} public string Platform=>"fixture";public string? AppName=>"fixture";public string? Lang=>"fa";public string? Mobile=>null;public string? CorrelationId=>null;
    public Dictionary<string,object> Properties{get;set;}
    public Task<LanguageId> GetLangIdAsync(CancellationToken cancellationToken=default)=>throw new NotSupportedException();
    public List<Claim> Claims()=>[];public bool? IsInRole(string role)=>false;
}
sealed class FixtureData:IAdminMerchantSimulationService,IIntegrationScenarioQueue,IIntegrationProductReadinessApi
{
    public const string Admin="readiness-fixture-admin";public int Prepares;public bool Submitted,EmptyConnections;
    public IntegrationConnectionCommandRequest? LastScope;public ProductPreparationInput? LastInput;public string? LastActor;public int LastRevision;
    public ProductPreparationPolicy Policy=new();
    public IntegrationAdminSimulation Selection{get;}=new(){AdminUserId=Admin,ShopId=7,TenantId="fixture-tenant",ShopName="مغازه آزمایشی",MerchantIdentifier="fixture",ExpiresAtUtc=DateTime.UtcNow.AddHours(1)};
    bool Own(IntegrationConnectionCommandRequest s)=>s==new IntegrationConnectionCommandRequest(7,"fixture-tenant",10);
    ProductPreparationItem Item()=>new(1,1,new(ProductTransferDirection.ToPlatform,"44",Description:"<script>fixtureAttack</script>"),new(ProductTransferDirection.ToPlatform,"44"),
        Submitted?"Pending":"NeedsAttention",Submitted?[]:[new("CategoryId","RequiredPositiveCategory")],[],Submitted?Guid.Parse("f6651c77-423b-497a-931e-0337c3c073c2"):null,Submitted?11:null,null,[]);
    public Task<ProductPreparationBoard?> BoardAsync(IntegrationConnectionCommandRequest s,int skip,int take,CancellationToken ct)=>Task.FromResult<ProductPreparationBoard?>(Own(s)?new(1,Submitted?0:1,Submitted?1:0,0,0,[Item()]):null);
    public Task<ProductPreparationPolicy?> PolicyAsync(IntegrationConnectionCommandRequest s,ProductTransferDirection d,CancellationToken ct)=>Task.FromResult<ProductPreparationPolicy?>(Own(s)?Policy:null);
    public Task<bool> SetPolicyAsync(IntegrationConnectionCommandRequest s,ProductTransferDirection d,ProductPreparationPolicy policy,string actor,CancellationToken ct)
    {if(!Own(s))return Task.FromResult(false);Policy=policy;return Task.FromResult(true);}
    public Task<ProductPreparationItem?> PrepareAsync(IntegrationConnectionCommandRequest s,ProductPreparationInput input,int expectedRevision,string actor,CancellationToken ct)
    {if(!Own(s))return Task.FromResult<ProductPreparationItem?>(null);Prepares++;LastScope=s;LastInput=input;LastActor=actor;LastRevision=expectedRevision;Submitted=true;return Task.FromResult<ProductPreparationItem?>(Item());}
    public Task<IntegrationAdminSimulation?> GetAsync(string adminId,Guid simulationId,CancellationToken ct)=>Task.FromResult<IntegrationAdminSimulation?>(simulationId==Selection.Id&&AdminSimulationBoundary.Allows(Selection,adminId,DateTime.UtcNow)?Selection:null);
    public Task<IReadOnlyList<IntegrationScenarioConnection>> ConnectionsAsync(OwnedIntegrationShop s,CancellationToken ct)=>Task.FromResult<IReadOnlyList<IntegrationScenarioConnection>>(!EmptyConnections&&s==new OwnedIntegrationShop(7,"fixture-tenant")?[new(10,"اتصال ساختگی باسلام",Provider.Basalam,true)]:[]);
    public Task<IReadOnlyList<SimulationShop>> SearchShopsAsync(string? search,CancellationToken ct)=>throw new NotSupportedException();
    public Task<IntegrationAdminSimulation?> SelectAsync(string adminId,int shopId,CancellationToken ct)=>throw new NotSupportedException();
    public Task EndAsync(string adminId,Guid id,CancellationToken ct)=>throw new NotSupportedException();
    public Task<IntegrationTokenRequest?> RequestTokenAsync(string adminId,Guid id,int shopId,Provider provider,IntegrationCredentialType type,CancellationToken ct)=>throw new NotSupportedException();
    public Task<long> EnqueueAsync(OwnedIntegrationShop s,long c,IntegrationScenarioRequest r,CancellationToken ct)=>throw new NotSupportedException();
    public Task<bool> ProcessNextAsync(CancellationToken ct)=>throw new NotSupportedException();
    public Task<IReadOnlyList<IntegrationScenarioJob>> RecentAsync(OwnedIntegrationShop s,CancellationToken ct)=>throw new NotSupportedException();
}
