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
using Taller_1_IDWM.src.Service;
using Microsoft.OpenApi.Models;

namespace Taller_1_IDWM
{
    public class Builder
    {
        public async Task<WebApplication> Build(string[] args) 
        {
            Env.Load();
            // Create a builder
            var builder = WebApplication.CreateBuilder(args);
            var cloudinary = BuildCloudinary();
            builder.Services.AddSingleton(cloudinary);
            // Add endpoints to the application
            builder.Services.AddEndpointsApiExplorer();
            // Add the database context to the application
            builder.Services.AddDbContext<DataContext>(opt => opt.UseSqlite(Environment.GetEnvironmentVariable("DATABASE_URL")));
            // Add swagger to the application
            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });
            // Add scoped seeder to the application
            builder.Services.AddScoped<Seeder>();
            // Add controllers to the application
            builder.Services.AddControllers();
            // Add  scoped UserRepository to the application
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            // Add  scoped ProductRepository to the application
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            // Add  scoped AuthRepository to the application
            builder.Services.AddScoped<IAuthRepository, AuthRepository>();
            // Add  scoped TokenService to the application
            builder.Services.AddScoped<ITokenService, TokenService>();
            // Add  scoped CartRepository to the application
            builder.Services.AddScoped<ICartRepository, CartRepository>();
            // Add Identity to the application to DataContext
            BuildIdentity(builder);
            // Builds the application
            var app = builder.Build();
            // Resolve the RoleManager from the service provider
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DataContext>();
                context.Database.Migrate();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
                CreateRolesAsync(roleManager);
            }
            // Seed the database
            await Seed(app);
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
                    opt.DefaultAuthenticateScheme = 
                    opt.DefaultChallengeScheme = 
                    opt.DefaultForbidScheme =
                    opt.DefaultScheme =
                    opt.DefaultSignInScheme = 
                    opt.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(opt => {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                      ValidateIssuer = true,
                      ValidIssuer = Environment.GetEnvironmentVariable("JWT_IUSSER"),
                      ValidateAudience = true,
                      ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                      ValidateIssuerSigningKey = true,
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SIGNINKEY") ?? throw new ArgumentNullException("JWT_SIGINKEY"))),
                    };
                    
                });
        }
        Cloudinary BuildCloudinary()
        {
            // Add cloudinary settings to the configuration
            var cloudinarySettings = new CloudinarySettings
            {
                CloudName = Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME")!,
                ApiKey = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY")!,
                ApiSecret = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET")!
            };

            var cloudinaryAccount = new Account(
                cloudinarySettings.CloudName,
                cloudinarySettings.ApiKey,
                cloudinarySettings.ApiSecret
            );

            var cloudinary = new Cloudinary(cloudinaryAccount);
            return cloudinary;
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
        async Task Seed(WebApplication app) 
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
                await Seeder.Initialize(services);
            }
        }
    }
}