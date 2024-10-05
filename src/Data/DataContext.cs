using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Data
{
    public class DataContext (DbContextOptions<DbContext> options) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<userModel> Users { get; set; }
    }
}