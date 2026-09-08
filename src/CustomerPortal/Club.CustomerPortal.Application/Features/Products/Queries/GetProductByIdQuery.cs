using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Products.Queries;

public record GetProductByIdQuery : IRequest<GetProductByIdQueryResponse>
{
    public string Id { get; set; } = null!;
}

public record GetProductByIdQueryResponse
{
    public ProductDto? Product { get; set; }
}

public class GetProductByIdQueryHandler(
    IProductService productService) : IRequestHandler<GetProductByIdQuery, GetProductByIdQueryResponse>
{
    public async Task<GetProductByIdQueryResponse> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await productService.GetProductByIdAsync(int.Parse(request.Id), cancellationToken);
        
        return new GetProductByIdQueryResponse
        {
            Product = product
        };
    }
}

