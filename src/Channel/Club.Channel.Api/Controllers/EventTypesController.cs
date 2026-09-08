using System.Diagnostics;
using Hyper.Channel.Api.Infrastructure.Telemetry;
using Hyper.Domain.Entities.Tenants.Enums;
using Hyper.Domain.Features.Attributes;
using Hyper.Domain.Features.Channels;

namespace Hyper.Channel.Api.Controllers;

/// <summary>
/// کنترلر مدیریت انواع رویداد (Event Types)
/// </summary>
[AppRoute("channel", "event-types")]
[Tags("event-types")]
public sealed class EventTypesController(
    IAttributeService attributeService,
    IChannelService channelService,
    IEventTypeService eventTypeService,
    ILogger<EventTypesController> logger
) : AppControllerBase
{
    /// <summary>
    /// دریافت لیست EventTypes برای یک Tenant
    /// </summary>
    /// <param name="tenantId">شناسه Tenant</param>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>لیست EventTypes</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<EventTypeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<EventTypeDto>>> GetAll(
        [FromQuery] int tenantId = 1,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        using var activity = ChannelApiTelemetry.StartActivity("EventTypes.GetAll");
        activity?.SetTag("tenant.id", tenantId.ToString());

        logger.LogInformation("Getting all event types for tenant {TenantId}", tenantId);

        try
        {
            ChannelApiTelemetry.RecordControllerRequest("EventTypes", "GetAll", null);

            var eventTypes = await eventTypeService.GetEventTypesAsync(tenantId, cancellationToken);

            var result = eventTypes.Select(et => new EventTypeDto
            {
                Id = et.Id,
                Key = et.Key,
                Title = et.Title,
                TenantId = et.TenantId
            }).ToList();

            stopwatch.Stop();
            activity?.SetStatus(ActivityStatusCode.Ok);
            activity?.SetTag("event.types.count", result?.Count.ToString());
            logger.LogInformation(
                "Successfully retrieved {Count} event types for tenant {TenantId}. Duration: {Duration}ms",
                result?.Count, tenantId, stopwatch.ElapsedMilliseconds);

            ChannelApiTelemetry.RecordHttpRequest("GET", "event-types", 200, stopwatch.Elapsed.TotalSeconds, null);

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
                "Error getting event types for tenant {TenantId}. Duration: {Duration}ms",
                tenantId, stopwatch.ElapsedMilliseconds);
            ChannelApiTelemetry.RecordHttpRequest("GET", "event-types", 500, stopwatch.Elapsed.TotalSeconds, null);
            throw;
        }
    }

    /// <summary>
    /// دریافت Attributes یک EventType
    /// </summary>
    /// <param name="eventTypeId">شناسه EventType</param>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>لیست Attributes</returns>
    [HttpGet("{eventTypeId}/attributes")]
    [ProducesResponseType(typeof(List<EventTypeAttributeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<EventTypeAttributeDto>>> GetAttributes(
        int eventTypeId,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        using var activity = ChannelApiTelemetry.StartActivity("EventTypes.GetAttributes");
        activity?.SetTag("event.type.id", eventTypeId.ToString());

        logger.LogInformation("Getting attributes for event type {EventTypeId}", eventTypeId);

        try
        {
            ChannelApiTelemetry.RecordControllerRequest("EventTypes", "GetAttributes", null);
            var channelDto = await channelService.GetChannelFromTokenAsync(HttpContext, cancellationToken);
            if(channelDto==null)
            {
                stopwatch.Stop();
                activity?.SetStatus(ActivityStatusCode.Error, "Channel not found");
                logger.LogWarning(
                    "Channel not found. Duration: {Duration}ms", stopwatch.ElapsedMilliseconds);
                ChannelApiTelemetry.RecordHttpRequest("GET", "{event-type}/attributes", 404, stopwatch.Elapsed.TotalSeconds, null);
                return NotFound($"Channel not found");
            }
            var attributes = await attributeService.GetAttributesAsync(channelDto.TenantId, AttributeArea.Event, eventTypeId, null, cancellationToken);

            var result = attributes.Select(p => new EventTypeAttributeDto
            {
                Id = p.Id,
                Key = p.Key,
                Title = p.Title?? p.Key,
                EventTypeId = eventTypeId
            }).ToList();

            stopwatch.Stop();
            activity?.SetStatus(ActivityStatusCode.Ok);
            activity?.SetTag("attibutes.count", result.Count.ToString());
            logger.LogInformation(
                "Successfully retrieved {Count} attibutes for event type {EventTypeId}. Duration: {Duration}ms",
                result.Count, eventTypeId, stopwatch.ElapsedMilliseconds);

            ChannelApiTelemetry.RecordHttpRequest("GET", "event-types/{eventTypeId}/attibutes", 200, stopwatch.Elapsed.TotalSeconds, null);

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
                "Error getting attibutes for event type {EventTypeId}. Duration: {Duration}ms",
                eventTypeId, stopwatch.ElapsedMilliseconds);
            ChannelApiTelemetry.RecordHttpRequest("GET", "event-types/{eventTypeId}/attibutes", 500, stopwatch.Elapsed.TotalSeconds, null);
            throw;
        }
    }

    /// <summary>
    /// دریافت EventTypes با attibutes
    /// </summary>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>لیست EventTypes با attibutes</returns>
    [HttpGet("with-attibutes")]
    [ProducesResponseType(typeof(List<EventTypeWithAttibutesDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<EventTypeWithAttibutesDto>>> GetWithAttibutes(
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        using var activity = ChannelApiTelemetry.StartActivity("EventTypes.GetWithAttibutes");
        var channelDto = await channelService.GetChannelFromTokenAsync(HttpContext, cancellationToken);
        if (channelDto == null)
        {
            stopwatch.Stop();
            activity?.SetStatus(ActivityStatusCode.Error, "Channel not found");
            logger.LogWarning(
                "Channel not found. Duration: {Duration}ms", stopwatch.ElapsedMilliseconds);
            ChannelApiTelemetry.RecordHttpRequest("GET", "{event-type}/attributes", 404, stopwatch.Elapsed.TotalSeconds, null);
            return NotFound($"Channel not found");
        }
        activity?.SetTag("tenant.id", channelDto.TenantId);
        activity?.SetTag("channel.key", channelDto.Key);

        logger.LogInformation(
            "Getting event types with attibutes. TenantId: {TenantId}, ChannelKey: {ChannelKey}",
            channelDto.TenantId, channelDto.Key);

        try
        {
            ChannelApiTelemetry.RecordControllerRequest("EventTypes", "GetWithAttibutes", channelDto.ClientId);

            var eventTypes = await eventTypeService.GetEventTypesAsync(channelDto.TenantId, cancellationToken);
            var result = new List<EventTypeWithAttibutesDto>();

            foreach (var et in eventTypes)
            {
                var attibutes = await attributeService.GetAttributesAsync(channelDto.TenantId, AttributeArea.Event, et.Id, null, cancellationToken);

                result.Add(new EventTypeWithAttibutesDto
                {
                    Id = et.Id,
                    Key = et.Key,
                    Title = et.Title,
                    TenantId = et.TenantId,
                    Attibutes = [.. attibutes.Select(p => new EventTypeAttributeDto
                    {
                        Id = p.Id,
                        Key = p.Key,
                        Title = p.Title!,
                        EventTypeId = et.Id
                    })]
                });
            }

            stopwatch.Stop();
            activity?.SetStatus(ActivityStatusCode.Ok);
            activity?.SetTag("event.types.count", result.Count.ToString());
            var totalAttibutes = result.Sum(et => et.Attibutes.Count);
            activity?.SetTag("total.attibutes.count", totalAttibutes.ToString());
            logger.LogInformation(
                "Successfully retrieved {Count} event types with attibutes. Duration: {Duration}ms",
                result.Count, stopwatch.ElapsedMilliseconds);

            ChannelApiTelemetry.RecordHttpRequest("GET", "event-types/with-attibutes", 200, stopwatch.Elapsed.TotalSeconds, channelDto.Key);

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
                "Error getting event types with attibutes. TenantId: {TenantId}, ChannelKey: {ChannelKey}. Duration: {Duration}ms",
                channelDto.TenantId, channelDto.Key, stopwatch.ElapsedMilliseconds);
            ChannelApiTelemetry.RecordHttpRequest("GET", "event-types/with-attibutes", 500, stopwatch.Elapsed.TotalSeconds, channelDto.Key);
            throw;
        }
    }
}

public sealed record EventTypeDto
{
    public int Id { get; init; }
    public string Key { get; init; } = null!;
    public string Title { get; init; } = null!;
    public int TenantId { get; init; }
}

public sealed record EventTypeAttributeDto
{
    public int Id { get; init; }
    public string Key { get; init; } = null!;
    public string Title { get; init; } = null!;
    public int EventTypeId { get; init; }
}

public sealed record EventTypeWithAttibutesDto
{
    public int Id { get; init; }
    public string Key { get; init; } = null!;
    public string Title { get; init; } = null!;
    public int TenantId { get; init; }
    public List<EventTypeAttributeDto> Attibutes { get; init; } = [];
}