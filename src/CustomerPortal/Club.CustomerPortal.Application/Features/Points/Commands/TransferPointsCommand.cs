using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Points.Commands;

public record TransferPointsCommand : IRequest
{
    public required string PointTypeId { get; set; }
    public required string ToCustomerId { get; set; }
    public int Amount { get; set; }
    public string? Description { get; set; }
}

public class TransferPointsCommandValidator : AbstractValidator<TransferPointsCommand>
{
    public TransferPointsCommandValidator()
    {
        RuleFor(x => x.PointTypeId).NotEmpty().WithMessage("نوع امتیاز الزامی است");
        RuleFor(x => x.ToCustomerId).NotEmpty().WithMessage("مشتری مقصد الزامی است");
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("مقدار باید بیشتر از صفر باشد");
    }
}

public class TransferPointsCommandHandler(
    IPointService pointService,
    ICustomerRequesterUser requesterUser,
    ILogger<TransferPointsCommandHandler> logger) : IRequestHandler<TransferPointsCommand>
{
    public async Task Handle(TransferPointsCommand request, CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;
        
        var result = await pointService.TransferPointsAsync(
            customerId,
            request.ToCustomerId, // این در واقع شماره موبایل است
            request.Amount,
            cancellationToken);
        
        if (!result.Success)
        {
            throw new InvalidOperationException(result.Message);
        }
        
        logger.LogInformation("Points transferred successfully from customer {CustomerId} to {ToCustomer}: {Amount}",
            customerId, request.ToCustomerId, request.Amount);
    }
}

