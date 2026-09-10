using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Products.Queries;

public record GetProductCatalogQuery : IRequest<GetProductCatalogQueryResponse>
{
    public int? CategoryId { get; set; }
}

public record GetProductCatalogQueryResponse
{
    public ProductCatalogDto Catalog { get; set; } = null!;
}

public class GetProductCatalogQueryHandler(
    IProductService productService) : IRequestHandler<GetProductCatalogQuery, GetProductCatalogQueryResponse>
{
    public async Task<GetProductCatalogQueryResponse> Handle(GetProductCatalogQuery request, CancellationToken cancellationToken)
    {
        var catalog = await productService.GetProductCatalogAsync(request.CategoryId, cancellationToken);
        
        return new GetProductCatalogQueryResponse
        {
            Catalog = catalog
        };
    }
}

