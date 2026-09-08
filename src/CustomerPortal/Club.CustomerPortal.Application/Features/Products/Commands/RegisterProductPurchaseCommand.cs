using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Products.Commands;

public record RegisterProductPurchaseCommand : IRequest<RegisterProductPurchaseCommandResponse>
{
    public string ProductId { get; set; } = null!;
    public string? SerialNumber { get; set; }
    public int Quantity { get; set; } = 1;
}

public record RegisterProductPurchaseCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public long? PointsAwarded { get; set; }
}

public class RegisterProductPurchaseCommandHandler(
    IProductService productService) : IRequestHandler<RegisterProductPurchaseCommand, RegisterProductPurchaseCommandResponse>
{
    public async Task<RegisterProductPurchaseCommandResponse> Handle(RegisterProductPurchaseCommand request, CancellationToken cancellationToken)
    {
        var response = await productService.RegisterProductPurchaseAsync(
            new RegisterProductPurchaseRequest
            {
                ProductId = int.Parse(request.ProductId),
                SerialNumber = request.SerialNumber,
                Quantity = request.Quantity
            },
            cancellationToken);

        return new RegisterProductPurchaseCommandResponse
        {
            Success = response.Success,
            Message = response.Message,
            PointsAwarded = response.PointsAwarded
        };
    }
}
