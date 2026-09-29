using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed class IntegrationCredentialVault(IIntegrationProtectionProvider protection,
    IOptions<IntegrationProtectionOptions> options)
{
    public const string ReservedPrefix = "hyper-credentials:";
    public const string ProtectedPrefix = ReservedPrefix + "v1:";
    private const int MaxDocumentLength = 65536;

    public IntegrationCredentialDocument Read(ExternalIntegrationConnection connection)
    {
        ValidateScope(connection);
        var stored = connection.CredentialsJson;
        if (stored is null || stored.Length > 131072)
            throw Failure("IntegrationCredentialsInvalid");
        if (stored.StartsWith(ReservedPrefix, StringComparison.Ordinal))
        {
            if (!stored.StartsWith(ProtectedPrefix, StringComparison.Ordinal)
                || stored.Length == ProtectedPrefix.Length)
                throw Failure("IntegrationCredentialsVersionUnsupported");
            try { return Parse(Protector(connection).Unprotect(stored[ProtectedPrefix.Length..])); }
            catch (CryptographicException) { throw Failure("IntegrationCredentialsUnreadable"); }
            catch (FormatException) { throw Failure("IntegrationCredentialsUnreadable"); }
        }
        if (!options.Value.AllowLegacyPlaintextRead)
            throw Failure("IntegrationCredentialsLegacyDisabled");
        return Parse(stored);
    }

    public string Protect(ExternalIntegrationConnection connection, string json)
    {
        ValidateScope(connection);
        Parse(json); // Validate before storing; retain the complete exact document.
        try { return ProtectedPrefix + Protector(connection).Protect(json); }
        catch (CryptographicException) { throw Failure("IntegrationCredentialsProtectionFailed"); }
    }

    // This is only used by the explicitly invoked migration service, never by
    // an authentication reader. Protected/unknown prefixes cannot become legacy.
    internal string ProtectLegacy(ExternalIntegrationConnection connection)
    {
        if (connection.CredentialsJson?.StartsWith(ReservedPrefix, StringComparison.Ordinal) != false)
            throw Failure("IntegrationCredentialsInvalid");
        return Protect(connection, connection.CredentialsJson);
    }

    private IDataProtector Protector(ExternalIntegrationConnection connection) => protection
        .CreateProtector("Hyper.Integration.ConnectionCredentials.v1")
        .CreateProtector(connection.Id.ToString(CultureInfo.InvariantCulture))
        .CreateProtector(connection.ShopId.ToString(CultureInfo.InvariantCulture))
        .CreateProtector(connection.TenantId)
        .CreateProtector(((byte)connection.Provider).ToString(CultureInfo.InvariantCulture));

    private static void ValidateScope(ExternalIntegrationConnection connection)
    {
        if (connection.Id <= 0 || connection.ShopId <= 0 || !Enum.IsDefined(connection.Provider)
            || string.IsNullOrWhiteSpace(connection.TenantId) || connection.TenantId.Length > 30
            || connection.TenantId != connection.TenantId.Trim() || connection.TenantId.Any(char.IsControl))
            throw Failure("IntegrationCredentialsScopeInvalid");
    }

    private static IntegrationCredentialDocument Parse(string json)
    {
        if (string.IsNullOrWhiteSpace(json) || json.Length > MaxDocumentLength
            || Encoding.UTF8.GetByteCount(json) > MaxDocumentLength)
            throw Failure("IntegrationCredentialsInvalid");
        try
        {
            using var document = JsonDocument.Parse(json);
            if (document.RootElement.ValueKind != JsonValueKind.Object)
                throw Failure("IntegrationCredentialsInvalid");
            var names = new HashSet<string>(StringComparer.Ordinal);
            foreach (var property in document.RootElement.EnumerateObject())
                if (!names.Add(property.Name)) throw Failure("IntegrationCredentialsInvalid");
            string? Field(string name, int limit)
            {
                if (!document.RootElement.TryGetProperty(name, out var value)) return null;
                if (value.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(value.GetString())
                    || value.GetString()!.Length > limit || value.GetString()!.Any(char.IsControl))
                    throw Failure("IntegrationCredentialsInvalid");
                return value.GetString();
            }
            return new(json, Field("webhookSecret", 4096), Field("webhookSignatureScheme", 128));
        }
        catch (JsonException) { throw Failure("IntegrationCredentialsInvalid"); }
    }

    private static IntegrationCredentialException Failure(string code) => new(code);
}

// A class deliberately avoids record-generated ToString including secret values.
// This internal application boundary is never returned by an HTTP response DTO.
public sealed class IntegrationCredentialDocument
{
    internal IntegrationCredentialDocument(string json, string? webhookSecret, string? webhookSignatureScheme)
    { Json = json; WebhookSecret = webhookSecret; WebhookSignatureScheme = webhookSignatureScheme; }
    public string Json { get; }
    public string? WebhookSecret { get; }
    public string? WebhookSignatureScheme { get; }

    public string WithWebhookSecret(string secret)
    {
        var document = JsonNode.Parse(Json)!.AsObject();
        document["webhookSecret"] = secret;
        return document.ToJsonString();
    }

    public override string ToString() => nameof(IntegrationCredentialDocument);
}
