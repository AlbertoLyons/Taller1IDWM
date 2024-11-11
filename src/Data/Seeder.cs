using Bogus;
using Bogus.DataSets;
using Microsoft.AspNetCore.Identity;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Data
{
    public class Seeder
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var customersContext = services.GetRequiredService<DataContext>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
                var userManager = services.GetRequiredService<UserManager<User>>();

                string[] roleNames = { "Admin", "User" };
                foreach (var roleName in roleNames)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole<int> { Name = roleName });
                    }
                }

                if (!customersContext.Users.Any())
                {
                    
                    List<string> genders = new List<string> { "Masculino", "Femenino", "Prefiero no decirlo", "Otro" };
                    Random random = new Random();
                    var userFaker = new Faker<User>()
                        .RuleFor(u => u.Rut, f => random.Next(1000000, 25000000).ToString() + "-" + random.Next(0, 9).ToString())
                        .RuleFor(u => u.Birthdate, f => DateOnly.FromDateTime(f.Date.Past(18)))
                        .RuleFor(u => u.Email, f => f.Internet.Email())
                        .RuleFor(u => u.SecurityStamp, f => Guid.NewGuid().ToString())
                        .RuleFor(u => u.UserName, (f, u) => u.Email)
                        .RuleFor(u => u.Name, f => f.Name.FullName())
                        .RuleFor(u => u.Gender, f => genders[random.Next(0, genders.Count)]);

                    var users = userFaker.Generate(100);
                    foreach (var user in users)
                    {
                        var userResult = await userManager.CreateAsync(user, "DefaultP4ssw0rd");
                        if (userResult.Succeeded)
                        {
                            await userManager.AddToRoleAsync(user, "User");
                        }
                    }
                    var adminUser = new User
                    {
                        Rut = "20416699-4",
                        Birthdate = DateOnly.Parse("2000-10-25"),
                        Email = "admin@idwm.cl",
                        SecurityStamp = Guid.NewGuid().ToString(),
                        UserName = "admin@idwm.cl",
                        Name = "Ignacio Mancilla",
                        Gender = "Masculino",
                    };
                    var result = await userManager.CreateAsync(adminUser, "P4ssw0rd!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }
                customersContext.SaveChanges();
                if (!customersContext.Products.Any())
                {
                    var productFaker = new Faker<Product>()
                        .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                        .RuleFor(p => p.Type, f => f.Commerce.Categories(1)[0])
                        .RuleFor(p => p.Price, f => f.Random.Int(100, 10000))
                        .RuleFor(p => p.Stock, f => f.Random.Int(1, 1000))
                        .RuleFor(p => p.ImageUrl, f => "");

                    var products = productFaker.Generate(100);
                    customersContext.Products.AddRange(products);
                    customersContext.SaveChanges();
                }

                if (!customersContext.Receipts.Any())
                {
                    var users = customersContext.Users.ToList();
                    if (users.Count != 0)
                    {
                        var receiptFaker = new Faker<Receipt>()
                            .RuleFor(r => r.UserId, f => users[f.Random.Int(0, users.Count - 1)].Id)
                            .RuleFor(r => r.BoughtAt, f => f.Date.Past(18))
                            .RuleFor(r => r.TotalPrice, f => f.Random.Int(100, 10000))
                            .RuleFor(r => r.Country, f => f.Address.Country())
                            .RuleFor(r => r.City, f => f.Address.City())
                            .RuleFor(r => r.County, f => f.Address.County())
                            .RuleFor(r => r.Address, f => f.Address.StreetAddress());

                        var receipts = receiptFaker.Generate(100);
                        customersContext.Receipts.AddRange(receipts);
                        customersContext.SaveChanges();
                    }
                }

                if (!customersContext.ReceiptProducts.Any())
                {
                    var receiptProductsFaker = new Faker<ReceiptProduct>()
                        .RuleFor(r => r.ReceiptId, f => customersContext.Receipts.ToList()[f.Random.Int(0, customersContext.Receipts.Count() - 1)].Id)
                        .RuleFor(r => r.ProductId, f => customersContext.Products.ToList()[f.Random.Int(0, customersContext.Products.Count() - 1)].ID)
                        .RuleFor(r => r.UnitPrice, f => f.Random.Int(100, 100000))
                        .RuleFor(r => r.Quantity, f => f.Random.Int(1, 100))
                        .RuleFor(r => r.TotalPrice, (f, p) => p.UnitPrice * p.Quantity);

                    var uniqueReceiptProducts = new HashSet<(int ReceiptId, int ProductId)>();
                    var receiptProducts = new List<ReceiptProduct>();

                    while (receiptProducts.Count < 100)
                    {
                        var newReceiptProduct = receiptProductsFaker.Generate();
                        if (!uniqueReceiptProducts.Contains((newReceiptProduct.ReceiptId, newReceiptProduct.ProductId)))
                        {
                            uniqueReceiptProducts.Add((newReceiptProduct.ReceiptId, newReceiptProduct.ProductId));
                            receiptProducts.Add(newReceiptProduct);
                        }
                    }

                    customersContext.ReceiptProducts.AddRange(receiptProducts);
                    customersContext.SaveChanges();
                }
            }
        }
    }
}
