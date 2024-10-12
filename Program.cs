using Taller_1_IDWM.src.Data;
using Microsoft.EntityFrameworkCore;
using Taller_1_IDWM.src.Data.Migrations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<DataContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("CustomersConnection")));
builder.Services.AddDbContext<AdminContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("AdminConnection")));
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<Seeder>();

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;   
    var context = services.GetRequiredService<DataContext>();
    await context.Database.MigrateAsync();
    Seeder.Initialize(services);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();


