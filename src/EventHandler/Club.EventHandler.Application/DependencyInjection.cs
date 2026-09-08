using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Hyper.EventHandler.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddEventHandlerApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        return services;
    }
}

