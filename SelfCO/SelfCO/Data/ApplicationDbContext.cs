using Microsoft.EntityFrameworkCore;
using global::SelfCheckoutSystem.Models;
using SelfCheckoutSystem.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace SelfCheckoutSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // unique constraints
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Code)
                .IsUnique();

            // relationships
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Payment)
                .WithOne(p => p.Order)
                .HasForeignKey<Payment>(p => p.OrderId);

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed categories
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    CategoryId = 1,
                    Name = "Beverages",
                    Description = "Drinks and beverages",
                    IsActive = true
                },
                new Category
                {
                    CategoryId = 2,
                    Name = "Snacks",
                    Description = "Chips and snacks",
                    IsActive = true
                },
                new Category
                {
                    CategoryId = 3,
                    Name = "Dairy",
                    Description = "Milk and dairy products",
                    IsActive = true
                }
            );

            // Seed products
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ProductId = 1,
                    Code = "123456789",
                    Name = "Coca Cola 500ml",
                    Description = "Refreshing cola drink", 
                    Price = 1.99m,
                    Stock = 100,
                    CategoryId = 1,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1) 
                },
                new Product
                {
                    ProductId = 2,
                    Code = "987654321",
                    Name = "Lays Chips",
                    Description = "Classic potato chips", 
                    Price = 2.49m,
                    Stock = 50,
                    CategoryId = 2,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1)  
                },
                new Product
                {
                    ProductId = 3,
                    Code = "456789123",
                    Name = "Milk 1L",
                    Description = "Fresh whole milk",  
                    Price = 3.99m,
                    Stock = 30,
                    CategoryId = 3,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1)  
                }
            );
        }
    }
}
