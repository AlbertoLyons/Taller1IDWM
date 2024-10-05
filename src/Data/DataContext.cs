using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taller_1_IDWM.src.Data
{
    public class DataContext (DbContextOptions<DbContext> options) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
    }
}