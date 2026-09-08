using System.Globalization;
using System.Net;
using Club.Domain.Entities.Events;
using Neo.Application.Features.Outbox;
using Neo.Domain.Features.Client;
using Neo.Domain.Repository;
using ValidationException = FluentValidation.ValidationException;

namespace Club.Channel.Application.Features.Channel.Commands;

public sealed record EnqueueChannelEventCommand(
    int TenantId,
    string ChannelKey,
    string EventTypeKey,
    string CustomerMobile,
    string? CorrelationId,
    IReadOnlyDictionary<string, string>? Parameters,
    string? ProductCategoryKey,
    string? ProductKey,
    string? RemoteIp
) : IRequest<OutboxResponse>;

internal sealed class EnqueueChannelEventCommandValidator : AbstractValidator<EnqueueChannelEventCommand>
{
    public EnqueueChannelEventCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .GreaterThan(0);

        RuleFor(x => x.ChannelKey)
            .NotEmpty()
            .MaximumLength(40);

        RuleFor(x => x.EventTypeKey)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(x => x.CustomerMobile)
            .NotEmpty()
            .MaximumLength(15);
    }
}

internal sealed class EnqueueChannelEventCommandHandler(
    IQueryRepository<EventChannel, int> eventChannelRepository,
    IQueryRepository<EventChannelValidIp, int> channelValidIpRepository,
    IQueryRepository<EventType, int> eventTypeRepository,
    IQueryRepository<EventChannelValidEvent, int> channelValidEventRepository,
    IOutboxMessageProcessor<ChannelAddEventMessage> outboxProcessor,
    IRequesterUser requesterUser,
    ILogger<EnqueueChannelEventCommandHandler> logger
) : IRequestHandler<EnqueueChannelEventCommand, OutboxResponse>
{
    public async Task<OutboxResponse> Handle(EnqueueChannelEventCommand request, CancellationToken cancellationToken)
    {
        EventChannel eventChannel = await eventChannelRepository.FirstOrDefaultAsync(
            x => x.Key == request.ChannelKey && x.TenantId == request.TenantId,
            cancellationToken) ?? throw new ValidationException("کانال معتبر نیست");

        await ValidateChannelIpAsync(request.RemoteIp, eventChannel.Id, cancellationToken);

        EventType eventType = await eventTypeRepository.FirstOrDefaultAsync(
            x => x.Key == request.EventTypeKey && x.TenantId == request.TenantId,
            cancellationToken) ?? throw new ValidationException("رویداد معتبر نیست");

        bool isEventAllowed = await channelValidEventRepository
            .AnyAsync(
                x => x.EventChannelId == eventChannel.Id && x.EventTypeId == eventType.Id,
                cancellationToken);

        if (!isEventAllowed)
        {
            throw new ValidationException("رویداد برای این کانال مجاز نیست");
        }

        SetTenantContext(requesterUser, request.TenantId, logger);

        var message = new ChannelAddEventMessage(
            request.TenantId,
            request.ChannelKey,
            request.EventTypeKey,
            request.CustomerMobile,
            request.Parameters,
            request.ProductCategoryKey,
            request.ProductKey,
            request.CorrelationId,
            null); // IdempotencyKey will be handled separately if needed

        OutboxResponse response = await outboxProcessor.EnqueueNonIdempotentAsync(message, cancellationToken);
        logger.LogInformation("Event message enqueued for channel {ChannelKey} (OutboxId: {OutboxId})", request.ChannelKey, response.OutboxId);
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

    private static void SetTenantContext(IRequesterUser requesterUser, int tenantId, ILogger logger)
    {
        if (requesterUser is null)
        {
            return;
        }

        string tenantValue = tenantId.ToString(CultureInfo.InvariantCulture);
        var tenantProperty = requesterUser.GetType().GetProperty(nameof(requesterUser.TenantId));
        if (tenantProperty?.CanWrite == true)
        {
            tenantProperty.SetValue(requesterUser, tenantValue);
            return;
        }

        requesterUser.SetProperty(nameof(requesterUser.TenantId), tenantValue);
        logger.LogDebug("Unable to set TenantId property on {RequesterUserType}. Stored fallback value in requester properties.",
            requesterUser.GetType().FullName);
    }
}

