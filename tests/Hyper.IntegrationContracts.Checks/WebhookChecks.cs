using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Hyper.Domain.Entities.Integrations;
using Hyper.Domain.Features.Integrations;
using Hyper.Infrastructure.Features.Integrations;

internal static class WebhookChecks
{
    public static int Run()
    {
        const string secret = "test-secret-012345678901234567890123456789";
        var now = DateTimeOffset.FromUnixTimeSeconds(1788998400);
        var verifier = new IntegrationWebhookVerifier();
        ExternalIntegrationConnection Connection() => new()
        {
            Id = 42, Provider = IntegrationProvider.Custom, IsEnabled = true,
            CredentialsJson = JsonSerializer.Serialize(new { webhookSignatureScheme = "hyper-hmac-v1", webhookSecret = secret })
        };
        // Sender-side construction: concatenate the published wire format, then sign once.
        IntegrationWebhookRequest Signed(long timestamp, byte[]? body = null)
        {
            body ??= Encoding.UTF8.GetBytes("{ \"message\": \"سلام\" }\n");
            var stamp = timestamp.ToString(CultureInfo.InvariantCulture);
            var wire = Encoding.UTF8.GetBytes("hyper-hmac-v1\n42\n" + stamp + "\nevent-1\norder.created\n").Concat(body).ToArray();
            return new(body, "event-1", "order.created", stamp,
                Convert.ToHexString(HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), wire)));
        }
        var valid = Signed(now.ToUnixTimeSeconds());
        var count = 0;
        void Check(string name, IntegrationWebhookRequest request, WebhookValidationResult expected = WebhookValidationResult.Invalid,
            Action<ExternalIntegrationConnection>? configure = null)
        {
            var connection = Connection();
            configure?.Invoke(connection);
            var actual = verifier.Verify(connection, request, now);
            if (actual != expected) throw new Exception($"{name}: expected {expected}, got {actual}");
            count++;
            Console.WriteLine($"PASS webhook {name}");
        }
        Check("raw UTF8 and whitespace accepted", valid, WebhookValidationResult.Valid);
        Check("identical redelivery authenticates for inbox deduplication", valid, WebhookValidationResult.Valid);
        Check("body whitespace tampering rejected", valid with { Body = Encoding.UTF8.GetBytes("{\"message\":\"سلام\"}") });
        Check("timestamp tampering inside window rejected", valid with { Timestamp = (now.ToUnixTimeSeconds() + 1).ToString(CultureInfo.InvariantCulture) });
        Check("event id tampering rejected", valid with { EventId = "event-2" });
        Check("event type tampering rejected", valid with { EventType = "order.cancelled" });
        Check("route tampering rejected", valid, configure: c => c.Id++);
        Check("missing timestamp rejected", valid with { Timestamp = null });
        Check("old signed timestamp rejected", Signed(now.ToUnixTimeSeconds() - 301));
        Check("future signed timestamp rejected", Signed(now.ToUnixTimeSeconds() + 301));
        Check("past window boundary accepted", Signed(now.ToUnixTimeSeconds() - 300), WebhookValidationResult.Valid);
        Check("future window boundary accepted", Signed(now.ToUnixTimeSeconds() + 300), WebhookValidationResult.Valid);
        Check("maximum timestamp rejected", valid with { Timestamp = long.MaxValue.ToString(CultureInfo.InvariantCulture) });
        Check("negative timestamp rejected", valid with { Timestamp = "-1" });
        Check("timestamp whitespace rejected", valid with { Timestamp = " " + valid.Timestamp });
        Check("missing signature rejected", valid with { Signature = null });
        Check("invalid signature hex rejected", valid with { Signature = new string('z', 64) });
        Check("short signature rejected", valid with { Signature = "abcd" });
        Check("lowercase signature accepted", valid with { Signature = valid.Signature!.ToLowerInvariant() }, WebhookValidationResult.Valid);
        Check("blank event rejected", valid with { EventId = " " });
        Check("header delimiter injection rejected", valid with { EventType = "order.created\nevent-2" });
        Check("oversized event id rejected", valid with { EventId = new string('a', 201) });
        Check("oversized body rejected", Signed(now.ToUnixTimeSeconds(), new byte[1024 * 1024 + 1]));
        Check("disabled connection rejected", valid, configure: c => c.IsEnabled = false);
        Check("missing secret rejected", valid, configure: c => c.CredentialsJson = "{\"webhookSignatureScheme\":\"hyper-hmac-v1\"}");
        Check("short secret rejected", valid, configure: c => c.CredentialsJson = "{\"webhookSignatureScheme\":\"hyper-hmac-v1\",\"webhookSecret\":\"short\"}");
        Check("nonstring secret rejected", valid, configure: c => c.CredentialsJson = "{\"webhookSignatureScheme\":\"hyper-hmac-v1\",\"webhookSecret\":123}");
        Check("invalid credential JSON rejected", valid, configure: c => c.CredentialsJson = "{");
        Check("missing scheme unsupported", valid, WebhookValidationResult.Unsupported, c => c.CredentialsJson = "{}");
        foreach (var provider in new[] { IntegrationProvider.Basalam, IntegrationProvider.Digikala, IntegrationProvider.Torob })
            Check($"{provider} cannot use invented custom protocol", valid, WebhookValidationResult.Unsupported, c => c.Provider = provider);
        return count;
    }
}
