using Hyper.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Hyper.Infrastructure.Data.Repository.Hyper;

/// <summary>Integration persistence without the copied Club business model.</summary>
public sealed class HyperIntegrationContext(DbContextOptions<HyperIntegrationContext> options) : DbContext(options)
{
    public DbSet<IntegrationOutboxMessage> IntegrationOutbox => Set<IntegrationOutboxMessage>();
    public DbSet<IntegrationMerchantAccess> IntegrationMerchantAccess => Set<IntegrationMerchantAccess>();
    public DbSet<IntegrationAdminSimulation> IntegrationAdminSimulations => Set<IntegrationAdminSimulation>();
    public DbSet<IntegrationTokenRequest> IntegrationTokenRequests => Set<IntegrationTokenRequest>();
    public DbSet<ExternalIntegrationConnection> ExternalIntegrationConnections => Set<ExternalIntegrationConnection>();
    public DbSet<ExternalProductMapping> ExternalProductMappings => Set<ExternalProductMapping>();
    public DbSet<IntegrationSyncRun> IntegrationSyncRuns => Set<IntegrationSyncRun>();
    public DbSet<IntegrationWebhookInbox> IntegrationWebhookInbox => Set<IntegrationWebhookInbox>();
    public DbSet<ExternalOrderMapping> ExternalOrderMappings => Set<ExternalOrderMapping>();
    public DbSet<InventoryReservationLog> InventoryReservationLogs => Set<InventoryReservationLog>();
    public DbSet<IntegrationEventAudit> IntegrationEventAudits => Set<IntegrationEventAudit>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Conventions.Remove(typeof(ForeignKeyIndexConvention));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new IntegrationOutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new IntegrationMerchantAccessConfiguration());
        modelBuilder.ApplyConfiguration(new IntegrationAdminSimulationConfiguration());
        modelBuilder.ApplyConfiguration(new IntegrationTokenRequestConfiguration());
        modelBuilder.ApplyConfiguration(new ExternalIntegrationConnectionConfiguration());
        modelBuilder.ApplyConfiguration(new ExternalProductMappingConfiguration());
        modelBuilder.ApplyConfiguration(new IntegrationSyncRunConfiguration());
        modelBuilder.ApplyConfiguration(new IntegrationWebhookInboxConfiguration());
        modelBuilder.ApplyConfiguration(new ExternalOrderMappingConfiguration());
        modelBuilder.ApplyConfiguration(new InventoryReservationLogConfiguration());
        modelBuilder.ApplyConfiguration(new IntegrationEventAuditConfiguration());
    }
}
