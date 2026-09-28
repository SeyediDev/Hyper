using System.Text.Json;

namespace Basalam.SDK.Models;

// Contract: basalam/php-sdk 44d49e4, OrderProcessing/Models/ParcelResponse.php.
// Read only explicit provider fields; never use status titles as machine codes.
public sealed record VendorParcelSnapshot(int Id, int VendorId, int OrderId, int StatusId,
    bool IsDelivered, int ShippingMethod, string? TrackingCode)
{
    public static VendorParcelSnapshot Parse(JsonElement root, int expectedId)
    {
        static int Id(JsonElement value, string name) => value.TryGetProperty(name, out var p)
            && p.TryGetInt32(out var n) && n > 0 ? n : throw new JsonException("Invalid parcel identity or code");
        var id = Id(root, "id");
        if (id != expectedId) throw new JsonException("Parcel identity mismatch");
        var vendor = Id(root.GetProperty("vendor"), "id");
        var order = Id(root.GetProperty("order"), "id");
        var status = Id(root.GetProperty("status"), "id");
        var method = Id(root.GetProperty("shipping_method").GetProperty("current"), "id");
        var delivered = root.GetProperty("is_delivered").GetBoolean();
        string? tracking = null;
        if (root.TryGetProperty("post_receipt", out var receipt) && receipt.ValueKind != JsonValueKind.Null)
        {
            if (receipt.ValueKind != JsonValueKind.Object) throw new JsonException("Invalid post receipt");
            if (receipt.TryGetProperty("tracking_code", out var code) && code.ValueKind != JsonValueKind.Null)
            {
                if (code.ValueKind != JsonValueKind.String) throw new JsonException("Invalid tracking code");
                tracking = code.GetString();
            }
        }
        return new(id, vendor, order, status, delivered, method, tracking);
    }
}
