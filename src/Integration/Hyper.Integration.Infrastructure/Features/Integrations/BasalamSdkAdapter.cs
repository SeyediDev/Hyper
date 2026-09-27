using System.Globalization;
using System.Text.Json;
using Basalam.SDK;
using Basalam.SDK.Clients;
using Basalam.SDK.Models;
using Basalam.SDK.Errors;
using Hyper.Integration.Domain.Entities.Integrations;
using Hyper.Integration.Domain.Features.Integrations;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed class BasalamSdkAdapter(IBasalamClient client, BasalamOAuthStore tokenStore) : IExternalIntegrationAdapter, IExternalProductPublisher
{
    public IntegrationProvider Provider => IntegrationProvider.Basalam;
    public bool IsImplemented => true;
    public bool SupportsCredentialType(IntegrationCredentialType type) =>
        type is IntegrationCredentialType.OAuth2 or IntegrationCredentialType.BearerToken;

    public Task<IReadOnlyCollection<ExternalCatalogItem>> ReadCatalogAsync(
        ExternalIntegrationConnection connection, CancellationToken cancellationToken) =>
        ProviderCall(async () =>
        {
            try { return await ReadCatalogCoreAsync(connection, cancellationToken); }
            catch (JsonException) { throw new IntegrationProviderException("CatalogInvalidResponse", false); }
        });

    private async Task<IReadOnlyCollection<ExternalCatalogItem>> ReadCatalogCoreAsync(
        ExternalIntegrationConnection connection, CancellationToken cancellationToken)
    {
        Validate(connection);
        await SetTokenAsync(connection, cancellationToken);
        var vendorId = int.Parse(connection.AccountIdentifier, CultureInfo.InvariantCulture);
        var result = new List<ExternalCatalogItem>();
        var productIds = new HashSet<int>();
        var variantIds = new HashSet<int>();

        for (var page = 1; page <= 10000; page++)
        {
            var products = await client.Catalog.GetProductsAsync(vendorId, page, 100, cancellationToken);
            if (products.Page != page || (products.Data.Count == 0 && products.HasMore))
                throw new IntegrationProviderException("CatalogInvalidResponse", false);
            if (products.Data.Count == 0) return result;

            foreach (var product in products.Data)
            {
                ValidateVendor(product, connection);
                if (product.Id <= 0 || !productIds.Add(product.Id))
                    throw new IntegrationProviderException("CatalogIdentityConflict", false);
                foreach (var variant in product.Variants)
                {
                    if (variant.Id <= 0 || !variantIds.Add(variant.Id)
                        || variant.ProductId != product.Id || variant.VendorId != vendorId)
                        throw new IntegrationProviderException("CatalogIdentityConflict", false);
                    result.Add(new ExternalCatalogItem(
                        // A Basalam variation is a sellable child of the product.
                        // Keep the parent product id as ExternalProductId and use
                        // the variation id as the discriminator; otherwise stock
                        // publishing would try to load the variation as a product.
                        product.Id.ToString(CultureInfo.InvariantCulture),
                        variant.Sku,
                        string.IsNullOrWhiteSpace(variant.Title)
                            ? product.Name
                            : $"{product.Name} - {variant.Title}",
                        variant.Price ?? product.Price,
                        variant.Stock,
                        variant.Id.ToString(CultureInfo.InvariantCulture)));
                }

                if (product.Variants.Count == 0)
                {
                    result.Add(new ExternalCatalogItem(
                        product.Id.ToString(CultureInfo.InvariantCulture),
                        product.Sku,
                        product.Name,
                        product.Price,
                        product.Stock,
                        null));
                }
            }

            if (products.HasMore == false) return result;
        }

        throw new IntegrationProviderException("CatalogLimitExceeded", false);
    }

    public Task PublishInventoryAsync(ExternalIntegrationConnection connection,
        IReadOnlyCollection<ExternalInventoryUpdate> updates, CancellationToken cancellationToken) =>
        ProviderCall(async () => { await PublishInventoryCoreAsync(connection, updates, cancellationToken); return true; });

    private async Task PublishInventoryCoreAsync(
        ExternalIntegrationConnection connection,
        IReadOnlyCollection<ExternalInventoryUpdate> updates,
        CancellationToken cancellationToken)
    {
        Validate(connection);
        foreach (var update in updates)
        {
            Identifier(update.ExternalProductId);
            if (update.VariantId is not null) Identifier(update.VariantId);
            if (update.Quantity < 0 || update.Quantity > int.MaxValue || decimal.Truncate(update.Quantity) != update.Quantity)
                throw new IntegrationProviderException("InvalidInventoryQuantity", false);
        }
        await SetTokenAsync(connection, cancellationToken);

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

    public Task PublishProductAsync(ExternalIntegrationConnection connection, ExternalProductUpdate update, CancellationToken ct) =>
        ProviderCall(async () =>
        {
            Validate(connection);
            if (update.ValidationError() is { } error) throw new IntegrationProviderException(error, false);
            Identifier(update.ExternalProductId);
            await SetTokenAsync(connection, ct);
            var id = int.Parse(update.ExternalProductId, CultureInfo.InvariantCulture);
            var product = await client.Catalog.GetProductAsync(id, ct)
                ?? throw new IntegrationProviderException("InvalidExternalIdentifier", false);
            ValidateVendor(product, connection);
            await client.Products.PatchDetailsAsync(id, update.Title,
                update.PrimaryPrice is { } price ? checked((long)price) : null, ct);
            return true;
        });

    private static async Task<T> ProviderCall<T>(Func<Task<T>> action)
    {
        try { return await action(); }
        catch (BasalamAPIError error)
        {
            // Provider response bodies can contain sensitive data. Persist only
            // a stable code understood by queue retry/dead-letter processing.
            throw new IntegrationProviderException($"Http{error.StatusCode}",
                error.StatusCode is 408 or 429 || error.StatusCode >= 500, error.RetryAfter);
        }
        catch (BasalamValidationError) { throw new IntegrationProviderException("ProviderValidation", false); }
    }

    private async Task SetTokenAsync(ExternalIntegrationConnection connection, CancellationToken ct)
    {
        var token = await tokenStore.GetTokenAsync(connection, ct);
        if (token is null) throw new IntegrationProviderException("InvalidCredentials", false);
        client.SetToken(token);
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

