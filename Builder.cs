using Taller_1_IDWM.src.Data;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using Taller_1_IDWM.src.Helpers;
using CloudinaryDotNet;
using Taller_1_IDWM.src.Interfaces;
using Taller_1_IDWM.src.Repositories;
using Microsoft.AspNetCore.Identity;
using Taller_1_IDWM.src.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Taller_1_IDWM
{
    public class Builder
    {
        public WebApplication Build(string[] args) 
        {
            Env.Load();
            // Create a builder
            var builder = WebApplication.CreateBuilder(args);
            BuildCloudinary(builder);
            // Add endpoints to the application
            builder.Services.AddEndpointsApiExplorer();
            // Add the database context to the application
            builder.Services.AddDbContext<DataContext>(opt => opt.UseSqlite(Environment.GetEnvironmentVariable("DATABASE_URL")));
            // Add swagger to the application
            builder.Services.AddSwaggerGen();
            // Add scoped seeder to the application
            builder.Services.AddScoped<Seeder>();
            // Add controllers to the application
            builder.Services.AddControllers();
            // Add  scoped UserRepository to the application
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            // Add  scoped ProductRepository to the application
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            // Add Identity to the application to DataContext
            BuildIdentity(builder);
            // Builds the application
            var app = builder.Build();
            // Resolve the RoleManager from the service provider
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
                CreateRolesAsync(roleManager);
            }
            // Seed the database
            Seed(app);
            return app;
        }
        void BuildIdentity(WebApplicationBuilder builder)
        {
            builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
            })
            .AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();
            builder.Services.AddAuthentication(
                opt =>
                {
                    opt.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                    opt.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                    opt.DefaultForbidScheme =
                    opt.DefaultScheme =
                    opt.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
                    opt.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(opt => {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                      ValidateIssuer = true,
                      ValidIssuer = builder.Configuration["JWT:Issuer"],
                      ValidateAudience = true,
                      ValidAudience = builder.Configuration["JWT:Audience"],
                      ValidateIssuerSigningKey = true,
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"] ?? throw new ArgumentNullException("JWT:SigninKey"))),
                    };
                    
                });
        }
        void BuildCloudinary(WebApplicationBuilder builder)
        {
            // Add cloudinary settings to the configuration
            var cloudinarySettings = new CloudinarySettings
            {
                CloudName = Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME"),
                ApiKey = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY"),
                ApiSecret = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET")
            };

            var cloudinaryAccount = new Account(
                cloudinarySettings.CloudName,
                cloudinarySettings.ApiKey,
                cloudinarySettings.ApiSecret
            );

            var cloudinary = new Cloudinary(cloudinaryAccount);
        }
        async void CreateRolesAsync(RoleManager<IdentityRole<int>> roleManager) 
        {
            await CreateRoles(roleManager);
        }

        static async Task CreateRoles(RoleManager<IdentityRole<int>> roleManager)
        {
            string[] roleNames = { "Admin", "User" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole<int> { Name = roleName });
                }
            }
        }
        async void Seed(WebApplication app) 
        {
            // Create a scope to get the services
            using (var scope = app.Services.CreateScope())
            {
            // Get the services
            var services = scope.ServiceProvider;
            // Get the DataContext
            var context = services.GetRequiredService<DataContext>();
            await context.Database.MigrateAsync();
            // Initialize the seeder
            Seeder.Initialize(services);
            }
        }
    }
}