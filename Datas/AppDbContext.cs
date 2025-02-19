using Microsoft.EntityFrameworkCore;
using Pet_s_Land.Models.CartModels;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WishList>()
                .HasOne(w => w.Users)
                .WithMany()
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


        }



    }
}
