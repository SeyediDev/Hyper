using Hyper.Domain.Entities.Promotions.Plans.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;

public class CustomerPlanConfiguration : IEntityTypeConfiguration<CustomerPlan>
{
    public void Configure(EntityTypeBuilder<CustomerPlan> builder)
    {
        builder.ToTable("CustomerPlans");

        builder.HasKey(cp => cp.Id);

        builder.Property(cp => cp.PurchaseDate)
            .IsRequired();

        builder.Property(cp => cp.StartDate)
            .IsRequired();

        builder.Property(cp => cp.ExpiryDate)
            .IsRequired();

        builder.Property(cp => cp.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(cp => cp.PaidAmount)
            .IsRequired();

        builder.Property(cp => cp.UsageCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(cp => cp.TotalDiscountReceived)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(cp => cp.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(cp => cp.Notes)
            .HasMaxLength(500);

        // Relationships
        builder.HasOne(cp => cp.CustomerTenant)
            .WithMany()
            .HasForeignKey(cp => cp.CustomerTenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cp => cp.Plan)
            .WithMany(p => p.CustomerPlans)
            .HasForeignKey(cp => cp.PlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cp => cp.CustomerTransaction)
            .WithMany()
            .HasForeignKey(cp => cp.CustomerTransactionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(cp => cp.CustomerTenantId);
        builder.HasIndex(cp => cp.PlanId);
        builder.HasIndex(cp => new { cp.CustomerTenantId, cp.Status });
        builder.HasIndex(cp => new { cp.CustomerTenantId, cp.Status, cp.IsActive, cp.ExpiryDate });
    }
}

