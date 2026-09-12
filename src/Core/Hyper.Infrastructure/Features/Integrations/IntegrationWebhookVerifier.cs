using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Hyper.Infrastructure.Features.Integrations;

/// <summary>Hyper's explicit custom webhook protocol. Not a Basalam/Digikala/Torob protocol.</summary>
public sealed class IntegrationWebhookVerifier : IIntegrationWebhookVerifier
{
    public const string Scheme = "hyper-hmac-v1";
    public WebhookValidationResult Verify(ExternalIntegrationConnection connection,
        IntegrationWebhookRequest request, DateTimeOffset utcNow)
    {
        if (connection.Provider != IntegrationProvider.Custom)
            return WebhookValidationResult.Unsupported;
        if (!connection.IsEnabled || connection.Id <= 0 || !ValidHeader(request.EventId)
            || !ValidHeader(request.EventType) || request.Body.Length > 1024 * 1024)
            return WebhookValidationResult.Invalid;
        if (request.Timestamp is null || !long.TryParse(request.Timestamp, NumberStyles.None,
            CultureInfo.InvariantCulture, out var timestamp)) return WebhookValidationResult.Invalid;
        var now = utcNow.ToUnixTimeSeconds();
        if (timestamp < now - 300 || timestamp > now + 300) return WebhookValidationResult.Invalid;
        if (request.Signature is null || request.Signature.Length != 64) return WebhookValidationResult.Invalid;
        byte[] signature;
        try { signature = Convert.FromHexString(request.Signature); }
        catch (FormatException) { return WebhookValidationResult.Invalid; }
        try
        {
            using var json = JsonDocument.Parse(connection.CredentialsJson);
            if (json.RootElement.ValueKind != JsonValueKind.Object
                || !json.RootElement.TryGetProperty("webhookSignatureScheme", out var scheme)
                || scheme.ValueKind != JsonValueKind.String || scheme.GetString() != Scheme)
                return WebhookValidationResult.Unsupported;
            if (!json.RootElement.TryGetProperty("webhookSecret", out var secret)
                || secret.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(secret.GetString()))
                return WebhookValidationResult.Invalid;
            var key = Encoding.UTF8.GetBytes(secret.GetString()!);
            if (key.Length < 32) return WebhookValidationResult.Invalid;
            try
            {
                using var hmac = IncrementalHash.CreateHMAC(HashAlgorithmName.SHA256, key);
                // Bound route, timestamp and event identity prevent replay under a different ID/connection.
                var prefix = $"{Scheme}\n{connection.Id.ToString(CultureInfo.InvariantCulture)}\n{request.Timestamp}\n{request.EventId}\n{request.EventType}\n";
                hmac.AppendData(Encoding.UTF8.GetBytes(prefix));
                hmac.AppendData(request.Body);
                return CryptographicOperations.FixedTimeEquals(hmac.GetHashAndReset(), signature)
                    ? WebhookValidationResult.Valid : WebhookValidationResult.Invalid;
            }
            finally { CryptographicOperations.ZeroMemory(key); }
        }
        catch (JsonException) { return WebhookValidationResult.Invalid; }
    }
    private static bool ValidHeader(string value) => !string.IsNullOrWhiteSpace(value)
        && value.Length <= 200 && !value.Any(char.IsControl);
}
