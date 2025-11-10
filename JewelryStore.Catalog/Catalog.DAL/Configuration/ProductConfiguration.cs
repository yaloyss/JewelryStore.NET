using JewelryStore.CatalogService.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JewelryStore.CatalogService.Catalog.DAL.Configuration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("products");

            builder.HasKey(p => p.ProductId).HasName("products_pkey");

            builder.Property(p => p.ProductId).HasColumnName("productid");

            builder.Property(p => p.Name).HasColumnName("name").HasMaxLength(300).IsRequired();

            builder.Property(p => p.Price).HasColumnName("price").HasColumnType("numeric(10,2)").IsRequired();

            builder.Property(p => p.Weight).HasColumnName("weight").HasColumnType("numeric(10,2)").IsRequired();

            builder.Property(p => p.Size).HasColumnName("size").HasColumnType("numeric(10,2)").IsRequired();

            builder.Property(p => p.Manufacturer).HasColumnName("manufacturer").HasMaxLength(100);

            builder.Property(p => p.MetalId).HasColumnName("metalid").IsRequired();

            builder.Property(p => p.CategoryId).HasColumnName("categoryid").IsRequired();

            builder.HasOne(p => p.Metal).WithMany()
                .HasForeignKey(p => p.MetalId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_products_metals");

            builder.HasOne(p => p.Category).WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_products_categories");

            builder.HasIndex(p => p.Name).HasDatabaseName("idx_product_name");

            builder.HasIndex(p => p.Price).HasDatabaseName("idx_product_price");

            builder.HasIndex(p => p.MetalId).HasDatabaseName("idx_product_metal");

            builder.HasIndex(p => p.CategoryId).HasDatabaseName("idx_product_category");
        }
    }
}

