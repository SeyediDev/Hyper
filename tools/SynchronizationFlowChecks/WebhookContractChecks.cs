using System.Text;
using Hyper.Infrastructure.Features.Integrations;
using Hyper.Integration.Api;
using Hyper.Integration.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

internal static class WebhookContractChecks
{
    internal static async Task Run(IIntegrationWebhookIngress ingress, Action<bool, string> check)
    {
        foreach (var body in new[] { "[]", "null", "42", "\"text\"", "{" })
        {
            var controller = Controller(ingress, body);
            check(await controller.Receive("basalam", "71", default) is BadRequestObjectResult,
                "HTTP controller rejects malformed/non-object JSON without a server exception: " + body);
        }
        check(await Controller(ingress, "[]", "invalid-shape", "product.updated").Receive("basalam", "71", default)
            is BadRequestObjectResult, "non-object JSON with valid metadata is also rejected by ingress");
        check(await Controller(ingress, "{}", "bad-provider", "product.updated").Receive("99", "71", default)
            is BadRequestObjectResult, "undefined numeric provider rejected by HTTP controller");
        var entityOnly = Controller(ingress, "{\"id\":\"123\",\"name\":\"PRODUCT_CREATE_CHANGES\"}");
        var result = (await entityOnly.Receive("basalam", "71", default) as BadRequestObjectResult)?.Value as WebhookIngressResult;
        check(result?.ErrorCode == "InvalidHeader", "Basalam entity ID/name cannot masquerade as delivery ID/type");
        var typeOnly = Controller(ingress, "{\"event_id\":8,\"event\":\"PRODUCT_CREATE_CHANGES\",\"id\":123}");
        result = (await typeOnly.Receive("basalam", "71", default) as BadRequestObjectResult)?.Value as WebhookIngressResult;
        check(result?.ErrorCode == "InvalidHeader", "numeric catalog event ID is not promoted to a unique delivery ID");

        var recording = new RecordingIngress();
        var explicitMetadata = Controller(recording,
            "{\"id\":\"entity-1\",\"event_id\":\"delivery-1\",\"event_type\":\"PRODUCT_CREATE_CHANGES\"}");
        await explicitMetadata.Receive("basalam", "71", default);
        check(recording.Request is { EventId: "delivery-1", EventType: "PRODUCT_CREATE_CHANGES" },
            "explicit normalized delivery metadata wins over entity ID");
        var headerMetadata = Controller(recording, "{\"event_id\":\"body-id\",\"event_type\":\"body-type\"}", "header-id", "header-type");
        await headerMetadata.Receive("basalam", "71", default);
        check(recording.Request is { EventId: "header-id", EventType: "header-type", Authorization: "Bearer fixture-secret-a" },
            "explicit transport metadata and per-connection authorization reach ingress unchanged");
        await Controller(recording, "{\"id\":\"legacy-id\",\"name\":\"legacy-event\"}").Receive("hyperyek", "71", default);
        check(recording.Request is { EventId: "legacy-id", EventType: "legacy-event" },
            "internal legacy metadata fallback remains compatible");
        check(BasalamWebhookEvents.All.Count == 9
            && BasalamWebhookEvents.All.Select(x => x.Id).SequenceEqual(Enumerable.Range(1, 9))
            && BasalamWebhookEvents.All.Select(x => x.Name).Distinct().Count() == 9,
            "registration catalog contains exactly the nine documented event types");
    }

    internal static IntegrationWebhookController Controller(IIntegrationWebhookIngress ingress, string body,
        string? eventId = null, string? eventType = null)
    {
        var context = new DefaultHttpContext();
        context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
        context.Request.Headers.Authorization = "Bearer fixture-secret-a";
        if (eventId is not null) context.Request.Headers["X-Event-Id"] = eventId;
        if (eventType is not null) context.Request.Headers["X-Event-Type"] = eventType;
        return new(ingress) { ControllerContext = new ControllerContext { HttpContext = context } };
    }

    private sealed class RecordingIngress : IIntegrationWebhookIngress
    {
        public WebhookIngressRequest? Request;
        public Task<WebhookIngressResult> ReceiveAsync(WebhookIngressRequest request, CancellationToken ct = default)
        {
            Request = request;
            return Task.FromResult(new WebhookIngressResult(WebhookIngressStatus.Accepted));
        }
    }
}
