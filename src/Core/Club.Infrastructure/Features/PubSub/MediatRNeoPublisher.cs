using Neo.Domain.Features.PubSub;
using MediatR;

namespace Hyper.Infrastructure.Features.PubSub;

public class MediatRNeoPublisher(IPublisher publisher) : INeoPublisher
{
    public async Task Publish<T>(T message, CancellationToken cancellationToken = default) where T : class
    {
        await publisher.Publish(message, cancellationToken);
    }
}