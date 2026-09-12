using Neo.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hyper.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddHyperApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddNeoApplicationServices(configuration, typeof(DependencyInjection).Assembly);
        
        services.AddHttpClient();
        
        return services;
    }
}
