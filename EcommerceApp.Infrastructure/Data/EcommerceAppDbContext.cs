using EcommerceApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Infrastructure.Data
{
    public class EcommerceAppDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<FavouriteProduct> FavouriteProducts { get; set; }

        public EcommerceAppDbContext(DbContextOptions<EcommerceAppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Đảm bảo SKU của ProductVariant là duy nhất
            modelBuilder.Entity<ProductVariant>()
                .HasIndex(pv => pv.SKU)
                .IsUnique();

            // Khóa chính đơn lẻ cho CartItem
            modelBuilder.Entity<CartItem>()
                .HasKey(ci => ci.Id);

            // Quan hệ giữa CartItem và Cart
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId)
                .IsRequired();

            // Quan hệ giữa CartItem và Product
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .IsRequired();

            // Quan hệ giữa CartItem và ProductVariant
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Variant)
                .WithMany()
                .HasForeignKey(ci => ci.VariantId)
                .IsRequired(false);

            // Tạo ràng buộc tính duy nhất giữa CartId, ProductId, và VariantId
            modelBuilder.Entity<CartItem>()
                .HasIndex(ci => new { ci.CartId, ci.ProductId, ci.VariantId })
                .IsUnique();
        }

    }
}
