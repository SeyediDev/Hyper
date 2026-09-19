using Ardalis.GuardClauses;
using Hyper.Infrastructure.Data.Repository;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Infrastructure.Features.Integrations;
using Hyper.SDK;
using Hyper.SDK.Clients;
using Hyper.SDK.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Neo.Domain.Entities.Base;
using Neo.Infrastructure.Features.Queue.Hangfire;
using Quartz;

namespace Hyper.Infrastructure;

public static class DependencyInjection
{
	public static void AddHyperRepositories(this IServiceCollection services, IConfiguration configuration)
	{
		// Initialize Mapster configurations
        var commandConnectionString = configuration.GetConnectionString($"{nameof(DomainProvider.Domain)}CommandConnection");
        Guard.Against.Null(commandConnectionString, message: $"Connection string '{nameof(DomainProvider.Domain)}CommandConnection' not found.");
        services.AddDbContext<HyperContextCommand>((serviceProvider, options) =>
        {
            options.UseSqlServer(commandConnectionString);
            options.AddInterceptors(serviceProvider.GetServices<ISaveChangesInterceptor>());
        });
        services.AddHyperIntegrations(commandConnectionString);
        services.Configure<IntegrationInventoryCaptureOptions>(configuration.GetSection("IntegrationInventoryCapture"));
        services.AddScoped<IHyperUnitOfWorkCommand>(serviceProvider => serviceProvider.GetRequiredService<HyperContextCommand>());

        var queryConnectionString = configuration.GetConnectionString($"{nameof(DomainProvider.Domain)}QueryConnection");
        Guard.Against.Null(queryConnectionString, message: $"Connection string '{nameof(DomainProvider.Domain)}QueryConnection' not found.");
        services.AddDbContextPool<HyperContextQuery>(options => options.UseSqlServer(queryConnectionString)
                   .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
        services.AddScoped<IHyperUnitOfWorkQuery>(serviceProvider => serviceProvider.GetRequiredService<HyperContextQuery>());

		services.AddScoped(typeof(ICommandRepository<>), typeof(CommandHyperEntityRepository<>));
		services.AddScoped(typeof(IQueryRepository<>), typeof(QueryHyperEntityRepository<>));
        services.AddScoped(typeof(ICommandRepositoryL<>), typeof(CommandHyperEntityRepositoryL<>));
        services.AddScoped(typeof(IQueryRepositoryL<>), typeof(QueryHyperEntityRepositoryL<>));
        services.AddScoped(typeof(ICommandRepository<,>), typeof(CommandHyperEntityRepository<,>));
		services.AddScoped(typeof(IQueryRepository<,>), typeof(QueryHyperEntityRepository<,>));

		services.AddScoped<IUserQueryRepository, UserQueryRepository>();
		services.AddScoped<ICultureTermQueryRepository, CultureTermQueryRepository>();

	// Basalam OAuth2
	services.Configure<BasalamOAuthSettings>(configuration.GetSection("Basalam"));
	services.AddScoped<BasalamOAuthStore>();
	services.AddHttpClient<BasalamOAuthService>()
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false })
			.ConfigureHttpClient(client =>
			{
				client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("User-Agent", "HyperIntegration/1.0");
			});

        // Hyper SDK
        services.AddBasalamSdk(configuration.GetSection("Basalam"));
        services.AddSingleton<IBasalamHttpClient>(sp =>
        {
            var config = sp.GetRequiredService<IOptionsMonitor<BasalamConfig>>().CurrentValue;
            return new BasalamHttpClient(config);
        });

	services.AddDataProtection();
	}

    public static void UseHyper(this IApplicationBuilder app,
        IConfiguration configuration, IHostEnvironment environment)
    {
        app.UseNeoHangfireDashboard(configuration, environment);
    }
}
