using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.EntityFrameworkCore.SqlServer;
using OKAY.Assignment.MVC.Data;
using System.Data;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using OKAY.Assignment.MVC.Constraints;
using Microsoft.Data.Sqlite;

namespace OKAY.Assignment.MVC.Test.Services
{
    public abstract class RepositoryTestBase
    {
        internal ApplicationDbContext context;
        internal IHttpContextAccessor accessor;
        internal UserManager<ApplicationUser> userManager;
        internal RoleManager<ApplicationRole> roleManager;

        /// <summary>
        /// default constructor, use in-memory database
        /// </summary>
        public RepositoryTestBase()
        {
            SqliteConnection conn = new SqliteConnection("DataSource=:memory:");
            conn.Open();
            conn.CreateFunction("newid", () => Guid.NewGuid());
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(conn)
                .Options;

            context = new ApplicationDbContext(options);
            context.Database.EnsureCreated();
            BuildServices().GetAwaiter().GetResult();
            SwitchUser(true).GetAwaiter().GetResult();
        }

        /// <summary>
        /// constructor to use sql with connection string
        /// </summary>
        /// <param name="connectionString"></param>
        public RepositoryTestBase(string connectionString)
        {
            context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(connectionString).Options);
            BuildServices().GetAwaiter().GetResult();
            SwitchUser(true).GetAwaiter().GetResult();
        }

        private async Task BuildServices()
        {
            IServiceCollection collection = new ServiceCollection();
            collection.AddSingleton<ApplicationDbContext>(context);
            collection.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<ApplicationRole>()
                .AddRoleManager<RoleManager<ApplicationRole>>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            var provider = collection.BuildServiceProvider();

            userManager = provider.GetService<UserManager<ApplicationUser>>();
            roleManager = provider.GetService<RoleManager<ApplicationRole>>();

            await context.SeedIdentity(userManager, roleManager);
            await context.SeedPropertiesForSQLite();
            await context.SeedTransactionsForSQLite();
        }

        internal async Task SwitchUser(bool isAdmin)
        {
            string email = isAdmin ? "admin@example.com" : "user@example.com";
            var u = await userManager.FindByEmailAsync(email);
            var mAccessor = new Mock<IHttpContextAccessor>();

            var claims = new List<Claim>()
            {
                new Claim(Constraints.ClaimTypes.Email, email),
                new Claim(Constraints.ClaimTypes.Name, email),
                new Claim(Constraints.ClaimTypes.NameIdentifier, u.Id.ToString())
            };

            if (isAdmin)
            {
                claims.Add(new Claim(Constraints.ClaimTypes.Role, Constraints.IdentityRolesNames.Administrator));
            };

            mAccessor.Setup(x => x.HttpContext.User).Returns(new TestPrincpal(claims.ToArray()));
            accessor = mAccessor.Object;
        }
    }

    public class TestPrincpal : ClaimsPrincipal
    {
        public TestPrincpal(params Claim[] claims) : base(new TestIdentity(claims)) { }
    }

    public class TestIdentity : ClaimsIdentity
    {
        public TestIdentity(params Claim[] claims) : base(claims)
        {
        }
    }
}
