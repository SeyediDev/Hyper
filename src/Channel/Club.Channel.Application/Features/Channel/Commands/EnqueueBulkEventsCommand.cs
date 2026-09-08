namespace Hyper.Channel.Application.Features.Channel.Commands;

/// <summary>
/// دستور ارسال چندین رویداد به صورت bulk
/// </summary>
public sealed record EnqueueBulkEventsCommand(
    IReadOnlyList<EnqueueEventCommand> Events,
    string? CorrelationId,
    string? RemoteIp
    ) : IRequest<BulkOutboxResponse>;

internal sealed class EnqueueBulkEventsCommandValidator : AbstractValidator<EnqueueBulkEventsCommand>
{
    public EnqueueBulkEventsCommandValidator()
    {
        RuleFor(x => x.Events)
            .NotEmpty()
            .Must(events => events.Count <= 100) // محدودیت تعداد رویدادها
            .WithMessage("تعداد رویدادها نمی‌تواند بیشتر از 100 باشد");

        RuleForEach(x => x.Events)
            .SetValidator(new EnqueueEventCommandValidator());
    }
}

public sealed class EnqueueBulkEventsCommandHandler(
    ILogger<EnqueueBulkEventsCommandHandler> logger, ISender sender
) : IRequestHandler<EnqueueBulkEventsCommand, BulkOutboxResponse>
{
    public async Task<BulkOutboxResponse> Handle(EnqueueBulkEventsCommand request, CancellationToken cancellationToken)
    {
        var results = new List<OutboxResponse>();

        foreach (var eventDto in request.Events)
        {
            try
            {
                var response = await sender.Send(eventDto, cancellationToken);
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