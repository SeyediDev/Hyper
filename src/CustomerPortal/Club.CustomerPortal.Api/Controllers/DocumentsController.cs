using Hyper.Application.Features.Common.Queries.Documents;
using Microsoft.AspNetCore.Authorization;

namespace Hyper.CustomerPortal.Api.Controllers;

[AppRoute("Hyper", "documents")]
[Tags("documents")]
[ApiController]
[Authorize]
public class DocumentsController : AppControllerBase
{
    [HttpGet("{id:int}/download")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Download(int id)
    {
        var document = await Sender.Send(new GetOneDocumentQuery { DocumentId = id });

        if (document is null)
        {
            return NotFound();
        }

        var contentType = document.Dto.Type ?? "application/octet-stream";
        return File(document.Dto.Content, contentType);
    }
}

