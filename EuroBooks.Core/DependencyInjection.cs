using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using EuroBooks.Application.Common.Interfaces;
using EuroBooks.Infrastructure.Identity;
using EuroBooks.Infrastructure.Services;
using Microsoft.IdentityModel.Tokens;
using EuroBooks.Infrastructure.Configuration;
using EuroBooks.Infrastructure.Persistance;
using Microsoft.Extensions.Hosting;

namespace EuroBooks.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("EuroBooksConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
            );

            services.AddTransient(provider => (IApplicationDbContext)provider.GetService<ApplicationDbContext>());


            services.AddTransient<IDateTime, DateTimeService>();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddScoped<IEmailService, EmailService>();

          
            return services;
        }
    }
}
