using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.DAL.Configuration
{
    public class ProductStoneConfiguration : IEntityTypeConfiguration<ProductStone>
    {
        public void Configure(EntityTypeBuilder<ProductStone> builder)
        {
            builder.ToTable("product_stone");

            builder.HasKey(ps => new { ps.ProductId, ps.StoneId }).HasName("product_stone_pkey");

            builder.Property(ps => ps.ProductId).HasColumnName("productid");

            builder.Property(ps => ps.StoneId).HasColumnName("stoneid");

            builder.HasOne(ps => ps.Product).WithMany(p => p.ProductStones)
                .HasForeignKey(ps => ps.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_productstone_products");

            builder.HasOne(ps => ps.Stone).WithMany()
                .HasForeignKey(ps => ps.StoneId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_productstone_stones");
        }
    }
}

