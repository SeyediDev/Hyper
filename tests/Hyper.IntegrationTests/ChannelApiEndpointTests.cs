using System.Net;
using System.Net.Http.Json;
using Hyper.Channel.Api.Controllers;
using Hyper.Channel.Api.Controllers.Dto;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Program = Hyper.Channel.Api.Program;

namespace Hyper.IntegrationTests;

/// <summary>
/// تست‌های Integration برای API های Channel
/// این تست‌ها endpoint های API را مستقیماً تست می‌کنند
/// </summary>
[Collection("Integration Tests")]
public class ChannelApiEndpointTests : IClassFixture<HyperChannelWebApplicationFactory>
{
    private readonly HttpClient _httpClient;
    private const int TenantId = 1;
    private const string ChannelKey = "bajet";
    private const string CustomerMobile = "09123456789";

    public ChannelApiEndpointTests(HyperChannelWebApplicationFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    #region EventsController Tests

    /// <summary>
    /// تست POST /api/v1/channel/events - ارسال یک رویداد
    /// </summary>
    [Fact]
    public async Task CreateEvent_WithValidRequest_ShouldReturnAccepted()
    {
        // Arrange
        var request = new ChannelAddEventRequestDto
        {
            CustomerMobile = CustomerMobile,
            EventTypeKey = "purchase",
            Parameters = new Dictionary<string, string> { { "amount", "100000" } },
            ProductCategoryKey = "electronics",
            ProductKey = "laptop-001",
            IdempotencyKey = Guid.NewGuid().ToString()
        };

        _httpClient.DefaultRequestHeaders.Add("X-Channel-Key", ChannelKey);

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/channel/events", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        var result = await response.Content.ReadFromJsonAsync<OutboxResponseDto>();
        result.Should().NotBeNull();
        result!.OutboxId.Should().BeGreaterThan(0);
        result.State.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// تست POST /api/v1/channel/events - بدون ChannelKey در header
    /// </summary>
    [Fact]
    public async Task CreateEvent_WithoutChannelKey_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new ChannelAddEventRequestDto
        {
            CustomerMobile = CustomerMobile,
            EventTypeKey = "purchase",
            Parameters = new Dictionary<string, string> { { "amount", "100000" } }
        };

        _httpClient.DefaultRequestHeaders.Remove("X-Channel-Key");

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/channel/events", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// تست POST /api/v1/channel/events - با ChannelKey نامعتبر
    /// </summary>
    [Fact]
    public async Task CreateEvent_WithInvalidChannelKey_ShouldReturnNotFound()
    {
        // Arrange
        var request = new ChannelAddEventRequestDto
        {
            CustomerMobile = CustomerMobile,
            EventTypeKey = "purchase",
            Parameters = new Dictionary<string, string> { { "amount", "100000" } }
        };

        _httpClient.DefaultRequestHeaders.Remove("X-Channel-Key");
        _httpClient.DefaultRequestHeaders.Add("X-Channel-Key", "invalid-channel-key");

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/channel/events", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// تست POST /api/v1/channel/events - بدون IdempotencyKey (باید CorrelationId استفاده شود)
    /// </summary>
    [Fact]
    public async Task CreateEvent_WithoutIdempotencyKey_ShouldUseCorrelationId()
    {
        // Arrange
        var request = new ChannelAddEventRequestDto
        {
            CustomerMobile = CustomerMobile,
            EventTypeKey = "purchase",
            Parameters = new Dictionary<string, string> { { "amount", "100000" } }
        };

        _httpClient.DefaultRequestHeaders.Remove("X-Channel-Key");
        _httpClient.DefaultRequestHeaders.Add("X-Channel-Key", ChannelKey);
        _httpClient.DefaultRequestHeaders.Add("X-Correlation-ID", "test-correlation-id-123");

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/channel/events", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        var result = await response.Content.ReadFromJsonAsync<OutboxResponseDto>();
        result.Should().NotBeNull();
    }

    /// <summary>
    /// تست POST /api/v1/channel/events/bulk - ارسال چندین رویداد
    /// </summary>
    [Fact]
    public async Task CreateBulkEvents_WithValidRequest_ShouldReturnAccepted()
    {
        // Arrange
        var request = new BulkEventRequestDto
        {
            Events = new List<ChannelAddEventRequestDto>
            {
                new()
                {
                    CustomerMobile = CustomerMobile,
                    EventTypeKey = "purchase",
                    Parameters = new Dictionary<string, string> { { "amount", "100000" } },
                    IdempotencyKey = Guid.NewGuid().ToString()
                },
                new()
                {
                    CustomerMobile = "09123456790",
                    EventTypeKey = "purchase",
                    Parameters = new Dictionary<string, string> { { "amount", "200000" } },
                    IdempotencyKey = Guid.NewGuid().ToString()
                }
            }
        };

        _httpClient.DefaultRequestHeaders.Remove("X-Channel-Key");
        _httpClient.DefaultRequestHeaders.Add("X-Channel-Key", ChannelKey);

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/channel/events/bulk", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        var result = await response.Content.ReadFromJsonAsync<BulkEventResponseDto>();
        result.Should().NotBeNull();
        result!.Results.Should().HaveCount(2);
        result.Results.All(r => r.OutboxId > 0).Should().BeTrue();
    }

    /// <summary>
    /// تست POST /api/v1/channel/events/bulk - با لیست خالی
    /// </summary>
    [Fact]
    public async Task CreateBulkEvents_WithEmptyList_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new BulkEventRequestDto
        {
            Events = new List<ChannelAddEventRequestDto>()
        };

        _httpClient.DefaultRequestHeaders.Remove("X-Channel-Key");
        _httpClient.DefaultRequestHeaders.Add("X-Channel-Key", ChannelKey);

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/channel/events/bulk", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region ChannelsController Tests

    /// <summary>
    /// تست GET /api/v1/channel/channels/{channelKey} - دریافت کانال با کلید
    /// </summary>
    [Fact]
    public async Task GetChannelByKey_WithValidKey_ShouldReturnOk()
    {
        // Act
        var response = await _httpClient.GetAsync($"/api/v1/channel/channels/{ChannelKey}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<EventChannelDto>();
        result.Should().NotBeNull();
        result!.Key.Should().Be(ChannelKey);
        result.Id.Should().BeGreaterThan(0);
        result.TenantId.Should().BeGreaterThan(0);
    }

    /// <summary>
    /// تست GET /api/v1/channel/channels/{channelKey} - با کلید نامعتبر
    /// </summary>
    [Fact]
    public async Task GetChannelByKey_WithInvalidKey_ShouldReturnNotFound()
    {
        // Act
        var response = await _httpClient.GetAsync("/api/v1/channel/channels/invalid-key");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// تست GET /api/v1/channel/channels - دریافت همه کانال‌ها
    /// </summary>
    [Fact]
    public async Task GetAllChannels_WithoutTenantId_ShouldReturnOk()
    {
        // Act
        var response = await _httpClient.GetAsync("/api/v1/channel/channels");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<EventChannelDto>>();
        result.Should().NotBeNull();
        result!.Should().NotBeEmpty();
    }

    /// <summary>
    /// تست GET /api/v1/channel/channels?tenantId={tenantId} - دریافت کانال‌های یک Tenant
    /// </summary>
    [Fact]
    public async Task GetAllChannels_WithTenantId_ShouldReturnFilteredChannels()
    {
        // Act
        var response = await _httpClient.GetAsync($"/api/v1/channel/channels?tenantId={TenantId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<EventChannelDto>>();
        result.Should().NotBeNull();
        result!.All(c => c.TenantId == TenantId).Should().BeTrue();
    }

    #endregion

    #region ProductsController Tests

    /// <summary>
    /// تست GET /api/v1/channel/products/{productKey}/price - دریافت قیمت محصول
    /// </summary>
    [Fact]
    public async Task GetProductPrice_WithValidRequest_ShouldReturnOk()
    {
        // Arrange
        var productKey = "laptop-001";

        // Act
        var response = await _httpClient.GetAsync($"/api/v1/channel/products/{productKey}/price?channelKey={ChannelKey}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ProductPriceDto>();
        result.Should().NotBeNull();
        result!.ProductKey.Should().Be(productKey);
        result.Currency.Should().Be("IRR");
    }

    /// <summary>
    /// تست GET /api/v1/channel/products/{productKey}/price - با customerId
    /// </summary>
    [Fact]
    public async Task GetProductPrice_WithCustomerId_ShouldReturnOk()
    {
        // Arrange
        var productKey = "laptop-001";
        var customerId = "customer-123";

        // Act
        var response = await _httpClient.GetAsync($"/api/v1/channel/products/{productKey}/price?channelKey={ChannelKey}&customerId={customerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ProductPriceDto>();
        result.Should().NotBeNull();
        result!.ProductKey.Should().Be(productKey);
    }

    /// <summary>
    /// تست GET /api/v1/channel/products/{productKey}/clv - دریافت CLV محصول
    /// </summary>
    [Fact]
    public async Task GetProductClv_WithValidRequest_ShouldReturnOk()
    {
        // Arrange
        var productKey = "laptop-001";

        // Act
        var response = await _httpClient.GetAsync($"/api/v1/channel/products/{productKey}/clv?channelKey={ChannelKey}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ProductClvDto>();
        result.Should().NotBeNull();
        result!.ProductKey.Should().Be(productKey);
        result.Currency.Should().Be("IRR");
    }

    /// <summary>
    /// تست GET /api/v1/channel/products/{productKey}/clv - با customerId
    /// </summary>
    [Fact]
    public async Task GetProductClv_WithCustomerId_ShouldReturnOk()
    {
        // Arrange
        var productKey = "laptop-001";
        var customerId = "customer-123";

        // Act
        var response = await _httpClient.GetAsync($"/api/v1/channel/products/{productKey}/clv?channelKey={ChannelKey}&customerId={customerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ProductClvDto>();
        result.Should().NotBeNull();
        result!.ProductKey.Should().Be(productKey);
    }

    #endregion

    #region EventTypesController Tests

    /// <summary>
    /// تست GET /api/v1/channel/event-types - دریافت همه EventTypes
    /// </summary>
    [Fact]
    public async Task GetAllEventTypes_WithTenantId_ShouldReturnOk()
    {
        // Act
        var response = await _httpClient.GetAsync($"/api/v1/channel/event-types?tenantId={TenantId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<EventTypeDto>>();
        result.Should().NotBeNull();
    }

    /// <summary>
    /// تست GET /api/v1/channel/event-types/{eventTypeId}/parameters - دریافت Parameters یک EventType
    /// </summary>
    [Fact]
    public async Task GetEventTypeParameters_WithValidEventTypeId_ShouldReturnOk()
    {
        // Arrange - ابتدا یک EventType دریافت می‌کنیم
        var eventTypesResponse = await _httpClient.GetAsync($"/api/v1/channel/event-types?tenantId={TenantId}");
        var eventTypes = await eventTypesResponse.Content.ReadFromJsonAsync<List<EventTypeDto>>();
        
        if (eventTypes == null || eventTypes.Count == 0)
        {
            // اگر EventType وجود ندارد، تست را skip می‌کنیم
            return;
        }

        var eventTypeId = eventTypes[0].Id;

        // Act
        var response = await _httpClient.GetAsync($"/api/v1/channel/event-types/{eventTypeId}/parameters");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<EventTypeParameterDto>>();
        result.Should().NotBeNull();
    }

    /// <summary>
    /// تست GET /api/v1/channel/event-types/with-parameters - دریافت EventTypes با Parameters
    /// </summary>
    [Fact]
    public async Task GetEventTypesWithParameters_WithTenantId_ShouldReturnOk()
    {
        // Act
        var response = await _httpClient.GetAsync($"/api/v1/channel/event-types/with-parameters?tenantId={TenantId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<EventTypeWithParametersDto>>();
        result.Should().NotBeNull();
        result!.All(et => et.Parameters != null).Should().BeTrue();
    }

    /// <summary>
    /// تست GET /api/v1/channel/event-types/with-parameters - با channelKey
    /// </summary>
    [Fact]
    public async Task GetEventTypesWithParameters_WithChannelKey_ShouldReturnOk()
    {
        // Act
        var response = await _httpClient.GetAsync($"/api/v1/channel/event-types/with-parameters?channelKey={ChannelKey}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<EventTypeWithParametersDto>>();
        result.Should().NotBeNull();
    }

    /// <summary>
    /// تست GET /api/v1/channel/event-types/with-parameters - با channelKey نامعتبر
    /// </summary>
    [Fact]
    public async Task GetEventTypesWithParameters_WithInvalidChannelKey_ShouldReturnNotFound()
    {
        // Act
        var response = await _httpClient.GetAsync("/api/v1/channel/event-types/with-parameters?channelKey=invalid-key");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region AssetsController Tests

    /// <summary>
    /// تست POST /api/v1/channel/assets/consume - مصرف دارایی
    /// </summary>
    [Fact]
    public async Task ConsumeAsset_WithValidRequest_ShouldReturnAccepted()
    {
        // Arrange
        var request = new ChannelConsumeAssetRequestDto
        {
            CustomerMobile = CustomerMobile,
            Serial = "ASSET-001"
        };

        _httpClient.DefaultRequestHeaders.Remove("X-Channel-Key");
        _httpClient.DefaultRequestHeaders.Add("X-Channel-Key", ChannelKey);

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/channel/assets/consume", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        var result = await response.Content.ReadFromJsonAsync<OutboxResponseDto>();
        result.Should().NotBeNull();
        result!.OutboxId.Should().BeGreaterThan(0);
        result.State.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// تست POST /api/v1/channel/assets/consume - بدون ChannelKey
    /// </summary>
    [Fact]
    public async Task ConsumeAsset_WithoutChannelKey_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new ChannelConsumeAssetRequestDto
        {
            CustomerMobile = CustomerMobile,
            Serial = "ASSET-001"
        };

        _httpClient.DefaultRequestHeaders.Remove("X-Channel-Key");

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/channel/assets/consume", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// تست POST /api/v1/channel/assets/consume - با CorrelationId
    /// </summary>
    [Fact]
    public async Task ConsumeAsset_WithCorrelationId_ShouldReturnAccepted()
    {
        // Arrange
        var request = new ChannelConsumeAssetRequestDto
        {
            CustomerMobile = CustomerMobile,
            Serial = "ASSET-002"
        };

        _httpClient.DefaultRequestHeaders.Remove("X-Channel-Key");
        _httpClient.DefaultRequestHeaders.Add("X-Channel-Key", ChannelKey);
        _httpClient.DefaultRequestHeaders.Add("X-Correlation-ID", "test-correlation-id-456");

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/channel/assets/consume", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        var result = await response.Content.ReadFromJsonAsync<OutboxResponseDto>();
        result.Should().NotBeNull();
    }

    #endregion
}

/// <summary>
/// Factory برای ایجاد WebApplication در Integration Tests برای Channel API
/// </summary>
public class HyperChannelWebApplicationFactory : WebApplicationFactory<Hyper.Channel.Api.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            // تنظیمات تست
            var testConfig = new Dictionary<string, string?>
            {
                { "ConnectionStrings:Domain", "Server=.;Database=HyperTest;TrustServerCertificate=True" }
            };
            config.AddInMemoryCollection(testConfig);
        });
        
        builder.ConfigureServices(services =>
        {
            // تنظیمات سرویس‌های تست
            // در صورت نیاز می‌توانید mock services را اینجا اضافه کنید
        });
    }
}

