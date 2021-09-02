using System;
using System.Text;
using EuroBooks.API.Services;
using EuroBooks.Application;
using EuroBooks.Application.Common.Interfaces;
using EuroBooks.Infrastructure;
using EuroBooks.Infrastructure.Configuration;
using EuroBooks.Infrastructure.Identity;
using EuroBooks.Infrastructure.Persistance;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace EuroBooks
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;  // configuration.BuildAppSettings(environment);
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region configure cors 
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins(Configuration.GetSection("ApplicationSettings")["CLient_URL"])
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials()
                                .SetIsOriginAllowed((host) => true)
                                .SetPreflightMaxAge(TimeSpan.FromSeconds(2520));
                    });
            });
            #endregion

            #region Infrastructure
            // Add infrastructure & DBContext using extension class
            services.AddApplication();
            services.AddInfrastructure(Configuration, Environment);
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();


            // inject AppSettings 
            services.Configure<ApplicationSettings>(Configuration.GetSection("ApplicationSettings"));
            #endregion

            services.AddControllers();

            #region AppSettings Configurations
            var jwtConfig = new JwtConfiguration();
            Configuration.Bind("Jwt", jwtConfig);
            services.AddSingleton<IJwtConfiguration>(jwtConfig);
            #endregion AppSettings Configurations


            #region JWT Authentication 
            var key = Encoding.UTF8.GetBytes(Configuration["ApplicationSettings:JWT_Secret"].ToString());

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = false;
                x.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });
            #endregion

            #region Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo 
                {
                    Title = "EuroBooks Public API",
                    Description = "Public API to interact with EuroBooks services" ,
                    Version = "v1" 
                });
            });
            #endregion

        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwaggerUI(options =>
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "EuroBooks API"));

                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder =>
            builder.WithOrigins(Configuration["ApplicationSettings:CLient_URL"].ToString())
            .AllowAnyHeader()
            .AllowAnyMethod()
            );

            app.UseAuthentication();

            // MyIdentityDataInitializer.SeedData(userManager, roleManager);

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}