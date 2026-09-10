using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Hyper.Channel.Api.Infrastructure.Telemetry;

/// <summary>
/// Centralized telemetry infrastructure for Hyper.Channel.Api
/// Provides ActivitySource for tracing and Meter for metrics
/// </summary>
public static class ChannelApiTelemetry
{
    private static readonly ActivitySource ActivitySource = new("Hyper.Channel.Api", "1.0.0");
    private static readonly Meter Meter = new("Hyper.Channel.Api", "1.0.0");

    // Metrics - Counters
    public static readonly Counter<long> HttpRequestCounter = Meter.CreateCounter<long>(
        "Hyper_channel_api_http_requests_total",
        "requests",
        "Total number of HTTP requests processed");

    public static readonly Counter<long> HttpErrorCounter = Meter.CreateCounter<long>(
        "Hyper_channel_api_http_errors_total",
        "errors",
        "Total number of HTTP errors");

    // Metrics - Histograms
    public static readonly Histogram<double> HttpRequestDuration = Meter.CreateHistogram<double>(
        "Hyper_channel_api_http_request_duration_seconds",
        "seconds",
        "HTTP request duration in seconds");

    // Controller-specific counters
    public static readonly Counter<long> ProductsRequestCounter = Meter.CreateCounter<long>(
        "Hyper_channel_api_products_requests_total",
        "requests",
        "Total number of Products API requests");

    public static readonly Counter<long> EventsRequestCounter = Meter.CreateCounter<long>(
        "Hyper_channel_api_events_requests_total",
        "requests",
        "Total number of Events API requests");

    public static readonly Counter<long> AssetsRequestCounter = Meter.CreateCounter<long>(
        "Hyper_channel_api_assets_requests_total",
        "requests",
        "Total number of Assets API requests");

    public static readonly Counter<long> ChannelsRequestCounter = Meter.CreateCounter<long>(
        "Hyper_channel_api_channels_requests_total",
        "requests",
        "Total number of Channels API requests");

    public static readonly Counter<long> EventTypesRequestCounter = Meter.CreateCounter<long>(
        "Hyper_channel_api_event_types_requests_total",
        "requests",
        "Total number of EventTypes API requests");

    // Business metrics
    public static readonly Counter<long> EventsEnqueuedCounter = Meter.CreateCounter<long>(
        "Hyper_channel_api_events_enqueued_total",
        "events",
        "Total number of events enqueued");

    public static readonly Counter<long> AssetsConsumedCounter = Meter.CreateCounter<long>(
        "Hyper_channel_api_assets_consumed_total",
        "assets",
        "Total number of assets consumed");

    public static readonly Counter<long> ProductsPurchasedCounter = Meter.CreateCounter<long>(
        "Hyper_channel_api_products_purchased_total",
        "purchases",
        "Total number of products purchased");

    /// <summary>
    /// Creates a new Activity for tracing API operations
    /// </summary>
    public static Activity? StartActivity(string name, ActivityKind kind = ActivityKind.Server)
    {
        return ActivitySource.StartActivity(name, kind);
    }

    /// <summary>
    /// Records HTTP request metrics
    /// </summary>
    public static void RecordHttpRequest(string method, string route, int statusCode, double durationSeconds, string? clientId = null)
    {
        var tags = new TagList
        {
            { "http.method", method },
            { "http.route", route },
            { "http.status_code", statusCode.ToString() }
        };

        if (!string.IsNullOrWhiteSpace(clientId))
        {
            tags.Add("client.id", clientId);
        }

        HttpRequestCounter.Add(1, tags);
        HttpRequestDuration.Record(durationSeconds, tags);

        if (statusCode >= 400)
        {
            HttpErrorCounter.Add(1, tags);
        }
    }

    /// <summary>
    /// Records controller-specific request metrics
    /// </summary>
    public static void RecordControllerRequest(string controller, string action, string? clientId = null)
    {
        var tags = new TagList
        {
            { "controller", controller },
            { "action", action }
        };

        if (!string.IsNullOrWhiteSpace(clientId))
        {
            tags.Add("client.id", clientId);
        }

        switch (controller.ToLowerInvariant())
        {
            case "products":
                ProductsRequestCounter.Add(1, tags);
                break;
            case "events":
                EventsRequestCounter.Add(1, tags);
                break;
            case "assets":
                AssetsRequestCounter.Add(1, tags);
                break;
            case "channels":
                ChannelsRequestCounter.Add(1, tags);
                break;
            case "eventtypes":
                EventTypesRequestCounter.Add(1, tags);
                break;
        }
    }

    /// <summary>
    /// Records business event metrics
    /// </summary>
    public static void RecordEventEnqueued(string clientId, string eventTypeKey)
    {
        var tags = new TagList
        {
            { "client.id", clientId },
            { "event.type", eventTypeKey }
        };
        EventsEnqueuedCounter.Add(1, tags);
    }

    /// <summary>
    /// Records asset consumption metrics
    /// </summary>
    public static void RecordAssetConsumed(string clientId, string serial)
    {
        var tags = new TagList
        {
            { "client.id", clientId },
            { "asset.serial", serial }
        };
        AssetsConsumedCounter.Add(1, tags);
    }

    /// <summary>
    /// Records product purchase metrics
    /// </summary>
    public static void RecordProductPurchased(string? clientId, int productId, int quantity)
    {
        var tags = new TagList
        {
            { "client.id", clientId ?? "unknown" },
            { "product.id", productId.ToString() },
            { "quantity", quantity.ToString() }
        };
        ProductsPurchasedCounter.Add(1, tags);
    }
}