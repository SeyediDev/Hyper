using Club.Domain.Features.Promotions;
using Club.Domain.Features;
using Moq;
using Xunit;

namespace Club.Domain.Tests.Features.Promotions;

/// <summary>
/// تست‌های AdvancedFormulaEvaluator برای Product.*
/// </summary>
public class AdvancedFormulaEvaluatorTests
{
    private readonly Mock<IFormulaContextProvider> _contextProviderMock;
    private readonly AdvancedFormulaEvaluator _evaluator;

    public AdvancedFormulaEvaluatorTests()
    {
        _contextProviderMock = new Mock<IFormulaContextProvider>();
        _evaluator = new AdvancedFormulaEvaluator(_contextProviderMock.Object);
    }

    [Fact]
    public async Task EvaluateAsync_WithProductCondition_ConvertsProductDotToUnderscore()
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

        var context = new Dictionary<string, object>
        {
            ["Product"] = new Dictionary<string, object>
            {
                ["Price"] = 1000000L,
                ["Quantity"] = 2L
            }
        };

        _contextProviderMock.Setup(x => x.BuildContextAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(context);

        // Act
        var result = await _evaluator.EvaluateAsync("Product.Price > 500000", request);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task EvaluateValueAsync_WithProductFormula_ReturnsCorrectValue()
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

        var context = new Dictionary<string, object>
        {
            ["Product"] = new Dictionary<string, object>
            {
                ["Price"] = 1000000L,
                ["Quantity"] = 2L,
                ["Discount"] = 0.1f
            }
        };

        _contextProviderMock.Setup(x => x.BuildContextAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(context);

        // Act
        var result = await _evaluator.EvaluateValueAsync("Product.Price * Product.Quantity", request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2000000L, Convert.ToInt64(result));
    }

    [Fact]
    public async Task EvaluateValueAsync_WithProductDiscountFormula_ReturnsCorrectValue()
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

        var context = new Dictionary<string, object>
        {
            ["Product"] = new Dictionary<string, object>
            {
                ["Price"] = 1000000L,
                ["Discount"] = 0.1f
            }
        };

        _contextProviderMock.Setup(x => x.BuildContextAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(context);

        // Act
        var result = await _evaluator.EvaluateValueAsync("Product.Price * (1 - Product.Discount)", request);

        // Assert
        Assert.NotNull(result);
        var expected = 1000000L * (1 - 0.1f);
        Assert.InRange(Convert.ToDouble(result), expected - 0.01, expected + 0.01);
    }
}

