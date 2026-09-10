using Hyper.CustomerPortal.Application.Features.Transactions.Queries;
using Microsoft.AspNetCore.Authorization;

namespace Hyper.CustomerPortal.Api.Controllers;

[AppRoute("Hyper", "customer/transactions")]
[Tags("customer/transactions")]
[ApiController]
[Authorize]
public class CustomerTransactionsController : AppControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(GetTransactionsQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetTransactionsQueryResponse>> GetTransactions([FromQuery] GetTransactionsQuery query)
    {
        var response = await Sender.Send(query);
        return Ok(response);
    }
}







