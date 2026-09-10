using System.Diagnostics;
using Hyper.Channel.Api.Controllers.Dto;
using Hyper.Channel.Api.Infrastructure.Telemetry;
using Hyper.Channel.Application.Features.Channel.Commands;
using Hyper.Domain.Features.Attributes;
using Hyper.Domain.Features.Channels;
using Microsoft.Extensions.Primitives;
using Neo.Application.Features.Outbox.Dto;

namespace Hyper.Channel.Api.Controllers;

/// <summary>
/// کنترلر مدیریت دارایی‌ها (Assets)
/// </summary>
[AppRoute("channel", "assets")]
[Tags("assets")]
public sealed class AssetsController(
    IChannelService channelService,
    ILogger<AssetsController> logger) : AppControllerBase
{
    private const string ClientIdClaim = "client_id";
    private const string NameIdClaim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

    /// <summary>
    /// استخراج clientId از claim توکن
    /// </summary>
    private string? GetClientIdFromToken()
    {
        // اول از claim "client_id" استخراج کن
        var clientIdClaim = HttpContext.User?.FindFirst(ClientIdClaim)?.Value;
        if (!string.IsNullOrWhiteSpace(clientIdClaim))
        {
            return clientIdClaim;
        }

        // Fallback به claim "nameid" (NameIdentifier)
        var nameIdClaim = HttpContext.User?.FindFirst(NameIdClaim)?.Value;
        if (!string.IsNullOrWhiteSpace(nameIdClaim))
        {
            return nameIdClaim;
        }

        return null;
    }

    /// <summary>
    /// مصرف دارایی (Asset)
    /// </summary>
    /// <remarks>
    /// این endpoint برای مصرف یک دارایی استفاده می‌شود.
    /// 
    /// **نکات مهم:**
    /// - channelKey باید در header X-Channel-Key ارسال شود
    /// - serial دارایی باید معتبر باشد
    /// - شماره موبایل باید با فرمت 09xxxxxxxxx باشد
    /// 
    /// **مثال درخواست:**
    /// ```
    /// POST /api/assets/consume
    /// X-Channel-Key: bajet
    /// Authorization: Bearer {token}
    /// Content-Type: application/json
    /// 
    /// {
    ///   "tenantId": 1,
    ///   "customerMobile": "09123456789",
    ///   "serial": "ASSET-001"
    /// }
    /// ```
    /// </remarks>
    /// <param name="dto">اطلاعات مصرف دارایی</param>
    /// <param name="channelKey">کلید کانال (از header)</param>
    /// <returns>پاسخ شامل OutboxId و State</returns>
    /// <response code="202">درخواست مصرف دارایی با موفقیت در صف قرار گرفت</response>
    /// <response code="400">خطا در اعتبارسنجی درخواست</response>
    [HttpPost("consume")]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OutboxResponseDto), StatusCodes.Status202Accepted)]
    public async Task<Results<Accepted<OutboxResponseDto>, ProblemHttpResult>> Consume(
        [FromBody] ConsumeAssetRequestDto dto)
    {
        var stopwatch = Stopwatch.StartNew();
        using var activity = ChannelApiTelemetry.StartActivity("Assets.Consume");
        activity?.SetTag("asset.serial", dto.Serial);
        activity?.SetTag("customer.mobile", dto.CustomerMobile);

        logger.LogInformation(
            "Consuming asset {Serial} for customer {CustomerMobile}",
            dto.Serial, dto.CustomerMobile);

        try
        {
            // استخراج clientId از claim توکن
            var clientId = GetClientIdFromToken();
            if (string.IsNullOrWhiteSpace(clientId))
            {
                stopwatch.Stop();
                activity?.SetStatus(ActivityStatusCode.Error, "Client ID not found in token");
                logger.LogWarning("Client ID not found in token. Duration: {Duration}ms", stopwatch.ElapsedMilliseconds);
                ChannelApiTelemetry.RecordHttpRequest("POST", "assets/consume", 400, stopwatch.Elapsed.TotalSeconds, null);
                return TypedResults.Problem("شناسه کلاینت در توکن یافت نشد", statusCode: StatusCodes.Status400BadRequest);
            }

            activity?.SetTag("client.id", clientId);

            // دریافت کانال بر اساس clientId
            var channelDto = await channelService.GetChannelByClientIdAsync(clientId, CancellationToken.None);
            if (channelDto == null)
            {
                stopwatch.Stop();
                activity?.SetStatus(ActivityStatusCode.Error, "Channel not found");
                logger.LogWarning("Channel not found for client ID {ClientId}. Duration: {Duration}ms", clientId, stopwatch.ElapsedMilliseconds);
                ChannelApiTelemetry.RecordHttpRequest("POST", "assets/consume", 404, stopwatch.Elapsed.TotalSeconds, null);
                return TypedResults.Problem("کانال با شناسه کلاینت یافت نشد", statusCode: StatusCodes.Status404NotFound);
            }

            var channelKey = channelDto.Key;
            activity?.SetTag("channel.key", channelKey);
            activity?.SetTag("tenant.id", channelDto.TenantId.ToString());

            ChannelApiTelemetry.RecordControllerRequest("Assets", "Consume", channelKey);

            var command = new EnqueueConsumeAssetCommand(
                channelDto.TenantId,
                channelDto.Id,
                dto.CustomerMobile,
                dto.Serial,
                GetCorrelationId(),
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                dto.Attributes);

            OutboxResponse response = await Sender.Send(command);
            
            stopwatch.Stop();
            activity?.SetStatus(ActivityStatusCode.Ok);
            activity?.SetTag("outbox.id", response.OutboxId.ToString());
            logger.LogInformation(
                "Consume asset queued for channel {ChannelKey} with OutboxId {OutboxId}. Duration: {Duration}ms",
                channelKey, response.OutboxId, stopwatch.ElapsedMilliseconds);

            ChannelApiTelemetry.RecordHttpRequest("POST", "assets/consume", 202, stopwatch.Elapsed.TotalSeconds, channelKey);
            ChannelApiTelemetry.RecordAssetConsumed(channelKey, dto.Serial);

            return TypedResults.Accepted(string.Empty, new Dto.OutboxResponseDto(response));
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.SetTag("error.type", ex.GetType().Name);
            activity?.SetTag("error.message", ex.Message);
            activity?.SetTag("error.stacktrace", ex.StackTrace);
            logger.LogError(ex,
                "Error consuming asset {Serial} for customer {CustomerMobile}. Duration: {Duration}ms",
                dto.Serial, dto.CustomerMobile, stopwatch.ElapsedMilliseconds);
            ChannelApiTelemetry.RecordHttpRequest("POST", "assets/consume", 500, stopwatch.Elapsed.TotalSeconds, null);
            throw;
        }
    }

    private string? GetCorrelationId()
    {
        if (HttpContext.Request.Headers.TryGetValue("X-Correlation-ID", out StringValues values))
        {
            return values.ToString();
        }

        return null;
    }
}

public sealed record ConsumeAssetRequestDto
{
    [Required]
    [MaxLength(15)]
    public string CustomerMobile { get; init; } = null!;

    [Required]
    [MaxLength(128)]
    public string Serial { get; init; } = null!;
    public AttributesValuesList? Attributes { get; init; }
}