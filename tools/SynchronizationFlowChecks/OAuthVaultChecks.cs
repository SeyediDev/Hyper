using System.Net;
using System.Text;
using Basalam.SDK.Auth;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Infrastructure.Features.Integrations;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

internal static class OAuthVaultChecks
{
    public static async Task Run(DbContextOptions<HyperIntegrationContext> options, Action<bool, string> check)
    {
        await using var db = new HyperIntegrationContext(options);
        using var transport = new RefreshTransport();
        using var http = new HttpClient(transport);
        var oauth = new BasalamOAuthService(Options.Create(new BasalamOAuthSettings
        { ClientId = "vault-fixture-client", ClientSecret = "vault-fixture-secret" }), http, new EphemeralDataProtectionProvider());
        var first = Connection("vault-a");
        var other = Connection("vault-b");
        db.ExternalIntegrationConnections.AddRange(first, other);
        await db.SaveChangesAsync();
        var token = oauth.CreateTokenEntity(first.Id, 37, first.TenantId, first.Provider,
            new() { AccessToken = "vault-access-a", RefreshToken = "vault-refresh-a", ExpiresIn = 3600, Scope = "fixture-scope" });
        var otherToken = oauth.CreateTokenEntity(other.Id, 37, other.TenantId, other.Provider,
            new() { AccessToken = "vault-access-b", RefreshToken = "vault-refresh-b", ExpiresIn = 3600 });
        db.ExternalOAuthTokens.AddRange(token, otherToken);
        await db.SaveChangesAsync();
        var store = new BasalamOAuthStore(db, oauth);
        check((await store.GetTokenAsync(first, default))?.AccessToken == "vault-access-a"
            && (await store.GetTokenAsync(other, default))?.AccessToken == "vault-access-b" && transport.Calls == 0,
            "vault selects tenant-specific grant even when shop/vendor IDs match");
        foreach (var scope in new[]
        {
            new ExternalIntegrationConnection { Id = first.Id, ShopId = 99, TenantId = first.TenantId, Provider = first.Provider },
            new ExternalIntegrationConnection { Id = first.Id, ShopId = 37, TenantId = other.TenantId, Provider = first.Provider },
            new ExternalIntegrationConnection { Id = first.Id, ShopId = 37, TenantId = first.TenantId, Provider = (IntegrationProvider)99 },
            new ExternalIntegrationConnection { Id = long.MaxValue, ShopId = 37, TenantId = first.TenantId, Provider = first.Provider }
        })
            check(await store.GetTokenAsync(scope, default) is null && transport.Calls == 0,
                "mismatched shop/tenant/provider/connection cannot obtain or refresh a grant");
        await db.ExternalOAuthTokens.Where(x => x.Id == token.Id).ExecuteUpdateAsync(s => s.SetProperty(x => x.IsActive, false));
        check(await store.GetTokenAsync(first, default) is null && transport.Calls == 0, "inactive token is not returned or refreshed");
        await db.ExternalOAuthTokens.Where(x => x.Id == token.Id).ExecuteUpdateAsync(s => s
            .SetProperty(x => x.IsActive, true).SetProperty(x => x.ExpiresAtUtc, DateTime.UtcNow.AddMinutes(-1))
            .SetProperty(x => x.RefreshToken, (string?)null));
        check(await store.GetTokenAsync(first, default) is null && transport.Calls == 0, "expired grant without refresh token fails closed");

        // Simulate another writer rotating the database grant while this context
        // still tracks the old token. Refresh must never use that tracked value.
        var current = oauth.CreateTokenEntity(first.Id, 37, first.TenantId, first.Provider,
            new() { AccessToken = "vault-current", RefreshToken = "vault-current-refresh", ExpiresIn = 30, Scope = "fixture-scope" });
        await db.ExternalOAuthTokens.Where(x => x.Id == token.Id).ExecuteUpdateAsync(s => s
            .SetProperty(x => x.AccessToken, current.AccessToken).SetProperty(x => x.RefreshToken, current.RefreshToken)
            .SetProperty(x => x.ExpiresAtUtc, current.ExpiresAtUtc).SetProperty(x => x.RawTokenResponse, "legacy-fixture-raw"));
        first.DisplayName = "unsaved caller edit";
        transport.ExpectedRefresh = "vault-current-refresh";
        transport.Body = """{"access_token":"vault-rotated","refresh_token":"vault-rotated-refresh","expires_in":3600,"scope":"updated-scope"}""";
        var grant = await store.GetTokenAsync(first, default);
        check(grant?.AccessToken == "vault-rotated" && grant.RefreshToken == "vault-rotated-refresh" && transport.Calls == 1,
            "refresh reads current locked database grant despite stale EF tracked entity");
        var saved = await Snapshot();
        check(saved.AccessToken != "vault-rotated" && oauth.DecryptToken(saved.AccessToken) == "vault-rotated"
            && saved.RefreshToken != "vault-rotated-refresh" && oauth.DecryptToken(saved.RefreshToken!) == "vault-rotated-refresh"
            && saved.ExpiresAtUtc > DateTime.UtcNow.AddMinutes(50) && saved.Scopes == "updated-scope" && saved.RawTokenResponse is null,
            "rotated credentials persist encrypted with expiry/scope and no raw duplicate");
        check((await db.ExternalIntegrationConnections.AsNoTracking().SingleAsync(x => x.Id == first.Id)).DisplayName == "vault fixture"
            && first.DisplayName == "unsaved caller edit",
            "refresh does not flush or discard unrelated pending caller changes");
        check((await db.ExternalOAuthTokens.AsNoTracking().SingleAsync(x => x.Id == otherToken.Id)).AccessToken == otherToken.AccessToken,
            "refresh leaves other tenant token unchanged");
        check((await store.GetTokenAsync(first, default))?.AccessToken == "vault-rotated" && transport.Calls == 1,
            "fresh grant is reused without a second OAuth request");

        await Expire();
        transport.ExpectedRefresh = "vault-rotated-refresh";
        transport.Body = """{"access_token":"vault-preserved","expires_in":3600}""";
        grant = await store.GetTokenAsync(first, default);
        saved = await Snapshot();
        check(grant?.RefreshToken == "vault-rotated-refresh" && saved.Scopes == "updated-scope",
            "refresh response omitting refresh token and scopes preserves their database values");

        await Expire();
        saved = await Snapshot();
        foreach (var invalid in new[] { false, true })
        {
            transport.Status = invalid ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
            transport.Body = invalid ? """{"access_token":"should-not-persist","expires_in":-1}""" : "sensitive-provider-fixture";
            try { await store.GetTokenAsync(first, default); throw new Exception("Expected OAuth rejection"); }
            catch (InvalidOperationException ex)
            {
                var after = await Snapshot();
                check(after.AccessToken == saved.AccessToken && after.RefreshToken == saved.RefreshToken
                    && after.ExpiresAtUtc == saved.ExpiresAtUtc && !ex.Message.Contains("sensitive-provider-fixture"),
                    "failed or invalid refresh rolls back without partial credential replacement or raw error disclosure");
            }
        }
        transport.Status = HttpStatusCode.OK;
        transport.Body = """{"access_token":"vault-concurrent","refresh_token":"vault-concurrent-refresh","expires_in":3600}""";
        transport.Hold();
        var callsBefore = transport.Calls;
        await using (var workerA = new HyperIntegrationContext(options))
        await using (var workerB = new HyperIntegrationContext(options))
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var a = new BasalamOAuthStore(workerA, oauth).GetTokenAsync(first, timeout.Token);
            Task<TokenInfo?>? b = null;
            try
            {
                await transport.Entered!.Task.WaitAsync(timeout.Token);
                b = new BasalamOAuthStore(workerB, oauth).GetTokenAsync(first, timeout.Token);
            }
            finally { transport.Release!.TrySetResult(); }
            var grants = await Task.WhenAll(a, b!);
            check(grants.All(x => x?.AccessToken == "vault-concurrent" && x.RefreshToken == "vault-concurrent-refresh")
                && transport.Calls == callsBefore + 1,
                "concurrent worker contexts share one rotated grant and make only one refresh request");
        }

        await Expire();
        transport.ExpectedRefresh = "vault-concurrent-refresh";
        transport.Hold();
        saved = await Snapshot();
        using (var cancellation = new CancellationTokenSource())
        {
            var cancelled = store.GetTokenAsync(first, cancellation.Token);
            await transport.Entered!.Task.WaitAsync(TimeSpan.FromSeconds(30));
            cancellation.Cancel();
            try { await cancelled; throw new Exception("Expected cancelled refresh"); }
            catch (OperationCanceledException)
            {
                var after = await Snapshot();
                check(after.AccessToken == saved.AccessToken && after.RefreshToken == saved.RefreshToken,
                    "cancelled refresh rolls back and releases the token transaction");
            }
            finally { transport.Release!.TrySetResult(); }
        }
        transport.Body = """{"access_token":"vault-after-cancel","expires_in":3600}""";
        check((await store.GetTokenAsync(first, default))?.AccessToken == "vault-after-cancel",
            "subsequent refresh can acquire token lock after cancellation");

        Task<ExternalOAuthToken> Snapshot() => db.ExternalOAuthTokens.AsNoTracking().SingleAsync(x => x.Id == token.Id);
        Task<int> Expire() => db.ExternalOAuthTokens.Where(x => x.Id == token.Id)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.ExpiresAtUtc, DateTime.UtcNow.AddSeconds(20)));
    }

    private static ExternalIntegrationConnection Connection(string tenant) => new()
    {
        ShopId = 37, TenantId = tenant, Provider = IntegrationProvider.Basalam, DisplayName = "vault fixture",
        AccountIdentifier = "71", CredentialType = IntegrationCredentialType.OAuth2, CredentialsJson = "{}", IsEnabled = true
    };

    private sealed class RefreshTransport : HttpMessageHandler
    {
        public int Calls;
        public string? ExpectedRefresh;
        public string Body = "{}";
        public HttpStatusCode Status = HttpStatusCode.OK;
        public TaskCompletionSource? Entered;
        public TaskCompletionSource? Release;
        public void Hold()
        {
            Entered = new(TaskCreationOptions.RunContinuationsAsynchronously);
            Release = new(TaskCreationOptions.RunContinuationsAsynchronously);
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            Interlocked.Increment(ref Calls);
            var form = QueryHelpers.ParseQuery("?" + await request.Content!.ReadAsStringAsync(ct));
            if (request.Method != HttpMethod.Post || request.RequestUri?.AbsoluteUri != "https://auth.basalam.com/oauth/token"
                || form["grant_type"] != "refresh_token" || form["refresh_token"] != ExpectedRefresh
                || form["client_id"] != "vault-fixture-client" || form["client_secret"] != "vault-fixture-secret")
                throw new InvalidOperationException("Unexpected fixture refresh contract or stale credential");
            Entered?.TrySetResult();
            if (Release is not null) await Release.Task.WaitAsync(ct);
            return new HttpResponseMessage(Status) { Content = new StringContent(Body, Encoding.UTF8, "application/json") };
        }
    }
}
