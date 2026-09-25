namespace Hyper.Integration.Domain.Features.Integrations;

/// <summary>Engagement boundary. Subscription, review and chat are not accounting commands.</summary>
public interface IIntegrationEngagementPort
{
    Task<EngagementCommandResult> ApplySubscriptionAsync(IntegrationSubscriptionCommand command, CancellationToken ct);
    Task<EngagementCommandResult> ApplyReviewAsync(IntegrationReviewCommand command, CancellationToken ct);
    Task<EngagementCommandResult> ApplyChatMessageAsync(IntegrationChatMessageCommand command, CancellationToken ct);
}

public sealed record IntegrationSubscriptionCommand(string EventId, int ShopId, string TenantId,
    long ConnectionId, string ExternalSubscriptionId, string Status, DateTimeOffset? EffectiveAt);
public sealed record IntegrationReviewCommand(string EventId, int ShopId, string TenantId,
    long ConnectionId, string ExternalReviewId, string? ExternalOrderId, int Rating, string? Text);
public sealed record IntegrationChatMessageCommand(string EventId, int ShopId, string TenantId,
    long ConnectionId, string ExternalConversationId, string ExternalMessageId, bool Outbound, string? Text);
public enum EngagementCommandStatus : byte { Applied = 1, Duplicate = 2, PendingDependency = 3, Rejected = 4 }
public sealed record EngagementCommandResult(EngagementCommandStatus Status, string? ErrorCode = null);
