﻿using Microsoft.EntityFrameworkCore;
using Pet_s_Land.Models.AdressModels;
using Pet_s_Land.Models.CartModels;
using Pet_s_Land.Models.CategoryModels;
using Pet_s_Land.Models.OrderModels;
using Pet_s_Land.Models.ProductsModels;
using Pet_s_Land.Models.UserModels;
using Pet_s_Land.Models.WhishlistModel;

namespace Pet_s_Land.Datas
{
    public class AppDbContext:DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<WishList> WishLists { get; set; }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Address> Addresses { get; set; }

        public DbSet<Category> Categories { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<User>()
                .Property(u => u.IsBlocked)
                .HasDefaultValue(false);

            modelBuilder.Entity<WishList>()
                .HasOne(w => w.Users)
                .WithMany(u => u.WishList)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WishList>()
                .HasOne(w => w.Products)
                .WithMany()
                .HasForeignKey(w => w.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            //cart to cartitem relation
            modelBuilder.Entity<User>()
                .HasOne(u => u.Cart)
                .WithOne(u => u.User)
                .HasForeignKey<Cart>(x => x.UserId);

            //Cart to CartItem relation
            modelBuilder.Entity<Cart>()
                .HasMany(x => x.CartItems)
                .WithOne(c => c.Cart)
                .HasForeignKey(i => i.CartId);

            //CartItem to product relation
            modelBuilder.Entity<CartItem>()
                .HasOne(c => c.Product)
                .WithMany(c => c.CartItems)
                .HasForeignKey(i => i.ProductId);

            
            modelBuilder.Entity<Order>()
                .HasOne(u => u.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(o => o.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(o => o.OrderId);
            
            modelBuilder.Entity<OrderItem>()
                .HasOne(p => p.Product)
                .WithMany()
                .HasForeignKey(p => p.ProductId);
            
            modelBuilder.Entity<OrderItem>()
                .Property(pr => pr.TotalPrice)
                . HasPrecision(30, 2);
            modelBuilder.Entity<Address>()
                .HasOne(a => a.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(u => u.UserId);
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Address)
                .WithMany(a => a.Orders)
                .HasForeignKey(u => u.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            
            modelBuilder.Entity<Order>()
                .Property(o => o.OrderStatus)
                .HasDefaultValue("pending");


            modelBuilder.Entity<Product>()
                .HasOne(c => c.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(c => c.CategoryId);


            modelBuilder.Entity<Category>().HasData(
            new Category { CategoryId = 1, CategoryName = "Dog" },
            new Category { CategoryId = 2, CategoryName = "Cat" });

        }



    }
}
