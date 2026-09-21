namespace BasalamSandbox;

public sealed record Vendor(long Id, string Identifier, string Title);
public sealed record Product(long Id, string Sku, string Name, long Price, int Stock);
public sealed record ProductDetails(long Id, long VendorId, string Sku, string Name, long Price, int Stock);
public sealed record Variant(long Id, long ProductId, string Title, string Sku, long Price, int Stock);
public sealed record VendorProfile(long Id, string Identifier, string Title)
{
    public object vendor => new { id = Id, identifier = Identifier, title = Title, is_active = true };
    public string Username => Identifier;
    public string Name => Title;
}
