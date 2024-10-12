using Bogus;
using Taller_1_IDWM.Migrations.Data;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Data
{
    public class Seeder
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var customersContext = services.GetRequiredService<DataContext>();
                var adminContext = services.GetRequiredService<AdminContext>();

                if (!adminContext.Admin.Any())
                {
                    adminContext.Admin.AddRange(
                        new User
                        {
                            Rut = "20416699-4",
                            Birthdate = new DateTime(2000, 10, 25),
                            Mail = "admin@idwm.cl",
                            Name = "Ignacio Mancilla",
                            Gender = "Masculino",
                            Password = "P4ssw0rd",
                        }
                    );
                    adminContext.SaveChanges();
                }

                if (!customersContext.Users.Any())
                {
                    List<string> genders = new List<string> { "Masculino", "Femenino", "Prefiero no decirlo", "Otro" };
                    Random random = new Random();
                    var userFaker = new Faker<User>()

                    .RuleFor(u => u.Rut, f => random.Next(1000000, 25000000).ToString() + "-" + random.Next(0, 9).ToString())
                    .RuleFor(u => u.Birthdate, f => f.Date.Past(18))
                    .RuleFor(u => u.Mail, f => f.Internet.Email())
                    .RuleFor(u => u.Name, f => f.Name.FullName())
                    .RuleFor(u => u.Gender, f => genders[random.Next(0, genders.Count)])
                    .RuleFor(u => u.Password, f => f.Internet.Password());

                    var users = userFaker.Generate(100);
                    customersContext.Users.AddRange(users);
                    customersContext.SaveChanges();
                }
                if (!customersContext.Products.Any()) 
                {
                    var productFaker = new Faker<Product>()
                    
                    .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                    .RuleFor(p => p.Type, f => f.Commerce.Categories(1)[0])
                    .RuleFor(p => p.Price, f => f.Random.Int(100, 10000))
                    .RuleFor(p => p.Stock, f => f.Random.Int(1, 1000))
                    .RuleFor(p => p.Image, f => "");

                    var products = productFaker.Generate(100);
                    customersContext.Products.AddRange(products);
                    customersContext.SaveChanges();
                }

                customersContext.SaveChanges();

                if (!customersContext.Receipts.Any()) 
                {
                    var receiptFaker = new Faker<Receipt>()
                    
                    .RuleFor(p => p.UserId, f => customersContext.Users.ToList()[f.Random.Int(0, customersContext.Users.Count() - 1)].Id)
                    .RuleFor(p => p.BoughtAt, f => f.Date.Past(18))
                    .RuleFor(p => p.TotalPrice, f => f.Random.Int(100, 10000))
                    .RuleFor(p => p.Country, f => f.Address.Country())
                    .RuleFor(p => p.City, f => f.Address.City())
                    .RuleFor(p => p.County, f => f.Address.County())
                    .RuleFor(p => p.Address, f => f.Address.StreetAddress());

                    var receipts = receiptFaker.Generate(100);
                    customersContext.Receipts.AddRange(receipts);
                    customersContext.SaveChanges();
                }

                customersContext.SaveChanges();

                if (!customersContext.ReceiptProducts.Any()) 
                {
                    var receiptProductsFaker = new Faker<ReceiptProduct>()
                    
                    .RuleFor(p => p.ReceiptId, f => customersContext.Receipts.ToList()[f.Random.Int(0, customersContext.Receipts.Count() - 1)].Id)
                    .RuleFor(p => p.ProductId, f => customersContext.Products.ToList()[f.Random.Int(0, customersContext.Products.Count() - 1)].ID)
                    .RuleFor(p => p.UnitPrice, f => f.Random.Int(100, 100000))
                    .RuleFor(p => p.Quantity, f => f.Random.Int(1, 100))
                    .RuleFor(p => p.TotalPrice, (f, p) => p.UnitPrice * p.Quantity);

                    var uniqueReceiptProducts = new HashSet<(int ReceiptId, int ProductId)>();

                    var receiptProducts = new List<ReceiptProduct>();

                    while (receiptProducts.Count < 100) 
                    {
                        var newReceiptProduct = receiptProductsFaker.Generate();
                        
                        // Verificar si ya existe la combinación de ReceiptId y ProductId en el HashSet
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