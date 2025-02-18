using Microsoft.EntityFrameworkCore;
using Pet_s_Land.Models.ProductsModels;
using Pet_s_Land.Models.UserModels;

namespace Pet_s_Land.Datas
{
    public class AppDbContext:DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }





    }
}
