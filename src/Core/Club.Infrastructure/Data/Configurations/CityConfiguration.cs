using Hyper.Domain.Entities.Geography;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        // Configure decimal properties with explicit precision and scale
        // Latitude and Longitude need higher precision (10,7) for geographic coordinates
        builder.Property(c => c.Latitude)
            .HasColumnType("decimal(10,7)");

        builder.Property(c => c.Longitude)
            .HasColumnType("decimal(10,7)");
    }
}

