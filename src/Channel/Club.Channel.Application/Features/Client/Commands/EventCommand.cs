using Neo.Domain.Features.Multilingual;

namespace Club.Channel.Application.Features.Client.Commands;

public record EventCommand : IRequest
{
	/// <summary>
	/// شناسه اکوسیستم
	/// </summary>
	[Required]
    [Range(1, int.MaxValue, ErrorMessage = "شناسه اکوسیستم الزامی است")]
    public int TenantId { get; set; }

    /// <summary>
    /// شماره موبایل مشتری
    /// </summary>
    public string CustomerMobile { get; set; } = null!;
    public string EventType { get; set; } = null!;
    public string EventChannel { get; set; } = null!;
    public Dictionary<string, string>? Parameters { get; set; }
}

public class EventCommandValidator : AbstractValidator<EventCommand>
{
    public EventCommandValidator(IMultiLingualService multiLingual)
    {
        RuleFor(x => x.TenantId)
            .GreaterThan(0)
            .WithMessage("شناسه اکوسیستم الزامی است");

        RuleFor(x => x.CustomerMobile)
            .NotEmpty()
            .WithMessage("شماره موبایل مشتری الزامی است");

        RuleFor(x => x.EventChannel)
            .NotEmpty()
            .WithMessage("شناسه کانال رویداد الزامی است");

        RuleFor(x => x.EventType)
            .NotEmpty()
            .WithMessage("شناسه نوع رویداد الزامی است");
    }
}

public class EventCommandHandler(
    IEventService eventLogService, IPromotionService promotionService
    ) : IRequestHandler<EventCommand>
{
    public async Task Handle(EventCommand request, CancellationToken cancellationToken)
    {
        var eventRequest = new EventRequest(
            TriggerType.Event,
            request.CustomerMobile, // شماره موبایل
            request.Parameters)
        {
            TenantId = request.TenantId,
            EventChannel = request.EventChannel,
            EventType = request.EventType
        };
        EventResponse? eventResponse = await eventLogService.RecordEventAsync(eventRequest, cancellationToken);
        if (eventResponse != null)
        {
            await promotionService.ProcessEventAsync(
                new PromotionProcessingRequest(
                    request.TenantId,
                    (int)eventResponse.EventLogId,
                    eventResponse.EventChannelId ?? 0,
                    eventResponse.EventTypeId ?? 0,
                    eventResponse.Customer,
                    request.Parameters)
                {
                    TriggerType = TriggerType.Event
                }, cancellationToken);
        }
    }
}
