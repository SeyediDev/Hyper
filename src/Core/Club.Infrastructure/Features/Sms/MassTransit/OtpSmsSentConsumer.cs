using Neo.Domain.Features.Sms;
using Neo.Domain.Features.Sms.Dto;
using MassTransit;

namespace Hyper.Infrastructure.Features.Sms.MassTransit;

public class OtpSmsSentConsumer(ISmsProvidorService smsProvidorService) 
    : IConsumer<OtpSmsDto>
{
    public async Task Consume(ConsumeContext<OtpSmsDto> context)
    {
        await smsProvidorService.SendOtpAsync(context.Message);
    }
}
