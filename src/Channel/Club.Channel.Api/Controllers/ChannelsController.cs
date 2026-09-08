using Hyper.Channel.Api.Infrastructure.Telemetry;
using System.Diagnostics;
using Hyper.Domain.Features.Channels;

namespace Hyper.Channel.Api.Controllers;

/// <summary>
/// کنترلر مدیریت کانال‌ها (Channels)
/// </summary>
[AppRoute("channel", "channels")]
[Tags("channels")]
public sealed class ChannelsController(
    IChannelService channelService,
    ILogger<ChannelsController> logger
) : AppControllerBase
{
    /// <summary>
    /// دریافت Channel با ChannelKey
    /// </summary>
    /// <param name="channelKey">کلید کانال</param>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>اطلاعات کانال</returns>
    /// <response code="200">کانال با موفقیت دریافت شد</response>
    /// <response code="404">کانال یافت نشد</response>
    [HttpGet("{channelKey}")]
    [ProducesResponseType(typeof(EventChannelDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventChannelDto>> GetByKey(
        string channelKey, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        using var activity = ChannelApiTelemetry.StartActivity("Channels.GetByKey");
        activity?.SetTag("channel.key", channelKey);

        logger.LogInformation("Getting channel by key {ChannelKey}", channelKey);

        try
        {
            ChannelApiTelemetry.RecordControllerRequest("Channels", "GetByKey", channelKey);

            var channel = await channelService.GetChannelByKeyAsync(channelKey, cancellationToken);

            if (channel == null)
            {
                stopwatch.Stop();
                activity?.SetStatus(ActivityStatusCode.Error, "Channel not found");
                logger.LogWarning("Channel with key {ChannelKey} not found. Duration: {Duration}ms", channelKey, stopwatch.ElapsedMilliseconds);
                ChannelApiTelemetry.RecordHttpRequest("GET", "channels/{channelKey}", 404, stopwatch.Elapsed.TotalSeconds, channelKey);
                return NotFound($"Channel with key '{channelKey}' not found");
            }

            stopwatch.Stop();
            activity?.SetStatus(ActivityStatusCode.Ok);
            activity?.SetTag("tenant.id", channel.TenantId.ToString());
            logger.LogInformation(
                "Successfully retrieved channel {ChannelKey}. Duration: {Duration}ms",
                channelKey, stopwatch.ElapsedMilliseconds);

            ChannelApiTelemetry.RecordHttpRequest("GET", "channels/{channelKey}", 200, stopwatch.Elapsed.TotalSeconds, channelKey);

            return Ok(channel);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.SetTag("error.type", ex.GetType().Name);
            activity?.SetTag("error.message", ex.Message);
            activity?.SetTag("error.stacktrace", ex.StackTrace);
            logger.LogError(ex,
                "Error getting channel by key {ChannelKey}. Duration: {Duration}ms",
                channelKey, stopwatch.ElapsedMilliseconds);
            ChannelApiTelemetry.RecordHttpRequest("GET", "channels/{channelKey}", 500, stopwatch.Elapsed.TotalSeconds, channelKey);
            throw;
        }
    }

    /// <summary>
    /// دریافت لیست EventChannels
    /// </summary>
    /// <param name="tenantId">شناسه Tenant (0 = همه)</param>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>لیست کانال‌ها</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<EventChannelDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<EventChannelDto>>> GetAll(
        [FromQuery] int tenantId = 0, // 0 = همه
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        using var activity = ChannelApiTelemetry.StartActivity("Channels.GetAll");
        activity?.SetTag("tenant.id", tenantId == 0 ? "all" : tenantId.ToString());

        logger.LogInformation("Getting all channels. TenantId: {TenantId}", tenantId == 0 ? "all" : tenantId);

        try
        {
            ChannelApiTelemetry.RecordControllerRequest("Channels", "GetAll", null);

            var channels = await channelService.GetChannelsAsync(tenantId, cancellationToken);

            stopwatch.Stop();
            activity?.SetStatus(ActivityStatusCode.Ok);
            activity?.SetTag("channels.count", channels.Count.ToString());
            logger.LogInformation(
                "Successfully retrieved {Count} channels. Duration: {Duration}ms",
                channels.Count, stopwatch.ElapsedMilliseconds);

            ChannelApiTelemetry.RecordHttpRequest("GET", "channels", 200, stopwatch.Elapsed.TotalSeconds, null);

            return Ok(channels);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.SetTag("error.type", ex.GetType().Name);
            activity?.SetTag("error.message", ex.Message);
            activity?.SetTag("error.stacktrace", ex.StackTrace);
            logger.LogError(ex,
                "Error getting all channels. TenantId: {TenantId}. Duration: {Duration}ms",
                tenantId, stopwatch.ElapsedMilliseconds);
            ChannelApiTelemetry.RecordHttpRequest("GET", "channels", 500, stopwatch.Elapsed.TotalSeconds, null);
            throw;
        }
    }

    /// <summary>
    /// دریافت لیست اکوسیستم‌ها و کانال‌ها به صورت یکجا
    /// </summary>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>لیست اکوسیستم‌ها و کانال‌ها</returns>
    [HttpGet("with-tenants")]
    [ProducesResponseType(typeof(TenantsAndChannelsResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<TenantsAndChannelsResponseDto>> GetTenantsAndChannels(CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        using var activity = ChannelApiTelemetry.StartActivity("Channels.GetTenantsAndChannels");

        logger.LogInformation("Getting tenants and channels");

        try
        {
            ChannelApiTelemetry.RecordControllerRequest("Channels", "GetTenantsAndChannels", null);

            var result = await channelService.GetTenantsAndChannelsAsync(cancellationToken);

            stopwatch.Stop();
            activity?.SetStatus(ActivityStatusCode.Ok);
            activity?.SetTag("tenants.count", result.Tenants.Count.ToString());
            activity?.SetTag("channels.count", result.Channels.Count.ToString());
            logger.LogInformation(
                "Successfully retrieved {TenantCount} tenants and {ChannelCount} channels. Duration: {Duration}ms",
                result.Tenants.Count, result.Channels.Count, stopwatch.ElapsedMilliseconds);

            ChannelApiTelemetry.RecordHttpRequest("GET", "channels/with-tenants", 200, stopwatch.Elapsed.TotalSeconds, null);

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
                "Error getting tenants and channels. Duration: {Duration}ms",
                stopwatch.ElapsedMilliseconds);
            ChannelApiTelemetry.RecordHttpRequest("GET", "channels/with-tenants", 500, stopwatch.Elapsed.TotalSeconds, null);
            throw;
        }
    }
}