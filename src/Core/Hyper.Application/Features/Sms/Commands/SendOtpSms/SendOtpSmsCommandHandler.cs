namespace Hyper.Application.Features.Sms.Commands.SendOtpSms;

/// <summary>
/// Handler for sending OTP SMS
/// </summary>
public class SendOtpSmsCommandHandler(
    ISmsProvidorService smsProvidorService,
    ILogger<SendOtpSmsCommandHandler> logger) 
    : IRequestHandler<SendOtpSmsCommand, bool>
{
    public async Task<bool> Handle(SendOtpSmsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Sending OTP SMS to {Mobile}", request.Mobile);
            
            var otpDto = request.ToDto();
            var result = await smsProvidorService.SendOtpAsync(otpDto);
            
            if (result)
            {
                logger.LogInformation("OTP SMS sent successfully to {Mobile}", request.Mobile);
            }
            else
            {
                logger.LogWarning("Failed to send OTP SMS to {Mobile}", request.Mobile);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending OTP SMS to {Mobile}", request.Mobile);
            throw;
        }
    }
}

