using Hyper.Domain.Features.Client;
using Hyper.Domain.Features.Themes;
using Microsoft.Extensions.Configuration;
using Neo.Domain;
using Neo.Domain.Features.Client;

namespace Hyper.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddHyperDomainServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddNeoDomainServices(configuration);
        services.AddScoped<ILoginUserService<UserId>, LoginUserService>();
        
        // Theme Service
        services.AddSingleton<IThemeService, ThemeService>();
        
        return services;
    }
}
