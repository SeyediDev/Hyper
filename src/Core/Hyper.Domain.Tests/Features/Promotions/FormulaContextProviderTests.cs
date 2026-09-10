using Hyper.Domain.Entities.Customers;
using Hyper.Domain.Entities.Products;
using Hyper.Domain.Entities.Products.Enums;
using Hyper.Domain.Features.Promotions;
using Hyper.Domain.Features;
using Moq;
using Neo.Domain;
using Xunit;

namespace Hyper.Domain.Tests.Features.Promotions;

/// <summary>
/// تست‌های FormulaContextProvider برای Product Context
/// </summary>
public class FormulaContextProviderTests
{
    private readonly Mock<ICustomerService> _customerServiceMock;
    private readonly Mock<IQueryRepository<TenantConfig, int>> _tenantConfigRepoMock;
    private readonly Mock<IEventService> _eventServiceMock;
    private readonly Mock<IQueryRepository<ProductAttributeValue, long>> _productAttributeValueRepoMock;
    private readonly Mock<IQueryRepository<ProductAttribute, int>> _productAttributeRepoMock;
    private readonly Mock<IQueryRepository<TenantProductAttribute, int>> _tenantProductAttributeRepoMock;
    private readonly Mock<IQueryRepository<ProductCategoryAttribute, int>> _productCategoryAttributeRepoMock;
    private readonly Mock<IQueryRepository<Product, int>> _productRepoMock;
    private readonly FormulaContextProvider _provider;

    public FormulaContextProviderTests()
    {
        _customerServiceMock = new Mock<ICustomerService>();
        _tenantConfigRepoMock = new Mock<IQueryRepository<TenantConfig, int>>();
        _eventServiceMock = new Mock<IEventService>();
        _productAttributeValueRepoMock = new Mock<IQueryRepository<ProductAttributeValue, long>>();
        _productAttributeRepoMock = new Mock<IQueryRepository<ProductAttribute, int>>();
        _tenantProductAttributeRepoMock = new Mock<IQueryRepository<TenantProductAttribute, int>>();
        _productCategoryAttributeRepoMock = new Mock<IQueryRepository<ProductCategoryAttribute, int>>();
        _productRepoMock = new Mock<IQueryRepository<Product, int>>();

        _provider = new FormulaContextProvider(
            _customerServiceMock.Object,
            _tenantConfigRepoMock.Object,
            _eventServiceMock.Object,
            _productAttributeValueRepoMock.Object,
            _productAttributeRepoMock.Object,
            _tenantProductAttributeRepoMock.Object,
            _productCategoryAttributeRepoMock.Object,
            _productRepoMock.Object);
    }

    [Fact]
    public async Task BuildProductContextAsync_WhenProductIdIsNull_ReturnsEmptyContext()
    {
        // Arrange
        var customer = new Customer { Id = 1 };
        var request = new PromotionProcessingRequest(
            TenantId: 1,
            EventLogId: 1,
            EventChannelId: 1,
            EventTypeId: 1,
            Customer: customer,
            Parameters: null)
        {
            ProductId = null
        };

        // Act
        var context = await _provider.BuildContextAsync(request);

        // Assert
        Assert.NotNull(context);
        Assert.True(context.ContainsKey("Product"));
        var productContext = context["Product"] as Dictionary<string, object>;
        Assert.NotNull(productContext);
        Assert.Empty(productContext);
    }

    [Fact]
    public async Task BuildProductContextAsync_WhenProductNotFound_ReturnsEmptyContext()
    {
        // Arrange
        var customer = new Customer { Id = 1 };
        var request = new PromotionProcessingRequest(
            TenantId: 1,
            EventLogId: 1,
            EventChannelId: 1,
            EventTypeId: 1,
            Customer: customer,
            Parameters: null)
        {
            ProductId = 1
        };

        _productRepoMock.Setup(x => x.FirstOrDefaultAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var context = await _provider.BuildContextAsync(request);

        // Assert
        Assert.NotNull(context);
        Assert.True(context.ContainsKey("Product"));
        var productContext = context["Product"] as Dictionary<string, object>;
        Assert.NotNull(productContext);
        Assert.Empty(productContext);
    }

    [Fact]
    public async Task BuildProductContextAsync_WithProductAttributes_ReturnsContextWithAttributes()
    {
        // Arrange
        var customer = new Customer { Id = 1 };
        var request = new PromotionProcessingRequest(
            TenantId: 1,
            EventLogId: 1,
            EventChannelId: 1,
            EventTypeId: 1,
            Customer: customer,
            Parameters: null)
        {
            ProductId = 1
        };

        var product = new Product
        {
            Id = 1,
            TenantId = 1,
            CategoryId = 1
        };

        var productAttribute = new ProductAttribute
        {
            Id = 1,
            ProductId = 1,
            Key = "Price",
            Title = "قیمت",
            ParameterType = ParameterType.Long,
            DefaultValue = "0"
        };

        var productAttributeValue = new ProductAttributeValue
        {
            Id = 1,
            EventLogId = 1,
            AttributeType = ProductAttributeType.Product,
            ProductAttributeId = 1,
            Value = "1000000"
        };

        _productRepoMock.Setup(x => x.FirstOrDefaultAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _productAttributeValueRepoMock.Setup(x => x.Query())
            .Returns(new List<ProductAttributeValue> { productAttributeValue }.AsQueryable());

        _productAttributeRepoMock.Setup(x => x.Query())
            .Returns(new List<ProductAttribute> { productAttribute }.AsQueryable());

        _tenantProductAttributeRepoMock.Setup(x => x.Query())
            .Returns(new List<TenantProductAttribute>().AsQueryable());

        _productCategoryAttributeRepoMock.Setup(x => x.Query())
            .Returns(new List<ProductCategoryAttribute>().AsQueryable());

        // Act
        var context = await _provider.BuildContextAsync(request);

        // Assert
        Assert.NotNull(context);
        Assert.True(context.ContainsKey("Product"));
        var productContext = context["Product"] as Dictionary<string, object>;
        Assert.NotNull(productContext);
        Assert.True(productContext.ContainsKey("Price"));
        Assert.Equal(1000000L, productContext["Price"]);
    }

    [Fact]
    public async Task BuildProductContextAsync_WithTenantAttributes_ReturnsContextWithTenantAttributes()
    {
        // Arrange
        var customer = new Customer { Id = 1 };
        var request = new PromotionProcessingRequest(
            TenantId: 1,
            EventLogId: 1,
            EventChannelId: 1,
            EventTypeId: 1,
            Customer: customer,
            Parameters: null)
        {
            ProductId = 1
        };

        var product = new Product
        {
            Id = 1,
            TenantId = 1,
            CategoryId = null
        };

        var tenantAttribute = new TenantProductAttribute
        {
            Id = 1,
            TenantId = 1,
            Key = "Discount",
            Title = "تخفیف",
            ParameterType = ParameterType.Float,
            DefaultValue = "0.0"
        };

        var productAttributeValue = new ProductAttributeValue
        {
            Id = 1,
            EventLogId = 1,
            AttributeType = ProductAttributeType.Tenant,
            TenantAttributeId = 1,
            Value = "0.1"
        };

        _productRepoMock.Setup(x => x.FirstOrDefaultAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _productAttributeValueRepoMock.Setup(x => x.Query())
            .Returns(new List<ProductAttributeValue> { productAttributeValue }.AsQueryable());

        _productAttributeRepoMock.Setup(x => x.Query())
            .Returns(new List<ProductAttribute>().AsQueryable());

        _tenantProductAttributeRepoMock.Setup(x => x.Query())
            .Returns(new List<TenantProductAttribute> { tenantAttribute }.AsQueryable());

        _productCategoryAttributeRepoMock.Setup(x => x.Query())
            .Returns(new List<ProductCategoryAttribute>().AsQueryable());

        // Act
        var context = await _provider.BuildContextAsync(request);

        // Assert
        Assert.NotNull(context);
        Assert.True(context.ContainsKey("Product"));
        var productContext = context["Product"] as Dictionary<string, object>;
        Assert.NotNull(productContext);
        Assert.True(productContext.ContainsKey("Discount"));
        Assert.Equal(0.1f, productContext["Discount"]);
    }
}

