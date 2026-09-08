using Hyper.Domain.Entities.CallCenter;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;

public class CustomerInteractionConfiguration : IEntityTypeConfiguration<CustomerInteraction>
{
    public void Configure(EntityTypeBuilder<CustomerInteraction> builder)
    {
        builder.ToTable("CustomerInteractions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Title)
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.OutcomeNotes)
            .HasMaxLength(1000);

        builder.Property(x => x.CustomerFeedback)
            .HasMaxLength(2000);

        builder.Property(x => x.TicketId)
            .HasMaxLength(100);

        builder.Property(x => x.CallId)
            .HasMaxLength(100);

        builder.Property(x => x.ContactPhone)
            .HasMaxLength(20);

        builder.Property(x => x.ContactEmail)
            .HasMaxLength(200);

        builder.Property(x => x.TagsJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.AdditionalDataJson)
            .HasColumnType("nvarchar(max)");

        builder.HasIndex(x => x.CustomerTenantId);
        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.InteractionTypeId);
        builder.HasIndex(x => x.AgentUserId);
        builder.HasIndex(x => x.StartTime);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => new { x.CustomerTenantId, x.StartTime });
        builder.HasIndex(x => new { x.AgentUserId, x.StartTime });
    }
}

