using CustomerInfo.REST.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerInfo.REST.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Customer> Customers { get; set; }

        // !!!Demo use only!!! to seed initial data
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().HasData(
                new Customer()
                {
                    SSN = "197210161234",
                    Email = "test@gmail.com",
                    PhoneNumber = "+46701234567"
                }
            );
        }
    }
}
