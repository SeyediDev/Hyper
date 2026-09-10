using Hyper.Application.Features.Channels;
using Hyper.Domain.Enums;
using Hyper.Domain.Features.Channels;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hyper.EventHandler.Application.Features.Channel;

internal sealed class HandleEnqueueEventMessage(
    IEventService eventService,
    ILogger<HandleEnqueueEventMessage> logger
    ) : IRequestHandler<EnqueueEventMessage>
{
    public async Task Handle(EnqueueEventMessage request, CancellationToken cancellationToken)
    {
        EventRequest eventRequest = new()
        {
            TenantId = request.TenantId,
            ChannelId = request.ChannelId,
            EventTypeId = request.EventTypeId,
            ProductCategoryId = request.ProductCategoryId,
            ProductId = request.ProductId,
            CustomerMobile = request.CustomerMobile,
            ReferrerCode = request.ReferrerCode,
            Attributes = request.Attributes,
            IsInquiry  = false,
            ReceiveEventType = ReceiveEventType.DynamicEvent,
        };

        EventResponse eventResponse = await eventService.ReceiveEventAsync(eventRequest, cancellationToken);

        if (eventResponse == null)
        {
            logger.LogWarning("Event service returned null response for channel {Channel} event {EventType}", request.ChannelId, request.EventTypeId);
            return;
        }
    }
}

