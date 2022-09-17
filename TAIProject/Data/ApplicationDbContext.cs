using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TAIProject.Models;

namespace TAIProject.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("Identity");
            builder.Entity<User>(entity =>
            {
                entity.ToTable(name: "User");
            });
            builder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Role");
            });
            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles");
            });
            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims");
            });
            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins");
            });
            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims");
            });
            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens");
            });
            builder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");
            });
            builder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");
            });
            builder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");
            });

            builder.Entity<OrderProduct>(entity =>
            {
                entity.HasKey(l => new { l.OrderId, l.ProductID });
                entity.ToTable("OrderProducts");
            });
        }

        public DbSet<TAIProject.Models.Category>? Category { get; set; }

        public DbSet<TAIProject.Models.Order>? Order { get; set; }

        public DbSet<TAIProject.Models.Product>? Product { get; set; }


        public DbSet<TAIProject.Models.OrderProduct>? OrderProduct { get; set; }
    }
}