using Ardalis.GuardClauses;
using Hyper.Domain.Repository;
using Hyper.Infrastructure.Data.Repository;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neo.Domain.Repository;
using Neo.Infrastructure.Features.Queue.Hangfire;
using Quartz;

namespace Hyper.Infrastructure;

public static class DependencyInjection
{
	public static void AddHyperRepositories(this IServiceCollection services, IConfiguration configuration)
	{
		// Initialize Mapster configurations
		_ = typeof(Configuration.AttributeValueMappingConfig);
        var commandConnectionString = configuration.GetConnectionString($"{nameof(DomainProvider.Domain)}CommandConnection");
        Guard.Against.Null(commandConnectionString, message: $"Connection string '{nameof(DomainProvider.Domain)}CommandConnection' not found.");
        services.AddDbContext<HyperContextCommand>((serviceProvider, options) =>
        {
            options.UseSqlServer(commandConnectionString);
            options.AddInterceptors(serviceProvider.GetServices<ISaveChangesInterceptor>());
        });
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
    }

    public static void UseHyper(this IApplicationBuilder app,
        IConfiguration configuration, IHostEnvironment environment)
    {
        app.UseNeoHangfireDashboard(configuration, environment);
    }
}