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
            TransformPhoneIfNeeded(customer);
            _dbContext.Customers.Add(customer);
            return customer;
        }

        public Customer Update(Customer customer)
        {
            TransformPhoneIfNeeded(customer);
            var customerDB = _dbContext.Customers.Find(c => c.SSN == customer.SSN);

            // Only update if not null
            if (customer.Email != null) customerDB.Email = customer.Email; 
            if (customer.PhoneNumber != null) customerDB.PhoneNumber = customer.PhoneNumber; 
            
            return customerDB;
        }

        public bool Delete(string ssn)
        {
            return (_dbContext.Customers.RemoveAll(c => c.SSN == ssn) != 0);
        }

        private void TransformPhoneIfNeeded(Customer customer)
        {
            // Replace leading 0 with +46
            if (customer.PhoneNumber != null && customer.PhoneNumber.StartsWith("0"))
                customer.PhoneNumber = $"+46{customer.PhoneNumber.Remove(0, 1)}";
        }
    }

}

