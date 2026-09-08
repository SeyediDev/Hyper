using Hyper.Channel.Api.Controllers.Dto;
using Hyper.Channel.Api.Infrastructure.Telemetry;
using Hyper.Channel.Application.Features.Channel.Commands;
using Microsoft.Extensions.Primitives;
using Neo.Application.Features.Outbox.Dto;
using System.Diagnostics;
using Hyper.Domain.Features.Channels;
using Hyper.Channel.Api.Controllers.Dto.Events;

namespace Hyper.Channel.Api.Controllers;

/// <summary>
/// کنترلر مدیریت رویدادها (Events)
/// </summary>
[AppRoute("channel", "events")]
[Tags("events")]
public sealed class EventsController(
        IChannelService channelService,
        ILogger<EventsController> logger
    ) : AppControllerBase
{

    /// <summary>
    /// دریافت یک رویداد از کانال
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="dto">اطلاعات رویداد</param>
    /// <param name="clientId">کلید کانال (از header)</param>
    /// <returns>پاسخ شامل OutboxId و State</returns>
    /// <response code="202">رویداد با موفقیت در صف قرار گرفت</response>
    /// <response code="400">خطا در اعتبارسنجی درخواست</response>
    /// <response code="404">کانال یافت نشد</response>
    [HttpPost]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OutboxResponseDto), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(BulkReceiveDynamicEventResponseDto), StatusCodes.Status202Accepted)]
    public async Task<Results<Accepted<OutboxResponseDto>, Accepted<BulkReceiveDynamicEventResponseDto>, ProblemHttpResult>> Receive(
        [FromBody] ReceiveDynamicEventRequestDto dto, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        using var activity = ChannelApiTelemetry.StartActivity("Events.Receive");
        activity?.SetTag("event.type", dto.EventTypeKey);
        activity?.SetTag("product.category.key", dto.ProductCategoryKey);
        activity?.SetTag("product.key", dto.ProductKey);
        activity?.SetTag("customer.mobile", dto.CustomerMobile);
        activity?.SetTag("is.inquiry", dto.IsInquiry.ToString());

        logger.LogInformation(
            "Creating event. Type: {EventTypeKey}, Customer: {CustomerMobile}, Inquiry: {IsInquiry}",
            dto.EventTypeKey, dto.CustomerMobile, dto.IsInquiry);

        try
        {
            // استخراج clientId از claim توکن
            var clientId = channelService.GetClientIdFromToken(HttpContext);
            if (string.IsNullOrWhiteSpace(clientId))
            {
                stopwatch.Stop();
                activity?.SetStatus(ActivityStatusCode.Error, "Client ID not found in token");
                logger.LogWarning("Client ID not found in token. Duration: {Duration}ms", stopwatch.ElapsedMilliseconds);
                ChannelApiTelemetry.RecordHttpRequest("POST", "events", 400, stopwatch.Elapsed.TotalSeconds, null);
                return TypedResults.Problem("شناسه کلاینت در توکن یافت نشد", statusCode: StatusCodes.Status400BadRequest);
            }

            activity?.SetTag("client.id", clientId);
            string? correlationId = GetCorrelationId();
            if (string.IsNullOrWhiteSpace(correlationId))
            {
                activity?.SetTag("correlation.id", correlationId ?? "null");
            }
            string? idempotencyKey = dto.IdempotencyKey;
            if (string.IsNullOrWhiteSpace(idempotencyKey))
            {
                idempotencyKey = GetIdempotencyKey() ?? correlationId;
			}
            activity?.SetTag("idempotency.key", idempotencyKey ?? "null");

            var command = new EnqueueEventCommand(
                idempotencyKey,
                correlationId,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                clientId,
                dto.IsInquiry,
                dto.EventTypeKey,
                dto.ProductCategoryKey,
                dto.ProductKey,
                dto.CustomerMobile,
                dto.ReferrerCode,
                dto.Attributes);

            OutboxResponse response = await Sender.Send(command, cancellationToken);
            
            stopwatch.Stop();
            activity?.SetStatus(ActivityStatusCode.Ok);
            activity?.SetTag("outbox.id", response.OutboxId.ToString());
            logger.LogInformation(
                "Event queued for channel {client} with OutboxId {OutboxId}. Duration: {Duration}ms",
                clientId, response.OutboxId, stopwatch.ElapsedMilliseconds);

            ChannelApiTelemetry.RecordHttpRequest("POST", "events", 202, stopwatch.Elapsed.TotalSeconds, clientId);
            if (!dto.IsInquiry)
            {
                ChannelApiTelemetry.RecordEventEnqueued(clientId, dto.EventTypeKey);
            }

            return TypedResults.Accepted(string.Empty, new OutboxResponseDto(response));
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.SetTag("error.type", ex.GetType().Name);
            activity?.SetTag("error.message", ex.Message);
            activity?.SetTag("error.stacktrace", ex.StackTrace);
            logger.LogError(ex,
                "Error creating event. Type: {EventTypeKey}, Customer: {CustomerMobile}. Duration: {Duration}ms",
                dto.EventTypeKey, dto.CustomerMobile, stopwatch.ElapsedMilliseconds);
            ChannelApiTelemetry.RecordHttpRequest("POST", "events", 500, stopwatch.Elapsed.TotalSeconds, null);
            throw;
        }
    }

    /// <param name="dto">لیست رویدادها</param>
    /// <param name="clientId">کلید کانال (از header)</param>
    /// <returns>پاسخ شامل لیست نتایج</returns>
    /// <response code="202">رویدادها با موفقیت در صف قرار گرفتند</response>
    /// <response code="400">خطا در اعتبارسنجی درخواست</response>
    /// <response code="404">کانال یافت نشد</response>
    [HttpPost("bulk")]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BulkReceiveDynamicEventResponseDto), StatusCodes.Status202Accepted)]
    public async Task<Results<Accepted<BulkReceiveDynamicEventResponseDto>, ProblemHttpResult>> ReceiveBulk(
        [FromBody] BulkReceiveDynamicEventRequestDto dto, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        using var activity = ChannelApiTelemetry.StartActivity("Events.ReceiveBulk");
        var eventCount = dto.Events?.Count ?? 0;
        activity?.SetTag("events.count", eventCount.ToString());

        logger.LogInformation("Creating bulk events. Count: {EventCount}", eventCount);

        try
        {
            // استخراج clientId از claim توکن
            var clientId = channelService.GetClientIdFromToken(HttpContext);
            if (string.IsNullOrWhiteSpace(clientId))
            {
                stopwatch.Stop();
                activity?.SetStatus(ActivityStatusCode.Error, "Client ID not found in token");
                logger.LogWarning("Client ID not found in token for bulk events. Duration: {Duration}ms", stopwatch.ElapsedMilliseconds);
                ChannelApiTelemetry.RecordHttpRequest("POST", "events/bulk", 400, stopwatch.Elapsed.TotalSeconds, null);
                return TypedResults.Problem("شناسه کلاینت در توکن یافت نشد", statusCode: StatusCodes.Status400BadRequest);
            }

            activity?.SetTag("client.id", clientId);

            if (dto.Events == null || dto.Events.Count == 0)
            {
                stopwatch.Stop();
                activity?.SetStatus(ActivityStatusCode.Error, "Events list is empty");
                logger.LogWarning("Events list is empty. Duration: {Duration}ms", stopwatch.ElapsedMilliseconds);
                ChannelApiTelemetry.RecordHttpRequest("POST", "events/bulk", 400, stopwatch.Elapsed.TotalSeconds, null);
                return TypedResults.Problem("لیست رویدادها خالی است", statusCode: StatusCodes.Status400BadRequest);
            }

            ChannelApiTelemetry.RecordControllerRequest("Events", "ReceiveBulk", clientId);
            string? correlationId = GetCorrelationId();
            if (string.IsNullOrWhiteSpace(correlationId))
            {
                activity?.SetTag("correlation.id", correlationId ?? "null");
            }

            // تبدیل DTOs
            var eventDtos = dto.Events.Select(e =>
            {
                return new EnqueueEventCommand(
                    GetIdempotencyKey(e, correlationId),
                    correlationId,
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    clientId,
                    e.IsInquiry,
                    e.EventTypeKey,
                    e.ProductCategoryKey,
                    e.ProductKey,
                    e.CustomerMobile,
                    e.ReferrerCode,
                    e.Attributes
                    );
            }).ToList();

            var command = new EnqueueBulkEventsCommand(
                eventDtos,
                GetCorrelationId(),
                HttpContext.Connection.RemoteIpAddress?.ToString());

            var response = await Sender.Send(command, cancellationToken);
            
            stopwatch.Stop();
            activity?.SetStatus(ActivityStatusCode.Ok);
            activity?.SetTag("results.count", response.Results.Count.ToString());
            logger.LogInformation(
                "Bulk events queued for channel {clientId} with {Count} events. Duration: {Duration}ms",
                clientId, response.Results.Count, stopwatch.ElapsedMilliseconds);

            ChannelApiTelemetry.RecordHttpRequest("POST", "events/bulk", 202, stopwatch.Elapsed.TotalSeconds, clientId);
            
            // Record metrics for each successfully enqueued event
            foreach (var eventDto in dto.Events.Where(e => !e.IsInquiry))
            {
                ChannelApiTelemetry.RecordEventEnqueued(clientId, eventDto.EventTypeKey);
            }

            return TypedResults.Accepted(string.Empty, new BulkReceiveDynamicEventResponseDto(response));
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.SetTag("error.type", ex.GetType().Name);
            activity?.SetTag("error.message", ex.Message);
            activity?.SetTag("error.stacktrace", ex.StackTrace);
            logger.LogError(ex,
                "Error creating bulk events. Count: {EventCount}. Duration: {Duration}ms",
                eventCount, stopwatch.ElapsedMilliseconds);
            ChannelApiTelemetry.RecordHttpRequest("POST", "events/bulk", 500, stopwatch.Elapsed.TotalSeconds, null);
            throw;
        }
    }

    private string? GetIdempotencyKey(ReceiveDynamicEventRequestDto e, string? correlationId)
    {
        string? idempotencyKey = e.IdempotencyKey;
        if (string.IsNullOrWhiteSpace(idempotencyKey))
        {
            idempotencyKey = GetIdempotencyKey() ?? correlationId;
        }

        return idempotencyKey;
    }

    private string? GetCorrelationId()
    {
        if (HttpContext.Request.Headers.TryGetValue("X-Correlation-ID", out StringValues values))
        {
            return values.ToString();
        }

        return null;
    }

    private string? GetIdempotencyKey()
    {
        if (HttpContext.Request.Headers.TryGetValue("X-Idempotency-ID", out StringValues values))
        {
            return values.ToString();
        }

        return null;
    }
}
