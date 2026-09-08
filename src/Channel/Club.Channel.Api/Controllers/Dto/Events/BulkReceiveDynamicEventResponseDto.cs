using Hyper.Channel.Application.Features.Channel.Commands;

namespace Hyper.Channel.Api.Controllers.Dto.Events;

public sealed record BulkReceiveDynamicEventResponseDto
{
    public List<OutboxResponseDto> Results { get; init; } = [];

    public BulkReceiveDynamicEventResponseDto(BulkOutboxResponse response)
    {
        Results = [.. response.Results.Select(r => new OutboxResponseDto(r))];
    }
}