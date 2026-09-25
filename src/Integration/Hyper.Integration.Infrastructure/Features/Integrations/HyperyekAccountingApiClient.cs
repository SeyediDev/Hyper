using System.Net.Http.Json;
using Hyperyek.Accounting.Contracts;
using IntegrationCommandResult = Hyper.Integration.Domain.Features.Integrations.BusinessCommandResult;
using IntegrationCommandStatus = Hyper.Integration.Domain.Features.Integrations.BusinessCommandStatus;
using IntegrationCounterparty = Hyper.Integration.Domain.Features.Integrations.IntegrationCounterpartyCommand;
using IntegrationVendorOrder = Hyper.Integration.Domain.Features.Integrations.IntegrationVendorOrderCommand;
using IntegrationCustomerOrder = Hyper.Integration.Domain.Features.Integrations.IntegrationCustomerOrderCommand;
using IntegrationCancelOrder = Hyper.Integration.Domain.Features.Integrations.IntegrationOrderCancellationCommand;
using IntegrationParcelStatus = Hyper.Integration.Domain.Features.Integrations.IntegrationParcelStatusCommand;
using IntegrationExternalProductChanged = Hyper.Integration.Domain.Features.Integrations.IntegrationExternalProductChangedCommand;
using Hyper.Integration.Domain.Features.Integrations;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed class HyperyekAccountingApiOptions
{
    public string BaseAddress { get; set; } = "http://localhost:5100/";
}

/// <summary>HTTP adapter for the platform-owned accounting API.</summary>
public sealed class HyperyekAccountingApiClient(
    HttpClient client) : IIntegrationBusinessCommandPort, IIntegrationAccountingPort,
    IIntegrationPlatformShopPort, IIntegrationPlatformCatalogPort
{
    public async Task<IReadOnlyList<IntegrationPlatformShop>> SearchAsync(string? search, CancellationToken ct)
    {
        var route = string.IsNullOrWhiteSpace(search) ? "api/hyperyek/v1/accounting/platform/shops"
            : $"api/hyperyek/v1/accounting/platform/shops?search={Uri.EscapeDataString(search.Trim())}";
        var result = await GetAsync<List<AccountingShopRead>>(route, ct);
        return result.Select(ToShop).ToArray();
    }

    public async Task<IReadOnlyList<IntegrationPlatformShop>> GetByIdsAsync(IReadOnlyCollection<int> shopIds,
        CancellationToken ct)
    {
        var result = await client.PostAsJsonAsync("api/hyperyek/v1/accounting/platform/shops/by-ids", shopIds, ct);
        result.EnsureSuccessStatusCode();
        var shops = await result.Content.ReadFromJsonAsync<List<AccountingShopRead>>(cancellationToken: ct) ?? [];
        return shops.Select(ToShop).ToArray();
    }

    public async Task<IntegrationPlatformShop?> GetAsync(int shopId, CancellationToken ct)
    {
        using var response = await client.GetAsync($"api/hyperyek/v1/accounting/platform/shops/{shopId}", ct);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        var shop = await response.Content.ReadFromJsonAsync<AccountingShopRead>(cancellationToken: ct);
        return shop is null ? null : ToShop(shop);
    }

    public async Task<IReadOnlyList<IntegrationPlatformProduct>> GetProductsAsync(int shopId, string tenantId,
        CancellationToken ct)
    {
        var route = $"api/hyperyek/v1/accounting/platform/shops/{shopId}/products?tenantId={Uri.EscapeDataString(tenantId)}";
        var products = await GetAsync<List<AccountingProductRead>>(route, ct);
        return products.Select(x => new IntegrationPlatformProduct(x.ProductId, x.Name, x.Sku, x.Price,
            x.Stock, x.IsEnabled, x.IsStockable, x.MinimumStock)).ToArray();
    }

    public async Task<bool> ValidateLinkedCustomerAsync(IntegrationCustomerIdentity customer, int personId,
        CancellationToken ct)
    {
        var result = await PostAsync("api/hyperyek/v1/accounting/counterparties/validate",
            new ValidateCustomerCommand(new(customer.ShopId, customer.TenantId), personId,
                new(customer.Name, customer.Mobile, customer.IdentifierNumber, customer.ExternalCustomerId)), ct);
        return result.Status == IntegrationCommandStatus.Applied;
    }

    public async Task<int> ResolveOrCreateCustomerAsync(IntegrationCustomerIdentity customer,
        CancellationToken ct)
    {
        var result = await PostAsync("api/hyperyek/v1/accounting/counterparties/resolve",
            new ResolveCustomerCommand(new(customer.ShopId, customer.TenantId),
                new(customer.Name, customer.Mobile, customer.IdentifierNumber, customer.ExternalCustomerId)), ct);
        if (result.Status != IntegrationCommandStatus.Applied || !int.TryParse(result.InternalReference, out var id))
            throw new InvalidOperationException(result.ErrorCode ?? "AccountingCustomerResolveFailed");
        return id;
    }
    public Task<IntegrationCommandResult> ApplyCounterpartyAsync(IntegrationCounterparty command, CancellationToken ct) =>
        PostAsync("api/hyperyek/v1/accounting/counterparties", new CounterpartyCommand(command.EventId,
            new(command.ShopId, command.TenantId), command.ExternalCustomerId, command.DisplayName,
            command.Mobile, command.NationalCode), ct);

    public Task<IntegrationCommandResult> ApplyVendorOrderAsync(IntegrationVendorOrder command, CancellationToken ct) =>
        PostAsync("api/hyperyek/v1/accounting/vendor-orders", new VendorOrderCommand(command.EventId,
            new(command.ShopId, command.TenantId), command.ConnectionId, command.ExternalOrderId,
            command.ExternalParcelId, command.ExternalCustomerId, command.Lines.Select(ToLine).ToArray(),
            command.TotalAmount, (byte)command.PaymentStatus, command.AccountingCustomerId), ct);

    public Task<IntegrationCommandResult> ApplyCustomerOrderAsync(IntegrationCustomerOrder command, CancellationToken ct) =>
        PostAsync("api/hyperyek/v1/accounting/customer-orders", new CustomerOrderCommand(command.EventId,
            new(command.ShopId, command.TenantId), command.ConnectionId, command.ExternalOrderId,
            command.Lines.Select(ToLine).ToArray(), command.TotalAmount, (byte)command.PaymentStatus), ct);

    public Task<IntegrationCommandResult> CancelOrderAsync(IntegrationCancelOrder command, CancellationToken ct) =>
        PostAsync("api/hyperyek/v1/accounting/orders/cancel", new CancelOrderCommand(command.EventId,
            new(command.ShopId, command.TenantId), command.ConnectionId, command.ExternalOrderId, command.Reason), ct);

    public Task<IntegrationCommandResult> ApplyParcelStatusAsync(IntegrationParcelStatus command, CancellationToken ct) =>
        PostAsync("api/hyperyek/v1/accounting/parcels/status", new ParcelStatusCommand(command.EventId,
            new(command.ShopId, command.TenantId), command.ConnectionId, command.ExternalOrderId,
            command.ExternalParcelId, command.Status, command.TrackingCode), ct);

    public Task<IntegrationCommandResult> ApplyExternalProductChangedAsync(IntegrationExternalProductChanged command, CancellationToken ct) =>
        PostAsync("api/hyperyek/v1/accounting/products/external-changed", new ExternalProductChangedCommand(
            command.EventId, new(command.ShopId, command.TenantId), command.ConnectionId,
            command.HyperProductId, command.ExternalProductId, command.ExternalVariantId,
            command.Sku, command.Title, command.Price, command.Inventory, command.SourceVersion), ct);

    private async Task<IntegrationCommandResult> PostAsync<T>(string route, T command, CancellationToken ct)
    {
        using var response = await client.PostAsJsonAsync(route, command, ct);
        if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            return new(IntegrationCommandStatus.Duplicate);
        if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            return new(IntegrationCommandStatus.PendingDependency);
        if (!response.IsSuccessStatusCode)
            return new(IntegrationCommandStatus.Rejected, ErrorCode: $"AccountingApi_{(int)response.StatusCode}");
        var result = await response.Content.ReadFromJsonAsync<AccountingCommandResult>(cancellationToken: ct);
        return result is null
            ? new(IntegrationCommandStatus.Rejected, ErrorCode: "AccountingApiEmptyResponse")
            : new((IntegrationCommandStatus)(byte)result.Status, result.InternalReference, result.ErrorCode);
    }

    private async Task<T> GetAsync<T>(string route, CancellationToken ct)
    {
        using var response = await client.GetAsync(route, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: ct)
            ?? throw new InvalidOperationException("AccountingApiEmptyResponse");
    }

    private static IntegrationPlatformShop ToShop(AccountingShopRead shop) =>
        new(shop.ShopId, shop.ShopName, shop.MerchantIdentifier, shop.TenantId);

    private static OrderLineCommand ToLine(IntegrationOrderLineCommand line) =>
        new(line.HyperProductId, line.Quantity, line.UnitPrice, line.ExternalProductId, line.ExternalVariantId);
}
