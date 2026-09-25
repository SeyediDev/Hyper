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
        if (IsParcelEvent(inbox.EventType))
        {
            root = Envelope(root);
            var parcel = await commands.ApplyParcelStatusAsync(new(job.EventId, job.ShopId, job.TenantId,
                job.ConnectionId,
                RequiredAny(root, "externalOrderId", "order_id", "orderId", "order_item_id"),
                RequiredAny(root, "externalParcelId", "parcel_id", "parcelId", "id"),
                RequiredAny(root, "status", "parcel_status", "parcelStatus", "state"),
                OptionalAny(root, "trackingCode", "tracking_code", "trackingNumber")), ct);
            return Result(parcel);
        }
        if (job.Item is IntegrationSyncItem.Sale or IntegrationSyncItem.Purchase
            && IsCancellationEvent(inbox.EventType, root))
        {
            root = Envelope(root);
            var cancellation = await commands.CancelOrderAsync(new(job.EventId, job.ShopId, job.TenantId,
                job.ConnectionId, RequiredAny(root, "externalOrderId", "order_id", "orderId", "id"),
                OptionalAny(root, "reason", "cancel_reason", "cancelReason", "status") ?? inbox.EventType), ct);
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
        root = Envelope(root);
        var lines = Lines(root);
        return commands.ApplyVendorOrderAsync(new(job.EventId, job.ShopId, job.TenantId, job.ConnectionId,
            RequiredAny(root, "externalOrderId", "order_id", "orderId", "id"),
            OptionalAny(root, "externalParcelId", "parcel_id", "parcelId"),
            RequiredAny(root, "externalCustomerId", "customer_id", "customerId", "user_id", "userId"),
            lines, DecimalAny(root, "totalAmount", "total_amount", "total", "amount"),
            PaymentStatus(root)), ct);
    }

    private Task<BusinessCommandResult> PurchaseAsync(IntegrationScenarioJob job, JsonElement root, CancellationToken ct)
    {
        root = Envelope(root);
        var lines = Lines(root);
        return commands.ApplyCustomerOrderAsync(new(job.EventId, job.ShopId, job.TenantId, job.ConnectionId,
            RequiredAny(root, "externalOrderId", "order_id", "orderId", "id"), lines,
            DecimalAny(root, "totalAmount", "total_amount", "total", "amount"), PaymentStatus(root)), ct);
    }

    private static IReadOnlyCollection<IntegrationOrderLineCommand> Lines(JsonElement root) =>
        Array(root, "lines", "items", "order_items", "orderItems").Select(line => new IntegrationOrderLineCommand(
            IntegerAny(line, "hyperProductId", "product_id", "productId", "id"),
            OptionalAny(line, "externalProductId", "product_id", "productId"),
            OptionalAny(line, "externalVariantId", "variant_id", "variantId"),
            DecimalAny(line, "quantity", "count", "amount"),
            DecimalAny(line, "unitPrice", "unit_price", "price"))).ToArray();

    private static JsonElement Envelope(JsonElement root)
    {
        foreach (var name in new[] { "data", "order", "payload" })
            if (root.TryGetProperty(name, out var child) && child.ValueKind == JsonValueKind.Object) return child;
        return root;
    }

    private static bool IsParcelEvent(string eventType) =>
        eventType.Contains("parcel", StringComparison.OrdinalIgnoreCase)
        && (eventType.Contains("status", StringComparison.OrdinalIgnoreCase)
            || eventType.Equals("VENDOR_PARCEL_CHANGES", StringComparison.OrdinalIgnoreCase));

    private static bool IsCancellationEvent(string eventType, JsonElement root)
    {
        if (eventType.Contains("cancel", StringComparison.OrdinalIgnoreCase)
            || eventType.Contains("return", StringComparison.OrdinalIgnoreCase)) return true;
        if (!eventType.Equals("VENDOR_ORDER_ITEM_CHANGES", StringComparison.OrdinalIgnoreCase)) return false;
        root = Envelope(root);
        var status = OptionalAny(root, "status", "item_status", "itemStatus", "state");
        return status is not null && (status.Contains("cancel", StringComparison.OrdinalIgnoreCase)
            || status.Contains("return", StringComparison.OrdinalIgnoreCase)
            || status.Contains("reject", StringComparison.OrdinalIgnoreCase));
    }

    private static IEnumerable<JsonElement> Array(JsonElement root, params string[] names)
    {
        foreach (var name in names)
            if (root.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.Array)
                return value.EnumerateArray();
        throw new ArgumentException("MissingOrderLines");
    }

    private static string RequiredAny(JsonElement root, params string[] names) =>
        OptionalAny(root, names) ?? throw new ArgumentException($"Missing{names[0]}");
    private static string? OptionalAny(JsonElement root, params string[] names)
    {
        foreach (var name in names)
            if (root.TryGetProperty(name, out var value) && value.ValueKind is not JsonValueKind.Null)
                return value.ValueKind == JsonValueKind.String ? value.GetString() : value.ToString();
        return null;
    }
    private static int IntegerAny(JsonElement root, params string[] names) =>
        int.TryParse(OptionalAny(root, names), out var value) ? value : throw new ArgumentException($"Missing{names[0]}");
    private static decimal DecimalAny(JsonElement root, params string[] names) =>
        decimal.TryParse(OptionalAny(root, names), out var value) ? value : throw new ArgumentException($"Missing{names[0]}");
    private static SalePaymentStatus PaymentStatus(JsonElement root) =>
        Enum.TryParse<SalePaymentStatus>(OptionalAny(root, "paymentStatus", "payment_status", "paymentState") ?? "", true, out var status)
            ? status : (SalePaymentStatus)IntegerAny(root, "paymentStatus", "payment_status");

    private static string Required(JsonElement element, string name) =>
        Optional(element, name) ?? throw new ArgumentException($"Missing{char.ToUpperInvariant(name[0])}{name.Substring(1)}");
    private static string? Optional(JsonElement element, string name) =>
        element.TryGetProperty(name, out var value) && value.ValueKind != JsonValueKind.Null ? value.GetString() : null;
    private static int Integer(JsonElement element, string name) => element.GetProperty(name).GetInt32();
    private static decimal Decimal(JsonElement element, string name) => element.GetProperty(name).GetDecimal();
    private static DateTimeOffset? OptionalDate(JsonElement element, string name) =>
        Optional(element, name) is { } value && DateTimeOffset.TryParse(value, out var parsed) ? parsed : null;
}
