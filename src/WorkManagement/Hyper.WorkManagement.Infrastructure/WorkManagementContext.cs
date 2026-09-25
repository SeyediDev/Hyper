using Hyper.WorkManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace Hyper.WorkManagement.Infrastructure;

public sealed class WorkManagementContext(DbContextOptions<WorkManagementContext> options) : DbContext(options)
{
    public DbSet<WorkItem> WorkItems => Set<WorkItem>();
    public DbSet<WorkRole> WorkRoles => Set<WorkRole>();
    public DbSet<WorkItemLog> WorkItemLogs => Set<WorkItemLog>();
    public DbSet<ChatWorkIntake> ChatWorkIntakes => Set<ChatWorkIntake>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<WorkItem>().ToTable("WorkItems").HasKey(x => x.Id);
        b.Entity<WorkItem>().HasIndex(x => x.Key).IsUnique();
        b.Entity<WorkItem>().Property(x => x.Key).HasMaxLength(40).IsRequired();
        b.Entity<WorkItem>().Property(x => x.Domain).HasMaxLength(80).IsRequired();
        b.Entity<WorkRole>().ToTable("WorkRoles").HasKey(x => x.Id);
        b.Entity<WorkRole>().HasIndex(x => x.Key).IsUnique();
        b.Entity<WorkItemLog>().ToTable("WorkItemLogs").HasKey(x => x.Id);
        b.Entity<ChatWorkIntake>().ToTable("ChatWorkIntakes").HasKey(x => x.Id);
        b.Entity<ChatWorkIntake>().HasIndex(x => new { x.ChatId, x.CreatedAtUtc });
    }
}
