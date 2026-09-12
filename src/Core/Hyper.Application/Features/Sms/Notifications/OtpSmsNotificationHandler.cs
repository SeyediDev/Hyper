using Neo.Domain.Features.Sms.Dto;

namespace Hyper.Application.Features.Sms.Notifications;

/// <summary>
/// Handler for OTP SMS notification from Domain
/// </summary>
public class OtpSmsNotificationHandler(
    ISmsProvidorService smsProvidorService,
    ILogger<OtpSmsNotificationHandler> logger) 
    : INotificationHandler<OtpSmsNotification>
{
    public async Task Handle(OtpSmsNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Publishing OTP SMS notification for {Mobile}", notification.Mobile);
            
            var otpDto = notification.ToDto();
            var result = await smsProvidorService.SendOtpAsync(otpDto);
            
            if (result)
            {
                logger.LogInformation("OTP SMS notification processed successfully for {Mobile}", notification.Mobile);
            }
            else
            {
                logger.LogWarning("Failed to process OTP SMS notification for {Mobile}", notification.Mobile);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing OTP SMS notification for {Mobile}", notification.Mobile);
            // Note: در Notification Handler معمولاً exception رو throw نمی‌کنیم
            // چون نباید جلوی Publish به handlers دیگه رو بگیره
        }
    }
}

