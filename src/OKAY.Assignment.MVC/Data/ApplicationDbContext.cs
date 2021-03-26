using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using OKAY.Assignment.MVC.Models;

namespace OKAY.Assignment.MVC.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Property> Properties { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(b => b.Property(u => u.Id).HasDefaultValueSql("newid()"));
            builder.Entity<ApplicationRole>(b => b.Property(r => r.Id).HasDefaultValueSql("newid()"));

            builder.Entity<Property>(x => x.HasOne(p => p.User).WithMany(u => u.Properties).HasForeignKey(p => p.userId).HasConstraintName("FK_Properties_ASPNetUsers_Userid").OnDelete(DeleteBehavior.Cascade));
            builder.Entity<Transaction>(x =>
            {
                x.HasOne(t => t.User).WithMany(u => u.Transactions).HasForeignKey(t => t.userId).HasConstraintName("FK_Transactions_ASPNetUsers_UserId").OnDelete(DeleteBehavior.ClientSetNull);
                x.HasOne(t => t.Property).WithMany(p => p.Transactions).HasForeignKey(t => t.propertyId).HasConstraintName("FK_Transactions_Properties_PropertyId").OnDelete(DeleteBehavior.Cascade);
            });


            // seeding();
        }

        void seeding()
        {
            IConfigurationRoot Configuration = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            IServiceCollection collection = new ServiceCollection();
            collection.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            collection.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<ApplicationRole>()
                .AddRoleManager<RoleManager<ApplicationRole>>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            var provider = collection.BuildServiceProvider();

            var userManager = provider.GetService<UserManager<ApplicationUser>>();
            var roleManager = provider.GetService<RoleManager<ApplicationRole>>();
            var context = provider.GetService<ApplicationDbContext>();

            Seeder.SeedIdentity(context, userManager, roleManager).GetAwaiter().GetResult();
            context.SeedProperties().GetAwaiter().GetResult();
            context.SeedTransactions().GetAwaiter().GetResult();
        }

        public DbSet<OKAY.Assignment.MVC.Models.PropertyViewModel> PropertyViewModel { get; set; }

        public DbSet<OKAY.Assignment.MVC.Models.TransactionViewModel> TransactionViewModel { get; set; }
    }
}
