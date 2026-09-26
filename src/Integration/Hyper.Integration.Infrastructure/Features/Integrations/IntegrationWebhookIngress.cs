using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Hyper.Integration.Contracts;
using Microsoft.EntityFrameworkCore;
using Hyper.Infrastructure.Data.Repository.Hyper;

using ContractProvider = Hyper.Integration.Contracts.IntegrationProvider;
using DomainProvider = Hyper.Integration.Domain.Entities.Integrations.IntegrationProvider;

namespace Hyper.Infrastructure.Features.Integrations;

/// <summary>
/// The persistence entry point for the new provider webhook API. It validates, audits
/// and enqueues the raw event; business processing is deliberately left to the worker.
/// </summary>
public sealed class IntegrationWebhookIngress(
    HyperIntegrationContext db,
    IIntegrationWebhookVerifier verifier) : IIntegrationWebhookIngress
{
    public async Task<WebhookIngressResult> ReceiveAsync(WebhookIngressRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Body is null || request.Body.Length == 0 || request.Body.Length > 1024 * 1024)
            return new(WebhookIngressStatus.Invalid, ErrorCode: "PayloadTooLargeOrEmpty");
        if (!Valid(request.ConnectionKey) || !Valid(request.EventId, 128) || !Valid(request.EventType))
            return new(WebhookIngressStatus.Invalid, ErrorCode: "InvalidHeader");
        JsonDocument payload;
        try { payload = JsonDocument.Parse(request.Body); }
        catch (JsonException) { return new(WebhookIngressStatus.Invalid, ErrorCode: "InvalidJson"); }
        using var payloadLifetime = payload;
        if (payload.RootElement.ValueKind != JsonValueKind.Object)
            return new(WebhookIngressStatus.Invalid, ErrorCode: "InvalidJsonObject");

        // Hyperyek owns the internal hyper-hmac-v1 protocol. Its public contract
        // name is kept separate from the legacy provider enum used by persistence.
        var provider = request.Provider == ContractProvider.Hyperyek
            ? DomainProvider.Custom
            : (DomainProvider)(byte)request.Provider;
        var candidates = await db.ExternalIntegrationConnections.AsNoTracking().Where(x =>
            x.Provider == provider && x.AccountIdentifier == request.ConnectionKey).ToListAsync(cancellationToken);
        if (candidates.Count == 0) return new(WebhookIngressStatus.Invalid, ErrorCode: "ConnectionNotFound");
        // The same vendor may be registered in different shop/tenant scopes.
        // Only the connection-specific credential can select one of those scopes.
        var verificationRequest = new Hyper.Integration.Domain.Features.Integrations.IntegrationWebhookRequest(
                request.Body, request.EventId, request.EventType, request.Timestamp, request.Signature,
                request.Authorization);
        var validations = candidates.Select(candidate => new
        {
            Connection = candidate,
            Result = verifier.Verify(candidate, verificationRequest, DateTimeOffset.UtcNow)
        }).ToArray();
        if (validations.All(x => x.Result == WebhookValidationResult.Unsupported))
            return new(WebhookIngressStatus.Unsupported, ErrorCode: "ProviderWebhookUnsupported");
        var verified = validations.Where(x => x.Result == WebhookValidationResult.Valid).ToArray();
        if (verified.Length == 0)
            return new(WebhookIngressStatus.Invalid, ErrorCode: "SignatureInvalid");
        if (verified.Length != 1)
            return new(WebhookIngressStatus.Invalid, ErrorCode: "ConnectionKeyAmbiguous");
        var connection = verified[0].Connection;

        var existing = await db.IntegrationWebhookInbox.AsNoTracking().SingleOrDefaultAsync(x =>
            x.ConnectionId == connection.Id && x.ExternalEventId == request.EventId, cancellationToken);
        if (existing is not null) return ReplayResult(existing, request);

        var audit = new IntegrationEventAudit
        {
            ConnectionId = connection.Id,
            Direction = "Inbound",
            EventType = request.EventType,
            ExternalEventId = request.EventId,
            PayloadHash = Convert.ToHexString(SHA256.HashData(request.Body)),
            SignatureValid = true,
            CorrelationId = request.CorrelationId,
            ReceivedAtUtc = DateTime.UtcNow
        };
        var inbox = new IntegrationWebhookInbox
        {
            ConnectionId = connection.Id,
            ExternalEventId = request.EventId,
            EventType = request.EventType,
            PayloadJson = Encoding.UTF8.GetString(request.Body),
            Status = 0,
            ReceivedAtUtc = DateTime.UtcNow
        };
        db.IntegrationEventAudits.Add(audit);
        db.IntegrationWebhookInbox.Add(inbox);
        IntegrationScenarioJob? scenarioJob = null;
        var isLoopback = IntegrationSourceRules.IsLoopback(IntegrationSourceRules.ReadSource(payload.RootElement));
        if (!isLoopback && TryMapScenario(request.EventType, out var item))
        {
            // Keep ingress atomic: the durable inbox and the worker job are committed
            // together. The worker remains the only component that applies business work.
            scenarioJob = new IntegrationScenarioJob
            {
                ConnectionId = connection.Id,
                ShopId = connection.ShopId,
                TenantId = connection.TenantId,
                EventId = request.EventId,
                Item = item,
                Trigger = request.Provider == ContractProvider.Hyperyek
                    ? IntegrationSyncTrigger.StoreChanged : IntegrationSyncTrigger.BoothChanged,
                CreatedAtUtc = DateTime.UtcNow,
                NextAttemptAtUtc = DateTime.UtcNow
            };
            db.IntegrationScenarioJobs.Add(scenarioJob);
        }
        else
        {
            // An ignored echo or unsupported event has no worker job. Do not
            // leave its Inbox entry permanently reporting pending processing.
            inbox.Status = isLoopback ? (byte)1 : (byte)2;
            inbox.ProcessedAtUtc = DateTime.UtcNow;
            inbox.Error = isLoopback ? null : "UnsupportedEventType";
        }
        try
        {
            await db.SaveChangesAsync(cancellationToken);
            return new(WebhookIngressStatus.Accepted, inbox.Id, scenarioJob?.Id);
        }
        catch (DbUpdateException)
        {
            var duplicate = await db.IntegrationWebhookInbox.AsNoTracking().SingleOrDefaultAsync(x =>
                x.ConnectionId == connection.Id && x.ExternalEventId == request.EventId, cancellationToken);
            if (duplicate is not null)
            {
                db.Entry(audit).State = EntityState.Detached;
                db.Entry(inbox).State = EntityState.Detached;
                if (scenarioJob is not null) db.Entry(scenarioJob).State = EntityState.Detached;
                return ReplayResult(duplicate, request);
            }
            throw;
        }
    }

    private static WebhookIngressResult ReplayResult(IntegrationWebhookInbox existing, WebhookIngressRequest request) =>
        existing.EventType == request.EventType && existing.PayloadJson == Encoding.UTF8.GetString(request.Body)
            ? new(WebhookIngressStatus.Duplicate, existing.Id)
            : new(WebhookIngressStatus.Invalid, ErrorCode: "EventIdentityConflict");

    private static bool Valid(string value, int maximumLength = 200) => !string.IsNullOrWhiteSpace(value)
        && value.Length <= maximumLength && !value.Any(char.IsControl);

    private static bool TryMapScenario(string eventType, out IntegrationSyncItem item)
    {
        item = eventType.Trim().ToLowerInvariant() switch
        {
            "product.created" or "product.updated" or "product.changed" or "product_create_changes" => IntegrationSyncItem.Product,
            "inventory.updated" or "stock.updated" or "inventory.changed" => IntegrationSyncItem.Inventory,
            "customer.created" or "customer.updated" or "customer.changed" => IntegrationSyncItem.Counterparty,
            "order.vendor.created" or "order.vendor.updated" or "order.vendor.cancelled"
                or "order.vendor.returned" or "parcel.created" or "parcel.status_changed"
                or "vendor_new_order" or "vendor_order_item_changes" or "vendor_parcel_changes" => IntegrationSyncItem.Sale,
            "order.customer.created" or "order.customer.updated" or "order.customer.cancelled"
                or "order.customer.returned" or "new_order" or "order_item_changes" => IntegrationSyncItem.Purchase,
            "subscription.created" or "subscription.renewed" or "subscription.cancelled" => IntegrationSyncItem.Subscription,
            "review.created" or "review.updated" or "review_create_changes" => IntegrationSyncItem.Review,
            "chat.message.received" or "chat.message.sent" or "chat_received_message" or "chat_send_message" => IntegrationSyncItem.Chat,
            _ => default
        };
        return item != default;
    }
}

