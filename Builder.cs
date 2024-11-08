using Taller_1_IDWM.src.Data;
using Microsoft.EntityFrameworkCore;
using Taller_1_IDWM.src.Data.Migrations;
using Taller_1_IDWM.src.Helpers;
using CloudinaryDotNet;
using Taller_1_IDWM.src.Interfaces;
using Taller_1_IDWM.src.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Taller_1_IDWM
{
    public class Builder
    {
        public WebApplication Build(string[] args) 
        {
            // Create a builder
            var builder = WebApplication.CreateBuilder(args);
            // Add cloudinary settings to the configuration
            var CloudinarySettings = builder.Configuration.GetSection("CloudinarySettings").Get<CloudinarySettings>();
            var CloudinaryAccount = new Account(
                CloudinarySettings!.CloudName, 
                CloudinarySettings!.ApiKey, 
                CloudinarySettings!.ApiSecret
                );
            var Cloudinary = new Cloudinary(CloudinaryAccount);
            // Add endpoints to the application
            builder.Services.AddEndpointsApiExplorer();
            // Add the database context to the application
            builder.Services.AddDbContext<DataContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("CustomersConnection")));
            // Add the admin context to the application
            builder.Services.AddDbContext<AdminContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("AdminConnection")));
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
            // Add Identity to the application to AdminContext
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<AdminContext>()
            .AddDefaultTokenProviders();
            // Builds the application
            var app = builder.Build();
            // Resolve the RoleManager from the service provider
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                CreateRolesAsync(roleManager);
            }
            // Seed the database
            Seed(app);
            return app;
        }
        async void CreateRolesAsync(RoleManager<IdentityRole> roleManager) 
        {
            await CreateRoles(roleManager);
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
        
        static async Task CreateRoles(RoleManager<IdentityRole> roleManager)
        {
        string[] roleNames = { "Admin", "User" };
        IdentityResult roleResult;

        foreach (var roleName in roleNames)
        {
            var roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
        }
    }
}