using System.Text;
using Neo.Application.Exceptions;
using Neo.Common.Extensions;
using Neo.Domain.Features.Sms;
using Neo.Domain.Features.Sms.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hyper.Infrastructure.Features.Sms.SmsIR;

public class SmsClientService(
    HttpClient httpClient, IConfiguration configuration, ILogger<SmsClientService> logger
    ) : ISmsProvidorService
{
    public Task<bool> SendAsync(SmsDto model)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> SendOtpAsync(OtpSmsDto model)
    {
        try
        {
            var dto = new OtpRequestDto
            {
                Mobile = model.mobile,
                TemplateId = int.Parse(configuration["Sms:OtpTemplateId"]!),
                Parameters =
           [
               new ParametersDto {Name = "code", Value =  model.message}
           ]
            };
            var content = new StringContent(dto.ToJson(), Encoding.UTF8, "application/json");
            var result = await httpClient.PostAsync("v1/send/verify", content);
            logger.LogInformation("opt send {status} {@request} {@response}", result.IsSuccessStatusCode, dto, await result.Content.ReadAsStringAsync());
            result.EnsureSuccessStatusCode();
            return result.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError("send sms {@error}", ex);
            throw new BadRequestException("خطایی رخ داده است. مجددا تلاش کنید.");
        }
        //return await Task.Run(() => true);
    }
}
