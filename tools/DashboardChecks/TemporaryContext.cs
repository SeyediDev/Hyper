global using Hyper.Integration.Domain.Entities.Integrations;
global using Hyper.Integration.Domain.Features.Integrations;
using Microsoft.EntityFrameworkCore;

namespace Hyper.Infrastructure.Data.Repository.Hyper;

// Only the table mapping is replaced. Query, entities and API mapping are linked
// from production source. SQL Server drops these tables when the connection closes.
public sealed class HyperIntegrationContext(DbContextOptions<HyperIntegrationContext> options) : DbContext(options)
{
    public DbSet<ExternalIntegrationConnection> ExternalIntegrationConnections => Set<ExternalIntegrationConnection>();
    public DbSet<ExternalProductMapping> ExternalProductMappings => Set<ExternalProductMapping>();
    public DbSet<IntegrationSyncRun> IntegrationSyncRuns => Set<IntegrationSyncRun>();
    public DbSet<IntegrationWebhookInbox> IntegrationWebhookInbox => Set<IntegrationWebhookInbox>();
    public DbSet<IntegrationOutboxMessage> IntegrationOutbox => Set<IntegrationOutboxMessage>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<ExternalIntegrationConnection>().ToTable("#DashboardConnections");
        model.Entity<ExternalProductMapping>().ToTable("#DashboardMappings");
        model.Entity<IntegrationSyncRun>().ToTable("#DashboardRuns");
        model.Entity<IntegrationWebhookInbox>().ToTable("#DashboardInbox");
        model.Entity<IntegrationOutboxMessage>().ToTable("#DashboardOutbox");
        foreach (var entity in model.Model.GetEntityTypes())
        {
            model.Entity(entity.ClrType).Property("Id").ValueGeneratedNever();
            foreach (var property in entity.GetProperties().Where(x => x.ClrType == typeof(decimal) || x.ClrType == typeof(decimal?)))
            {
                property.SetPrecision(18);
                property.SetScale(4);
            }
        }
    }
}
