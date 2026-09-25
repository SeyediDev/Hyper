using Hyper.Integration.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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
        var body = buffer.ToArray();
        string HeaderOrPayload(string header, params string[] names)
        {
            var value = Request.Headers[header].ToString();
            if (!string.IsNullOrWhiteSpace(value)) return value;
            try
            {
                using var json = JsonDocument.Parse(body);
                foreach (var name in names)
                    if (json.RootElement.TryGetProperty(name, out var property) && property.ValueKind == JsonValueKind.String)
                        return property.GetString()!;
            }
            catch (JsonException) { }
            return string.Empty;
        }
        var result = await ingress.ReceiveAsync(new WebhookIngressRequest(
            parsedProvider, connectionKey,
            HeaderOrPayload("X-Event-Id", "id", "event_id", "eventId"),
            HeaderOrPayload("X-Event-Type", "event", "event_type", "eventType", "name"),
            Request.Headers["X-Event-Timestamp"].ToString(),
            Request.Headers["X-Event-Signature"].ToString(),
            body, Request.Headers["X-Correlation-Id"].ToString(),
            Request.Headers.Authorization.ToString()), cancellationToken);

        return result.Status switch
        {
            WebhookIngressStatus.Accepted => Accepted(result),
            WebhookIngressStatus.Duplicate => Ok(result),
            WebhookIngressStatus.Unsupported => StatusCode(StatusCodes.Status422UnprocessableEntity, result),
            _ => BadRequest(result)
        };
    }
}
