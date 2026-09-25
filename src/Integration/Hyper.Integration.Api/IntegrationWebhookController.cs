using Hyper.Integration.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hyper.Integration.Api;

[ApiController]
[AllowAnonymous]
[Route("api/integrations/v1/webhooks")]
public sealed class IntegrationWebhookController(IIntegrationWebhookIngress ingress) : ControllerBase
{
    [HttpPost("{provider}/{connectionKey}")]
    [RequestSizeLimit(1024 * 1024)]
    public async Task<IActionResult> Receive(string provider, string connectionKey, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<IntegrationProvider>(provider, true, out var parsedProvider))
            return BadRequest(new { error = "UnsupportedProvider" });

        await using var buffer = new MemoryStream();
        await Request.Body.CopyToAsync(buffer, cancellationToken);
        var result = await ingress.ReceiveAsync(new WebhookIngressRequest(
            parsedProvider, connectionKey,
            Request.Headers["X-Event-Id"].ToString(),
            Request.Headers["X-Event-Type"].ToString(),
            Request.Headers["X-Event-Timestamp"].ToString(),
            Request.Headers["X-Event-Signature"].ToString(),
            buffer.ToArray(), Request.Headers["X-Correlation-Id"].ToString()), cancellationToken);

        return result.Status switch
        {
            WebhookIngressStatus.Accepted => Accepted(result),
            WebhookIngressStatus.Duplicate => Ok(result),
            WebhookIngressStatus.Unsupported => StatusCode(StatusCodes.Status422UnprocessableEntity, result),
            _ => BadRequest(result)
        };
    }
}
