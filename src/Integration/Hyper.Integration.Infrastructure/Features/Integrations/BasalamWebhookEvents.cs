namespace Hyper.Infrastructure.Features.Integrations;

/// <summary>
/// Provider event catalog, verified against the official webhook guide on 2026-09-26.
/// https://developers.basalam.com/docs/services/webhook
/// IDs identify event TYPES, never individual deliveries. Registration is not
/// evidence of payload support or downstream business completion.
/// </summary>
public static class BasalamWebhookEvents
{
    public sealed record Definition(int Id, string Name, string Scope, IntegrationSyncItem Item);

    public static IReadOnlyList<Definition> All { get; } = System.Array.AsReadOnly<Definition>(
    [
        new(1, "CHAT_RECEIVED_MESSAGE", "customer.chat.read", IntegrationSyncItem.Chat),
        new(2, "ORDER_ITEM_CHANGES", "customer.order.read", IntegrationSyncItem.Purchase),
        new(3, "VENDOR_ORDER_ITEM_CHANGES", "vendor.parcel.read", IntegrationSyncItem.Sale),
        new(4, "CHAT_SEND_MESSAGE", "customer.chat.read", IntegrationSyncItem.Chat),
        new(5, "VENDOR_NEW_ORDER", "vendor.parcel.read", IntegrationSyncItem.Sale),
        new(6, "NEW_ORDER", "customer.order.read", IntegrationSyncItem.Purchase),
        new(7, "VENDOR_PARCEL_CHANGES", "vendor.parcel.read", IntegrationSyncItem.Sale),
        new(8, "PRODUCT_CREATE_CHANGES", "vendor.product.read", IntegrationSyncItem.Product),
        new(9, "REVIEW_CREATE_CHANGES", "vendor.parcel.read", IntegrationSyncItem.Review)
    ]);

    public static bool TryMap(string eventType, out IntegrationSyncItem item)
    {
        var definition = All.FirstOrDefault(x => x.Name.Equals(eventType.Trim(), StringComparison.OrdinalIgnoreCase));
        item = definition?.Item ?? default;
        return definition is not null;
    }
}
