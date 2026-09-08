using Club.Domain.Entities.Events;
using Neo.Application.Features.Outbox;
using Neo.Domain.Repository;
using ValidationException = FluentValidation.ValidationException;

namespace Club.Channel.Application.Features.Channel.Commands;

/// <summary>
/// دستور ارسال چندین رویداد به صورت bulk
/// </summary>
public sealed record EnqueueBulkChannelEventsCommand(
    string ChannelKey,
    IReadOnlyList<ChannelAddEventRequestDto> Events,
    string? CorrelationId,
    string? RemoteIp
) : IRequest<BulkOutboxResponse>;

internal sealed class EnqueueBulkChannelEventsCommandValidator : AbstractValidator<EnqueueBulkChannelEventsCommand>
{
    public EnqueueBulkChannelEventsCommandValidator()
    {
        RuleFor(x => x.ChannelKey)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(x => x.Events)
            .NotEmpty()
            .Must(events => events.Count <= 100) // محدودیت تعداد رویدادها
            .WithMessage("تعداد رویدادها نمی‌تواند بیشتر از 100 باشد");

        RuleForEach(x => x.Events)
            .SetValidator(new ChannelAddEventRequestDtoValidator());
    }
}

public sealed record ChannelAddEventRequestDto
{
    public required string CustomerMobile { get; init; }
    public required string EventTypeKey { get; init; }
    public IReadOnlyDictionary<string, string>? Parameters { get; init; }
    public string? ProductCategoryKey { get; init; }
    public string? ProductKey { get; init; }
    public string? IdempotencyKey { get; init; }
}

internal sealed class ChannelAddEventRequestDtoValidator : AbstractValidator<ChannelAddEventRequestDto>
{
    public ChannelAddEventRequestDtoValidator()
    {
        RuleFor(x => x.CustomerMobile)
            .NotEmpty()
            .MaximumLength(15)
            .Matches(@"^09\d{9}$")
            .WithMessage("شماره موبایل باید با 09 شروع شود و 11 رقم باشد");

        RuleFor(x => x.EventTypeKey)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(x => x.ProductCategoryKey)
            .MaximumLength(128)
            .When(x => !string.IsNullOrWhiteSpace(x.ProductCategoryKey));

        RuleFor(x => x.ProductKey)
            .MaximumLength(128)
            .When(x => !string.IsNullOrWhiteSpace(x.ProductKey));

        RuleFor(x => x.IdempotencyKey)
            .MaximumLength(128)
            .When(x => !string.IsNullOrWhiteSpace(x.IdempotencyKey));
    }
}

public sealed class EnqueueBulkChannelEventsCommandHandler(
    IQueryRepository<EventChannel, int> eventChannelRepository,
    IQueryRepository<EventType, int> eventTypeRepository,
    IQueryRepository<EventChannelValidEvent, int> channelValidEventRepository,
    IOutboxMessageProcessor<ChannelAddEventMessage> outboxProcessor,
    ILogger<EnqueueBulkChannelEventsCommandHandler> logger
) : IRequestHandler<EnqueueBulkChannelEventsCommand, BulkOutboxResponse>
{
    public async Task<BulkOutboxResponse> Handle(EnqueueBulkChannelEventsCommand request, CancellationToken cancellationToken)
    {
        // Validate channel
        var eventChannel = await eventChannelRepository.FirstOrDefaultAsync(
            x => x.Key == request.ChannelKey, cancellationToken) 
            ?? throw new ValidationException("کانال معتبر نیست");

        var results = new List<OutboxResponse>();

        foreach (var eventDto in request.Events)
        {
            try
            {
                // Validate event type
                var eventType = await eventTypeRepository.FirstOrDefaultAsync(
                    x => x.Key == eventDto.EventTypeKey && x.TenantId == eventChannel.TenantId,
                    cancellationToken) ?? throw new ValidationException($"رویداد {eventDto.EventTypeKey} معتبر نیست");

                // Validate event is allowed for channel
                bool isEventAllowed = await channelValidEventRepository
                    .AnyAsync(
                        x => x.EventChannelId == eventChannel.Id && x.EventTypeId == eventType.Id,
                        cancellationToken);

                if (!isEventAllowed)
                {
                    throw new ValidationException($"رویداد {eventDto.EventTypeKey} برای این کانال مجاز نیست");
                }

                var message = new ChannelAddEventMessage(
					eventChannel.TenantId,
                    request.ChannelKey,
                    eventDto.EventTypeKey,
                    eventDto.CustomerMobile,
                    eventDto.Parameters,
                    eventDto.ProductCategoryKey,
                    eventDto.ProductKey,
                    request.CorrelationId,
                    eventDto.IdempotencyKey);

                OutboxResponse response;
                if (!string.IsNullOrWhiteSpace(eventDto.IdempotencyKey))
                {
                    response = await outboxProcessor.EnqueueAsync(message, eventDto.IdempotencyKey, cancellationToken);
                }
                else
                {
                    response = await outboxProcessor.EnqueueNonIdempotentAsync(message, cancellationToken);
                }

                results.Add(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to enqueue event for customer {CustomerMobile}", eventDto.CustomerMobile);
                results.Add(new OutboxResponse(
                    0,
                    OutboxState.Failed,
                    null,
                    eventDto.IdempotencyKey));
            }
        }

        logger.LogInformation("Bulk events enqueued: {SuccessCount}/{TotalCount}",
            results.Count(r => r.OutboxState != OutboxState.Failed),
            results.Count);

        return new BulkOutboxResponse(results);
    }
}

/// <summary>
/// پاسخ bulk operations
/// </summary>
public sealed record BulkOutboxResponse(List<OutboxResponse> Results);

