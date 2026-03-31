using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<ProductCategory> ProductCategories { get; set; } = null!;
    public DbSet<Warehouse> Warehouses { get; set; } = null!;
    public DbSet<StockLevel> StockLevels { get; set; } = null!;
    public DbSet<StockTransaction> StockTransactions { get; set; } = null!;
    public DbSet<Supplier> Suppliers { get; set; } = null!;
    public DbSet<ProductSupplier> ProductSuppliers { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Product Category Configuration
        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId);
            entity.Property(e => e.CategoryName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasOne(e => e.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(e => e.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Product Configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId);
            entity.HasIndex(e => e.SKU).IsUnique();
            entity.Property(e => e.SKU).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.UnitOfMeasure).HasMaxLength(20);
            entity.Property(e => e.Cost).HasPrecision(18, 2);
            entity.Property(e => e.ListPrice).HasPrecision(18, 2);

            entity.HasOne(e => e.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Warehouse Configuration
        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(e => e.WarehouseId);
            entity.Property(e => e.WarehouseName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Location).HasMaxLength(200);
        });

        // StockLevel Configuration
        modelBuilder.Entity<StockLevel>(entity =>
        {
            entity.HasKey(e => e.StockLevelId);
            entity.HasIndex(e => new { e.ProductId, e.WarehouseId }).IsUnique();

            entity.HasOne(e => e.Product)
                .WithMany(p => p.StockLevels)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Warehouse)
                .WithMany(w => w.StockLevels)
                .HasForeignKey(e => e.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // StockTransaction Configuration
        modelBuilder.Entity<StockTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId);
            entity.Property(e => e.TransactionType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Reference).HasMaxLength(100);

            entity.HasOne(e => e.Product)
                .WithMany(p => p.Transactions)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Warehouse)
                .WithMany()
                .HasForeignKey(e => e.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Supplier Configuration
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId);
            entity.Property(e => e.SupplierName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        // ProductSupplier Configuration
        modelBuilder.Entity<ProductSupplier>(entity =>
        {
            entity.HasKey(e => e.SupplierProductId);
            entity.Property(e => e.SupplierSKU).HasMaxLength(50);

            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Supplier)
                .WithMany(s => s.ProductSuppliers)
                .HasForeignKey(e => e.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // User Configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);

            entity.HasOne(e => e.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Role Configuration
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
        });
    }
}
