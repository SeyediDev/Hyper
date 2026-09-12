namespace Hyper.Domain.Features.Integrations;

public sealed record IntegrationWebhookRequest(
    byte[] Body, string EventId, string EventType, string? Timestamp, string? Signature);
public enum WebhookValidationResult { Valid, Invalid, Unsupported }
public interface IIntegrationWebhookVerifier
{
    WebhookValidationResult Verify(ExternalIntegrationConnection connection,
        IntegrationWebhookRequest request, DateTimeOffset utcNow);
}
