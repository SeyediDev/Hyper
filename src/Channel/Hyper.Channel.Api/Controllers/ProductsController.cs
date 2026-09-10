using System.Diagnostics;
using Hyper.Channel.Api.Infrastructure.Telemetry;
using Hyper.Domain.Entities.Channels;
using Hyper.Domain.Entities.Products;
using Hyper.Domain.Features.Products;

namespace Hyper.Channel.Api.Controllers;

/// <summary>
/// کنترلر مدیریت محصولات (Products)
/// </summary>
[AppRoute("channel", "products")]
[Tags("products")]
public sealed class ProductsController(
    ILogger<ProductsController> logger,
    IQueryRepository<Product, int> productRepository,
    IQueryRepository<ProductCategory, int> productCategoryRepository,
    IQueryRepository<EventChannel, int> eventChannelRepository,
    IProductService productService
) : AppControllerBase
{
    /// <summary>
    /// دریافت ویژگی‌ها قابل استفاده برای یک محصول (از طریق ProductId)
    /// </summary>
    /// <param name="productId">شناسه محصول</param>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>لیست ویژگی‌ها قابل استفاده</returns>
    /// <response code="200">ویژگی‌ها با موفقیت دریافت شد</response>
    /// <response code="404">محصول یافت نشد</response>
    [HttpGet("{productId}/attributes")]
    [ProducesResponseType(typeof(List<ProductAttributeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<ProductAttributeDto>>> GetAttributes(
        int productId,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        using var activity = ChannelApiTelemetry.StartActivity("Products.GetAttributes");
        activity?.SetTag("product.id", productId.ToString());

        logger.LogInformation("Getting attributes for product {ProductId}", productId);

        try
        {
            ChannelApiTelemetry.RecordControllerRequest("Products", "GetAttributes", null);

            // بررسی وجود محصول
            var product = await productRepository.FirstOrDefaultAsync(
                x => x.Id == productId && !x.IsDeleted,
                cancellationToken);
            
            if (product == null)
            {
                stopwatch.Stop();
                activity?.SetStatus(ActivityStatusCode.Error, "Product not found");
                logger.LogWarning("Product with id {ProductId} not found. Duration: {Duration}ms", productId, stopwatch.ElapsedMilliseconds);
                ChannelApiTelemetry.RecordHttpRequest("GET", "products/{productId}/attributes", 404, stopwatch.Elapsed.TotalSeconds, null);
                return NotFound($"Product with id {productId} not found");
            }

            // دریافت ویژگی‌ها قابل استفاده
            var attributes = await productService.GetAvailableAttributesAsync(productId, cancellationToken);

            var result = attributes.Select(a => new ProductAttributeDto
            {
                Key = a.Key,
                Title = a.Title!,
                Reference = product.Title,
                ParameterType = a.ValueType.ToString(),
                IsOptional = a.IsOptional ?? true,
                DefaultValue = a.DefaultValue,
                AttributeType = a.Area.ToString()
            }).ToList();

            stopwatch.Stop();
            activity?.SetStatus(ActivityStatusCode.Ok);
            activity?.SetTag("attributes.count", result.Count.ToString());
            logger.LogInformation(
                "Successfully retrieved {Count} attributes for product {ProductId}. Duration: {Duration}ms",
                result.Count, productId, stopwatch.ElapsedMilliseconds);

            ChannelApiTelemetry.RecordHttpRequest("GET", "products/{productId}/attributes", 200, stopwatch.Elapsed.TotalSeconds, null);

            return Ok(result);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.SetTag("error.type", ex.GetType().Name);
            activity?.SetTag("error.message", ex.Message);
            activity?.SetTag("error.stacktrace", ex.StackTrace);
            
            logger.LogError(ex,
                "Error getting attributes for product {ProductId}. Duration: {Duration}ms",
                productId, stopwatch.ElapsedMilliseconds);

            ChannelApiTelemetry.RecordHttpRequest("GET", "products/{productId}/attributes", 500, stopwatch.Elapsed.TotalSeconds, null);
            
            throw;
        }
    }

    /// <summary>
    /// دریافت ویژگی‌ها قابل استفاده برای یک محصول (از طریق ProductKey)
    /// </summary>
    /// <param name="productKey">کلید محصول</param>
    /// <param name="channelKey">کلید کانال (برای دریافت TenantId)</param>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>لیست ویژگی‌ها قابل استفاده</returns>
    /// <response code="200">ویژگی‌ها با موفقیت دریافت شد</response>
    /// <response code="404">محصول یافت نشد</response>
    [HttpGet("by-key/{productKey}/attributes")]
    [ProducesResponseType(typeof(List<ProductAttributeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<ProductAttributeDto>>> GetAttributesByKey(
        string productKey,
        [FromQuery] string? channelKey = null,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        using var activity = ChannelApiTelemetry.StartActivity("Products.GetAttributesByKey");
        activity?.SetTag("product.key", productKey);
        activity?.SetTag("channel.key", channelKey ?? "null");

        logger.LogInformation(
            "Getting attributes for product {ProductKey} from channel {ChannelKey}",
            productKey, channelKey);

        try
        {
            ChannelApiTelemetry.RecordControllerRequest("Products", "GetAttributesByKey", channelKey);

            // اگر channelKey موجود باشد، از آن برای دریافت TenantId استفاده می‌کنیم
            int? tenantId = null;
            if (!string.IsNullOrWhiteSpace(channelKey))
            {
                var eventChannel = await eventChannelRepository.FirstOrDefaultAsync(
                    x => x.Key == channelKey && !x.IsDeleted,
                    cancellationToken);
                
                if (eventChannel != null)
                {
                    tenantId = eventChannel.TenantId;
                    activity?.SetTag("tenant.id", tenantId.ToString());
                }
            }

            // بررسی وجود محصول
            var product = await productRepository.FirstOrDefaultAsync(
                x => x.Key == productKey && !x.IsDeleted && (tenantId == null || x.TenantId == tenantId),
                cancellationToken);
            
            if (product == null)
            {
                stopwatch.Stop();
                activity?.SetStatus(ActivityStatusCode.Error, "Product not found");
                logger.LogWarning(
                    "Product with key {ProductKey} not found in channel {ChannelKey}. Duration: {Duration}ms",
                    productKey, channelKey, stopwatch.ElapsedMilliseconds);
                ChannelApiTelemetry.RecordHttpRequest("GET", "products/by-key/{productKey}/attributes", 404, stopwatch.Elapsed.TotalSeconds, channelKey);
                return NotFound($"Product with key {productKey} not found");
            }

            // دریافت ویژگی‌ها قابل استفاده
            var attributes = await productService.GetAvailableAttributesAsync(product.Id, cancellationToken);

            var result = attributes.Select(a => new ProductAttributeDto
            {
                Key = a.Key,
                Title = a.Title!,
                Reference = product.Title,
                ParameterType = a.ValueType.ToString(),
                IsOptional = a.IsOptional ?? true,
                DefaultValue = a.DefaultValue,
                AttributeType = a.Area.ToString()
            }).ToList();

            stopwatch.Stop();
            activity?.SetStatus(ActivityStatusCode.Ok);
            activity?.SetTag("attributes.count", result.Count.ToString());
            logger.LogInformation(
                "Successfully retrieved {Count} attributes for product {ProductKey} from channel {ChannelKey}. Duration: {Duration}ms",
                result.Count, productKey, channelKey, stopwatch.ElapsedMilliseconds);

            ChannelApiTelemetry.RecordHttpRequest("GET", "products/by-key/{productKey}/attributes", 200, stopwatch.Elapsed.TotalSeconds, channelKey);

            return Ok(result);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.SetTag("error.type", ex.GetType().Name);
            activity?.SetTag("error.message", ex.Message);
            activity?.SetTag("error.stacktrace", ex.StackTrace);
            
            logger.LogError(ex,
                "Error getting attributes for product {ProductKey} from channel {ChannelKey}. Duration: {Duration}ms",
                productKey, channelKey, stopwatch.ElapsedMilliseconds);

            ChannelApiTelemetry.RecordHttpRequest("GET", "products/by-key/{productKey}/attributes", 500, stopwatch.Elapsed.TotalSeconds, channelKey);
            
            throw;
        }
    }

    /// <summary>
    /// دریافت درختواره محصولات (دسته‌بندی‌ها و محصولات)
    /// </summary>
    /// <param name="tenantId">شناسه اکوسیستم (اختیاری)</param>
    /// <param name="channelKey">کلید کانال (برای دریافت TenantId - اختیاری)</param>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>درختواره محصولات</returns>
    /// <response code="200">درختواره با موفقیت دریافت شد</response>
    [HttpGet("tree")]
    [ProducesResponseType(typeof(List<ProductTreeItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ProductTreeItemDto>>> GetTree(
        [FromQuery] int? tenantId = null,
        [FromQuery] string? channelKey = null,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        using var activity = ChannelApiTelemetry.StartActivity("Products.GetTree");
        activity?.SetTag("tenant.id", tenantId?.ToString() ?? "null");
        activity?.SetTag("channel.key", channelKey ?? "null");

        logger.LogInformation(
            "Getting product tree for tenant {TenantId} from channel {ChannelKey}",
            tenantId, channelKey);

        try
        {
            ChannelApiTelemetry.RecordControllerRequest("Products", "GetTree", channelKey);

            // اگر channelKey موجود باشد، از آن برای دریافت TenantId استفاده می‌کنیم
            if (tenantId == null && !string.IsNullOrWhiteSpace(channelKey))
            {
                var eventChannel = await eventChannelRepository.FirstOrDefaultAsync(
                    x => x.Key == channelKey && !x.IsDeleted,
                    cancellationToken);
                
                if (eventChannel != null)
                {
                    tenantId = eventChannel.TenantId;
                    activity?.SetTag("tenant.id", tenantId.ToString());
                }
            }

            if (tenantId == null)
            {
                stopwatch.Stop();
                activity?.SetStatus(ActivityStatusCode.Error, "TenantId is required");
                logger.LogWarning("TenantId is required for product tree. Duration: {Duration}ms", stopwatch.ElapsedMilliseconds);
                ChannelApiTelemetry.RecordHttpRequest("GET", "products/tree", 400, stopwatch.Elapsed.TotalSeconds, channelKey);
                return BadRequest("TenantId is required. Provide either tenantId query parameter or channelKey.");
            }

            // دریافت تمام دسته‌بندی‌های فعال
            var categories = (await productCategoryRepository.GetAllAsync(
                cancellationToken,
                x => x.TenantId == tenantId && !x.IsDeleted && x.IsActive,
                q => q.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Title))).ToList();

            // دریافت تمام محصولات فعال
            var products = await productRepository.GetAllAsync(
                cancellationToken,
                x => x.TenantId == tenantId && !x.IsDeleted && x.IsActive,
                q => q.OrderBy(p => p.Title));
            if(products.Any(x=>x.ProductCategoryId is null or 0))
            {
                categories.Add(new ProductCategory() { Id = 0, Title = "طبقه بندی نشده" });
                foreach (var product in products.Where(x=>x.ProductCategoryId is null or 0))
                {
                    product.ProductCategoryId = 0;
                }
            }

            // ساخت درختواره
            var tree = BuildProductTree(categories, [.. products]);

            stopwatch.Stop();
            activity?.SetStatus(ActivityStatusCode.Ok);
            activity?.SetTag("tree.items.count", tree.Count.ToString());
            logger.LogInformation(
                "Successfully retrieved product tree for tenant {TenantId}. Items: {Count}. Duration: {Duration}ms",
                tenantId, tree.Count, stopwatch.ElapsedMilliseconds);

            ChannelApiTelemetry.RecordHttpRequest("GET", "products/tree", 200, stopwatch.Elapsed.TotalSeconds, channelKey);

            return Ok(tree);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.SetTag("error.type", ex.GetType().Name);
            activity?.SetTag("error.message", ex.Message);
            activity?.SetTag("error.stacktrace", ex.StackTrace);
            
            logger.LogError(ex,
                "Error getting product tree for tenant {TenantId} from channel {ChannelKey}. Duration: {Duration}ms",
                tenantId, channelKey, stopwatch.ElapsedMilliseconds);

            ChannelApiTelemetry.RecordHttpRequest("GET", "products/tree", 500, stopwatch.Elapsed.TotalSeconds, channelKey);
            
            throw;
        }
    }

    private static List<ProductTreeItemDto> BuildProductTree(
        List<ProductCategory> categories,
        List<Product> products)
    {
        var categoryDict = categories.ToDictionary(c => c.Id);
        var productDict = products.GroupBy(p => p.ProductCategoryId!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());

        var rootCategories = categories.Where(c => c.ParentCategoryId == null).ToList();
        var result = new List<ProductTreeItemDto>();

        foreach (var category in rootCategories)
        {
            result.Add(BuildCategoryTreeItem(category, categoryDict, productDict));
        }

        return result;
    }

    private static ProductTreeItemDto BuildCategoryTreeItem(
        ProductCategory category,
        Dictionary<int, ProductCategory> categoryDict,
        Dictionary<int, List<Product>> productDict)
    {
        var item = new ProductTreeItemDto
        {
            Id = category.Id,
            Key = category.Key,
            Title = category.Title,
            Type = "category",
            ParentId = category.ParentCategoryId,
            Children = []
        };

        // اضافه کردن دسته‌بندی‌های فرزند
        var childCategories = categoryDict.Values
            .Where(c => c.ParentCategoryId == category.Id)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Title);

        foreach (var childCategory in childCategories)
        {
            item.Children!.Add(BuildCategoryTreeItem(childCategory, categoryDict, productDict));
        }

        // اضافه کردن محصولات این دسته‌بندی
        if (productDict.TryGetValue(category.Id, out var categoryProducts))
        {
            foreach (var product in categoryProducts)
            {
                item.Children!.Add(new ProductTreeItemDto
                {
                    Id = product.Id,
                    Key = product.Key,
                    Title = product.Title,
                    Type = "product",
                    ParentId = category.Id
                });
            }
        }

        return item;
    }
}

public sealed record ProductAttributeDto
{
    public string Key { get; init; } = null!;
    public string Title { get; init; } = null!;
    public string Reference { get; init; } = null!;
    public string ParameterType { get; init; } = null!;
    public bool IsOptional { get; init; }
    public string? DefaultValue { get; init; }
    public string AttributeType { get; init; } = null!; // Tenant, Category, Product
}

public sealed record ProductTreeItemDto
{
    public int Id { get; init; }
    public string Key { get; init; } = null!;
    public string Title { get; init; } = null!;
    public string Type { get; init; } = null!; // "category" or "product"
    public int? ParentId { get; init; }
    public List<ProductTreeItemDto>? Children { get; init; }
}