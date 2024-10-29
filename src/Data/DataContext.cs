using Microsoft.EntityFrameworkCore;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Data
{
    public class DataContext (DbContextOptions<DataContext> options) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<ReceiptProduct> ReceiptProducts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ReceiptProduct>().HasKey(rp => new { rp.ReceiptId, rp.ProductId });
        }
    }
}