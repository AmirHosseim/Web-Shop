using ChannelServices.CategoriesServices;
using ChannelServices.ChannelServices;
using ChannelServices.Following_Services;
using ChannelServices.ProductServices;
using ChannelServices.RecuritmentServices;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserServices.UserFavorite;
using UserServices.UserManager;

namespace ShopWeb.Data
{
    public class ShopWebContext : IdentityDbContext
    {
        public ShopWebContext(DbContextOptions<ShopWebContext> options) : base(options)
        {
         
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<RatePoan> RatePoans { get; set; }
        public DbSet<SpecialUser> SpecialUsers { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<RecruitmentInChannelForm> RecruitmentInChannelForms { get; set; }
        public DbSet<RecruitmentInChannel> RecruitmentInChannels { get; set; }
        public DbSet<ReplyToRecruitmentForm> ReplyToRecruitmentForms { get; set; }
        public DbSet<Access> Accesses { get; set; }
        public DbSet<AccessToEmploy> AccessToEmploys { get; set; }
        public DbSet<ProductImageInfo> ProductsImagesInfo { get; set; }
        public DbSet<SettelmentReceipt> SettelmentReceipts { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        { 
            builder.Entity<Channel>().HasKey(x => x.Id);

            builder.Entity<Follow>().HasKey(k => new { k.ChannelId , k.UserId });
            builder.Entity<RecruitmentInChannelForm>().HasKey(k => new { k.Id });
            builder.Entity<RecruitmentInChannel>().HasKey(k => new { k.UserId, k.ChannelId });
            builder.Entity<ReplyToRecruitmentForm>().HasKey(k => new { k.UserId, k.RecruitmenFormtId });

            builder.Entity<OutTimeDay>().HasNoKey();
            base.OnModelCreating(builder);
        }
    }
}
