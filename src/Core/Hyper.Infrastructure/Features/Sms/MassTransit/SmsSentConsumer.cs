using Neo.Domain.Features.Sms;
using Neo.Domain.Features.Sms.Dto;
using MassTransit;

namespace Hyper.Infrastructure.Features.Sms.MassTransit;

public class SmsSentConsumer(ISmsProvidorService smsProvidorService) : IConsumer<SmsDto>
{
    public async Task Consume(ConsumeContext<SmsDto> context)
    {
        await smsProvidorService.SendAsync(context.Message);
    }
}