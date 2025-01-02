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
    /// <summary>
    /// Clase para construir la aplicación
    /// </summary>
    /// <remarks>
    /// La clase se utiliza para construir la aplicación, añadiendo los servicios necesarios y configurando la autenticación y autorización.
    /// </remarks> 
    public class Builder
    {
        /// <summary>
        /// Construye un WebApplication
        /// </summary>
        /// <param name="args">El argumento para construir la aplicación</param>
        /// <returns>La aplicación construida</returns>
        public async Task<WebApplication> Build(string[] args) 
        {
            // Cargar las variables de entorno
            Env.Load();
            // Crea el builder
            var builder = WebApplication.CreateBuilder(args);
            // Construye el servicio de Cloudinary
            var cloudinary = BuildCloudinary();
            // Agrega la API de cloudinary
            builder.Services.AddSingleton(cloudinary);
            // Añade los endpoints a la aplicación
            builder.Services.AddEndpointsApiExplorer();
            // Añade el DbContext a la aplicación
            builder.Services.AddDbContext<DataContext>(opt => opt.UseSqlite(Environment.GetEnvironmentVariable("DATABASE_URL")));
            // Para desarrollo, agrega el proceso de token de Swagger a la aplicación
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
            // Añade el seeder a la aplicación
            builder.Services.AddScoped<Seeder>();
            // Añade los controladores a la aplicación
            builder.Services.AddControllers();
            // Añade los repositorios a la aplicación
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IAuthRepository, AuthRepository>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IReceiptProductRepository, ReceiptProductRepository>();
            builder.Services.AddScoped<IReceiptRepository, ReceiptRepository>();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("allowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });
            // Añade Identity de Microsoft a la aplicación
            BuildIdentity(builder);
            // Construye la aplicación
            var app = builder.Build();
            // Configura la aplicación
            app.UseCors("allowAll");
            // Utiliza gestión por roles y se añade a la base de datos
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DataContext>();
                // Crea la base de datos si no existe
                context.Database.Migrate();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
                CreateRolesAsync(roleManager);
            }
            // LLena la base de datos con datos de prueba
            await Seed(app);
            return app;
        }
        /// <summary>
        /// Configura la autenticación y autorización de la aplicación con Identity de Microsoft.
        /// </summary>
        /// <param name="builder">Builder de la aplicación.</param>
        void BuildIdentity(WebApplicationBuilder builder)
        {
            // Añade Identity de Microsoft a la aplicación
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
            // Añade la autenticación con JWT
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
        /// <summary>
        /// Construye y retorna el servicio de Cloudinary con las configuraciones dadas por las variables de entorno.
        /// </summary>
        /// <returns>Una instancia ya configurada de Cloudinary.</returns>
        Cloudinary BuildCloudinary()
        {
            // Crea las configuraciones de Cloudinary
            var cloudinarySettings = new CloudinarySettings
            {
                CloudName = Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME")!,
                ApiKey = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY")!,
                ApiSecret = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET")!
            };
            // Crea la cuenta de Cloudinary
            var cloudinaryAccount = new Account(
                cloudinarySettings.CloudName,
                cloudinarySettings.ApiKey,
                cloudinarySettings.ApiSecret
            );
            // Retorna una instancia de Cloudinary
            var cloudinary = new Cloudinary(cloudinaryAccount);
            return cloudinary;
        }
        /// <summary>
        /// Crea los roles de la aplicacion.
        /// </summary>
        /// <param name="roleManager">El gestor de roles.</param>
        async void CreateRolesAsync(RoleManager<IdentityRole<int>> roleManager) 
        {
            await CreateRoles(roleManager);
        }
        /// <summary>
        /// Crea los roles de la aplicacion. Si el role ya existe, no hace nada.
        /// </summary>
        /// <param name="roleManager">El gestor de roles.</param>
        static async Task CreateRoles(RoleManager<IdentityRole<int>> roleManager)
        {
            // Crea los roles
            string[] roleNames = { "Admin", "User" };
            IdentityResult roleResult;
            // Itera sobre los roles
            foreach (var roleName in roleNames)
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                // Si el role no existe, lo crea
                if (!roleExists)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole<int> { Name = roleName });
                }
            }
        }
        /// <summary>
        /// Llena la base de datos con datos de prueba.
        /// </summary>
        /// <param name="app">La aplicación a llenar.</param>
        /// <remarks>
        /// Él metodo crea un Scope para obtener el servicio, obtiene el DataContext, migra la base de datos e inicializa el seeder,
        /// </remarks>
        async Task Seed(WebApplication app) 
        {
            // Crea el Scope para obtener los servicios
            using (var scope = app.Services.CreateScope())
            {
                // Obtiene los servicios
                var services = scope.ServiceProvider;
                // Obtiene el data context
                var context = services.GetRequiredService<DataContext>();
                // Migra la base de datos
                await context.Database.MigrateAsync();
                // Inicializa el Seeder.
                await Seeder.Initialize(services);
            }
        }
    }
}