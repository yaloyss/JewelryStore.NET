using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.DAL.Configuration
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("categories");

            builder.HasKey(c => c.CategoryId).HasName("categories_pkey");

            builder.Property(c => c.CategoryId).HasColumnName("categoryid");

            builder.Property(c => c.Name).HasColumnName("name").HasMaxLength(100).IsRequired();

            builder.HasIndex(c => c.Name);

            builder.HasMany(c => c.Products)
                 .WithOne(p => p.Category)
                 .HasForeignKey(p => p.CategoryId);
        }
    }
}

