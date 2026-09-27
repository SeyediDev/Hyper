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

internal static class BasalamRetryChecks
{
    public static async Task Run(HyperIntegrationContext db, DbContextOptions<HyperIntegrationContext> options,
        Action<bool, string> check)
    {
        using var provider = new ProviderTransport();
        using var http = new HttpClient(provider, disposeHandler: false);
        using var sdk = new BasalamClient(new BasalamConfig(), httpClient: http);
        var oauth = new BasalamOAuthService(Options.Create(new BasalamOAuthSettings()), http,
            new EphemeralDataProtectionProvider());
        var connection = new ExternalIntegrationConnection
        {
            ShopId = 17, TenantId = "retry-tenant", Provider = Provider.Basalam, DisplayName = "retry-fixture",
            AccountIdentifier = "71", CredentialType = IntegrationCredentialType.OAuth2,
            CredentialsJson = "{\"webhookSecret\":\"retry-fixture-secret\"}", IsEnabled = true
        };
        db.ExternalIntegrationConnections.Add(connection);
        await db.SaveChangesAsync();
        var mapping = new ExternalProductMapping
        { ConnectionId = connection.Id, ShopId = 17, HyperProductId = 44, ExternalProductId = "12" };
        db.ExternalProductMappings.Add(mapping);
        db.ExternalOAuthTokens.Add(oauth.CreateTokenEntity(connection.Id, 17, "retry-tenant", Provider.Basalam,
            new() { AccessToken = "fixture-grant", ExpiresIn = 3600, Scope = "vendor.product.read vendor.product.write" }));
        await db.SaveChangesAsync();
        var resolver = Resolver(db, sdk);
        var outbox = new IntegrationOutbox(db, resolver);
        using var accounting = new AccountingTransport();
        using var accountingHttp = new HttpClient(accounting) { BaseAddress = new Uri("https://accounting.fixture.invalid/") };
        var commands = new HyperyekAccountingApiClient(accountingHttp);
        var ingress = new IntegrationWebhookIngress(db, new IntegrationWebhookVerifier());
        var queue = Queue(db, resolver);
        long version = 0;

        foreach (var status in new[] { HttpStatusCode.TooManyRequests, HttpStatusCode.ServiceUnavailable })
        foreach (var product in new[] { false, true })
        {
            var label = $"Basalam {(int)status} {(product ? "product" : "inventory")}";
            var id = product
                ? await outbox.EnqueueProductAsync(connection.Id, mapping.Id, ++version, new("12", null, "Retry title", 0), default)
                : await outbox.EnqueueInventoryAsync(connection.Id, mapping.Id, ++version, 0, default);
            var original = await db.IntegrationOutbox.AsNoTracking().SingleAsync(x => x.Id == id);
            provider.FailStatus = status; provider.FailMethod = HttpMethod.Patch;
            provider.RetryAfterHeader = product ? DateTimeOffset.UtcNow.AddMinutes(9).ToString("R", CultureInfo.InvariantCulture) : "540";
            var before = DateTime.UtcNow;
            var requests = provider.Requests;
            await outbox.ProcessConnectionAsync(connection.Id, default);
            var pending = await db.IntegrationOutbox.AsNoTracking().SingleAsync(x => x.Id == id);
            check(pending.Status == 0 && pending.Attempts == 1 && pending.LastError == "Http" + (int)status
                && pending.NextAttemptAtUtc > before.AddMinutes(8) && pending.CompletedAtUtc is null
                && pending.LeaseId is null && provider.Requests == requests + 2,
                label + " stores cooldown after one owner GET and one PATCH, releasing lease");
            requests = provider.Requests;
            check(!await outbox.ProcessConnectionAsync(connection.Id, default) && requests == provider.Requests,
                label + " cannot resend before persisted deadline");
            provider.FailStatus = null;
            await db.IntegrationOutbox.Where(x => x.Id == id)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.NextAttemptAtUtc, DateTime.UtcNow.AddSeconds(-1)));
            await using (var restarted = new HyperIntegrationContext(options))
            {
                using var restartedHttp = new HttpClient(provider, disposeHandler: false);
                using var restartedSdk = new BasalamClient(new BasalamConfig(), httpClient: restartedHttp);
                await new IntegrationOutbox(restarted, Resolver(restarted, restartedSdk)).ProcessConnectionAsync(connection.Id, default);
            }
            var delivered = await db.IntegrationOutbox.AsNoTracking().SingleAsync(x => x.Id == id);
            check(delivered.Status == 2 && delivered.Attempts == 2 && delivered.LastError is null
                && delivered.PayloadJson == original.PayloadJson && delivered.SourceVersion == original.SourceVersion
                && (product ? provider.ProductPatch.GetProperty("primary_price").GetInt64() == 0 : provider.Stock == 0),
                label + " fresh worker delivers original identity/version/payload with explicit zero");
        }

        foreach (var status in new[] { HttpStatusCode.TooManyRequests, HttpStatusCode.ServiceUnavailable })
        {
            provider.FailStatus = status; provider.FailMethod = HttpMethod.Get; provider.RetryAfterHeader = "540";
            var accepted = await ingress.ReceiveAsync(Event("basalam-retry-" + (int)status));
            var before = DateTime.UtcNow;
            var requests = provider.Requests;
            var calls = accounting.Calls;
            await queue.ProcessConnectionAsync(connection.Id, default);
            var pending = await db.IntegrationScenarioJobs.AsNoTracking().SingleAsync(x => x.Id == accepted.ScenarioJobId);
            check(pending.Status == IntegrationScenarioStatus.Pending && pending.Attempts == 1
                && pending.ErrorCode == "Http" + (int)status && pending.NextAttemptAtUtc >= before.AddMinutes(9)
                && pending.LeaseId is null && pending.CompletedAtUtc is null && provider.Requests == requests + 1
                && accounting.Calls == calls
                && await db.IntegrationWebhookInbox.AsNoTracking().AnyAsync(x => x.Id == accepted.InboxId
                    && x.Status == 0 && x.Error == pending.ErrorCode && x.ProcessedAtUtc == null),
                $"Basalam {(int)status} read preserves pending inbox and cooldown without accounting mutation");
            requests = provider.Requests;
            check(!await queue.ProcessConnectionAsync(connection.Id, default) && requests == provider.Requests,
                "catalog retry does not reread before its deadline");
            provider.FailStatus = null;
            await db.IntegrationScenarioJobs.Where(x => x.Id == accepted.ScenarioJobId)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.NextAttemptAtUtc, DateTime.UtcNow.AddSeconds(-1)));
            await using (var restarted = new HyperIntegrationContext(options))
            {
                using var restartedHttp = new HttpClient(provider, disposeHandler: false);
                using var restartedSdk = new BasalamClient(new BasalamConfig(), httpClient: restartedHttp);
                await Queue(restarted, Resolver(restarted, restartedSdk)).ProcessConnectionAsync(connection.Id, default);
            }
            check(accounting.Calls == calls + 1 && accounting.LastCommand?.SourceVersion == accepted.ScenarioJobId
                && accounting.LastCommand?.EventId == pending.EventId
                && await db.IntegrationWebhookInbox.AsNoTracking().AnyAsync(x => x.Id == accepted.InboxId && x.Status == 1 && x.Error == null),
                "fresh catalog worker completes original delivery exactly once after provider recovery");
        }

        foreach (var status in new[] { HttpStatusCode.TooManyRequests, HttpStatusCode.Unauthorized })
        {
            provider.FailStatus = status; provider.FailMethod = null; provider.RetryAfterHeader = "540";
            var exhausted = status == HttpStatusCode.TooManyRequests;
            var id = await outbox.EnqueueInventoryAsync(connection.Id, mapping.Id, ++version, 0, default);
            var accepted = await ingress.ReceiveAsync(Event("basalam-terminal-" + (int)status));
            if (exhausted)
            {
                await db.IntegrationOutbox.Where(x => x.Id == id)
                    .ExecuteUpdateAsync(s => s.SetProperty(x => x.Attempts, IntegrationRetryPolicy.MaxAttempts - 1));
                await db.IntegrationScenarioJobs.Where(x => x.Id == accepted.ScenarioJobId)
                    .ExecuteUpdateAsync(s => s.SetProperty(x => x.Attempts, IntegrationRetryPolicy.MaxAttempts - 1));
            }
            await outbox.ProcessConnectionAsync(connection.Id, default);
            await queue.ProcessConnectionAsync(connection.Id, default);
            var expectedAttempts = exhausted ? IntegrationRetryPolicy.MaxAttempts : 1;
            check(await db.IntegrationOutbox.AsNoTracking().AnyAsync(x => x.Id == id && x.Status == 3
                    && x.Attempts == expectedAttempts && x.LastError == "Http" + (int)status && x.CompletedAtUtc != null)
                && await db.IntegrationScenarioJobs.AsNoTracking().AnyAsync(x => x.Id == accepted.ScenarioJobId
                    && x.Status == IntegrationScenarioStatus.DeadLetter && x.Attempts == expectedAttempts)
                && await db.IntegrationWebhookInbox.AsNoTracking().AnyAsync(x => x.Id == accepted.InboxId && x.Status == 2),
                exhausted ? "Retry-After cannot bypass finite durable attempt limit in either queue"
                    : "401 with Retry-After remains terminal in both queues, no invented credential recovery");
        }

        IntegrationStrategyResolver Resolver(HyperIntegrationContext context, BasalamClient client) =>
            new([new BasalamSdkAdapter(client, new BasalamOAuthStore(context, oauth))]);
        IntegrationScenarioQueue Queue(HyperIntegrationContext context, IntegrationStrategyResolver strategies) =>
            new(context, new IntegrationScenarioProcessor(context, strategies, new NoCapture(),
                Options.Create(new IntegrationInventoryCaptureOptions()), Options.Create(new BasalamOAuthSettings()), null!,
                new IntegrationBusinessEventDispatcher(commands, new UnregisteredIntegrationEngagementPort(),
                    new NoReservations(), context, strategies), commands));
        WebhookIngressRequest Event(string id) => new(Hyper.Integration.Contracts.IntegrationProvider.Basalam,
            "71", id, "product.updated", null, null, Encoding.UTF8.GetBytes("{\"externalProductId\":\"12\"}"),
            Authorization: "Bearer retry-fixture-secret");
    }
}
