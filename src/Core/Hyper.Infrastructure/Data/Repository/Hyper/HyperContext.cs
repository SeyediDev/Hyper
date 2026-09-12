using Neo.Domain.Entities.Common;
using Neo.Infrastructure.Data.Repository.Ef;
using Microsoft.EntityFrameworkCore;
using User = Hyper.Domain.Entities.Common.User;

namespace Hyper.Infrastructure.Data.Repository.Hyper;

public abstract partial class HyperContext<TContext>(DbContextOptions<TContext> options)
    : EfDbContext<TContext>(options), IUnitOfWork
    where TContext : DbContext
{
    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<DocumentType> DocumentTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<CultureTerm> CultureTerms { get; set; }

    public virtual DbSet<Language> Language { get; set; }

    //public virtual DbSet<FaqType> Type { get; set; }

    public virtual DbSet<Faq> Faq { get; set; }
    public virtual DbSet<Help> Help { get; set; }

    public virtual DbSet<ExternalIntegrationConnection> ExternalIntegrationConnections { get; set; }
    public virtual DbSet<ExternalProductMapping> ExternalProductMappings { get; set; }
    public virtual DbSet<IntegrationSyncRun> IntegrationSyncRuns { get; set; }
    public virtual DbSet<IntegrationWebhookInbox> IntegrationWebhookInbox { get; set; }
    public virtual DbSet<ExternalOrderMapping> ExternalOrderMappings { get; set; }
    public virtual DbSet<InventoryReservationLog> InventoryReservationLogs { get; set; }
    public virtual DbSet<IntegrationEventAudit> IntegrationEventAudits { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply all entity configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HyperContext<>).Assembly);
    }
}
