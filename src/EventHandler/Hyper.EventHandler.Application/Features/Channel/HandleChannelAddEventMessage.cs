using Hyper.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hyper.EventHandler.Application.Features.Channel;

internal sealed class HandleChannelAddEventMessage(
    IEventService eventService,
    IPromotionService promotionService,
    ILogger<HandleChannelAddEventMessage> logger
) : IRequestHandler<ChannelAddEventMessage>
{
    public async Task Handle(ChannelAddEventMessage request, CancellationToken cancellationToken)
    {
        Dictionary<string, string>? parameters = request.Parameters?.ToDictionary(kv => kv.Key, kv => kv.Value);

        EventRequest eventRequest = new(
            TriggerType.Event,
            request.CustomerMobile,
            parameters)
        {
            TenantId = request.TenantId,
            EventChannel = request.ChannelKey,
            EventType = request.EventTypeKey,
            ProductCategoryKey = request.ProductCategoryKey,
            ProductKey = request.ProductKey
        };

        EventResponse eventResponse = await eventService.RecordEventAsync(eventRequest, cancellationToken);

        if (eventResponse == null)
        {
            logger.LogWarning("Event service returned null response for channel {Channel} event {EventType}", request.ChannelKey, request.EventTypeKey);
            return;
        }

        await promotionService.ProcessEventAsync(
            new PromotionProcessingRequest(
                request.TenantId,
                (int)eventResponse.EventLogId,
                eventResponse.EventChannelId ?? 0,
                eventResponse.EventTypeId ?? 0,
                eventResponse.Customer,
                parameters)
            {
                TriggerType = TriggerType.Event
            },
            cancellationToken);
    }
}

