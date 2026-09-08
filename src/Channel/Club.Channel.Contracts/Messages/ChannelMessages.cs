using MediatR;
using Neo.Application.Features.Outbox.Dto;

namespace Club.Channel.Contracts.Messages;

public sealed record ChannelAddEventMessage(
    int TenantId,
    string ChannelKey,
    string EventTypeKey,
    string CustomerMobile,
    IReadOnlyDictionary<string, string>? Parameters,
    string? ProductCategoryKey,
    string? ProductKey,
    string? CorrelationId,
    string? IdempotencyKey
) : IOutboxMessage, IRequest;

public sealed record ChannelConsumeAssetMessage(
    int TenantId,
    string ChannelKey,
    string CustomerMobile,
    string Serial,
    string? CorrelationId
) : IOutboxMessage, IRequest;


