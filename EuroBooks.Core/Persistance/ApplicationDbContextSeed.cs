using EuroBooks.Application.Common.Interfaces;
using EuroBooks.Domain.Enities;
using EuroBooks.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EuroBooks.Infrastructure.Persistance
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IApplicationDbContext context)
        {
            #region ROLES

            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new ApplicationRole
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                });
                await roleManager.CreateAsync(new ApplicationRole
                {
                    Name = "Subscriber",
                    NormalizedName = "SUBSCRIBER"
                });
            }
            #endregion ROLES

            #region DEFAULT USER
            var defaultUser = new ApplicationUser
            {
                UserName = "support@EuroBooks.co.za",
                Email = "support@EuroBooks.co.za",
                FirstName = "Qiniso",
                LastName = "Mazibuko",
                IsActive = true
            };

            if (userManager.Users.All(u => u.UserName != defaultUser.UserName))
            {
                await userManager.CreateAsync(defaultUser, "Password1!");
                await userManager.AddToRoleAsync(defaultUser, "Admin");
            }
            #endregion DEFAULT USER

            #region SETTINGS
            if (context.ApplicationVariables.Where(c => c.Key.Contains("SMTP")).Count() == 0)
            {
                context.ApplicationVariables.Add(new ApplicationVariables { Key = "SMTPServer", Value = "smtp.gmail.com", Description = "SMTP Server" });
                context.ApplicationVariables.Add(new ApplicationVariables { Key = "SMTPUser", Value = "tester.eurobooks@gmail.com", Description = "SMTP User" });
                context.ApplicationVariables.Add(new ApplicationVariables { Key = "SMTPPassword", Value = "P@ssword3#", Description = "SMTP Password" });
                context.ApplicationVariables.Add(new ApplicationVariables { Key = "SMTPPort", Value = "587", Description = "SMTP Port" });
                context.ApplicationVariables.Add(new ApplicationVariables { Key = "SMTPFromEmail", Value = "tester.eurobooks@gmail.com", Description = "SMTP From Email" });
                context.ApplicationVariables.Add(new ApplicationVariables { Key = "SMTPFromName", Value = "EuroBooks", Description = "SMTP From Name" });

                await context.SaveChangesAsync(CancellationToken.None);
            }
            #endregion SETTINGS

        }
    }
}
