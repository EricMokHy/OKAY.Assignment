using Microsoft.AspNetCore.Identity;
using OKAY.Assignment.MVC.Constraints;
using OKAY.Assignment.MVC.Data;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace OKAY.Assignment.MVC.Entities
{
    public static class Seeder
    {
        static readonly string adminEmail = "admin@example.com";
        static readonly string userEmail = "user@example.com";

        public static async Task SeedTransactions(this ApplicationDbContext context)
        {
            string command = @"Merge Transactions AS Target 
                            Using (Values (@AdminId, 1, GETDATE()),
		                            (@AdminId, 3, GETDATE()),
		                            (@AdminId, 7, GETDATE()),
		                            (@AdminId, 9, GETDATE()),
		                            (@UserId, 7, GETDATE()),
		                            (@UserId, 8, GETDATE()),
		                            (@UserId, 8, GETDATE()))
	                            As Source (userId, propertyId, transactionDate)
	                            On Target.userId = Source.userId and Target.propertyId = Source.propertyId
                            When Not Matched Then
	                            Insert (userId, propertyId, transactionDate) 
	                            Values (userId, propertyId, transactionDate);";
            var admin = await context.GetUserAsync(true);
            var user = await context.GetUserAsync(false);

            var adminParameter = new SqlParameter("@AdminId", admin);
            var userParameter = new SqlParameter("@UserId", user);
            
            await context.Database.ExecuteSqlRawAsync(command, adminParameter, userParameter);

            await context.SaveChangesAsync();
        }

        public static async Task SeedProperties(this ApplicationDbContext context)
        {
            string command = @" Merge Properties As Target
                                Using (Values (@AdminId, N'Property belog to admin 1', 1, 1, 10000),
			                                (@AdminId, N'Property belog to admin 2', 2, 1, 20000),
			                                (@AdminId, N'Property belog to admin 3', 3, 1, 30000),
			                                (@AdminId, N'Property belog to admin 4', 4, 1, 40000),
			                                (@AdminId, N'Property belog to admin 5', 5, 1, 50000),
			                                (@UserId, N'Property belog to user 1', 1, 1, 10000),
			                                (@UserId, N'Property belog to user 2', 2, 1, 20000),
			                                (@UserId, N'Property belog to user 3', 3, 1, 30000),
			                                (@UserId, N'Property belog to user 4', 4, 1, 40000),
			                                (@UserId, N'Property belog to user 5', 5, 1, 50000))
	                                As Source (userid, name, bedroom, isAvailable, leasePrice)
	                                on Target.name = Source.Name and Target.UserId = Source.UserId
                                When Not Matched Then
	                                Insert (userid, name, bedroom, isavailable, leasePrice, createddate, updateddate)
	                                values (userid, name, bedroom, isavailable, leasePrice, getdate(), getdate());";
            var admin = await context.GetUserAsync(true);
            var user = await context.GetUserAsync(false);

            var adminParameter = new SqlParameter("@AdminId", admin);
            var userParameter = new SqlParameter("@UserId", user);
            await context.Database.ExecuteSqlRawAsync(command, adminParameter, userParameter);
            await context.SaveChangesAsync();
        }

        private static async Task<Guid> GetUserAsync(this ApplicationDbContext context, bool isAdmin = true)
        {
            var email = isAdmin ? adminEmail : userEmail;
            var id = await context.Users.Where(x => x.Email.ToLower() == email.ToLower()).Select(x => x.Id).FirstAsync();
            return id;
        }

        public static async Task SeedIdentity(this ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            var r = roleManager.FindByNameAsync(IdentityRolesNames.Administrator);
            if (r == null)
            {
                await roleManager.CreateAsync(new ApplicationRole(IdentityRolesNames.Administrator));
            }

            var user = new ApplicationUser
            {
                Email = adminEmail,
                UserName = adminEmail
            };
            await CreateAsync(userManager, user, new[] { IdentityRolesNames.Administrator });

            user = new ApplicationUser
            {
                Email = userEmail,
                UserName = userEmail
            };
            await CreateAsync(userManager, user, new string[0]);
        }

        private static async Task CreateAsync(UserManager<ApplicationUser> userManager, 
            ApplicationUser user, string[] roles)
        {
            var exist = await userManager.FindByEmailAsync(user.Email);
            if (exist == null)
            {
                await userManager.CreateAsync(user, "P@ssw0rd");
                if (roles.Count() > 0)
                {
                    await userManager.AddToRolesAsync(exist, roles);
                }
            }
        }
    }
}
