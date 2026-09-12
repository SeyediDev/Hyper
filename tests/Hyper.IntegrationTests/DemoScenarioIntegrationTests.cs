using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Hyper.IntegrationTests;

/// <summary>
/// تست‌های Integration سناریوی دمو باشگاه مشتریان سیموتک
/// </summary>
[Collection("Integration Tests")]
public class DemoScenarioIntegrationTests : IClassFixture<HyperWebApplicationFactory>
{
    private readonly HttpClient _httpClient;
    private const int TenantId = 1;
    private const string ChannelKey = "bajet";
    private const string CustomerMobile = "09127165496";

    public DemoScenarioIntegrationTests(HyperWebApplicationFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    /// <summary>
    /// تست ارسال رویداد افتتاح حساب دیجیتال (50 امتیاز)
    /// </summary>
    [Fact]
    public async Task DigitalAccountOpeningEvent_ShouldReturnSuccess()
    {
        // Arrange
        var eventRequest = new
        {
            tenantId = TenantId,
            customerMobile = CustomerMobile,
            eventTypeKey = "DigitalAccountOpening",
            parameters = new Dictionary<string, string>()
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync(
            $"/api/v1/channel/commands/events?channelKey={ChannelKey}",
            eventRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // بررسی اینکه رویداد ثبت شده است
        // بررسی اینکه امتیاز 50 اعطا شده است
    }

    /// <summary>
    /// تست ارسال رویداد دعوت دوست (20 امتیاز)
    /// </summary>
    [Fact]
    public async Task FriendInvitationEvent_ShouldReturnSuccess()
    {
        // Arrange
        var eventRequest = new
        {
            tenantId = TenantId,
            customerMobile = CustomerMobile,
            eventTypeKey = "FriendInvitation",
            parameters = new Dictionary<string, string>
            {
                { "ReferrerCode", "REF123" }
            }
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync(
            $"/api/v1/channel/commands/events?channelKey={ChannelKey}",
            eventRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    /// <summary>
    /// تست ارسال رویداد خرید شارژ 20,000 تومانی (2 امتیاز)
    /// </summary>
    [Fact]
    public async Task Charge20kPurchaseEvent_ShouldReturnSuccess()
    {
        // Arrange
        var eventRequest = new
        {
            tenantId = TenantId,
            customerMobile = CustomerMobile,
            eventTypeKey = "PurchaseProductOrService",
            parameters = new Dictionary<string, string>
            {
                { "ProductKey", "charge-20k" },
                { "ProductPrice", "20000" }
            }
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync(
            $"/api/v1/channel/commands/events?channelKey={ChannelKey}",
            eventRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    /// <summary>
    /// تست ارسال رویداد خرید بسته اینترنت 30,000 تومانی (3 امتیاز)
    /// </summary>
    [Fact]
    public async Task Internet30kPurchaseEvent_ShouldReturnSuccess()
    {
        // Arrange
        var eventRequest = new
        {
            tenantId = TenantId,
            customerMobile = CustomerMobile,
            eventTypeKey = "PurchaseProductOrService",
            parameters = new Dictionary<string, string>
            {
                { "ProductKey", "internet-30k" },
                { "ProductPrice", "30000" }
            }
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync(
            $"/api/v1/channel/commands/events?channelKey={ChannelKey}",
            eventRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    /// <summary>
    /// تست سناریوی کامل دمو: کسب 75 امتیاز
    /// </summary>
    [Fact]
    public async Task FullDemoScenario_Earning75Points_ShouldSucceed()
    {
        // Arrange & Act
        // 1. افتتاح حساب دیجیتال (50 امتیاز)
        await DigitalAccountOpeningEvent_ShouldReturnSuccess();
        
        // 2. دعوت دوست (20 امتیاز)
        await FriendInvitationEvent_ShouldReturnSuccess();
        
        // 3. خرید شارژ 20k (2 امتیاز)
        await Charge20kPurchaseEvent_ShouldReturnSuccess();
        
        // 4. خرید بسته اینترنت 30k (3 امتیاز)
        await Internet30kPurchaseEvent_ShouldReturnSuccess();

        // Assert
        // بررسی بالانس نهایی مشتری (باید 75 امتیاز باشد)
        // این نیاز به یک endpoint برای دریافت بالانس امتیازات دارد
    }

    /// <summary>
    /// تست دریافت بالانس امتیازات مشتری
    /// </summary>
    [Fact]
    public async Task GetCustomerPointBalance_ShouldReturnBalance()
    {
        // Arrange
        var customerMobile = CustomerMobile;

        // Act
        var response = await _httpClient.GetAsync(
            $"/api/v1/customer/queries/points/balance?customerMobile={customerMobile}&tenantId={TenantId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var balance = await response.Content.ReadFromJsonAsync<PointBalanceResponse>();
        balance.Should().NotBeNull();
        // بررسی اینکه بالانس صحیح است
    }
}

/// <summary>
/// Factory برای ایجاد WebApplication در Integration Tests
/// </summary>
public class HyperWebApplicationFactory : WebApplicationFactory<Microsoft.VisualStudio.TestPlatform.TestHost.Program>
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
        });
    }
}

public record PointBalanceResponse
{
    public long Balance { get; set; }
    public int PointId { get; set; }
    public string PointTitle { get; set; } = string.Empty;
}

