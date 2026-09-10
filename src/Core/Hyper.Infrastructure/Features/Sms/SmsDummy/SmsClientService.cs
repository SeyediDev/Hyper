using Neo.Domain.Features.Sms;
using Neo.Domain.Features.Sms.Dto;
using Microsoft.Extensions.Logging;

namespace Hyper.Infrastructure.Features.Sms.SmsDummy;

public class SmsClientService(
    ILogger<SmsClientService> logger
    ) : ISmsProvidorService
{
    public Task<bool> SendAsync(SmsDto model)
    {
        logger.LogInformation("SendAsync model {@model}", model);
        return Task.FromResult(true);
    }

    public Task<bool> SendOtpAsync(OtpSmsDto model)
    {
        logger.LogInformation("SendOtpAsync model {@model}", model);
        return Task.FromResult(true);
    }
}