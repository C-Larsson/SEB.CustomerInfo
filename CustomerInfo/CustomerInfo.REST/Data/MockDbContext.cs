using CustomerInfo.REST.Models;

namespace CustomerInfo.REST.Data
{
    public class MockDbContext
    {
        public List<Customer> Customers { get; set; }

        public MockDbContext()
        {
            Customers = new List<Customer>()
            {
                // Add some mock data
                new Customer()
                {
                    SSN = "197210161234",
                    Email = "test@gmail.com",
                    PhoneNumber = "+46701234567"
                }
            };
        }
    }
}
