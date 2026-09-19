using Hyper.SDK.Auth;
using Hyper.SDK.Clients;
using Hyper.SDK.Config;
using Hyper.SDK.Errors;
using Hyper.SDK.Models;
using Microsoft.Extensions.Logging;

namespace Hyper.SDK.Services;

public interface IVendorService
{
    Task<Vendor?> GetVendorAsync(int vendorId, CancellationToken ct = default);
}

public sealed class VendorService : IVendorService
{
    private readonly IBasalamHttpClient _client;
    private readonly ILogger<VendorService>? _logger;

    public VendorService(IBasalamHttpClient client, ILogger<VendorService>? logger = null)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<Vendor?> GetVendorAsync(int vendorId, CancellationToken ct = default)
    {
        _logger?.LogInformation("Getting vendor {VendorId}", vendorId);
        return await _client.GetAsync<Vendor>($"/v1/vendors/{vendorId}", ct);
    }
}
