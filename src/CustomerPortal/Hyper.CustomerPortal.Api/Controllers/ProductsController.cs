using Hyper.CustomerPortal.Application.Features.Products.Queries;
using Microsoft.AspNetCore.Authorization;

namespace Hyper.CustomerPortal.Api.Controllers;

[AppRoute("Hyper", "customer/products")]
[Tags("customer/products")]
[ApiController]
[Authorize]
public class ProductsController : AppControllerBase
{
    [HttpGet("catalog")]
    [ProducesResponseType(typeof(GetProductCatalogQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetProductCatalogQueryResponse>> GetCatalog(
        [FromQuery] GetProductCatalogQuery query)
    {
        var response = await Sender.Send(query);
        return Ok(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetProductByIdQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetProductByIdQueryResponse>> GetById(string id)
    {
        var query = new GetProductByIdQuery { Id = id };
        var response = await Sender.Send(query);
        return Ok(response);
    }
}

