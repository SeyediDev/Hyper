using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Hyper.Infrastructure.Features.Integrations;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

internal static class PureChecks
{
    private const string Secret = "vault-fixture-secret-0123456789-ABC";
    internal static ExternalIntegrationConnection Connection(long id = 41) => new()
    {
        Id = id, ShopId = 71, TenantId = "tenant-a", Provider = IntegrationProvider.Basalam,
        AccountIdentifier = "987", CredentialType = IntegrationCredentialType.OAuth2,
        DisplayName = "Credential fixture", CredentialsJson = "{}", IsEnabled = true
    };

    internal static void Run()
    {
        var keys = new EphemeralDataProtectionProvider();
        var strict = FixtureProtection.Vault(keys: keys);
        var legacy = FixtureProtection.Vault(true, keys);
        var row = Connection();
        var json = " {\"webhookSecret\":\"" + Secret + "\",\"webhookSignatureScheme\":\"hyper-hmac-v1\",\"privateApiToken\":\"unrelated-fixture-token\",\"future\":{\"values\":[1,true,\"فارسی\"]}} ";
        row.CredentialsJson = strict.Protect(row, json);
        var ciphertext = row.CredentialsJson;
        var decoded = strict.Read(row);
        VaultCheck.That(decoded.Json == json && decoded.WebhookSecret == Secret
            && decoded.WebhookSignatureScheme == "hyper-hmac-v1", "full credential document round trips exactly");
        VaultCheck.That(ciphertext.StartsWith(IntegrationCredentialVault.ProtectedPrefix, StringComparison.Ordinal)
            && !ciphertext.Contains(Secret, StringComparison.Ordinal)
            && !ciphertext.Contains("unrelated-fixture-token", StringComparison.Ordinal)
            && !ciphertext.Contains("webhookSecret", StringComparison.Ordinal), "stored envelope contains no plaintext credential fields");
        VaultCheck.That(decoded.ToString() == nameof(IntegrationCredentialDocument), "document diagnostics contain no secret");
        var largeUnicode = "{\"future\":\"" + new string('界', 21000) + "\"}";
        row.CredentialsJson = strict.Protect(row, largeUnicode);
        VaultCheck.That(strict.Read(row).Json == largeUnicode, "near-limit non-ASCII document remains readable after protection expansion");
        VaultCheck.Reject(() => strict.Protect(row, "{\"future\":\"" + new string('界', 65000) + "\"}"),
            "non-ASCII document exceeding UTF-8 byte limit rejects before protection");

        foreach (var mutate in new Action<ExternalIntegrationConnection>[]
        {
            c => c.Id++, c => c.ShopId++, c => c.TenantId = "TENANT-A",
            c => c.TenantId = "tenant-b", c => c.Provider = IntegrationProvider.Custom
        })
        {
            var wrong = Connection(); wrong.CredentialsJson = ciphertext; mutate(wrong);
            VaultCheck.Reject(() => strict.Read(wrong), "ciphertext rejects another exact scope");
            VaultCheck.Reject(() => legacy.Read(wrong), "legacy capability does not bypass protected scope");
        }
        foreach (var stored in new[]
        {
            "hyper-credentials:v2:" + ciphertext[IntegrationCredentialVault.ProtectedPrefix.Length..],
            "hyper-credentials:v1:", "hyper-credentials:v1:invalid",
            ciphertext[..^12] + new string('x', 12)
        })
        {
            row.CredentialsJson = stored;
            VaultCheck.Reject(() => strict.Read(row), "unsupported or tampered envelope rejected");
            VaultCheck.Reject(() => legacy.Read(row), "invalid envelope never falls back to legacy JSON");
        }
        row.CredentialsJson = ciphertext;
        VaultCheck.Reject(() => FixtureProtection.Vault(keys: new EphemeralDataProtectionProvider()).Read(row), "wrong key rejects stored ciphertext");
        row.CredentialsJson = json;
        VaultCheck.Reject(() => strict.Read(row), "strict runtime rejects plaintext credentials");
        VaultCheck.That(legacy.Read(row).Json == json, "explicit legacy compatibility preserves complete document");
        row.CredentialsJson = "{}";
        VaultCheck.Reject(() => strict.Read(row), "strict runtime rejects raw empty document too");
        VaultCheck.That(legacy.Read(row).WebhookSecret is null, "valid legacy empty document differs from malformed credentials");
        row.CredentialsJson = strict.Protect(row, "{}");
        VaultCheck.That(strict.Read(row).WebhookSecret is null, "protected empty document has no configured secret");
        foreach (var malformed in new[]
        {
            "", " ", "{", "[]", "null", "42", "\"text\"",
            "{\"webhookSecret\":null}", "{\"webhookSecret\":1}", "{\"webhookSecret\":\"\"}",
            "{\"webhookSecret\":\"  \"}", "{\"webhookSecret\":\"a\\nb\"}",
            "{\"webhookSecret\":\"a\",\"webhookSecret\":\"b\"}",
            "{\"webhookSignatureScheme\":null}", "{\"future\":1,\"future\":2}"
        })
        {
            row.CredentialsJson = malformed;
            VaultCheck.Reject(() => legacy.Read(row), "malformed credential fields are never treated as missing");
            VaultCheck.Reject(() => strict.Protect(row, malformed), "malformed document cannot be protected for storage");
        }
        var filled = legacy.Read(new ExternalIntegrationConnection
        {
            Id = row.Id, ShopId = row.ShopId, TenantId = row.TenantId, Provider = row.Provider,
            CredentialsJson = "{\"future\":{\"enabled\":true},\"webhookSignatureScheme\":\"hyper-hmac-v1\"}"
        }).WithWebhookSecret(Secret);
        using (var document = JsonDocument.Parse(filled))
            VaultCheck.That(document.RootElement.GetProperty("future").GetProperty("enabled").GetBoolean()
                && document.RootElement.GetProperty("webhookSecret").GetString() == Secret,
                "adding a genuinely absent secret retains unrelated fields");
        VerifyWebhooks(strict);
        VerifySharedRing();
        VerifyConfiguration();
        VerifyHostWriterPolicy();
        VaultCheck.That(typeof(IntegrationWebhookVerifier).GetConstructors().All(c => c.GetParameters().Length > 0)
            && typeof(BasalamOAuthService).GetConstructors().All(c => c.GetParameters().Any(p => p.ParameterType == typeof(IIntegrationProtectionProvider))),
            "runtime consumers require explicit protection composition");
    }

    private static void VerifyWebhooks(IntegrationCredentialVault vault)
    {
        var row = Connection();
        row.CredentialsJson = vault.Protect(row, JsonSerializer.Serialize(new { webhookSecret = Secret }));
        var verifier = new IntegrationWebhookVerifier(vault);
        var now = DateTimeOffset.UtcNow;
        var request = new IntegrationWebhookRequest([], "event", "VENDOR_NEW_ORDER", null, null, "Bearer " + Secret);
        VaultCheck.That(verifier.Verify(row, request, now) == WebhookValidationResult.Valid, "protected Basalam bearer authenticates with original secret");
        VaultCheck.That(verifier.Verify(row, request with { Authorization = "Bearer wrong" }, now) == WebhookValidationResult.Invalid,
            "wrong bearer cannot authenticate protected connection");
        row.Provider = IntegrationProvider.Custom;
        row.CredentialsJson = vault.Protect(row, JsonSerializer.Serialize(new { webhookSecret = Secret, webhookSignatureScheme = "hyper-hmac-v1" }));
        var timestamp = now.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
        var body = Encoding.UTF8.GetBytes("{\"fixture\":true}");
        var prefix = Encoding.UTF8.GetBytes($"hyper-hmac-v1\n{row.Id}\n{timestamp}\nevent\nproduct.updated\n");
        byte[] message = [.. prefix, .. body];
        var signature = Convert.ToHexString(HMACSHA256.HashData(Encoding.UTF8.GetBytes(Secret), message));
        request = new(body, "event", "product.updated", timestamp, signature);
        VaultCheck.That(verifier.Verify(row, request, now) == WebhookValidationResult.Valid, "protected Custom HMAC uses unchanged signature protocol");
        VaultCheck.That(verifier.Verify(row, request with { EventId = "changed" }, now) == WebhookValidationResult.Invalid,
            "protected Custom HMAC still binds event identity");
        row.CredentialsJson = "hyper-credentials:v1:corrupt";
        VaultCheck.That(verifier.Verify(row, request, now) == WebhookValidationResult.Invalid, "corrupt storage fails authentication without escaping exception");
    }

    private static void VerifySharedRing()
    {
        using var ring = new KeyRingFixture();
        using var otherRing = new KeyRingFixture();
        var firstKeys = ring.Provider();
        var oldToken = firstKeys.CreateProtector("Basalam.OAuth.Token").Protect("old-fixture-access");
        var first = FixtureProtection.Vault(keys: firstKeys);
        var row = Connection();
        row.CredentialsJson = first.Protect(row, "{\"webhookSecret\":\"shared-fixture-secret\"}");
        var independentKeys = ring.Provider();
        var second = FixtureProtection.Vault(keys: independentKeys);
        VaultCheck.That(second.Read(row).WebhookSecret == "shared-fixture-secret", "independent shared-ring application instance decrypts credentials");
        row.CredentialsJson = second.Protect(row, "{\"webhookSecret\":\"reverse-fixture-secret\"}");
        VaultCheck.That(first.Read(row).WebhookSecret == "reverse-fixture-secret", "shared-ring credentials are readable in both directions");
        VaultCheck.Reject(() => FixtureProtection.Vault(keys: ring.Provider("DifferentApp")).Read(row), "different application discriminator cannot decrypt");
        VaultCheck.Reject(() => FixtureProtection.Vault(keys: otherRing.Provider()).Read(row), "independent key ring cannot decrypt");
        using var http = new HttpClient(new NoNetwork());
        var hostKeys = new EphemeralDataProtectionProvider();
        var settings = Options.Create(new BasalamOAuthSettings
        {
            ClientId = "fixture-client", ClientSecret = "fixture-client-secret",
            RedirectUri = "http://localhost/api/auth/basalam/callback"
        });
        var oauth = new BasalamOAuthService(settings, http, hostKeys, FixtureProtection.Provider(independentKeys));
        VaultCheck.That(oauth.DecryptToken(oldToken) == "old-fixture-access", "existing Basalam.OAuth.Token purpose remains compatible");
        if (OperatingSystem.IsWindows())
        {
            var production = new IntegrationProtectionProvider(Options.Create(new IntegrationProtectionOptions
            { KeyRingPath = ring.DirectoryPath, ApplicationName = "Hyper.AdminPanel", KeyEncryption = "DpapiCurrentUser" }));
            var productionVault = new IntegrationCredentialVault(production, Options.Create(new IntegrationProtectionOptions()));
            var productionOauth = new BasalamOAuthService(settings, http, hostKeys, production);
            VaultCheck.That(productionOauth.DecryptToken(oldToken) == "old-fixture-access"
                && productionVault.Read(row).WebhookSecret == "reverse-fixture-secret",
                "production shared provider preserves old-format key ring token and credential compatibility");
        }
        var nonce = BasalamOAuthService.Nonce();
        var requestId = Guid.NewGuid();
        var url = oauth.CreateAuthorizationUrl(requestId, Guid.NewGuid(), "fixture-admin", nonce);
        var state = QueryHelpers.ParseQuery(new Uri(url).Query)["state"].ToString();
        var restarted = new BasalamOAuthService(settings, http, hostKeys, FixtureProtection.Provider(otherRing.Provider()));
        VaultCheck.That(restarted.ReadState(state, nonce).RequestId == requestId, "timed OAuth state stays on unchanged host provider when persistent provider changes");
        var hostProtector = hostKeys.CreateProtector("Hyper.Basalam.Authorization.v2").ToTimeLimitedDataProtector();
        VaultCheck.That(!string.IsNullOrEmpty(hostProtector.Unprotect(state)), "existing host timed-state purpose still reads emitted state");
    }

    private static void VerifyConfiguration()
    {
        using var ring = new KeyRingFixture();
        using var noNetwork = new HttpClient(new NoNetwork());
        var missingProvider = new IntegrationProtectionProvider(Options.Create(new IntegrationProtectionOptions()));
        var configuredOauth = new BasalamOAuthService(Options.Create(new BasalamOAuthSettings
        {
            ClientId = "fixture-client", ClientSecret = "fixture-client-secret",
            RedirectUri = "http://localhost/api/auth/basalam/callback"
        }), noNetwork, new EphemeralDataProtectionProvider(), missingProvider);
        var configurationError = configuredOauth.ConfigurationError();
        VaultCheck.That(configurationError is { Length: < 200 } && configurationError.Contains("IntegrationProtection", StringComparison.Ordinal)
            && !configurationError.Contains("fixture-client-secret", StringComparison.Ordinal),
            "valid OAuth settings fail early with bounded missing persistent protection configuration");
        foreach (var options in new[]
        {
            new IntegrationProtectionOptions(),
            new IntegrationProtectionOptions { KeyRingPath = "relative", ApplicationName = "app", KeyEncryption = "DpapiCurrentUser" },
            new IntegrationProtectionOptions { KeyRingPath = ring.DirectoryPath, ApplicationName = "app" },
            new IntegrationProtectionOptions { KeyRingPath = Path.Combine(ring.DirectoryPath, "not-created"), ApplicationName = "app", KeyEncryption = "DpapiCurrentUser" }
        })
            VaultCheck.Reject(() => new IntegrationProtectionProvider(Options.Create(options)).CreateProtector("fixture"),
                "missing or unsafe runtime protection configuration fails closed");
        if (OperatingSystem.IsWindows())
        {
            var options = Options.Create(new IntegrationProtectionOptions
            { KeyRingPath = ring.DirectoryPath, ApplicationName = "Hyper.AdminPanel", KeyEncryption = "DpapiCurrentUser" });
            var a = new IntegrationProtectionProvider(options);
            var b = new IntegrationProtectionProvider(options);
            var ciphertext = a.CreateProtector("fixture").Protect("fixture-value");
            VaultCheck.That(b.CreateProtector("fixture").Unprotect(ciphertext) == "fixture-value", "configured production providers share DPAPI-protected persistent keys");
            VaultCheck.That(Directory.EnumerateFiles(ring.DirectoryPath, "*.xml").All(path =>
                !File.ReadAllText(path).Contains("<masterKey", StringComparison.Ordinal)), "new production key files contain encrypted key material");
        }
    }

    private static void VerifyHostWriterPolicy()
    {
        if (!OperatingSystem.IsWindows()) return;
        using var ring = new KeyRingFixture();
        using var otherRing = new KeyRingFixture();
        var options = new IntegrationProtectionOptions
        { KeyRingPath = ring.DirectoryPath, ApplicationName = "DifferentIntegrationIdentity", KeyEncryption = "DpapiCurrentUser" };
        var services = new ServiceCollection();
        services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(ring.DirectoryPath))
            .SetApplicationName("OriginalHostIdentity").ApplySharedKeyRingEncryption(options, ring.DirectoryPath);
        using var provider = services.BuildServiceProvider();
        var host = provider.GetRequiredService<IDataProtectionProvider>();
        var token = host.CreateProtector("Basalam.OAuth.Token").Protect("host-token");
        var cookie = host.CreateProtector("fixture-cookie").Protect("host-cookie");
        var state = host.CreateProtector("Hyper.Basalam.Authorization.v2").ToTimeLimitedDataProtector()
            .Protect("host-state", DateTimeOffset.UtcNow.AddMinutes(10));
        VaultCheck.That(provider.GetRequiredService<IOptions<DataProtectionOptions>>().Value.ApplicationDiscriminator == "OriginalHostIdentity",
            "shared-key encryption hook preserves root application discriminator");
        var files = Directory.EnumerateFiles(ring.DirectoryPath, "*.xml").ToArray();
        VaultCheck.That(files.Length > 0 && files.All(path => !File.ReadAllText(path).Contains("<masterKey", StringComparison.Ordinal)),
            "host writer creates only encrypted key XML in its original shared ring");
        var sameIdentity = ring.Provider("OriginalHostIdentity");
        VaultCheck.That(sameIdentity.CreateProtector("Basalam.OAuth.Token").Unprotect(token) == "host-token"
            && sameIdentity.CreateProtector("fixture-cookie").Unprotect(cookie) == "host-cookie"
            && sameIdentity.CreateProtector("Hyper.Basalam.Authorization.v2").ToTimeLimitedDataProtector().Unprotect(state) == "host-state",
            "host token cookie and timed-state purposes remain compatible after key policy hook");
        var separate = new ServiceCollection();
        separate.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(otherRing.DirectoryPath))
            .SetApplicationName("SeparateHostIdentity").ApplySharedKeyRingEncryption(options, otherRing.DirectoryPath);
        using var separateProvider = separate.BuildServiceProvider();
        VaultCheck.That(separateProvider.GetRequiredService<IOptions<DataProtectionOptions>>().Value.ApplicationDiscriminator == "SeparateHostIdentity"
            && separateProvider.GetRequiredService<IOptions<KeyManagementOptions>>().Value.XmlEncryptor is null,
            "different key ring leaves unrelated host identity and key policy unchanged");
    }
}

internal sealed class NoNetwork : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
        throw new VaultCheckFailure("Unexpected HTTP request.");
}
