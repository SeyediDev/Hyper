using Microsoft.Data.SqlClient;

namespace Hyper.AdminPanel.Web.Infrastructure;

public static class AdminRuntime
{
    public static void ConfigureLocalSql(WebApplicationBuilder builder)
    {
        if (!builder.Configuration.GetValue<bool>("HyperLocalSql:Enabled")) return;
        if (!builder.Environment.IsDevelopment()) throw new InvalidOperationException("Local SQL mode is development-only.");
        var overrides = new Dictionary<string, string?>();
        foreach (var entry in builder.Configuration.GetSection("ConnectionStrings").GetChildren()
            .Append(builder.Configuration.GetSection("Hangfire:ConnectionString")))
        {
            if (string.IsNullOrWhiteSpace(entry.Value) || !entry.Value.Contains('=')) continue;
            SqlConnectionStringBuilder sql;
            try { sql = new(entry.Value); } catch (ArgumentException) { continue; }
            if (string.IsNullOrEmpty(sql.InitialCatalog)) continue;
            // The admin host may have unrelated framework connection strings (for example
            // metadata or telemetry stores). Only the Hyperyek connection is overridden.
            if (sql.InitialCatalog != "Hyperyek") continue;
            if (sql.DataSource is not ("." or "localhost" or "(local)" or "lpc:."))
                throw new InvalidOperationException("Local SQL mode accepts only a local Hyperyek database.");
            sql.DataSource = "lpc:.";
            sql.Encrypt = SqlConnectionEncryptOption.Optional;
            overrides[entry.Path] = sql.ConnectionString;
        }
        builder.Configuration.AddInMemoryCollection(overrides);
    }

    public static void AddAdminJobContracts(this IServiceCollection services, IConfiguration configuration)
    {
        // Retain framework DI contracts without creating storage or executing retired Club jobs.
        services.AddScoped<IJobExecuter, AdminBackgroundJobsUnavailable>();
        services.AddScoped<IRecurringJobsManager, AdminBackgroundJobsUnavailable>();
        services.AddScoped<ICronJobManager, HangfireCronJobManager>();
    }
}
