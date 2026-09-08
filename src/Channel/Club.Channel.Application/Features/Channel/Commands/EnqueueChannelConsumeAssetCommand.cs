using System.Globalization;
using System.Net;
using Club.Domain.Entities.Events;
using Neo.Application.Features.Outbox;
using Neo.Domain.Features.Client;
using Neo.Domain.Repository;
using ValidationException = FluentValidation.ValidationException;

namespace Club.Channel.Application.Features.Channel.Commands;

public sealed record EnqueueChannelConsumeAssetCommand(
    int TenantId,
    string ChannelKey,
    string CustomerMobile,
    string Serial,
    string? CorrelationId,
    string? RemoteIp
) : IRequest<OutboxResponse>;

internal sealed class EnqueueChannelConsumeAssetCommandValidator : AbstractValidator<EnqueueChannelConsumeAssetCommand>
{
    public EnqueueChannelConsumeAssetCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .GreaterThan(0);

        RuleFor(x => x.ChannelKey)
            .NotEmpty()
            .MaximumLength(40);

        RuleFor(x => x.CustomerMobile)
            .NotEmpty()
            .MaximumLength(15);

        RuleFor(x => x.Serial)
            .NotEmpty()
            .MaximumLength(128);
    }
}

internal sealed class EnqueueChannelConsumeAssetCommandHandler(
    IQueryRepository<EventChannel, int> eventChannelRepository,
    IQueryRepository<EventChannelValidIp, int> channelValidIpRepository,
    IOutboxMessageProcessor<ChannelConsumeAssetMessage> outboxProcessor,
    IRequesterUser requesterUser,
    ILogger<EnqueueChannelConsumeAssetCommandHandler> logger
) : IRequestHandler<EnqueueChannelConsumeAssetCommand, OutboxResponse>
{
    public async Task<OutboxResponse> Handle(EnqueueChannelConsumeAssetCommand request, CancellationToken cancellationToken)
    {
        EventChannel eventChannel = await eventChannelRepository.FirstOrDefaultAsync(
            x => x.Key == request.ChannelKey && x.TenantId == request.TenantId,
            cancellationToken) ?? throw new ValidationException("کانال معتبر نیست");

        await ValidateChannelIpAsync(request.RemoteIp, eventChannel.Id, cancellationToken);

        SetTenantContext(requesterUser, request.TenantId, logger);

        var message = new ChannelConsumeAssetMessage(
            request.TenantId,
            request.ChannelKey,
            request.CustomerMobile,
            request.Serial,
            request.CorrelationId);

        OutboxResponse response = await outboxProcessor.EnqueueNonIdempotentAsync(message, cancellationToken);
        logger.LogInformation("Consume asset message enqueued for channel {ChannelKey} (OutboxId: {OutboxId})", request.ChannelKey, response.OutboxId);
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

