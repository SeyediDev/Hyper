using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Hyper.Application.Features.Products.Services;

/// <summary>
/// پیاده‌سازی سرویس دریافت قیمت و CLV از کانال
/// </summary>
public class ChannelPriceService : IChannelPriceService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ChannelPriceService> _logger;
    private readonly IConfiguration _configuration;

    public ChannelPriceService(
        IHttpClientFactory httpClientFactory,
        ILogger<ChannelPriceService> logger,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<decimal?> GetProductPriceFromChannelAsync(
        string channelKey,
        string productKey,
        string? customerId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var channelApiUrl = _configuration["Channel:ApiUrl"] ?? "http://localhost:5000";
            
            using var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(channelApiUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
            
            var url = $"/v1/channel/products/{Uri.EscapeDataString(productKey)}/price?channelKey={Uri.EscapeDataString(channelKey)}";
            if (!string.IsNullOrEmpty(customerId))
            {
                url += $"&customerId={Uri.EscapeDataString(customerId)}";
            }
            
            var response = await client.GetAsync(url, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var priceResponse = JsonSerializer.Deserialize<PriceResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return priceResponse?.Price;
            }
            
            _logger.LogWarning(
                "Failed to fetch price from channel {ChannelKey} for product {ProductKey}. Status: {Status}",
                channelKey, productKey, response.StatusCode);
            
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error fetching price from channel {ChannelKey} for product {ProductKey}",
                channelKey, productKey);
            return null;
        }
    }

    public async Task<decimal?> GetProductClvFromChannelAsync(
        string channelKey,
        string productKey,
        string? customerId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var channelApiUrl = _configuration["Channel:ApiUrl"] ?? "http://localhost:5000";
            
            using var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(channelApiUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
            
            var url = $"/v1/channel/products/{Uri.EscapeDataString(productKey)}/clv?channelKey={Uri.EscapeDataString(channelKey)}";
            if (!string.IsNullOrEmpty(customerId))
            {
                url += $"&customerId={Uri.EscapeDataString(customerId)}";
            }
            
            var response = await client.GetAsync(url, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var clvResponse = JsonSerializer.Deserialize<ClvResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return clvResponse?.Clv;
            }
            
            _logger.LogWarning(
                "Failed to fetch CLV from channel {ChannelKey} for product {ProductKey}. Status: {Status}",
                channelKey, productKey, response.StatusCode);
            
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error fetching CLV from channel {ChannelKey} for product {ProductKey}",
                channelKey, productKey);
            return null;
        }
    }

    private record PriceResponse(decimal? Price);
    private record ClvResponse(decimal? Clv);
}

