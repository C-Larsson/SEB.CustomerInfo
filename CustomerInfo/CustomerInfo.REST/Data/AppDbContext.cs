using CustomerInfo.REST.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerInfo.REST.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          
            modelBuilder.Entity<Customer>().HasData(
                new Customer()
                {
                    SSN = "200001011001",
                    Email = "test1@gmail.com",
                    PhoneNumber = "+46720010001"
                }
            );
        }
    }
}
