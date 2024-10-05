using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OpenApi.Writers;
using Bogus;
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
                var context = services.GetRequiredService<DataContext>();

                if (!context.Users.Any())
                {
                    context.Users.AddRange(
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
                    context.Users.AddRange(users);
                    context.SaveChanges();
                }
                if (!context.Products.Any()) 
                {
                    var productFaker = new Faker<Product>()
                    
                    .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                    .RuleFor(p => p.Type, f => f.Commerce.Categories(1)[0])
                    .RuleFor(p => p.Price, f => f.Random.Int(100, 10000))
                    .RuleFor(p => p.Stock, f => f.Random.Int(1, 1000))
                    .RuleFor(p => p.Image, f => "");

                    var products = productFaker.Generate(100);
                    context.Products.AddRange(products);
                    context.SaveChanges();
                }

                context.SaveChanges();
            }
        }
    }
}