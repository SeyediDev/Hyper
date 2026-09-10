namespace Hyper.Application.Features.Channels;

public sealed record EnqueueEventMessage(
    string? IdempotencyKey,
    string? CorrelationId,

    int TenantId,
    int? ChannelId,
    int? EventTypeId,
    int? ProductCategoryId,
    int? ProductId,

    string CustomerMobile,
    string? ReferrerCode,
    AttributesValuesList? Attributes
    ) : IIdempotenceOutboxMessage, IRequest, IEventRequest
{
}

public sealed record ChannelConsumeAssetMessage(
    int TenantId,
    int ChannelId,
    string CustomerMobile,
    string Serial,
    string? CorrelationId,
    AttributesValuesList? Attributes
) : IOutboxMessage, IRequest;