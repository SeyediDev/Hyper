using Hyper.Domain.Entities.Channels;
using Hyper.Infrastructure.Data.Repository.Hyper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Hyper.Infrastructure.Tests;

public sealed class HyperContextModelTests
{
    [Fact]
    public void Model_Should_Map_Only_One_EventChannel_Entity()
    {
        var options = new DbContextOptionsBuilder<HyperContextQuery>()
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=HyperModelValidation")
            .Options;

        using var context = new HyperContextQuery(options);

        var eventChannelEntities = context.Model
            .GetEntityTypes()
            .Where(entityType => entityType.ClrType.Name == nameof(EventChannel))
            .ToList();

        eventChannelEntities.Should().ContainSingle();
        eventChannelEntities[0].ClrType.Should().Be<EventChannel>();
        eventChannelEntities[0].GetSchema().Should().Be("CoreConfig");
        eventChannelEntities[0].GetTableName().Should().Be("EventChannels");
    }
}
