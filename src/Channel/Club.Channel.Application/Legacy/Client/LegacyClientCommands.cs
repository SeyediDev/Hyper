#if LEGACY_CHANNEL_SCENARIOS
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Hyper.Domain.Features;
using FluentValidation;
using MediatR;
using Neo.Domain.Features.Multilingual;

namespace Hyper.Channel.Application.Legacy.Client;

/// <summary>
/// Legacy client commands kept for reference until we migrate all scenarios to the new outbox-driven pipeline.
/// Enable the LEGACY_CHANNEL_SCENARIOS compilation symbol to bring these handlers back.
/// </summary>
public record ConsumeAssetCommand : IRequest<ConsumeAssetCommandResponse>
{
    public string CustomerMobile { get; set; } = null!;
    public string Serial { get; set; } = null!;
}

public record ConsumeAssetCommandResponse
{
    public string CustomerMobile { get; set; } = null!;
    public string Serial { get; set; } = null!;
}

public class ConsumeAssetCommandValidator : AbstractValidator<ConsumeAssetCommand>
{
    public ConsumeAssetCommandValidator(IMultiLingualService multiLingual)
    {
    }
}

public class ConsumeAssetCommandHandler(IAwardAssetService awardAssetService)
    : IRequestHandler<ConsumeAssetCommand, ConsumeAssetCommandResponse>
{
    public async Task<ConsumeAssetCommandResponse> Handle(ConsumeAssetCommand request, CancellationToken cancellationToken)
    {
        ConsumeAccetResponse response = await awardAssetService.ConsumeAward(new()
            {
                Customer = request.CustomerMobile,
                Serial = request.Serial,
            }, cancellationToken);
        return new ConsumeAssetCommandResponse
        {
            CustomerMobile = request.CustomerMobile,
            Serial = request.Serial
        };
    }
}

public record TransferPointsCommand : IRequest<TransferPointsResponse>
{
    [Required]
    public int TenantId { get; set; }

    [Required]
    [MaxLength(15)]
    public string SourceCustomerMobile { get; set; } = null!;

    [Required]
    [MaxLength(15)]
    public string DestinationCustomerMobile { get; set; } = null!;

    [Required]
    public int PointId { get; set; }

    [Required]
    [Range(1, long.MaxValue)]
    public long Amount { get; set; }

    [Required]
    public int EventChannelId { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
}

public record TransferPointsResponse
{
    public long SourceTransactionId { get; set; }
    public long DestinationTransactionId { get; set; }
    public long EventLogId { get; set; }
    public long AmountTransferred { get; set; }
    public long? CommissionCharged { get; set; }
    public string Message { get; set; } = "انتقال امتیاز با موفقیت انجام شد";
}

public class TransferPointsCommandValidator : AbstractValidator<TransferPointsCommand>
{
    public TransferPointsCommandValidator(IMultiLingualService multiLingual)
    {
        RuleFor(x => x.TenantId).GreaterThan(0);
        RuleFor(x => x.SourceCustomerMobile).NotEmpty().Matches(@"^09\d{9}$");
        RuleFor(x => x.DestinationCustomerMobile).NotEmpty().Matches(@"^09\d{9}$")
            .NotEqual(x => x.SourceCustomerMobile);
        RuleFor(x => x.PointId).GreaterThan(0);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.EventChannelId).GreaterThan(0);
    }
}

public class TransferPointsCommandHandler(IPointTransferService pointTransferService)
    : IRequestHandler<TransferPointsCommand, TransferPointsResponse>
{
    public async Task<TransferPointsResponse> Handle(TransferPointsCommand request, CancellationToken cancellationToken)
    {
        var transferRequest = new PointTransferRequest
        {
            TenantId = request.TenantId,
            SourceCustomerMobile = request.SourceCustomerMobile,
            DestinationCustomerMobile = request.DestinationCustomerMobile,
            PointId = request.PointId,
            Amount = request.Amount,
            EventChannelId = request.EventChannelId,
            Description = request.Description
        };

        var result = await pointTransferService.TransferPointsAsync(transferRequest, cancellationToken);

        return new TransferPointsResponse
        {
            SourceTransactionId = result.SourceTransactionId,
            DestinationTransactionId = result.DestinationTransactionId,
            EventLogId = result.EventLogId,
            AmountTransferred = result.AmountTransferred,
            CommissionCharged = result.CommissionCharged
        };
    }
}

public record EventCommand : IRequest
{
    public string CustomerMobile { get; set; } = null!;
    public string EventType { get; set; } = null!;
    public string EventChannel { get; set; } = null!;
    public Dictionary<string, string>? Parameters { get; set; }
}

public class EventCommandValidator : AbstractValidator<EventCommand>
{
    public EventCommandValidator(IMultiLingualService multiLingual)
    {
    }
}

public class EventCommandHandler(
    IEventService eventLogService,
    IPromotionService promotionService
) : IRequestHandler<EventCommand>
{
    public async Task Handle(EventCommand request, CancellationToken cancellationToken)
    {
        var eventRequest = new EventRequest(
            TriggerType.Event,
            request.CustomerMobile,
            request.Parameters)
        {
            EventChannel = request.EventChannel,
            EventType = request.EventType
        };
        EventResponse? eventResponse = await eventLogService.RecordEventAsync(eventRequest, cancellationToken);
        if (eventResponse != null)
        {
            await promotionService.ProcessEventAsync(
                new PromotionProcessingRequest(
                    0, // Legacy - باید از eventResponse یا request گرفته شود
                    eventResponse.EventLogId,
                    eventResponse.EventChannelId,
                    eventResponse.EventTypeId,
                    eventResponse.Customer,
                    request.Parameters)
                {
                    TriggerType = TriggerType.Event
                }, cancellationToken);
        }
    }
}
#endif

