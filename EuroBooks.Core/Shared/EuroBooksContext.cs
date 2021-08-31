using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;

namespace EuroBooks.Core.Shared
{
    public class EuroBooksContext : IdentityDbContext<ApplicationUser>
    {
        public EuroBooksContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
       
    }

    // data seeding for roles and default admin user 


    public static class MyIdentityDataInitializer
    {
        public static void SeedData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        public static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            if (userManager.FindByEmailAsync("Admin@DevAssist.com").Result == null)
            {
                ApplicationUser user = new ApplicationUser();
                user.Email = "Admin@EuroBooks.com";
                user.UserName = user.Email;
                user.FirstName = "administrator";
                user.LastName = "EuroBooks";

                IdentityResult result = userManager.CreateAsync(user, "Password@12").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }
        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {

            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                IdentityRole role = new IdentityRole();

                role.Name = "Admin";
                role.Id = GenerateID();
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;

            }
            if (!roleManager.RoleExistsAsync("Developer").Result)
            {
                IdentityRole role = new IdentityRole();

                role.Name = "Developer";
                role.Id = GenerateID();
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;

            }
            if (!roleManager.RoleExistsAsync("Client").Result)
            {
                IdentityRole role = new IdentityRole();

                role.Name = "Client";
                role.Id = GenerateID();
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;

            }

        }

        public static string GenerateID() => Guid.NewGuid().ToString("N");


    }
}
