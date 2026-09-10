using Neo.Domain.Features.Sms;
using Neo.Infrastructure.Features.ServiceCaller;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hyper.Infrastructure.Features.Sms.SmsIR;

public static class DependencyInjection
{
    public static IServiceCollection AddSmsIRServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<ISmsProvidorService, SmsClientService>(options =>
        {
            options.BaseAddress = new Uri(configuration["Sms:Url"]!);
            options.DefaultRequestHeaders.Add("x-api-key", configuration["Sms:X-API-kEY"]!);
        }).AddHttpMessageHandler<LoggingHandler>();
        return services;
    }
}
