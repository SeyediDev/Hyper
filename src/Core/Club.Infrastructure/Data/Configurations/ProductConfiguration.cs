using Hyper.Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Configure decimal properties with explicit precision and scale
        // to avoid EF Core warnings about silent truncation
        builder.Property(p => p.ReorderThreshold)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.AveragePurchasesPerCustomerLifetime)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.RepeatPurchaseRate)
            .HasColumnType("decimal(18,2)");
    }
}

