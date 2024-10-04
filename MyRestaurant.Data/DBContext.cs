using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MyRestaurant.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MyRestaurant.Data
{

     public class DBContext :
          IdentityDbContext<User, Role, string, IdentityUserClaim<string>,
    UserRole, IdentityUserLogin<string>,
    IdentityRoleClaim<string>, IdentityUserToken<string>>

    //IdentityDbContext<IdentityUser>/*<Microsoft.AspNetCore.Identity.IdentityUser, Microsoft.AspNetCore.Identity.IdentityRole, string>*/

    {
        public DBContext(DbContextOptions<DBContext> options)
            : base(options) { }
        //public DBContext() : base() { }


        DbSet<Menu> Menu { get; set; }
        DbSet<Order> Order { get; set; }
        DbSet<User> User { get; set; }
        DbSet<OrderLine> OrderLine { get; set; }
        DbSet<OrderRate> OrderRate { get; set; }
        DbSet<WaiterRate> WaiterRate { get; set; }
        DbSet<Notification> Notification { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });
        }
    }



}
