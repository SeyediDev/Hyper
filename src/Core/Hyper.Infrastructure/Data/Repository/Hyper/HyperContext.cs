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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Integration owns a separate DbContext/database. Its configurations must
        // never be discovered by Hyperyek's core command/query contexts.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HyperContext<>).Assembly,
            type => !typeof(global::Hyper.Infrastructure.Data.Configurations.IIntegrationEntityConfiguration)
                .IsAssignableFrom(type));
    }
}
