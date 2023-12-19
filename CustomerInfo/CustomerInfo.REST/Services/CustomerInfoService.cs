using CustomerInfo.REST.Data;
using CustomerInfo.REST.Models;

namespace CustomerInfo.REST.Services
{
    public class CustomerInfoService : ICustomerInfoService
    {

        private readonly MockDbContext _dbContext;

        public CustomerInfoService(MockDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Customer? GetBySsn(string ssn)
        {
            return _dbContext.Customers.Find(c => c.SSN == ssn);
        }

        public Customer Create(Customer customer)
        {
            // Replace leading 0 with +46
            if (customer.PhoneNumber != null && customer.PhoneNumber.StartsWith("0"))
                customer.PhoneNumber = $"+46{customer.PhoneNumber.Remove(0, 1)}";

            _dbContext.Customers.Add(customer);
            return customer;
        }

        public Customer Update(Customer customer)
        {
            // Replace leading 0 with +46
            if (customer.PhoneNumber != null && customer.PhoneNumber.StartsWith("0"))
                customer.PhoneNumber = $"+46{customer.PhoneNumber.Remove(0, 1)}";

            var customerDB = _dbContext.Customers.Find(c => c.SSN == customer.SSN);
            customerDB.Email = customer.Email;
            customerDB.PhoneNumber = customer.PhoneNumber;
            return customerDB;
        }

        public bool Delete(string ssn)
        {
            var result = _dbContext.Customers.RemoveAll(c => c.SSN == ssn);
            
            // If no customer was removed, return false
            if (result == 0) 
                return false;
            
            return true;
        }
    }

}

