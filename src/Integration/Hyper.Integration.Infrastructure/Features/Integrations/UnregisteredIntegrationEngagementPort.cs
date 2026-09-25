using Hyper.Integration.Domain.Features.Integrations;

namespace Hyper.Infrastructure.Features.Integrations;

/// <summary>Safe default until the owner API for CRM/subscription/chat is registered.</summary>
public sealed class UnregisteredIntegrationEngagementPort : IIntegrationEngagementPort
{
    public Task<EngagementCommandResult> ApplySubscriptionAsync(IntegrationSubscriptionCommand command, CancellationToken ct) => Pending();
    public Task<EngagementCommandResult> ApplyReviewAsync(IntegrationReviewCommand command, CancellationToken ct) => Pending();
    public Task<EngagementCommandResult> ApplyChatMessageAsync(IntegrationChatMessageCommand command, CancellationToken ct) => Pending();
    private static Task<EngagementCommandResult> Pending() => Task.FromResult(new EngagementCommandResult(EngagementCommandStatus.PendingDependency, "EngagementOwnerApiNotRegistered"));
}
