namespace Hyper.SDK.Services;

public interface IVendorService
{
    Task<Vendor?> GetVendorAsync(int vendorId, CancellationToken ct = default);
}

public sealed class VendorService(IBasalamHttpClient client, ILogger<VendorService>? logger = null) : IVendorService
{
    public async Task<Vendor?> GetVendorAsync(int vendorId, CancellationToken ct = default)
    {
        logger?.LogInformation("Getting vendor {VendorId}", vendorId);
        return await client.GetAsync<Vendor>($"/v1/vendors/{vendorId}", ct);
    }
}
