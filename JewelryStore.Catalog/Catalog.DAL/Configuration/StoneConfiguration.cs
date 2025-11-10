using JewelryStore.CatalogService.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JewelryStore.CatalogService.Catalog.DAL.Configuration
{
    public class StoneConfiguration : IEntityTypeConfiguration<Stone>
    {
        public void Configure(EntityTypeBuilder<Stone> builder)
        {
            builder.ToTable("stones");

            builder.HasKey(s => s.StoneId).HasName("stones_pkey");

            builder.Property(s => s.StoneId).HasColumnName("stoneid");

            builder.Property(s => s.Name).HasColumnName("name").HasMaxLength(100).IsRequired();

            builder.HasIndex(s => s.Name).IsUnique();
        }
    }
}

