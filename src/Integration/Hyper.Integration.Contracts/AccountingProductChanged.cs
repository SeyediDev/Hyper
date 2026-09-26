namespace Hyper.Integration.Contracts;

// PrimaryPrice is an absolute base price in the provider's currency unit; no conversion is implied.
public sealed record AccountingProductChangedRequest(int ShopId, string TenantId, long ConnectionId,
    string ExternalProductId, string? ExternalVariantId, long SourceVersion,
    string? Title = null, decimal? PrimaryPrice = null);

public sealed record AccountingProductChangedResponse(long? OutboxMessageId, string Status, string? ErrorCode = null);

public interface IIntegrationAccountingProductEventIngress
{
    Task<AccountingProductChangedResponse> ReceiveAsync(AccountingProductChangedRequest request, CancellationToken ct = default);
}
