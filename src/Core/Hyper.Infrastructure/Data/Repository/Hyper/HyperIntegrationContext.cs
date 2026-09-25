using Hyper.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Hyper.Infrastructure.Data.Repository.Hyper;

/// <summary>Integration persistence without the copied Club business model.</summary>
public sealed class HyperIntegrationContext(DbContextOptions<HyperIntegrationContext> options) : DbContext(options)
{
    public DbSet<IntegrationScenarioJob> IntegrationScenarioJobs => Set<IntegrationScenarioJob>();
    public DbSet<IntegrationOutboxMessage> IntegrationOutbox => Set<IntegrationOutboxMessage>();
    public DbSet<IntegrationMerchantAccess> IntegrationMerchantAccess => Set<IntegrationMerchantAccess>();
    public DbSet<IntegrationAdminSimulation> IntegrationAdminSimulations => Set<IntegrationAdminSimulation>();
    public DbSet<IntegrationTokenRequest> IntegrationTokenRequests => Set<IntegrationTokenRequest>();
    public DbSet<ExternalIntegrationConnection> ExternalIntegrationConnections => Set<ExternalIntegrationConnection>();
    public DbSet<ExternalOAuthToken> ExternalOAuthTokens => Set<ExternalOAuthToken>();
    public DbSet<IntegrationCustomerMapping> IntegrationCustomerMappings => Set<IntegrationCustomerMapping>();
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
        modelBuilder.Entity<IntegrationScenarioJob>(entity =>
        {
            entity.ToTable("IntegrationScenarioJobs", "dbo");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.EventId).HasMaxLength(128).UseCollation("Latin1_General_100_BIN2");
            entity.Property(x => x.TenantId).HasMaxLength(128);
            entity.Property(x => x.ErrorCode).HasMaxLength(100);
            entity.Property(x => x.Item).HasConversion<byte>();
            entity.Property(x => x.Trigger).HasConversion<byte>();
            entity.Property(x => x.Status).HasConversion<byte>();
            entity.HasIndex(x => new { x.ConnectionId, x.EventId }).IsUnique();
        });
        modelBuilder.ApplyConfiguration(new IntegrationOutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new IntegrationMerchantAccessConfiguration());
        modelBuilder.ApplyConfiguration(new IntegrationAdminSimulationConfiguration());
        modelBuilder.ApplyConfiguration(new IntegrationTokenRequestConfiguration());
        modelBuilder.ApplyConfiguration(new ExternalIntegrationConnectionConfiguration());
        modelBuilder.ApplyConfiguration(new ExternalOAuthTokenConfiguration());
        modelBuilder.Entity<IntegrationCustomerMapping>(entity =>
        {
            entity.ToTable("IntegrationCustomerMappings", "dbo");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TenantId).HasMaxLength(30).IsRequired();
            entity.Property(x => x.ExternalCustomerId).HasMaxLength(128).IsRequired();
            entity.HasIndex(x => new { x.ShopId, x.TenantId, x.ExternalCustomerId }).IsUnique()
                .HasDatabaseName("UX_IntegrationCustomerMappings_ExternalIdentity");
            entity.HasIndex(x => new { x.ShopId, x.TenantId, x.PersonId }).IsUnique()
                .HasDatabaseName("UX_IntegrationCustomerMappings_Person");
        });
        modelBuilder.ApplyConfiguration(new ExternalProductMappingConfiguration());
        modelBuilder.ApplyConfiguration(new IntegrationSyncRunConfiguration());
        modelBuilder.ApplyConfiguration(new IntegrationWebhookInboxConfiguration());
        modelBuilder.ApplyConfiguration(new ExternalOrderMappingConfiguration());
        modelBuilder.ApplyConfiguration(new InventoryReservationLogConfiguration());
        modelBuilder.ApplyConfiguration(new IntegrationEventAuditConfiguration());
    }
}
