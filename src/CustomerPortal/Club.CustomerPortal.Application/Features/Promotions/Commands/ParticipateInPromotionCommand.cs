using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Promotions.Commands;

public record ParticipateInPromotionCommand : IRequest<ParticipateInPromotionCommandResponse>
{
    public string PromotionId { get; set; } = null!;
}

public record ParticipateInPromotionCommandResponse
{
    public string Status { get; set; } = null!;
}

public class ParticipateInPromotionCommandValidator : AbstractValidator<ParticipateInPromotionCommand>
{
    public ParticipateInPromotionCommandValidator()
    {
        RuleFor(x => x.PromotionId).NotEmpty().WithMessage("شناسه کمپین الزامی است");
    }
}

public class ParticipateInPromotionCommandHandler(
    IPromotionService promotionService,
    ICustomerRequesterUser requesterUser,
    ILogger<ParticipateInPromotionCommandHandler> logger) : IRequestHandler<ParticipateInPromotionCommand, ParticipateInPromotionCommandResponse>
{
    public async Task<ParticipateInPromotionCommandResponse> Handle(ParticipateInPromotionCommand request, CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;
        var result = await promotionService.ParticipateAsync(customerId, int.Parse(request.PromotionId), cancellationToken);
        
        if (!result.Success)
        {
            throw new InvalidOperationException(result.Message);
        }
        
        logger.LogInformation("Customer {CustomerId} participated in promotion {PromotionId}", customerId, request.PromotionId);
        
        return new ParticipateInPromotionCommandResponse
        {
            Status = "Success"
        };
    }
}

