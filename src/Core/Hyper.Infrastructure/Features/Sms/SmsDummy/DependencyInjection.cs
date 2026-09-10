using Neo.Domain.Features.Sms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hyper.Infrastructure.Features.Sms.SmsDummy;

public static class DependencyInjection
{
    public static IServiceCollection AddSmsDummyServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ISmsProvidorService, SmsClientService>();
        return services;
    }
}
