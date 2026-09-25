namespace Hyper.Integration.Domain.Features.Integrations;

public sealed record IntegrationWebhookRequest(
    byte[] Body, string EventId, string EventType, string? Timestamp, string? Signature,
    string? Authorization = null);
public enum WebhookValidationResult { Valid, Invalid, Unsupported }
public interface IIntegrationWebhookVerifier
{
    WebhookValidationResult Verify(ExternalIntegrationConnection connection,
        IntegrationWebhookRequest request, DateTimeOffset utcNow);
}

