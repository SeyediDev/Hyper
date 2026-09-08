using System.Diagnostics;
using System.Net;
using Hyper.Application.Features.Channels;
using Hyper.Domain.Entities.Channels;
using Hyper.Domain.Entities.Events;
using Hyper.Domain.Entities.Products;
using Hyper.Domain.Features.Attributes;
using Hyper.Domain.Features.Channels;
using EventChannelValidEvent = Hyper.Domain.Entities.Channels.EventChannelValidEvent;
using EventChannelValidIp = Hyper.Domain.Entities.Channels.EventChannelValidIp;
using Hyper.Domain.Features.Products;
using Neo.Application.Features.Outbox;
using Neo.Domain.Features.Telementry;
using Neo.Domain.Repository;

namespace Hyper.Channel.Application.Features.Channel.Commands;

public sealed record EnqueueEventCommand(
    string? IdempotencyKey,
    string? CorrelationId,
    string? RemoteIp,
    string ClientId,
    bool IsInquiry,

    string EventTypeKey,
    string? ProductCategoryKey,
    string? ProductKey,

    string CustomerMobile,
    string? ReferrerCode,
    AttributesValuesList? Attributes
) : IRequest<OutboxResponse>;

internal sealed class EnqueueEventCommandValidator : AbstractValidator<EnqueueEventCommand>
{
    public EnqueueEventCommandValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty();

        RuleFor(x => x.EventTypeKey)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(x => x.CustomerMobile)
            .NotEmpty()
            .MaximumLength(15);
    }
}

internal sealed class EnqueueEventCommandHandler(
    IEventTypeService eventTypeService,
    IChannelService channelService,
    IProductService productService,
    IQueryRepository<EventChannelValidIp, int> channelValidIpRepository,
    IQueryRepository<EventChannelValidEvent, int> channelValidEventRepository,
    IOutboxMessageProcessor<EnqueueEventMessage> outboxProcessor,
    ITelementryObject telementry,
    ILogger<EnqueueEventCommandHandler> logger
) : IRequestHandler<EnqueueEventCommand, OutboxResponse>
{
    public async Task<OutboxResponse> Handle(EnqueueEventCommand request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var tags = GetTags("Hyper.Channel", "EnqueueEvent", request);
        using var activity = telementry.StartActivity(request, "Hyper.Channel", "EnqueueEvent", tags);
        var channelDto = await channelService.GetChannelByClientIdAsync(request.ClientId, cancellationToken);
        if (channelDto == null)
        {
            telementry.OnFinalize<EnqueueEventCommand, OutboxResponse>(request, null, "Hyper.Channel", "EnqueueEvent", activity,
                ActivityStatusCode.Error, "Channel not found from client id", stopwatch, tags);
            throw new ValidationException("توکن معتبر نیست");
        }

        var channelKey = channelDto.Key;
        activity?.SetTag("channel.key", channelKey);
        activity?.SetTag("tenant.id", channelDto.TenantId.ToString());

        await ValidateChannelIpAsync(request.RemoteIp, channelDto.Id, cancellationToken);

        EventType? eventType = await eventTypeService.GetEventTypeAsync(channelDto.TenantId, request.EventTypeKey, cancellationToken);
        if (eventType == null)
        {
            telementry.OnFinalize<EnqueueEventCommand, OutboxResponse>(request, null, "Hyper.Channel", "EnqueueEvent", activity,
                ActivityStatusCode.Error, $"Invalid Event Type {request.EventTypeKey}", stopwatch, tags);
            throw new ValidationException("رویداد معتبر نیست");
        }

        bool isEventAllowed = await channelValidEventRepository
            .AnyAsync(x => x.EventChannelId == channelDto.Id && x.EventTypeId == eventType.Id, cancellationToken);

        if (!isEventAllowed)
        {
            telementry.OnFinalize<EnqueueEventCommand, OutboxResponse>(request, null, "Hyper.Channel", "EnqueueEvent", activity,
                ActivityStatusCode.Error, $"رویداد برای این کانال مجاز نیست", stopwatch, tags);
            throw new ValidationException("رویداد برای این کانال مجاز نیست");
        }
        ProductCategory? productCategory = null;
        if (!string.IsNullOrEmpty(request.ProductCategoryKey))
        {
            productCategory = await productService.GetProductCategoryAsync(request.ProductCategoryKey, cancellationToken);
            if (productCategory == null)
            {
                logger.LogWarning("Can not found product cagerory {productCagerory}", request.ProductCategoryKey);
            }
        }
        Product? product = null;
        if (!string.IsNullOrEmpty(request.ProductKey))
        {
            product = await productService.GetAsync(request.ProductKey, cancellationToken);
            if (product == null)
            {
                logger.LogWarning("Can not found product cagerory {productCagerory}", request.ProductKey);
            }
        }

        var message = new EnqueueEventMessage(
            request.IdempotencyKey, request.CorrelationId,
            channelDto.TenantId, channelDto.Id, eventType.Id,
            productCategory?.Id, product?.Id,
            request.CustomerMobile, request.ReferrerCode,
            request.Attributes);

        OutboxResponse response =
            request.IdempotencyKey==null ? 
            await outboxProcessor.EnqueueNonIdempotentAsync(message, cancellationToken) :
            await outboxProcessor.EnqueueAsync(message, request.IdempotencyKey, cancellationToken);
        telementry.OnFinalize(request, response, "Hyper.Channel", "EnqueueEvent", activity,
            ActivityStatusCode.Ok, "Event message enqueued", stopwatch, tags);
        return response;
    }

    private async Task ValidateChannelIpAsync(string? remoteIp, int eventChannelId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(remoteIp))
        {
            return;
        }

        if (IPAddress.TryParse(remoteIp, out IPAddress? parsed))
        {
            remoteIp = parsed.MapToIPv4().ToString();
        }

        bool hasAnyIpRestrictions = await channelValidIpRepository
            .AnyAsync(
                x => x.EventChannelId == eventChannelId,
                cancellationToken);

        if (!hasAnyIpRestrictions)
        {
            return;
        }

        bool isIpAllowed = await channelValidIpRepository
            .AnyAsync(
                x => x.EventChannelId == eventChannelId && x.ValidIp == remoteIp,
                cancellationToken);

        if (!isIpAllowed)
        {
            throw new ValidationException("آدرس IP ورودی برای کانال معتبر نیست");
        }
    }

    private static KeyValuePair<string, object?>[] GetTags(string component, string serviceName,
        EnqueueEventCommand request)
    {
        Dictionary<string, object?> tagDict = new()
        {
            // بر اساس کلید یکتا اضافه می‌کنیم
            ["span.name"] = $"{component}.{serviceName}",
            ["client.id"] = request.ClientId,
            ["event.key"] = request.EventTypeKey,
            ["product.category.key"] = request.ProductCategoryKey,
            ["product.key"] = request.ProductKey,
            ["customer.mobile"] = request.CustomerMobile,
            ["referrer.code"] = request.ReferrerCode,
            ["remote.ip"] = request.RemoteIp,
            ["correlation.id"] = request.CorrelationId,
            ["idempotency.key"] = request.IdempotencyKey,
            ["is.inquiry"] = request.IsInquiry
        };

        // تبدیل دیکشنری به آرایه
        return [.. tagDict.Select(kv => new KeyValuePair<string, object?>(kv.Key, kv.Value))];
    }
}