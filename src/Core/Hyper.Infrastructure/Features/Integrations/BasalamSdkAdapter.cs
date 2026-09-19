using System.Globalization;
using Hyper.SDK;
using Hyper.SDK.Auth;
using Hyper.SDK.Clients;
using Hyper.SDK.Config;
using Hyper.SDK.Models;
using Hyper.Domain.Entities.Integrations;
using Hyper.Domain.Features.Integrations;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed class BasalamSdkAdapter(IBasalamHttpClient httpClient)
    : IExternalIntegrationAdapter
{
    private readonly IBasalamHttpClient _http = httpClient;

    public IntegrationProvider Provider => IntegrationProvider.Basalam;
    public bool IsImplemented => true;
    public bool SupportsCredentialType(IntegrationCredentialType type) =>
        type is IntegrationCredentialType.OAuth2 or IntegrationCredentialType.BearerToken;

    public async Task<IReadOnlyCollection<ExternalCatalogItem>> ReadCatalogAsync(
        ExternalIntegrationConnection connection, CancellationToken cancellationToken)
    {
        Validate(connection);
        var vendorId = long.Parse(connection.AccountIdentifier, CultureInfo.InvariantCulture);
        var result = new List<ExternalCatalogItem>();

        for (var page = 1; page <= 10000; page++)
        {
            var url = $"/v1/vendors/{vendorId}/products?page={page}&per_page=100&variants_flatting=false&sort=id:asc";
            var products = await _http.GetAsync<PageResult<CatalogProduct>>(url, cancellationToken);
            if (products is null || products.Data.Count == 0) return result;

            foreach (var product in products.Data)
            {
                if (product.Variants.Count > 0)
                {
                    foreach (var variant in product.Variants)
                    {
                        result.Add(new ExternalCatalogItem(
                            variant.Id.ToString(),
                            variant.Sku,
                            product.Name,
                            variant.Price,
                            variant.Stock ?? product.Stock ?? 0,
                            variant.Id.ToString()));
                    }
                }
                else
                {
                    result.Add(new ExternalCatalogItem(
                        product.Id.ToString(),
                        product.Sku,
                        product.Name,
                        product.Price,
                        product.Stock ?? 0,
                        null));
                }
            }

            if (products.HasMore == false) return result;
        }

        throw new IntegrationProviderException("CatalogLimitExceeded", false);
    }

    public async Task PublishInventoryAsync(
        ExternalIntegrationConnection connection,
        IReadOnlyCollection<ExternalInventoryUpdate> updates,
        CancellationToken cancellationToken)
    {
        Validate(connection);

        foreach (var update in updates)
        {
            var externalProductId = long.Parse(update.ExternalProductId, CultureInfo.InvariantCulture);
            var product = await _http.GetAsync<Product>($"/v1/products/{externalProductId}", cancellationToken);
            if (product is null)
                throw new IntegrationProviderException("InvalidExternalIdentifier", false);

            if (product.VendorId != long.Parse(connection.AccountIdentifier, CultureInfo.InvariantCulture))
                throw new IntegrationProviderException("VendorMismatch", false);

            if (update.VariantId is not null)
            {
                var variantId = long.Parse(update.VariantId, CultureInfo.InvariantCulture);
                var variation = await _http.GetAsync<Variation>($"/v1/variations/{variantId}", cancellationToken);
                if (variation is null || variation.ProductId != externalProductId)
                    throw new IntegrationProviderException("VariantMismatch", false);
                await _http.PatchAsync<object>($"/v1/variations/{variantId}", new { stock = (int)update.Quantity }, cancellationToken);
            }
            else
            {
                await _http.PatchAsync<object>($"/v1/products/{externalProductId}", new { stock = (int)update.Quantity }, cancellationToken);
            }
        }
    }

    private void Validate(ExternalIntegrationConnection connection)
    {
        IntegrationConnectionReadiness.Validate(connection, DateTime.UtcNow);
        if (connection.Provider != Provider || !SupportsCredentialType(connection.CredentialType))
            throw new NotSupportedException("Unsupported Basalam credential strategy.");
        Identifier(connection.AccountIdentifier);
    }

    private static void Identifier(string id)
    {
        if (!long.TryParse(id, NumberStyles.None, CultureInfo.InvariantCulture, out var value) || value <= 0)
            throw new IntegrationProviderException("InvalidExternalIdentifier", false);
    }
}
