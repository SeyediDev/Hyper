using MediatR;
namespace Hyper.Infrastructure.Features.Integrations;

/// <summary>Neo/MediatR subscriber: publication completes only once the durable request has been saved.</summary>
public sealed class IntegrationScenarioSubscriber(IIntegrationScenarioQueue queue) : INotificationHandler<IntegrationScenarioRequested>
{
    public async Task Handle(IntegrationScenarioRequested notification, CancellationToken cancellationToken) =>
        _ = await queue.EnqueueAsync(notification.Shop, notification.ConnectionId, notification.Request, cancellationToken);
}
