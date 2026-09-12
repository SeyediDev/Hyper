using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Hyper.Infrastructure.Data.Repository.Hyper;

public partial class HyperContextQuery(DbContextOptions<HyperContextQuery> options)
    : HyperContext<HyperContextQuery>(options), IHyperUnitOfWorkQuery
{
    protected override Assembly ContextAssembly => typeof(HyperContextQuery).Assembly;
}
