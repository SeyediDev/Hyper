using System.Net;
using System.Text;
using System.Text.Json;
using Basalam.SDK;
using Basalam.SDK.Config;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Infrastructure.Features.Integrations;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

internal static class PersistenceChecks
{
    internal static async Task Run(VaultSqlFixture fixture)
    {
        var vault = FixtureProtection.Vault();
        await Migration(fixture, vault);
        await Writers(fixture, vault);
        await Registration(fixture, vault);
    }

    private static async Task Migration(VaultSqlFixture fixture, IntegrationCredentialVault vault)
    {
        const string json = " {\"webhookSecret\":\"old-value\",\"future\":{\"a\":1}} ";
        var row = await fixture.SeedAsync(json);
        var other = await fixture.SeedAsync("{\"webhookSecret\":\"other-row\"}");
        await using (var db = fixture.Open())
        {
            var migration = new IntegrationCredentialMigration(db, vault);
            foreach (var scope in new[] { new OwnedIntegrationShop(row.ShopId + 1, row.TenantId),
                new OwnedIntegrationShop(row.ShopId, "TENANT-A"), new OwnedIntegrationShop(row.ShopId, "tenant-b") })
                await VaultCheck.RejectAsync(async () => await migration.PrepareAsync(scope, row.Id, default), "migration requires ordinal owned scope");
            var plan = await migration.PrepareAsync(new(row.ShopId, row.TenantId), row.Id, default);
            VaultCheck.That(!plan.IsAlreadyProtected && plan.ConnectionId == row.Id
                && plan.ToString() == nameof(IntegrationCredentialMigrationPlan)
                && !JsonSerializer.Serialize(plan).Contains("old-value", StringComparison.Ordinal), "migration plan diagnostics expose only safe metadata");
            var tracked = await db.ExternalIntegrationConnections.SingleAsync(c => c.Id == other.Id);
            tracked.DisplayName = "must-not-be-flushed";
            VaultCheck.That(await migration.ApplyAsync(plan, default) == IntegrationCredentialMigrationResult.Migrated,
                "explicit migration protects selected legacy row with strict reader configuration");
            var stored = await fixture.ReadAsync(row.Id);
            VaultCheck.That(stored.CredentialsJson != json && vault.Read(stored).Json == json,
                "migration preserves complete exact credential document and secret");
            VaultCheck.That((await fixture.ReadAsync(other.Id)).CredentialsJson == other.CredentialsJson
                && (await fixture.ReadAsync(other.Id)).DisplayName == other.DisplayName,
                "migration neither alters another row nor flushes unrelated tracked edits");
            VaultCheck.That(await migration.ApplyAsync(plan, default) == IntegrationCredentialMigrationResult.AlreadyProtected,
                "reapplying successful plan is idempotent");
            var protectedPlan = await migration.PrepareAsync(new(row.ShopId, row.TenantId), row.Id, default);
            VaultCheck.That(protectedPlan.IsAlreadyProtected
                && await migration.ApplyAsync(protectedPlan, default) == IntegrationCredentialMigrationResult.AlreadyProtected
                && (await fixture.ReadAsync(row.Id)).CredentialsJson == stored.CredentialsJson,
                "already protected migration validates readability without rewrapping");
        }
        foreach (var changed in new[] { json.Replace("old-value", "OLD-value", StringComparison.Ordinal), json + " " })
        {
            var target = await fixture.SeedAsync(json);
            await using var db = fixture.Open();
            var migration = new IntegrationCredentialMigration(db, vault);
            var plan = await migration.PrepareAsync(new(target.ShopId, target.TenantId), target.Id, default);
            await fixture.SetDocumentAsync(target.Id, changed);
            VaultCheck.That(await migration.ApplyAsync(plan, default) == IntegrationCredentialMigrationResult.Conflict
                && (await fixture.ReadAsync(target.Id)).CredentialsJson == changed,
                "CAS detects case-only or trailing-space credential mutation exactly");
        }
        foreach (var scopeMutation in new[] { "tenant-case", "account-case", "account-space", "provider", "credential-type", "shop" })
        {
            var target = await fixture.SeedAsync(json);
            await using var db = fixture.Open();
            var migration = new IntegrationCredentialMigration(db, vault);
            var plan = await migration.PrepareAsync(new(target.ShopId, target.TenantId), target.Id, default);
            await using (var writer = fixture.Open())
            {
                var query = writer.ExternalIntegrationConnections.Where(c => c.Id == target.Id);
                if (scopeMutation == "tenant-case") await query.ExecuteUpdateAsync(s => s.SetProperty(c => c.TenantId, "TENANT-A"));
                else if (scopeMutation == "account-case")
                {
                    // Start from a letter-bearing identity to exercise CI_AS.
                    await query.ExecuteUpdateAsync(s => s.SetProperty(c => c.AccountIdentifier, "account"));
                    plan = await migration.PrepareAsync(new(target.ShopId, target.TenantId), target.Id, default);
                    await query.ExecuteUpdateAsync(s => s.SetProperty(c => c.AccountIdentifier, "ACCOUNT"));
                }
                else if (scopeMutation == "account-space") await query.ExecuteUpdateAsync(s => s.SetProperty(c => c.AccountIdentifier, target.AccountIdentifier + " "));
                else if (scopeMutation == "provider") await query.ExecuteUpdateAsync(s => s.SetProperty(c => c.Provider, IntegrationProvider.Custom));
                else if (scopeMutation == "credential-type") await query.ExecuteUpdateAsync(s => s.SetProperty(c => c.CredentialType, IntegrationCredentialType.BearerToken));
                else await query.ExecuteUpdateAsync(s => s.SetProperty(c => c.ShopId, target.ShopId + 100000));
            }
            VaultCheck.That(await migration.ApplyAsync(plan, default) == IntegrationCredentialMigrationResult.Conflict
                && (await fixture.ReadAsync(target.Id)).CredentialsJson == json, "CAS preserves row after scope mutation " + scopeMutation);
        }
        foreach (var invalid in new[] { "{", "{\"webhookSecret\":null}", "hyper-credentials:v2:unknown", "hyper-credentials:v1:corrupt" })
        {
            var target = await fixture.SeedAsync(invalid);
            await using var db = fixture.Open();
            await VaultCheck.RejectAsync(async () => await new IntegrationCredentialMigration(db, vault)
                .PrepareAsync(new(target.ShopId, target.TenantId), target.Id, default), "migration refuses malformed or unreadable storage");
            VaultCheck.That((await fixture.ReadAsync(target.Id)).CredentialsJson == invalid, "invalid migration leaves source untouched");
        }
    }

    private static async Task Writers(VaultSqlFixture fixture, IntegrationCredentialVault vault)
    {
        using var http = new HttpClient(new NoNetwork());
        var oauth = new BasalamOAuthService(Options.Create(new BasalamOAuthSettings()), http,
            new EphemeralDataProtectionProvider(), FixtureProtection.Provider());
        var shop = fixture.NextShop();
        long registryId;
        await using (var db = fixture.Open())
        {
            var response = await new IntegrationManagementApi(db, vault).CreateConnectionAsync(new(shop, "tenant-a",
                Hyper.Integration.Contracts.IntegrationProvider.Basalam, "fixture", "987", (byte)IntegrationCredentialType.OAuth2));
            registryId = response?.Connection.Id ?? throw new VaultCheckFailure("Registry fixture creation failed.");
        }
        var registry = await fixture.ReadAsync(registryId);
        VaultCheck.That(!registry.IsEnabled && registry.CredentialsJson.StartsWith(IntegrationCredentialVault.ProtectedPrefix, StringComparison.Ordinal)
            && vault.Read(registry).Json == "{}", "registry commits a protected empty document");
        var first = await Authorize(fixture, oauth, vault, shop, "tenant-a", "987", "first-access");
        VaultCheck.That(first == registryId, "OAuth reuses registry connection without plaintext intermediate secret");
        var stored = await fixture.ReadAsync(first);
        var originalSecret = vault.Read(stored).WebhookSecret;
        VaultCheck.That(!string.IsNullOrWhiteSpace(originalSecret) && !stored.CredentialsJson.Contains(originalSecret, StringComparison.Ordinal),
            "OAuth writer persists only protected new webhook secret");
        var withFields = JsonSerializer.Serialize(new { webhookSecret = originalSecret, future = new { important = 41 }, webhookSignatureScheme = "hyper-hmac-v1" });
        await fixture.SetDocumentAsync(first, vault.Protect(stored, withFields));
        VaultCheck.That(await Authorize(fixture, oauth, vault, shop, "tenant-a", "987", "renewed-access") == first,
            "OAuth reconnect retains connection identity");
        stored = await fixture.ReadAsync(first);
        VaultCheck.That(vault.Read(stored).Json == withFields && vault.Read(stored).WebhookSecret == originalSecret,
            "OAuth reconnect retains same webhook secret and all unrelated fields");
        await using (var db = fixture.Open())
        {
            var token = await db.ExternalOAuthTokens.AsNoTracking().SingleAsync(t => t.ConnectionId == first);
            VaultCheck.That(token.RawTokenResponse is null && token.AccessToken != "renewed-access"
                && oauth.DecryptToken(token.AccessToken) == "renewed-access"
                && token.RefreshToken != "synthetic-refresh" && oauth.DecryptToken(token.RefreshToken!) == "synthetic-refresh",
                "OAuth tokens remain encrypted without raw token duplicate");
        }
        var freshId = await Authorize(fixture, oauth, vault, fixture.NextShop(), "tenant-a", "988", "new-access");
        VaultCheck.That(vault.Read(await fixture.ReadAsync(freshId)).WebhookSecret is { Length: > 0 },
            "OAuth creates protected credentials for a new connection directly");

        foreach (var invalid in new[] { "{", "{\"webhookSecret\":null}", "{\"webhookSecret\":\"\"}",
            "{\"webhookSecret\":\"a\",\"webhookSecret\":\"b\"}", "hyper-credentials:v1:corrupt", "hyper-credentials:v2:unknown" })
        {
            await fixture.SetDocumentAsync(first, invalid);
            await VaultCheck.RejectAsync(async () => await Authorize(fixture, oauth, FixtureProtection.LegacyVault(), shop,
                "tenant-a", "987", "must-not-replace"), "OAuth reconnect rejects invalid storage without secret rotation");
            VaultCheck.That((await fixture.ReadAsync(first)).CredentialsJson == invalid,
                "invalid reconnect leaves original credential bytes untouched");
            await using var db = fixture.Open();
            VaultCheck.That(oauth.DecryptToken(await db.ExternalOAuthTokens.Where(t => t.ConnectionId == first).Select(t => t.AccessToken).SingleAsync())
                == "renewed-access", "invalid reconnect cannot overwrite token");
        }
        var raw = "{\"webhookSecret\":\"stable-legacy-secret\",\"future\":42}";
        await fixture.SetDocumentAsync(first, raw);
        await Authorize(fixture, oauth, FixtureProtection.LegacyVault(), shop, "tenant-a", "987", "legacy-renewed");
        VaultCheck.That(vault.Read(await fixture.ReadAsync(first)).Json == raw,
            "explicit legacy reconnect protects exact existing document without rotation");
    }

    private static async Task<long> Authorize(VaultSqlFixture fixture, BasalamOAuthService oauth,
        IntegrationCredentialVault vault, int shop, string tenant, string vendor, string access)
    {
        await using var db = fixture.Open();
        var simulation = new IntegrationAdminSimulation
        {
            AdminUserId = "credential-fixture", ShopId = shop, TenantId = tenant, MerchantIdentifier = "fixture",
            ShopName = "fixture", ExpiresAtUtc = DateTime.UtcNow.AddMinutes(10)
        };
        var request = new IntegrationTokenRequest
        { SimulationId = simulation.Id, Provider = IntegrationProvider.Basalam, CredentialType = IntegrationCredentialType.OAuth2, Status = 1 };
        db.AddRange(simulation, request); await db.SaveChangesAsync();
        return await new BasalamOAuthStore(db, oauth, vault).SaveAsync(
            new(request.Id, simulation.Id, simulation.AdminUserId, "nonce", null, "http://localhost/callback"), simulation,
            new(vendor, "fixture"), new() { AccessToken = access, RefreshToken = "synthetic-refresh", ExpiresIn = 3600 }, default);
    }

    private static async Task Registration(VaultSqlFixture fixture, IntegrationCredentialVault vault)
    {
        const string secret = "original-provider-secret";
        var row = await fixture.SeedAsync(JsonSerializer.Serialize(new { webhookSecret = secret }));
        await using var db = fixture.Open();
        var migration = new IntegrationCredentialMigration(db, vault);
        var plan = await migration.PrepareAsync(new(row.ShopId, row.TenantId), row.Id, default);
        await migration.ApplyAsync(plan, default);
        using var transport = new RegistrationTransport();
        using var http = new HttpClient(transport);
        using var sdk = new BasalamClient(new BasalamConfig(), httpClient: http);
        var oauth = new BasalamOAuthService(Options.Create(new BasalamOAuthSettings()), http,
            new EphemeralDataProtectionProvider(), FixtureProtection.Provider());
        db.Add(oauth.CreateTokenEntity(row.Id, row.ShopId, row.TenantId, row.Provider,
            new() { AccessToken = "registration-access", RefreshToken = "registration-refresh", ExpiresIn = 3600 }));
        await db.SaveChangesAsync();
        var registration = new BasalamWebhookRegistration(db, new BasalamOAuthStore(db, oauth, vault), sdk, vault);
        await registration.RegisterForConnectionAsync(row.Id, row.ShopId, row.TenantId, row.AccountIdentifier, "https://fixture.invalid/callback", default);
        VaultCheck.That(transport.Calls == 1 && transport.Authorization == "Bearer registration-access"
            && transport.WebhookAuthorization == "Authorization: Bearer " + secret,
            "registration sends the original migrated secret with scoped grant exactly once");
        await db.ExternalOAuthTokens.Where(t => t.ConnectionId == row.Id).ExecuteUpdateAsync(s => s.SetProperty(t => t.ExpiresAtUtc, DateTime.UtcNow.AddMinutes(-10)));
        await fixture.SetDocumentAsync(row.Id, "hyper-credentials:v1:corrupt");
        await VaultCheck.RejectAsync(() => registration.RegisterForConnectionAsync(row.Id, row.ShopId, row.TenantId,
            row.AccountIdentifier, "https://fixture.invalid/callback", default), "invalid stored credentials reject before remote refresh or registration");
        VaultCheck.That(transport.Calls == 1, "invalid storage caused no provider request");
    }

    private sealed class RegistrationTransport : HttpMessageHandler
    {
        internal int Calls;
        internal string? Authorization;
        internal string? WebhookAuthorization;
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            Calls++;
            if (request.Method != HttpMethod.Post || request.RequestUri?.Host != "webhook.basalam.com"
                || request.RequestUri.AbsolutePath != "/v1/webhooks") throw new VaultCheckFailure("Unexpected provider request.");
            Authorization = request.Headers.Authorization?.ToString();
            using var json = JsonDocument.Parse(await request.Content!.ReadAsStringAsync(ct));
            WebhookAuthorization = json.RootElement.GetProperty("request_headers").GetString();
            return new(HttpStatusCode.OK) { Content = new StringContent("{}", Encoding.UTF8, "application/json") };
        }
    }
}
