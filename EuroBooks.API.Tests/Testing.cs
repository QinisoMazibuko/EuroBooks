using EuroBooks.Application.Common.Interfaces;
using EuroBooks.Infrastructure.Identity;
using EuroBooks.Infrastructure.Persistance;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Respawn;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EuroBooks.API.Tests
{
    [SetUpFixture]
    public class Testing
    {
        private static IConfigurationRoot configuration;
        private static IServiceScopeFactory scopeFactory;
        private static IWebHostEnvironment environment;

        private static Checkpoint checkpoint;
        private static long currentUserId;

        public static HttpClient client;
        public static TestServer server;

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables();

            configuration = builder.Build();

            environment = Moq.Mock.Of<IWebHostEnvironment>(w =>
            w.EnvironmentName == "Development" &&
            w.ApplicationName == "EuroBooks.API");

            var startup = new Startup(configuration, environment);



            var services = new ServiceCollection();

            services.AddSingleton(environment);

            services.AddLogging();

            startup.ConfigureServices(services);

            // Replace service registration for ICurrentUserService
            // Remove existing registration
            var currentUserServiceDescriptor = services.FirstOrDefault(d =>
                d.ServiceType == typeof(ICurrentUserService));

            services.Remove(currentUserServiceDescriptor);

            // Register testing version
            services.AddTransient(provider =>
                Mock.Of<ICurrentUserService>(s => s.UserId == currentUserId));

            scopeFactory = services.BuildServiceProvider().GetService<IServiceScopeFactory>();

            checkpoint = new Checkpoint
            {
                TablesToIgnore = new[] { "__EFMigrationsHistory" }
            };

            EnsureDatabase();

            // Testing environment
            server = new TestServer(new WebHostBuilder()
                        .UseStartup<Startup>());
            client = server.CreateClient();
            client.BaseAddress = new Uri("https://localhost:5001");
        }

        private static void EnsureDatabase()
        {
            using var scope = scopeFactory.CreateScope();

            var context = scope.ServiceProvider.GetService<ApplicationDbContext>();

            context.Database.Migrate();
        }

        public static async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
        {
            using var scope = scopeFactory.CreateScope();

            var mediator = scope.ServiceProvider.GetService<IMediator>();

            return await mediator.Send(request);
        }

        public static async Task<long> RunAsUserAsync(string userName, string password, string firstName, string lastName, string role)
        {
            using var scope = scopeFactory.CreateScope();
            var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();

            var user = new ApplicationUser
            {
                UserName = userName,
                Email = userName,
                FirstName = firstName,
                LastName = lastName,
                IsActive = true
            };
            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
                currentUserId = user.Id;

                return currentUserId;
            }

            var errors = string.Join(Environment.NewLine, result.ToApplicationResult().Errors);

            throw new Exception($"Unable to create {userName}.{Environment.NewLine}{errors}");
        }

        public static async Task<ApplicationUser> AddUserAsync(ApplicationUser user, string password, string role)
        {
            using var scope = scopeFactory.CreateScope();
            var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
                currentUserId = user.Id;

                return user;
            }
            var errors = string.Join(Environment.NewLine, result.ToApplicationResult().Errors);

            throw new Exception($"Unable to create {user.UserName}.{Environment.NewLine}{errors}");
        }

        public static async Task<long> RunAsUserAsync(string userName, string password, List<string> roles)
        {
            using var scope = scopeFactory.CreateScope();
            var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();

            var user = new ApplicationUser { UserName = userName, Email = userName };
            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await userManager.AddToRolesAsync(user, roles);
                currentUserId = user.Id;

                return currentUserId;
            }

            var errors = string.Join(Environment.NewLine, result.ToApplicationResult().Errors);

            throw new Exception($"Unable to create {userName}.{Environment.NewLine}{errors}");
        }

        public static async Task ResetState()
        {
            await checkpoint.Reset(configuration.GetConnectionString("EuroBooks"));
            currentUserId = -1;
        }

        public static async Task<TEntity> FindAsync<TEntity>(params object[] keyValues) where TEntity : class
        {
            using var scope = scopeFactory.CreateScope();

            var context = scope.ServiceProvider.GetService<ApplicationDbContext>();

            return await context.FindAsync<TEntity>(keyValues);
        }

        public static async Task<TEntity> AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            using var scope = scopeFactory.CreateScope();

            var context = scope.ServiceProvider.GetService<ApplicationDbContext>();

            context.Add(entity);

            await context.SaveChangesAsync();

            return entity;
        }

        public static async Task AddRangeAsync<TEntity>(params TEntity[] entity) where TEntity : class
        {
            using var scope = scopeFactory.CreateScope();

            var context = scope.ServiceProvider.GetService<ApplicationDbContext>();

            await context.AddRangeAsync(entity);

            await context.SaveChangesAsync();
        }

        public static class ContentHelper
        {
            public static StringContent GetStringContent(object obj)
                => new StringContent(JsonConvert.SerializeObject(obj), Encoding.Default, "application/json");
        }

        [OneTimeTearDown]
        public void RunAfterAnyTests()
        {
        }
    }
}