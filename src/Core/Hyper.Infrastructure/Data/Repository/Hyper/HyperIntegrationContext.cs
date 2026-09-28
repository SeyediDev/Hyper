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
        modelBuilder.Entity<IntegrationProductPreparation>(e =>
        {
            e.ToTable("IntegrationProductPreparations", "dbo"); e.HasKey(x => x.Id);
            e.Property(x => x.SourceProductId).HasMaxLength(128).UseCollation("Latin1_General_100_BIN2");
            e.HasIndex(x => new { x.ConnectionId, x.Direction, x.SourceProductId }).IsUnique();
            e.HasOne<ExternalIntegrationConnection>().WithMany().HasForeignKey(x => x.ConnectionId).OnDelete(DeleteBehavior.NoAction);
        });
        modelBuilder.Entity<IntegrationProductPreparationHistory>(e =>
        {
            e.ToTable("IntegrationProductPreparationHistory", "dbo"); e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.PreparationId, x.Revision }).IsUnique();
            e.HasOne<IntegrationProductPreparation>().WithMany().HasForeignKey(x => x.PreparationId).OnDelete(DeleteBehavior.NoAction);
        });
        modelBuilder.Entity<IntegrationProductPreparationPolicy>(e =>
        {
            e.ToTable("IntegrationProductPreparationPolicies", "dbo"); e.HasKey(x => x.Id);
            e.Property(x => x.UpdatedBy).HasMaxLength(256);
            e.HasIndex(x => new { x.ConnectionId, x.Direction }).IsUnique();
            e.HasOne<ExternalIntegrationConnection>().WithMany().HasForeignKey(x => x.ConnectionId).OnDelete(DeleteBehavior.NoAction);
        });
        modelBuilder.Entity<IntegrationProductCreation>(entity =>
        {
            entity.ToTable("IntegrationProductCreations", "dbo");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.RequestJson).IsRequired();
            entity.Property(x => x.PayloadJson).IsRequired();
            entity.Property(x => x.AccountIdentifier).HasMaxLength(200).IsRequired();
            entity.Property(x => x.ExternalProductId).HasMaxLength(200);
            entity.Property(x => x.ErrorCode).HasMaxLength(100);
            entity.HasIndex(x => new { x.ConnectionId, x.RequestId }).IsUnique();
            // Includes failed/ambiguous attempts: a new request ID must never
            // bypass an uncertain remote creation for the same source product.
            entity.HasIndex(x => new { x.ConnectionId, x.HyperProductId }).IsUnique();
            entity.HasOne<ExternalIntegrationConnection>().WithMany().HasForeignKey(x => x.ConnectionId).OnDelete(DeleteBehavior.NoAction);
        });
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
        modelBuilder.Entity<IntegrationVersionOwnership>(entity =>
        {
            entity.ToTable("IntegrationVersionOwnership", "dbo", table =>
                table.HasCheckConstraint("CK_IntegrationVersionOwnership_Source", "[Source] IN (1,2) AND [VersionFloor] >= 0"));
            entity.HasKey(x => x.MappingId);
            entity.Property(x => x.MappingId).ValueGeneratedNever();
            entity.HasOne<ExternalProductMapping>().WithMany().HasForeignKey(x => x.MappingId).OnDelete(DeleteBehavior.NoAction);
        });
        modelBuilder.ApplyConfiguration(new IntegrationSyncRunConfiguration());
        modelBuilder.ApplyConfiguration(new IntegrationWebhookInboxConfiguration());
        modelBuilder.ApplyConfiguration(new ExternalOrderMappingConfiguration());
        modelBuilder.ApplyConfiguration(new InventoryReservationLogConfiguration());
        modelBuilder.ApplyConfiguration(new IntegrationEventAuditConfiguration());
    }
}
