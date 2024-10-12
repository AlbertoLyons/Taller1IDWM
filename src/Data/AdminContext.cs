using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Data
{
    public class AdminContext (DbContextOptions<AdminContext> options) : DbContext(options)
    {
        public DbSet<User> Admin { get; set; }
    }
}