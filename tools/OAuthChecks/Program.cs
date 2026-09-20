using System.Net;
using System.Text;
using System.Text.Json;
using Hyper.Infrastructure.Features.Integrations;
using Hyper.Domain.Entities.Integrations;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

var count = 0;
void Check(bool condition, string name) { if (!condition) throw new Exception(name); count++; Console.WriteLine("PASS " + name); }
void Reject(Action action, string name) { try { action(); } catch (InvalidOperationException) { Check(true,name); return; } throw new Exception(name); }
async Task RejectAsync(Func<Task> action,string name) { try { await action(); } catch (InvalidOperationException) { Check(true,name); return; } throw new Exception(name); }
var settings = new BasalamOAuthSettings { ClientId="test-client", ClientSecret="test-secret",
    RedirectUri="http://localhost:5000/api/auth/basalam/callback" };
var keys = new EphemeralDataProtectionProvider();
var handler = new FakeHttp();
var service = new BasalamOAuthService(Options.Create(settings),new HttpClient(handler),keys);
Check(service.ConfigurationError() is null,"valid configuration");
var nonce = BasalamOAuthService.Nonce();
var requestId = Guid.NewGuid();
var simulationId = Guid.NewGuid();
var url = new Uri(service.CreateAuthorizationUrl(requestId,simulationId,"admin-test",nonce));
var query = QueryHelpers.ParseQuery(url.Query);
Check(url.GetLeftPart(UriPartial.Path)=="https://basalam.com/accounts/sso","official authorize endpoint");
Check(query["client_id"]=="test-client" && query["redirect_uri"]==settings.RedirectUri
    && query["response_type"]=="code" && query.ContainsKey("scope"),"standard authorization parameters");
Check(!query.ContainsKey("encrypted_state") && !query.ContainsKey("client_secret"),"standard state; no secret in URL");
var state = service.ReadState(query["state"],nonce);
Check(state.RequestId==requestId && state.SimulationId==simulationId && state.AdminId=="admin-test","shop request binding");
Reject(()=>service.ReadState(query["state"],"different-browser"),"wrong browser rejected");
Reject(()=>service.ReadState("tampered",nonce),"tampered state rejected");
Reject(()=>service.ReadState(null,nonce),"missing state rejected");
var expired = keys.CreateProtector("Hyper.Basalam.Authorization.v2").ToTimeLimitedDataProtector()
    .Protect(JsonSerializer.Serialize(state),DateTimeOffset.UtcNow.AddSeconds(-1));
Reject(()=>service.ReadState(expired,nonce),"expired state rejected");
var second = new BasalamOAuthService(Options.Create(settings),new HttpClient(handler),keys);
Check(second.ReadState(query["state"],nonce).RequestId==requestId,"state survives new service instance without memory cache");
settings.RedirectUri="https://example.org/api/auth/basalam/callback";
Reject(()=>service.ReadState(query["state"],nonce),"changed redirect rejected");
settings.RedirectUri=state.RedirectUri;
settings.UsePkce=true;
var pkceQuery=QueryHelpers.ParseQuery(new Uri(service.CreateAuthorizationUrl(requestId,simulationId,"admin-test",nonce)).Query);
Check(pkceQuery["code_challenge_method"]=="S256" && !string.IsNullOrEmpty(service.ReadState(pkceQuery["state"],nonce).CodeVerifier),"optional PKCE");
handler.Json="""{"access_token":"fake-access","refresh_token":"fake-refresh","expires_in":3600,"token_type":"Bearer"}""";
var token=await service.ExchangeCodeForTokenAsync("fake-code",state,default);
using (var tokenRequest = JsonDocument.Parse(handler.Body!))
{
    Check(handler.LastUri=="https://auth.basalam.com/oauth/token"
        && handler.ContentType == "application/json; charset=utf-8"
        && tokenRequest.RootElement.GetProperty("grant_type").GetString() == "authorization_code"
        && tokenRequest.RootElement.GetProperty("client_secret").GetString() == "test-secret"
        && tokenRequest.RootElement.GetProperty("code").GetString() == "fake-code",
        "confidential server JSON code exchange");
}
var entity=service.CreateTokenEntity(7,9,"tenant-test",IntegrationProvider.Basalam,token);
Check(entity.AccessToken!="fake-access" && service.DecryptToken(entity.AccessToken)=="fake-access"
    && service.DecryptToken(entity.RefreshToken!)=="fake-refresh" && entity.RawTokenResponse is null,"encrypted credentials; no raw duplicate");
handler.Json="""{"vendor":{"id":123,"title":"test booth"}}""";
var vendor=await service.GetVendorAsync(token,default);
Check(vendor.Id=="123" && handler.LastUri=="https://core.basalam.com/v3/users/me" && handler.Authorization=="Bearer fake-access","vendor identified through token");
handler.Json="""{"vendor":null}""";
await RejectAsync(()=>service.GetVendorAsync(token,default),"account without booth rejected");
handler.Json="""{"access_token":""}""";
await RejectAsync(()=>service.ExchangeCodeForTokenAsync("fake-code",state,default),"empty access token rejected");
handler.Json="""{"access_token":"fake-access","expires_in":-1}""";
await RejectAsync(()=>service.ExchangeCodeForTokenAsync("fake-code",state,default),"invalid expiry rejected");
handler.Status=HttpStatusCode.BadRequest;
await RejectAsync(()=>service.ExchangeCodeForTokenAsync("fake-code",state,default),"provider failure rejected");
settings.ClientId="your-client-id";
Check(service.ConfigurationError() is not null,"placeholder configuration rejected");
Console.WriteLine($"Completed {count} checks; no real HTTP requests or tokens.");

sealed class FakeHttp : HttpMessageHandler
{
 public string Json {get;set;}="{}";
 public HttpStatusCode Status {get;set;}=HttpStatusCode.OK;
 public string? Body {get;private set;}
 public string? ContentType {get;private set;}
 public string? LastUri {get;private set;}
 public string? Authorization {get;private set;}
 protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,CancellationToken ct)
 {
  LastUri=request.RequestUri!.AbsoluteUri;
  Authorization=request.Headers.Authorization?.ToString();
 Body=request.Content is null?null:await request.Content.ReadAsStringAsync(ct);
  ContentType=request.Content?.Headers.ContentType?.ToString();
  return new HttpResponseMessage(Status){Content=new StringContent(Json,Encoding.UTF8,"application/json")};
 }
}
