using Hyper.SDK.Auth;
using Hyper.SDK.Errors;

namespace Hyper.SDK.Auth;

public sealed class WebhookSignatureVerifier
{
    private readonly byte[] _secret;
    private readonly string _scheme;
    private static readonly TimeSpan ClockSkew = TimeSpan.FromMinutes(5);

    public WebhookSignatureVerifier(string secret, string scheme = "hyper-hmac-v1")
    {
        if (string.IsNullOrEmpty(secret))
            throw new ArgumentException("Webhook secret cannot be empty", nameof(secret));
        if (System.Text.Encoding.UTF8.GetByteCount(secret) < 32)
            throw new ArgumentException("Webhook secret must be at least 32 bytes UTF-8", nameof(secret));

        _secret = System.Text.Encoding.UTF8.GetBytes(secret);
        _scheme = scheme;
    }

    public bool Verify(
        string signature,
        string timestamp,
        string eventId,
        string eventType,
        string body,
        out string? errorMessage)
    {
        errorMessage = null;

        if (string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(timestamp) || string.IsNullOrEmpty(eventId) || string.IsNullOrEmpty(eventType))
        {
            errorMessage = "Missing required headers";
            return false;
        }

        if (!long.TryParse(timestamp, out var timestampLong))
        {
            errorMessage = "Invalid timestamp format";
            return false;
        }

        var eventTime = DateTimeOffset.FromUnixTimeSeconds(timestampLong).UtcDateTime;
        if (DateTime.UtcNow - eventTime > ClockSkew)
        {
            errorMessage = "Timestamp outside acceptable window";
            return false;
        }

        var parts = new[] { _scheme, "", timestamp, eventId, eventType };
        var signingString = string.Join("\n", parts) + "\n" + body;

        using var hmac = new System.Security.Cryptography.HMACSHA256(_secret);
        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(signingString));
        var computedSignature = BitConverter.ToString(computedHash).Replace("-", "").ToLowerInvariant();

        if (!fixedTimeEquals(computedSignature, signature))
        {
            errorMessage = "Signature mismatch";
            return false;
        }

        return true;
    }

    public string ComputeSignature(string timestamp, string eventId, string eventType, string body)
    {
        var parts = new[] { _scheme, "", timestamp, eventId, eventType };
        var signingString = string.Join("\n", parts) + "\n" + body;

        using var hmac = new System.Security.Cryptography.HMACSHA256(_secret);
        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(signingString));
        return BitConverter.ToString(computedHash).Replace("-", "").ToLowerInvariant();
    }

    private static bool fixedTimeEquals(string a, string b)
    {
        if (a.Length != b.Length)
            return false;
        var result = 0;
        for (var i = 0; i < a.Length; i++)
            result |= a[i] ^ b[i];
        return result == 0;
    }
}
