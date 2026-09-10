using System.Globalization;
using System.Net;
using Hyper.Application.Features.Channels;
using Hyper.Domain.Entities.Channels;
using Hyper.Domain.Features.Attributes;
using Neo.Application.Features.Outbox;
using Neo.Domain.Features.Client;
using Neo.Domain.Repository;
using ValidationException = FluentValidation.ValidationException;

namespace Hyper.Channel.Application.Features.Channel.Commands;

public sealed record EnqueueConsumeAssetCommand(
    int TenantId,
    int ChannelId,
    string CustomerMobile,
    string Serial,
    string? CorrelationId,
    string? RemoteIp,
    AttributesValuesList? Attributes
) : IRequest<OutboxResponse>;

internal sealed class EnqueueConsumeAssetCommandValidator : AbstractValidator<EnqueueConsumeAssetCommand>
{
    public EnqueueConsumeAssetCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .GreaterThan(0);

        RuleFor(x => x.ChannelId)
            .GreaterThan(0);

        RuleFor(x => x.CustomerMobile)
            .NotEmpty()
            .MaximumLength(15);

        RuleFor(x => x.Serial)
            .NotEmpty()
            .MaximumLength(128);
    }
}

internal sealed class EnqueueConsumeAssetCommandHandler(
    IQueryRepository<EventChannelValidIp, int> channelValidIpRepository,
    IOutboxMessageProcessor<ChannelConsumeAssetMessage> outboxProcessor,
    IRequesterUser requesterUser,
    ILogger<EnqueueConsumeAssetCommandHandler> logger
) : IRequestHandler<EnqueueConsumeAssetCommand, OutboxResponse>
{
    public async Task<OutboxResponse> Handle(EnqueueConsumeAssetCommand request, CancellationToken cancellationToken)
    {
        await ValidateChannelIpAsync(request.RemoteIp, request.ChannelId, cancellationToken);

        SetTenantContext(requesterUser, request.TenantId, logger);

        var message = new ChannelConsumeAssetMessage(
            request.TenantId,
            request.ChannelId,
            request.CustomerMobile,
            request.Serial,
            request.CorrelationId, request.Attributes);

        OutboxResponse response = await outboxProcessor.EnqueueNonIdempotentAsync(message, cancellationToken);
        logger.LogInformation("Consume asset message enqueued for channel {ChannelKey} (OutboxId: {OutboxId})", request.ChannelId, response.OutboxId);
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
        var tenantProperty = requesterUser.GetType().GetProperty("TenantId");
        if (tenantProperty?.CanWrite == true)
        {
            tenantProperty.SetValue(requesterUser, tenantValue);
            return;
        }

        requesterUser.SetProperty("TenantId", tenantValue);
        logger.LogDebug("Unable to set TenantId property on {RequesterUserType}. Stored fallback value in requester properties.",
            requesterUser.GetType().FullName);
    }
}

