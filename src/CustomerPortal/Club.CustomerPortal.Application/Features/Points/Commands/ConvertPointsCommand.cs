using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Points.Commands;

public record ConvertPointsCommand : IRequest
{
    public required string FromPointTypeId { get; set; }
    public required string ToPointTypeId { get; set; }
    public int Amount { get; set; }
}

public class ConvertPointsCommandValidator : AbstractValidator<ConvertPointsCommand>
{
    public ConvertPointsCommandValidator()
    {
        RuleFor(x => x.FromPointTypeId).NotEmpty().WithMessage("نوع امتیاز مبدا الزامی است");
        RuleFor(x => x.ToPointTypeId).NotEmpty().WithMessage("نوع امتیاز مقصد الزامی است");
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("مقدار باید بیشتر از صفر باشد");
    }
}

public class ConvertPointsCommandHandler(
    IPointService pointService,
    ICustomerRequesterUser requesterUser,
    ILogger<ConvertPointsCommandHandler> logger) : IRequestHandler<ConvertPointsCommand>
{
    public async Task Handle(ConvertPointsCommand request, CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;
        
        var result = await pointService.ConvertPointsAsync(
            customerId,
            int.Parse(request.FromPointTypeId),
            int.Parse(request.ToPointTypeId),
            request.Amount,
            cancellationToken);
        
        if (!result.Success)
        {
            throw new InvalidOperationException(result.Message);
        }
        
        logger.LogInformation("Points converted successfully for customer {CustomerId}: {Amount} from {From} to {To}",
            customerId, request.Amount, request.FromPointTypeId, request.ToPointTypeId);
    }
}

