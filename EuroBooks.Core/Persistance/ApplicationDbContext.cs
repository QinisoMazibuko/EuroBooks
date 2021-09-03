using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using EuroBooks.Infrastructure.Identity;
using EuroBooks.Application.Common.Interfaces;
using EuroBooks.Domain.Enities;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EuroBooks.Domain.Common;

namespace EuroBooks.Infrastructure.Persistance
{
    public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, long>, IApplicationDbContext
    {
        private readonly ICurrentUserService currentUserService;
        private readonly IDateTime dateTime;

        public ApplicationDbContext(
              DbContextOptions options,
              ICurrentUserService currentUserService,
              IDateTime dateTime
              ) : base(options)
        {
            this.currentUserService = currentUserService;
            this.dateTime = dateTime;
        }

        public DbSet<ApplicationVariables> ApplicationVariables { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }


        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = currentUserService.UserId;
                        entry.Entity.Created = dateTime.Now;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedBy = currentUserService.UserId;
                        entry.Entity.LastModified = dateTime.Now;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            builder.Entity<ApplicationUser>().ToTable("Users", "user");
            builder.Entity<ApplicationRole>().ToTable("Roles", "user");
            builder.Entity<IdentityUserClaim<long>>().ToTable("UserClaims", "user");
            builder.Entity<IdentityUserRole<long>>().ToTable("UserRoles", "user");
            builder.Entity<IdentityUserLogin<long>>().ToTable("UserLogins", "user");
            builder.Entity<IdentityRoleClaim<long>>().ToTable("RoleClaims", "user");
            builder.Entity<IdentityUserToken<long>>().ToTable("UserToken", "user");

            builder.Entity<ApplicationVariables>(entity =>
            {
                entity.ToTable("ApplicationVariables", "Configuration");
            });


            base.OnModelCreating(builder);
        }

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
