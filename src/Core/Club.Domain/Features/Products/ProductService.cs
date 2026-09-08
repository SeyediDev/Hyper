namespace Hyper.Domain.Features.Products;

public interface IProductService
{
    [Telemetry]
    Task<Product?> GetAsync(string productKey, CancellationToken cancellationToken);

    [Telemetry]
    Task<ProductCategory?> GetProductCategoryAsync(string productCategoryKey, CancellationToken cancellationToken);
    [Telemetry]
    Task<Product?> GetAsync(int productId, CancellationToken cancellationToken);

    Task<AttributeDto?> GetAttributeAsync(int productId, string attributeKey, CancellationToken cancellationToken);

    /// <summary>
    /// دریافت تمام ویژگی‌ها قابل استفاده برای یک محصول (از همه لایه‌ها)
    /// </summary>
    Task<List<AttributeDto>> GetAvailableAttributesAsync(int productId, CancellationToken cancellationToken);
}

public record TenantAttributeInfoDto
{
    public string Key { get; init; } = null!;
    public string Title { get; init; } = null!;
    public TenantAttributeType AttributeType { get; init; }
    public bool? IsOptional { get; init; }
    public string? DefaultValue { get; init; }
    public int AttributeId { get; init; }
}

internal class ProductService(
      IAttributeService attributeService
    , IQueryRepository<ProductCategory, int> productCategoryQuery
    , ICommandRepository<Product, int> productCommand
    ) : IProductService
{
    public async Task<Product?> GetAsync(string productKey, CancellationToken cancellationToken)
    {
        Product? product = await productCommand.FirstOrDefaultAsync(x => x.Key == productKey, cancellationToken);
        return product;
    }

    public async Task<ProductCategory?> GetProductCategoryAsync(string productCategoryKey, CancellationToken cancellationToken)
    {
        ProductCategory? productCategory = await productCategoryQuery.FirstOrDefaultAsync(x => x.Key == productCategoryKey, cancellationToken);
        return productCategory;
    }

    public async Task<Product?> GetAsync(int productId, CancellationToken cancellationToken)
    {
        Product? product = await productCommand.FirstOrDefaultAsync(x => x.Id == productId, cancellationToken);
        return product;
    }

    public async Task<AttributeDto?> GetAttributeAsync(int productId, string attributeKey, CancellationToken cancellationToken)
    {
        // ابتدا محصول را دریافت می‌کنیم تا TenantId و CategoryId را داشته باشیم
        Product? product = await productCommand.FirstOrDefaultAsync(
            x => x.Id == productId, cancellationToken);

        if (product == null)
        {
            return null;
        }

        AttributeDto? attribute = await attributeService.GetAttributeAsync(product.TenantId,
            AttributeArea.Product, attributeKey, productId, null, cancellationToken);

        if (attribute != null)
        {
            return attribute;
        }

        // 2. بررسی ویژگی‌ها سطح Category (اولویت دوم)
        int? categoryId = product.ProductCategoryId;
        while (categoryId!=null && attribute == null)
        {
            attribute = await attributeService.GetAttributeAsync(product.TenantId,
                AttributeArea.Product, attributeKey, null, categoryId, cancellationToken);
            if(attribute==null)
            {
                categoryId = (await productCategoryQuery.GetByIdAsync(categoryId.Value, cancellationToken))?.ParentCategoryId;
            }
        }
        attribute ??= await attributeService.AddAttributeAsync(product.TenantId,
            AttributeArea.Product, attributeKey, productId, null, attributeKey, null, cancellationToken);
        return attribute;
    }

    public async Task<List<AttributeDto>> GetAvailableAttributesAsync(int productId, CancellationToken cancellationToken)
    {
        Product? product = await GetAsync(productId, cancellationToken);
        if (product == null)
        {
            return [];
        }
        var attributes = await attributeService.GetAttributesAsync(product.TenantId, AttributeArea.Product, productId, product.ProductCategoryId, cancellationToken);
        return attributes;
    }
}