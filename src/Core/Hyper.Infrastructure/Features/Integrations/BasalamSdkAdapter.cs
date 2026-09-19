using System.Globalization;
using Hyper.SDK;
using Hyper.SDK.Clients;
using Hyper.SDK.Models;
using Hyper.Domain.Entities.Integrations;
using Hyper.Domain.Features.Integrations;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed class BasalamSdkAdapter(IBasalamClient client) : IExternalIntegrationAdapter
{
    public IntegrationProvider Provider => IntegrationProvider.Basalam;
    public bool IsImplemented => true;
    public bool SupportsCredentialType(IntegrationCredentialType type) =>
        type is IntegrationCredentialType.OAuth2 or IntegrationCredentialType.BearerToken;

    public async Task<IReadOnlyCollection<ExternalCatalogItem>> ReadCatalogAsync(
        ExternalIntegrationConnection connection, CancellationToken cancellationToken)
    {
        Validate(connection);
        var vendorId = int.Parse(connection.AccountIdentifier, CultureInfo.InvariantCulture);
        var result = new List<ExternalCatalogItem>();

        for (var page = 1; page <= 10000; page++)
        {
            var products = await client.Catalog.GetProductsAsync(vendorId, page, 100, cancellationToken);
            if (products.Data.Count == 0) return result;

            foreach (var product in products.Data)
            {
                foreach (var variant in product.Variants)
                {
                    Add(new ExternalCatalogItem(
                        variant.Id.ToString(),
                        variant.Sku,
                        product.Name,
                        variant.Price,
                        variant.Stock ?? product.Stock ?? 0,
                        variant.Id.ToString()));
                }

                if (product.Variants.Count == 0)
                {
                    Add(new ExternalCatalogItem(
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

        void Add(ExternalCatalogItem item)
        {
            result.Add(item);
        }
    }

    public async Task PublishInventoryAsync(
        ExternalIntegrationConnection connection,
        IReadOnlyCollection<ExternalInventoryUpdate> updates,
        CancellationToken cancellationToken)
    {
        Validate(connection);

        foreach (var update in updates)
        {
            var externalProductId = int.Parse(update.ExternalProductId, CultureInfo.InvariantCulture);
            var product = await client.Catalog.GetProductAsync(externalProductId, cancellationToken);
            if (product is null)
                throw new IntegrationProviderException("InvalidExternalIdentifier", false);

            ValidateVendor(product, connection);

            if (update.VariantId is not null)
            {
                var variantId = int.Parse(update.VariantId, CultureInfo.InvariantCulture);
                var variation = await client.Variations.GetVariationAsync(variantId, cancellationToken);
                if (variation is null || variation.ProductId != externalProductId)
                    throw new IntegrationProviderException("VariantMismatch", false);
                ValidateVendor(variation, connection);
                await client.Variations.PatchStockAsync(variantId, (int)update.Quantity, cancellationToken);
            }
            else
            {
                await client.Products.PatchStockAsync(externalProductId, (int)update.Quantity, cancellationToken);
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

    private static void ValidateVendor(CatalogProduct product, ExternalIntegrationConnection connection)
    {
        if (product.VendorId != int.Parse(connection.AccountIdentifier, CultureInfo.InvariantCulture))
            throw new IntegrationProviderException("VendorMismatch", false);
    }

    private static void ValidateVendor(Variation variation, ExternalIntegrationConnection connection)
    {
        if (variation.VendorId != int.Parse(connection.AccountIdentifier, CultureInfo.InvariantCulture))
            throw new IntegrationProviderException("VendorMismatch", false);
    }

    private static void Identifier(string id)
    {
        if (!int.TryParse(id, NumberStyles.None, CultureInfo.InvariantCulture, out var value) || value <= 0)
            throw new IntegrationProviderException("InvalidExternalIdentifier", false);
    }
}
