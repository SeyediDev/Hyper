using Hyper.WorkManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace Hyper.WorkManagement.Infrastructure;

public sealed class WorkManagementContext(DbContextOptions<WorkManagementContext> options) : DbContext(options)
{
    public DbSet<WorkItem> WorkItems => Set<WorkItem>();
    public DbSet<WorkProject> WorkProjects => Set<WorkProject>();
    public DbSet<WorkRole> WorkRoles => Set<WorkRole>();
    public DbSet<WorkItemLog> WorkItemLogs => Set<WorkItemLog>();
    public DbSet<ChatWorkIntake> ChatWorkIntakes => Set<ChatWorkIntake>();
    public DbSet<WorkItemDependency> WorkItemDependencies => Set<WorkItemDependency>();
    public DbSet<WorkItemCommit> WorkItemCommits => Set<WorkItemCommit>();
    public DbSet<WorkItemTestEvidence> WorkItemTestEvidence => Set<WorkItemTestEvidence>();
    public DbSet<WorkItemTimeEntry> WorkItemTimeEntries => Set<WorkItemTimeEntry>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<WorkItem>().ToTable("WorkItems").HasKey(x => x.Id);
        b.Entity<WorkItem>().HasOne(x => x.Project).WithMany().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<WorkItem>().HasOne<WorkItem>().WithMany().HasForeignKey(x => x.ParentWorkItemId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<WorkProject>().ToTable("Projects").HasKey(x => x.Id);
        b.Entity<WorkProject>().HasIndex(x => x.Key).IsUnique();
        b.Entity<WorkProject>().Property(x => x.Key).HasMaxLength(80).IsRequired();
        b.Entity<WorkItem>().HasIndex(x => new { x.ProjectId, x.Key }).IsUnique();
        b.Entity<WorkItem>().Property(x => x.Key).HasMaxLength(40).IsRequired();
        b.Entity<WorkItem>().Property(x => x.Domain).HasMaxLength(80).IsRequired();
        b.Entity<WorkRole>().ToTable("WorkRoles").HasKey(x => x.Id);
        b.Entity<WorkRole>().HasIndex(x => x.Key).IsUnique();
        b.Entity<WorkItemLog>().ToTable("WorkItemLogs").HasKey(x => x.Id);
        b.Entity<ChatWorkIntake>().ToTable("ChatWorkIntakes").HasKey(x => x.Id);
        b.Entity<ChatWorkIntake>().HasIndex(x => new { x.ChatId, x.CreatedAtUtc });
        b.Entity<WorkItemDependency>().ToTable("WorkItemDependencies").HasKey(x => x.Id);
        b.Entity<WorkItemCommit>().ToTable("WorkItemCommits").HasKey(x => x.Id);
        b.Entity<WorkItemTestEvidence>().ToTable("WorkItemTestEvidence").HasKey(x => x.Id);
        b.Entity<WorkItemTimeEntry>().ToTable("WorkItemTimeEntries").HasKey(x => x.Id);
        b.Entity<WorkItemTimeEntry>().HasIndex(x => new { x.WorkItemId, x.EndedAtUtc });
    }
}
