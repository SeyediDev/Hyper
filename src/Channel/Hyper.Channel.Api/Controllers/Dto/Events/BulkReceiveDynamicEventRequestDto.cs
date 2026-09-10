namespace Hyper.Channel.Api.Controllers.Dto.Events;

public sealed record BulkReceiveDynamicEventRequestDto
{
    [Required]
    [MinLength(1)]
    public List<ReceiveDynamicEventRequestDto> Events { get; init; } = null!;
}