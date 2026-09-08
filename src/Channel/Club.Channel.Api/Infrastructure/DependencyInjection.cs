using Hyper.Channel.Application.Features.Channel.Commands;
using FluentValidation;

namespace Hyper.Channel.Api.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddChannelApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var applicationAssembly = typeof(EnqueueEventCommand).Assembly;
        
        services.AddValidatorsFromAssembly(applicationAssembly);
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(applicationAssembly);
        });

        return services;
    }
}