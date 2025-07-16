using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OnlineStore.Models
{
    public class myContext: IdentityDbContext<ApplicationUser>
    {
        public myContext(DbContextOptions option):base(option)
        {
            
        }
        public DbSet<Product> products { get; set; }
        public DbSet<MainCategory> categories { get; set; }
        public DbSet<MSubCategory> subCategories { get; set; }
        public DbSet<Commment> comments { get; set; }
        public DbSet<ProductImages> productImages { get; set; }
        public DbSet<ProductRate> productRates { get; set; }
        public DbSet<PublicInfo> publicInfos { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }


    }
}
