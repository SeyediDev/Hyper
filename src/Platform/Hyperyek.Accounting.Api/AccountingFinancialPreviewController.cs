using Hyperyek.Accounting.Contracts;
using Hyperyek.Accounting.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyperyek.Accounting.Api;

[ApiController]
[Authorize]
[Route("api/hyperyek/v2/accounting/financial-preview")]
public sealed class AccountingFinancialPreviewController(FinancialPreviewCalculator calculator) : ControllerBase
{
    [HttpPost]
    [RequestSizeLimit(1_048_576)]
    public ActionResult<FinancialPreviewResult> Preview([FromBody] FinancialPreviewRequest request) =>
        Ok(calculator.Preview(request));
}
