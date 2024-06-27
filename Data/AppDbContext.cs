using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ERP;
using ERP.Extensions;

namespace ERP.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<Store> Stores { get; set; }
    public virtual DbSet<Raincheck> Rainchecks { get; set; }
    public virtual DbSet<CartItem> CartItems { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK_dbo.Categories");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK_dbo.Products");

            entity.HasOne(d => d.Category).WithMany(p => p.Products).HasConstraintName("FK_dbo.Products_dbo.Categories_CategoryId");
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.HasKey(e => e.StoreId).HasName("PK_dbo.Stores");
        });

        modelBuilder.Entity<Raincheck>(entity =>
        {
            entity.HasKey(e => e.RaincheckId).HasName("PK_dbo.Rainchecks");

            entity.HasOne(d => d.Product).WithMany(p => p.Rainchecks).HasConstraintName("FK_dbo.Rainchecks_dbo.Products_ProductId");

            entity.HasOne(d => d.Store).WithMany(p => p.Rainchecks).HasConstraintName("FK_dbo.Rainchecks_dbo.Stores_StoreId");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.CartItemId).HasName("PK_dbo.CartItems");

            entity.HasOne(d => d.Product).WithMany(p => p.CartItems).HasConstraintName("FK_dbo.CartItems_dbo.Products_ProductId");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK_dbo.Orders");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK_dbo.OrderDetails");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails).HasConstraintName("FK_dbo.OrderDetails_dbo.Orders_OrderId");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails).HasConstraintName("FK_dbo.OrderDetails_dbo.Products_ProductId");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
