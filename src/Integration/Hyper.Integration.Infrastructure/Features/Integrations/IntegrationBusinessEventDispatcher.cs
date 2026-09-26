using System.Text.Json;
using Hyper.Integration.Domain.Features.Integrations;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;

namespace Hyper.Infrastructure.Features.Integrations;

/// <summary>Translates provider-neutral inbox payloads into platform command contracts.</summary>
public sealed class IntegrationBusinessEventDispatcher(IIntegrationBusinessCommandPort commands,
    IIntegrationEngagementPort engagement,
    IIntegrationInventoryReservation reservations,
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
            var externalOrderId = RequiredAny(root, "externalOrderId", "order_id", "orderId", "id");
            var cancellation = await commands.CancelOrderAsync(new(job.EventId, job.ShopId, job.TenantId,
                job.ConnectionId, externalOrderId,
                OptionalAny(root, "reason", "cancel_reason", "cancelReason", "status") ?? inbox.EventType), ct);
            if (cancellation.Status is BusinessCommandStatus.Applied or BusinessCommandStatus.Duplicate)
                await reservations.ReleaseAsync(new OwnedIntegrationShop(job.ShopId, job.TenantId),
                    IntegrationReservationIdentity.ForOrder(new(job.ShopId, job.TenantId), job.ConnectionId, externalOrderId), ct);
            return Result(cancellation);
        }
        if (job.Item is IntegrationSyncItem.Subscription or IntegrationSyncItem.Review or IntegrationSyncItem.Chat)
            return await DispatchEngagementAsync(job, inbox.EventType, root, ct);
        var result = job.Item switch
        {
            IntegrationSyncItem.Counterparty => await CounterpartyAsync(job, root, ct),
            IntegrationSyncItem.Product => await ProductAsync(job, connection, root, ct),
            IntegrationSyncItem.Sale => await SaleAsync(job, connection, root, ct),
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

    private async Task<BusinessCommandResult> ProductAsync(IntegrationScenarioJob job,
        ExternalIntegrationConnection connection, JsonElement root, CancellationToken ct)
    {
        root = Envelope(root);
        var externalProductId = RequiredAny(root, "externalProductId", "product_id", "productId", "id");
        var variantId = OptionalAny(root, "externalVariantId", "variant_id", "variantId");
        var mapping = await db.ExternalProductMappings.AsNoTracking().SingleOrDefaultAsync(x =>
            x.ConnectionId == connection.Id && x.ShopId == connection.ShopId
            && x.ExternalProductId == externalProductId && x.ExternalVariantId == variantId && x.IsActive, ct);
        if (mapping is null || mapping.HyperProductId <= 0)
            throw new IntegrationProviderException("ProductMappingUnavailable", false);
        return await commands.ApplyExternalProductChangedAsync(new(job.EventId, job.ShopId, job.TenantId,
            job.ConnectionId, mapping.HyperProductId, externalProductId, variantId,
            OptionalAny(root, "sku", "barcode", "tax_code"), RequiredAny(root, "title", "name", "product_name"),
            DecimalOptional(root, "price", "sale_price", "salePrice"),
            DecimalOptional(root, "inventory", "stock", "quantity"),
            LongAny(root, "sourceVersion", "source_version", "version")), ct);
    }

    private async Task<BusinessCommandResult> SaleAsync(IntegrationScenarioJob job,
        ExternalIntegrationConnection connection, JsonElement root, CancellationToken ct)
    {
        root = Envelope(root);
        var lines = await ResolveMappedLinesAsync(job, connection, Lines(root), ct);
        return await SaleCoreAsync(job, root, lines, ct);
    }

    private async Task<IReadOnlyCollection<IntegrationOrderLineCommand>> ResolveMappedLinesAsync(
        IntegrationScenarioJob job, ExternalIntegrationConnection connection,
        IReadOnlyCollection<IntegrationOrderLineCommand> lines, CancellationToken ct)
    {
        var externalIds = lines.Select(x => x.ExternalProductId).Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.Ordinal).ToArray();
        if (externalIds.Length == 0 || lines.Any(x => string.IsNullOrWhiteSpace(x.ExternalProductId)))
            throw new IntegrationProviderException("OrderLineProductIdentityMissing", false);
        var mappings = await db.ExternalProductMappings.AsNoTracking().Where(x =>
            x.ConnectionId == connection.Id && x.ShopId == job.ShopId && x.IsActive
            && externalIds.Contains(x.ExternalProductId)).ToListAsync(ct);
        var resolved = new List<IntegrationOrderLineCommand>(lines.Count);
        foreach (var line in lines)
        {
            var matches = mappings.Where(x => x.ExternalProductId == line.ExternalProductId
                && x.ExternalVariantId == line.ExternalVariantId).ToArray();
            if (matches.Length != 1 || matches[0].HyperProductId <= 0)
                throw new IntegrationProviderException(
                    matches.Length == 0 ? "ProductMappingUnavailable" : "ProductMappingAmbiguous", false);
            resolved.Add(line with { HyperProductId = matches[0].HyperProductId });
        }
        return resolved;
    }

    private async Task<BusinessCommandResult> SaleCoreAsync(IntegrationScenarioJob job, JsonElement root,
        IReadOnlyCollection<IntegrationOrderLineCommand> lines, CancellationToken ct)
    {
        var payment = PaymentStatus(root);
        if (payment != SalePaymentStatus.Paid)
            return new(BusinessCommandStatus.PendingDependency, ErrorCode: "BoothOrderPaymentNotConfirmed");
        var externalCustomerId = RequiredAny(root, "externalCustomerId", "customer_id", "customerId", "user_id", "userId");
        var mapping = await db.IntegrationCustomerMappings.AsNoTracking().SingleOrDefaultAsync(x =>
            x.ShopId == job.ShopId && x.TenantId == job.TenantId && x.ExternalCustomerId == externalCustomerId, ct);
        if (mapping is null || mapping.PersonId <= 0)
            throw new IntegrationProviderException("AccountingCustomerMappingUnavailable", false);
        var externalOrderId = RequiredAny(root, "externalOrderId", "order_id", "orderId", "id");
        var shop = new OwnedIntegrationShop(job.ShopId, job.TenantId);
        var reservationKey = IntegrationReservationIdentity.ForOrder(shop, job.ConnectionId, externalOrderId);
        var command = new IntegrationVendorOrderCommand(job.EventId, job.ShopId, job.TenantId, job.ConnectionId,
            externalOrderId, OptionalAny(root, "externalParcelId", "parcel_id", "parcelId"), externalCustomerId,
            lines, DecimalAny(root, "totalAmount", "total_amount", "total", "amount"), payment, mapping.PersonId);
        await reservations.ReserveAsync(shop, reservationKey, lines, ct);
        // A timeout or error does not prove the remote write failed. Retain the hold
        // for idempotent replay; only an acknowledged cancellation releases it.
        var result = await commands.ApplyVendorOrderAsync(command, ct);
        if (result.StockCommitted && result.Status is BusinessCommandStatus.Applied or BusinessCommandStatus.Duplicate)
            await reservations.CommitAsync(shop, reservationKey, ct);
        return result;
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
            IntegerOptional(line, "hyperProductId"),
            OptionalAny(line, "externalProductId", "product_id", "productId"),
            OptionalAny(line, "externalVariantId", "variant_id", "variantId"),
            DecimalAny(line, "quantity", "count", "amount"),
            DecimalAny(line, "unitPrice", "unit_price", "price"))).ToArray();

    private static int IntegerOptional(JsonElement root, params string[] names) =>
        int.TryParse(OptionalAny(root, names), out var value) ? value : 0;

    private static decimal? DecimalOptional(JsonElement root, params string[] names)
    {
        var value = OptionalAny(root, names);
        return value is null ? null : decimal.TryParse(value, out var parsed) ? parsed : throw new ArgumentException($"Invalid{names[0]}");
    }

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
    private static long LongAny(JsonElement root, params string[] names) =>
        long.TryParse(OptionalAny(root, names), out var value) && value > 0 ? value : throw new ArgumentException($"Missing{names[0]}");
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
