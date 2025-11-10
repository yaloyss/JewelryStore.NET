using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.DAL.Configuration
{
    public class MetalConfiguration : IEntityTypeConfiguration<Metal>
    {
        public void Configure(EntityTypeBuilder<Metal> builder)
        {
            builder.ToTable("metals");

            builder.HasKey(m => m.MetalId).HasName("metals_pkey");

            builder.Property(m => m.MetalId).HasColumnName("metalid");

            builder.Property(m => m.Name).HasColumnName("name").HasMaxLength(50).IsRequired();

            builder.Property(m => m.Color).HasColumnName("color").HasMaxLength(50);

            builder.HasIndex(m => m.Name).IsUnique();
        }
    }
}

