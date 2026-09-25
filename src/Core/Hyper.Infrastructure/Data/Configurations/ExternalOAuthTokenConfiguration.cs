// Auto-generated
using Hyper.Integration.Domain.Entities.Integrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;

public sealed class ExternalOAuthTokenConfiguration : IEntityTypeConfiguration<ExternalOAuthToken>, IIntegrationEntityConfiguration
{
    public void Configure(EntityTypeBuilder<ExternalOAuthToken> entity)
    {
        entity.ToTable("ExternalOAuthTokens", "dbo");

        // Primary key
        entity.HasKey(e => e.Id)
            .HasName("PK_ExternalOAuthTokens")
            .IsClustered(true);

        entity.Property(e => e.Id)
            .HasColumnName("Id")
            .HasColumnType("bigint")
            .IsRequired()
            .UseIdentityColumn(1L, 1);

        // Foreign keys and indices
        entity.Property(e => e.ShopId)
            .HasColumnName("ShopId")
            .HasColumnType("int")
            .IsRequired();

        entity.Property(e => e.TenantId)
            .HasColumnName("TenantId")
            .HasColumnType("nvarchar(30)")
            .IsRequired()
            .HasMaxLength(30)
            .IsUnicode(true)
            .UseCollation("SQL_Latin1_General_CP1_CI_AS");

        entity.Property(e => e.ConnectionId)
            .HasColumnName("ConnectionId")
            .HasColumnType("bigint")
            .IsRequired();

        entity.Property(e => e.Provider)
            .HasColumnName("Provider")
            .HasColumnType("tinyint")
            .IsRequired();

        // Token data
        entity.Property(e => e.AccessToken)
            .HasColumnName("AccessToken")
            .HasColumnType("nvarchar(max)")
            .IsRequired()
            .IsUnicode(true)
            .UseCollation("SQL_Latin1_General_CP1_CI_AS");

        entity.Property(e => e.RefreshToken)
            .HasColumnName("RefreshToken")
            .HasColumnType("nvarchar(max)")
            .IsRequired(false)
            .IsUnicode(true)
            .UseCollation("SQL_Latin1_General_CP1_CI_AS");

        entity.Property(e => e.TokenType)
            .HasColumnName("TokenType")
            .HasColumnType("nvarchar(50)")
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Bearer")
            .IsUnicode(true)
            .UseCollation("SQL_Latin1_General_CP1_CI_AS");

        entity.Property(e => e.Scopes)
            .HasColumnName("Scopes")
            .HasColumnType("nvarchar(2000)")
            .IsRequired(false)
            .HasMaxLength(2000)
            .IsUnicode(true)
            .UseCollation("SQL_Latin1_General_CP1_CI_AS");

        // Timestamps
        entity.Property(e => e.IssuedAtUtc)
            .HasColumnName("IssuedAtUtc")
            .HasColumnType("datetime2(7)")
            .IsRequired()
            .HasDefaultValueSql("(sysutcdatetime())");

        entity.Property(e => e.ExpiresAtUtc)
            .HasColumnName("ExpiresAtUtc")
            .HasColumnType("datetime2(7)")
            .IsRequired(false);

        entity.Property(e => e.UpdatedAtUtc)
            .HasColumnName("UpdatedAtUtc")
            .HasColumnType("datetime2(7)")
            .IsRequired(false);

        // Status
        entity.Property(e => e.IsActive)
            .HasColumnName("IsActive")
            .HasColumnType("bit")
            .IsRequired()
            .HasDefaultValue(true);

        // Raw response
        entity.Property(e => e.RawTokenResponse)
            .HasColumnName("RawTokenResponse")
            .HasColumnType("nvarchar(max)")
            .IsRequired(false)
            .IsUnicode(true)
            .UseCollation("SQL_Latin1_General_CP1_CI_AS");

        // Indices
        entity.HasIndex(e => new { e.ShopId, e.Provider })
            .HasDatabaseName("UX_ExternalOAuthTokens_ShopId_Provider")
            .IsUnique();

        entity.HasIndex(e => e.TenantId)
            .HasDatabaseName("IX_ExternalOAuthTokens_TenantId");

        entity.HasIndex(e => e.ConnectionId)
            .HasDatabaseName("IX_ExternalOAuthTokens_ConnectionId");

        entity.HasIndex(e => e.ExpiresAtUtc)
            .HasDatabaseName("IX_ExternalOAuthTokens_ExpiresAtUtc");
    }
}


