using System.Text.Json;
using Hyper.Integration.Domain.Features.Integrations;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;

namespace Hyper.Infrastructure.Features.Integrations;

/// <summary>Translates provider-neutral inbox payloads into platform command contracts.</summary>
public sealed class IntegrationBusinessEventDispatcher(IIntegrationBusinessCommandPort commands,
    IIntegrationEngagementPort engagement,
    HyperIntegrationContext db)
{
    public async Task<IntegrationScenarioResult> DispatchAsync(IntegrationScenarioJob job,
        ExternalIntegrationConnection connection, CancellationToken ct)
    {
        var inbox = await db.IntegrationWebhookInbox.AsNoTracking().SingleOrDefaultAsync(x =>
            x.ConnectionId == job.ConnectionId && x.ExternalEventId == job.EventId, ct)
            ?? throw new IntegrationProviderException("WebhookInboxNotFound", false);
        using var document = JsonDocument.Parse(inbox.PayloadJson);
        var root = document.RootElement;
        if (inbox.EventType.Contains("parcel", StringComparison.OrdinalIgnoreCase)
            && inbox.EventType.Contains("status", StringComparison.OrdinalIgnoreCase))
        {
            var parcel = await commands.ApplyParcelStatusAsync(new(job.EventId, job.ShopId, job.TenantId,
                job.ConnectionId, Required(root, "externalOrderId"), Required(root, "externalParcelId"),
                Required(root, "status"), Optional(root, "trackingCode")), ct);
            return Result(parcel);
        }
        if (job.Item is IntegrationSyncItem.Sale or IntegrationSyncItem.Purchase
            && (inbox.EventType.Contains("cancel", StringComparison.OrdinalIgnoreCase)
                || inbox.EventType.Contains("return", StringComparison.OrdinalIgnoreCase)))
        {
            var cancellation = await commands.CancelOrderAsync(new(job.EventId, job.ShopId, job.TenantId,
                job.ConnectionId, Required(root, "externalOrderId"),
                Optional(root, "reason") ?? inbox.EventType), ct);
            return Result(cancellation);
        }
        if (job.Item is IntegrationSyncItem.Subscription or IntegrationSyncItem.Review or IntegrationSyncItem.Chat)
            return await DispatchEngagementAsync(job, inbox.EventType, root, ct);
        var result = job.Item switch
        {
            IntegrationSyncItem.Counterparty => await CounterpartyAsync(job, root, ct),
            IntegrationSyncItem.Sale => await SaleAsync(job, root, ct),
            IntegrationSyncItem.Purchase => await PurchaseAsync(job, root, ct),
            _ => throw new IntegrationProviderException("BusinessItemUnsupported", false)
        };
        if (result.Status is BusinessCommandStatus.Rejected)
            throw new IntegrationProviderException(result.ErrorCode ?? "AccountingCommandRejected", false);
        return new(1, result.Status == BusinessCommandStatus.PendingDependency
            ? [new(null, null, null, "AccountingDependencyPending")] : []);
    }

    private async Task<IntegrationScenarioResult> DispatchEngagementAsync(IntegrationScenarioJob job, string eventType, JsonElement root, CancellationToken ct)
    {
        EngagementCommandResult result = job.Item switch
        {
            IntegrationSyncItem.Subscription => await engagement.ApplySubscriptionAsync(new(job.EventId, job.ShopId, job.TenantId, job.ConnectionId,
                Required(root, "externalSubscriptionId"), Optional(root, "status") ?? eventType, OptionalDate(root, "effectiveAt")), ct),
            IntegrationSyncItem.Review => await engagement.ApplyReviewAsync(new(job.EventId, job.ShopId, job.TenantId, job.ConnectionId,
                Required(root, "externalReviewId"), Optional(root, "externalOrderId"), Integer(root, "rating"), Optional(root, "text")), ct),
            _ => await engagement.ApplyChatMessageAsync(new(job.EventId, job.ShopId, job.TenantId, job.ConnectionId,
                Required(root, "externalConversationId"), Required(root, "externalMessageId"), eventType.EndsWith(".sent", StringComparison.OrdinalIgnoreCase), Optional(root, "text")), ct)
        };
        if (result.Status == EngagementCommandStatus.Rejected)
            throw new IntegrationProviderException(result.ErrorCode ?? "EngagementCommandRejected", false);
        return result.Status == EngagementCommandStatus.PendingDependency
            ? new(1, [new(null, null, null, result.ErrorCode ?? "EngagementDependencyPending")]) : new(1, []);
    }

    private static IntegrationScenarioResult Result(BusinessCommandResult result) =>
        result.Status is BusinessCommandStatus.Rejected
            ? throw new IntegrationProviderException(result.ErrorCode ?? "AccountingCommandRejected", false)
            : new(1, result.Status == BusinessCommandStatus.PendingDependency
                ? [new(null, null, null, "AccountingDependencyPending")] : []);

    private Task<BusinessCommandResult> CounterpartyAsync(IntegrationScenarioJob job, JsonElement root, CancellationToken ct) =>
        commands.ApplyCounterpartyAsync(new(job.EventId, job.ShopId, job.TenantId,
            Required(root, "externalCustomerId"), Optional(root, "displayName"), Optional(root, "mobile"),
            Optional(root, "nationalCode")), ct);

    private Task<BusinessCommandResult> SaleAsync(IntegrationScenarioJob job, JsonElement root, CancellationToken ct)
    {
        var lines = Lines(root);
        return commands.ApplyVendorOrderAsync(new(job.EventId, job.ShopId, job.TenantId, job.ConnectionId,
            Required(root, "externalOrderId"), Optional(root, "externalParcelId"),
            Required(root, "externalCustomerId"), lines, Decimal(root, "totalAmount"),
            (SalePaymentStatus)Integer(root, "paymentStatus")), ct);
    }

    private Task<BusinessCommandResult> PurchaseAsync(IntegrationScenarioJob job, JsonElement root, CancellationToken ct)
    {
        var lines = Lines(root);
        return commands.ApplyCustomerOrderAsync(new(job.EventId, job.ShopId, job.TenantId, job.ConnectionId,
            Required(root, "externalOrderId"), lines, Decimal(root, "totalAmount"),
            (SalePaymentStatus)Integer(root, "paymentStatus")), ct);
    }

    private static IReadOnlyCollection<IntegrationOrderLineCommand> Lines(JsonElement root) =>
        root.GetProperty("lines").EnumerateArray().Select(line => new IntegrationOrderLineCommand(
            Integer(line, "hyperProductId"), Optional(line, "externalProductId"),
            Optional(line, "externalVariantId"), Decimal(line, "quantity"), Decimal(line, "unitPrice"))).ToArray();

    private static string Required(JsonElement element, string name) =>
        Optional(element, name) ?? throw new ArgumentException($"Missing{char.ToUpperInvariant(name[0])}{name.Substring(1)}");
    private static string? Optional(JsonElement element, string name) =>
        element.TryGetProperty(name, out var value) && value.ValueKind != JsonValueKind.Null ? value.GetString() : null;
    private static int Integer(JsonElement element, string name) => element.GetProperty(name).GetInt32();
    private static decimal Decimal(JsonElement element, string name) => element.GetProperty(name).GetDecimal();
    private static DateTimeOffset? OptionalDate(JsonElement element, string name) =>
        Optional(element, name) is { } value && DateTimeOffset.TryParse(value, out var parsed) ? parsed : null;
}
