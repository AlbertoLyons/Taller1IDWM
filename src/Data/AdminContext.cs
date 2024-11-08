using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Data
{
    public class AdminContext : IdentityDbContext<IdentityUser>
    {
        public AdminContext(DbContextOptions<AdminContext> options) : base(options) { }
        public DbSet<User> Admin { get; set; }
    }
}