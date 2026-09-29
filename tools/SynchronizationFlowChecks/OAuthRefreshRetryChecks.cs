using System.Globalization;
using System.Net;
using System.Text;
using Basalam.SDK;
using Basalam.SDK.Config;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Infrastructure.Features.Integrations;
using Hyper.Integration.Contracts;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Provider = Hyper.Integration.Domain.Entities.Integrations.IntegrationProvider;

internal static class OAuthRefreshRetryChecks
{
    public static async Task Run(DbContextOptions<HyperIntegrationContext> options, Action<bool, string> check)
    {
        await using var db = new HyperIntegrationContext(options);
        using var provider = new ProviderTransport();
        using var auth = new RefreshTransport { InnerHandler = provider };
        using var http = new HttpClient(auth, disposeHandler: false);
        using var sdk = new BasalamClient(new BasalamConfig(), httpClient: http);
        var oauth = new BasalamOAuthService(Options.Create(new BasalamOAuthSettings()), http, new EphemeralDataProtectionProvider(), FixtureProtection.Provider());
        var connection = new ExternalIntegrationConnection
        {
            ShopId = 47, TenantId = "refresh-retry", Provider = Provider.Basalam, DisplayName = "refresh fixture",
            AccountIdentifier = "71", CredentialType = IntegrationCredentialType.OAuth2,
            CredentialsJson = "{\"webhookSecret\":\"refresh-fixture-secret\"}", IsEnabled = true
        };
        db.ExternalIntegrationConnections.Add(connection);
        await db.SaveChangesAsync();
        var mapping = new ExternalProductMapping { ConnectionId = connection.Id, ShopId = 47, HyperProductId = 44, ExternalProductId = "12" };
        var token = oauth.CreateTokenEntity(connection.Id, 47, connection.TenantId, Provider.Basalam,
            new() { AccessToken = "fixture-expired", RefreshToken = "fixture-refresh", ExpiresIn = 3600 });
        db.ExternalProductMappings.Add(mapping); db.ExternalOAuthTokens.Add(token);
        await db.SaveChangesAsync();
        var outbox = new IntegrationOutbox(db, Resolver(db, sdk));
        using var accounting = new AccountingTransport();
        using var accountingHttp = new HttpClient(accounting) { BaseAddress = new Uri("https://accounting.fixture.invalid/") };
        var commands = new HyperyekAccountingApiClient(accountingHttp);
        var ingress = new IntegrationWebhookIngress(db, new IntegrationWebhookVerifier(FixtureProtection.LegacyVault()));
        var queue = Queue(db, sdk);
        long version = 0;

        foreach (var status in new[] { 429, 503 })
        foreach (var path in new[] { "inventory", "product", "catalog" })
        {
            await Expire();
            var saved = await Snapshot();
            auth.Status = status;
            auth.Header = path == "product" ? DateTimeOffset.UtcNow.AddMinutes(9).ToString("R", CultureInfo.InvariantCulture) : "540";
            var catalog = path == "catalog";
            var eventId = $"refresh-{status}-{path}";
            var inbox = catalog ? await ingress.ReceiveAsync(Event(eventId)) : null;
            var id = catalog ? inbox!.ScenarioJobId!.Value : path == "product"
                ? await outbox.EnqueueProductAsync(connection.Id, mapping.Id, ++version, new("12", null, "Refreshed title", 0), default)
                : await outbox.EnqueueInventoryAsync(connection.Id, mapping.Id, ++version, 0, default);
            var before = DateTime.UtcNow;
            var calls = auth.Calls; var providerCalls = provider.Requests; var accountingCalls = accounting.Calls;
            await Process();
            var code = "OAuthRefreshHttp" + status;
            var pending = catalog
                ? await db.IntegrationScenarioJobs.AsNoTracking().AnyAsync(x => x.Id == id && x.Status == IntegrationScenarioStatus.Pending
                    && x.ErrorCode == code && x.Attempts == 1 && x.NextAttemptAtUtc > before.AddMinutes(8) && x.LeaseId == null)
                : await db.IntegrationOutbox.AsNoTracking().AnyAsync(x => x.Id == id && x.Status == 0 && x.LastError == code
                    && x.Attempts == 1 && x.NextAttemptAtUtc > before.AddMinutes(8) && x.LeaseId == null);
            var after = await Snapshot();
            check(pending && auth.Calls == calls + 1 && provider.Requests == providerCalls && accounting.Calls == accountingCalls
                && after.AccessToken == saved.AccessToken && after.RefreshToken == saved.RefreshToken && after.ExpiresAtUtc == saved.ExpiresAtUtc,
                $"refresh {status} {path}: one auth attempt, persisted cooldown, unchanged token, no downstream effect");
            check(!await Process() && auth.Calls == calls + 1, "refresh cooldown prevents early token and business requests");
            auth.Status = 200;
            if (catalog)
                await db.IntegrationScenarioJobs.Where(x => x.Id == id).ExecuteUpdateAsync(s => s.SetProperty(x => x.NextAttemptAtUtc, DateTime.UtcNow.AddSeconds(-1)));
            else
                await db.IntegrationOutbox.Where(x => x.Id == id).ExecuteUpdateAsync(s => s.SetProperty(x => x.NextAttemptAtUtc, DateTime.UtcNow.AddSeconds(-1)));
            await using (var restarted = new HyperIntegrationContext(options))
            {
                using var freshHttp = new HttpClient(auth, disposeHandler: false);
                using var freshSdk = new BasalamClient(new BasalamConfig(), httpClient: freshHttp);
                if (catalog) await Queue(restarted, freshSdk).ProcessConnectionAsync(connection.Id, default);
                else await new IntegrationOutbox(restarted, Resolver(restarted, freshSdk)).ProcessConnectionAsync(connection.Id, default);
            }
            after = await Snapshot();
            var completed = catalog
                ? await db.IntegrationScenarioJobs.AsNoTracking().AnyAsync(x => x.Id == id && x.Status == IntegrationScenarioStatus.Completed && x.Attempts == 2)
                    && accounting.LastCommand?.EventId == eventId && accounting.LastCommand?.SourceVersion == id
                    && await db.IntegrationWebhookInbox.AsNoTracking().AnyAsync(x => x.Id == inbox!.InboxId && x.Status == 1 && x.Error == null)
                : await db.IntegrationOutbox.AsNoTracking().AnyAsync(x => x.Id == id && x.Status == 2 && x.Attempts == 2 && x.SourceVersion == version);
            check(completed && auth.Calls == calls + 2 && oauth.DecryptToken(after.AccessToken) == "fixture-grant"
                && after.ExpiresAtUtc > DateTime.UtcNow.AddMinutes(50), "fresh worker refreshes grant then completes original queued effect");
            check(!await Process() && auth.Calls == calls + 2, "completed refresh recovery is not delivered again");
            Task<bool> Process() => catalog ? queue.ProcessConnectionAsync(connection.Id, default) : outbox.ProcessConnectionAsync(connection.Id, default);
        }

        foreach (var status in new[] { 429, 400 })
        {
            await Expire(); auth.Status = status; auth.Header = "540";
            var saved = await Snapshot();
            var id = await outbox.EnqueueInventoryAsync(connection.Id, mapping.Id, ++version, 0, default);
            var inbox = await ingress.ReceiveAsync(Event("refresh-terminal-" + status));
            if (status == 429)
            {
                await db.IntegrationOutbox.Where(x => x.Id == id).ExecuteUpdateAsync(s => s.SetProperty(x => x.Attempts, IntegrationRetryPolicy.MaxAttempts - 1));
                await db.IntegrationScenarioJobs.Where(x => x.Id == inbox.ScenarioJobId).ExecuteUpdateAsync(s => s.SetProperty(x => x.Attempts, IntegrationRetryPolicy.MaxAttempts - 1));
            }
            await outbox.ProcessConnectionAsync(connection.Id, default);
            await queue.ProcessConnectionAsync(connection.Id, default);
            var after = await Snapshot();
            check(await db.IntegrationOutbox.AsNoTracking().AnyAsync(x => x.Id == id && x.Status == 3)
                && await db.IntegrationScenarioJobs.AsNoTracking().AnyAsync(x => x.Id == inbox.ScenarioJobId && x.Status == IntegrationScenarioStatus.DeadLetter)
                && await db.IntegrationWebhookInbox.AsNoTracking().AnyAsync(x => x.Id == inbox.InboxId && x.Status == 2)
                && saved.AccessToken == after.AccessToken && saved.RefreshToken == after.RefreshToken,
                status == 429 ? "refresh retry exhaustion is terminal in both queues without erasing grant"
                    : "invalid_grant rejection remains terminal in both queues and requires attention, not endless refresh");
        }

        Task<ExternalOAuthToken> Snapshot() => db.ExternalOAuthTokens.AsNoTracking().SingleAsync(x => x.Id == token.Id);
        Task<int> Expire() => db.ExternalOAuthTokens.Where(x => x.Id == token.Id).ExecuteUpdateAsync(s => s.SetProperty(x => x.ExpiresAtUtc, DateTime.UtcNow.AddMinutes(-1)));
        IntegrationStrategyResolver Resolver(HyperIntegrationContext context, BasalamClient client) => new([new BasalamSdkAdapter(client, new BasalamOAuthStore(context, oauth, FixtureProtection.LegacyVault()))]);
        IntegrationScenarioQueue Queue(HyperIntegrationContext context, BasalamClient client)
        {
            var resolver = Resolver(context, client);
            return new(context, new IntegrationScenarioProcessor(context, resolver, new NoCapture(), Options.Create(new IntegrationInventoryCaptureOptions()),
                Options.Create(new BasalamOAuthSettings()), null!, new IntegrationBusinessEventDispatcher(commands, new UnregisteredIntegrationEngagementPort(),
                    new NoReservations(), context, resolver), commands));
        }
        WebhookIngressRequest Event(string id) => new(Hyper.Integration.Contracts.IntegrationProvider.Basalam, "71", id, "product.updated", null, null,
            Encoding.UTF8.GetBytes("{\"externalProductId\":\"12\"}"), Authorization: "Bearer refresh-fixture-secret");
    }

    private sealed class RefreshTransport : DelegatingHandler
    {
        public int Status; public string? Header; public int Calls;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            if (request.RequestUri?.Host != "auth.basalam.com") return base.SendAsync(request, ct);
            Calls++;
            var response = new HttpResponseMessage((HttpStatusCode)Status)
            {
                Content = new StringContent(Status == 200 ? """{"access_token":"fixture-grant","expires_in":3600}"""
                    : """{"error":"invalid_grant","error_description":"sensitive-fixture"}""", Encoding.UTF8, "application/json")
            };
            if (Header is not null) response.Headers.TryAddWithoutValidation("Retry-After", Header);
            return Task.FromResult(response);
        }
    }
}
