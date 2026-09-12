using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Hyper.Infrastructure.Data.Repository.Hyper;

public partial class HyperContextCommand(DbContextOptions<HyperContextCommand> options)
    : HyperContext<HyperContextCommand>(options), IHyperUnitOfWorkCommand
{
    protected override Assembly ContextAssembly => typeof(HyperContextCommand).Assembly;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
