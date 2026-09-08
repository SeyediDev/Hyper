using Hyper.Domain.Entities.Geography;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;

public class ProvinceConfiguration : IEntityTypeConfiguration<Province>
{
    public void Configure(EntityTypeBuilder<Province> builder)
    {
        // Configure decimal properties with explicit precision and scale
        // Latitude and Longitude need higher precision (10,7) for geographic coordinates
        builder.Property(p => p.Latitude)
            .HasColumnType("decimal(10,7)");

        builder.Property(p => p.Longitude)
            .HasColumnType("decimal(10,7)");
    }
}

